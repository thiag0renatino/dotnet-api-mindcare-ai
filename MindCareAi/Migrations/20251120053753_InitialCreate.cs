using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindCareAi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "RM556934");

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_ACOMPANHAMENTO_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_EMPRESA_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_ENCAMINHAMENTO_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_PROFISSIONAL_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_TRIAGEM_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateSequence<int>(
                name: "SEQ_USUARIO_GS",
                schema: "RM556934",
                startValue: 11L);

            migrationBuilder.CreateTable(
                name: "EMPRESA",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_EMPRESA_GS\".NEXTVAL"),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    PLANO_SAUDE = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("EMPRESA_PK", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PROFISSIONAL",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_PROFISSIONAL_GS\".NEXTVAL"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIALIDADE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    CONVENIO = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: true),
                    CONTATO = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PROFISSIONAL_PK", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO_SISTEMA",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_USUARIO_GS\".NEXTVAL"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SENHA = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMPRESA_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("USUARIO_PK", x => x.ID);
                    table.ForeignKey(
                        name: "USUARIO_EMPRESA_FK",
                        column: x => x.EMPRESA_ID,
                        principalSchema: "RM556934",
                        principalTable: "EMPRESA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRIAGEM",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_TRIAGEM_GS\".NEXTVAL"),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RELATO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RISCO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SUGESTAO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USUARIO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TRIAGEM_PK", x => x.ID);
                    table.ForeignKey(
                        name: "TRIAGEM_USUARIO_FK",
                        column: x => x.USUARIO_ID,
                        principalSchema: "RM556934",
                        principalTable: "USUARIO_SISTEMA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ENCAMINHAMENTO",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_ENCAMINHAMENTO_GS\".NEXTVAL"),
                    TIPO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EXAME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false, defaultValue: "N/A"),
                    ESPECIALIDADE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false, defaultValue: "N/A"),
                    PRIORIDADE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false, defaultValue: "MEDIA"),
                    STATUS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false, defaultValue: "PENDENTE"),
                    OBSERVACAO = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false, defaultValue: "N/A"),
                    TRIAGEM_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PROFISSIONAL_ID = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ENCAMINHAMENTO_PK", x => x.ID);
                    table.ForeignKey(
                        name: "ENCAMINHAMENTO_PROFISSIONAL_FK",
                        column: x => x.PROFISSIONAL_ID,
                        principalSchema: "RM556934",
                        principalTable: "PROFISSIONAL",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "ENCAMINHAMENTO_TRIAGEM_FK",
                        column: x => x.TRIAGEM_ID,
                        principalSchema: "RM556934",
                        principalTable: "TRIAGEM",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ACOMPANHAMENTO",
                schema: "RM556934",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "\"RM556934\".\"SEQ_ACOMPANHAMENTO_GS\".NEXTVAL"),
                    DATA_EVENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TIPO_EVENTO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ANEXO_URL = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false, defaultValue: "N/A"),
                    ENCAMINHAMENTO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ACOMPANHAMENTO_PK", x => x.ID);
                    table.ForeignKey(
                        name: "ACOMPANHAMENTO_ENCAMINHAMENTO_FK",
                        column: x => x.ENCAMINHAMENTO_ID,
                        principalSchema: "RM556934",
                        principalTable: "ENCAMINHAMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACOMPANHAMENTO_ENCAMINHAMENTO_ID",
                schema: "RM556934",
                table: "ACOMPANHAMENTO",
                column: "ENCAMINHAMENTO_ID");

            migrationBuilder.CreateIndex(
                name: "EMPRESA_CNPJ_UN",
                schema: "RM556934",
                table: "EMPRESA",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ENCAMINHAMENTO_PROFISSIONAL_ID",
                schema: "RM556934",
                table: "ENCAMINHAMENTO",
                column: "PROFISSIONAL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ENCAMINHAMENTO_TRIAGEM_ID",
                schema: "RM556934",
                table: "ENCAMINHAMENTO",
                column: "TRIAGEM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TRIAGEM_USUARIO_ID",
                schema: "RM556934",
                table: "TRIAGEM",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_SISTEMA_EMPRESA_ID",
                schema: "RM556934",
                table: "USUARIO_SISTEMA",
                column: "EMPRESA_ID");

            migrationBuilder.CreateIndex(
                name: "USUARIO_EMAIL_UN",
                schema: "RM556934",
                table: "USUARIO_SISTEMA",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACOMPANHAMENTO",
                schema: "RM556934");

            migrationBuilder.DropTable(
                name: "ENCAMINHAMENTO",
                schema: "RM556934");

            migrationBuilder.DropTable(
                name: "PROFISSIONAL",
                schema: "RM556934");

            migrationBuilder.DropTable(
                name: "TRIAGEM",
                schema: "RM556934");

            migrationBuilder.DropTable(
                name: "USUARIO_SISTEMA",
                schema: "RM556934");

            migrationBuilder.DropTable(
                name: "EMPRESA",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_ACOMPANHAMENTO_GS",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_EMPRESA_GS",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_ENCAMINHAMENTO_GS",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_PROFISSIONAL_GS",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_TRIAGEM_GS",
                schema: "RM556934");

            migrationBuilder.DropSequence(
                name: "SEQ_USUARIO_GS",
                schema: "RM556934");
        }
    }
}
