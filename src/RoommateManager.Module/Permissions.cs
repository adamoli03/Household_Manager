using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoommateManager.Module
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageActivities = new Permission("ManageActivities", "Manage Activities");
        public static readonly Permission ManageNotes = new Permission("ManageNotes", "Manage Private Notes");
        public static readonly Permission ManageGroceryList = new Permission("ManageGroceryList", "Manage Grocery List");
        public static readonly Permission ManageRooms = new Permission("ManageRooms", "Manage Rooms");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[]
            {
                ManageActivities,
                ManageNotes,
                ManageGroceryList,
                ManageRooms
            }.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[] 
                    { 
                        ManageActivities, 
                        ManageNotes, 
                        ManageGroceryList, 
                        ManageRooms 
                    }
                },
                new PermissionStereotype
                {
                    Name = "Authenticated",
                    Permissions = new[] 
                    { 
                        ManageActivities, 
                        ManageNotes, 
                        ManageGroceryList 
                    }
                }
            };
        }
    }
}