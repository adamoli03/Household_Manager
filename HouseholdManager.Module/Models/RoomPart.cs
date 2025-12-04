using OrchardCore.ContentManagement;

namespace HouseholdManager.Module.Models;

public class RoomPart : ContentPart
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
