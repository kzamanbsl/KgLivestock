using Firm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Service.Services.Report_Services.IndividualCowReport_Services
{
    public class IndividualCowReport: IIndividualCowReport
    {
        public readonly FirmDBContext _dBContext;
        public IndividualCowReport(FirmDBContext dBContext)
        {
            _dBContext= dBContext;
        }
      public async Task< IndividualCowReportVM> IndividualCowSummary(IndividualCowReportVM individualCow)
        {
            var TotalCost = await _dBContext.Cows.AsQueryable().AsNoTracking()
               .Where(c => c.IsActive == true && c.TagId.Equals(individualCow.TagId.ToString())).Select(c => new
               {
                   tagId = c.TagId,
                   cowBuy = c.Price,
                   firstWeight = c.Weight,
                   vaccineCost = _dBContext.Vaccines.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                   .Sum(v => v.Price),
                   tratmentCost = _dBContext.Treatments.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                   .Sum(v => v.Price),
                   feedingCost = _dBContext.FeedConsumptionCowWises.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                   .Sum(c => c.Quantity * c.UnitPrice),
               }).ToListAsync();

            var individualCowCost = new IndividualCowReportVM();
            foreach(var cow in TotalCost)
            {
                individualCowCost.BuyCost = cow.cowBuy;
                individualCowCost.TotalVacCost = cow.vaccineCost;
                individualCowCost.TotalTreatment = cow.tratmentCost;
                individualCowCost.TotalFeedCost = cow.feedingCost;
                individualCowCost.Weight = individualCow.Weight;
                individualCowCost.CurrentMeatPrice = individualCow.CurrentMeatPrice;
                individualCowCost.CowPrice = (individualCow.Weight + Convert.ToDecimal(cow.firstWeight)) * individualCow.CurrentMeatPrice;
                individualCowCost.TotalCowCost = Convert.ToDecimal(cow.cowBuy) + cow.vaccineCost + cow.tratmentCost + cow.feedingCost;

            }

            return individualCowCost;
        }
    }
}
