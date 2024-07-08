using CampaignScheduler.Models;

namespace CampaignScheduler.Interfaces;

public interface ICampaignScheduler
{
    Task ScheduleCampaign(Campaign campaign, Customer customers);
}