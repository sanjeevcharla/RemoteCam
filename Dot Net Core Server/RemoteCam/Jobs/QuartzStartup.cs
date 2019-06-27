using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Impl;
using System;

namespace RemoteCam.Jobs
{
    public class QuartzStartup
    {
        private IScheduler scheduler;
        private readonly IApplicationBuilder applicationBuilder;

        public QuartzStartup(IApplicationBuilder applicationBuilder)
        {
            this.applicationBuilder = applicationBuilder;
        }

        public void Start()
        {
            if (scheduler == null)
            {
                scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                scheduler.JobFactory = new JobFactory(applicationBuilder);

                #region Capture Job
                var captureJobDetail = JobBuilder
                            .Create<CaptureJob>()
                            .WithIdentity("CaptureJob")
                            .Build();

                var captureJobTrigger = TriggerBuilder
                    .Create()
                    .WithIdentity("CaptureJobTrigger")
                    .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(5))
                    .ForJob("CaptureJob")
                    .Build();

                scheduler.ScheduleJob(captureJobDetail, captureJobTrigger).Wait();
                #endregion

                #region Watch Inbox Job
                var watchInboxJobDetail = JobBuilder
                           .Create<WatchInboxJob>()
                           .WithIdentity("WatchInboxJob")
                           .Build();

                var watchInboxJobTrigger = TriggerBuilder
                    .Create()
                    .WithIdentity("WatchInboxJobTrigger")
                    .WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(5))
                    .ForJob("WatchInboxJob")
                    .Build();

                scheduler.ScheduleJob(watchInboxJobDetail, watchInboxJobTrigger).Wait();

                scheduler.Start().Wait();
                #endregion

            }
        }

        public void Stop()
        {
            if (scheduler != null)
                scheduler.Shutdown(waitForJobsToComplete: true).Wait();
        }
    }
}
