/****** Object:  Table [dbo].[Member]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Member](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[account] [varchar](16) NOT NULL,
	[password] [varchar](32) NOT NULL,
	[loginIp] [varchar](32) NULL,
	[loginDtt] [datetime] NULL,
	[errorCount] [int] NULL,
	[frozen] [bit] NULL,
	[number] [varchar](16) NULL,
	[name] [varchar](16) NULL,
	[email] [varchar](32) NULL,
	[gold] [float] NULL,
	[integral] [float] NULL,
	[sex] [int] NULL,
	[phone] [varchar](13) NULL,
	[qq] [varchar](16) NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Member_account] ON [dbo].[Member] 
(
	[account] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Member_name] ON [dbo].[Member] 
(
	[name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Member_dtt] ON [dbo].[Member] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysAccount]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysAccount](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[parent] [varchar](32) NULL,
	[account] [varchar](16) NOT NULL,
	[password] [varchar](32) NOT NULL,
	[loginIp] [varchar](32) NULL,
	[loginDtt] [smalldatetime] NULL,
	[errorCount] [int] NULL,
	[frozen] [bit] NULL,
	[securityCode] [varchar](16) NULL,
 CONSTRAINT [PK_SysAccount] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SysAccount_dtt] ON [dbo].[SysAccount] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysRole_Competence]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysRole_Competence](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[role] [varchar](32) NOT NULL,
	[competence] [varchar](32) NOT NULL,
 CONSTRAINT [PK_Role_Competence] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Role_Competence_dtt] ON [dbo].[SysRole_Competence] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysRole]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysRole](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[account] [varchar](32) NULL,
	[parent] [varchar](32) NULL,
	[role] [varchar](32) NOT NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Role_dtt] ON [dbo].[SysRole] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysLogin]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysLogin](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[session] [varchar](64) NULL,
	[account] [varchar](64) NOT NULL,
	[category] [int] NULL,
	[ip] [varchar](32) NULL,
	[data] [nvarchar](max) NULL,
	[area] [varchar](64) NULL,
	[result] [int] NOT NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_Login] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Login_dtt] ON [dbo].[SysLogin] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysLog]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysLog](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[type] [int] NOT NULL,
	[session] [varchar](64) NULL,
	[account] [varchar](64) NULL,
	[member] [varchar](128) NOT NULL,
	[value] [nvarchar](max) NOT NULL,
	[result] [nvarchar](max) NULL,
	[time] [float] NULL,
	[ip] [varchar](32) NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_SysLog] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SysLog_account] ON [dbo].[SysLog] 
(
	[account] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SysLog_dtt] ON [dbo].[SysLog] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SysLog_member] ON [dbo].[SysLog] 
(
	[member] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SysLog_type] ON [dbo].[SysLog] 
(
	[type] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysConfig]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysConfig](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[type] [int] NOT NULL,
	[childType] [int] NULL,
	[name] [varchar](64) NOT NULL,
	[value] [nvarchar](max) NOT NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_SysConfig] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SysConfig_dtt] ON [dbo].[SysConfig] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysCompetence]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysCompetence](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[parent] [varchar](32) NULL,
	[competence] [varchar](1024) NOT NULL,
	[describe] [varchar](1024) NULL,
 CONSTRAINT [PK_Competence] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Competence_dtt] ON [dbo].[SysCompetence] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysAccount_Role]    Script Date: 10/14/2015 09:54:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysAccount_Role](
	[gid] [varchar](32) NOT NULL,
	[dtt] [datetime] NOT NULL,
	[hide] [bit] NOT NULL,
	[account] [varchar](32) NOT NULL,
	[role] [varchar](32) NOT NULL,
 CONSTRAINT [PK_SysAccount_Role] PRIMARY KEY CLUSTERED 
(
	[gid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_SysAccount_Role_dtt] ON [dbo].[SysAccount_Role] 
(
	[dtt] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF_Member_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_Member_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_Member_errorCount]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_errorCount]  DEFAULT ((0)) FOR [errorCount]
GO
/****** Object:  Default [DF_Member_gold]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_gold]  DEFAULT ((0)) FOR [gold]
GO
/****** Object:  Default [DF_Member_integral]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_integral]  DEFAULT ((0)) FOR [integral]
GO
/****** Object:  Default [DF_Member_frozen]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[Member] ADD  CONSTRAINT [DF_Member_frozen]  DEFAULT ((0)) FOR [frozen]
GO
/****** Object:  Default [DF_SysAccount_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysAccount] ADD  CONSTRAINT [DF_SysAccount_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_SysAccount_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysAccount] ADD  CONSTRAINT [DF_SysAccount_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_SysAccount_Role_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysAccount_Role] ADD  CONSTRAINT [DF_SysAccount_Role_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_SysAccount_Role_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysAccount_Role] ADD  CONSTRAINT [DF_SysAccount_Role_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_Competence_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysCompetence] ADD  CONSTRAINT [DF_Competence_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_Competence_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysCompetence] ADD  CONSTRAINT [DF_Competence_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_SysConfig_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysConfig] ADD  CONSTRAINT [DF_SysConfig_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_SysConfig_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysConfig] ADD  CONSTRAINT [DF_SysConfig_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_SysLog_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysLog] ADD  CONSTRAINT [DF_SysLog_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_SysLog_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysLog] ADD  CONSTRAINT [DF_SysLog_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_Login_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysLogin] ADD  CONSTRAINT [DF_Login_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_Login_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysLogin] ADD  CONSTRAINT [DF_Login_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_Role_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysRole] ADD  CONSTRAINT [DF_Role_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_Role_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysRole] ADD  CONSTRAINT [DF_Role_hide]  DEFAULT ((0)) FOR [hide]
GO
/****** Object:  Default [DF_Role_Competence_dtt]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysRole_Competence] ADD  CONSTRAINT [DF_Role_Competence_dtt]  DEFAULT (getdate()) FOR [dtt]
GO
/****** Object:  Default [DF_Role_Competence_hide]    Script Date: 10/14/2015 09:54:16 ******/
ALTER TABLE [dbo].[SysRole_Competence] ADD  CONSTRAINT [DF_Role_Competence_hide]  DEFAULT ((0)) FOR [hide]
GO
