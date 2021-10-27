using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using EqualityInformationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.Tests.V1.E2ETests.Fixtures
{
    public class EqualityInformationFixture : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        public IDynamoDBContext DbContext { get; private set; }

        public EqualityInformationDb Entity { get; private set; }
        public List<EqualityInformationDb> Entities { get; private set; } = new List<EqualityInformationDb>();

        public EqualityInformationFixture(IDynamoDBContext context)
        {
            DbContext = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != Entity)
                {
                    DbContext.DeleteAsync<EqualityInformationDb>(Entity.TargetId, Entity.Id).GetAwaiter().GetResult();
                }

                if (Entities.Count != 0)
                {
                    foreach (var entity in Entities)
                    {
                        DbContext.DeleteAsync<EqualityInformationDb>(entity.TargetId, entity.Id).GetAwaiter().GetResult();
                    }
                }

                _disposed = true;
            }
        }

        public void GivenAnEntityDoesNotExist()
        {
            //
        }

        public void GivenAnEntityExists()
        {
            var entity = _fixture.Create<EqualityInformationDb>();

            DbContext.SaveAsync(entity).GetAwaiter().GetResult();

            Entity = entity;
        }

        public void GivenManyEntitiesExist(int numberOfEntities, Guid targetId)
        {
            var entities = _fixture
                .Build<EqualityInformationDb>()
                .With(x => x.TargetId, targetId)
                .CreateMany(numberOfEntities);

            Entities = entities.ToList();

            foreach (var entity in entities)
            {
                DbContext.SaveAsync(entity).GetAwaiter().GetResult();
            }
        }
    }
}
