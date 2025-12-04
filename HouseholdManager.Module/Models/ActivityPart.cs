using OrchardCore.ContentManagement;

namespace HouseholdManager.Module.Models;

public class ActivityPart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string AssignedUserId { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedByUserId { get; set; }
}
