namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HiddenTasks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HiddenTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskNo = c.String(),
                        UserProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id);
            
            AddColumn("dbo.UserProfiles", "EmployeeNo", c => c.String());
            DropColumn("dbo.UserProfiles", "EmployeeCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "EmployeeCode", c => c.String());
            DropForeignKey("dbo.HiddenTasks", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.HiddenTasks", new[] { "UserProfile_Id" });
            DropColumn("dbo.UserProfiles", "EmployeeNo");
            DropTable("dbo.HiddenTasks");
        }
    }
}
