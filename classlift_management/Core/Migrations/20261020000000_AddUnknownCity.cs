using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Core.Contexts;

#nullable disable

namespace Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261020000000_AddUnknownCity")]
    public partial class AddUnknownCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO provinces (Name, CreatedBy, CreatedDate)
                SELECT 'Unknown', MIN(Id), UTC_TIMESTAMP()
                FROM users
                WHERE NOT EXISTS (
                    SELECT 1 FROM provinces WHERE LOWER(TRIM(Name)) = 'unknown'
                )
                HAVING MIN(Id) IS NOT NULL;");

            migrationBuilder.Sql(@"
                INSERT INTO cities (Name, ProvinceID, CreatedBy, CreatedDate)
                SELECT 'Unknown', p.ProvinceID, MIN(u.Id), UTC_TIMESTAMP()
                FROM users u
                INNER JOIN provinces p ON LOWER(TRIM(p.Name)) = 'unknown'
                WHERE NOT EXISTS (
                    SELECT 1 FROM cities WHERE LOWER(TRIM(Name)) = 'unknown'
                )
                GROUP BY p.ProvinceID;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM cities
                WHERE LOWER(TRIM(Name)) = 'unknown'
                  AND NOT EXISTS (SELECT 1 FROM coaches WHERE coaches.CityID = cities.CityID)
                  AND NOT EXISTS (SELECT 1 FROM children WHERE children.CityID = cities.CityID);");

            migrationBuilder.Sql(@"
                DELETE FROM provinces
                WHERE LOWER(TRIM(Name)) = 'unknown'
                  AND NOT EXISTS (SELECT 1 FROM cities WHERE cities.ProvinceID = provinces.ProvinceID);");
        }
    }
}
