using CampaignScheduler.Interfaces;
using CampaignScheduler.Services;
using Quartz.Impl;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
    var scheduler = schedulerFactory.GetScheduler().Result;
    scheduler.Start().Wait();
    return scheduler;
});

builder.Services.AddSingleton<ICampaignScheduler, CampaignSchedulerService>();
builder.Services.AddSingleton<ICampaignService, CampaignService>();
builder.Services.AddSingleton<ITemplateLoader, TemplateLoaderService>();
builder.Services.AddSingleton<ICustomerLoader, CustomerLoaderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var campaignService = app.Services.GetRequiredService<ICampaignService>();
campaignService.LoadAndScheduleCampaigns().Wait();

app.Run();
