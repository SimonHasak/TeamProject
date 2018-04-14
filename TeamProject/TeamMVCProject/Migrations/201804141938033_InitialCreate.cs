namespace TeamMVCProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PlayerTeam",
                c => new
                    {
                        PlayerID = c.Int(nullable: false),
                        TeamID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PlayerID, t.TeamID })
                .ForeignKey("dbo.Players", t => t.PlayerID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .Index(t => t.PlayerID)
                .Index(t => t.TeamID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerTeam", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.PlayerTeam", "PlayerID", "dbo.Players");
            DropIndex("dbo.PlayerTeam", new[] { "TeamID" });
            DropIndex("dbo.PlayerTeam", new[] { "PlayerID" });
            DropTable("dbo.PlayerTeam");
            DropTable("dbo.Teams");
            DropTable("dbo.Players");
        }
    }
}
