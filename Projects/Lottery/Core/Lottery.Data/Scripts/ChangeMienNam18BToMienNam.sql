--Mien Nam
UPDATE BetKinds SET Id = Id + 1 where Id >= 45 AND Id < 1000;

UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 42;	--	2D Đầu
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 43;	--	2D Đuôi
UPDATE BetKinds SET RegionId = 3, CategoryId = 20, ReplaceByIdWhenLive = 45 WHERE Id = 44;	--	2D 18Lô
IF NOT EXISTS (SELECT 1 FROM BetKinds WHERE Id = 45)
BEGIN
	INSERT INTO BetKinds (
		Id, Name, RegionId, CategoryId, IsLive, OrderInCategory, Enabled, Award, CorrelationBetKindIds, IsMixed
	) 
	VALUES (
		45, 
		N'2D 18Lô Live',
		3,
		20,
		1,
		4,
		1,
		75000,
		NULL,
		0
	);
END
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 46;	--	2D 18Lô Đầu
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 47;	--	2D 7Lô
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 48;	--	3D Đầu
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 49;	--	3D Đuôi
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 50;	--	3D 17Lô
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 51;	--	3D 7Lô
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 52;	--	4D Đuôi
UPDATE BetKinds SET RegionId = 3, CategoryId = 20 WHERE Id = 53;	--	4D 16Lô
UPDATE BetKinds SET RegionId = 3, CategoryId = 20, OrderInCategory = 14 WHERE Id = 54;	--	Xiên 2
UPDATE BetKinds SET RegionId = 3, CategoryId = 20, OrderInCategory = 15 WHERE Id = 55;	--	Xiên 3
UPDATE BetKinds SET RegionId = 3, CategoryId = 20, OrderInCategory = 16 WHERE Id = 56;	--	Xiên 4

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
	1002, 
	N'Xiên', 
	3, 
	20, 
	0, 
	13,	
	1,
	0, 
	'54,55,56',
	1
);