using Amazon.DynamoDBv2.DataModel;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using EqualityInformationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;

namespace EqualityInformationApi.Tests.V1.E2ETests.Fixtures
{
    public class EqualityInformationFixture : IDisposable
    {
        public IDynamoDBContext DbContext { get; private set; }
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly Fixture _fixture;

        public EqualityInformationDb Entity { get; private set; }
        public List<EqualityInformationDb> Entities { get; private set; } = new List<EqualityInformationDb>();

        public EqualityInformationFixture(IDynamoDBContext context, IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            DbContext = context;
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _fixture = new Fixture();
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
        }

        public void GivenAnEntityExists()
        {
            Entity = _fixture.Build<EqualityInformationDb>()
                .Create();

            DbContext.SaveAsync(Entity).GetAwaiter().GetResult();
        }
    }
}
