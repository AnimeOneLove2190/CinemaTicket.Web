using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaTicket.DataAccess.Migrations
{
    public partial class createdCreatedByAndModifiedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Rows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Rows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Places",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Places",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Movies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Movies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Halls",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Halls",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "CreatedOn", "Email", "HashPassword", "Login", "RoleId" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "System", new Guid("3868c13d-8d12-46a6-b709-52bcf010bdff") });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CreatedBy",
                table: "Tickets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ModifiedBy",
                table: "Tickets",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedBy",
                table: "Sessions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ModifiedBy",
                table: "Sessions",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_CreatedBy",
                table: "Rows",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Rows_ModifiedBy",
                table: "Rows",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Places_CreatedBy",
                table: "Places",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Places_ModifiedBy",
                table: "Places",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CreatedBy",
                table: "Movies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ModifiedBy",
                table: "Movies",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Halls_CreatedBy",
                table: "Halls",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Halls_ModifiedBy",
                table: "Halls",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_CreatedBy",
                table: "Genres",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ModifiedBy",
                table: "Genres",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Accounts_CreatedBy",
                table: "Genres",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Accounts_ModifiedBy",
                table: "Genres",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Halls_Accounts_CreatedBy",
                table: "Halls",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Halls_Accounts_ModifiedBy",
                table: "Halls",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Accounts_CreatedBy",
                table: "Movies",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Accounts_ModifiedBy",
                table: "Movies",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Accounts_CreatedBy",
                table: "Places",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Accounts_ModifiedBy",
                table: "Places",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rows_Accounts_CreatedBy",
                table: "Rows",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rows_Accounts_ModifiedBy",
                table: "Rows",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Accounts_CreatedBy",
                table: "Sessions",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Accounts_ModifiedBy",
                table: "Sessions",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Accounts_CreatedBy",
                table: "Tickets",
                column: "CreatedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Accounts_ModifiedBy",
                table: "Tickets",
                column: "ModifiedBy",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);


            migrationBuilder.Sql("UPDATE Genres SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Halls SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Movies SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Places SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Rows SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Sessions SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
            migrationBuilder.Sql("UPDATE Tickets SET CreatedBy = '00000000-0000-0000-0000-000000000001', ModifiedBy = '00000000-0000-0000-0000-000000000001' WHERE CreatedBy IS NULL OR ModifiedBy IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Accounts_CreatedBy",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Accounts_ModifiedBy",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Halls_Accounts_CreatedBy",
                table: "Halls");

            migrationBuilder.DropForeignKey(
                name: "FK_Halls_Accounts_ModifiedBy",
                table: "Halls");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Accounts_CreatedBy",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Accounts_ModifiedBy",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Accounts_CreatedBy",
                table: "Places");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Accounts_ModifiedBy",
                table: "Places");

            migrationBuilder.DropForeignKey(
                name: "FK_Rows_Accounts_CreatedBy",
                table: "Rows");

            migrationBuilder.DropForeignKey(
                name: "FK_Rows_Accounts_ModifiedBy",
                table: "Rows");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Accounts_CreatedBy",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Accounts_ModifiedBy",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Accounts_CreatedBy",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Accounts_ModifiedBy",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CreatedBy",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ModifiedBy",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_CreatedBy",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ModifiedBy",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Rows_CreatedBy",
                table: "Rows");

            migrationBuilder.DropIndex(
                name: "IX_Rows_ModifiedBy",
                table: "Rows");

            migrationBuilder.DropIndex(
                name: "IX_Places_CreatedBy",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_ModifiedBy",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CreatedBy",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_ModifiedBy",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Halls_CreatedBy",
                table: "Halls");

            migrationBuilder.DropIndex(
                name: "IX_Halls_ModifiedBy",
                table: "Halls");

            migrationBuilder.DropIndex(
                name: "IX_Genres_CreatedBy",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ModifiedBy",
                table: "Genres");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Rows");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Rows");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Genres");
        }
    }
}
