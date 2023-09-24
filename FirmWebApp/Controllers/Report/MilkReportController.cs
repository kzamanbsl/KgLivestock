using Firm.Service.Services.Milk_Services;
using Firm.Service.Services.Report_Services;
using Firm.Service.Services.Report_Services.MilkReport_Services;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class MilkReportController : Controller
    {
        public IMilkReportServices _milkReportService { get; set; }
        public MilkReportController(IMilkReportServices milkReportService)
        {
            _milkReportService = milkReportService; 
        
        }


        public async Task< IActionResult> Index()
        {
            MilkReportVM milkReport =  new MilkReportVM();

            milkReport.StartDate = DateTime.Now.AddMonths(-2);
            milkReport.EndDate = DateTime.Now;

            milkReport = await  _milkReportService.MilkReport(milkReport);
            milkReport.StartDate = DateTime.Now.AddMonths(-2);
            milkReport.EndDate = DateTime.Now;
            return View(milkReport);
        }
     
        [HttpPost]
        public async Task<IActionResult> Index(MilkReportVM milkReportVM)
        {
      
            var model = new MilkReportVM();
            model=await _milkReportService.MilkReport(milkReportVM);
            return View(model);
        } 
        

        public async Task<IActionResult> MilkReport(MilkReportVM milkReportVM)
        {
      
            var model = new MilkReportVM();
            model=await _milkReportService.MilkReport(milkReportVM);
            return View(model);
        }
    }
}
