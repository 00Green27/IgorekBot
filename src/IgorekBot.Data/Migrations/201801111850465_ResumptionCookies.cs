namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResumptionCookies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConversationReferences",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ResumptionCookie = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConversationReferences", "Id", "dbo.UserProfiles");
            DropIndex("dbo.ConversationReferences", new[] { "Id" });
            DropTable("dbo.ConversationReferences");
        }
    }
}
