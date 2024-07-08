namespace CampaignScheduler.Models;

public class Customer
{
    public int Id { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string City { get; set; }
    public decimal Deposit { get; set; }
    public bool IsNewCustomer { get; set; }
}