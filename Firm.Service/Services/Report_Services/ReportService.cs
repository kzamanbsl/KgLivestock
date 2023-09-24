using Firm.Infrastructure.Data;
using Firm.Service.Services.FeedConsumptionCowWise_Services;
using Firm.Service.Services.Report_Services.MilkReport_Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Service.Services.Report_Services
{
    public class ReportService : IReportService
    {
        private readonly FirmDBContext _context;

        public ReportService(FirmDBContext context)
        {

            _context = context;
        }




        public async Task<Vaccine_Treatment_ReportVM> Treatment_Report(Vaccine_Treatment_ReportVM VTReportVM)
        {
            var tratmentData = await _context.Treatments.AsQueryable().AsNoTracking()
                            .Where(c => c.IsActive == true && (c.TreatmentDate >= VTReportVM.StartDate &&
                            c.TreatmentDate <= VTReportVM.EndDate)).OrderBy(c => c.TreatmentDate)
                            .Select(c => new { c.TreatmentDate, c.CowId, c.DoctorId, c.Investigation, c.Price })
                            .ToListAsync();


            var vaccineData = await _context.Vaccines.AsQueryable().AsNoTracking()
                            .Where(c => c.IsActive == true && (c.VaccineDate >= VTReportVM.StartDate &&
                            c.VaccineDate <= VTReportVM.EndDate)).OrderBy(c => c.VaccineDate)
                            .Select(c => new { c.VaccineDate, c.CowId, c.DoctorId, c.Name, c.Price })
                            .ToListAsync();


            var vtModelList = new List<Vaccine_Treatment_ReportVM>();

            foreach (var data in tratmentData)
            {
                var Doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(c => c.Id == data.DoctorId);
                var cow = await _context.Cows.AsNoTracking().FirstOrDefaultAsync(c => c.Id == data.CowId);
                var model = new Vaccine_Treatment_ReportVM()
                {
                    Day = data.TreatmentDate.ToString("dd MMM yy"),
                    TagId = cow.TagId,
                    TreatmentFor = data.Investigation,
                    DoctorName = Doctor.DoctorName,
                    Price = data.Price ?? 0,
                    CostingType= "Treatment"
                };

                vtModelList.Add(model);



            }
            foreach (var data in vaccineData)
            {
                var Doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(c => c.Id == data.DoctorId);
                var cow = await _context.Cows.AsNoTracking().FirstOrDefaultAsync(c => c.Id == data.CowId);
                var model = new Vaccine_Treatment_ReportVM()
                {
                    Day = data.VaccineDate.ToString("dd MMM yy"),
                    TagId = cow.TagId,
                    TreatmentFor = data.Name,
                    DoctorName = Doctor.DoctorName,
                    CostingType = "Vaccine",
                    Price = data.Price
                };

                vtModelList.Add(model);



            }
            var trObject = new Vaccine_Treatment_ReportVM();
            trObject.StartDate = VTReportVM.StartDate;
            trObject.EndDate = VTReportVM.EndDate;
            trObject.VTReportVM = vtModelList;
            trObject.TottalDay = vtModelList.DistinctBy(c => c.Day).Count();
            trObject.TottalCow = vtModelList.DistinctBy(c => c.TagId).Count();
            trObject.TottalPrice = vtModelList.Sum(c => c.Price);
            trObject.VTReportVM.OrderBy(c => c.Day).ToList();

            return trObject;
        }
        public async Task<FeddingCostReportVM> FeddingCostReport(FeddingCostReportVM FeddingReport)
        {
            var feedingData = await  _context.FeedConsumptionCowWises.AsQueryable().AsNoTracking()
                 .Where(c => c.IsActive == true & (c.Date >= FeddingReport.StartDate && c.Date <= FeddingReport.EndDate))
                 .OrderBy(c => c.Date).GroupBy(c=>c.Date)
                 .Select(c => new { date = c.Key,feedData= c.GroupBy(x=>x.CowId)
                 .Select(x=>new {cowId=x.Key,foodUnit= x.Sum(c=>c.Quantity),Price=x.Sum(x=>x.UnitPrice*x.Quantity)})})
                 .ToListAsync();
               
           var feedCostList= new List<FeddingCostReportVM>();

            foreach (var feed in feedingData)
            {

                foreach (var data in feed.feedData)
                {
                    var cow = await _context.Cows.AsNoTracking().FirstOrDefaultAsync(c => c.Id == data.cowId);
                    var feedObject = new FeddingCostReportVM()
                    {
                        Day = feed.date.ToString("dd MMM yy"),
                        TagNo = cow.TagId,
                        Consumption = data.Price,
                        FoodUnit= data.foodUnit

                    };
                    feedCostList.Add(feedObject);
                }
            }
            var feedingCostReport = new FeddingCostReportVM();
            feedingCostReport.FeddingCostList= feedCostList;
            feedingCostReport.StartDate= FeddingReport.StartDate;
            feedingCostReport.EndDate= FeddingReport.EndDate;
            feedingCostReport.TottalDay= feedCostList.DistinctBy(c=>c.Day).Count();
            feedingCostReport.TottalCow = feedCostList.DistinctBy(c => c.TagNo).Count();
            feedingCostReport.TottalConsumption= feedCostList.Sum(c=>c.Consumption);
            feedingCostReport.TottalFoodUnit= feedCostList.Sum(c=>c.FoodUnit);
            feedingCostReport.FeddingCostList.OrderBy(c => c.Day).ToList();

            return feedingCostReport;
        }




    }
}
