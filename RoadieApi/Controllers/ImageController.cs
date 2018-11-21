﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Roadie.Api.Services;
using Roadie.Library.Caching;
using Roadie.Library.Identity;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Roadie.Api.Controllers
{
    [Produces("application/json")]
    [Route("images")]
    [ApiController]
    //  [Authorize]
    public class ImageController : EntityControllerBase
    {
        private IImageService ImageService { get; }

        public ImageController(IImageService imageService, ILoggerFactory logger, ICacheManager cacheManager, IConfiguration configuration, UserManager<ApplicationUser> userManager)
            : base(cacheManager, configuration, userManager)
        {
            this.Logger = logger.CreateLogger("RoadieApi.Controllers.ImageController");
            this.ImageService = imageService;
        }

        //[EnableQuery]
        //public IActionResult Get()
        //{
        //    return Ok(this._RoadieDbContext.Tracks.ProjectToType<models.Image>());
        //}

        [HttpGet("artist/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ArtistImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.ArtistImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }


        [HttpGet("collection/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CollectionImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.CollectionImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        [HttpPost("{id}")]
        [Authorize(Policy = "Editor")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await this.ImageService.Delete(await this.CurrentUserModel(), id);
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

        [HttpGet("{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.ById(id, width, height);
            if (result == null)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        [HttpGet("label/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> LabelImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.LabelImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        [HttpGet("playlist/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PlaylistImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.PlaylistImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        [HttpGet("release/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReleaseImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.ReleaseImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        [HttpGet("track/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TrackImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.TrackImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.jpg",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }

        /// <summary>
        /// NOTE that user images/avatars are PNG not JPG this is so it looks better in the menus/applications
        /// </summary>
        [HttpGet("user/{id}/{width:int?}/{height:int?}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UserImage(Guid id, int? width, int? height)
        {
            var result = await this.ImageService.UserImage(id, width ?? this.RoadieSettings.ThumbnailImageSize.Width, height ?? this.RoadieSettings.ThumbnailImageSize.Height);
            if (result == null || result.IsNotFoundResult)
            {
                return NotFound();
            }
            if (!result.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return File(fileContents: result.Data.Bytes,
                        contentType: result.ContentType,
                        fileDownloadName: $"{ result.Data.Caption ?? id.ToString()}.png",
                        lastModified: result.LastModified,
                        entityTag: result.ETag);
        }
    }
}