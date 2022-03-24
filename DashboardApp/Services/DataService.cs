using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DashboardApp.Services
{
    public interface IDataService
    {
        Task NofifyDashBoardDataHasChanged();
    }
    public class DataService : IDataService
    {
        private readonly IHubContext<SignalHub, ISignalHub> _context;

        public DataService(IHubContext<SignalHub, ISignalHub> context)
        {
            _context = context;
        }

        public async Task NofifyDashBoardDataHasChanged()
        {
           await _context.Clients.All.ReceiveNotification();
        }
    }
}
