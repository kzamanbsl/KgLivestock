using Firm.Infrastructure.Data;
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

    }
}
