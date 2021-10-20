using System.Collections.Generic;
using EqualityInformationApi.V1.Domain;

namespace EqualityInformationApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
