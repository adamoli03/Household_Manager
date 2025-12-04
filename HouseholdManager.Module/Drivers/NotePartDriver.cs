using HouseholdManager.Module.Models;
using HouseholdManager.Module.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace HouseholdManager.Module.Drivers;

public class NotePartDriver : ContentPartDisplayDriver<NotePart>
{
    public override IDisplayResult Display(NotePart part, BuildPartDisplayContext context)
    {
        return Initialize<NotePartViewModel>("NotePart", model =>
        {
            model.Title = part.Title;
            model.NoteContent = part.NoteContent;
            model.UserId = part.UserId;
            model.NotePart = part;
        })
        .Location("Detail", "Content:1")
        .Location("Summary", "Content:1");
    }

    public override IDisplayResult Edit(NotePart part, BuildPartEditorContext context)
    {
        return Initialize<NotePartViewModel>("NotePart_Edit", model =>
        {
            model.Title = part.Title;
            model.NoteContent = part.NoteContent;
            model.UserId = part.UserId;
            model.NotePart = part;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(NotePart part, UpdatePartEditorContext context)
    {
        var viewModel = new NotePartViewModel();

        await context.Updater.TryUpdateModelAsync(viewModel, Prefix);

        part.Title = viewModel.Title;
        part.NoteContent = viewModel.NoteContent;
        part.UserId = viewModel.UserId;

        return Edit(part, context);
    }
}
