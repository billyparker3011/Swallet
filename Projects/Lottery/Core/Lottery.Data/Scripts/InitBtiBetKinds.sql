Declare @BetKindId int = 1
Declare @BetKindName nvarchar(255) = N'Default'
If NOT EXISTS (Select Top 1 1 From BtiBetKinds Where Id = @BetKindId)
Begin
	INSERT INTO BtiBetKinds (Id, Name, Enabled, BranchId) 
	VALUES 
		(@BetKindId, @BetKindName, 1, 0)
End