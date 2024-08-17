﻿Declare @CASettingValue nvarchar(MAX) = N'{"Agent":"zgmpla","ApiURL":"https://sw2.apitestenv.net:8443","ContentType":"application/json","OperatorId":"4563104","AllbetApiKey":"EBf0iabQyqf/iN3hAXc19IwFj26FUo9EPqxWDgOVHO3YL7ZeNAH/RU0TvdJqUIL9M/4AHtXFFbbQwO8EEjzeCA==","PartnerApiKey":"7+zQ5fw0doV+Y3Wm0W5LmBMjonPd25TlNP4MpnJY/BaGVAgpqEsL633OzpiFFS/X84AMhKi++NfQN9Hh7pPDmA==","Suffix":"ogw"}'
Declare @CABookieType int = 1

If NOT EXISTS (Select Top 1 1 From BookieSettings Where BookieTypeId = @CABookieType)
Begin
	INSERT INTO BookieSettings (BookieTypeId, SettingValue, CreatedAt, CreatedBy) 
	VALUES 
		(@CABookieType, @CASettingValue, GETDATE(), 0)
End
ELSE
Begin
	UPDATE BookieSettings
    SET SettingValue = @CASettingValue
    WHERE BookieTypeId = @CABookieType;
End


If NOT EXISTS (Select Top 1 1 From CasinoBetKinds)
Begin
	INSERT INTO CasinoBetKinds (Id, Name, Code) 
	VALUES 
		(1, N'CA', '1')
End


