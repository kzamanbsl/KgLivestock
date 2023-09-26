using Firm.Core.EntityModel;
using Firm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Firm.Utility.Miscellaneous.Enum;

namespace Firm.Service.Services.FeedConsumptionBulk_Services
{
    public class FeedConsumptionBulkService  : IFeedConsumptionBulkService
    {
        private readonly FirmDBContext context;

        public FeedConsumptionBulkService(FirmDBContext context)
        {
            this.context = context;
        }
        public async Task<FeedConsumptionBulkServiceVM> AddNewFeedConsumptionBulk(FeedConsumptionBulkServiceVM model)
        {
            //bool isExists = await context.FeedConsumptionBulks.AnyAsync(c => c.FeedCategoryName == model.FeedCategoryName);
            //if (isExists)
            //{
            //    model.ErrorMessage = "Feed Category already exists. Please add a unique Feed Category.";
            //    return model;
            //}
            try
            {
                if((model.LineNo | model.ShadeNo)<1)
                {
                    return model;
                }

              

                var feedCurrentStock = await context.FeedCurrentStocks.FirstOrDefaultAsync(c => c.FeedCategoryId == model.FeedCategoryId);
                if (feedCurrentStock != null )
                {
                    if(feedCurrentStock.CurrentQuantity < model.Quantity)
                    {
                        model.ErrorMessage = "Invalid Quantity, amount unavailable in store";
                    }
                    else
                    {
                        FeedConsumptionBulk feedConsumptionBulk = new FeedConsumptionBulk();
                        feedConsumptionBulk.Date = model.Date;
                        feedConsumptionBulk.FeedCategoryId = model.FeedCategoryId;
                        feedConsumptionBulk.Quantity = model.Quantity;
                        feedConsumptionBulk.UnitPrice = /*model.UnitPrice;*/ feedCurrentStock.CurrentUnitPrice;
                        feedConsumptionBulk.CreatedOn = DateTime.Now;
                        feedConsumptionBulk.IsActive = true;
                        context.FeedConsumptionBulks.Add(feedConsumptionBulk);
                        var res = await context.SaveChangesAsync();

                        if (res != 0)
                        {
                            model.Id = feedConsumptionBulk.Id;
                            //update stock table
                            feedCurrentStock.CurrentQuantity = feedCurrentStock.CurrentQuantity - feedConsumptionBulk.Quantity;
                            feedCurrentStock.UpdatedOn = DateTime.Now;
                            context.Entry(feedCurrentStock).State = EntityState.Modified;
                            await context.SaveChangesAsync();

                            //cow wise consumption
                            var liveStocksAdults = await context.Cows.Where(l => l.IsActive == true && (l.LivestockTypeVal == (LivestockType)1 || l.LivestockTypeVal == (LivestockType)2)).ToListAsync();
                            var liveStocksChilds = await context.Cows.Where(l => l.IsActive == true && (l.LivestockTypeVal == (LivestockType)3 || l.LivestockTypeVal == (LivestockType)4)).ToListAsync();
                            decimal totalLiveStocks = (liveStocksAdults.Count() * 2) + liveStocksChilds.Count();
                            decimal UnitPerLiveStock = feedConsumptionBulk.Quantity / totalLiveStocks;
                            foreach (var liveStock in liveStocksAdults)
                            {
                                FeedConsumptionCowWise feedConsumptionCowWise = new FeedConsumptionCowWise();
                                feedConsumptionCowWise.Date = feedConsumptionBulk.Date;
                                feedConsumptionCowWise.CowId = liveStock.Id;
                                feedConsumptionCowWise.FeedCategoryId = feedConsumptionBulk.FeedCategoryId;
                                feedConsumptionCowWise.Quantity = UnitPerLiveStock * 2;
                                feedConsumptionCowWise.UnitPrice = feedConsumptionBulk.UnitPrice;
                                feedConsumptionCowWise.CreatedOn = feedConsumptionBulk.CreatedOn;
                                feedConsumptionCowWise.IsActive = true;
                                context.FeedConsumptionCowWises.Add(feedConsumptionCowWise);
                                var res2 = await context.SaveChangesAsync();
                            }
                            foreach (var liveStock in liveStocksChilds)
                            {
                                FeedConsumptionCowWise feedConsumptionCowWise = new FeedConsumptionCowWise();
                                feedConsumptionCowWise.Date = feedConsumptionBulk.Date;
                                feedConsumptionCowWise.CowId = liveStock.Id;
                                feedConsumptionCowWise.FeedCategoryId = feedConsumptionBulk.FeedCategoryId;
                                feedConsumptionCowWise.Quantity = UnitPerLiveStock;
                                feedConsumptionCowWise.UnitPrice = feedConsumptionBulk.UnitPrice;
                                feedConsumptionCowWise.CreatedOn = feedConsumptionBulk.CreatedOn;
                                feedConsumptionCowWise.IsActive = true;
                                context.FeedConsumptionCowWises.Add(feedConsumptionCowWise);
                                var res2 = await context.SaveChangesAsync();
                            }


                        }
                    }
                    
                }
                else
                {
                    model.ErrorMessage = "Invalid Category, store is empty";
                }
                


                return model;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error occurred while adding a new feed entry. Details: " + ex.Message;
                return model;
            }
        }

