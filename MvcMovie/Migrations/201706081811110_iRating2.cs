namespace MvcMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iRating2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "PosterURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "PosterURL");
        }
    }
}
