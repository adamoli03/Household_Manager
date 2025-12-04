using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using YesSql;

namespace HouseholdManager.Module.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    private readonly ISession _session;

    public HomeController(ISession session)
    {
        _session = session;
    }

    [Route("")]
    [Route("home")]
    public async Task<IActionResult> Index()
    {
        var allActivities = await _session
            .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Activity" && x.Published)
            .ListAsync();

        var pendingActivities = allActivities.Count(item =>
        {
            var part = item.As<ActivityPart>();
            return part != null && !part.IsCompleted;
        });

        var completedActivities = allActivities.Count(item =>
        {
            var part = item.As<ActivityPart>();
            return part != null && part.IsCompleted;
        });

        var groceryItems = await _session
            .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "GroceryItem" && x.Published)
            .CountAsync();

        var rooms = await _session
            .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Room" && x.Published)
            .CountAsync();

        ViewData["PendingActivities"] = pendingActivities;
        ViewData["CompletedActivities"] = completedActivities;
        ViewData["GroceryItems"] = groceryItems;
        ViewData["Rooms"] = rooms;

        return View();
    }
}
