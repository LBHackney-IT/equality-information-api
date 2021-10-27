using EqualityInformationApi.V1.Domain;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Factories
{
    public interface ISnsFactory
    {
        EntityEventSns Create(EqualityInformation equalityInformation, Token token);
    }
}