        public async Task<List<FeedConsumptionBulkServiceVM>> GetAll()
        {
            List<FeedConsumptionBulkServiceVM> lists = new List<FeedConsumptionBulkServiceVM>();
            var data = await context.FeedConsumptionBulks.Where(x => x.IsActive).ToListAsync();
            foreach (var feedConsumptionBulk in data)
            {
                FeedConsumptionBulkServiceVM model = new FeedConsumptionBulkServiceVM();
                model.Date = feedConsumptionBulk.Date;
                model.FeedCategoryId = feedConsumptionBulk.FeedCategoryId;
                model.FeedCategoryName = feedConsumptionBulk.FeedCategoryId == 0 ? "" : context.FeedCategories.FirstOrDefault(a => a.Id == feedConsumptionBulk.FeedCategoryId).FeedCategoryName;
                model.Quantity = feedConsumptionBulk.Quantity;
                model.UnitPrice = feedConsumptionBulk.UnitPrice;
                model.IsActive = feedConsumptionBulk.IsActive;
                model.Id = feedConsumptionBulk.Id;
                lists.Add(model);
            }

            return lists;
        }

        public async Task<FeedConsumptionBulkServiceVM> GetById(long id)
        {
            var feedConsumptionBulk = await context.FeedConsumptionBulks.FindAsync(id);
            FeedConsumptionBulkServiceVM model = new FeedConsumptionBulkServiceVM();
            if (feedConsumptionBulk != null)
            {
                model.Date = feedConsumptionBulk.Date;
                model.FeedCategoryId = feedConsumptionBulk.FeedCategoryId;
                model.FeedCategoryName = feedConsumptionBulk.FeedCategoryId == 0 ? "" : context.FeedCategories.FirstOrDefault(a => a.Id == feedConsumptionBulk.FeedCategoryId).FeedCategoryName;
                model.Quantity = feedConsumptionBulk.Quantity;
                model.UnitPrice = feedConsumptionBulk.UnitPrice;
                model.IsActive = feedConsumptionBulk.IsActive;
                model.Id = feedConsumptionBulk.Id;
            }
            return model;
        }

        public async Task<FeedConsumptionBulkServiceVM> UpdateFeedConsumptionBulk(FeedConsumptionBulkServiceVM model)
        {
            //bool isExists = await context.FeedConsumptionBulks.AnyAsync(c => c.FeedCategoryName == model.FeedCategoryName && c.Id != model.Id);
            //if (isExists)
            //{
            //    model.ErrorMessage = "FeedConsumptionBulk already exists. Please add another Phone Number.";
            //    return model;
            //}

            try
            {
                var feedConsumptionBulk = await context.FeedConsumptionBulks.FirstOrDefaultAsync(c => c.Id == model.Id);
                if (feedConsumptionBulk != null)
                {
                    feedConsumptionBulk.Date = model.Date;
                    feedConsumptionBulk.FeedCategoryId = model.FeedCategoryId;
                    feedConsumptionBulk.Quantity = model.Quantity;
                    feedConsumptionBulk.UnitPrice = model.UnitPrice;
                    //context.FeedConsumptionBulks.Add(feedConsumptionBulk);
                    //await context.SaveChangesAsync();
                    context.Entry(feedConsumptionBulk).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }

                return model;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error occurred while adding a new feedConsumptionBulk. Details: " + ex.Message;
                return model;
            }
        }
        public async Task<bool> Remove(long id)
        {
            var feedConsumptionBulk = await context.FeedConsumptionBulks.FirstOrDefaultAsync(c => c.Id == id);
            feedConsumptionBulk.IsActive = false;
            context.Entry(feedConsumptionBulk).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
