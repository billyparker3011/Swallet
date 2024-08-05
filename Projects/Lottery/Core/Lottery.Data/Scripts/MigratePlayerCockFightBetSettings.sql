Declare @PlayerId bigint
Declare MigratePlayerCockFightBetSettings cursor for
Select p.PlayerId from Players p
Open MigratePlayerCockFightBetSettings

Fetch next from MigratePlayerCockFightBetSettings into @PlayerId

	while @@FETCH_STATUS = 0
	begin 
		If not exists (select top 1 1 from CockFightPlayerBetSettings where PlayerId = @PlayerId)
		begin
			Insert Into CockFightPlayerBetSettings (PlayerId, BetKindId, MainLimitAmountPerFight, DrawLimitAmountPerFight, LimitNumTicketPerFight, CreatedAt, CreatedBy) 
			Values 
				(@PlayerId, 1, 1000, 1000, 5, GETDATE(), 0)
		end

		Fetch next from MigratePlayerCockFightBetSettings into @PlayerId
	end
	close MigratePlayerCockFightBetSettings
	deallocate MigratePlayerCockFightBetSettings