using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Users.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMassTransitOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "user_password", "user_status" },
                values: new object[] { "$2a$12$gjZJ.srEyPnLQOd926FweOccmMHKVw82fZKboGl0ylQnqbB4KaMoq", "A" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Consumed = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ConsumerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true),
                    LockId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MessageId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    Received = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "longblob", rowVersion: true, nullable: true)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "char(36)", nullable: false),
                    BusName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true),
                    LockId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "longblob", rowVersion: true, nullable: true)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(type: "longtext", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    ConversationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DestinationAddress = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EnqueueTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FaultAddress = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Headers = table.Column<string>(type: "longtext", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "char(36)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "char(36)", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    MessageId = table.Column<Guid>(type: "char(36)", nullable: false),
                    MessageType = table.Column<string>(type: "longtext", nullable: false),
                    OutboxId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Properties = table.Column<string>(type: "longtext", nullable: true),
                    RequestId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ResponseAddress = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SourceAddress = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "user_password", "user_status" },
                values: new object[] { "$2a$12$3ESYC5JNdL2aH1cVrPRibu3VfPlOBi8Ea/AGXyMD1KM6mBfJApdJG", "\0" });

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_BusName_Created",
                table: "OutboxState",
                columns: new[] { "BusName", "Created" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");
        }
    }
}
