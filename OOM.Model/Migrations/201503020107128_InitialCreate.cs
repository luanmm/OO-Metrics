namespace OOM.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "OOM.Field",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        FullyQualifiedIdentifier = c.String(nullable: false, maxLength: 250),
                        Encapsulation = c.Int(nullable: false),
                        Qualification = c.Int(nullable: false),
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
                        BaseClassId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 250),
                        FullyQualifiedIdentifier = c.String(nullable: false, maxLength: 250),
                        Encapsulation = c.Int(nullable: false),
                        Qualification = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("OOM.Class", t => t.BaseClassId)
                .ForeignKey("OOM.Namespace", t => t.NamespaceId)
                .Index(t => t.NamespaceId)
                .Index(t => t.BaseClassId);
            
            CreateTable(
                "OOM.Method",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        FullyQualifiedIdentifier = c.String(nullable: false, maxLength: 250),
                        Encapsulation = c.Int(nullable: false),
                        Qualification = c.Int(nullable: false),
                        LineCount = c.Int(nullable: false),
                        ExitPoints = c.Int(nullable: false),
                        Method_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("OOM.Class", t => t.ClassId)
                .ForeignKey("OOM.Method", t => t.Method_Id)
                .Index(t => t.ClassId)
                .Index(t => t.Method_Id);
            
            CreateTable(
                "OOM.Namespace",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RevisionId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        FullyQualifiedIdentifier = c.String(nullable: false, maxLength: 250),
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
                .ForeignKey("OOM.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "OOM.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        RepositoryProtocol = c.Int(nullable: false),
                        URI = c.String(nullable: false, maxLength: 500),
                        User = c.String(maxLength: 250),
                        Password = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "OOM.Metric",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Expression = c.String(nullable: false, maxLength: 500),
                        TargetType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "OOM.MethodField",
                c => new
                    {
                        MethodId = c.Int(nullable: false),
                        FieldId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MethodId, t.FieldId })
                .ForeignKey("OOM.Method", t => t.MethodId, cascadeDelete: true)
                .ForeignKey("OOM.Field", t => t.FieldId, cascadeDelete: true)
                .Index(t => t.MethodId)
                .Index(t => t.FieldId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("OOM.Field", "ClassId", "OOM.Class");
            DropForeignKey("OOM.Class", "NamespaceId", "OOM.Namespace");
            DropForeignKey("OOM.Namespace", "RevisionId", "OOM.Revision");
            DropForeignKey("OOM.Revision", "ProjectId", "OOM.Project");
            DropForeignKey("OOM.MethodField", "FieldId", "OOM.Field");
            DropForeignKey("OOM.MethodField", "MethodId", "OOM.Method");
            DropForeignKey("OOM.Method", "Method_Id", "OOM.Method");
            DropForeignKey("OOM.Method", "ClassId", "OOM.Class");
            DropForeignKey("OOM.Class", "BaseClassId", "OOM.Class");
            DropIndex("OOM.MethodField", new[] { "FieldId" });
            DropIndex("OOM.MethodField", new[] { "MethodId" });
            DropIndex("OOM.Revision", new[] { "ProjectId" });
            DropIndex("OOM.Namespace", new[] { "RevisionId" });
            DropIndex("OOM.Method", new[] { "Method_Id" });
            DropIndex("OOM.Method", new[] { "ClassId" });
            DropIndex("OOM.Class", new[] { "BaseClassId" });
            DropIndex("OOM.Class", new[] { "NamespaceId" });
            DropIndex("OOM.Field", new[] { "ClassId" });
            DropTable("OOM.MethodField");
            DropTable("OOM.Metric");
            DropTable("OOM.Project");
            DropTable("OOM.Revision");
            DropTable("OOM.Namespace");
            DropTable("OOM.Method");
            DropTable("OOM.Class");
            DropTable("OOM.Field");
        }
    }
}
