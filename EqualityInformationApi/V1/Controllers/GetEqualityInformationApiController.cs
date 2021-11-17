using EqualityInformationApi.V1.Boundary.Request;
using EqualityInformationApi.V1.Factories;
using EqualityInformationApi.V1.Infrastructure;
using EqualityInformationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

        public GetEqualityInformationApiController(IGetUseCase getUseCase)
        {
            _getUseCase = getUseCase;
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

            HttpContext.Response.Headers.Add(HeaderConstants.ETag, EntityTagHeaderValue.Parse($"\"{eTag}\"").Tag);

            return Ok(response.ToResponse());
        }
    }
}
