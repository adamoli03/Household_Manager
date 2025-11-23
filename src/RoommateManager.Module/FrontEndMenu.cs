using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace RoommateManager.Module
{
    public class FrontEndMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public FrontEndMenu(IStringLocalizer<FrontEndMenu> localizer)
        {
            S = localizer;
        }

        public ValueTask BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "main", StringComparison.OrdinalIgnoreCase))
            {
                return ValueTask.CompletedTask;
            }

            builder
                .Add(S["My Dashboard"], "1", dashboard => dashboard
                    .Add(S["My Activities"], "1", activities => activities
                        .Action("MyActivities", "Activity", new { area = "RoommateManager.Module" })
                        .Permission(Permissions.ManageActivities)
                    )
                    .Add(S["Activity History"], "2", history => history
                        .Action("History", "Activity", new { area = "RoommateManager.Module" })
                        .Permission(Permissions.ManageActivities)
                    )
                    .Add(S["My Notes"], "3", notes => notes
                        .Action("Index", "Note", new { area = "RoommateManager.Module" })
                        .Permission(Permissions.ManageNotes)
                    )
                    .Add(S["Grocery List"], "4", grocery => grocery
                        .Action("Index", "Grocery", new { area = "RoommateManager.Module" })
                        .Permission(Permissions.ManageGroceryList)
                    )
                );

            return ValueTask.CompletedTask;
        }
    }
}