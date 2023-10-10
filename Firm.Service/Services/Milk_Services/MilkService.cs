using Firm.Core.EntityModel;
using Firm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Firm.Utility.Miscellaneous.Enum;

namespace Firm.Service.Services.Milk_Services
{
    public class MilkService : IMilkService
    {
        private readonly FirmDBContext context;

        public MilkService(FirmDBContext context)
        {
            this.context = context;
        }
        public async Task<MilkServiceViewModel> AddNewMilk(MilkServiceViewModel model)
        {
            //bool isExists = await context.MilkMonitors.AnyAsync(c => c.vaccineName == model.vaccineName);
            //if (isExists)
            //{
            //    model.ErrorMessage = "milk already exists. Please add a unique milk.";
            //     return model;
            // }

            try
            {
                var milkList = new List<MilkMonitor>();

                foreach (var milkmodel in model.milkServiceVmList)
                {
                    MilkMonitor milk = new MilkMonitor();
                    milk.Date = model.Date;
                    milk.ShiftVal = model.ShiftVal ;
                    milk.TotalMilk = milkmodel.TotalMilk;
                    //milk.Remarks = model.Remarks;
                    milk.CowId = milkmodel.CowId;
                    milk.CreatedOn = DateTime.Now;
                    milk.IsActive = true;
                    milkList.Add(milk);
                }
                context.MilkMonitors.AddRange(milkList);
                var res = await context.SaveChangesAsync();

                //if (res != 0)
                //{
                //    model.Id = milk.Id;
                //}


                return model;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error occurred while adding a new milk. Details: " + ex.Message;
                return model;
            }
        }

        public async Task<List<MilkServiceViewModel>> GetAll()
        {
            List<MilkServiceViewModel> lists = new List<MilkServiceViewModel>();
            var data = await context.MilkMonitors.AsQueryable().AsNoTracking().Where(x => x.IsActive == true && x.Date.Date >= DateTime.Now.Date.AddDays(-15))
                .GroupBy(c => c.Date).Select(c => new
                {
                    date = c.Key,
                    milkdata = c.GroupBy(c => c.CowId).Select(c => new
                    {
                        cowId = c.Key,
                        id = c.Select(c => c.Id).FirstOrDefault(),
                        dayShift = c.Where(c => c.ShiftVal == (Shift)2).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                        mourningShift = c.Where(c => c.ShiftVal == (Shift)1).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                        eveningShift = c.Where(c => c.ShiftVal == (Shift)3).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                        remark = c.Select(c => c.Remarks).FirstOrDefault()
                    }),
                }).ToListAsync();
            foreach (var milk in data)
            {
                foreach (var cow in milk.milkdata)
                {
                    var cowObject = context.Cows.AsNoTracking().FirstOrDefault(a => a.Id == cow.cowId);
                    MilkServiceViewModel model = new MilkServiceViewModel();
                    model.Date = milk.date;
                    //model.Remarks = milk.Remarks;
                    model.DayShift = cow.dayShift;
                    model.MourningShift = cow.mourningShift;
                    model.EveningShift = cow.eveningShift;
                    model.TotalMilk = cow.dayShift + cow.mourningShift + cow.eveningShift;
                    model.Remarks = cow.remark;
                    model.CowId = cowObject.Id;
                    model.Id = cow.id;
                    model.CowTagId = cow.cowId == 0 ? "" : cowObject.TagId;

                    lists.Add(model);
                }
            }
            return lists;
        }


        public async Task<MilkServiceViewModel> GetById(long id)
        {
            var milk = await context.MilkMonitors.FindAsync(id);

            var milkdata = await context.MilkMonitors.AsQueryable().AsNoTracking()
                       .Where(c => c.CowId == milk.CowId && c.Date == milk.Date&& c.IsActive==true).GroupBy(c => c.Date)
                       .Select(c => new
                       {
                           date = c.Select(c => c.Date).FirstOrDefault(),
                           dayShift = c.Where(c => c.ShiftVal == (Shift)2).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                           mourningShift = c.Where(c => c.ShiftVal == (Shift)1).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                           eveningShift = c.Where(c => c.ShiftVal == (Shift)3).Select(c => c.TotalMilk).ToList().AsQueryable().Sum(),
                           remark = c.Select(c => c.Remarks).FirstOrDefault()
                       }).ToListAsync();


            MilkServiceViewModel model = new MilkServiceViewModel();
            foreach (var cowMilk in milkdata)
            {

                model.Date = cowMilk.date;
                //model.Remarks = milk.Remarks;
                model.DayShift = cowMilk.dayShift;
                model.MourningShift = cowMilk.mourningShift;
                model.EveningShift = cowMilk.eveningShift;
                model.TotalMilk = cowMilk.dayShift + cowMilk.mourningShift + cowMilk.eveningShift;
                model.Remarks = cowMilk.remark;
                model.CowId = milk.CowId;
                model.Id = milk.Id;
                model.CowTagId = milk.CowId == 0 ? "" : context.Cows.AsNoTracking().FirstOrDefault(a => a.Id == milk.CowId).TagId;

            }



            return model;
        }
        public async Task<MilkServiceViewModel> UpdateMilk(MilkServiceViewModel model)
        {
            var milkData = await context.MilkMonitors.AsQueryable().AsNoTracking()
                         .Where(c => c.CowId == model.CowId && c.Date == model.Date &&c.IsActive==true)
                         .ToListAsync();

            //var dayShift = milkData.Where(c => c.ShiftVal == (Shift)2).FirstOrDefault();
            //var mourningShift = milkData.Where(c => c.ShiftVal == (Shift)1).FirstOrDefault();
            //var eveningShift = milkData.Where(c => c.ShiftVal == (Shift)3).FirstOrDefault();
            //var remark = milkData.FirstOrDefault();
          
            int dayShift = 0;
            int mourningShift = 0;
            int eveningShift = 0;
            
            foreach (var milk in milkData)
            {
               

                if (milk.ShiftVal == (Shift)2 && dayShift<1)
                {
                    milk.TotalMilk = Convert.ToDecimal(model.DayShift);

                  
                    dayShift++;
                   
                }else if (milk.ShiftVal == (Shift)1 && mourningShift < 1)
                {
                    milk.TotalMilk = Convert.ToDecimal(model.MourningShift);
                
                    mourningShift++;
                    
                }else if (milk.ShiftVal == (Shift)3 && eveningShift < 1)
                {
                    milk.TotalMilk = Convert.ToDecimal(model.EveningShift);
                  
                    eveningShift++;
                    
                }else{
                        milk.TotalMilk = 0;

                        
                }
                    
                

                milk.UpdatedOn = DateTime.Now;
                milk.Remarks = model.Remarks;


            }

            try
            {

                context.UpdateRange(milkData);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error occurred while adding a new milk. Details: " + ex.Message;
                return model;
            }
        }

