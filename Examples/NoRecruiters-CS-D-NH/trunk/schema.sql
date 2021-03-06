USE [master]
GO


CREATE LOGIN [developer_nh] WITH PASSWORD='devel1234OPER'
GO

/****** Object:  Database [norecruiters_demo_nh]    Script Date: 05/20/2009 11:31:33 ******/
CREATE DATABASE [norecruiters_demo_nh] ON  PRIMARY 
( NAME = N'norecruiters_demo_nh', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\norecruiters_demo_nh.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'norecruiters_demo_nh_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\norecruiters_demo_nh_log.ldf' , SIZE = 35712KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'norecruiters_demo_nh', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [norecruiters_demo_nh].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [norecruiters_demo_nh] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET ARITHABORT OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [norecruiters_demo_nh] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [norecruiters_demo_nh] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [norecruiters_demo_nh] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET  DISABLE_BROKER 
GO
ALTER DATABASE [norecruiters_demo_nh] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [norecruiters_demo_nh] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [norecruiters_demo_nh] SET  READ_WRITE 
GO
ALTER DATABASE [norecruiters_demo_nh] SET RECOVERY FULL 
GO
ALTER DATABASE [norecruiters_demo_nh] SET  MULTI_USER 
GO
ALTER DATABASE [norecruiters_demo_nh] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [norecruiters_demo_nh] SET DB_CHAINING OFF 

USE [norecruiters_demo_nh]
GO
/****** Object:  User [developer_nh]    Script Date: 05/20/2009 11:21:58 ******/
CREATE USER [developer_nh] FOR LOGIN [developer_nh] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[PostingContents]    Script Date: 05/20/2009 11:21:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PostingContents](
	[ContentsId] [uniqueidentifier] NOT NULL,
	[Contents] [varchar](max) NOT NULL,
 CONSTRAINT [PK_PostingContents] PRIMARY KEY CLUSTERED 
(
	[ContentsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WTTagSearch]    Script Date: 05/20/2009 11:21:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WTTagSearch](
	[SessionId] [int] NOT NULL,
	[NormalizedTag] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WTPostingSearch]    Script Date: 05/20/2009 11:21:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WTPostingSearch](
	[SessionId] [int] NOT NULL,
	[PostingId] [uniqueidentifier] NOT NULL,
	[Rank] [smallint] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTypes]    Script Date: 05/20/2009 11:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserTypes](
	[UserTypeId] [smallint] NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserTypes] PRIMARY KEY CLUSTERED 
(
	[UserTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Actions]    Script Date: 05/20/2009 11:21:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Actions](
	[ActionId] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05/20/2009 11:21:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[PostingId] [uniqueidentifier] NULL,
	[UserTypeId] [smallint] NULL,
	[Email] [varchar](50) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserActions]    Script Date: 05/20/2009 11:21:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserActions](
	[UserActionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ActionId] [tinyint] NOT NULL,
	[Comment] [varchar](50) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[PostingId] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PostingApplications]    Script Date: 05/20/2009 11:21:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PostingApplications](
	[PostingApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[TargetPostingId] [uniqueidentifier] NOT NULL,
	[SubmittedPostingId] [uniqueidentifier] NULL,
	[SubmittedOn] [datetime] NOT NULL,
	[SubmittedByUserId] [uniqueidentifier] NOT NULL,
	[Comment] [varchar](2000) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ContentTypes]    Script Date: 05/20/2009 11:21:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContentTypes](
	[ContentTypeId] [smallint] NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ContentTypes] PRIMARY KEY CLUSTERED 
