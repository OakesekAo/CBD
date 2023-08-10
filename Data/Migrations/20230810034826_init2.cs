using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CBD.Data.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Build_AspNetUsers_AuthorId",
                table: "Build");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_AuthorId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Server_AspNetUsers_AuthorId",
                table: "Server");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_AuthorId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Build_AuthorId",
                table: "Build");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Tag",
                newName: "CBDUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_AuthorId",
                table: "Tag",
                newName: "IX_Tag_CBDUserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Server",
                newName: "CBDUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Server_AuthorId",
                table: "Server",
                newName: "IX_Server_CBDUserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Comment",
                newName: "CBDUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                newName: "IX_Comment_CBDUserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Build",
                newName: "RawJson");

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

            migrationBuilder.AddColumn<string>(
                name: "Alignment",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BuiltWithId",
                table: "Build",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CBDUserId",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassDisplay",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LastPower",
                table: "Build",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Build",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "GlobalName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "BuildOld",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadyStatus = table.Column<int>(type: "integer", nullable: false),
                    ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Builtwith",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    App = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Database = table.Column<string>(type: "text", nullable: false),
                    DatabaseVersion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builtwith", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enhancement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhancementName = table.Column<string>(type: "text", nullable: false),
                    Grade = table.Column<string>(type: "text", nullable: false),
                    IoLevel = table.Column<int>(type: "integer", nullable: false),
                    RelativeLevel = table.Column<string>(type: "text", nullable: false),
                    Obtained = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enhancement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Powerentry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PowerName = table.Column<string>(type: "text", nullable: false),
                    PowerNameDisplay = table.Column<string>(type: "text", nullable: false),
                    PowerSetType = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    StatInclude = table.Column<bool>(type: "boolean", nullable: false),
                    ProcInclude = table.Column<bool>(type: "boolean", nullable: false),
                    VariableValue = table.Column<int>(type: "integer", nullable: false),
                    InherentSlotsUsed = table.Column<int>(type: "integer", nullable: false),
                    BuildId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Powerentry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Powerentry_Build_BuildId",
                        column: x => x.BuildId,
                        principalTable: "Build",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PowerSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameDisplay = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    BuildId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerSets_Build_BuildId",
                        column: x => x.BuildId,
                        principalTable: "Build",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Slotentry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsInherent = table.Column<bool>(type: "boolean", nullable: false),
                    EnhancementId = table.Column<int>(type: "integer", nullable: false),
                    PowerentryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slotentry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slotentry_Enhancement_EnhancementId",
                        column: x => x.EnhancementId,
                        principalTable: "Enhancement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slotentry_Powerentry_PowerentryId",
                        column: x => x.PowerentryId,
                        principalTable: "Powerentry",
                        principalColumn: "Id");
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
                name: "IX_Build_BuiltWithId",
                table: "Build",
                column: "BuiltWithId");

            migrationBuilder.CreateIndex(
                name: "IX_Build_CBDUserId",
                table: "Build",
                column: "CBDUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOld_AuthorId",
                table: "BuildOld",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOld_ServerId",
                table: "BuildOld",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Powerentry_BuildId",
                table: "Powerentry",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerSets_BuildId",
                table: "PowerSets",
                column: "BuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Slotentry_EnhancementId",
                table: "Slotentry",
                column: "EnhancementId");

            migrationBuilder.CreateIndex(
                name: "IX_Slotentry_PowerentryId",
                table: "Slotentry",
                column: "PowerentryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Build_AspNetUsers_CBDUserId",
                table: "Build",
                column: "CBDUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Build_Builtwith_BuiltWithId",
                table: "Build",
                column: "BuiltWithId",
                principalTable: "Builtwith",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_CBDUserId",
                table: "Comment",
                column: "CBDUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BuildOld_BuildOldId",
                table: "Comment",
                column: "BuildOldId",
                principalTable: "BuildOld",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Server_AspNetUsers_CBDUserId",
                table: "Server",
                column: "CBDUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_CBDUserId",
                table: "Tag",
                column: "CBDUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_BuildOld_BuildOldId",
                table: "Tag",
                column: "BuildOldId",
                principalTable: "BuildOld",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Build_AspNetUsers_CBDUserId",
                table: "Build");

            migrationBuilder.DropForeignKey(
                name: "FK_Build_Builtwith_BuiltWithId",
                table: "Build");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_CBDUserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BuildOld_BuildOldId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Server_AspNetUsers_CBDUserId",
                table: "Server");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_CBDUserId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_BuildOld_BuildOldId",
                table: "Tag");

            migrationBuilder.DropTable(
                name: "BuildOld");

            migrationBuilder.DropTable(
                name: "Builtwith");

            migrationBuilder.DropTable(
                name: "PowerSets");

            migrationBuilder.DropTable(
                name: "Slotentry");

            migrationBuilder.DropTable(
                name: "Enhancement");

            migrationBuilder.DropTable(
                name: "Powerentry");

            migrationBuilder.DropIndex(
                name: "IX_Tag_BuildOldId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Comment_BuildOldId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Build_BuiltWithId",
                table: "Build");

            migrationBuilder.DropIndex(
                name: "IX_Build_CBDUserId",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "BuildOldId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "BuildOldId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Alignment",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "BuiltWithId",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "CBDUserId",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "ClassDisplay",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "LastPower",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Build");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Build");

            migrationBuilder.RenameColumn(
                name: "CBDUserId",
                table: "Tag",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_CBDUserId",
                table: "Tag",
                newName: "IX_Tag_AuthorId");

            migrationBuilder.RenameColumn(
                name: "CBDUserId",
                table: "Server",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Server_CBDUserId",
                table: "Server",
                newName: "IX_Server_AuthorId");

            migrationBuilder.RenameColumn(
                name: "CBDUserId",
                table: "Comment",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CBDUserId",
                table: "Comment",
                newName: "IX_Comment_AuthorId");

            migrationBuilder.RenameColumn(
                name: "RawJson",
                table: "Build",
                newName: "AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "GlobalName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Build_AuthorId",
                table: "Build",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Build_AspNetUsers_AuthorId",
                table: "Build",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_AuthorId",
                table: "Comment",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Server_AspNetUsers_AuthorId",
                table: "Server",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_AuthorId",
                table: "Tag",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
