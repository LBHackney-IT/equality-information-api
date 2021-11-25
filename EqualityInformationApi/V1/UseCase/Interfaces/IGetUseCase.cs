using EqualityInformationApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.UseCase.Interfaces
{
    public interface IGetUseCase
    {
        Task<EqualityInformation> Execute(Guid targetId);
    }
}