(
	[ContentTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PostingTags]    Script Date: 05/20/2009 11:21:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PostingTags](
	[TagId] [int] IDENTITY(1000,1) NOT NULL,
	[PostingId] [uniqueidentifier] NOT NULL,
	[TagText] [varchar](50) NOT NULL,
	[SafeText] [varchar](50) NULL,
 CONSTRAINT [PK_PostingTags] PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 05/20/2009 11:21:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserRoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Postings]    Script Date: 05/20/2009 11:21:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Postings](
	[PostingId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedOn] [datetime] NOT NULL,
	[ContentTypeId] [smallint] NOT NULL,
	[Heading] [varchar](100) NOT NULL,
	[Views] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[Flagged] [bit] NOT NULL,
	[Published] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[ContentsId] [uniqueidentifier] NULL,
	[ShortName] [varchar](100) NULL,
	[ShortText] [varchar](500) NULL,
 CONSTRAINT [PK_Postings] PRIMARY KEY CLUSTERED 
(
	[PostingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  FullTextCatalog [contents]    Script Date: 05/20/2009 11:41:39 ******/
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
	CREATE FULLTEXT CATALOG [contents]
	IN PATH N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\FTData'
	WITH ACCENT_SENSITIVITY = ON
	AS DEFAULT
	AUTHORIZATION [dbo]

	CREATE FULLTEXT INDEX ON [dbo].PostingContents
	( Contents 
	   Language 1033 )
	   KEY INDEX PK_PostingContents
		  ON contents
		  WITH CHANGE_TRACKING OFF, NO POPULATION;
end
GO
/****** Object:  StoredProcedure [dbo].[i_search_tags]    Script Date: 05/20/2009 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[i_search_tags](@tag_list varchar(8000)) as
begin
	declare @pos int, @tag varchar(50)
	select @pos = charindex(',', @tag_list)

	while (@pos <> 0)
	begin
		select @tag = substring(@tag_list, 1, @pos-1)
		select @tag_list = substring(@tag_list, @pos+1, len(@tag_list))
		select @pos = charindex(',', @tag_list)

		insert into WTTagSearch (SessionId, NormalizedTag) values (@@SPID, ltrim(rtrim(@tag)))
	end
	
	-- by this point either the tag list is a single tag, or we've parsed everything else out
	insert into WTTagSearch (SessionId, NormalizedTag) values (@@SPID, ltrim(rtrim(@tag_list)))		
end
/****** Object:  StoredProcedure [dbo].[d_search_tags]    Script Date: 05/20/2009 12:00:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[d_search_tags] as
begin
	delete WTTagSearch where SessionId = @@SPID
end
/****** Object:  StoredProcedure [dbo].[s_postings_by_query]    Script Date: 05/20/2009 11:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_postings_by_query] (
		@content_type smallint, 
		@tag_list varchar(8000),
		@query varchar(8000),
		@precision int) as
begin
	if not @tag_list is null
	begin
		exec i_search_tags @tag_list

		declare @max_rank int
		select @max_rank = count(*) from WTTagSearch where SessionId = @@SPID		

		insert into WTPostingSearch (SessionId, PostingId, Rank) 
			select @@SPID, t.PostingId, count(*) as Cnt 
			from 
				PostingTags T 
				inner join WTTagSearch on SessionId = @@SPID and NormalizedTag = SafeText
				inner join Postings P on T.PostingId = P.PostingId
			where P.ContentTypeId = @content_type
			group by T.PostingId

		select 
			convert(varchar(36), p.PostingId) as PostingId,
			ShortText,
			CreatedOn,
			LastModifiedOn,
			convert(varchar(2), ContentTypeId) as ContentType,
			Heading,
			ShortName,
			Views,
			Deleted,
			Flagged,
			Published,
			Active,
			UserId,
			Rank - @max_rank as Rank,
			p.ContentsId
		from
			Postings p 
			inner join WTPostingSearch s on
				s.SessionId = @@SPID and s.PostingId = p.PostingId and Rank >= (@max_rank - @precision)
			inner join PostingContents pc on
				p.ContentsId = pc.ContentsId and (@query is null or contains(contents, @query))
		where
			published = 1
			and deleted = 0
			and p.ContentTypeId = @content_type
		order by Rank desc

		exec d_search_tags
		delete WTPostingSearch where SessionId = @@SPID
	end 
	else
		select 
			convert(varchar(36), p.PostingId) as PostingId,
			ShortText,
			CreatedOn,
			LastModifiedOn,
			convert(varchar(2), ContentTypeId) as ContentType,
			Heading,
			ShortName,
			Views,
			Deleted,
			Flagged,
			Published,
			Active,
			UserId,
			null as Rank,
			p.ContentsId
		from
			Postings p 
			inner join PostingContents pc on
				p.ContentsId = pc.ContentsId and (@query is null or contains(contents, @query))
		where
			published = 1
			and deleted = 0
			and p.ContentTypeId = @content_type
end
GO
GRANT EXECUTE ON [dbo].[s_postings_by_query] to PUBLIC
GO
/****** Object:  ForeignKey [FK_Postings_ContentTypes]    Script Date: 05/20/2009 11:21:37 ******/
ALTER TABLE [dbo].[Postings]  WITH CHECK ADD  CONSTRAINT [FK_Postings_ContentTypes] FOREIGN KEY([ContentTypeId])
REFERENCES [dbo].[ContentTypes] ([ContentTypeId])
GO
ALTER TABLE [dbo].[Postings] CHECK CONSTRAINT [FK_Postings_ContentTypes]
GO
/****** Object:  ForeignKey [FK_PostingTags_Postings]    Script Date: 05/20/2009 11:21:40 ******/
ALTER TABLE [dbo].[PostingTags]  WITH CHECK ADD  CONSTRAINT [FK_PostingTags_Postings] FOREIGN KEY([PostingId])
REFERENCES [dbo].[Postings] ([PostingId])
GO
ALTER TABLE [dbo].[PostingTags] CHECK CONSTRAINT [FK_PostingTags_Postings]
GO
/****** Object:  ForeignKey [FK_UserRoles_Users]    Script Date: 05/20/2009 11:21:47 ******/
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users]
GO
EXEC sp_addrolemember N'db_datareader', N'developer_nh'
GO
EXEC sp_addrolemember N'db_datawriter', N'developer_nh'
GO
