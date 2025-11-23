using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using RoommateManager.Module.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace RoommateManager.Module.Controllers
{
    // [Authorize]
    public class ActivityController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;
        private readonly IAuthorizationService _authorizationService;

        public ActivityController(
            IContentManager contentManager,
            ISession session,
            IAuthorizationService authorizationService)
        {
            _contentManager = contentManager;
            _session = session;
            _authorizationService = authorizationService;
        }

        // GET: /Activity/MyActivities
        [HttpGet]
        public async Task<IActionResult> MyActivities()
        {
            var activities = await _session.Query<ContentItem, ContentItemIndex>(x =>
                x.ContentType == "Activity" &&
                x.Published)
                .ListAsync();

            var groupedActivities = new Dictionary<string, List<ActivityViewModel>>();

            foreach (var item in activities)
            {
                var activityPart = item.As<ActivityPart>();
                if (activityPart == null) continue;

                string activityName = GetTextField(activityPart, "ActivityName");
                string description = GetTextField(activityPart, "Description");
                string roomType = GetTextField(activityPart, "RoomType");
                int estimatedMinutes = (int)GetNumericField(activityPart, "EstimatedMinutes");
                bool isCompleted = GetBooleanField(activityPart, "IsCompleted");

                var viewModel = new ActivityViewModel
                {
                    ContentItemId = item.ContentItemId,
                    ActivityName = activityName,
                    Description = description,
                    RoomType = roomType,
                    EstimatedMinutes = estimatedMinutes,
                    IsCompleted = isCompleted
                };

                if (string.IsNullOrEmpty(roomType))
                {
                    roomType = "Uncategorized";
                }

                if (!groupedActivities.ContainsKey(roomType))
                {
                    groupedActivities[roomType] = new List<ActivityViewModel>();
                }
                groupedActivities[roomType].Add(viewModel);
            }

            return View(groupedActivities);
        }

        // GET: /Activity/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ActivityViewModel());
        }

        // POST: /Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contentItem = await _contentManager.NewAsync("Activity");
            var activityPart = contentItem.As<ActivityPart>();

            if (activityPart != null)
            {
                SetTextField(activityPart, "ActivityName", model.ActivityName);
                SetTextField(activityPart, "Description", model.Description ?? "");
                SetTextField(activityPart, "RoomType", model.RoomType);
                SetNumericField(activityPart, "EstimatedMinutes", model.EstimatedMinutes);
                SetBooleanField(activityPart, "IsCompleted", false);

                contentItem.DisplayText = model.ActivityName;
                contentItem.Author = User.Identity?.Name ?? "Unknown";

                await _contentManager.CreateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(MyActivities));
        }

        // POST: /Activity/MarkAsCompleted
        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(string contentItemId)
        {
            if (string.IsNullOrEmpty(contentItemId))
            {
                return RedirectToAction(nameof(MyActivities));
            }

            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
            {
                return RedirectToAction(nameof(MyActivities));
            }

            var activityPart = contentItem.As<ActivityPart>();
            if (activityPart != null)
            {
                try
                {
                    dynamic content = activityPart.Content;
                    if (content.IsCompleted != null && content.IsCompleted.Value != null)
                    {
                        content.IsCompleted.Value = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting IsCompleted: {ex.Message}");
                }

                await _contentManager.UpdateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(MyActivities));
        }

        [HttpGet]
        public async Task<IActionResult> MarkAsCompletedGet(string contentItemId)
        {
            return await MarkAsCompleted(contentItemId);
        }

        // POST: /Activity/Reopen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(string contentItemId)
        {
            if (string.IsNullOrEmpty(contentItemId))
            {
                return RedirectToAction(nameof(MyActivities));
            }

            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            if (contentItem == null)
            {
                return RedirectToAction(nameof(MyActivities));
            }

            var activityPart = contentItem.As<ActivityPart>();
            if (activityPart != null)
            {
                try
                {
                    dynamic content = activityPart.Content;
                    if (content.IsCompleted != null && content.IsCompleted.Value != null)
                    {
                        content.IsCompleted.Value = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting IsCompleted: {ex.Message}");
                }

                await _contentManager.UpdateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(MyActivities));
        }

        // GET: /Activity/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var activities = await _session.Query<ContentItem, ContentItemIndex>(x =>
                x.ContentType == "Activity" &&
                x.Published)
                .ListAsync();

            var completedActivities = new List<ActivityViewModel>();

            foreach (var item in activities)
            {
                var activityPart = item.As<ActivityPart>();
                if (activityPart == null) continue;

                var isCompleted = GetBooleanField(activityPart, "IsCompleted");
                if (!isCompleted) continue;

                completedActivities.Add(new ActivityViewModel
                {
                    ContentItemId = item.ContentItemId,
                    ActivityName = GetTextField(activityPart, "ActivityName"),
                    Description = GetTextField(activityPart, "Description"),
                    RoomType = GetTextField(activityPart, "RoomType"),
                    IsCompleted = true,
                    CompletedBy = User.Identity?.Name ?? "Unknown",
                    CompletedDate = item.ModifiedUtc ?? DateTime.UtcNow
                });
            }

            return View(completedActivities.OrderByDescending(a => a.CompletedDate));
        }

        // Helper methods
        private string GetTextField(ContentPart part, string fieldName)
        {
            try
            {
                dynamic content = part.Content;
                var field = content[fieldName];

                if (field != null && field.Text != null)
                {
                    return field.Text.ToString();
                }

                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTextField({fieldName}) failed: {ex.Message}");
                return "";
            }
        }

        private decimal GetNumericField(ContentPart part, string fieldName)
        {
            try
            {
                dynamic content = part.Content;
                var field = content[fieldName];

                if (field != null && field.Value != null)
                {
                    return Convert.ToDecimal(field.Value);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetNumericField({fieldName}) failed: {ex.Message}");
                return 0;
            }
        }

        private bool GetBooleanField(ContentPart part, string fieldName)
        {
            try
            {
                dynamic content = part.Content;
                var field = content[fieldName];

                if (field != null && field.Value != null)
                {
                    return Convert.ToBoolean(field.Value);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBooleanField({fieldName}) failed: {ex.Message}");
                return false;
            }
        }

        private void SetTextField(ContentPart part, string fieldName, string value)
        {
            try
            {
                dynamic content = part.Content;
                if (content[fieldName] == null)
                {
                    content[fieldName] = new { Text = value };
                }
                else
                {
                    content[fieldName].Text = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetTextField({fieldName}) failed: {ex.Message}");
            }
        }

        private void SetNumericField(ContentPart part, string fieldName, decimal value)
        {
            try
            {
                dynamic content = part.Content;
                if (content[fieldName] == null)
                {
                    content[fieldName] = new { Value = value };
                }
                else
                {
                    content[fieldName].Value = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetNumericField({fieldName}) failed: {ex.Message}");
            }
        }

        private void SetBooleanField(ContentPart part, string fieldName, bool value)
        {
            try
            {
                dynamic content = part.Content;
                if (content[fieldName] == null)
                {
                    content[fieldName] = new { Value = value };
                }
                else
                {
                    content[fieldName].Value = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetBooleanField({fieldName}) failed: {ex.Message}");
            }
        }
    }

    public class ActivityViewModel
    {
        public string ContentItemId { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Activity name is required")]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string ActivityName { get; set; }
        
        public string Description { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Room type is required")]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string RoomType { get; set; }
        
        public int EstimatedMinutes { get; set; }
        public bool IsCompleted { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}