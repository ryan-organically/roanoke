// Mock marketplace service for POC
const MOCK_ITEMS = [
  { id: 1, name: 'Colonial Musket', rarity: 'Uncommon', price: 250, currentBid: 255, bids: 3, endTime: Date.now() + 3600000, imageUrl: null },
  { id: 2, name: 'Tribal Medallion', rarity: 'Rare', price: 750, currentBid: 800, bids: 5, endTime: Date.now() + 7200000, imageUrl: null },
  { id: 3, name: 'Explorer\'s Compass', rarity: 'Common', price: 75, currentBid: 75, bids: 0, endTime: Date.now() + 1800000, imageUrl: null },
  { id: 4, name: 'Ornate Tobacco Pipe', rarity: 'Common', price: 120, currentBid: 125, bids: 2, endTime: Date.now() + 5400000, imageUrl: null },
  { id: 5, name: 'Wampum Belt', rarity: 'Legendary', price: 1800, currentBid: 2050, bids: 12, endTime: Date.now() + 10800000, imageUrl: null },
  { id: 6, name: 'Feathered Headdress', rarity: 'Mythic', price: 4500, currentBid: 5100, bids: 8, endTime: Date.now() + 14400000, imageUrl: null },
  { id: 7, name: 'Governor\'s Seal', rarity: 'Relic', price: 12000, currentBid: 12500, bids: 4, endTime: Date.now() + 18000000, imageUrl: null },
  { id: 8, name: 'Lost Colony Map', rarity: 'Epoch', price: 75000, currentBid: 78250, bids: 6, endTime: Date.now() + 86400000, imageUrl: null },
  { id: 9, name: 'Settler\'s Axe', rarity: 'Common', price: 90, currentBid: 90, bids: 0, endTime: Date.now() + 2700000, imageUrl: null },
  { id: 10, name: 'Native Bow', rarity: 'Uncommon', price: 350, currentBid: 375, bids: 3, endTime: Date.now() + 9000000, imageUrl: null },
];

export const MarketplaceService = {
  // Get all marketplace items
  getItems: () => {
    return new Promise((resolve) => {
      // Simulate API delay
      setTimeout(() => {
        resolve(MOCK_ITEMS);
      }, 300);
    });
  },
  
  // Get a specific item by ID
  getItemById: (id) => {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const item = MOCK_ITEMS.find(item => item.id === id);
        if (item) {
          resolve(item);
        } else {
          reject(new Error('Item not found'));
        }
      }, 150);
    });
  },
  
  // Place a bid on an item
  placeBid: (itemId, bidAmount) => {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const itemIndex = MOCK_ITEMS.findIndex(item => item.id === itemId);
        if (itemIndex === -1) {
          reject(new Error('Item not found'));
          return;
        }
        
        const item = MOCK_ITEMS[itemIndex];
        
        // Check if bid is valid (higher than current bid)
        if (bidAmount <= item.currentBid) {
          reject(new Error('Bid must be higher than current bid'));
          return;
        }
        
        // Update item with new bid
        MOCK_ITEMS[itemIndex] = {
          ...item,
          currentBid: bidAmount,
          bids: item.bids + 1
        };
        
        // Return successful bid info
        resolve({
          success: true,
          transaction: {
            itemId,
            itemName: item.name,
            bidAmount,
            timestamp: new Date().toISOString(),
            transactionId: `bid-${Math.random().toString(36).substr(2, 9)}`
          }
        });
      }, 500);
    });
  },
  
  // Buy item now (instant purchase)
  buyNow: (itemId) => {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const item = MOCK_ITEMS.find(item => item.id === itemId);
        if (!item) {
          reject(new Error('Item not found'));
          return;
        }
        
        // Mock transaction for instant purchase
        resolve({
          success: true,
          transaction: {
            itemId,
            itemName: item.name,
            purchasePrice: item.price,
            timestamp: new Date().toISOString(),
            transactionId: `purchase-${Math.random().toString(36).substr(2, 9)}`
          }
        });
      }, 500);
    });
  }
};

export default MarketplaceService;