USE [PoeSimCraft]
GO
/****** Object:  Table [dbo].[Affix]    Script Date: 5/16/2017 8:25:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Affix](
	[AffixId] [int] IDENTITY(1,1) NOT NULL,
	[Domain] [nchar](50) NULL,
	[GenerationType] [nchar](50) NULL,
	[Group] [nchar](200) NULL,
	[Name] [nchar](50) NULL,
	[ModName] [nchar](100) NULL,
	[ILvl] [int] NULL,
	[StatName1] [nchar](200) NULL,
	[StatMin1] [int] NULL,
	[StatMax1] [int] NULL,
	[StatName2] [nchar](200) NULL,
	[StatMin2] [int] NULL,
	[StatMax2] [int] NULL,
	[StatName3] [nchar](200) NULL,
	[StatMin3] [int] NULL,
	[StatMax3] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AffixId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
