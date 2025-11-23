using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace RoommateManager.Module
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            S = localizer;
        }

        // Changed return type from Task to ValueTask
        public ValueTask BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return ValueTask.CompletedTask;
            }

            builder
                .Add(S["Roommate Manager"], "10", roommate => roommate
                    .AddClass("roommate-manager").Id("roommate-manager")
                    .Add(S["Activities"], "1", activities => activities
                        .Action("Index", "Admin", new { area = "OrchardCore.Contents", contentTypeId = "Activity" })
                        .Permission(Permissions.ManageActivities)
                        .LocalNav()
                    )
                    .Add(S["Rooms"], "2", rooms => rooms
                        .Action("Index", "Admin", new { area = "OrchardCore.Contents", contentTypeId = "Room" })
                        .Permission(Permissions.ManageRooms)
                        .LocalNav()
                    )
                    .Add(S["Grocery Items"], "3", grocery => grocery
                        .Action("Index", "Admin", new { area = "OrchardCore.Contents", contentTypeId = "GroceryItem" })
                        .Permission(Permissions.ManageGroceryList)
                        .LocalNav()
                    )
                );

            return ValueTask.CompletedTask;
        }
    }
}