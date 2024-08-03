IF EXISTS(SELECT 1 FROM BetKinds WHERE Id = 1001)
BEGIN
	UPDATE BetKinds SET CategoryId = 11, CorrelationBetKindIds = '39,40,41' WHERE Id = 1001;
END
ELSE
BEGIN
	INSERT INTO BetKinds (Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Award, Enabled, CorrelationBetKindIds, IsMixed) 
	VALUES (1001, N'Xiên', 2, 10, 0, 13, 0, 1, '39,40,41', 1);
END

IF EXISTS(SELECT 1 FROM BetKinds WHERE Id = 1002)
BEGIN
	UPDATE BetKinds SET Name = N'Xiên 18A+B', RegionId = 2, CategoryId = 11, CorrelationBetKindIds = '57,58,59' WHERE Id = 1002;
END
ELSE
BEGIN
	INSERT INTO BetKinds (Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Award, Enabled, CorrelationBetKindIds, IsMixed) 
	VALUES (1002, N'Xiên 18A+B', 2, 11, 0, 1, 0, 1, '57,58,59', 1);
END

IF EXISTS(SELECT 1 FROM BetKinds WHERE Id = 1003)
BEGIN
	UPDATE BetKinds SET Name = N'Xiên', RegionId = 3, CategoryId = 20, CorrelationBetKindIds = '54,55,56' WHERE Id = 1003;
END
ELSE
BEGIN
	INSERT INTO BetKinds (Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Award, Enabled, CorrelationBetKindIds, IsMixed) 
	VALUES (1003, N'Xiên', 3, 20, 0, 1, 0, 1, '54,55,56', 1);
END

IF EXISTS(SELECT 1 FROM BetKinds WHERE Id = 1004)
BEGIN
	UPDATE BetKinds SET Name = N'Xiên 18A+B', RegionId = 3, CategoryId = 21, CorrelationBetKindIds = '60,61,62' WHERE Id = 1004;
END
ELSE
BEGIN
	INSERT INTO BetKinds (Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Award, Enabled, CorrelationBetKindIds, IsMixed) 
	VALUES (1004, N'Xiên 18A+B', 3, 21, 0, 1, 0, 1, '60,61,62', 1);
END