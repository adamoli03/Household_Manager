using OrchardCore.ContentManagement;

namespace HouseholdManager.Module.Models;

public class NotePart : ContentPart
{
    public string Title { get; set; } = string.Empty;
    public string NoteContent { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
