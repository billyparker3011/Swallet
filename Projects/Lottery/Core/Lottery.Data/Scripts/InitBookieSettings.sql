Declare @Ga28SettingValue nvarchar(MAX) = N'{\"ApiAddress\":\"https://partner-api.dev.ga28.cc\",\"PrivateKey\":\"886d0e57-d8b4-4190-8521-f4e529cce660\",\"PartnerAccountId\":\"d551c780-f80e-4dfb-8df3-1b3546f4c3a7\",\"GameClientId\":\"6627f902-0149-434a-a25c-5625cd30e8b9\",\"AuthValue\":Token886d0e57-d8b4-4190-8521-f4e529cce660}'

INSERT INTO BookieSettings (BookieTypeId, SettingValue, CreatedAt, CreatedBy) 
VALUES 
	(0, @Ga28SettingValue, GETDATE(), 0)