namespace OOM.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Scope = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Class", t => t.ClassId)
                .Index(t => t.ClassId);
            
            CreateTable(
                "dbo.Class",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NamespaceId = c.Int(nullable: false),
                        Abstractness = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Namespace", t => t.NamespaceId)
                .Index(t => t.NamespaceId);
            
            CreateTable(
                "dbo.Method",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        Abstractness = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Scope = c.Int(nullable: false),
                        DefinitionType = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                        LineCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Class", t => t.ClassId)
                .Index(t => t.ClassId);
            
            CreateTable(
                "dbo.Namespace",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RevisionId = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Revision", t => t.RevisionId)
                .Index(t => t.RevisionId);
            
            CreateTable(
                "dbo.Revision",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        RID = c.String(nullable: false, maxLength: 50),
                        Message = c.String(maxLength: 500),
                        Author = c.String(maxLength: 250),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        RepositoryProtocol = c.Int(nullable: false),
                        URI = c.String(nullable: false, maxLength: 500),
                        User = c.String(maxLength: 250),
                        Password = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Revision", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Namespace", "RevisionId", "dbo.Revision");
            DropForeignKey("dbo.Class", "NamespaceId", "dbo.Namespace");
            DropForeignKey("dbo.Method", "ClassId", "dbo.Class");
            DropForeignKey("dbo.Attribute", "ClassId", "dbo.Class");
            DropIndex("dbo.Revision", new[] { "ProjectId" });
            DropIndex("dbo.Namespace", new[] { "RevisionId" });
            DropIndex("dbo.Method", new[] { "ClassId" });
            DropIndex("dbo.Class", new[] { "NamespaceId" });
            DropIndex("dbo.Attribute", new[] { "ClassId" });
            DropTable("dbo.Project");
            DropTable("dbo.Revision");
            DropTable("dbo.Namespace");
            DropTable("dbo.Method");
            DropTable("dbo.Class");
            DropTable("dbo.Attribute");
        }
    }
}
