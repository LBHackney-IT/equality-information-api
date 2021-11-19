using System;

namespace EqualityInformationApi.V1.Boundary.Request
{
    public class PatchEqualityInformationObject : EqualityInformationObject
    {
        public Guid Id { get; set; }
    }
}
