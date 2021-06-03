using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.Service.Migrations
{
    public partial class AddProductView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE VIEW public.\"ProductView\" as " +
                                 "select p.\"Id\"," +
                                 "p.\"BusinessId\"," +
                                 "p.\"SupplierId\"," +
                                 "p.\"Name\"," +
                                 "pp.\"Id\" as \"ProductPropertyId\"," +
                                 "pp.\"UoMId\"," +
                                 "pp.\"FolderId\"," +
                                 "pp.\"Color\"," +
                                 "pp.\"Size\"" +
                                 "from \"Products\" p " +
                                 "join \"ProductProperties\" pp on p.\"Id\" = pp.\"ProductId\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW public.\"ProductView\";");
        }
    }
}
