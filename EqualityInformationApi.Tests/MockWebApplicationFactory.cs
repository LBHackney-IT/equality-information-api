using Amazon.DynamoDBv2;
using Hackney.Core.DynamoDb;
using Hackney.Core.Sns;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace EqualityInformationApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
            : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef {
                Name = "EqualityInformation",
                KeyName = "targetId",
                KeyType = ScalarAttributeType.S,
                RangeKeyName = "id",
                RangeKeyType = ScalarAttributeType.S
            }
        };


        private readonly ServiceProvider _fixtureServices;

        public IDynamoDbFixture DynamoDbFixture { get; private set; }
        public ISnsFixture SnsFixture { get; private set; }
        public HttpClient Client { get; private set; }

        public MockWebApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            EnsureEnvVarConfigured("Sns_LocalMode", "true");
            EnsureEnvVarConfigured("Localstack_SnsServiceUrl", "http://localhost:4566");

            var services = new ServiceCollection();
            services.ConfigureDynamoDB();
            services.ConfigureDynamoDbFixture();
            services.ConfigureSns();
            services.ConfigureSnsFixture();
            _fixtureServices = services.BuildServiceProvider();

            try
            {
                DynamoDbFixture = _fixtureServices.GetRequiredService<IDynamoDbFixture>();
                DynamoDbFixture.EnsureTablesExist(_tables);

                SnsFixture = _fixtureServices.GetRequiredService<ISnsFixture>();
                SnsFixture.CreateSnsTopic<EntityEventSns>("equalityInformation.fifo", "EQUALITY_INFORMATION_SNS_ARN");

                Client = CreateClient();
            }
            catch
            {
                _fixtureServices.Dispose();
                throw;
            }
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _fixtureServices.Dispose();
                if (Client != null)
                    Client.Dispose();

                base.Dispose(disposing);

                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }
    }
}
