Declare @password nvarchar(50) = '<ENCRYPTED_PASSWORD>';
Declare @companyRole int = 0;
Declare @supermasterRole int = 1;
Declare @masterRole int = 2;
Declare @agentRole int = 3;
Declare @companyId bigint;
Set @companyId = (Select AgentId From Agents Where ParentId = 0 And RoleId = @companyRole);
----Create Supermaster
Declare @supermasterUsername nvarchar(50) = 'UT';
Declare @supermasterId bigint;
If NOT EXISTS (Select Top 1 1 From Agents Where Username = @supermasterUsername)
Begin
	Insert Into Agents (ParentId, Username, Password, RoleId, State, Credit, Lock, Permissions, SupermasterId, MasterId, CreatedAt, CreatedBy) 
	Values (0, @supermasterUsername, @password, @supermasterRole, 0, 10000000000, 0, '[AV],[AU],[AFC],[R],[BL],[VL]', 0, 0, GETDATE(), @companyId);
End
Set @supermasterId = (Select AgentId From Agents Where Username = @supermasterUsername);
Insert Into AgentOdds (AgentId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, CreatedAt, CreatedBy) 
Select @supermasterId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, GETDATE(), @companyId From AgentOdds Where AgentId = @companyId;

Insert Into AgentPositionTakings (AgentId, BetKindId, PositionTaking, CreatedAt, CreatedBy) 
Select @supermasterId, BetKindId, PositionTaking, GETDATE(), @companyId From AgentPositionTakings Where AgentId = @companyId;
----Create Master
Declare @masterUsername nvarchar(50) = 'UT01';
Declare @masterId bigint;
If NOT EXISTS (Select Top 1 1 From Agents Where Username = @masterUsername)
Begin
	Insert Into Agents (ParentId, Username, Password, RoleId, State, Credit, Lock, Permissions, SupermasterId, MasterId, CreatedAt, CreatedBy) 
	Values (0, @masterUsername, @password, @masterRole, 0, 10000000000, 0, '[AV],[AU],[AFC],[R],[BL],[VL]', @supermasterId, 0, GETDATE(), @supermasterId);
End
Set @masterId = (Select AgentId From Agents Where Username = @masterUsername);
Insert Into AgentOdds (AgentId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, CreatedAt, CreatedBy) 
Select @masterId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, GETDATE(), @supermasterId From AgentOdds Where AgentId = @supermasterId;

Insert Into AgentPositionTakings (AgentId, BetKindId, PositionTaking, CreatedAt, CreatedBy) 
Select @masterId, BetKindId, PositionTaking, GETDATE(), @supermasterId From AgentPositionTakings Where AgentId = @supermasterId;
----Create Agent
Declare @agentUsername nvarchar(50) = 'UT0101';
Declare @agentId bigint;
If NOT EXISTS (Select Top 1 1 From Agents Where Username = @agentUsername)
Begin
	Insert Into Agents (ParentId, Username, Password, RoleId, State, Credit, MemberMaxCredit, Lock, Permissions, SupermasterId, MasterId, CreatedAt, CreatedBy) 
	Values (0, @agentUsername, @password, @agentRole, 0, 10000000000, 1000000000, 0, '[AV],[AU],[AFC],[R],[BL],[VL]', @supermasterId, @masterId, GETDATE(), @masterId);
End
Set @agentId = (Select AgentId From Agents Where Username = @agentUsername);
Insert Into AgentOdds (AgentId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, CreatedAt, CreatedBy) 
Select @agentId, BetKindId, Buy, MinBuy, MaxBuy, MinBet, MaxBet, MaxPerNumber, GETDATE(), @masterId From AgentOdds Where AgentId = @masterId;

Insert Into AgentPositionTakings (AgentId, BetKindId, PositionTaking, CreatedAt, CreatedBy) 
Select @agentId, BetKindId, PositionTaking, GETDATE(), @masterId From AgentPositionTakings Where AgentId = @masterId;
----Create Player
Declare @playerUsername nvarchar(50) = 'UT0101001';
Declare @playerId bigint;
If NOT EXISTS (Select Top 1 1 From Players Where Username = @playerUsername)
Begin
	Insert Into Players (AgentId, MasterId, SupermasterId, Username, Password, Credit, State, Lock, CreatedAt, CreatedBy) 
	Values (@agentId, @masterId, @supermasterId, @playerUsername, @password, 1000000000, 0, 0, GETDATE(), @agentId);
End
Set @playerId = (Select PlayerId From Players Where Username = @playerUsername);
Insert Into PlayerOdds (PlayerId, BetKindId, Buy, MinBet, MaxBet, MaxPerNumber, CreatedAt, CreatedBy) 
Select @playerId, BetKindId, Buy, MinBet, MaxBet, MaxPerNumber, GETDATE(), @agentId From AgentOdds Where AgentId = @agentId;