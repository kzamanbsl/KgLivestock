using Firm.Core.EntityModel;
using Firm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                MilkMonitor milk = new MilkMonitor();
                milk.Date = model.Date;
                milk.ShiftVal = model.ShiftVal;
                milk.TotalMilk = model.TotalMilk;
                milk.Remarks = model.Remarks;
                milk.CowId = model.CowId;
                milk.CreatedOn = DateTime.Now;
                milk.IsActive = true;
                context.MilkMonitors.Add(milk);
                var res = await context.SaveChangesAsync();

                if (res != 0)
                {
                    model.Id = milk.Id;
                }


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
            var data = await context.MilkMonitors.Where(x => x.IsActive).ToListAsync();
            foreach (var milk in data)
            {
                MilkServiceViewModel model = new MilkServiceViewModel();
                model.Date = milk.Date;
                model.ShiftVal = milk.ShiftVal;
                model.TotalMilk = milk.TotalMilk;
                model.Remarks = milk.Remarks;
                if (model.ShiftVal != 0)
                {
                    model.ShiftName = Enum.GetName(typeof(Shift), model.ShiftVal);
                }
                model.CowId = milk.CowId;
                model.CowTagId = milk.CowId == 0 ? "" : context.Cows.FirstOrDefault(a => a.Id == milk.CowId).TagId;
                model.Id = milk.Id;
                lists.Add(model);
            }

            return lists;
        }

        public async Task<MilkServiceViewModel> GetById(long id)
        {
            var milk = await context.MilkMonitors.FindAsync(id);
            MilkServiceViewModel model = new MilkServiceViewModel();
            if (milk != null)
            {
                model.Date = milk.Date;
                model.ShiftVal = milk.ShiftVal;
                model.TotalMilk = milk.TotalMilk;
                model.Remarks = milk.Remarks;
                if (model.ShiftVal != 0)
                {
                    model.ShiftName = Enum.GetName(typeof(Shift), model.ShiftVal);
                }
                model.CowId = milk.CowId;
                model.CowTagId = milk.CowId == 0 ? "" : context.Cows.FirstOrDefault(a => a.Id == milk.CowId).TagId;
                model.Id = milk.Id;
            }
            return model;
        }
        public async Task<MilkServiceViewModel> UpdateMilk(MilkServiceViewModel model)
        {
            //bool isExists = await context.MilkMonitors.AnyAsync(c => c.vaccineName == model.vaccineName);
            //if (isExists)
            //{
            //    model.ErrorMessage = "milk already exists. Please add a unique milk.";
            //    return model;
            //}

            try
            {
                var milk = await context.MilkMonitors.FindAsync(model.Id);
                milk.Date = model.Date;
                milk.ShiftVal = model.ShiftVal;
                milk.TotalMilk = model.TotalMilk;
                milk.Remarks = model.Remarks;
                milk.CowId = model.CowId;
                milk.CreatedOn = DateTime.Now;
                milk.IsActive = true;
                milk.UpdatedOn = DateTime.Now;
                context.Entry(milk).State = EntityState.Modified;
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
            milk.IsActive = false;
            context.Entry(milk).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return true;
        }
    }
}
