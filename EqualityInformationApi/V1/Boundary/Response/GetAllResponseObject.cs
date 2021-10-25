using EqualityInformationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Response
{
    public class GetAllResponseObject
    {
        public List<EqualityInformationResponseObject> EqualityData { get; set; }
    }
}
