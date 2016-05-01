namespace EhrgoHealth.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AllergicMedications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedicationName = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllergicMedications", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AllergicMedications", new[] { "ApplicationUser_Id" });
            DropTable("dbo.AllergicMedications");
        }
    }
}
