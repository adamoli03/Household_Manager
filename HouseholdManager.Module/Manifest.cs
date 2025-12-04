using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Household Manager Module",
    Author = "IEC77C",
    Website = "https://householdmanager.example.com",
    Version = "1.0.0",
    Description = "A household management module for tracking activities, notes, and shared grocery lists.",
    Category = "Content Management",
    Dependencies = new[] {
        "OrchardCore.Contents",
        "OrchardCore.ContentTypes"
    }
)]
