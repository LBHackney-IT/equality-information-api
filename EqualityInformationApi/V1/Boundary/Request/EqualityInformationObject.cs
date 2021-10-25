using EqualityInformationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Boundary.Request
{
    public class EqualityInformationObject
    {
        public Guid TargetId { get; set; }
        public Gender Gender { get; set; }
        public string Nationality { get; set; }
        public Ethnicity Ethnicity { get; set; }
        public ReligionOrBelief ReligionOrBelief { get; set; }
        public SexualOrientation SexualOrientation { get; set; }
        public MarriageOrCivilPartnership MarriageOrCivilPartnership { get; set; }
        public List<PregnancyOrMaternity> PregnancyOrMaternity { get; set; }
        public string NationalInsuranceNumber { get; set; }
        public List<Languages> Languages { get; set; }
        public CaringResponsibilities CaringResponsibilities { get; set; }
        public bool Disabled { get; set; }
        public List<string> CommunicationRequirements { get; set; }
        public EconomicSituation EconomicSituation { get; set; }
        public HomeSituation HomeSituation { get; set; }
        public string ArmedForces { get; set; }
    }
}
