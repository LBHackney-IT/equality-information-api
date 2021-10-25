using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IGetAllUseCase
    {
        Task<GetAllResponseObject> Execute(EqualityInformationQuery query);
    }
}
