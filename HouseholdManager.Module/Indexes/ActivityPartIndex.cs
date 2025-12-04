using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace HouseholdManager.Module.Indexes;

public class ActivityPartIndex : MapIndex
{
    public string ContentItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string AssignedUserId { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedByUserId { get; set; }
}
