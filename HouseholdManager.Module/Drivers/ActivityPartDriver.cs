using HouseholdManager.Module.Models;
using HouseholdManager.Module.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace HouseholdManager.Module.Drivers;

public class ActivityPartDriver : ContentPartDisplayDriver<ActivityPart>
{
    public override IDisplayResult Display(ActivityPart part, BuildPartDisplayContext context)
    {
        return Initialize<ActivityPartViewModel>("ActivityPart", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.RoomType = part.RoomType;
            model.AssignedUserId = part.AssignedUserId;
            model.IsCompleted = part.IsCompleted;
            model.CompletedDate = part.CompletedDate;
            model.CompletedByUserId = part.CompletedByUserId;
            model.ActivityPart = part;
        })
        .Location("Detail", "Content:1")
        .Location("Summary", "Content:1");
    }

    public override IDisplayResult Edit(ActivityPart part, BuildPartEditorContext context)
    {
        return Initialize<ActivityPartViewModel>("ActivityPart_Edit", model =>
        {
            model.Title = part.Title;
            model.Description = part.Description;
            model.RoomType = part.RoomType;
            model.AssignedUserId = part.AssignedUserId;
            model.IsCompleted = part.IsCompleted;
            model.CompletedDate = part.CompletedDate;
            model.CompletedByUserId = part.CompletedByUserId;
            model.ActivityPart = part;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(ActivityPart part, UpdatePartEditorContext context)
    {
        var viewModel = new ActivityPartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Title = viewModel.Title;
        part.Description = viewModel.Description;
        part.RoomType = viewModel.RoomType;
        part.AssignedUserId = viewModel.AssignedUserId;
        part.IsCompleted = viewModel.IsCompleted;
        part.CompletedDate = viewModel.CompletedDate;
        part.CompletedByUserId = viewModel.CompletedByUserId;

        return Edit(part, context);
    }
}
