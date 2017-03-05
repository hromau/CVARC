using System;
using System.Collections.Generic;
using CvarcWeb.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CvarcWeb.Migrations
{
    public partial class TournamentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tournaments",
                nullable: false,
                defaultValue: TournamentType.Olympic);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tournaments");
        }
    }
}
