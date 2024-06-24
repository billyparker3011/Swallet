﻿Declare @agentId bigint;
Set @agentId = (Select AgentId From Agents Where ParentId = 0 And RoleId = 0);
If NOT EXISTS (Select Top 1 1 From AgentOdds Where AgentId = @agentId)
Begin
	Insert Into AgentOdds (AgentId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, CreatedAt, CreatedBy) 
	Values 
		(@agentId, 1, 705, 705, 805, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 2, 21680, 21680, 22820, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 3, 21700, 21700, 22820, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 4, 560, 560, 960, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 5, 520, 520, 920, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 6, 450, 450, 850, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 7, 70000, 70000, 73000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 8, 80000, 80000, 83000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 9, 21700, 21700, 22540, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 10, 710, 710, 810, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 11, 710, 710, 810, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 12, 710, 710, 810, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 13, 710, 710, 810, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 14, 710, 710, 810, 1, 1000, 1000000, GETDATE(), 0),
	
		(@agentId, 15, 755, 755, 775, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 16, 755, 755, 775, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 17, 754, 754, 774, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 18, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 19, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 20, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 21, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 22, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 23, 754, 754, 774, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 24, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 25, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 26, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
	
		(@agentId, 27, 755, 755, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 28, 755, 755, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 29, 753, 753, 998, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 30, 755, 755, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 31, 755, 755, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 32, 755, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 33, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 34, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 35, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 36, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 37, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 38, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 39, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 40, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 41, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
	
		(@agentId, 42, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 43, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 44, 753, 753, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 45, 755, 755, 1002, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 46, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 47, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 48, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 49, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 50, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 51, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 52, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 53, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 54, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 55, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
	
		(@agentId, 56, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 57, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 58, 753, 753, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 59, 755, 755, 1002, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 60, 755, 755, 1010, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 61, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 62, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 63, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 64, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 65, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 66, 670, 670, 1000, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 67, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 68, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 69, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
	
		(@agentId, 70, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 71, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0),
		(@agentId, 72, 755, 755, 780, 1, 1000, 1000000, GETDATE(), 0);
End