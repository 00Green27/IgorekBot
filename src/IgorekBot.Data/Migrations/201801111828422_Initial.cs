namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HiddenTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskNo = c.String(),
                        ProjectNo = c.String(),
                        UserProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        EmployeeNo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HiddenTasks", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.HiddenTasks", new[] { "UserProfile_Id" });
            DropTable("dbo.UserProfiles");
            DropTable("dbo.HiddenTasks");
        }
    }
}
