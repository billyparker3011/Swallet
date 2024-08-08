Declare @agentId bigint;
Set @agentId = (Select AgentId From Agents Where ParentId = 0 And RoleId = 0);

If NOT EXISTS (Select Top 1 1 From CockFightAgentPostionTakings Where AgentId = @agentId)
Begin
	Insert Into CockFightAgentPostionTakings (AgentId, BetKindId, PositionTaking, CreatedAt, CreatedBy) 
	Values 
		(@agentId, 1, 1, GETDATE(), 0)
End