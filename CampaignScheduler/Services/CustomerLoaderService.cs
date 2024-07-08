using CampaignScheduler.Interfaces;
using CampaignScheduler.Models;
using System.Globalization;

namespace CampaignScheduler.Services;

/// <summary>
/// Service for loading customers from a CSV file.
/// </summary>
public class CustomerLoaderService : ICustomerLoader
{
    /// <summary>
    /// Loads customers from the specified CSV file.
    /// </summary>
    /// <param name="filePath">The path to the CSV file.</param>
    /// <returns>A list of loaded customers.</returns>
    /// <exception cref="FormatException">Thrown when the CSV format is incorrect.</exception>
    public List<Customer> LoadCustomers(string filePath)
    {
        var customers = new List<Customer>();

        try
        {
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var values = line.Split(',');

                if (values.Length != 6)
                {
                    throw new FormatException("CSV line does not contain exactly 6 values.");
                }

                try
                {
                    customers.Add(new Customer
                    {
                        Id = int.Parse(values[0]),
                        Age = int.Parse(values[1]),
                        Gender = values[2],
                        City = values[3],
                        Deposit = decimal.Parse(values[4], CultureInfo.InvariantCulture),
                        IsNewCustomer = values[5] == "1"
                    });
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Error parsing line: {line}. Error: {ex.Message}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IO error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }

        return customers;
    }
}