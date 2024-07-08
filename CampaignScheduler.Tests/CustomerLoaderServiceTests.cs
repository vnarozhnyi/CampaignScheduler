using CampaignScheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignScheduler.Tests
{

    public class CustomerLoaderServiceTests
    {
        private readonly CustomerLoaderService _customerLoaderService;

        public CustomerLoaderServiceTests()
        {
            _customerLoaderService = new CustomerLoaderService();
        }

        [Fact]
        public void LoadCustomers_SuccessfullyLoadsCustomers()
        {
            // Arrange
            var csvContent = "Id,Age,Gender,City,Deposit,IsNewCustomer\n" +
                             "1,30,Male,New York,100.50,1\n" +
                             "2,25,Female,Los Angeles,200.75,0";

            var filePath = SetupMockFile(csvContent);

            // Act
            var customers = _customerLoaderService.LoadCustomers(filePath);

            // Assert
            Assert.Equal(2, customers.Count);
            Assert.Equal(1, customers[0].Id);
            Assert.Equal(30, customers[0].Age);
            Assert.Equal("Male", customers[0].Gender);
            Assert.Equal("New York", customers[0].City);
            Assert.Equal(100.50m, customers[0].Deposit);
            Assert.True(customers[0].IsNewCustomer);

            Assert.Equal(2, customers[1].Id);
            Assert.Equal(25, customers[1].Age);
            Assert.Equal("Female", customers[1].Gender);
            Assert.Equal("Los Angeles", customers[1].City);
            Assert.Equal(200.75m, customers[1].Deposit);
            Assert.False(customers[1].IsNewCustomer);
        }

        [Fact]
        public void LoadCustomers_ThrowsFormatException_ForInvalidLineFormat()
        {
            // Arrange
            var csvContent = "Id,Age,Gender,City,Deposit,IsNewCustomer\n" +
                             "1,30,Male,New York,100.50,1\n" +
                             "2,25,Female,Los Angeles,200.75"; // Invalid line

            var filePath = SetupMockFile(csvContent);

            // Act & Assert
            var ex = Assert.Throws<FormatException>(() => _customerLoaderService.LoadCustomers(filePath));
            Assert.Contains("CSV line does not contain exactly 6 values.", ex.Message);
        }

        [Fact]
        public void LoadCustomers_ThrowsFormatException_ForInvalidData()
        {
            // Arrange
            var csvContent = "Id,Age,Gender,City,Deposit,IsNewCustomer\n" +
                             "1,30,Male,New York,invalid,1"; // Invalid deposit

            var filePath = SetupMockFile(csvContent);

            // Act & Assert
            var ex = Assert.Throws<FormatException>(() => _customerLoaderService.LoadCustomers(filePath));
            Assert.Contains("Error parsing line", ex.Message);
        }

        [Fact]
        public void LoadCustomers_ThrowsIOException_ForFileReadError()
        {
            // Arrange
            var filePath = "nonexistentfile.csv";

            // Act & Assert
            var ex = Assert.Throws<FileNotFoundException>(() => _customerLoaderService.LoadCustomers(filePath));
            Assert.Contains("Could not find file", ex.Message);
        }

        private string SetupMockFile(string content)
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}
