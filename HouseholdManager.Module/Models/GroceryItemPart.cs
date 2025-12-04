using OrchardCore.ContentManagement;

namespace HouseholdManager.Module.Models;

public class GroceryItemPart : ContentPart
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public bool IsPurchased { get; set; }
    public string? PurchasedByUserId { get; set; }
    public DateTime? PurchasedDate { get; set; }
}
