using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngineerOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisMetadataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "repository_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Path = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    ParentDirectory = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repository_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_repository_files_repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "code_classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Namespace = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Kind = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AccessModifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Modifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_classes_repository_files_RepositoryFileId",
                        column: x => x.RepositoryFileId,
                        principalTable: "repository_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "code_dependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepositoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceCodeClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceTypeName = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TargetCodeClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetTypeName = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DependencyType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_dependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_dependencies_code_classes_SourceCodeClassId",
                        column: x => x.SourceCodeClassId,
                        principalTable: "code_classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_code_dependencies_code_classes_TargetCodeClassId",
                        column: x => x.TargetCodeClassId,
                        principalTable: "code_classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_code_dependencies_repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "code_methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReturnType = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AccessModifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_methods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_methods_code_classes_CodeClassId",
                        column: x => x.CodeClassId,
                        principalTable: "code_classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "code_method_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_method_parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_code_method_parameters_code_methods_CodeMethodId",
                        column: x => x.CodeMethodId,
                        principalTable: "code_methods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_code_classes_RepositoryFileId",
                table: "code_classes",
                column: "RepositoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_code_dependencies_RepositoryId",
                table: "code_dependencies",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_code_dependencies_SourceCodeClassId",
                table: "code_dependencies",
                column: "SourceCodeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_code_dependencies_TargetCodeClassId",
                table: "code_dependencies",
                column: "TargetCodeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_code_method_parameters_CodeMethodId_Position",
                table: "code_method_parameters",
                columns: new[] { "CodeMethodId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_code_methods_CodeClassId",
                table: "code_methods",
                column: "CodeClassId");

            migrationBuilder.CreateIndex(
                name: "IX_repository_files_RepositoryId_Path",
                table: "repository_files",
                columns: new[] { "RepositoryId", "Path" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "code_dependencies");

            migrationBuilder.DropTable(
                name: "code_method_parameters");

            migrationBuilder.DropTable(
                name: "code_methods");

            migrationBuilder.DropTable(
                name: "code_classes");

            migrationBuilder.DropTable(
                name: "repository_files");
        }
    }
}
