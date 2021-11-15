using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DocumentModel;

namespace EqualityInformationApi.V1.Infrastructure
{
    [DynamoDBTable("EqualityInformation", LowerCamelCaseProperties = true)]
    public class EqualityInformationDb
    {
        [DynamoDBHashKey]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey]
        public Guid Id { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public string AgeGroup { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Gender>))]
        public Gender Gender { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public string Nationality { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Ethnicity>))]
        public Ethnicity Ethnicity { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<ReligionOrBelief>))]
        public ReligionOrBelief ReligionOrBelief { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<SexualOrientation>))]
        public SexualOrientation SexualOrientation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<MarriageOrCivilPartnership>))]
        public MarriageOrCivilPartnership MarriageOrCivilPartnership { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<PregnancyOrMaternity>))]
        public List<PregnancyOrMaternity> PregnancyOrMaternity { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public string NationalInsuranceNumber { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<LanguageInfo>))]
        public List<LanguageInfo> Languages { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<CaringResponsibilities>))]
        public CaringResponsibilities CaringResponsibilities { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public string Disabled { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public List<string> CommunicationRequirements { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<EconomicSituation>))]
        public EconomicSituation EconomicSituation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<HomeSituation>))]
        public HomeSituation HomeSituation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter))]
        public string ArmedForces { get; set; }
    }

    public class DynamoDbNullConverter : IPropertyConverter
    {
        private static JsonSerializerOptions CreateJsonOptions()
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions();
            serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            serializerOptions.WriteIndented = true;
            serializerOptions.Converters.Add((JsonConverter) new JsonStringEnumConverter());
            return serializerOptions;
        }

        public DynamoDBEntry ToEntry(object value) => value == null ? (DynamoDBEntry) new DynamoDBNull() : (DynamoDBEntry) Document.FromJson(JsonSerializer.Serialize<object>(value, DynamoDbNullConverter.CreateJsonOptions()));

        public object FromEntry(DynamoDBEntry entry)
        {
            if (entry == null || entry.AsDynamoDBNull() != null)
                return (object) null;

            return entry.AsString();
        }
    }
}
