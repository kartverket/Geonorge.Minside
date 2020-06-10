using Geonorge.MinSide.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                TimeSpan.FromMinutes(25));

            return Task.CompletedTask;
        }

        private void NotifyToDoDeadline(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<OrganizationContext>();

                _logger.LogInformation("Timed Background Service is working.");
                var notifications = _context.Todo.Where(d => DateTime.Now > d.Deadline.AddDays(-2)).ToList();
                foreach(var notification in notifications)
                {
                    _logger.LogInformation("Deadline "+notification.Deadline+" expired for " + notification.Description);
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