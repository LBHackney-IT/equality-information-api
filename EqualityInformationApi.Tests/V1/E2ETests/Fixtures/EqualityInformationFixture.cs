using Amazon.SimpleNotificationService;
using AutoFixture;
using EqualityInformationApi.V1.Infrastructure;
using Hackney.Core.Testing.DynamoDb;
using System;

namespace EqualityInformationApi.Tests.V1.E2ETests.Fixtures
{
    public class EqualityInformationFixture : IDisposable
    {
        public IDynamoDbFixture DbFixture { get; private set; }
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly Fixture _fixture;

        public EqualityInformationDb Entity { get; private set; }

        public EqualityInformationFixture(IDynamoDbFixture dbFixture, IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            DbFixture = dbFixture;
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
                _disposed = true;
            }
        }

        public void GivenAnEntityDoesNotExist()
        {
        }

        public void GivenAnEntityExists(Guid targetId)
        {
            Entity = _fixture.Build<EqualityInformationDb>()
                .With(x => x.TargetId, Guid.NewGuid())
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.VersionNumber, (int?)null)
                .Create();

            Entity.TargetId = targetId;

            DbFixture.SaveEntityAsync(Entity).GetAwaiter().GetResult();
            Entity.VersionNumber = 0;
        }
    }
}
