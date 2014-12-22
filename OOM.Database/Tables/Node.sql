CREATE TABLE [dbo].[Node]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[ProjectId] INT NOT NULL, 
    [NodeType] INT NOT NULL, -- 1-File, 2-Directory, 3-Unknown
    [Name] NVARCHAR(250) NOT NULL, 
    [CreatedAt] DATETIME NOT NULL, 
    [DeletedAt] DATETIME NOT NULL, 
    CONSTRAINT [FK_Node_Project] FOREIGN KEY ([ProjectId]) REFERENCES [Project]([Id])
)
