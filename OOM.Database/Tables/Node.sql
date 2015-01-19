CREATE TABLE [dbo].[Node]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[RevisionId] INT NOT NULL, 
    [NodeType] INT NOT NULL, -- 1-File, 2-Directory, 3-Unknown
    [Name] NVARCHAR(250) NOT NULL,
	[Path] NVARCHAR(500) NOT NULL,
    CONSTRAINT [FK_Node_Revision] FOREIGN KEY ([RevisionId]) REFERENCES [Revision]([Id])
)
