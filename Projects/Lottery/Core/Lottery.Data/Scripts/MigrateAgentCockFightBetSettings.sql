Declare @AgentId bigint
Declare MigrateAgentCockFightBetSettings cursor for
Select a.AgentId from Agents a
Open MigrateAgentCockFightBetSettings

Fetch next from MigrateAgentCockFightBetSettings into @AgentId

	while @@FETCH_STATUS = 0
	begin 
		If not exists (select top 1 1 from CockFightAgentBetSettings where AgentId = @AgentId)
		begin
			Insert Into CockFightAgentBetSettings (AgentId, BetKindId, MainLimitAmountPerFight, DrawLimitAmountPerFight, LimitNumTicketPerFight, CreatedAt, CreatedBy) 
			Values 
				(@AgentId, 1, 1000, 1000, 5, GETDATE(), 0)
		end

		Fetch next from MigrateAgentCockFightBetSettings into @AgentId
	end
	close MigrateAgentCockFightBetSettings
	deallocate MigrateAgentCockFightBetSettings