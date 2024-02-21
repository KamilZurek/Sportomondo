using Microsoft.EntityFrameworkCore;
using Sportomondo.Api.Context;
using Sportomondo.Api.Helpers;
using Sportomondo.Api.Models;
using Sportomondo.Api.Services;
using System.Data;

namespace Sportomondo.Api.BackgroundServices
{
    public class ActivitySeriesReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILogger<ActivitySeriesReminderService> _logger;
        private readonly string ReminderName = "ActivitySeriesReminder";

        public ActivitySeriesReminderService(IServiceProvider serviceProvider, IEmailSenderService emailSenderService, ILogger<ActivitySeriesReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _emailSenderService = emailSenderService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RemindAboutActivitySeries(stoppingToken);
                    
                    await Task.Delay(CalculateDelay(), stoppingToken); //1 iteration per 60 minutes, every hour o'clock
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"BackgroundService stopped: {ex}");

                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"BackgroundService error: {ex}");

                    await Task.Delay(new TimeSpan(0, 1, 0), stoppingToken); //1 minute waiting
                }
            }
        }

        private async Task RemindAboutActivitySeries(CancellationToken stoppingToken = default)
        {
            var emailsToSend = new List<EmailTemplate>();
            var isReminderActive = false;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<SportomondoDbContext>();

                var reminder = await GetReminderIfAppropriateTime(dbContext, stoppingToken); //returns null if NOT

                if (reminder != null)
                {
                    isReminderActive = true;

                    foreach (var user in await dbContext.Users
                                            .Include(u => u.Activities)
                                            .ToListAsync(stoppingToken))
                    {
                        var emailTemplate = GetEmailTemplateIfUserInSeries(user);

                        if (emailTemplate != null)
                        {
                            emailsToSend.Add(emailTemplate);
                        }
                    }
                }
            }

            if (emailsToSend.Any())
            {
                await _emailSenderService.SendEmailsAsync(emailsToSend);
            }

            _logger.LogInformation($"{ReminderName} on {DateTime.Now} was active: {isReminderActive} and sent {emailsToSend.Count} email(s) " +
                    $"to users: {string.Join("; ", emailsToSend.Select(e => e.To))}");
        }

        private async Task<Reminder> GetReminderIfAppropriateTime(SportomondoDbContext dbContext, CancellationToken stoppingToken = default)
        {
            var currentHour = DateTime.Now.Hour; //prevents database delays?
            var currentMinute = DateTime.Now.Minute; //prevents database delays?

            var activeReminders = await dbContext.Reminders
                                            .Where(r => r.Type == this.ReminderName && r.Enabled)
                                            .ToListAsync(stoppingToken);

            foreach (var reminder in activeReminders)
            {
                if (reminder.Time.Hours == currentHour
                    && reminder.Time.Minutes == currentMinute)
                {
                    return reminder;
                }
            }

            return null;
        }

        private EmailTemplate GetEmailTemplateIfUserInSeries(User user)
        {
            var dateNow = DateTime.Now.Date;

            var userHasActivityToday = user.Activities
                                             .Any(a => a.DateStart.Date == dateNow);

            var seriesCount = GetSeriesCount(user, userHasActivityToday, dateNow); //"Series" mode is active for 3 days of activity in a row

            if (userHasActivityToday)
            {
                if (seriesCount >= 3) //activities done: today, yesterday, the day before yesterday (...) - "Series" is complete
                {
                    return new EmailTemplate()
                    {
                        To = user.Email,
                        Subject = "Sportomondo - activity series reminder",
                        Body = $"Hi {user.FirstName} 👋,\n\nCongratulations - You have series of {seriesCount} activities in a row.\n" +
                            $"Keep it up!\n\nSportomondo TEAM"
                    };
                }
            }
            else
            {
                if (seriesCount >= 2) //activities done: yesterday, the day before yesterday (...), but not today - "Series" is at risk
                {
                    return new EmailTemplate()
                    {
                        To = user.Email,
                        Subject = "Sportomondo - activity series reminder",
                        Body = $"Hi {user.FirstName} 👋,\n\nIn previous days You had series of {seriesCount} activities in a row.\n" +
                            $"Do not waste it - We are counting on You!\n\nSportomondo TEAM"
                    };
                }
            }

            return null;
        }

        private int GetSeriesCount(User user, bool userHasActivityToday, DateTime baseDate)
        {
            var seriesBreak = false;
            var dateBefore = baseDate;
            var seriesCount = userHasActivityToday ? 1 : 0; //if true - start value is "1", else "0"

            while (!seriesBreak)
            {
                dateBefore = dateBefore.AddDays(-1);

                var userHasActivityDayBefore = user.Activities
                                                        .Any(a => a.DateStart.Date == dateBefore);

                if (userHasActivityDayBefore)
                {
                    seriesCount++;
                }
                else
                {
                    seriesBreak = true;
                }
            }

            return seriesCount;
        }

        private TimeSpan CalculateDelay()
        {
            var nextInvoke = DateTime.Now.AddHours(1);
            nextInvoke = nextInvoke.AddMinutes(-nextInvoke.Minute);
            nextInvoke = nextInvoke.AddSeconds(-nextInvoke.Second);

            var delay = nextInvoke - DateTime.Now;

            return delay;
        }
    }
}
