namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserIdAndName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "UserId", c => c.String());
            AddColumn("dbo.UserProfiles", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "UserName");
            DropColumn("dbo.UserProfiles", "UserId");
        }
    }
}
