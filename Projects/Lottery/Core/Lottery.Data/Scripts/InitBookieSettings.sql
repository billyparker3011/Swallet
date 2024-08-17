Declare @Ga28SettingValue nvarchar(MAX) = N'{"ApiAddress":"https://partner-api.dev.ga28.cc","PrivateKey":"886d0e57-d8b4-4190-8521-f4e529cce660","PartnerAccountId":"d551c780-f80e-4dfb-8df3-1b3546f4c3a7","GameClientId":"6627f902-0149-434a-a25c-5625cd30e8b9","AuthValue":"886d0e57-d8b4-4190-8521-f4e529cce660","ApplicationStaticToken":"uxJWWVY17fodLq7Tyq2w8HyTHCkovBjBtX9ErAftYkLqxW3BOltAmaoZVcbSurlu","ScanTicketTime":"2024-08-17T00:00:00Z"}'
Declare @Ga28BookieType int = 0

If NOT EXISTS (Select Top 1 1 From BookieSettings Where BookieTypeId = @Ga28BookieType)
Begin
	INSERT INTO BookieSettings (BookieTypeId, SettingValue, CreatedAt, CreatedBy) 
	VALUES 
		(@Ga28BookieType, @Ga28SettingValue, GETDATE(), 0)
End
ELSE
Begin
	UPDATE BookieSettings
    SET SettingValue = @Ga28SettingValue
    WHERE BookieTypeId = @Ga28BookieType;
End