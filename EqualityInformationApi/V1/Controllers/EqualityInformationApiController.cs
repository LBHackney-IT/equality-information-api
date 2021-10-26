using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Boundary.Response;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EqualityInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/equality-information")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class EqualityInformationApiController : BaseController
    {
        private readonly ICreateUseCase _createUseCase;

        public EqualityInformationApiController(ICreateUseCase createUseCase)
        {
            _createUseCase = createUseCase;
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Create([FromBody] EqualityInformationObject request)
        {
            var response = await _createUseCase.Execute(request).ConfigureAwait(false);

            var location = $"/api/v1/equality-information/{response.Id}/?targetId={response.TargetId}";
            return Created(location, response);
        }
    }
}
