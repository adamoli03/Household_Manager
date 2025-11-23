using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using RoommateManager.Module.Models;

namespace RoommateManager.Module
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            
            services.AddContentPart<ActivityPart>();
            services.AddContentPart<NotePart>();
            services.AddContentPart<RoomPart>();
            services.AddContentPart<GroceryItemPart>();

            services.AddScoped<IDataMigration, Migrations>();

            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<INavigationProvider, FrontEndMenu>();
        }

    }
}