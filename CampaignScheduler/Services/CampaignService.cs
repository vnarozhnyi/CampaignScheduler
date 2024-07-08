using CampaignScheduler.Interfaces;
using CampaignScheduler.Models;

namespace CampaignScheduler.Services;

public class CampaignService : ICampaignService
{
    private readonly ICampaignScheduler _campaignScheduler;
    private readonly ITemplateLoader _templateLoader;
    private readonly ICustomerLoader _customerLoader;

    public CampaignService(ICampaignScheduler campaignScheduler, ITemplateLoader templateLoader, ICustomerLoader customerLoader)
    {
        _campaignScheduler = campaignScheduler ?? throw new ArgumentNullException(nameof(campaignScheduler));
        _templateLoader = templateLoader ?? throw new ArgumentNullException(nameof(templateLoader));
        _customerLoader = customerLoader ?? throw new ArgumentNullException(nameof(customerLoader));
    }

    /// <summary>
    /// Loads templates and customers, and schedules campaigns based on predefined conditions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadAndScheduleCampaigns()
    {
        try
        {
            List<Template> templates = _templateLoader.LoadTemplates();
            List<Customer> customers = _customerLoader.LoadCustomers("customers.csv");

            var campaigns = new List<Campaign>
                {
                    new Campaign { TemplateName = "Template A", Condition = CustomerFiltersService.MaleCustomers, SendTime = DateTime.Today.AddHours(10).AddMinutes(15), Priority = 1 },
                    new Campaign { TemplateName = "Template B", Condition = CustomerFiltersService.CustomersAbove45, SendTime = DateTime.Today.AddHours(10).AddMinutes(5), Priority = 2 },
                    new Campaign { TemplateName = "Template C", Condition = CustomerFiltersService.CustomersInNewYork, SendTime = DateTime.Today.AddHours(10).AddMinutes(10), Priority = 5 },
                    new Campaign { TemplateName = "Template A", Condition = CustomerFiltersService.CustomersDepositMoreThan100, SendTime = DateTime.Today.AddHours(10).AddMinutes(15), Priority = 3 },
                    new Campaign { TemplateName = "Template C", Condition = CustomerFiltersService.NewCustomers, SendTime = DateTime.Today.AddHours(10).AddMinutes(5), Priority = 4 }
                };

            var scheduledCustomers = new HashSet<int>();

            foreach (var campaign in campaigns.OrderBy(c => c.Priority))
            {
                var filteredCustomers = customers.Where(campaign.Condition).ToList();

                foreach (var customer in filteredCustomers)
                {
                    if (!scheduledCustomers.Contains(customer.Id))
                    {
                        scheduledCustomers.Add(customer.Id);
                        await _campaignScheduler.ScheduleCampaign(campaign, customer);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LoadAndScheduleCampaigns: {ex.Message}");
            throw;
        }
    }
}