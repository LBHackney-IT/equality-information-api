using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class GetEqualityInformationApiController : BaseController
    {
        private readonly IGetUseCase _getUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public GetEqualityInformationApiController(
            IGetUseCase getUseCase,
            ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _getUseCase = getUseCase;

            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
        }

        [ProducesResponseType(typeof(List<EqualityInformationObject>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Get([FromQuery] Guid targetId)
        {
            if (targetId == Guid.Empty)
                return BadRequest();

            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            var response = await _getUseCase.Execute(targetId, token).ConfigureAwait(false);

            if (response == null)
                return NotFound(response);

            return Ok(response);
        }
    }
}
