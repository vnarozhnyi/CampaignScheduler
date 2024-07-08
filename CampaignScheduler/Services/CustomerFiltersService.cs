using CampaignScheduler.Models;

namespace CampaignScheduler.Services;

/// <summary>
/// Provides various customer filter functions.
/// </summary>
public static class CustomerFiltersService
{
    public static Func<Customer, bool> MaleCustomers = c => c.Gender == "Male";
    public static Func<Customer, bool> CustomersAbove45 = c => c.Age > 45;
    public static Func<Customer, bool> CustomersInNewYork = c => c.City == "New York";
    public static Func<Customer, bool> CustomersDepositMoreThan100 = c => c.Deposit > 100;
    public static Func<Customer, bool> NewCustomers = c => c.IsNewCustomer;
}