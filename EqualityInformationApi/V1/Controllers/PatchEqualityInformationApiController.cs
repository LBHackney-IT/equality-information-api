using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Infrastructure.Exceptions;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Hackney.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeaderConstants = EqualityInformationApi.V1.Infrastructure.HeaderConstants;

namespace EqualityInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/equality-information")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PatchEqualityInformationApiController : BaseController
    {
        private readonly IPatchUseCase _patchUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public PatchEqualityInformationApiController(
            IPatchUseCase patchUseCase,
            ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _patchUseCase = patchUseCase;

            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
        }

        private int? GetIfMatchFromHeader()
        {
            var header = HttpContext.Request.Headers.GetHeaderValue(HeaderConstants.IfMatch);

            if (header == null)
                return null;

            _ = EntityTagHeaderValue.TryParse(header, out var entityTagHeaderValue);

            if (entityTagHeaderValue == null)
                return null;

            var version = entityTagHeaderValue.Tag.Replace("\"", string.Empty);

            if (int.TryParse(version, out var numericValue))
                return numericValue;

            return null;
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Patch([FromRoute] Guid id, [FromBody] PatchEqualityInformationObject request)
        {
            var ifMatch = GetIfMatchFromHeader();
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));
            request.Id = id;

            try
            {
                var response = await _patchUseCase.Execute(request, token, ifMatch).ConfigureAwait(false);
                if (response is null) return NotFound(id);

                return Ok(response);
            }
            catch (VersionNumberConflictException vncErr)
            {
                return Conflict(vncErr.Message);
            }
        }
    }
}
