Declare @agentId bigint;
Set @agentId = (Select AgentId From Agents Where ParentId = 0 And RoleId = 0);

If NOT EXISTS (Select Top 1 1 From CockFightAgentBetSettings Where AgentId = @agentId)
Begin
	Insert Into CockFightAgentBetSettings (AgentId, BetKindId, MainLimitAmountPerFight, DrawLimitAmountPerFight, LimitNumTicketPerFight, CreatedAt, CreatedBy) 
	Values 
		(@agentId, 1, 1000, 1000, 5, GETDATE(), 0)
End