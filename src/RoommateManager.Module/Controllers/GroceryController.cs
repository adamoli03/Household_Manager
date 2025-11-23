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
    public class GroceryController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly ISession _session;

        public GroceryController(
            IContentManager contentManager,
            ISession session)
        {
            _contentManager = contentManager;
            _session = session;
        }

        // GET: /Grocery/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _session.Query<ContentItem, ContentItemIndex>(x => 
                x.ContentType == "GroceryItem" && 
                x.Published)
                .ListAsync();

            var groceryItems = new List<GroceryItemViewModel>();

            foreach (var item in items)
            {
                var groceryPart = item.As<GroceryItemPart>();
                if (groceryPart == null) continue;

                groceryItems.Add(new GroceryItemViewModel
                {
                    ContentItemId = item.ContentItemId,
                    ItemName = GetTextField(groceryPart, "ItemName"),
                    Quantity = (int)GetNumericField(groceryPart, "Quantity"),
                    Unit = GetTextField(groceryPart, "Unit"),
                    Notes = GetTextField(groceryPart, "Notes"),
                    IsPurchased = GetBooleanField(groceryPart, "IsPurchased"),
                    AddedBy = item.Author ?? "Unknown",
                    PurchasedBy = GetBooleanField(groceryPart, "IsPurchased") ? User.Identity?.Name : null,
                    PurchasedDate = GetBooleanField(groceryPart, "IsPurchased") ? item.ModifiedUtc : null
                });
            }

            return View(groceryItems
                .OrderBy(i => i.IsPurchased)
                .ThenBy(i => i.ItemName));
        }

        // GET: /Grocery/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new GroceryItemViewModel { Quantity = 1 });
        }

        // POST: /Grocery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroceryItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contentItem = await _contentManager.NewAsync("GroceryItem");
            var groceryPart = contentItem.As<GroceryItemPart>();

            if (groceryPart != null)
            {
                SetTextField(groceryPart, "ItemName", model.ItemName);
                SetNumericField(groceryPart, "Quantity", model.Quantity);
                SetTextField(groceryPart, "Unit", model.Unit ?? "");
                SetTextField(groceryPart, "Notes", model.Notes ?? "");
                SetBooleanField(groceryPart, "IsPurchased", false);

                contentItem.DisplayText = model.ItemName;
                contentItem.Author = User.Identity?.Name ?? "Unknown";

                await _contentManager.CreateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Grocery/MarkAsPurchased
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPurchased(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            
            if (contentItem == null)
            {
                return NotFound();
            }

            var groceryPart = contentItem.As<GroceryItemPart>();
            if (groceryPart != null)
            {
                try
                {
                    dynamic content = groceryPart.Content;
                    if (content.IsPurchased != null && content.IsPurchased.Value != null)
                    {
                        content.IsPurchased.Value = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting IsPurchased: {ex.Message}");
                }
                
                await _contentManager.UpdateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Grocery/Reopen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            
            if (contentItem == null)
            {
                return NotFound();
            }

            var groceryPart = contentItem.As<GroceryItemPart>();
            if (groceryPart != null)
            {
                try
                {
                    dynamic content = groceryPart.Content;
                    if (content.IsPurchased != null && content.IsPurchased.Value != null)
                    {
                        content.IsPurchased.Value = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting IsPurchased: {ex.Message}");
                }
                
                await _contentManager.UpdateAsync(contentItem);
                await _contentManager.PublishAsync(contentItem);
            }

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

    public class GroceryItemViewModel
    {
        public string ContentItemId { get; set; }
        
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Item name is required")]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string ItemName { get; set; }
        
        [System.ComponentModel.DataAnnotations.Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999")]
        public int Quantity { get; set; } = 1;
        
        [System.ComponentModel.DataAnnotations.StringLength(20)]
        public string Unit { get; set; }
        
        public string Notes { get; set; }
        public bool IsPurchased { get; set; }
        public string AddedBy { get; set; }
        public string PurchasedBy { get; set; }
        public DateTime? PurchasedDate { get; set; }
    }
}