        public async Task<bool> Remove(long id)
        {
            var milk = await context.MilkMonitors.FindAsync(id);
            var milkList = await context.MilkMonitors.AsQueryable()
                           .Where(x => x.CowId == milk.CowId && x.Date == milk.Date)
                           .ToListAsync();

            foreach (var data in milkList)
            {
                data.IsActive = false;
            }

            try
            {
                context.UpdateRange(milkList);
                await context.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }

        }

        public async Task<List<MilkServiceViewModel>> MilkingCowList(MilkServiceViewModel model)
        {
            if (model is null)
            {

            }
            var cowList = await context.Cows.AsQueryable().AsNoTracking()

                .Where(c => c.IsActive == true && c.LivestockTypeVal == (LivestockType)2
                && (c.Status == Status.Live || c.Status.Value == 0 || c.Status.Value == null)
                && c.ShedNo.Equals(model.ShadeNo))

                .Select(c => new { cowId = c.Id, tagId = c.TagId }).OrderBy(c => c.tagId).ToListAsync();


            var listVM = new List<MilkServiceViewModel>();
            foreach (var cow in cowList)
            {
                var modelObject = new MilkServiceViewModel()
                {
                    CowId = cow.cowId,
                    CowTagId = cow.tagId,
                    ShiftVal=model.ShiftVal,
                    Date=model.Date
                    
                };



                listVM.Add(modelObject);

            }
            return listVM;
        }




        //    public async Task<MilkServiceViewModel> GetById(long id)
        //    {
        //        var milk = await context.MilkMonitors.FindAsync(id);
        //        MilkServiceViewModel model = new MilkServiceViewModel();
        //        if (milk != null)
        //        {
        //            model.Date = milk.Date;
        //            model.ShiftVal = milk.ShiftVal;
        //            model.TotalMilk = milk.TotalMilk;
        //            model.Remarks = milk.Remarks;
        //            if (model.ShiftVal != 0)
        //            {
        //                model.ShiftName = Enum.GetName(typeof(Shift), model.ShiftVal);
        //            }
        //            model.CowId = milk.CowId;
        //            model.CowTagId = milk.CowId == 0 ? "" : context.Cows.FirstOrDefault(a => a.Id == milk.CowId).TagId;
        //            model.Id = milk.Id;
        //        }
        //        return model;
        //    }
        //    public async Task<MilkServiceViewModel> UpdateMilk(MilkServiceViewModel model)
        //    {
        //        //bool isExists = await context.MilkMonitors.AnyAsync(c => c.vaccineName == model.vaccineName);
        //        //if (isExists)
        //        //{
        //        //    model.ErrorMessage = "milk already exists. Please add a unique milk.";
        //        //    return model;
        //        //}

        //        try
        //        {
        //            var milk = await context.MilkMonitors.FindAsync(model.Id);
        //            milk.Date = model.Date;
        //            milk.ShiftVal = model.ShiftVal;
        //            milk.TotalMilk = model.TotalMilk;
        //            milk.Remarks = model.Remarks;
        //            milk.CowId = model.CowId;
        //            milk.CreatedOn = DateTime.Now;
        //            milk.IsActive = true;
        //            milk.UpdatedOn = DateTime.Now;
        //            context.Entry(milk).State = EntityState.Modified;
        //            await context.SaveChangesAsync();
        //            return model;
        //        }
        //        catch (Exception ex)
        //        {
        //            model.ErrorMessage = "Error occurred while adding a new milk. Details: " + ex.Message;
        //            return model;
        //        }
        //    }
        //}
    }
}
