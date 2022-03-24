using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DashboardApp.Services
{
    public interface ISignalHub
    {
        Task ReceiveNotification();
    }

    public class SignalHub : Hub<ISignalHub>
    {

    }
}
