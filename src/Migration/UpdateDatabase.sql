SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[AnonnymousCodeInfos]

CREATE TABLE [dbo].[AnonnymousCodeInfos](
	[Id] [uniqueidentifier] NOT NULL,
	[AllowedRetries] int NOT NULL DEFAULT(0),
	[AuthorizedScopes] [nvarchar](max) NULL,
	[ClientId] [nvarchar](max) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NULL,
	[Description] [nvarchar](max) NULL,
	[ExpiresOnUtc] [datetime2](7) NOT NULL,
	[Lifetime] [int] NOT NULL, 
	[RequestedScopes] [nvarchar](max) NULL,
	[RetryCounter] int NULL DEFAULT(0),
	[ReturnUrl] [nvarchar](max) Not NULL,
	[Transport] [nvarchar](max) NOT NULL,
	[UserCode] [nvarchar](max) NOT NULL,
	[VerificationCode] [nvarchar](max) NOT NULL,
	[VerifiedOnUtc] [datetime2](7) NULL,
	CONSTRAINT [PK_AnonnymousCodeInfos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AnonnymousCodeInfos] ADD DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[AnonnymousCodeInfos] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedOnUtc]
GO
