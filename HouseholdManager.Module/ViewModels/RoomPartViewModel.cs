using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Module.ViewModels;

public class RoomPartViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [BindNever]
    public RoomPart? RoomPart { get; set; }
}
