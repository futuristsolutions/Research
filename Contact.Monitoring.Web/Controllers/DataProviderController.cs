using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Contact.Monitoring.Web.Models;
using Contact.Monitoring.Web.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Contact.Monitoring.Web.Controllers
{
    public class DataProviderController : Controller
    {

        public JsonResult GetSystemUpTime([DataSourceRequest] DataSourceRequest request)
        {
            var context = new MonitoringContext();
            var result = context.SystemUpTimes
                .ToList()
                .Select(s => new SystemUpTimeViewModel
            {
                Service = s.Service,
                LastBootUpTime = s.LastBootUpTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MachineName = s.MachineName
            });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSystemActivity([DataSourceRequest] DataSourceRequest request)
        {
            var context = new MonitoringContext();
            var result = context.PerformanceCounterDatas
                .ToList()
                .Select(s => new PerformanceCounterDataViewModel
                {
                    Service = s.Service,
                    Id = s.Id,
                    Timestamp = s.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    MachineName = s.MachineName,
                    Counter = s.Counter,
                    Value = s.CounterValue
                });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSchedulerStatus([DataSourceRequest] DataSourceRequest request)
        {
            var context = new MonitoringContext();
            var queryResult = context.PerformanceCounterDatas
                .Where(p => p.Service.Equals("Scheduler") && p.Counter.Equals("successful requests"))
                .GroupBy(g => new {g.MachineName,g.Service},
                 (key, group) => new  
                 {
                       key.Service,
                       Timestamp = group.Max(p => p.Timestamp),
                       key.MachineName,
                       Value = group.Max(p => p.CounterValue)
                 })
                 .ToList();
            var result = queryResult
                 .Select(r => new PerformanceCounterDataViewModel
                 {
                     Service = r.Service,
                     Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                     MachineName = r.MachineName,
                     Counter = "successful requests",
                     Value = r.Value
                 });
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSystemStatus([DataSourceRequest] DataSourceRequest request)
        {
            var context = new MonitoringContext();
            var result = context.PerformanceCounterDatas
                  .Where(p => p.Counter.Equals("% processor time") || p.Counter.Equals("available mbytes"))
                  .GroupBy(g => new { g.MachineName, g.Service, g.Counter },
                      (key, group) => new
                      {
                          key.Service,
                          key.MachineName,
                          key.Counter,
                          Timestamp = group.Max(p => p.Timestamp),
                          Value = group.Max(p => p.CounterValue)
                      })
                    .ToList()
                    .Select(s => new PerformanceCounterDataViewModel
                    {
                        Service = s.Service,
                        Timestamp = s.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                        MachineName = s.MachineName,
                        Counter = s.Counter,
                        Value = s.Value
                    });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
