using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request
{
    public class EqualityInformationQuery
    {
        [FromQuery(Name = "targetId")]
        public Guid TargetId { get; set; }
    }
}
