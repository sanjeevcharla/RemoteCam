using Microsoft.AspNetCore.SignalR;
using Quartz;
using RemoteCam.Hubs;
using RemoteCam.Pi;
using System;
using System.Threading.Tasks;

namespace RemoteCam.Jobs
{
    public class CaptureJob : IJob
    {
        IHubContext<CamHub> hubContext;
        public CaptureJob(IHubContext<CamHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Camera.TakePicture();
            hubContext.Clients.All.SendAsync("capture");
            return Task.FromResult(true);
        }
    }
}
