--Mien Trung
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 27;	--	2D Đầu
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 28;	--	2D Đuôi
UPDATE BetKinds SET RegionId = 2, CategoryId = 10, ReplaceByIdWhenLive = 30 WHERE Id = 29;	--	2D 18Lô
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 30;	--	2D 18Lô Live
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 31;	--	2D 18Lô Đầu
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 32;	--	2D 7Lô
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 33;	--	3D Đầu
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 34;	--	3D Đuôi
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 35;	--	3D 17Lô
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 36;	--	3D 7Lô
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 37;	--	4D Đuôi
UPDATE BetKinds SET RegionId = 2, CategoryId = 10 WHERE Id = 38;	--	4D 16Lô
UPDATE BetKinds SET RegionId = 2, CategoryId = 10, OrderInCategory = 14 WHERE Id = 39;	--	Xiên 2
UPDATE BetKinds SET RegionId = 2, CategoryId = 10, OrderInCategory = 15 WHERE Id = 40;	--	Xiên 3
UPDATE BetKinds SET RegionId = 2, CategoryId = 10, OrderInCategory = 16 WHERE Id = 41;	--	Xiên 4

INSERT INTO BetKinds (
	Id, 
	Name, 
	RegionId, 
	CategoryId, 
	IsLive, 
	OrderInCategory, 
	Enabled, 
	Award, 
	CorrelationBetKindIds, 
	IsMixed) 
VALUES (
	1001, 
	N'Xiên', 
	2, 
	10, 
	0, 
	13,	
	1,
	0, 
	'39,40,41',
	1
);