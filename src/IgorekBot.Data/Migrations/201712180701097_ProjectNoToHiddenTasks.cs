namespace IgorekBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectNoToHiddenTasks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HiddenTasks", "ProjectNo", c => c.String(nullable: false, defaultValue: ""));

        }

        public override void Down()
        {
            DropColumn("dbo.HiddenTasks", "ProjectNo");
        }
    }
}
