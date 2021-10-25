using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Domain
{
    public class CaringResponsibilities
    {
        public bool ProvideUnpaidCare { get; set; }
        public string HoursSpentProvidingUnpaidCare { get; set; }
    }
}