If NOT EXISTS (Select Top 1 1 From [CasinoGameTypes])
BEGIN
SET IDENTITY_INSERT [dbo].[CasinoGameTypes] ON 
INSERT INTO [dbo].[CasinoGameTypes] ([Id], [Name], [TableCode], [GameCode], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [Caterory])
VALUES
(1, N'Normal Baccarat', N'B001', N'101', GETDATE(), 0, NULL, NULL, NULL),
(2, N'Quick Baccarat', N'Q001', N'103', GETDATE(), 0, NULL, NULL, NULL),
(3, N'Sicbo(HiLo)', N'S001', N'201', GETDATE(), 0, NULL, NULL, NULL),
(4, N'Dran Tiger', N'D001', N'301', GETDATE(), 0, NULL, NULL, NULL),
(5, N'Roulette', N'R001', N'401', GETDATE(), 0, NULL, NULL, NULL),
(6, N'Bull Bull', N'BB001', N'801', GETDATE(), 0, NULL, NULL, NULL),
(7, N'Win Three Cards / Three Pictures', N'W001', N'901', GETDATE(), 0, NULL, NULL, NULL),
(8, N'Classic Pok Deng / Two Sides Pok Deng', N'P001', N'501', GETDATE(), 0, NULL, NULL, NULL),
(9, N'Insurance Baccarat', N'IB001', N'110', GETDATE(), 0, NULL, NULL, NULL),
(10, N'Teen Patti 20-20', N'TP001', N'603', GETDATE(), 0, NULL, NULL, NULL),
(11, N'Andar Bahar', N'AB001', N'602', GETDATE(), 0, NULL, NULL, NULL),
(12, N'Ultimate Texas Hold', N'T001', N'702', GETDATE(), 0, NULL, NULL, NULL),
(13, N'Casino War', N'CW001', N'703', GETDATE(), 0, NULL, NULL, NULL),
(14, N'Fish Prawn Crab', N'F001', N'202', GETDATE(), 0, NULL, NULL, NULL),
(15, N'Infinite Blackjack', N'BJ001', N'704', GETDATE(), 0, NULL, NULL, NULL),
(16, N'VIP Baccarat', N'V901', N'111', GETDATE(), 0, NULL, NULL, NULL),
(17, N'See Card Baccarat', N'C001', N'104', GETDATE(), 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[CasinoGameTypes] OFF
End

If NOT EXISTS (Select Top 1 1 From [CasinoAgentHandicaps])
BEGIN
SET IDENTITY_INSERT [dbo].[CasinoAgentHandicaps] ON 
INSERT INTO [dbo].[CasinoAgentHandicaps] ([Id], [Name], [Type], [MinBet], [MaxBet], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy])
VALUES
(1, N'A', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(2, N'B', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(3, N'C', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(4, N'D', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(5, N'E', N'general', CAST(300.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(6, N'F', N'general', CAST(400.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(7, N'G', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(8, N'H', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(9, N'I', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(10, N'J', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(11, N'K', N'general', CAST(1.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(12, N'L', N'general', CAST(1.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(13, N'A1', N'general', CAST(40.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(14, N'B1', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(15, N'C1', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(16, N'D1', N'general', CAST(400.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(17, N'E1', N'general', CAST(600.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(18, N'F1', N'general', CAST(800.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(19, N'G1', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(20, N'H1', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(21, N'I1', N'general', CAST(4000.00 AS Decimal(18, 2)), CAST(400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(22, N'J1', N'general', CAST(4000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(23, N'K1', N'general', CAST(2.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(24, N'L1', N'general', CAST(2.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(25, N'A2', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(26, N'B2', N'general', CAST(250.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(27, N'C2', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(28, N'D2', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(29, N'E2', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(30, N'F2', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(31, N'G2', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(32, N'H2', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(33, N'I2', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(34, N'J2', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(2500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(35, N'K2', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(36, N'L2', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(37, N'A3', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(38, N'B3', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(39, N'C3', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(40, N'D3', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(41, N'E3', N'general', CAST(3000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(42, N'F3', N'general', CAST(4000.00 AS Decimal(18, 2)), CAST(400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(43, N'G3', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(44, N'H3', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(45, N'I3', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(46, N'J3', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(5000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(47, N'K3', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(48, N'L3', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(49, N'A4', N'general', CAST(220.00 AS Decimal(18, 2)), CAST(110000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(50, N'B4', N'general', CAST(550.00 AS Decimal(18, 2)), CAST(55000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(51, N'C4', N'general', CAST(1100.00 AS Decimal(18, 2)), CAST(110000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(52, N'D4', N'general', CAST(2200.00 AS Decimal(18, 2)), CAST(220000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(53, N'E4', N'general', CAST(3300.00 AS Decimal(18, 2)), CAST(330000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(54, N'F4', N'general', CAST(4400.00 AS Decimal(18, 2)), CAST(440000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(55, N'G4', N'general', CAST(5500.00 AS Decimal(18, 2)), CAST(550000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(56, N'H4', N'general', CAST(11000.00 AS Decimal(18, 2)), CAST(1100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(57, N'I4', N'general', CAST(22000.00 AS Decimal(18, 2)), CAST(2200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(58, N'J4', N'general', CAST(22000.00 AS Decimal(18, 2)), CAST(5500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(59, N'K4', N'general', CAST(11.00 AS Decimal(18, 2)), CAST(1100.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(60, N'L4', N'general', CAST(11.00 AS Decimal(18, 2)), CAST(110.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(61, N'A5', N'general', CAST(360.00 AS Decimal(18, 2)), CAST(180000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(62, N'B5', N'general', CAST(900.00 AS Decimal(18, 2)), CAST(90000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(63, N'C5', N'general', CAST(1800.00 AS Decimal(18, 2)), CAST(180000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(64, N'D5', N'general', CAST(3600.00 AS Decimal(18, 2)), CAST(360000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(65, N'E5', N'general', CAST(5400.00 AS Decimal(18, 2)), CAST(540000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(66, N'F5', N'general', CAST(7200.00 AS Decimal(18, 2)), CAST(720000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(67, N'G5', N'general', CAST(9000.00 AS Decimal(18, 2)), CAST(900000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(68, N'H5', N'general', CAST(18000.00 AS Decimal(18, 2)), CAST(1800000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(69, N'I5', N'general', CAST(36000.00 AS Decimal(18, 2)), CAST(3600000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(70, N'J5', N'general', CAST(36000.00 AS Decimal(18, 2)), CAST(9000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(71, N'K5', N'general', CAST(18.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(72, N'L5', N'general', CAST(18.00 AS Decimal(18, 2)), CAST(180.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(73, N'A6', N'general', CAST(3600.00 AS Decimal(18, 2)), CAST(1800000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(74, N'B6', N'general', CAST(9000.00 AS Decimal(18, 2)), CAST(900000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(75, N'C6', N'general', CAST(18000.00 AS Decimal(18, 2)), CAST(1800000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(76, N'D6', N'general', CAST(36000.00 AS Decimal(18, 2)), CAST(3600000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(77, N'E6', N'general', CAST(54000.00 AS Decimal(18, 2)), CAST(5400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(78, N'F6', N'general', CAST(72000.00 AS Decimal(18, 2)), CAST(7200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(79, N'G6', N'general', CAST(90000.00 AS Decimal(18, 2)), CAST(9000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(80, N'H6', N'general', CAST(180000.00 AS Decimal(18, 2)), CAST(18000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(81, N'I6', N'general', CAST(360000.00 AS Decimal(18, 2)), CAST(36000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(82, N'J6', N'general', CAST(360000.00 AS Decimal(18, 2)), CAST(90000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(83, N'K6', N'general', CAST(180.00 AS Decimal(18, 2)), CAST(18000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(84, N'L6', N'general', CAST(180.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(85, N'A7', N'general', CAST(40800.00 AS Decimal(18, 2)), CAST(20400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(86, N'B7', N'general', CAST(102000.00 AS Decimal(18, 2)), CAST(10200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(87, N'C7', N'general', CAST(204000.00 AS Decimal(18, 2)), CAST(20400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(88, N'D7', N'general', CAST(408000.00 AS Decimal(18, 2)), CAST(40800000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(89, N'E7', N'general', CAST(612000.00 AS Decimal(18, 2)), CAST(61200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(90, N'F7', N'general', CAST(816000.00 AS Decimal(18, 2)), CAST(81600000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(91, N'G7', N'general', CAST(1020000.00 AS Decimal(18, 2)), CAST(102000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(92, N'H7', N'general', CAST(2040000.00 AS Decimal(18, 2)), CAST(204000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(93, N'I7', N'general', CAST(4080000.00 AS Decimal(18, 2)), CAST(408000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(94, N'J7', N'general', CAST(4080000.00 AS Decimal(18, 2)), CAST(1020000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(95, N'K7', N'general', CAST(2040.00 AS Decimal(18, 2)), CAST(204000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(96, N'L7', N'general', CAST(2040.00 AS Decimal(18, 2)), CAST(20400.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(97, N'A8', N'general', CAST(68200.00 AS Decimal(18, 2)), CAST(34100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(98, N'B8', N'general', CAST(170500.00 AS Decimal(18, 2)), CAST(17050000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(99,  N'C8', N'general', CAST(341000.00 AS Decimal(18, 2)), CAST(34100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(100, N'D8', N'general', CAST(682000.00 AS Decimal(18, 2)), CAST(68200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(101, N'E8', N'general', CAST(1023000.00 AS Decimal(18, 2)), CAST(102300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(102, N'F8', N'general', CAST(1364000.00 AS Decimal(18, 2)), CAST(136400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(103, N'G8', N'general', CAST(1705000.00 AS Decimal(18, 2)), CAST(170500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(104, N'H8', N'general', CAST(3410000.00 AS Decimal(18, 2)), CAST(341000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(105, N'I8', N'general', CAST(6820000.00 AS Decimal(18, 2)), CAST(682000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(106, N'J8', N'general', CAST(6820000.00 AS Decimal(18, 2)), CAST(1705000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(107, N'K8', N'general', CAST(3410.00 AS Decimal(18, 2)), CAST(341000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(108, N'L8', N'general', CAST(3410.00 AS Decimal(18, 2)), CAST(34100.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(109, N'N1', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(110, N'N2', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(111, N'N3', N'general', CAST(50000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(112, N'N4', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(113, N'N5', N'general', CAST(80000.00 AS Decimal(18, 2)), CAST(8000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(114, N'N6', N'general', CAST(100000.00 AS Decimal(18, 2)), CAST(3000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(115, N'N7', N'general', CAST(500000.00 AS Decimal(18, 2)), CAST(10000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(116, N'N8', N'general', CAST(200000.00 AS Decimal(18, 2)), CAST(5000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(117, N'N10', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(118, N'N9', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(119, N'N11', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(120, N'N12', N'general', CAST(50000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(121, N'A9', N'general', CAST(1.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(122, N'A10', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(123, N'A11', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(124, N'A12', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(125, N'A13', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(126, N'A14', N'general', CAST(200000.00 AS Decimal(18, 2)), CAST(20000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(127, N'A15', N'general', CAST(400000.00 AS Decimal(18, 2)), CAST(40000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(128, N'SP1', N'general', CAST(8.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(129, N'SP2', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(130, N'SP3', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(131, N'SP9', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(132, N'SP4', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(133, N'SP6', N'general', CAST(80.00 AS Decimal(18, 2)), CAST(8000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(134, N'SP5', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(4000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(135, N'SP7', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(136, N'SP8', N'general', CAST(400.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(137, N'AB1', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(138, N'AB2', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(139, N'AB3', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(600.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(140, N'AB4', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(141, N'AB5', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(142, N'AB6', N'general', CAST(300.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(143, N'AB7', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(144, N'AB8', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(145, N'AB9', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(146, N'AB10', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(147, N'MB1', N'general', CAST(2.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(148, N'MB2', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(149, N'MB3', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(4000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(150, N'MB4', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(151, N'MB5', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(8000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(152, N'MB6', N'general', CAST(25.00 AS Decimal(18, 2)), CAST(7000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(153, N'MB8', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(154, N'MB28', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(8000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(155, N'MB9', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(156, N'MB27', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(3000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(157, N'MB7', N'general', CAST(25.00 AS Decimal(18, 2)), CAST(7500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(158, N'MB10', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(159, N'MB26', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(625000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(160, N'MB25', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(1500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(161, N'MB24', N'general', CAST(3000.00 AS Decimal(18, 2)), CAST(650000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(162, N'MB23', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(750000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(163, N'MB11', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(164, N'MB12', N'general', CAST(250.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(165, N'MB22', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(166, N'MB13', N'general', CAST(250.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(167, N'MB21', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(168, N'MB14', N'general', CAST(250.00 AS Decimal(18, 2)), CAST(65000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(169, N'MB20', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(170, N'MB15', N'general', CAST(300.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(171, N'MB19', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(172, N'MB16', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(125000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(173, N'MB18', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(174, N'MB17', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(175, N'MBS1', N'general', CAST(-0.02 AS Decimal(18, 2)), CAST(-0.01 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(176, N'AC1', N'general', CAST(25.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(177, N'AC2', N'general', CAST(40.00 AS Decimal(18, 2)), CAST(4000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(178, N'AC3', N'general', CAST(70.00 AS Decimal(18, 2)), CAST(7000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(179, N'AC4', N'general', CAST(250.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(180, N'AC5', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(181, N'AC6', N'general', CAST(150.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(182, N'AC7', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(183, N'AC8', N'general', CAST(25000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(184, N'AE1', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(185, N'AE2', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(186, N'AE3', N'general', CAST(25.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(187, N'AE4', N'general', CAST(25.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(188, N'AE5', N'general', CAST(15.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(189, N'AE6', N'general', CAST(15.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(190, N'AE7', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(191, N'AE8', N'general', CAST(70.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(192, N'AE9', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(193, N'AE10', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(194, N'AE11', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(195, N'AE12', N'general', CAST(120.00 AS Decimal(18, 2)), CAST(400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(196, N'AE13', N'general', CAST(120.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(197, N'AE14', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(650000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(198, N'AE15', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(160000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(199, N'AE16', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(12000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(200, N'AE17', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(3000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(201, N'AE18', N'general', CAST(350.00 AS Decimal(18, 2)), CAST(1200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(202, N'AE19', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(32000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(203, N'VA1', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(204, N'VA2', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(205, N'VA3', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(206, N'VA4', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(207, N'VA5', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(208, N'VA6', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(209, N'VA7', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(210, N'VB1', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(211, N'VB2', N'general', CAST(3000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(212, N'VB3', N'general', CAST(6000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(213, N'VB4', N'general', CAST(60000.00 AS Decimal(18, 2)), CAST(1500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(214, N'VB5', N'general', CAST(150000.00 AS Decimal(18, 2)), CAST(3000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(215, N'VB6', N'general', CAST(150000.00 AS Decimal(18, 2)), CAST(6000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(216, N'VB7', N'general', CAST(150000.00 AS Decimal(18, 2)), CAST(9000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(217, N'VC1', N'general', CAST(15.00 AS Decimal(18, 2)), CAST(750.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(218, N'VC2', N'general', CAST(30.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(219, N'VC3', N'general', CAST(60.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(220, N'VC4', N'general', CAST(600.00 AS Decimal(18, 2)), CAST(15000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(221, N'VC5', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(222, N'VC6', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(223, N'VC7', N'general', CAST(1500.00 AS Decimal(18, 2)), CAST(90000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(224, N'VD1', N'general', CAST(30.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(225, N'VD2', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(226, N'VD3', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(227, N'VD4', N'general', CAST(1000.00 AS Decimal(18, 2)), CAST(25000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(228, N'VD5', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(229, N'VD6', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(230, N'VD7', N'general', CAST(2500.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(231, N'VE1', N'general', CAST(2.00 AS Decimal(18, 2)), CAST(75.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(232, N'VE2', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(125.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(233, N'VE3', N'general', CAST(5.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(234, N'VE4', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(1500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(235, N'VE5', N'general', CAST(125.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(236, N'VE6', N'general', CAST(125.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(237, N'VE7', N'general', CAST(125.00 AS Decimal(18, 2)), CAST(7500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(238, N'VF1', N'general', CAST(50.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(239, N'VF2', N'general', CAST(100.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(240, N'VF3', N'general', CAST(200.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(241, N'VF4', N'general', CAST(2000.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(242, N'VF5', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(243, N'VF6', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(244, N'VF7', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(245, N'AF1', N'general', CAST(1000000.00 AS Decimal(18, 2)), CAST(1500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(246, N'AF2', N'general', CAST(500000.00 AS Decimal(18, 2)), CAST(1500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(247, N'JPA', N'general', CAST(500.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(248, N'JPB', N'general', CAST(5000.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(249, N'JPC', N'general', CAST(10000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(250, N'JPD', N'general', CAST(20000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(251, N'AG', N'general', CAST(40.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(252, N'AH', N'general', CAST(40.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(253, N'AI', N'general', CAST(20.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(254, N'BC', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(50000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(255, N'BD', N'general', CAST(10.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(256, N'VIP_O', N'vip', CAST(2000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(257, N'VIP_P', N'vip', CAST(10000.00 AS Decimal(18, 2)), CAST(500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(258, N'VIP_O1', N'vip', CAST(4000.00 AS Decimal(18, 2)), CAST(400000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(259, N'VIP_P1', N'vip', CAST(20000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(260, N'VIP_U', N'vip', CAST(10000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(261, N'VIP_V', N'vip', CAST(50000.00 AS Decimal(18, 2)), CAST(2500000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(262, N'VIP_S1', N'vip', CAST(20000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(263, N'VIP_T1', N'vip', CAST(100000.00 AS Decimal(18, 2)), CAST(5000000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(264, N'VIP_Q1', N'vip', CAST(22000.00 AS Decimal(18, 2)), CAST(2200000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL),
(265, N'VIP_Z2', N'vip', CAST(8000.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), GETDATE(), 0, NULL, NULL)
SET IDENTITY_INSERT [dbo].[CasinoAgentHandicaps] OFF
End