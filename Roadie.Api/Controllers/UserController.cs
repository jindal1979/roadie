﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Roadie.Api.Services;
using Roadie.Library.Caching;
using Roadie.Library.Configuration;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Roadie.Api.Controllers
{
    [Produces("application/json")]
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UserController : EntityControllerBase
    {
        private readonly ITokenService TokenService;

        private IHttpContext RoadieHttpContext { get; }

        private IUserService UserService { get; }

        public UserController(IUserService userService, ILogger<UserController> logger, ICacheManager cacheManager,
                              ITokenService tokenService, UserManager<Library.Identity.User> userManager,
            IHttpContext httpContext, IRoadieSettings roadieSettings)
            : base(cacheManager, roadieSettings, userManager)
        {
            Logger = logger;
            UserService = userService;
            TokenService = tokenService;
            RoadieHttpContext = httpContext;
        }

        [HttpGet("accountsettings/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAccountSettings(Guid id, string inc = null)
        {
            var user = await CurrentUserModel().ConfigureAwait(false);
            var result = await CacheManager.GetAsync($"urn:user_edit_model_by_id:{id}", async () => await UserService.ById(user, id, (inc ?? Library.Models.Users.User.DefaultIncludes).ToLower().Split(","), true).ConfigureAwait(false), ControllerCacheRegionUrn).ConfigureAwait(false);
            if (result?.IsNotFoundResult != false)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            result.AdditionalClientData = new Dictionary<string, object>();
            if (RoadieSettings.Integrations.LastFmProviderEnabled)
            {
                var lastFmCallBackUrl = $"{RoadieHttpContext.BaseUrl}/users/integration/grant?userId={user.UserId}&iname=lastfm";
                result.AdditionalClientData.Add("lastFMIntegrationUrl", $"http://www.last.fm/api/auth/?api_key={RoadieSettings.Integrations.LastFMApiKey}&cb={WebUtility.UrlEncode(lastFmCallBackUrl)}");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid id, string inc = null)
        {
            var user = await CurrentUserModel().ConfigureAwait(false);
            var result = await CacheManager.GetAsync($"urn:user_model_by_id:{id}",
                async () => await UserService.ById(user, id, (inc ?? Library.Models.Users.User.DefaultIncludes).ToLower().Split(",")).ConfigureAwait(false), ControllerCacheRegionUrn).ConfigureAwait(false);
            if (result?.IsNotFoundResult != false)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("integration/grant")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> IntegrationGrant(Guid userId, string iname, string token)
        {
            var result = await UserService.UpdateIntegrationGrant(userId, iname, token).ConfigureAwait(false);
            if (!result.IsSuccess) return Content("Error while attempting to enable integration");
            return Content("Successfully enabled integration!");
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> List([FromQuery] PagedRequest request)
        {
            var result = await UserService.List(request).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            return Ok(result);
        }

        [HttpPost("setArtistBookmark/{artistId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistBookmark(Guid artistId, bool isBookmarked)
        {
            var result = await UserService.SetArtistBookmark(artistId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setArtistDisliked/{artistId}/{isDisliked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistDisliked(Guid artistId, bool isDisliked)
        {
            var result = await UserService.SetArtistDisliked(artistId, await CurrentUserModel().ConfigureAwait(false), isDisliked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setArtistFavorite/{artistId}/{isFavorite}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistFavorite(Guid artistId, bool isFavorite)
        {
            var result = await UserService.SetArtistFavorite(artistId, await CurrentUserModel().ConfigureAwait(false), isFavorite).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setArtistRating/{releaseId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetArtistRating(Guid releaseId, short rating)
        {
            var result = await UserService.SetArtistRating(releaseId, await CurrentUserModel().ConfigureAwait(false), rating).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("deleteAllBookmarks")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteAllBookmarks()
        {
            var result = await UserService.DeleteAllBookmarks(await CurrentUserModel().ConfigureAwait(false)).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setCollectionBookmark/{collectionId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetCollectionBookmark(Guid collectionId, bool isBookmarked)
        {
            var result = await UserService.SetCollectionBookmark(collectionId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setLabelBookmark/{labelId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetLabelBookmark(Guid labelId, bool isBookmarked)
        {
            var result = await UserService.SetLabelBookmark(labelId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setPlaylistBookmark/{playlistId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetPlaylistBookmark(Guid playlistId, bool isBookmarked)
        {
            var result = await UserService.SetPlaylistBookmark(playlistId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseBookmark/{releaseId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseBookmark(Guid releaseId, bool isBookmarked)
        {
            var result = await UserService.SetReleaseBookmark(releaseId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseDisliked/{releaseId}/{isDisliked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseDisliked(Guid releaseId, bool isDisliked)
        {
            var result = await UserService.SetReleaseDisliked(releaseId, await CurrentUserModel().ConfigureAwait(false), isDisliked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseFavorite/{releaseId}/{isFavorite}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseFavorite(Guid releaseId, bool isFavorite)
        {
            var result = await UserService.SetReleaseFavorite(releaseId, await CurrentUserModel().ConfigureAwait(false), isFavorite).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setReleaseRating/{releaseId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetReleaseRating(Guid releaseId, short rating)
        {
            var result = await UserService.SetReleaseRating(releaseId, await CurrentUserModel().ConfigureAwait(false), rating).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackBookmark/{trackId}/{isBookmarked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackBookmark(Guid trackId, bool isBookmarked)
        {
            var result = await UserService.SetTrackBookmark(trackId, await CurrentUserModel().ConfigureAwait(false), isBookmarked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackDisliked/{trackId}/{isDisliked}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackDisliked(Guid trackId, bool isDisliked)
        {
            var result = await UserService.SetTrackDisliked(trackId, await CurrentUserModel().ConfigureAwait(false), isDisliked).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackFavorite/{trackId}/{isFavorite}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackFavorite(Guid trackId, bool isFavorite)
        {
            var result = await UserService.SetTrackFavorite(trackId, await CurrentUserModel().ConfigureAwait(false), isFavorite).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("setTrackRating/{trackId}/{rating}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SetTrackRating(Guid trackId, short rating)
        {
            var result = await UserService.SetTrackRating(trackId, await CurrentUserModel().ConfigureAwait(false), rating).ConfigureAwait(false);
            if (!result.IsSuccess) return StatusCode((int)HttpStatusCode.InternalServerError);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            return Ok(result);
        }

        [HttpPost("profile/edit")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProfile(Library.Models.Users.User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await CurrentUserModel().ConfigureAwait(false);
            var result = await UserService.UpdateProfile(user, model).ConfigureAwait(false);
            if (result?.IsNotFoundResult != false) return NotFound();
            if (!result.IsSuccess)
            {
                if (result.IsAccessDeniedResult)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden);
                }
                if (result.Messages?.Any() ?? false)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, result.Messages);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            var modelUser = await UserManager.FindByNameAsync(model.UserName).ConfigureAwait(false);
            var t = await TokenService.GenerateToken(modelUser, UserManager).ConfigureAwait(false);
            CacheManager.ClearRegion(ControllerCacheRegionUrn);
            var avatarUrl = $"{RoadieHttpContext.ImageBaseUrl}/user/{modelUser.RoadieId}/{RoadieSettings.ThumbnailImageSize.Width}/{RoadieSettings.ThumbnailImageSize.Height}";
            return Ok(new
            {
                IsSuccess = true,
                Username = modelUser.UserName,
                modelUser.RemoveTrackFromQueAfterPlayed,
                modelUser.Email,
                modelUser.LastLogin,
                avatarUrl,
                Token = t,
                modelUser.Timeformat,
                modelUser.Timezone,
                DefaultRowsPerPage = modelUser.DefaultRowsPerPage ?? RoadieSettings.DefaultRowsPerPage
            });
        }

        public class PagingParams
        {
            public int Limit { get; set; } = 5;
            public int Page { get; set; } = 1;
        }
    }
}