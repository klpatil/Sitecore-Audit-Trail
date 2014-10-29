-- =========================================
-- Create table template
-- =========================================
USE SCAuditTrail
GO

IF OBJECT_ID('dbo.Logs', 'U') IS NOT NULL
  DROP TABLE dbo.Logs
GO

CREATE TABLE [dbo].[Logs](
 [ID] [int] IDENTITY(1,1) NOT NULL,
 [Date] [datetime] NOT NULL,
 [Thread] [varchar](255) NOT NULL,
 [Level] [varchar](20) NOT NULL,
 [Logger] [varchar](255) NOT NULL,
 [Message] [varchar](4000) NOT NULL,
 [Exception] [varchar](2000) NULL,
 [SCUser] [varchar](255) NULL,
 [SCAction] [varchar](255) NULL,
 [SCItemPath] [varchar](255) NULL,
 [SCLanguage] [varchar](100) NULL,
 [SCVersion] [varchar](100) NULL,
 [SCItemId] [varchar](38) NULL,
 [SiteName] [varchar](255) NULL,
 [SCMisc] [varchar](255) NULL
) ON [PRIMARY]

GO
