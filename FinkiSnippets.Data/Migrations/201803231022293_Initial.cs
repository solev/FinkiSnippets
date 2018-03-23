namespace FinkiSnippets.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        timeElapsed = c.Int(nullable: false),
                        isCorrect = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        answered = c.Boolean(nullable: false),
                        SnippetID = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EventID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Events", t => t.EventID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Snippets", t => t.SnippetID, cascadeDelete: true)
                .Index(t => t.SnippetID)
                .Index(t => t.UserID)
                .Index(t => t.EventID);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EventSnippets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SnippetID = c.Int(nullable: false),
                        EventID = c.Int(nullable: false),
                        OrderNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Events", t => t.EventID, cascadeDelete: true)
                .ForeignKey("dbo.Snippets", t => t.SnippetID, cascadeDelete: true)
                .Index(t => t.SnippetID)
                .Index(t => t.EventID);
            
            CreateTable(
                "dbo.Snippets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Output = c.String(),
                        Question = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SnippetOperations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SnippetID = c.Int(nullable: false),
                        OperationID = c.Int(nullable: false),
                        Frequency = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Snippets", t => t.SnippetID, cascadeDelete: true)
                .Index(t => t.SnippetID);
            
            CreateTable(
                "dbo.UserEvents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        EventID = c.Int(nullable: false),
                        Finished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Events", t => t.EventID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID)
                .Index(t => t.EventID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Operator = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.TemporarySnippets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Output = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.GroupSnippets",
                c => new
                    {
                        Group_ID = c.Int(nullable: false),
                        Snippet_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_ID, t.Snippet_ID })
                .ForeignKey("dbo.Groups", t => t.Group_ID, cascadeDelete: true)
                .ForeignKey("dbo.Snippets", t => t.Snippet_ID, cascadeDelete: true)
                .Index(t => t.Group_ID)
                .Index(t => t.Snippet_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AnswerLogs", "SnippetID", "dbo.Snippets");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserEvents", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AnswerLogs", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserEvents", "EventID", "dbo.Events");
            DropForeignKey("dbo.SnippetOperations", "SnippetID", "dbo.Snippets");
            DropForeignKey("dbo.GroupSnippets", "Snippet_ID", "dbo.Snippets");
            DropForeignKey("dbo.GroupSnippets", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.EventSnippets", "SnippetID", "dbo.Snippets");
            DropForeignKey("dbo.EventSnippets", "EventID", "dbo.Events");
            DropForeignKey("dbo.AnswerLogs", "EventID", "dbo.Events");
            DropIndex("dbo.GroupSnippets", new[] { "Snippet_ID" });
            DropIndex("dbo.GroupSnippets", new[] { "Group_ID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.UserEvents", new[] { "EventID" });
            DropIndex("dbo.UserEvents", new[] { "UserID" });
            DropIndex("dbo.SnippetOperations", new[] { "SnippetID" });
            DropIndex("dbo.EventSnippets", new[] { "EventID" });
            DropIndex("dbo.EventSnippets", new[] { "SnippetID" });
            DropIndex("dbo.AnswerLogs", new[] { "EventID" });
            DropIndex("dbo.AnswerLogs", new[] { "UserID" });
            DropIndex("dbo.AnswerLogs", new[] { "SnippetID" });
            DropTable("dbo.GroupSnippets");
            DropTable("dbo.TemporarySnippets");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Operations");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserEvents");
            DropTable("dbo.SnippetOperations");
            DropTable("dbo.Groups");
            DropTable("dbo.Snippets");
            DropTable("dbo.EventSnippets");
            DropTable("dbo.Events");
            DropTable("dbo.AnswerLogs");
        }
    }
}
