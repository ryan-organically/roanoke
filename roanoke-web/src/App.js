import React, { useEffect, useState } from 'react';
import './App.css';
import MarketplaceList from './components/MarketplaceList';
import MarketplaceService from './services/MarketplaceService';
import LandingPage from './components/LandingPage';
import './components/LandingPage.css';

function App() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [rcBalance, setRcBalance] = useState(10000); // Mock RC balance
  const [showLanding, setShowLanding] = useState(true);

  useEffect(() => {
    // Load marketplace items
    MarketplaceService.getItems()
      .then(data => {
        setItems(data);
        setLoading(false);
      })
      .catch(err => {
        setError('Failed to load marketplace items');
        setLoading(false);
        console.error(err);
      });
  }, []);

  const handleBidSuccess = (result) => {
    // Update items after successful bid
    MarketplaceService.getItems()
      .then(data => {
        setItems(data);
        // Simulate removing RC from balance
        setRcBalance(prev => prev - result.transaction.bidAmount);
      });
  };

  const handleExploreClick = () => {
    setShowLanding(false);
  };

  if (showLanding) {
    return (
      <div onClick={handleExploreClick}>
        <LandingPage onExplore={handleExploreClick} />
      </div>
    );
  }

  return (
    <div className="App">
      <header className="App-header">
        <h1>Roanoke Marketplace</h1>
        <div className="user-balance">RC Balance: {rcBalance}</div>
      </header>
      
      <main>
        {loading ? (
          <div className="loading">Loading marketplace items...</div>
        ) : error ? (
          <div className="error">{error}</div>
        ) : (
          <MarketplaceList items={items} onBidSuccess={handleBidSuccess} />
        )}
      </main>
      
      <footer>
        <div className="footer-content">
          <p>Roanoke Game - Marketplace Proof of Concept</p>
        </div>
      </footer>
    </div>
  );
}

export default App;