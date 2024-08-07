Declare @CockFightBetKindName nvarchar(255) = N'Cockfight bet kind'
If NOT EXISTS (Select Top 1 1 From CockFightBetKinds Where Name = @CockFightBetKindName)
Begin
	INSERT INTO CockFightBetKinds (Name, Enabled, CreatedAt, CreatedBy) 
	VALUES 
		(@CockFightBetKindName, 1, GETDATE(), 0)
End