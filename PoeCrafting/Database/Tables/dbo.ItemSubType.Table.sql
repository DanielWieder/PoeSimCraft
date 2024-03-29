USE [PoeSimCraft]
GO
/****** Object:  Table [dbo].[ItemSubType]    Script Date: 5/16/2017 8:25:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSubType](
	[Name] [varchar](50) NULL,
	[ItemTypeId] [int] NULL,
	[ItemSubtypeId] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemSubtypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
