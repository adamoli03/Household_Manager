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
    [Authorize]
    public class NoteController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;

        public NoteController(
            IContentManager contentManager,
            ISession session)
        {
            _contentManager = contentManager;
            _session = session;
        }

        // GET: /Note/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var notes = await _session.Query<ContentItem, ContentItemIndex>(x => 
                x.ContentType == "Note" && 
                x.Published)
                .ListAsync();

            var noteViewModels = new List<NoteViewModel>();

            foreach (var item in notes)
            {
                var notePart = item.As<NotePart>();
                if (notePart == null) continue;

                noteViewModels.Add(new NoteViewModel
                {
                    ContentItemId = item.ContentItemId,
                    NoteTitle = GetTextField(notePart, "NoteTitle"),
                    NoteContent = GetTextField(notePart, "NoteContent"),
                    Category = GetTextField(notePart, "Category"),
                    IsPinned = GetBooleanField(notePart, "IsPinned"),
                    ModifiedDate = item.ModifiedUtc,
                    CreatedBy = item.Author ?? "Unknown"
                });
            }

            return View(noteViewModels
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.ModifiedDate));
        }

        // GET: /Note/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new NoteViewModel());
        }

        // POST: /Note/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contentItem = await _contentManager.NewAsync("Note");
            var notePart = contentItem.As<NotePart>();

            if (notePart != null)
            {
                SetTextField(notePart, "NoteTitle", model.NoteTitle);
                SetTextField(notePart, "NoteContent", model.NoteContent);
                SetTextField(notePart, "Category", model.Category ?? "");
                SetBooleanField(notePart, "IsPinned", model.IsPinned);

                contentItem.DisplayText = model.NoteTitle;
                contentItem.Author = User.Identity?.Name ?? "Unknown";

                await _contentManager.CreateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Note/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var contentItem = await _contentManager.GetAsync(id, VersionOptions.Latest);
            if (contentItem == null)
            {
                return NotFound();
            }

            var notePart = contentItem.As<NotePart>();
            if (notePart == null)
            {
                return NotFound();
            }

            var model = new NoteViewModel
            {
                ContentItemId = contentItem.ContentItemId,
                NoteTitle = GetTextField(notePart, "NoteTitle"),
                NoteContent = GetTextField(notePart, "NoteContent"),
                Category = GetTextField(notePart, "Category"),
                IsPinned = GetBooleanField(notePart, "IsPinned")
            };

            return View(model);
        }

        // POST: /Note/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contentItem = await _contentManager.GetAsync(id, VersionOptions.Latest);
            if (contentItem == null)
            {
                return NotFound();
            }

            var notePart = contentItem.As<NotePart>();
            if (notePart != null)
            {
                SetTextField(notePart, "NoteTitle", model.NoteTitle);
                SetTextField(notePart, "NoteContent", model.NoteContent);
                SetTextField(notePart, "Category", model.Category ?? "");
                SetBooleanField(notePart, "IsPinned", model.IsPinned);

                contentItem.DisplayText = model.NoteTitle;

                await _contentManager.UpdateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Note/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var contentItem = await _contentManager.GetAsync(id, VersionOptions.Latest);
            if (contentItem == null)
            {
                return NotFound();
            }

            await _contentManager.RemoveAsync(contentItem);

            return RedirectToAction(nameof(Index));
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

    public class NoteViewModel
    {
        public string ContentItemId { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Title is required")]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string NoteTitle { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Content is required")]
        public string NoteContent { get; set; }
        
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string Category { get; set; }
        
        public bool IsPinned { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}