using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/equality-information")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class EqualityInformationApiController : BaseController
    {
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly ICreateUseCase _createUseCase;
        private readonly IGetByIdUseCase _getByIdUseCase;
        private readonly IUpdateUseCase _updateUseCase;
        public EqualityInformationApiController(
            IGetAllUseCase getAllUseCase,
            ICreateUseCase createUseCase,
            IGetByIdUseCase getByIdUseCase,
            IUpdateUseCase updateUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _updateUseCase = updateUseCase;
        }

        [ProducesResponseType(typeof(GetAllResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetAll([FromQuery] EqualityInformationQuery query)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Create([FromBody] EqualityInformationObject request)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetById([FromQuery] GetByIdQuery query)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Update([FromQuery] UpdateQualityInformationQuery query, [FromBody] EqualityInformationObject request)
        {
            throw new NotImplementedException();
        }
    }
}
