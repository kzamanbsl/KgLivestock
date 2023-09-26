using Firm.Service.Services.Cow_Services;
using Firm.Service.Services.Milk_Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirmWebApp.Controllers.Cow
{
    public class MilkController : Controller
    {
        public readonly ICowService cowService;
        public readonly IMilkService milkService;
        public MilkController(ICowService cowService, IMilkService milkService)
        {
            this.cowService = cowService;
            this.milkService = milkService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.cowlist = new SelectList((await cowService.GetAll()).Select(s => new { Id = s.Id, Name = s.TagId }), "Id", "Name");
            var data = await milkService.GetAll();
            return View(data);
        }
        public async Task<IActionResult> Create()
        {
            var cowList = await cowService.GetAll();
            ViewBag.cowlist = new SelectList((cowList).Select(s => new { Id = s.Id, Name = s.TagId }), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(MilkServiceViewModel model)
        {
            try
            {
                var result = await milkService.AddNewMilk(model);

                //if (!string.IsNullOrEmpty(result.ErrorMessage))
                //{
                //    ViewData["ErrorMessage"] = result.ErrorMessage;
                //    return View(model);
                //}

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding cow.");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var cowList = await cowService.GetAll();

            ViewBag.cowlist = new SelectList((cowList).Select(s => new { Id = s.Id, Name = s.TagId }), "Id", "Name");
            var obj = await milkService.GetById(id);
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MilkServiceViewModel model)
        {
            try
            {
                 await milkService.UpdateMilk(model);

                //if (!string.IsNullOrEmpty(result.ErrorMessage))
                //{
                //    ViewData["ErrorMessage"] = result.ErrorMessage;
                //    return View(model);
                //}

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding cow.");
                return View(model);
            }
        }


        public async Task<IActionResult> Delete(long id)
        {
            var obj = await milkService.Remove(id);
            return RedirectToAction("Index");
        }
    }
}
