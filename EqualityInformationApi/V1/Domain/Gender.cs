using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Domain
{
    public class Gender
    {
        public string GenderValue { get; set; }
        public string GenderValueIfOther { get; set; }
        public bool? GenderDifferentToBirthSex { get; set; } = null;
    }
}
