using CampaignScheduler.Interfaces;
using CampaignScheduler.Models;

namespace CampaignScheduler.Services;

public class TemplateLoaderService : ITemplateLoader
{
    /// <summary>
    /// Loads templates from the specified files.
    /// </summary>
    /// <returns>A list of loaded templates.</returns>
    public List<Template> LoadTemplates()
    {
        try
        {
            return new List<Template>
            {
                new Template { Name = "Template A", Content = File.ReadAllText("Templates/TemplateA.html") },
                new Template { Name = "Template B", Content = File.ReadAllText("Templates/TemplateB.html") },
                new Template { Name = "Template C", Content = File.ReadAllText("Templates/TemplateC.html") }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading templates: {ex.Message}");
            throw;
        }
    }
}