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


        public async Task StartAsync(CancellationToken cancellationToken)
        {

            int waitingMinutes = 60 - DateTime.Now.Minute;
            await startTask("FTP", waitingMinutes, TimeSpan.FromHours(1));
            await startTask("Database", waitingMinutes, TimeSpan.FromMinutes(30));
        }

        private async Task startTask(string type, int watingTime, TimeSpan timeSpan)
        {
            _timer = new Timer(DoWork, type, TimeSpan.FromMinutes(watingTime), timeSpan);
        }

        private async void DoWork(object? typeName)
        {
            using (var scope = _services.CreateScope())
            {
                var dataService =
                    scope.ServiceProvider
                        .GetRequiredService<IDataService>();

                switch (typeName?.ToString())
                {
                    case "FTP":
                        await dataService.NotifyFTPDataHasChanged();
                        break;
                    case "Database":
                        await dataService.NotifyDatabaseDataHasChanged();
                        break;
                }


            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
