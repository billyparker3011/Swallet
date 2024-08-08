Declare @AgentId bigint
Declare MigrateAgentCockFightPositionTakings cursor for
Select a.AgentId from Agents a
Open MigrateAgentCockFightPositionTakings

Fetch next from MigrateAgentCockFightPositionTakings into @AgentId

	while @@FETCH_STATUS = 0
	begin 
		If not exists (select top 1 1 from CockFightAgentPostionTakings where AgentId = @AgentId)
		begin
			Insert Into CockFightAgentPostionTakings (AgentId, BetKindId, PositionTaking, CreatedAt, CreatedBy) 
			Values 
				(@AgentId, 1, 1, GETDATE(), 0)
		end

		Fetch next from MigrateAgentCockFightPositionTakings into @AgentId
	end
	close MigrateAgentCockFightPositionTakings
	deallocate MigrateAgentCockFightPositionTakings