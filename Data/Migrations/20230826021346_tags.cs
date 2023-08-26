using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CBD.Data.Migrations
{
    public partial class tags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BuildOld_BuildOldId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_BuildOld_BuildOldId",
                table: "Tag");

            migrationBuilder.DropTable(
                name: "BuildOld");

            migrationBuilder.DropIndex(
                name: "IX_Tag_BuildOldId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Comment_BuildOldId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "BuildOldId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "BuildOldId",
                table: "Comment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildOldId",
                table: "Tag",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildOldId",
                table: "Comment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BuildOld",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                    ReadyStatus = table.Column<int>(type: "integer", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOld", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOld_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildOld_Server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Server",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_BuildOldId",
                table: "Tag",
                column: "BuildOldId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_BuildOldId",
                table: "Comment",
                column: "BuildOldId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOld_AuthorId",
                table: "BuildOld",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOld_ServerId",
                table: "BuildOld",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BuildOld_BuildOldId",
                table: "Comment",
                column: "BuildOldId",
                principalTable: "BuildOld",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_BuildOld_BuildOldId",
                table: "Tag",
                column: "BuildOldId",
                principalTable: "BuildOld",
                principalColumn: "Id");
        }
    }
}
