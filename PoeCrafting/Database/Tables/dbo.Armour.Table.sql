USE [PoeSimCraft]
GO
/****** Object:  Table [dbo].[Armour]    Script Date: 5/16/2017 8:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Armour](
	[ItemId] [int] NULL,
	[Armour] [int] NULL,
	[Evasion] [int] NULL,
	[EnergyShield] [int] NULL,
	[Str] [int] NULL,
	[Dex] [int] NULL,
	[Int] [int] NULL,
	[ArmourId] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ArmourId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
