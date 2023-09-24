using Firm.Infrastructure.Data;
using Firm.Service.Services.Report_Services.TotalCosting;
using Firm.Service.Services.Report_Services.TottalCosting;
using Microsoft.AspNetCore.Mvc;

namespace FirmWebApp.Controllers.Report
{
    public class CowCostTotalController : Controller
    {
        public readonly ICowCostTotal_Services _dBContext;
        public CowCostTotalController(ICowCostTotal_Services dBContext)
        
        {
            _dBContext = dBContext;
        }
        public async Task< IActionResult> Index()
        {
            var model= new CowCostTotalModel();
            model = await _dBContext.CowCost();
            return View(model);
        }
        
        public async Task< IActionResult> AllCostSummary()
        {
            var model= new CowCostTotalModel();
            model = await _dBContext.CowCost();
            return View(model);
        }
    }
}
