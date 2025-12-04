using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Module.ViewModels;

public class GroceryItemPartViewModel
{
    [Required]
    public string ItemName { get; set; } = string.Empty;

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;

    public bool IsPurchased { get; set; }
    public string? PurchasedByUserId { get; set; }
    public DateTime? PurchasedDate { get; set; }

    [BindNever]
    public GroceryItemPart? GroceryItemPart { get; set; }
}
