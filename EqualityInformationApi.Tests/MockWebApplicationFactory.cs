using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Hackney.Core.DynamoDb;
using Hackney.Core.Sns;
using Hackney.Core.Testing.DynamoDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
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

        public IAmazonSimpleNotificationService SimpleNotificationService { get; private set; }
        public IAmazonSQS AmazonSQS { get; private set; }
        public SnsEventVerifier<EntityEventSns> SnsVerifer { get; private set; }

        public IDynamoDbFixture DynamoDbFixture { get; private set; }
        public HttpClient Client { get; private set; }

        public MockWebApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            EnsureEnvVarConfigured("Sns_LocalMode", "true");
            EnsureEnvVarConfigured("Localstack_SnsServiceUrl", "http://localhost:4566");

            Client = CreateClient();
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        private void CreateSnsTopic()
        {
            var snsAttrs = new Dictionary<string, string>();
            snsAttrs.Add("fifo_topic", "true");
            snsAttrs.Add("content_based_deduplication", "true");

            var response = SimpleNotificationService.CreateTopicAsync(new CreateTopicRequest
            {
                Name = "equalityInformation",
                Attributes = snsAttrs
            }).Result;

            Environment.SetEnvironmentVariable("EQUALITY_INFORMATION_SNS_ARN", response.TopicArn);

            SnsVerifer = new SnsEventVerifier<EntityEventSns>(AmazonSQS, SimpleNotificationService, response.TopicArn);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<TStartup>();

            builder.ConfigureServices(services =>
            {
                services.ConfigureDynamoDB();
                services.ConfigureDynamoDbFixture();
                services.ConfigureSns();

                var serviceProvider = services.BuildServiceProvider();

                DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
                DynamoDbFixture.EnsureTablesExist(_tables);

                SimpleNotificationService = serviceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

                var localstackUrl = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");
                AmazonSQS = new AmazonSQSClient(new AmazonSQSConfig() { ServiceURL = localstackUrl });

                CreateSnsTopic();
            });
        }
    }
}
