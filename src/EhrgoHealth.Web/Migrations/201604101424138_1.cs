namespace EhrgoHealth.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FhirPatientId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FhirPatientId");
        }
    }
}
