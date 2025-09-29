using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentVectorStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document_Store",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VectorStoreId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document_Store", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_Store_FileId",
                table: "Document_Store",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Document_Store_VectorStoreId",
                table: "Document_Store",
                column: "VectorStoreId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document_Store");
        }
    }
}
