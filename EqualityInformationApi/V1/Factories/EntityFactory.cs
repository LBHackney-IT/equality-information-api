using EqualityInformationApi.V1.Domain;
using EqualityInformationApi.V1.Infrastructure;

namespace EqualityInformationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Entity ToDomain(this EqualityInformationDb databaseEntity)
        {
            //TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-equality-information-api/wiki/Factory-object-mappings

            return new Entity
            {
                Id = databaseEntity.Id,
                CreatedAt = databaseEntity.CreatedAt
            };
        }

        public static EqualityInformationDb ToDatabase(this Entity entity)
        {
            //TODO: Map the rest of the fields in the database object.

            return new EqualityInformationDb
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
