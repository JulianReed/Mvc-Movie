namespace MvcMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "iRating", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "iRating");
        }
    }
}
