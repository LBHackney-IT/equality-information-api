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
    public class PatchEqualityInformationApiController : BaseController
    {
        private readonly ICreateUseCase _createUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public PatchEqualityInformationApiController(
            ICreateUseCase createUseCase,
            ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _createUseCase = createUseCase;

            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Patch([FromRoute] string id, [FromBody] PatchEqualityInformationObject request)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));
            request.Id = id;

            var response = await _createUseCase.Execute(request, token).ConfigureAwait(false);

            var location = $"/api/v1/equality-information/{response.Id}/?targetId={response.TargetId}";
            return Created(location, response);
        }
    }
}
