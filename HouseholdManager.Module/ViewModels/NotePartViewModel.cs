using HouseholdManager.Module.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Module.ViewModels;

public class NotePartViewModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string NoteContent { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;

    [BindNever]
    public NotePart? NotePart { get; set; }
}
