using HouseholdManager.Module.Models;
using HouseholdManager.Module.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace HouseholdManager.Module.Drivers;

public class GroceryItemPartDriver : ContentPartDisplayDriver<GroceryItemPart>
{
    public override IDisplayResult Display(GroceryItemPart part, BuildPartDisplayContext context)
    {
        return Initialize<GroceryItemPartViewModel>("GroceryItemPart", model =>
        {
            model.ItemName = part.ItemName;
            model.Quantity = part.Quantity;
            model.IsPurchased = part.IsPurchased;
            model.PurchasedByUserId = part.PurchasedByUserId;
            model.PurchasedDate = part.PurchasedDate;
            model.GroceryItemPart = part;
        })
        .Location("Detail", "Content:1")
        .Location("Summary", "Content:1");
    }

    public override IDisplayResult Edit(GroceryItemPart part, BuildPartEditorContext context)
    {
        return Initialize<GroceryItemPartViewModel>("GroceryItemPart_Edit", model =>
        {
            model.ItemName = part.ItemName;
            model.Quantity = part.Quantity;
            model.IsPurchased = part.IsPurchased;
            model.PurchasedByUserId = part.PurchasedByUserId;
            model.PurchasedDate = part.PurchasedDate;
            model.GroceryItemPart = part;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(GroceryItemPart part, UpdatePartEditorContext context)
    {
        var viewModel = new GroceryItemPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.ItemName = viewModel.ItemName;
        part.Quantity = viewModel.Quantity;
        part.IsPurchased = viewModel.IsPurchased;
        part.PurchasedByUserId = viewModel.PurchasedByUserId;
        part.PurchasedDate = viewModel.PurchasedDate;

        return Edit(part, context);
    }
}
