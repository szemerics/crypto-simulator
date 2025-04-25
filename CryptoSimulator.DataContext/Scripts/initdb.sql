-- CryptoCurrencies: 15 kriptovaluta
INSERT INTO CryptoCurrencies (Name, Symbol, CurrentPrice, isDeleted) VALUES
('Bitcoin', 'BTC', 75000.00, 0),
('Ethereum', 'ETH', 3500.00, 0),
('Binance Coin', 'BNB', 600.00, 0),
('Cardano', 'ADA', 1.50, 0),
('Solana', 'SOL', 200.00, 0),
('Ripple', 'XRP', 1.20, 0),
('Polkadot', 'DOT', 30.00, 0),
('Avalanche', 'AVAX', 50.00, 0),
('Polygon', 'MATIC', 2.50, 0),
('Chainlink', 'LINK', 25.00, 0),
('Cosmos', 'ATOM', 15.00, 0),
('Algorand', 'ALGO', 1.00, 0),
('Tezos', 'XTZ', 3.00, 0),
('Stellar', 'XLM', 0.50, 0),
('Dogecoin', 'DOGE', 0.30, 0);

-- Users: 10 felhasználó, első Admin (RoleId = 1), többi User (RoleId = 2), Username mező
INSERT INTO Users (Username, Email, PasswordHash, isDeleted, RoleId) VALUES
('admin', 'admin@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 1),
('jeansm', 'jeansm@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('alicej', 'alicej@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('bobw', 'bobw@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('clarab', 'clarab@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('davidl', 'davidl@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('emmad', 'emmad@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('frankm', 'frankm@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('gracet', 'gracet@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2),
('henryc', 'henryc@example.com', '$2a$11$F4sImHK6OuRdPksheGMR9ee5w4bpRvDzYkk.ztnNDt94lPPC.nbqe', 0, 2);

-- Wallets: 10 pénztárca, minden felhasználóhoz egy
INSERT INTO Wallets (UserId, Balance, isDeleted) VALUES
(1, 50000.00, 0),
(2, 24042.00, 0),
(3, 15000.00, 0),
(4, 30000.00, 0),
(5, 20000.00, 0),
(6, 45000.00, 0),
(7, 10000.00, 0),
(8, 28000.00, 0),
(9, 35000.00, 0),
(10, 18000.00, 0);

-- Transactions: 12 tranzakció, különböző felhasználók és kriptovaluták
INSERT INTO Transactions (UserId, CryptoCurrencyId, TransactionType, Quantity, Price, TransactionDate, isDeleted) VALUES
(1, 1, 0, 0.5, 75000.00, '2025-04-20 10:00:00', 0), -- Admin vásárol Bitcoin-t
(2, 4, 0, 100, 1.50, '2025-04-25 16:33:20', 0), -- Jean vásárol Cardano-t
(3, 2, 0, 2, 3500.00, '2025-04-22 14:20:00', 0), -- Alice vásárol Ethereum-ot
(4, 5, 0, 10, 200.00, '2025-04-23 09:15:00', 0), -- Bob vásárol Solana-t
(5, 6, 0, 200, 1.20, '2025-04-24 11:30:00', 0), -- Clara vásárol Ripple-t
(6, 3, 0, 5, 600.00, '2025-04-21 17:45:00', 0), -- David vásárol Binance Coin-t
(7, 7, 0, 50, 30.00, '2025-04-25 08:00:00', 0), -- Emma vásárol Polkadot-ot
(8, 8, 0, 20, 50.00, '2025-04-23 13:10:00', 0), -- Frank vásárol Avalanche-t
(9, 9, 0, 100, 2.50, '2025-04-24 15:25:00', 0), -- Grace vásárol Polygon-t
(10, 10, 0, 30, 25.00, '2025-04-22 12:00:00', 0), -- Henry vásárol Chainlink-et
(4, 5, 1, 5, 210.00, '2025-04-26 09:30:00', 0), -- Bob elad Solana-t
(7, 7, 1, 25, 32.00, '2025-04-26 10:00:00', 0); -- Emma elad Polkadot-ot

-- Portfolios: 10 portfólió, a Buy tranzakciók alapján, eladások figyelembevételével
INSERT INTO Portfolios (WalletId, CryptoCurrencyId, Quantity, PurchasePrice, isDeleted) VALUES
(1, 1, 0.5, 75000.00, 0), -- Admin Bitcoin portfóliója
(2, 4, 100, 1.50, 0), -- Jean Cardano portfóliója
(3, 2, 2, 3500.00, 0), -- Alice Ethereum portfóliója
(4, 5, 5, 200.00, 0), -- Bob Solana portfóliója (10 - 5 eladás)
(5, 6, 200, 1.20, 0), -- Clara Ripple portfóliója
(6, 3, 5, 600.00, 0), -- David Binance Coin portfóliója
(7, 7, 25, 30.00, 0), -- Emma Polkadot portfóliója (50 - 25 eladás)
(8, 8, 20, 50.00, 0), -- Frank Avalanche portfóliója
(9, 9, 100, 2.50, 0), -- Grace Polygon portfóliója
(10, 10, 30, 25.00, 0); -- Henry Chainlink portfóliója