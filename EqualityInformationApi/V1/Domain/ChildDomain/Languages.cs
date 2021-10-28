using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Domain
{
    public class Languages
    {
        public string Language { get; set; }
        public bool? IsPrimary { get; set; } = null;
    }
}
