namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConversationReference : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Cookies", "Id", "dbo.UserProfiles");
            DropIndex("dbo.Cookies", new[] { "Id" });
            CreateTable(
                "dbo.ConversationReferences",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        EncodedReference = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.Id)
                .Index(t => t.Id);
            
            DropTable("dbo.Cookies");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Cookies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ResumptionCookie = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.ConversationReferences", "Id", "dbo.UserProfiles");
            DropIndex("dbo.ConversationReferences", new[] { "Id" });
            DropTable("dbo.ConversationReferences");
            CreateIndex("dbo.Cookies", "Id");
            AddForeignKey("dbo.Cookies", "Id", "dbo.UserProfiles", "Id");
        }
    }
}
