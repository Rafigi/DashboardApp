using System;
using System.Threading;
using System.Threading.Tasks;
using DashboardApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DashboardApp.BackgroundService
{
    public class TimedHostedService : IHostedService
    {
        private readonly IServiceProvider _services;
        private Timer _timer = null!;

        public TimedHostedService(IServiceProvider services)
        {
            _services = services;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            int waitingMinutes = 60 - DateTime.Now.Minute;
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(waitingMinutes), TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using (var scope = _services.CreateScope())
            {
                var dataService =
                    scope.ServiceProvider
                        .GetRequiredService<IDataService>();

                await dataService.NotifyDashBoardDataHasChanged();
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
