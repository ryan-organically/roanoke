import React, { useState } from 'react';
import MarketplaceItem from './MarketplaceItem';

function MarketplaceList({ items, onBidSuccess }) {
  const [sortOption, setSortOption] = useState('price-asc');
  const [filterRarity, setFilterRarity] = useState('all');
  
  // Filter items based on rarity selection
  const filteredItems = filterRarity === 'all' 
    ? items 
    : items.filter(item => item.rarity.toLowerCase() === filterRarity.toLowerCase());
  
  // Sort filtered items based on sort option
  const sortedItems = [...filteredItems].sort((a, b) => {
    switch(sortOption) {
      case 'price-asc': return a.price - b.price;
      case 'price-desc': return b.price - a.price;
      case 'bid-asc': return a.currentBid - b.currentBid;
      case 'bid-desc': return b.currentBid - a.currentBid;
      case 'ending-soon': return a.endTime - b.endTime;
      case 'rarity-asc': 
        const rarityOrder = { common: 0, uncommon: 1, rare: 2, legendary: 3, mythic: 4, relic: 5, epoch: 6 };
        return rarityOrder[a.rarity.toLowerCase()] - rarityOrder[b.rarity.toLowerCase()];
      case 'rarity-desc': 
        const rarityOrderDesc = { common: 0, uncommon: 1, rare: 2, legendary: 3, mythic: 4, relic: 5, epoch: 6 };
        return rarityOrderDesc[b.rarity.toLowerCase()] - rarityOrderDesc[a.rarity.toLowerCase()];
      default: return 0;
    }
  });

  return (
    <div className="marketplace-container">
      <div className="marketplace-controls">
        <div className="sort-options">
          <label>Sort by: </label>
          <select value={sortOption} onChange={(e) => setSortOption(e.target.value)}>
            <option value="bid-asc">Current Bid: Low to High</option>
            <option value="bid-desc">Current Bid: High to Low</option>
            <option value="price-asc">Buy Now: Low to High</option>
            <option value="price-desc">Buy Now: High to Low</option>
            <option value="ending-soon">Ending Soon</option>
            <option value="rarity-asc">Rarity: Common to Epoch</option>
            <option value="rarity-desc">Rarity: Epoch to Common</option>
          </select>
        </div>
        
        <div className="filter-options">
          <label>Filter by rarity: </label>
          <select value={filterRarity} onChange={(e) => setFilterRarity(e.target.value)}>
            <option value="all">All Items</option>
            <option value="common">Common</option>
            <option value="uncommon">Uncommon</option>
            <option value="rare">Rare</option>
            <option value="legendary">Legendary</option>
            <option value="mythic">Mythic</option>
            <option value="relic">Relic</option>
            <option value="epoch">Epoch</option>
          </select>
        </div>
      </div>
      
      <div className="marketplace-items">
        {sortedItems.map((item) => (
          <MarketplaceItem key={item.id} item={item} onBidSuccess={onBidSuccess} />
        ))}
      </div>
    </div>
  );
}

export default MarketplaceList;