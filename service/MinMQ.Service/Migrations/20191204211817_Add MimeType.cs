using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
    public partial class AddMimeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("insert into \"tMimeTypes\" (\"Expression\", \"Added\", \"Changed\") values ('application/json', current_timestamp, current_timestamp), ('application/xml', current_timestamp, current_timestamp), ('text/xml', current_timestamp, current_timestamp)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
