﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Roadie.Api.Services;
using Roadie.Library.Caching;
using Roadie.Library.Identity;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Models.Users;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Roadie.Api.Controllers
{
    [Produces("application/json")]
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UserController : EntityControllerBase
    {
        private IUserService UserService { get; }
        private readonly ITokenService TokenService;

        public UserController(IUserService userService, ILoggerFactory logger, ICacheManager cacheManager, IConfiguration configuration, ITokenService tokenService, UserManager<ApplicationUser> userManager)
            : base(cacheManager, configuration, userManager)
        {
            this.Logger = logger.CreateLogger("RoadieApi.Controllers.UserController");
            this.UserService = userService;
            this.TokenService = tokenService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await this.CurrentUserModel();
            var result = await this.CacheManager.GetAsync($"urn:user_model_by_id:{ id }", async () =>
            {
                return await this.UserService.ById(user, id);
            }, ControllerCacheRegionUrn);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(result);
        }

        [HttpPost("profile/edit")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await this.CurrentUserModel();
            var result = await this.UserService.UpdateProfile(user, model);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(ControllerCacheRegionUrn);
            var modelUser = await UserManager.FindByNameAsync(model.UserName);
            var t = await this.TokenService.GenerateToken(modelUser, this.UserManager);
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            var avatarUrl = $"{this.Request.Scheme}://{this.Request.Host}/images/user/{ modelUser.RoadieId }/{ this.RoadieSettings.ThumbnailImageSize.Width }/{ this.RoadieSettings.ThumbnailImageSize.Height }";
            return Ok(new
            {
                IsSuccess = true,
                Username = modelUser.UserName,
                modelUser.Email,
                modelUser.LastLogin,
                avatarUrl,
                Token = t,
                modelUser.Timeformat,
                modelUser.Timezone
            });
        }


        [HttpPost("setArtistRating/{releaseId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistRating(Guid releaseId, short rating)
        {
            var result = await this.UserService.SetArtistRating(releaseId, await this.CurrentUserModel(), rating);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseRating/{releaseId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseRating(Guid releaseId, short rating)
        {
            var result = await this.UserService.SetReleaseRating(releaseId, await this.CurrentUserModel(), rating);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackRating/{releaseId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackRating(Guid releaseId, short rating)
        {
            var result = await this.UserService.SetTrackRating(releaseId, await this.CurrentUserModel(), rating);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setArtistFavorite/{artistId}/{isFavorite}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistFavorite(Guid artistId, bool isFavorite)
        {
            var result = await this.UserService.SetArtistFavorite(artistId, await this.CurrentUserModel(), isFavorite);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseFavorite/{releaseId}/{isFavorite}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseFavorite(Guid releaseId, bool isFavorite)
        {
            var result = await this.UserService.SetReleaseFavorite(releaseId, await this.CurrentUserModel(), isFavorite);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setArtistBookmark/{artistId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistBookmark(Guid artistId, bool isBookmarked)
        {
            var result = await this.UserService.SetArtistBookmark(artistId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseBookmark/{releaseId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseBookmark(Guid releaseId, bool isBookmarked)
        {
            var result = await this.UserService.SetReleaseBookmark(releaseId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackBookmark/{trackId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackBookmark(Guid trackId, bool isBookmarked)
        {
            var result = await this.UserService.SetTrackBookmark(trackId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setPlaylistBookmark/{playlistId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetPlaylistBookmark(Guid playlistId, bool isBookmarked)
        {
            var result = await this.UserService.SetPlaylistBookmark(playlistId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setCollectionBookmark/{collectionId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetCollectionBookmark(Guid collectionId, bool isBookmarked)
        {
            var result = await this.UserService.SetCollectionBookmark(collectionId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }


        [HttpPost("setLabelBookmark/{labelId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetLabelBookmark(Guid labelId, bool isBookmarked)
        {
            var result = await this.UserService.SetLabelBookmark(labelId, await this.CurrentUserModel(), isBookmarked);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            this.CacheManager.ClearRegion(EntityControllerBase.ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> List([FromQuery]PagedRequest request)
        {
            var result = await this.UserService.List(request);
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(result);
        }

        public class PagingParams
        {
            public int Page { get; set; } = 1;
            public int Limit { get; set; } = 5;
        }
    }
}