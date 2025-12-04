using HouseholdManager.Module.Drivers;
using HouseholdManager.Module.Handlers;
using HouseholdManager.Module.Indexes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace HouseholdManager.Module;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddContentPart<Models.ActivityPart>()
            .UseDisplayDriver<ActivityPartDriver>()
            .AddHandler<ActivityPartHandler>();

        services.AddContentPart<Models.RoomPart>()
            .UseDisplayDriver<RoomPartDriver>();

        services.AddContentPart<Models.NotePart>()
            .UseDisplayDriver<NotePartDriver>();

        services.AddContentPart<Models.GroceryItemPart>()
            .UseDisplayDriver<GroceryItemPartDriver>();


        services.AddScoped<IDataMigration, Migrations.Migrations>();
    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
    {
        // Used attribute routing instead
    }
}

