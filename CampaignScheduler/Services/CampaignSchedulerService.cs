using CampaignScheduler.Interfaces;
using CampaignScheduler.Models;
using Quartz;
using Quartz.Impl;

namespace CampaignScheduler.Services;

public class CampaignSchedulerService : ICampaignScheduler
{
    private readonly IScheduler _scheduler;

    public CampaignSchedulerService(ISchedulerFactory schedulerFactory)
    {
        try
        {
            _scheduler = schedulerFactory.GetScheduler().Result;
            _scheduler.Start().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing scheduler: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Schedules a campaign for a specific customer.
    /// </summary>
    /// <param name="campaign">The campaign to schedule.</param>
    /// <param name="customer">The customer to whom the campaign will be sent.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ScheduleCampaign(Campaign campaign, Customer customer)
    {
        try
        {
            var jobId = $"{campaign.TemplateName}_{customer.Id}_{campaign.SendTime:yyyyMMddHHmmss}_{campaign.Priority}";

            IJobDetail job = JobBuilder.Create<SendCampaignJob>()
                .WithIdentity(jobId, "Campaigns")
                .UsingJobData("templateName", campaign.TemplateName)
                .UsingJobData("sendTime", campaign.SendTime.ToString("o"))
                .UsingJobData("customerId", customer.Id.ToString())
                .UsingJobData("priority", campaign.Priority.ToString())
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobId, "Campaigns")
                .StartAt(campaign.SendTime)
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
        catch (SchedulerException ex)
        {
            Console.WriteLine($"Error scheduling job: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    public class SendCampaignJob : IJob
    {
        private static readonly object fileLock = new object();

        /// <summary>
        /// Executes the campaign job.
        /// </summary>
        /// <param name="context">The job execution context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task Execute(IJobExecutionContext context)
        {
            string templateName = context.JobDetail.JobDataMap.GetString("templateName");
            string sendTime = context.JobDetail.JobDataMap.GetString("sendTime");
            string customerId = context.JobDetail.JobDataMap.GetString("customerId");
            string priority = context.JobDetail.JobDataMap.GetString("priority");

            try
            {
                lock (fileLock)
                {
                    // Simulate sending the campaign by writing to a file
                    string path = $"sends_{DateTime.Parse(sendTime):yyyyMMdd}.txt";
                    File.AppendAllText(path, $"Sent {templateName} to customer ID {customerId} with priority {priority} at {sendTime}\n");
                }

                // Simulate 30 minutes wait
                Task.Delay(TimeSpan.FromMinutes(30)).Wait();

                return Task.CompletedTask;
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
        }
    }
}