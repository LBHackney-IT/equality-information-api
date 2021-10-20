using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EqualityInformationApi.V1.Infrastructure
{
    [DynamoDBTable("Equality Information", LowerCamelCaseProperties = true)]
    public class DatabaseEntity
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }
    }
}
