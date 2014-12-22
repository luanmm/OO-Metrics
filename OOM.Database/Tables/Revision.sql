CREATE TABLE [dbo].[Revision]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProjectId] INT NOT NULL, 
    [RID] NVARCHAR(50) NOT NULL, 
    [Message] NVARCHAR(500) NULL, 
    [Author] NVARCHAR(250) NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    CONSTRAINT [FK_Revision_Project] FOREIGN KEY ([ProjectId]) REFERENCES [Project]([Id])
)
