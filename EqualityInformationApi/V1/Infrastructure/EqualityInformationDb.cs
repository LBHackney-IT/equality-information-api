using Amazon.DynamoDBv2.DataModel;
using EqualityInformationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EqualityInformationApi.V1.Infrastructure
{
    [DynamoDBTable("EqualityInformation", LowerCamelCaseProperties = true)]
    public class EqualityInformationDb
    {
        [DynamoDBHashKey]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string AgeGroup { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Gender>))]
        public Gender Gender { get; set; }

        [DynamoDBProperty]
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

        [DynamoDBProperty]
        public string NationalInsuranceNumber { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<Languages>))]
        public List<Languages> Languages { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<CaringResponsibilities>))]
        public CaringResponsibilities CaringResponsibilities { get; set; }

        [DynamoDBProperty]
        public string Disabled { get; set; }

        [DynamoDBProperty]
        public List<string> CommunicationRequirements { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<EconomicSituation>))]
        public EconomicSituation EconomicSituation { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<HomeSituation>))]
        public HomeSituation HomeSituation { get; set; }

        [DynamoDBProperty]
        public string ArmedForces { get; set; }
    }
}
