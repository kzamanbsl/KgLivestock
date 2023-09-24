using Firm.Service.Services.Milk_Services;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class ReportController : Controller
    {
        public ReportController() { }
        public IActionResult Index()
        {
            return View();
        }
       


    }
}
