using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Contact.Monitoring.Web.Models;
using Contact.Monitoring.Web.Repository;
using Contact.Monitoring.Web.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Contact.Monitoring.Web.Controllers
{
    public class DataProviderController : Controller
    {
        public JsonResult GetSystemUpTime([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetAllSystemUpTimes();
            var result = queryResult
                .Select(s => new SystemUpTimeViewModel
                {
                    Service = s.Service,
                    LastBootUpTime = s.LastBootUpTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = s.MachineName
                });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSystemDiskUsage([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetAllSystemDiskUsage();
            var result = queryResult
                .Select(s => new SystemDiskSpaceViewModel
                {
                    Service = s.Service,
                    LastUpdatedDateTime = s.LastUpdatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = s.MachineName,
                    Drive = s.Drive,
                    FreeSpace = s.FreeSpace,
                    Size = s.Size
                });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalPushNotification([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetPeakCounterValueByService("Scheduler", "successful requests");
            var result = queryResult
                .Select(r => new PerformanceCounterDataViewModel
                {
                    Service = r.Service,
                    Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = r.MachineName,
                    Counter = "Push",
                    CounterValue = (double) r.CounterValue
                });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastNotification([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetLastCounterValueByService("Scheduler", "successful requests");
            var result = queryResult
                .Select(r => new PerformanceCounterDataViewModel
                {
                    Service = r.Service,
                    Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = r.MachineName,
                    Counter = "Push",
                    CounterValue = (double)r.CounterValue
                });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetLastSchedulerQueueCount([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetLastSchedulerQueueCount();
            var result = queryResult
                .Select(r => new SchedulerQueueCountViewModel
                {
                    LastUpdatedDateTime = r.LastUpdatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    QueueCount = r.QueueCount
                });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSystemStatus([DataSourceRequest] DataSourceRequest request)
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            var queryResult = _monitoringRepository.GetCounterValues(new[] { "% processor time", "available mbytes" }, last24Hours);
            var result = queryResult
                    .GroupBy(g => new { g.MachineName, g.Service, g.Counter },
                      (key, group) => new
                      {
                          key.Service,
                          key.MachineName,
                          key.Counter,
                          Timestamp = group.Max(p => p.Timestamp),
                          Value = group.Max(p => p.CounterValue)
                      })
                    .Select(s => new PerformanceCounterDataViewModel
                    {
                        Service = s.Service,
                        Timestamp = s.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        MachineName = s.MachineName,
                        Counter = s.Counter,
                        CounterValue = (double)s.Value
                    })
                    .ToList();

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSystemCurrentStatus([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = new List<PerformanceCounterData>
            {
                _monitoringRepository.GetLastCounterValue("WebApi", "% processor time"),
                _monitoringRepository.GetLastCounterValue("WebApi", "available mbytes"),
                _monitoringRepository.GetLastCounterValue("Scheduler", "% processor time"),
                _monitoringRepository.GetLastCounterValue("Scheduler", "available mbytes"),
                _monitoringRepository.GetLastCounterValue("Listener", "% processor time"),
                _monitoringRepository.GetLastCounterValue("Listener", "available mbytes"),
            };
            var result = queryResult.Select(s => new PerformanceCounterDataViewModel
                    {
                        Service = s.Service,
                        Timestamp = s.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        MachineName = s.MachineName,
                        Counter = s.Counter,
                        CounterValue = (double)s.CounterValue
                    })
                    .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllServiceStatus([DataSourceRequest] DataSourceRequest request)
        {
            var queryResult = _monitoringRepository.GetAllServiceStatus();
            var result = queryResult
                    
                    .Select(s => new ServiceStatuViewModel
                    {
                        Service = s.Service,
                        LastUpdatedDateTime = s.LastUpdatedDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        MachineName = s.MachineName,
                        Instance = s.Instance,
                        Status = s.Status
                    });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSystemActivity([DataSourceRequest] DataSourceRequest request)
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            var queryResult = _monitoringRepository.GetAllCounterValues(last24Hours);
            var result = queryResult
                .Select(s => new PerformanceCounterDataViewModel
                {
                    Service = s.Service,
                    Id = s.Id,
                    Timestamp = s.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = s.MachineName,
                    Counter = s.Counter,
                    CounterValue = (double) s.CounterValue
                });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCounterValuesByMachine(string counter)
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            var queryResult = _monitoringRepository.GetCounterValues(new[] { counter }, last24Hours);
            var result = queryResult
                .Select(s => new
                {
                    Time = s.Timestamp.ToString("yyyy-MM-dd HH"),
                    CounterValue = (double) s.CounterValue
                })
                .GroupBy(g => g.Time, (key, group) => new PerformanceCounterDataViewModel
                {
                    Timestamp = key,
                    CounterValue = group.Max(g => g.CounterValue)
                });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMemoryUsage()
        {
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            var queryResult = _monitoringRepository.GetCounterValues(new[] { "available mbytes" }, last24Hours);
            var result = queryResult
                .Select(s => new
                {
                    Time = s.Timestamp.ToString("yyyy-MM-dd HH"),
                    CounterValue = kMaxSystemMemoryMbytes - (double) s.CounterValue
                })
                .GroupBy(g => g.Time, (key, group) => new PerformanceCounterDataViewModel
                {
                    Timestamp = key,
                    CounterValue = group.Max(g => g.CounterValue)
                });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private readonly MonitoringRepository _monitoringRepository = new MonitoringRepository();
        private readonly int kMaxSystemMemoryMbytes = 8000;
    }
}