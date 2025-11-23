using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using RoommateManager.Module.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YesSql;

namespace RoommateManager.Module.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;

        public DiagnosticsController(
            IContentManager contentManager,
            ISession session)
        {
            _contentManager = contentManager;
            _session = session;
        }

        [HttpGet]
        public async Task<IActionResult> InspectActivity(string contentItemId)
        {
            if (string.IsNullOrEmpty(contentItemId))
            {
                // Get the first activity
                var firstActivity = await _session.Query<ContentItem, ContentItemIndex>(x =>
                    x.ContentType == "Activity" &&
                    x.Published)
                    .FirstOrDefaultAsync();

                if (firstActivity == null)
                {
                    return Content("No activities found");
                }

                contentItemId = firstActivity.ContentItemId;
            }

            var contentItem = await _contentManager.GetAsync(contentItemId);
            if (contentItem == null)
            {
                return Content($"Activity not found: {contentItemId}");
            }

            var activityPart = contentItem.As<ActivityPart>();
            if (activityPart == null)
            {
                return Content("ActivityPart not found on content item");
            }

            var output = new StringBuilder();
            output.AppendLine("=== ACTIVITY CONTENT STRUCTURE ===");
            output.AppendLine($"ContentItemId: {contentItem.ContentItemId}");
            output.AppendLine($"ContentType: {contentItem.ContentType}");
            output.AppendLine();
            output.AppendLine("=== ActivityPart.Content (JSON) ===");
            output.AppendLine(JsonSerializer.Serialize(activityPart.Content, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));
            output.AppendLine();
            output.AppendLine("=== Full ContentItem (JSON) ===");
            try
            {
                output.AppendLine(JsonSerializer.Serialize(contentItem, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    MaxDepth = 5,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                }));
            }
            catch (Exception ex)
            {
                output.AppendLine($"Could not serialize full content item: {ex.Message}");
            }

            return Content(output.ToString(), "text/plain");
        }
    }
}