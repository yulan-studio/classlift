using System;
using Core.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260813193802_AddProvincesToCities")]
public partial class AddProvincesToCities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "provinces",
            columns: table => new
            {
                ProvinceID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                CreatedBy = table.Column<int>(type: "int", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                UpdatedDate = table.Column<DateTime>(type: "timestamp", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_provinces", item => item.ProvinceID);
                table.ForeignKey(
                    name: "FK_provinces_users_CreatedBy",
                    column: item => item.CreatedBy,
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_provinces_users_UpdatedBy",
                    column: item => item.UpdatedBy,
                    principalTable: "users",
                    principalColumn: "Id");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AddColumn<int>(
            name: "ProvinceID",
            table: "cities",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_provinces_CreatedBy",
            table: "provinces",
            column: "CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_provinces_UpdatedBy",
            table: "provinces",
            column: "UpdatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_cities_ProvinceID",
            table: "cities",
            column: "ProvinceID");

        migrationBuilder.AddForeignKey(
            name: "FK_cities_provinces_ProvinceID",
            table: "cities",
            column: "ProvinceID",
            principalTable: "provinces",
            principalColumn: "ProvinceID",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_cities_provinces_ProvinceID",
            table: "cities");

        migrationBuilder.DropIndex(
            name: "IX_cities_ProvinceID",
            table: "cities");

        migrationBuilder.DropColumn(
            name: "ProvinceID",
            table: "cities");

        migrationBuilder.DropTable(name: "provinces");
    }
}
