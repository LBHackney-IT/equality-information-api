using EqualityInformationApi.V1.Boundary.Response;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
