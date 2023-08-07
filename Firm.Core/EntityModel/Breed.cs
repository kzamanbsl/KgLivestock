using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Core.EntityModel
{
    public class Breed : BaseEntity
    {
        public string BreedName { get; set; }
        //public long? CowId { get; set; } = 0;
        //public string BreedSource { get; set; }
        //public double? BreedPrice { get; set; } = 0.00;
    }
}
