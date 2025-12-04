using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using YesSql;

namespace HouseholdManager.Module.Controllers;

[Authorize]
public class GroceryController : Controller
{
    private readonly ISession _session;
    private readonly IContentManager _contentManager;

    public GroceryController(
        ISession session,
        IContentManager contentManager)
    {
        _session = session;
        _contentManager = contentManager;
    }

    [Route("grocery-list")]
    public async Task<IActionResult> Index()
    {
        var groceryItems = await _session
            .Query<ContentItem, OrchardCore.ContentManagement.Records.ContentItemIndex>(x => x.ContentType == "GroceryItem")
            .ListAsync();

        return View(groceryItems);
    }

    [HttpPost]
    [Route("grocery/mark-purchased/{contentItemId}")]
    public async Task<IActionResult> MarkPurchased(string contentItemId)
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

        var groceryPart = contentItem.As<GroceryItemPart>();
        if (groceryPart == null)
        {
            return NotFound();
        }

        groceryPart.IsPurchased = true;
        groceryPart.PurchasedDate = DateTime.UtcNow;
        groceryPart.PurchasedByUserId = userName;

        contentItem.Apply(groceryPart);
        await _contentManager.UpdateAsync(contentItem);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Route("grocery/mark-needed/{contentItemId}")]
    public async Task<IActionResult> MarkNeeded(string contentItemId)
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

        var groceryPart = contentItem.As<GroceryItemPart>();
        if (groceryPart == null)
        {
            return NotFound();
        }

        groceryPart.IsPurchased = false;
        groceryPart.PurchasedDate = null;
        groceryPart.PurchasedByUserId = null;

        contentItem.Apply(groceryPart);
        await _contentManager.UpdateAsync(contentItem);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("grocery/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("grocery/create")]
    public async Task<IActionResult> Create(string itemName, int quantity)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var contentItem = await _contentManager.NewAsync("GroceryItem");
        var groceryPart = contentItem.As<GroceryItemPart>();

        if (groceryPart != null)
        {
            groceryPart.ItemName = itemName;
            groceryPart.Quantity = quantity > 0 ? quantity : 1;
            groceryPart.IsPurchased = false;

            contentItem.Apply(groceryPart);
        }

        await _contentManager.CreateAsync(contentItem);
        await _contentManager.PublishAsync(contentItem);

        return RedirectToAction(nameof(Index));
    }
}
