# Roanoke Project - Development Guide

## Project Overview
Roanoke is an open-world action-adventure game with a dual-protagonist narrative (settler and Native American) and a player-driven economy. The marketplace uses Roanoke Coin (RC) as its primary currency.

## Running the Application
```bash
# Install dependencies
npm install

# Start development server
npm start

# Build for production
npm run build
```

## Core Components

### Marketplace
The marketplace is the central trading hub where players can:
- Bid on items of varying rarities
- Purchase items directly with Buy Now
- Filter and sort items by rarity, price, and auction end time

### Economy Design
- RC monetary model: 100 RC = $10.00
- Cash-out fees vary by KYC level
- Marketplace fees: 1% listing fee (burned) + 9% final-value fee (to dev)
- Account purchase cap: 25,000 RC/day

### Rarity System
Item rarities follow this distribution:
- Common: 68%
- Uncommon: 26%
- Rare: 4%
- Legendary: 1.5%
- Mythic: 0.40%
- Relic: 0.08%
- Epoch: 0.009%

## Project Architecture
- React frontend for marketplace
- Future integration with Unity for game client
- Node.js + TypeScript microservices for backend functionality
- PostgreSQL for transaction data, Redis for real-time queues

## Development Tasks
- Implement marketplace UI components
- Set up bidding and purchasing functionality
- Create RC balance tracking system
- Develop item rarity visualization
- Implement filtering and sorting of marketplace items