USE [master]
GO

CREATE LOGIN [developer] WITH PASSWORD='devel1234OPER'
GO

/****** Object:  Database [norecruiters_demo]    Script Date: 05/20/2009 11:31:33 ******/
CREATE DATABASE [norecruiters_demo] ON  PRIMARY 
( NAME = N'norecruiters_demo', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\norecruiters_demo.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'norecruiters_demo_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\norecruiters_demo_log.ldf' , SIZE = 35712KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'norecruiters_demo', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [norecruiters_demo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [norecruiters_demo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [norecruiters_demo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [norecruiters_demo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [norecruiters_demo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [norecruiters_demo] SET ARITHABORT OFF 
GO
ALTER DATABASE [norecruiters_demo] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [norecruiters_demo] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [norecruiters_demo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [norecruiters_demo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [norecruiters_demo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [norecruiters_demo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [norecruiters_demo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [norecruiters_demo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [norecruiters_demo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [norecruiters_demo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [norecruiters_demo] SET  DISABLE_BROKER 
GO
ALTER DATABASE [norecruiters_demo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [norecruiters_demo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [norecruiters_demo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [norecruiters_demo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [norecruiters_demo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [norecruiters_demo] SET  READ_WRITE 
GO
ALTER DATABASE [norecruiters_demo] SET RECOVERY FULL 
GO
ALTER DATABASE [norecruiters_demo] SET  MULTI_USER 
GO
ALTER DATABASE [norecruiters_demo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [norecruiters_demo] SET DB_CHAINING OFF 

USE [norecruiters_demo]
GO
/****** Object:  User [developer]    Script Date: 05/20/2009 11:21:58 ******/
CREATE USER [developer] FOR LOGIN [developer] WITH DEFAULT_SCHEMA=[dbo]
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
/****** Object:  StoredProcedure [dbo].[s_user]    Script Date: 05/20/2009 12:17:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_user] (
			@UserName varchar(50) = null,
			@UserId varchar(50) = null)
as
begin
	select 
		UserId,
		UserName,
		Email,
		Password,
		FirstName,
		LastName,
		convert(varchar(36), PostingId) PostingId,
		convert(varchar(1), UserTypeId) UserTypeId
	from 
		Users
	where 
		UserName = @UserName
		or (@UserName is null and UserId = @UserId)
end
GO
/****** Object:  StoredProcedure [dbo].[s_user_role]    Script Date: 05/20/2009 12:17:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_user_role](@UserId uniqueidentifier)
as
begin
	select 'user' as [name]
end
GO
/****** Object:  StoredProcedure [dbo].[iu_user]    Script Date: 05/20/2009 12:17:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[iu_user] (
		@UserId uniqueidentifier,
		@UserName varchar(50),
		@Email varchar(50),
		@Password varchar(50),
		@FirstName varchar(50),
		@LastName varchar(50),
		@PostingId uniqueidentifier,
		@UserTypeId smallint)
as
begin
	update Users set
		UserId = @UserId,
		UserName = @UserName,
		Email = @Email,
		Password = @Password,
		FirstName = @FirstName,
		LastName = @LastName,
		PostingId = @PostingId,
		UserTypeId = @UserTypeId
	where 
		UserId = @UserId

	if @@rowcount = 0
		insert into Users
			(UserId,
			UserName,
			Email,
			Password,
			FirstName,
			LastName,
			PostingId,
			UserTypeId)
		values (
			@UserId,
			@UserName,
			@Email,
			@Password,
			@FirstName,
			@LastName,
			@PostingId,
			@UserTypeId
		)
end
GO
/****** Object:  StoredProcedure [dbo].[s_contents]    Script Date: 05/20/2009 12:17:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[s_contents] (@ContentsId uniqueidentifier) as
begin
	select ContentsId, Contents from PostingContents where ContentsId = @ContentsId
end
GO
/****** Object:  StoredProcedure [dbo].[iu_contents]    Script Date: 05/20/2009 12:17:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[iu_contents] (@ContentsId uniqueidentifier, @Contents varchar(max)) as
begin
	update PostingContents set Contents = @Contents where ContentsId = @ContentsId

	if @@rowcount = 0
		insert into PostingContents (ContentsId, Contents) values (@ContentsId, @Contents)
end
GO
/****** Object:  StoredProcedure [dbo].[iu_application]    Script Date: 05/20/2009 12:17:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[iu_application] (
	@SubmittedPostingId uniqueidentifier, 
	@PostingId uniqueidentifier, 
	@SubmittedOn datetime,
	@SubmittedByUserId uniqueidentifier,
	@Comment varchar(2000)) as
begin

	if not exists (select * from PostingApplications where SubmittedPostingId = @SubmittedPostingId and TargetPostingId = @PostingId)
	begin
		insert into PostingApplications (TargetPostingId, SubmittedPostingId, SubmittedOn, SubmittedByUserId, Comment) values
			(@PostingId, @SubmittedPostingId, @SubmittedOn, @SubmittedByUserid, @Comment)
	end
end
GO
/****** Object:  StoredProcedure [dbo].[s_application]    Script Date: 05/20/2009 12:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_application] (@PostingId uniqueidentifier) as
begin
	select 
		convert(varchar(36), SubmittedPostingId) as SubmittedPostingId, 
		SubmittedOn, 
		convert(varchar(36), SubmittedByUserId) as SubmittedByUserId,
		Comment
	from PostingApplications where TargetPostingId = @PostingId
end
GO
/****** Object:  StoredProcedure [dbo].[i_user_action]    Script Date: 05/20/2009 12:17:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[i_user_action] (
	@UserActionId uniqueidentifier,
	@UserId uniqueidentifier,
	@ActionId tinyint,
	@Comment varchar(50),
	@CreatedOn datetime,
	@PostingId uniqueidentifier) as
begin
	insert into UserActions (UserId, ActionId, Comment, CreatedOn, PostingId) 
		values (@UserId, @ActionId, @Comment, @CreatedOn, @PostingId)
end
GO
/****** Object:  StoredProcedure [dbo].[s_tag_list]    Script Date: 05/20/2009 12:17:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_tag_list] (@count int) as
begin
	set rowcount @count
	select TagText, SafeText, count(*) as Cnt from postingtags group by SafeText, TagText order by count(*) desc
	set rowcount 0
end
GO
/****** Object:  StoredProcedure [dbo].[s_tags]    Script Date: 05/20/2009 12:17:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_tags] (@PostingId uniqueidentifier) as
begin
	select TagId, TagText, SafeText from PostingTags where PostingId = @PostingId
end
GO
/****** Object:  StoredProcedure [dbo].[iu_tags]    Script Date: 05/20/2009 12:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[iu_tags] (@TagId int, @PostingId uniqueidentifier, @TagText varchar(50), @SafeText varchar(50)) as
begin

	if (not @TagId is null)
		update PostingTags set TagText = @TagText, SafeText = @SafeText where TagId = @TagId

	if ((@@rowcount = 0) or (@TagId is null))
	begin
		insert into PostingTags (PostingId, TagText, SafeText) values (@PostingId, @TagText, @SafeText)
		select scope_identity() as TagId
	end
end
GO
/****** Object:  StoredProcedure [dbo].[d_tags]    Script Date: 05/20/2009 12:17:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[d_tags] (@TagId int, @PostingId uniqueidentifier, @TagText varchar(50), @SafeText varchar(50)) as
begin
	delete from PostingTags where TagId = @TagId
end
GO
/****** Object:  StoredProcedure [dbo].[i_search_tags]    Script Date: 05/20/2009 12:17:17 ******/
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
GO
/****** Object:  StoredProcedure [dbo].[d_search_tags]    Script Date: 05/20/2009 12:17:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[d_search_tags] as
begin
	delete WTTagSearch where SessionId = @@SPID
end
GO
/****** Object:  StoredProcedure [dbo].[iu_posting]    Script Date: 05/20/2009 12:17:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[iu_posting] (
	@PostingId uniqueidentifier,
	@UserId uniqueidentifier,
	@CreatedOn datetime,
	@LastModifiedOn datetime,
	@ContentTypeId smallint,
	@Heading varchar(100),
	@ShortName varchar(100),
	@ShortText varchar(500),
	@Views int,
	@Deleted bit,
	@Flagged bit,
	@Published bit,
	@Active bit,
	@ContentsId uniqueidentifier)
AS
BEGIN
	update Postings set
		PostingId = @PostingId,
		UserId = @UserId,
		CreatedOn = @CreatedOn,
		LastModifiedOn = @LastModifiedOn,
		ContentTypeId = @ContentTypeId,
		Heading = @Heading,
		ShortName = @ShortName,
		ShortText = @ShortText,
		Views = @Views,
		Deleted = @Deleted,
		Flagged = @Flagged,
		Published = @Published,
		Active = @Active,
		ContentsId = @ContentsId
	where
		PostingId = @PostingId

	if @@rowcount = 0
		insert into Postings (
			PostingId,
			UserId,
			CreatedOn,
			LastModifiedOn,
			ContentTypeId,
			Heading,
			ShortName,
			ShortText,
			Views,
			Deleted,
			Flagged,
			Published,
			Active,
			ContentsId)
		values (
			@PostingId,
			@UserId,
			@CreatedOn,
			@LastModifiedOn,
			@ContentTypeId,
			@Heading,
			@ShortName,
			@ShortText,
			@Views,
			@Deleted,
			@Flagged,
			@Published,
			@Active,
			@ContentsId)

END
GO
/****** Object:  StoredProcedure [dbo].[s_posting]    Script Date: 05/20/2009 12:17:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[s_posting] (
	@PostingId uniqueidentifier,
	@ShortName varchar(100))
AS
BEGIN
	select
		PostingId,
		convert(varchar(36), UserId) as UserId,
		convert(varchar(36), ContentsId) as ContentsId,
		CreatedOn,
		LastModifiedOn,
		convert(varchar(1), ContentTypeId) as ContentTypeId,
		Heading,
		ShortName,
		ShortText,
		Views,
		Deleted,
		Flagged,
		Published,
		Active
	from Postings
	where 
		(@ShortName is null and PostingId = @PostingId) or
		(ShortName = @ShortName)
END
GO
/****** Object:  StoredProcedure [dbo].[s_postings_by_application]    Script Date: 05/20/2009 12:17:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_postings_by_application] (@posting_id uniqueidentifier) as
begin

	select top 100
		convert(varchar(36), p.PostingId) as PostingId,
		UserName,
		p.ShortText,
		p.CreatedOn,
		p.LastModifiedOn,
		convert(varchar(2), p.ContentTypeId) as ContentType,
		p.Heading,
		p.ShortName,
		p.Views,
		p.Deleted,
		p.Flagged,
		p.Published,
		p.Active,
		SubmittedOn,
		Comment
	from
		Postings p 
			inner join PostingApplications a on (p.PostingId = a.SubmittedPostingId)
			inner join Users u on p.UserId = u.UserId
	where
		a.TargetPostingId = @posting_id
		and published = 1
		and deleted = 0
end
GO
/****** Object:  StoredProcedure [dbo].[s_all_postings_by_application]    Script Date: 05/20/2009 12:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_all_postings_by_application] (@user_id uniqueidentifier) as
begin

	select top 100
		convert(varchar(36), p.PostingId) as PostingId,
		UserName,
		p.ShortText,
		p.CreatedOn,
		p.LastModifiedOn,
		convert(varchar(2), p.ContentTypeId) as ContentType,
		p.Heading,
		p.ShortName,
		p.Views,
		p.Deleted,
		p.Flagged,
		p.Published,
		p.Active,
		src.PostingId as SourcePostingId,
		src.ShortName as SourceShortName,
		src.Heading as SourceHeading,
		SubmittedOn,
		Comment
	from
		Postings src
			inner join PostingApplications a on (src.PostingId = a.TargetPostingId)
			inner join Postings p on (p.PostingId = a.SubmittedPostingId)
			inner join Users u on p.UserId = u.UserId
	where
		src.UserId = @user_id
		and p.Published = 1
		and p.Deleted = 0
	order by src.ShortName, SubmittedOn
end
GO
/****** Object:  StoredProcedure [dbo].[s_user_postings]    Script Date: 05/20/2009 12:17:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_user_postings] (@user_id uniqueidentifier, @published bit = null) as
begin
	declare @profile_posting uniqueidentifier
	select @profile_posting = PostingId from Users where UserId = @user_id

	select 
		convert(varchar(36), PostingId) as PostingId,
		CreatedOn,
		LastModifiedOn,
		convert(varchar(2), ContentTypeId) as ContentType,
		Heading,
		Views,
		Deleted,
		Flagged,
		Published,
		Active,
		ShortName,
		IsNull(a.cnt, 0) as ApplicantCount
	from
		Postings p left outer join
			(select TargetPostingId, count(*) cnt from PostingApplications group by TargetPostingId) a
			on p.PostingId = a.TargetPostingId
	where
		UserId = @user_id
		and (@published is null or Published = @published)
		and (@profile_posting is null or (PostingId <> @profile_posting))
end
GO
/****** Object:  StoredProcedure [dbo].[s_postings]    Script Date: 05/20/2009 12:17:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[s_postings] (@content_type smallint) as
begin
	select top 100
		convert(varchar(36), PostingId) as PostingId,
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
		Active
	from
		Postings p 
	where
		published = 1
		and deleted = 0
		and p.ContentTypeId = @content_type
end
GO
/****** Object:  StoredProcedure [dbo].[s_user_type]    Script Date: 05/20/2009 12:17:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[s_user_type] (@UserTypeId smallint) as
begin
	select UserTypeId, [Description] from UserTypes where UserTypeId = @UserTypeId
end
GO
/****** Object:  StoredProcedure [dbo].[s_action]    Script Date: 05/20/2009 12:17:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[s_action] (@ActionId tinyint) as
begin
	select ActionId, Description from Actions where ActionId = @ActionId
end
GO
/****** Object:  StoredProcedure [dbo].[s_content_type]    Script Date: 05/20/2009 12:17:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[s_content_type] (@ContentTypeId smallint) as
begin
	select ContentTypeId, [Description] from ContentTypes where ContentTypeId = @ContentTypeId
end
GO
/****** Object:  StoredProcedure [dbo].[s_postings_by_query]    Script Date: 05/20/2009 12:17:29 ******/
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
EXEC sp_addrolemember N'db_datareader', N'developer'
GO
EXEC sp_addrolemember N'db_datawriter', N'developer'
GO

GRANT EXECUTE ON dbo.d_search_tags TO developer
GO
GRANT EXECUTE ON dbo.d_tags TO developer
GO
GRANT EXECUTE ON dbo.i_search_tags TO developer
GO
GRANT EXECUTE ON dbo.i_user_action TO developer
GO
GRANT EXECUTE ON dbo.iu_application TO developer
GO
GRANT EXECUTE ON dbo.iu_contents TO developer
GO
GRANT EXECUTE ON dbo.iu_posting TO developer
GO
GRANT EXECUTE ON dbo.iu_tags TO developer
GO
GRANT EXECUTE ON dbo.iu_user TO developer
GO
GRANT EXECUTE ON dbo.s_action TO developer
GO
GRANT EXECUTE ON dbo.s_all_postings_by_application TO developer
GO
GRANT EXECUTE ON dbo.s_application TO developer
GO
GRANT EXECUTE ON dbo.s_content_type TO developer
GO
GRANT EXECUTE ON dbo.s_contents TO developer
GO
GRANT EXECUTE ON dbo.s_posting TO developer
GO
GRANT EXECUTE ON dbo.s_postings TO developer
GO
GRANT EXECUTE ON dbo.s_postings_by_application TO developer
GO
GRANT EXECUTE ON dbo.s_postings_by_query TO developer
GO
GRANT EXECUTE ON dbo.s_tag_list TO developer
GO
GRANT EXECUTE ON dbo.s_tags TO developer
GO
GRANT EXECUTE ON dbo.s_user TO developer
GO
GRANT EXECUTE ON dbo.s_user_postings TO developer
GO
GRANT EXECUTE ON dbo.s_user_role TO developer
GO
GRANT EXECUTE ON dbo.s_user_type TO developer
GO

