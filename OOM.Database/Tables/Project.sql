CREATE TABLE [dbo].[Project]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(150) NOT NULL, 
    [RepositoryProtocol] INT NOT NULL, -- 1-Git, 2-Mercurial, 3-Subversion
    [URI] NVARCHAR(500) NOT NULL,
    [User] NVARCHAR(250) NULL, 
    [Password] NVARCHAR(250) NULL
)
