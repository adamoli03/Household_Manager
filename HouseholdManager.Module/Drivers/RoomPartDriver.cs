using HouseholdManager.Module.Models;
using HouseholdManager.Module.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace HouseholdManager.Module.Drivers;

public class RoomPartDriver : ContentPartDisplayDriver<RoomPart>
{
    public override IDisplayResult Display(RoomPart part, BuildPartDisplayContext context)
    {
        return Initialize<RoomPartViewModel>("RoomPart", model =>
        {
            model.Name = part.Name;
            model.Description = part.Description;
            model.RoomPart = part;
        })
        .Location("Detail", "Content:1")
        .Location("Summary", "Content:1");
    }

    public override IDisplayResult Edit(RoomPart part, BuildPartEditorContext context)
    {
        return Initialize<RoomPartViewModel>("RoomPart_Edit", model =>
        {
            model.Name = part.Name;
            model.Description = part.Description;
            model.RoomPart = part;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(RoomPart part, UpdatePartEditorContext context)
    {
        var viewModel = new RoomPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Name = viewModel.Name;
        part.Description = viewModel.Description;

        return Edit(part, context);
    }
}
