using HouseholdManager.Module.Models;
using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace HouseholdManager.Module.Indexes;

public class ActivityPartIndexProvider : IndexProvider<ContentItem>
{
    public override void Describe(DescribeContext<ContentItem> context)
    {
        context.For<ActivityPartIndex>()
            .Map(contentItem =>
            {
                var activityPart = contentItem.As<ActivityPart>();

                if (activityPart == null)
                {
                    return null;
                }

                return new ActivityPartIndex
                {
                    ContentItemId = contentItem.ContentItemId,
                    Title = activityPart.Title,
                    RoomType = activityPart.RoomType,
                    AssignedUserId = activityPart.AssignedUserId,
                    IsCompleted = activityPart.IsCompleted,
                    CompletedDate = activityPart.CompletedDate,
                    CompletedByUserId = activityPart.CompletedByUserId
                };
            });
    }
}
