using Firm.Service.Services.FeedConsumptionCowWise_Services;
using Firm.Service.Services.Report_Services;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class FeedingCostReportController : Controller
    {
        public readonly IReportService _reportService;

        public FeedingCostReportController(IReportService reportService)
        {
            _reportService= reportService;
        }
        public async Task< IActionResult> Index()
        {
            var feedModel = new FeddingCostReportVM();
            feedModel.StartDate= DateTime.Now.AddDays(-15);
            feedModel.EndDate= DateTime.Now;
            feedModel =  await _reportService.FeddingCostReport(feedModel);
            feedModel.StartDate = DateTime.Now.AddMonths(-2);
            feedModel.EndDate = DateTime.Now;

            return View(feedModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(FeddingCostReportVM feddingCostReport)
        {
            var model = await  _reportService.FeddingCostReport(feddingCostReport);
            return View(model);
        } 

        public async Task < IActionResult> FeddingCostSummary(FeddingCostReportVM feddingCostReport)
        {
            var model = await  _reportService.FeddingCostReport(feddingCostReport);
            return View(model);
        }
    }
}
