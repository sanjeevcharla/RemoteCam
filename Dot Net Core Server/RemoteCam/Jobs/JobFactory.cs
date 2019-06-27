using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Spi;
using System;

namespace RemoteCam.Jobs
{
    public class JobFactory : IJobFactory
    {
        protected readonly IApplicationBuilder appBuilder;
        public JobFactory(IApplicationBuilder serviceProvider)
        {
            this.appBuilder = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return appBuilder.ApplicationServices.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
