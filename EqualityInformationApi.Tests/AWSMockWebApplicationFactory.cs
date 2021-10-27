using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hackney.Core.DynamoDb;
using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace EqualityInformationApi.Tests
{
    public class AWSMockWebApplicationFactory<TStartup>
            : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables;

        public IAmazonDynamoDB DynamoDb { get; private set; }
        public IDynamoDBContext DynamoDbContext { get; private set; }
        public IAmazonSimpleNotificationService SimpleNotificationService { get; private set; }
        public IAmazonSQS AmazonSQS { get; private set; }

        public AWSMockWebApplicationFactory(List<TableDef> tables)
        {
            _tables = tables;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();

            builder.ConfigureServices(services =>
            {
                var url = Environment.GetEnvironmentVariable("DynamoDb_LocalServiceUrl");

                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = url };
                    return new AmazonDynamoDBClient(clientConfig);
                });

                services.ConfigureDynamoDB();
                services.ConfigureSns();

                var serviceProvider = services.BuildServiceProvider();
                DynamoDb = serviceProvider.GetRequiredService<IAmazonDynamoDB>();
                DynamoDbContext = serviceProvider.GetRequiredService<IDynamoDBContext>();
                SimpleNotificationService = serviceProvider.GetRequiredService<IAmazonSimpleNotificationService>();

                var localstackUrl = Environment.GetEnvironmentVariable("Localstack_SnsServiceUrl");
                AmazonSQS = new AmazonSQSClient(new AmazonSQSConfig() { ServiceURL = localstackUrl });

                EnsureTablesExist(DynamoDb, _tables);
            });
        }

        private static void EnsureTablesExist(IAmazonDynamoDB dynamoDb, List<TableDef> tables)
        {
            foreach (var table in tables)
            {
                try
                {
                    var request = new CreateTableRequest(table.Name,
                        new List<KeySchemaElement> { new KeySchemaElement(table.KeyName, KeyType.HASH) },
                        new List<AttributeDefinition> { new AttributeDefinition(table.KeyName, table.KeyType) },
                        new ProvisionedThroughput(3, 3));

                    // add range keys
                    request.KeySchema.Add(new KeySchemaElement(table.RangeKeyName, KeyType.RANGE));
                    request.AttributeDefinitions.Add(new AttributeDefinition(table.RangeKeyName, table.RangeKeyType));

                    _ = dynamoDb.CreateTableAsync(request).GetAwaiter().GetResult();
                }
                catch (ResourceInUseException)
                {
                    // It already exists :-)
                }
            }
        }
    }
}
