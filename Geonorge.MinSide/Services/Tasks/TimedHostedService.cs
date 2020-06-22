using Geonorge.MinSide.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MailKit.Net.Smtp;
using MimeKit;
using Geonorge.MinSide.Models;

namespace Geonorge.MinSide.Services.Tasks
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(NotifyToDoDeadline, null, TimeSpan.Zero,
               TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void NotifyToDoDeadline(object state)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<OrganizationContext>();
                var _applicationSettings = scope.ServiceProvider.GetRequiredService<ApplicationSettings>();

                _logger.LogInformation("Timed Background Service is working.");

                var sql = "SELECT convert(varchar, Todo.Number) + ' ' + Todo.Subject as Subject, Todo.Deadline, UserSettings.Email FROM Todo INNER JOIN UserSettings ON Todo.ResponsibleOrganization = UserSettings.Organization Where status != 'Utført' and UserSettings.TodoReminder = 1 and UserSettings.Email is not null and DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) = DATEADD(DAY, -UserSettings.TodoReminderTime, Deadline)";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;

                    _context.Database.OpenConnection();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try {
                                string title = reader.GetString(0);
                                DateTime deadlineFromDb = reader.GetDateTime(1);
                                string deadline = deadlineFromDb.ToShortDateString();
                                string email = reader.GetString(2);

                                _logger.LogInformation("Deadline " + deadline + " expired for " + title + ", sending email to " + email);

                                MimeMessage message = new MimeMessage();

                                MailboxAddress from = MailboxAddress.Parse(_applicationSettings.WebmasterEmail);
                                message.From.Add(from);

                                MailboxAddress to = MailboxAddress.Parse(email);
                                message.To.Add(to);

                                message.Subject = "Påminnelse: Geonorge min side oppfølgingspunkt";
                                string body = "Oppfølgingspunkt: " + title + " har frist " + deadline;
                                BodyBuilder bodyBuilder = new BodyBuilder();
                                bodyBuilder.HtmlBody = body;
                                bodyBuilder.TextBody = body;

                                message.Body = bodyBuilder.ToMessageBody();

                                SmtpClient client = new SmtpClient();
                                client.Connect(_applicationSettings.SmtpHost);

                                client.Send(message);
                                client.Disconnect(true);
                                client.Dispose();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }

                        }
                        reader.Close();
                    }
                    _context.Database.CloseConnection();
                }    
             }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}