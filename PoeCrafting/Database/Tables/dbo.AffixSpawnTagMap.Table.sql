USE [PoeSimCraft]
GO
/****** Object:  Table [dbo].[AffixSpawnTagMap]    Script Date: 5/16/2017 8:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AffixSpawnTagMap](
	[AffixId] [int] NULL,
	[SpawnTagId] [int] NULL,
	[Value] [bit] NULL,
	[Weight] [int] NULL
) ON [PRIMARY]
GO
