using HouseholdManager.Module.Models;
using OrchardCore.ContentManagement.Handlers;

namespace HouseholdManager.Module.Handlers;

public class ActivityPartHandler : ContentPartHandler<ActivityPart>
{
    public override Task InitializingAsync(InitializingContentContext context, ActivityPart part)
    {
        // Initialize default values when creating a new activity
        part.IsCompleted = false;
        return Task.CompletedTask;
    }

    public override Task UpdatedAsync(UpdateContentContext context, ActivityPart part)
    {
        // Additional logic when an activity is updated
        // For example, you could add validation or logging here
        return Task.CompletedTask;
    }
}
