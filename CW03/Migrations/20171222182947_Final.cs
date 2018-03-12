using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CW03.Migrations
{
    public partial class Final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uri",
                table: "BookmarkEntity");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "BookmarkEntity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uri",
                table: "BookmarkEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "BookmarkEntity",
                nullable: true);
        }
    }
}
