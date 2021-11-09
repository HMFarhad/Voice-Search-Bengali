namespace VoiceRecognition.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AirportModelAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Airports",
                c => new
                    {
                        AirportID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AirportID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Airports");
        }
    }
}
