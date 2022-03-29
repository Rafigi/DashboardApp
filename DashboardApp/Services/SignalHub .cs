using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DashboardApp.Services
{
    public interface ISignalHub
    {
        Task NotificationFtpChange();
        Task NotificatioDatabaseChange();
    }

    public class SignalHub : Hub<ISignalHub>
    {

    }
}
