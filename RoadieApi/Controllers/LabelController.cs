﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Roadie.Api.Services;
using Roadie.Library.Caching;
using Roadie.Library.Identity;
using Roadie.Library.Models.Pagination;
using System.Net;
using System.Threading.Tasks;

namespace Roadie.Api.Controllers
{
    [Produces("application/json")]
    [Route("labels")]
    [ApiController]
    [Authorize]
    public class LabelController : EntityControllerBase
    {
        private ILabelService LabelService { get; }

        public LabelController(ILabelService labelService, ILoggerFactory logger, ICacheManager cacheManager, IConfiguration configuration, UserManager<ApplicationUser> userManager)
            : base(cacheManager, configuration, userManager)
        {
            this._logger = logger.CreateLogger("RoadieApi.Controllers.LabelController");
            this.LabelService = labelService;
        }

        //[EnableQuery]
        //public IActionResult Get()
        //{
        //    return Ok(this._RoadieDbContext.Labels.ProjectToType<models.Label>());
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //public IActionResult Get(Guid id)
        //{
        //    var key = id.ToString();
        //    var result = this._cacheManager.Get<models.Label>(key, () =>
        //    {
        //        var d = this._RoadieDbContext.Labels.FirstOrDefault(x => x.RoadieId == id);
        //        if (d != null)
        //        {
        //            return d.Adapt<models.Label>();
        //        }
        //        return null;
        //    }, key);
        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> List([FromQuery]PagedRequest request)
        {
            var result = await this.LabelService.List(roadieUser: await this.CurrentUserModel(),
                                                        request: request);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(result);
        }
    }
}