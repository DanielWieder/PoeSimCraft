USE [PoeSimCraft]
GO
/****** Object:  Table [dbo].[SpawnTag]    Script Date: 5/16/2017 8:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpawnTag](
	[Name] [varchar](50) NULL,
	[SpawnTagId] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
GO
