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
        public readonly Fixture _fixture = new Fixture();
        public readonly IDynamoDBContext _dbContext;

        public EqualityInformationDb Entity { get; private set; }
        public List<EqualityInformationDb> Entities { get; private set; } = new List<EqualityInformationDb>();

        public EqualityInformationFixture(IDynamoDBContext context)
        {
            _dbContext = context;
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
                    _dbContext.DeleteAsync<EqualityInformationDb>(Entity.TargetId, Entity.Id).GetAwaiter().GetResult();
                }

                if (Entities.Count != 0)
                {
                    foreach (var entity in Entities)
                    {
                        _dbContext.DeleteAsync<EqualityInformationDb>(entity.TargetId, entity.Id).GetAwaiter().GetResult();
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

            _dbContext.SaveAsync(entity).GetAwaiter().GetResult();

            Entity = entity;
        }

        public void GivenManyEntitiesExist(int numberOfEntities, Guid targetId)
        {
            var entities = _fixture
                .Build<EqualityInformationDb>()
                .With(x => x.TargetId, targetId)
                .CreateMany(numberOfEntities);

            foreach (var entity in entities)
            {
                _dbContext.SaveAsync(entity).GetAwaiter().GetResult();
            }
        }
    }
}
