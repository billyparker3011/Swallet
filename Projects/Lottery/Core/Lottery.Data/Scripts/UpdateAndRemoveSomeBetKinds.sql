--Mien Nam
DELETE FROM BetKinds WHERE Id >= 57 AND Id <= 70;

INSERT INTO BetKinds (Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Enabled, Award, CorrelationBetKindIds, IsMixed) 
VALUES
   (57, N'Xiên 2', 2, 11, 0, 1, 1, 560000, NULL, 0), 
   (58, N'Xiên 3', 2, 11, 0, 2, 1, 2800000, NULL, 0), 
   (59, N'Xiên 4', 2, 11, 0, 3, 1, 11000000, NULL, 0);
   
UPDATE BetKinds SET Id = 60, CategoryId = 21 WHERE Id = 71;
UPDATE BetKinds SET Id = 61, CategoryId = 21 WHERE Id = 72;
UPDATE BetKinds SET Id = 62, CategoryId = 21 WHERE Id = 73;