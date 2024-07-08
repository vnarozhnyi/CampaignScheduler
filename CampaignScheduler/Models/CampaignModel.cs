namespace CampaignScheduler.Models;

public class Campaign
{
    public string TemplateName { get; set; }
    public Func<Customer, bool> Condition { get; set; }
    public DateTime SendTime { get; set; }
    public int Priority { get; set; }
}