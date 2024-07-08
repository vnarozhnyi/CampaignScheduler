using CampaignScheduler.Models;

namespace CampaignScheduler.Interfaces;

public interface ICustomerLoader
{
    List<Customer> LoadCustomers(string filePath);
}