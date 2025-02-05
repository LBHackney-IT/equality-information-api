using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Factories;
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
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeaderConstants = EqualityInformationApi.V1.Infrastructure.HeaderConstants;

namespace EqualityInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/equality-information")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class EqualityInformationApiController : BaseController
    {
        private readonly ICreateUseCase _createUseCase;
        private readonly IGetUseCase _getUseCase;
        private readonly IPatchUseCase _patchUseCase;

        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;

        public EqualityInformationApiController(
            ICreateUseCase createUseCase,
            IGetUseCase getUseCase,
            IPatchUseCase patchUseCase,
            ITokenFactory tokenFactory,
            IHttpContextWrapper contextWrapper)
        {
            _createUseCase = createUseCase;
            _getUseCase = getUseCase;
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
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Create([FromBody] EqualityInformationObject request)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            var response = await _createUseCase.Execute(request, token).ConfigureAwait(false);

            var location = $"/api/v1/equality-information/{response.Id}/?targetId={response.TargetId}";
            return Created(location, response);
        }

        [ProducesResponseType(typeof(List<EqualityInformationObject>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Get([FromQuery] Guid targetId)
        {
            var response = await _getUseCase.Execute(targetId).ConfigureAwait(false);
            if (response is null) return NotFound(targetId);

            var eTag = string.Empty;
            if (response.VersionNumber.HasValue)
                eTag = response.VersionNumber.ToString();

            HttpContext.Response.Headers.Append(HeaderConstants.ETag, EntityTagHeaderValue.Parse($"\"{eTag}\"").Tag);

            return Ok(response.ToResponse());
        }

        [ProducesResponseType(typeof(EqualityInformationObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> Patch([FromRoute] PatchEqualityInformationRequest request, [FromBody] EqualityInformationObject requestObject)
        {
            // get raw body text (Only the parameters that need to be changed will be sent.
            // Deserializing the request object makes it imposible to figure out if the requester
            // wants to set a parameter to null, or to not update that value.
            // The bodyText is the raw request object that will be used to determine this information).
            var bodyText = await HttpContext.Request.GetRawBodyStringAsync().ConfigureAwait(false);

            var ifMatch = GetIfMatchFromHeader();
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            try
            {
                var response = await _patchUseCase.Execute(request, requestObject, bodyText, token, ifMatch).ConfigureAwait(false);
                if (response is null) return NotFound(request.Id);

                return Ok(response);
            }
            catch (VersionNumberConflictException vncErr)
            {
                return Conflict(vncErr.Message);
            }
        }
    }
}
