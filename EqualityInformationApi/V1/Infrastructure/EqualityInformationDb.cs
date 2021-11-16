using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<string>))]
        public string AgeGroup { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Gender>))]
        public Gender Gender { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<string>))]
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

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<string>))]
        public string NationalInsuranceNumber { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<LanguageInfo>))]
        public List<LanguageInfo> Languages { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<CaringResponsibilities>))]
        public CaringResponsibilities CaringResponsibilities { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<string>))]
        public string Disabled { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<List<string>>))]
        public List<string> CommunicationRequirements { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<EconomicSituation>))]
        public EconomicSituation EconomicSituation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<HomeSituation>))]
        public HomeSituation HomeSituation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbNullConverter<string>))]
        public string ArmedForces { get; set; }
    }

    public class DynamoDbNullConverter<T> : IPropertyConverter
    {
        public DynamoDBEntry ToEntry(object value)
        {
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty((string) value))
                {
                    return new DynamoDBNull();
                }
            }
            else
            if ((List<string>) value == null || !((List<string>) value).Any())
            {
                return new Primitive { Value = new List<string>() };
            }

            DynamoDBEntry entry = new Primitive { Value = value };

            return entry;
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            if (entry == null || entry.AsDynamoDBNull() != null)
                return string.Empty;
            if (typeof(T) == typeof(string))
                return entry.AsString();

            return entry.AsListOfString();
        }
    }
}
