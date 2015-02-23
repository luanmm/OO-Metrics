namespace OOM.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "OOM.Attribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Scope = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("OOM.Class", t => t.ClassId)
                .Index(t => t.ClassId);
            
            CreateTable(
                "OOM.Class",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NamespaceId = c.Int(nullable: false),
                        Abstractness = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("OOM.Namespace", t => t.NamespaceId)
                .Index(t => t.NamespaceId);
            
            CreateTable(
                "OOM.Method",
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
                .ForeignKey("OOM.Class", t => t.ClassId)
                .Index(t => t.ClassId);
            
            CreateTable(
                "OOM.Namespace",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RevisionId = c.Int(nullable: false),
                        Identifier = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("OOM.Revision", t => t.RevisionId)
                .Index(t => t.RevisionId);
            
            CreateTable(
                "OOM.Revision",
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
                .ForeignKey("OOM.Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "OOM.Project",
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
            
            CreateTable(
                "OOM.MethodAttribute",
                c => new
                    {
                        MethodId = c.Int(nullable: false),
                        AttributeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MethodId, t.AttributeId })
                .ForeignKey("OOM.Method", t => t.MethodId, cascadeDelete: true)
                .ForeignKey("OOM.Attribute", t => t.AttributeId, cascadeDelete: true)
                .Index(t => t.MethodId)
                .Index(t => t.AttributeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("OOM.Revision", "ProjectId", "OOM.Project");
            DropForeignKey("OOM.Namespace", "RevisionId", "OOM.Revision");
            DropForeignKey("OOM.Class", "NamespaceId", "OOM.Namespace");
            DropForeignKey("OOM.Method", "ClassId", "OOM.Class");
            DropForeignKey("OOM.MethodAttribute", "AttributeId", "OOM.Attribute");
            DropForeignKey("OOM.MethodAttribute", "MethodId", "OOM.Method");
            DropForeignKey("OOM.Attribute", "ClassId", "OOM.Class");
            DropIndex("OOM.MethodAttribute", new[] { "AttributeId" });
            DropIndex("OOM.MethodAttribute", new[] { "MethodId" });
            DropIndex("OOM.Revision", new[] { "ProjectId" });
            DropIndex("OOM.Namespace", new[] { "RevisionId" });
            DropIndex("OOM.Method", new[] { "ClassId" });
            DropIndex("OOM.Class", new[] { "NamespaceId" });
            DropIndex("OOM.Attribute", new[] { "ClassId" });
            DropTable("OOM.MethodAttribute");
            DropTable("OOM.Project");
            DropTable("OOM.Revision");
            DropTable("OOM.Namespace");
            DropTable("OOM.Method");
            DropTable("OOM.Class");
            DropTable("OOM.Attribute");
        }
    }
}
