using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Records;
using YesSql;

namespace HouseholdManager.Module.Controllers;

[Authorize]
public class NoteController : Controller
{
    private readonly ISession _session;
    private readonly IContentManager _contentManager;

    public NoteController(ISession session, IContentManager contentManager)
    {
        _session = session;
        _contentManager = contentManager;
    }

    [Route("my-notes")]
    public async Task<IActionResult> MyNotes()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var userId = User.Identity.Name ?? string.Empty;

        var notes = await _session
            .Query<ContentItem, ContentItemIndex>(x => x.ContentType == "Note" && x.Published)
            .ListAsync();

        ViewData["CurrentUserId"] = userId;
        return View(notes);
    }

    [HttpGet]
    [Route("note/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("note/create")]
    public async Task<IActionResult> Create(string title, string noteContent)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized();
        }

        var userName = User.Identity.Name ?? string.Empty;

        var contentItem = await _contentManager.NewAsync("Note");
        var notePart = contentItem.As<NotePart>();

        if (notePart != null)
        {
            notePart.Title = title;
            notePart.NoteContent = noteContent;
            notePart.UserId = userName;

            contentItem.Apply(notePart);
        }

        await _contentManager.CreateAsync(contentItem);
        await _contentManager.PublishAsync(contentItem);

        return RedirectToAction(nameof(MyNotes));
    }

    [HttpPost]
    [Route("note/delete/{contentItemId}")]
    public async Task<IActionResult> Delete(string contentItemId)
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

        await _contentManager.RemoveAsync(contentItem);

        return RedirectToAction(nameof(MyNotes));
    }
}
