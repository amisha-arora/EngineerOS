using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace EngineerOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeChunks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    CodeMethodId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChunkIndex = table.Column<int>(type: "integer", nullable: false),
                    StartLine = table.Column<int>(type: "integer", nullable: false),
                    EndLine = table.Column<int>(type: "integer", nullable: false),
                    SymbolName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SymbolType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeChunks_repository_files_RepositoryFileId",
                        column: x => x.RepositoryFileId,
                        principalTable: "repository_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Embeddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentChunkId = table.Column<Guid>(type: "uuid", nullable: true),
                    CodeChunkId = table.Column<Guid>(type: "uuid", nullable: true),
                    Vector = table.Column<Vector>(type: "vector", nullable: false),
                    Model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dimensions = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Embeddings_CodeChunks_CodeChunkId",
                        column: x => x.CodeChunkId,
                        principalTable: "CodeChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Embeddings_DocumentChunks_DocumentChunkId",
                        column: x => x.DocumentChunkId,
                        principalTable: "DocumentChunks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeChunks_CodeClassId",
                table: "CodeChunks",
                column: "CodeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeChunks_CodeMethodId",
                table: "CodeChunks",
                column: "CodeMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeChunks_RepositoryFileId_ChunkIndex",
                table: "CodeChunks",
                columns: new[] { "RepositoryFileId", "ChunkIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_CodeChunkId",
                table: "Embeddings",
                column: "CodeChunkId");

            migrationBuilder.CreateIndex(
                name: "IX_Embeddings_DocumentChunkId",
                table: "Embeddings",
                column: "DocumentChunkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Embeddings");

            migrationBuilder.DropTable(
                name: "CodeChunks");
        }
    }
}
