USE [web]
GO

/****** Object:  Table [dbo].[usuario]    Script Date: 11/1/2023 1:22:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[usuario](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nchar](50) NOT NULL,
	[password] [nchar](151) NOT NULL,
	[Passwordhash] [varbinary](100) NULL,
	[passwordsalt] [varbinary](150) NULL,
	[refreshtoken] [varchar](100) NULL,
	[Tokencreated] [datetime] NULL,
	[Tokenexpires] [datetime] NULL,
	[rol] [nchar](10) NULL,
 CONSTRAINT [PK_usuario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

