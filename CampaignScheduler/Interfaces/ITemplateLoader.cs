using CampaignScheduler.Models;

namespace CampaignScheduler.Interfaces;

public interface ITemplateLoader
{
    List<Template> LoadTemplates();
}