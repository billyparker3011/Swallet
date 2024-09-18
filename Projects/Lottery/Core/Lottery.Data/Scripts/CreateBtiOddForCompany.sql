Declare @agentId bigint;
Set @agentId = (Select AgentId From Agents Where ParentId = 0 And RoleId = 0);

If NOT EXISTS (Select Top 1 1 From BtiAgentBetSettings Where AgentId = @agentId)
Begin
	Insert Into BtiAgentBetSettings (AgentId, BetKindId, MinBet, MaxBet, MaxWin, MaxLoss, CreatedAt, CreatedBy) 
	Values 
		(@agentId, 1, 10, 10000, 0, 0, GETDATE(), 0)
End