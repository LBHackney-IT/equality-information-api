using System.Threading.Tasks;
using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using Hackney.Core.JWT;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IPatchUseCase
    {
        Task<EqualityInformationResponseObject> Execute(PatchEqualityInformationRequest request,
            EqualityInformationObject requestObject, string requestBody, Token token, int? ifMatch);
    }
}
