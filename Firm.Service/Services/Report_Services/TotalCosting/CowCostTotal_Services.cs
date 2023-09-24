using Firm.Core.EntityModel;
using Firm.Infrastructure.Data;
using Firm.Service.Services.Report_Services.TotalCosting;
using Firm.Service.Services.Report_Services.TottalCosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Service.Services.Report_Services.TotalCosting
{
    public class CowCostTotal_Services : ICowCostTotal_Services
    {
        public readonly FirmDBContext _dBContext;

       public CowCostTotal_Services(FirmDBContext dBContext)
        {


            _dBContext = dBContext;
        }

        public async Task<CowCostTotalModel> CowCost()
        {
            //var vaccineCost= await  _dBContext.Vaccines.AsQueryable().AsNoTracking()
            //                 .Where(c=>c.IsActive==true).GroupBy(c=>c.CowId)
            //                 .Select(c=> new {cowId= c.Key,vaccinecost=c.Sum(c=>c.Price)})
            //                 .ToListAsync();   
            
            //var tratmentCost= await  _dBContext.Treatments.AsQueryable().AsNoTracking()
            //                 .Where(c=>c.IsActive==true).GroupBy(c=>c.CowId)
            //                 .Select(c=> new {cowId= c.Key,treatmentecost=c.Sum(c=>c.Price)})
            //                 .ToListAsync();
                     
            //var feedingCost= await  _dBContext.FeedConsumptionCowWises.AsQueryable().AsNoTracking()
            //                 .Where(c=>c.IsActive==true).GroupBy(c=>c.CowId)
            //                 .Select(c=> new {cowId= c.Key,treatmentecost=c.Sum(c=>c.Quantity*c.UnitPrice)})
            //                 .ToListAsync();    
            

            //var cowBuyCost= await  _dBContext.Cows.AsQueryable().AsNoTracking()
            //                 .Where(c=>c.IsActive==true)
            //                 .Select(c=> new { price = c.Price, id = c.Id }).ToListAsync();

            var TotalCost = await _dBContext.Cows.AsQueryable().AsNoTracking()
                           .Where(c => c.IsActive == true).Select(c => new
                           {
                               tagId = c.TagId,
                               cowBuy = c.Price,
                               vaccineCost = _dBContext.Vaccines.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                               .Sum(v => v.Price),
                               tratmentCost = _dBContext.Treatments.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                               .Sum(v => v.Price),
                               feedingCost = _dBContext.FeedConsumptionCowWises.AsNoTracking().Where(x => x.IsActive == true && x.CowId == c.Id)
                               .Sum(c => c.Quantity * c.UnitPrice),
                                }).ToListAsync();


            var CowCostList = new List<CowCostTotalModel>();
            foreach (var cost in TotalCost)
            {
                var CowCost = new CowCostTotalModel()
                {
                    TagNo = cost.tagId,
                    VacCost= cost.vaccineCost,
                    FeedCost=cost.feedingCost,
                    Treatment=cost.tratmentCost,
                    BuyCost= cost.cowBuy,
                    perCowCosting= Convert.ToDecimal(Convert.ToDecimal(cost.vaccineCost) 
                                   + Convert.ToDecimal(cost.feedingCost)+ 
                                   Convert.ToDecimal(cost.tratmentCost) +
                                    Convert.ToDecimal(cost.cowBuy))

                };

                CowCostList.Add(CowCost);
            }
            var cowcost = new CowCostTotalModel();
            cowcost.CowCostList= CowCostList;
            cowcost.TotalVacCost = CowCostList.Sum(c => c.VacCost);
            cowcost.TotalTreatment = CowCostList.Sum(c => c.Treatment);
            cowcost.TotalFeedCost = CowCostList.Sum(c => c.FeedCost);
            cowcost.Cow = CowCostList.DistinctBy(c => c.TagNo).Count();
            cowcost.TotalCowCost = CowCostList.Sum(c => c.perCowCosting);
            cowcost.CowBuying =Convert.ToDecimal( CowCostList.Sum(c => c.BuyCost));

            return cowcost;
        }
    }
}
