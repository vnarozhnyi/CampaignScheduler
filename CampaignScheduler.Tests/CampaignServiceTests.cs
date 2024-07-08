using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampaignScheduler.Interfaces;
using CampaignScheduler.Models;
using CampaignScheduler.Services;
using Moq;

namespace CampaignScheduler.Tests
{
    public class CampaignServiceTests
    {
        private readonly Mock<ICampaignScheduler> _mockCampaignScheduler;
        private readonly Mock<ITemplateLoader> _mockTemplateLoader;
        private readonly Mock<ICustomerLoader> _mockCustomerLoader;
        private readonly CampaignService _campaignService;

        public CampaignServiceTests()
        {
            _mockCampaignScheduler = new Mock<ICampaignScheduler>();
            _mockTemplateLoader = new Mock<ITemplateLoader>();
            _mockCustomerLoader = new Mock<ICustomerLoader>();
            _campaignService = new CampaignService(
                _mockCampaignScheduler.Object,
                _mockTemplateLoader.Object,
                _mockCustomerLoader.Object);
        }

        [Fact]
        public async Task LoadAndScheduleCampaigns_LoadsTemplatesAndCustomers()
        {
            // Arrange
            _mockTemplateLoader.Setup(t => t.LoadTemplates()).Returns(new List<Template>
        {
            new Template { Name = "Template A", Content = "Content A" },
            new Template { Name = "Template B", Content = "Content B" }
        });

            _mockCustomerLoader.Setup(c => c.LoadCustomers(It.IsAny<string>())).Returns(new List<Customer>
        {
            new Customer { Id = 1, Age = 50, Gender = "Male", City = "New York", Deposit = 200, IsNewCustomer = false },
            new Customer { Id = 2, Age = 30, Gender = "Female", City = "Los Angeles", Deposit = 150, IsNewCustomer = true }
        });

            // Act
            await _campaignService.LoadAndScheduleCampaigns();

            // Assert
            _mockTemplateLoader.Verify(t => t.LoadTemplates(), Times.Once);
            _mockCustomerLoader.Verify(c => c.LoadCustomers(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoadAndScheduleCampaigns_SchedulesCampaignsCorrectly()
        {
            // Arrange
            _mockTemplateLoader.Setup(t => t.LoadTemplates()).Returns(new List<Template>
        {
            new Template { Name = "Template A", Content = "Content A" },
            new Template { Name = "Template B", Content = "Content B" }
        });

            var customers = new List<Customer>
        {
            new Customer { Id = 1, Age = 50, Gender = "Male", City = "New York", Deposit = 200, IsNewCustomer = false },
            new Customer { Id = 2, Age = 30, Gender = "Female", City = "New York", Deposit = 150, IsNewCustomer = true },
            new Customer { Id = 3, Age = 30, Gender = "Male", City = "Los Angeles", Deposit = 50, IsNewCustomer = true }
        };

            _mockCustomerLoader.Setup(c => c.LoadCustomers(It.IsAny<string>())).Returns(customers);

            // Act
            await _campaignService.LoadAndScheduleCampaigns();

            // Assert
            _mockCampaignScheduler.Verify(cs => cs.ScheduleCampaign(It.IsAny<Campaign>(), It.IsAny<Customer>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task LoadAndScheduleCampaigns_OnlySchedulesUniqueCustomers()
        {
            // Arrange
            _mockTemplateLoader.Setup(t => t.LoadTemplates()).Returns(new List<Template>
        {
            new Template { Name = "Template A", Content = "Content A" },
            new Template { Name = "Template B", Content = "Content B" }
        });

            var customers = new List<Customer>
        {
            new Customer { Id = 1, Age = 50, Gender = "Male", City = "New York", Deposit = 200, IsNewCustomer = false },
            new Customer { Id = 2, Age = 30, Gender = "Female", City = "Los Angeles", Deposit = 150, IsNewCustomer = true }
        };

            _mockCustomerLoader.Setup(c => c.LoadCustomers(It.IsAny<string>())).Returns(customers);

            // Act
            await _campaignService.LoadAndScheduleCampaigns();

            // Assert
            _mockCampaignScheduler.Verify(cs => cs.ScheduleCampaign(It.IsAny<Campaign>(), customers[0]), Times.Once);
            _mockCampaignScheduler.Verify(cs => cs.ScheduleCampaign(It.IsAny<Campaign>(), customers[1]), Times.Once);
        }

        [Fact]
        public async Task LoadAndScheduleCampaigns_ThrowsExceptionWhenSchedulerFails()
        {
            // Arrange
            _mockTemplateLoader.Setup(t => t.LoadTemplates()).Returns(new List<Template>
        {
            new Template { Name = "Template A", Content = "Content A" },
            new Template { Name = "Template B", Content = "Content B" }
        });

            var customers = new List<Customer>
        {
            new Customer { Id = 1, Age = 50, Gender = "Male", City = "New York", Deposit = 200, IsNewCustomer = false },
            new Customer { Id = 2, Age = 30, Gender = "Female", City = "Los Angeles", Deposit = 150, IsNewCustomer = true }
        };

            _mockCustomerLoader.Setup(c => c.LoadCustomers(It.IsAny<string>())).Returns(customers);

            _mockCampaignScheduler
                .Setup(cs => cs.ScheduleCampaign(It.IsAny<Campaign>(), It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("Scheduler error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _campaignService.LoadAndScheduleCampaigns());
        }
    }
}
