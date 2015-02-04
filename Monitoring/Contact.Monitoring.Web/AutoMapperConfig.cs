using System;
using AutoMapper;
using Contact.Monitoring.Models;
using Contact.Monitoring.Services;
using Contact.Monitoring.ViewModel;

namespace Contact.Monitoring.Web
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.CreateMap<DateTime, string>().ConvertUsing((s) => s.ToString(DataTimeFormatString));
            Mapper.CreateMap<decimal, string>().ConvertUsing((s) => s.ToString("N2"));

            Mapper.CreateMap<AddressBook, AddressBookViewModel>();
            Mapper.CreateMap<PerformanceCounterData, PerformanceCounterDataViewModel>();
            Mapper.CreateMap<SchedulerQueue, SchedulerQueueViewModel>();
            Mapper.CreateMap<SchedulerQueuePending, SchedulerQueuePendingViewModel>();
            Mapper.CreateMap<ServiceStatu, ServiceStatuViewModel>();
            Mapper.CreateMap<SystemUpTime, SystemUpTimeViewModel>();
            Mapper.CreateMap<SystemDiskSpace, SystemDiskSpaceViewModel>();
            Mapper.CreateMap<MaxPerformanceCounterData, MaxPerformanceCounterDataViewModel>();

        }

        private const string DataTimeFormatString = "yyyy-MM-dd HH:mm:ss";
    }
}