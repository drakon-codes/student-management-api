IF OBJECT_ID(N'dbo.AdminUsers', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.AdminUsers
	(
		AdminUserId int IDENTITY(1,1) NOT NULL,
		Email nvarchar(256) NOT NULL,
		PasswordHash nvarchar(512) NOT NULL,
		CreatedAt datetime2(7) NOT NULL
			CONSTRAINT DF_AdminUsers_CreatedAt DEFAULT SYSUTCDATETIME(),
		CONSTRAINT PK_AdminUsers PRIMARY KEY (AdminUserId)
	);
END;
GO

IF NOT EXISTS
(
	SELECT 1
	FROM sys.indexes
	WHERE name = N'UX_AdminUsers_Email'
	  AND object_id = OBJECT_ID(N'dbo.AdminUsers')
)
BEGIN
	CREATE UNIQUE INDEX UX_AdminUsers_Email
		ON dbo.AdminUsers (Email);
END;
GO
