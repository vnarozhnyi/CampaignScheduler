using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CampaignScheduler.Models;
using CampaignScheduler.Services;
using Moq;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Xunit;

namespace CampaignScheduler.Tests
{
    public class CampaignSchedulerServiceTests
    {
        private readonly Mock<IScheduler> _mockScheduler;
        private readonly Mock<Quartz.ISchedulerFactory> _mockFactory;
        private readonly CampaignSchedulerService _service;

        public CampaignSchedulerServiceTests()
        {
            _mockScheduler = new Mock<IScheduler>();
            _mockFactory = new Mock<Quartz.ISchedulerFactory>();
            _mockFactory.Setup(f => f.GetScheduler(default)).Returns(Task.FromResult(_mockScheduler.Object));

            _service = new CampaignSchedulerService(_mockFactory.Object);
        }

        [Fact]
        public async Task ScheduleCampaign_ShouldScheduleJob()
        {
            // Arrange
            var campaign = new Campaign
            {
                TemplateName = "TestTemplate",
                SendTime = DateTime.UtcNow.AddMinutes(1),
                Priority = 1
            };
            var customer = new Customer
            {
                Id = 1
            };

            // Act
            await _service.ScheduleCampaign(campaign, customer);

            // Assert
            _mockScheduler.Verify(s => s.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), default), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenSchedulerInitializationFails()
        {
            // Arrange
            _mockFactory.Setup(f => f.GetScheduler(default)).Throws(new Exception("Scheduler initialization failed"));

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => new CampaignSchedulerService(_mockFactory.Object));
            Assert.Contains("Scheduler initialization failed", exception.Message);
        }

        [Fact]
        public async Task ScheduleCampaign_ShouldThrowException_WhenCampaignIsNull()
        {
            // Arrange
            Campaign campaign = null;
            var customer = new Customer
            {
                Id = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.ScheduleCampaign(campaign, customer));
        }

        [Fact]
        public async Task ScheduleCampaign_ShouldThrowException_WhenCustomerIsNull()
        {
            // Arrange
            var campaign = new Campaign
            {
                TemplateName = "TestTemplate",
                SendTime = DateTime.UtcNow.AddMinutes(1),
                Priority = 1
            };
            Customer customer = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.ScheduleCampaign(campaign, customer));
        }

        [Fact]
        public void SendCampaignJob_ShouldHandleFileIOException()
        {
            // Arrange
            var job = new CampaignSchedulerService.SendCampaignJob();
            var context = new Mock<IJobExecutionContext>();
            var jobDataMap = new JobDataMap
            {
                { "templateName", "TestTemplate" },
                { "sendTime", DateTime.UtcNow.ToString("o") },
                { "customerId", "1" },
                { "priority", "1" }
            };

            context.Setup(c => c.JobDetail.JobDataMap).Returns(jobDataMap);

            // Act & Assert
            var path = $"sends_{DateTime.UtcNow:yyyyMMdd}.txt";
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var exception = Assert.Throws<IOException>(() => job.Execute(context.Object).Wait());
                Assert.Contains("The process cannot access the file", exception.Message);
            }
        }
    }
}
