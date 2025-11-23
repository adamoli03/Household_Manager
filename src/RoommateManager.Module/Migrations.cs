using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using RoommateManager.Module.Models;
using System.Threading.Tasks;

namespace RoommateManager.Module
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<int> CreateAsync()
        {
            // Create Activity Content Part Definition
            await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(ActivityPart), part => part
                .Attachable()
                .WithDescription("Represents a must-do activity for a room"));

            // Create Activity Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Activity", type => type
                .DisplayedAs("Activity")
                .WithPart(nameof(ActivityPart))
                .WithPart("TitlePart")
                .Creatable()
                .Listable()
                .Draftable()
                .Securable());

            // Create Note Content Part Definition
            await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(NotePart), part => part
                .Attachable()
                .WithDescription("Represents a private note"));

            // Create Note Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Note", type => type
                .DisplayedAs("Note")
                .WithPart(nameof(NotePart))
                .WithPart("TitlePart")
                .Creatable()
                .Listable()
                .Draftable()
                .Securable());

            // Create Room Content Part Definition
            await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(RoomPart), part => part
                .Attachable()
                .WithDescription("Represents a room type in the household"));

            // Create Room Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Room", type => type
                .DisplayedAs("Room")
                .WithPart(nameof(RoomPart))  // This should be "RoomPart" not "Room"
                .WithPart("TitlePart")
                .Creatable()
                .Listable()
                .Draftable()
                .Securable());

            // Create GroceryItem Content Part Definition
            await _contentDefinitionManager.AlterPartDefinitionAsync(nameof(GroceryItemPart), part => part
                .Attachable()
                .WithDescription("Represents an item on the grocery list"));

            // Create GroceryItem Content Type
            await _contentDefinitionManager.AlterTypeDefinitionAsync("GroceryItem", type => type
                .DisplayedAs("Grocery Item")
                .WithPart(nameof(GroceryItemPart))
                .Creatable()
                .Listable()
                .Draftable()
                .Securable());

            return 1;
        }

        // Add this method to fix existing content types
        public async Task<int> UpdateFrom1Async()
        {
            // Remove the incorrectly named parts
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Room", type => type
                .RemovePart("Room"));  // Remove the wrong one
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Activity", type => type
                .RemovePart("Activity"));
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Note", type => type
                .RemovePart("Note"));
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("GroceryItem", type => type
                .RemovePart("GroceryItem"));

            // Add the correctly named parts
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Room", type => type
                .WithPart(nameof(RoomPart)));
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Activity", type => type
                .WithPart(nameof(ActivityPart)));
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("Note", type => type
                .WithPart(nameof(NotePart)));
            
            await _contentDefinitionManager.AlterTypeDefinitionAsync("GroceryItem", type => type
                .WithPart(nameof(GroceryItemPart)));

            return 2;
        }
    }
}