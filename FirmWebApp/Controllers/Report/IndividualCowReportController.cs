using Firm.Service.Services.Report_Services.IndividualCowReport_Services;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class IndividualCowReportController : Controller
    {

        public readonly IIndividualCowReport _individualCow;

        public IndividualCowReportController(IIndividualCowReport individualCow)
        {
            _individualCow = individualCow;

        }

        public async Task< IActionResult> Index()
        {
            


            return View();
        }


        [HttpPost]
        public async Task< IActionResult> Index(IndividualCowReportVM individualCow)
        {
            var model = new IndividualCowReportVM();
            model = await _individualCow.IndividualCowSummary(individualCow);


            return View(model);
        }
    }
}
