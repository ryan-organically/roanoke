import React, { useState } from 'react';
import MarketplaceService from '../services/MarketplaceService';

function MarketplaceItem({ item, onBidSuccess }) {
  const { id, name, rarity, price, currentBid, bids, endTime, imageUrl } = item;
  const [bidAmount, setBidAmount] = useState(currentBid + Math.ceil(currentBid * 0.05)); // Default 5% higher
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [showBidForm, setShowBidForm] = useState(false);
  
  // Format time remaining
  const getTimeRemaining = () => {
    const timeLeft = endTime - Date.now();
    if (timeLeft <= 0) return 'Auction ended';
    
    const hours = Math.floor(timeLeft / (1000 * 60 * 60));
    const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
    
    return `${hours}h ${minutes}m`;
  };
  
  // Get appropriate CSS class based on rarity
  const getRarityClass = () => {
    switch(rarity.toLowerCase()) {
      case 'common': return 'rarity-common';
      case 'uncommon': return 'rarity-uncommon';
      case 'rare': return 'rarity-rare';
      case 'legendary': return 'rarity-legendary';
      case 'mythic': return 'rarity-mythic';
      case 'relic': return 'rarity-relic';
      case 'epoch': return 'rarity-epoch';
      default: return '';
    }
  };
  
  // Handle bid submission
  const handleSubmitBid = async (e) => {
    e.preventDefault();
    if (isSubmitting) return;
    
    setIsSubmitting(true);
    setError(null);
    
    try {
      const result = await MarketplaceService.placeBid(id, bidAmount);
      setIsSubmitting(false);
      setShowBidForm(false);
      if (onBidSuccess) onBidSuccess(result);
    } catch (err) {
      setIsSubmitting(false);
      setError(err.message || 'Failed to place bid');
    }
  };
  
  // Handle buy now
  const handleBuyNow = async () => {
    if (isSubmitting) return;
    
    setIsSubmitting(true);
    setError(null);
    
    try {
      const result = await MarketplaceService.buyNow(id);
      setIsSubmitting(false);
      if (onBidSuccess) onBidSuccess(result);
    } catch (err) {
      setIsSubmitting(false);
      setError(err.message || 'Failed to purchase item');
    }
  };

  return (
    <div className={`marketplace-item ${getRarityClass()}`}>
      <div className="item-image">
        <img src={imageUrl || 'placeholder.png'} alt={name} />
        <div className="rarity-badge">{rarity}</div>
      </div>
      <div className="item-details">
        <h3 className="item-name">{name}</h3>
        <div className="bid-info">
          <div className="current-bid">
            <span className="bid-label">Current Bid:</span>
            <span className="bid-value">{currentBid} RC</span>
          </div>
          <div className="bid-meta">
            <span className="bid-count">{bids} bids</span>
            <span className="time-remaining">{getTimeRemaining()}</span>
          </div>
        </div>
        <div className="buy-now-price">
          <span className="buy-now-label">Buy Now:</span>
          <span className="buy-now-value">{price} RC</span>
        </div>
      </div>
      
      {error && <div className="bid-error">{error}</div>}
      
      {showBidForm ? (
        <form className="bid-form" onSubmit={handleSubmitBid}>
          <input
            type="number"
            min={currentBid + 1}
            value={bidAmount}
            onChange={(e) => setBidAmount(parseInt(e.target.value))}
            required
          />
          <div className="bid-buttons">
            <button type="submit" disabled={isSubmitting} className="place-bid-button">
              {isSubmitting ? 'Placing Bid...' : 'Confirm Bid'}
            </button>
            <button type="button" onClick={() => setShowBidForm(false)} className="cancel-bid-button">
              Cancel
            </button>
          </div>
        </form>
      ) : (
        <div className="item-actions">
          <button onClick={() => setShowBidForm(true)} disabled={isSubmitting} className="bid-button">
            Place Bid
          </button>
          <button onClick={handleBuyNow} disabled={isSubmitting} className="buy-now-button">
            Buy Now
          </button>
        </div>
      )}
    </div>
  );
}

export default MarketplaceItem;