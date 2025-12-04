using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using HouseholdManager.Module.Indexes;
using YesSql.Sql;

namespace HouseholdManager.Module.Migrations;

public class Migrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public Migrations(IContentDefinitionManager contentDefinitionManager)
    {
        _contentDefinitionManager = contentDefinitionManager;
    }

    public async Task<int> CreateAsync()
    {
        await _contentDefinitionManager.AlterPartDefinitionAsync("ActivityPart", part => part
            .Attachable()
            .WithDescription("Represents a household activity that needs to be completed"));

        await _contentDefinitionManager.AlterTypeDefinitionAsync("Activity", type => type
            .WithPart("ActivityPart")
            .Creatable()
            .Listable()
            .Draftable()
            .Securable());

        await _contentDefinitionManager.AlterPartDefinitionAsync("RoomPart", part => part
            .Attachable()
            .WithDescription("Represents a room type in the household"));

        await _contentDefinitionManager.AlterTypeDefinitionAsync("Room", type => type
            .WithPart("RoomPart")
            .Creatable()
            .Listable()
            .Draftable()
            .Securable());

        await _contentDefinitionManager.AlterPartDefinitionAsync("NotePart", part => part
            .Attachable()
            .WithDescription("Represents a private note for a user"));

        await _contentDefinitionManager.AlterTypeDefinitionAsync("Note", type => type
            .WithPart("NotePart")
            .Creatable()
            .Listable()
            .Draftable()
            .Securable());

        await _contentDefinitionManager.AlterPartDefinitionAsync("GroceryItemPart", part => part
            .Attachable()
            .WithDescription("Represents an item in the shared grocery list"));

        await _contentDefinitionManager.AlterTypeDefinitionAsync("GroceryItem", type => type
            .WithPart("GroceryItemPart")
            .Creatable()
            .Listable()
            .Draftable()
            .Securable());

        // TODO: Create the index for ActivityPart
        await SchemaBuilder.CreateMapIndexTableAsync<ActivityPartIndex>(table => table
            .Column<string>("ContentItemId", column => column.WithLength(26))
            .Column<string>("Title", column => column.WithLength(255))
            .Column<string>("RoomType", column => column.WithLength(255))
            .Column<string>("AssignedUserId", column => column.WithLength(255))
            .Column<bool>("IsCompleted")
            .Column<DateTime?>("CompletedDate")
            .Column<string>("CompletedByUserId", column => column.WithLength(255))
        );

        return 1;
    }
}
