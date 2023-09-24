using Firm.Service.Services.Report_Services;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class Vaccine_Treatment_ReportController : Controller
    {
        public readonly IReportService _reportService;
        public Vaccine_Treatment_ReportController(IReportService reportService)
        {
           
            _reportService = reportService;
        }

        public async Task< IActionResult> Index()
        {
            var vtModel = new Vaccine_Treatment_ReportVM();
            vtModel.StartDate= DateTime.Now.AddMonths(-2);
            vtModel.EndDate= DateTime.Now;
            vtModel = await _reportService.Treatment_Report(vtModel);
            vtModel.StartDate = DateTime.Now.AddMonths(-2);
            vtModel.EndDate = DateTime.Now;
            return View(vtModel);
        }
       
        [HttpPost]
       public async Task<IActionResult> Index(Vaccine_Treatment_ReportVM vaccine_Treatment)
        {
            var model =await  _reportService.Treatment_Report(vaccine_Treatment);

            return View(model);
        } 

       public async Task<IActionResult> VTreatment_Summary(Vaccine_Treatment_ReportVM vaccine_Treatment)
        {
            var model =await  _reportService.Treatment_Report(vaccine_Treatment);

            return View(model);
        }
    }
}
