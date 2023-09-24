using Firm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Service.Services.Report_Services.MilkReport_Services
{
    public class MilkReportServices : IMilkReportServices
    {
        private readonly FirmDBContext _context;
        public MilkReportServices(FirmDBContext context)
        {
            _context = context;


        }




        public async Task<MilkReportVM> MilkReport(MilkReportVM milkReport)
        {

            var milkData = await _context.MilkMonitors.AsQueryable().AsNoTracking()
                .Where(c => c.IsActive == true && (c.Date >= milkReport.StartDate && c.Date <= milkReport.EndDate))
                .OrderBy(c => c.Date)
                .GroupBy(c => c.Date)
                .Select(DateData => new
                {
                    date = DateData.Key,
                    cowData = DateData
                    .GroupBy(c => c.CowId)
                    .Select(cow => new { cowId = cow.Key, tottalMilk = cow.Sum(c => c.TotalMilk) })
                }).ToListAsync();

            var model = new List<MilkReportVM>();
            foreach (var milk in milkData)
            {
                foreach (var cow in milk.cowData)
                {


                    if (cow is null | cow.tottalMilk == (0 | null))
                    {
                        continue;
                    }
                    var MilkReport = new MilkReportVM();
                    MilkReport.Day = milk.date.ToString("dd MMM yy");
                    MilkReport.CowTagId = await _context.Cows.AsNoTracking().Where(c => c.Id == cow.cowId).Select(c => c.TagId).FirstOrDefaultAsync();
                    MilkReport.TotalMilk = cow.tottalMilk;


                    model.Add(MilkReport);
                }


            }
            var milkReportObject = new MilkReportVM();
            milkReportObject.StartDate = milkReport.StartDate;
            milkReportObject.EndDate = milkReport.EndDate;
            milkReportObject.ListMilkReportVM = model;
            milkReportObject.TotalCowMilk = milkReportObject.ListMilkReportVM.Sum(c => c.TotalMilk);
            milkReportObject.TotalCow = model.DistinctBy(c => c.CowTagId).Count();
            milkReportObject.tottalDay = model.DistinctBy(c => c.Day).Count();
            return milkReportObject;
        }
    }
}
