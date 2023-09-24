using Firm.Service.Services.Report_Services.TotalCosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Service.Services.Report_Services.TottalCosting
{
    public  interface ICowCostTotal_Services
    {
        Task<CowCostTotalModel> CowCost();
    }
}
