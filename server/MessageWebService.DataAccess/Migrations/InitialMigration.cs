using FluentMigrator;

namespace MessageWebService.DataAccess.Migrations;

[Migration(1)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("messages")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("text").AsString(128).NotNullable()
            .WithColumn("timestamp").AsDateTime().NotNullable()
            .WithColumn("sequence_number").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("messages");
    }
}
