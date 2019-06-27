using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RemoteCam.Hubs
{
    public class CamHub : Hub
    {
        public Task Capture()
        {
            return Clients.All.SendAsync("capture");
        }
    }
}
