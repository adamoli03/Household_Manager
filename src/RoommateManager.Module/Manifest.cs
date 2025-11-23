using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Roommate Manager",
    Author = "Your Name",
    Website = "https://orchardcore.net",
    Version = "1.0.0",
    Description = "A management system for roommates to track activities, notes, and grocery lists.",
    Category = "Content Management",
    Dependencies = new[]
    {
        "OrchardCore.ContentManagement",
        "OrchardCore.Contents",
        "OrchardCore.Mvc"
    }
)]