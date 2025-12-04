using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Module.ViewModels;

public class ActivityPartViewModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string AssignedUserId { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedByUserId { get; set; }

    [BindNever]
    public ActivityPart? ActivityPart { get; set; }
}
