using HouseholdManager.Module.Indexes;
using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using YesSql;

namespace HouseholdManager.Module.Controllers;

[Authorize]
public class ActivityController : Controller
{
    private readonly ISession _session;
    private readonly IContentManager _contentManager;

    public ActivityController(
        ISession session,
        IContentManager contentManager)
    {
        _session = session;
        _contentManager = contentManager;
    }

    [Route("my-activities")]
    public async Task<IActionResult> MyActivities()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.Identity.Name ?? string.Empty;

        
        var allActivities = await _session
            .Query<ContentItem, OrchardCore.ContentManagement.Records.ContentItemIndex>(x => x.ContentType == "Activity" && x.Published)
            .ListAsync();

        
        var activities = allActivities.Where(item =>
        {
            var part = item.As<ActivityPart>();
            return part != null && !part.IsCompleted;
        }).ToList();

        ViewData["CurrentUserId"] = userId;
        return View(activities);
    }

    [Route("activity-history")]
    public async Task<IActionResult> History()
    {
        var allActivities = await _session
            .Query<ContentItem, OrchardCore.ContentManagement.Records.ContentItemIndex>(x => x.ContentType == "Activity" && x.Published)
            .ListAsync();

        var completedActivities = allActivities
            .Where(item =>
            {
                var part = item.As<ActivityPart>();
                return part != null && part.IsCompleted;
            })
            .OrderByDescending(item =>
            {
                var part = item.As<ActivityPart>();
                return part?.CompletedDate ?? DateTime.MinValue;
            })
            .ToList();

        return View(completedActivities);
    }

    [HttpPost]
    [Route("activity/complete/{contentItemId}")]
    public async Task<IActionResult> MarkComplete(string contentItemId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var userName = User.Identity.Name ?? string.Empty;

        var contentItem = await _contentManager.GetAsync(contentItemId);
        if (contentItem == null)
        {
            return NotFound();
        }

        var activityPart = contentItem.As<ActivityPart>();
        if (activityPart == null)
        {
            return NotFound();
        }

        activityPart.IsCompleted = true;
        activityPart.CompletedDate = DateTime.UtcNow;
        activityPart.CompletedByUserId = userName;

        contentItem.Apply(activityPart);
        await _contentManager.UpdateAsync(contentItem);

        return RedirectToAction(nameof(MyActivities));
    }

    [HttpPost]
    [Route("activity/reopen/{contentItemId}")]
    public async Task<IActionResult> Reopen(string contentItemId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var contentItem = await _contentManager.GetAsync(contentItemId);
        if (contentItem == null)
        {
            return NotFound();
        }

        var activityPart = contentItem.As<ActivityPart>();
        if (activityPart == null)
        {
            return NotFound();
        }

        activityPart.IsCompleted = false;
        activityPart.CompletedDate = null;
        activityPart.CompletedByUserId = null;

        contentItem.Apply(activityPart);
        await _contentManager.UpdateAsync(contentItem);

        return RedirectToAction(nameof(History));
    }

    [HttpGet]
    [Route("activity/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("activity/create")]
    public async Task<IActionResult> Create(string title, string description, string roomType, string assignedUserId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var contentItem = await _contentManager.NewAsync("Activity");
        var activityPart = contentItem.As<ActivityPart>();

        if (activityPart != null)
        {
            activityPart.Title = title;
            activityPart.Description = description;
            activityPart.RoomType = roomType;
            activityPart.AssignedUserId = assignedUserId;
            activityPart.IsCompleted = false;

            contentItem.Apply(activityPart);
        }

        await _contentManager.CreateAsync(contentItem);
        await _contentManager.PublishAsync(contentItem);

        return RedirectToAction(nameof(MyActivities));
    }
}
