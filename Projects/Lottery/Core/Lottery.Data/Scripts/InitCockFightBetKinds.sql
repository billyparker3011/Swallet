Declare @CockFightBetKindId int = 1
Declare @CockFightBetKindName nvarchar(255) = N'Cockfight bet kind'
If NOT EXISTS (Select Top 1 1 From CockFightBetKinds Where Id = @CockFightBetKindId)
Begin
	INSERT INTO CockFightBetKinds (Id, Name, Enabled) 
	VALUES 
		(@CockFightBetKindId, @CockFightBetKindName, 1)
End