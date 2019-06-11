﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roadie.Api.Hubs;
using Roadie.Library;
using Roadie.Library.Caching;
using Roadie.Library.Configuration;
using Roadie.Library.Encoding;
using Roadie.Library.Models;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Models.Users;
using Roadie.Library.Scrobble;
using Roadie.Library.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using data = Roadie.Library.Data;

namespace Roadie.Api.Services
{
    public class PlayActivityService : ServiceBase, IPlayActivityService
    {
        protected IScrobbleHandler ScrobblerHandler { get; }
        protected IHubContext<PlayActivityHub> PlayActivityHub { get; }

        public PlayActivityService(IRoadieSettings configuration,
                             IHttpEncoder httpEncoder,
                             IHttpContext httpContext,
                             data.IRoadieDbContext dbContext,
                             ICacheManager cacheManager,
                             ILogger<PlayActivityService> logger,
                             IScrobbleHandler scrobbleHandler,
                             IHubContext<PlayActivityHub> playActivityHub)
            : base(configuration, httpEncoder, dbContext, cacheManager, logger, httpContext)
        {
            this.PlayActivityHub = playActivityHub;
            this.ScrobblerHandler = scrobbleHandler;
        }

        public Task<Library.Models.Pagination.PagedResult<PlayActivityList>> List(PagedRequest request, User roadieUser = null, DateTime? newerThan = null)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var result = (from t in this.DbContext.Tracks
                              join rm in this.DbContext.ReleaseMedias on t.ReleaseMediaId equals rm.Id
                              join r in this.DbContext.Releases on rm.ReleaseId equals r.Id
                              join trackArtist in this.DbContext.Artists on t.ArtistId equals trackArtist.Id into tas
                              from trackArtist in tas.DefaultIfEmpty()
                              join usertrack in this.DbContext.UserTracks on t.Id equals usertrack.TrackId
                              join u in this.DbContext.Users on usertrack.UserId equals u.Id
                              join releaseArtist in this.DbContext.Artists on r.ArtistId equals releaseArtist.Id
                              where (newerThan == null || usertrack.LastPlayed >= newerThan)
                              where ((roadieUser == null && !(u.IsPrivate ?? false)) || (roadieUser != null && (usertrack != null && usertrack.User.Id == roadieUser.Id)))
                              where (!request.FilterRatedOnly || (roadieUser == null && t.Rating > 0 || roadieUser != null && usertrack.Rating >0))
                              where (request.FilterValue.Length == 0 || (request.FilterValue.Length > 0 && (
                                        t.Title != null && t.Title.ToLower().Contains(request.Filter.ToLower()) ||
                                        t.AlternateNames != null && t.AlternateNames.ToLower().Contains(request.Filter.ToLower())
                              )))
                              select new PlayActivityList
                              {
                                  Release = new DataToken
                                  {
                                      Text = r.Title,
                                      Value = r.RoadieId.ToString()
                                  },
                                  Track = TrackList.FromDataTrack(null,
                                                                  t,
                                                                  rm.MediaNumber,
                                                                  r,
                                                                  releaseArtist,
                                                                  trackArtist,
                                                                  this.HttpContext.BaseUrl,
                                                                  this.MakeTrackThumbnailImage(t.RoadieId),
                                                                  this.MakeReleaseThumbnailImage(r.RoadieId),
                                                                  this.MakeArtistThumbnailImage(releaseArtist.RoadieId),
                                                                  this.MakeArtistThumbnailImage(trackArtist == null ? null : (Guid?)trackArtist.RoadieId)),
                                  User = new DataToken
                                  {
                                      Text = u.UserName,
                                      Value = u.RoadieId.ToString()
                                  },
                                  Artist = new DataToken
                                  {
                                      Text = releaseArtist.Name,
                                      Value = releaseArtist.RoadieId.ToString()
                                  },
                                  TrackArtist = trackArtist == null ? null : new DataToken
                                  {
                                      Text = trackArtist.Name,
                                      Value = trackArtist.RoadieId.ToString()
                                  },
                                  PlayedDateDateTime = usertrack.LastPlayed,
                                  ReleasePlayUrl = $"{ this.HttpContext.BaseUrl }/play/release/{ r.RoadieId}",
                                  Rating = t.Rating,
                                  UserRating = usertrack.Rating,
                                  TrackPlayUrl = $"{ this.HttpContext.BaseUrl }/play/track/{ t.RoadieId}.mp3",
                                  ArtistThumbnail = this.MakeArtistThumbnailImage(trackArtist != null ? trackArtist.RoadieId : releaseArtist.RoadieId),
                                  ReleaseThumbnail = this.MakeReleaseThumbnailImage(r.RoadieId),
                                  UserThumbnail = this.MakeUserThumbnailImage(u.RoadieId)
                              });

                var sortBy = string.IsNullOrEmpty(request.Sort) ? request.OrderValue(new Dictionary<string, string> { { "PlayedDateDateTime", "DESC" } }) : request.OrderValue(null);
                var rowCount = result.Count();
                var rows = result.OrderBy(sortBy).Skip(request.SkipValue).Take(request.LimitValue).ToArray();
                sw.Stop();
                return Task.FromResult(new Library.Models.Pagination.PagedResult<PlayActivityList>
                {
                    TotalCount = rowCount,
                    CurrentPage = request.PageValue,
                    TotalPages = (int)Math.Ceiling((double)rowCount / request.LimitValue),
                    OperationTime = sw.ElapsedMilliseconds,
                    Rows = rows
                });
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex);
            }
            return Task.FromResult(new Library.Models.Pagination.PagedResult<PlayActivityList>());
        }

        public async Task<OperationResult<bool>> NowPlaying(User roadieUser, ScrobbleInfo scrobble)
        {
            var scrobbleResult = await this.ScrobblerHandler.NowPlaying(roadieUser, scrobble);
            if (!scrobbleResult.IsSuccess)
            {
                return scrobbleResult;
            }
            await PublishPlayActivity(roadieUser, scrobble, true);
            return scrobbleResult;
        }

        public async Task<OperationResult<bool>> Scrobble(User roadieUser, ScrobbleInfo scrobble)
        {
            var scrobbleResult = await this.ScrobblerHandler.Scrobble(roadieUser, scrobble);
            if(!scrobbleResult.IsSuccess)
            {
                return scrobbleResult;
            }
            await PublishPlayActivity(roadieUser, scrobble, false);
            return scrobbleResult;
        }

        private async Task PublishPlayActivity(User roadieUser, ScrobbleInfo scrobble, bool isNowPlaying)
        {
                // Only broadcast if the user is not public and played duration is more than half of duration
                if (!roadieUser.IsPrivate &&
                    scrobble.ElapsedTimeOfTrackPlayed.TotalSeconds > (scrobble.TrackDuration.TotalSeconds / 2))
                {
                var sw = Stopwatch.StartNew();
                var track = this.DbContext.Tracks
                                         .Include(x => x.ReleaseMedia)
                                         .Include(x => x.ReleaseMedia.Release)
                                         .Include(x => x.ReleaseMedia.Release.Artist)
                                         .Include(x => x.TrackArtist)
                                        .FirstOrDefault(x => x.RoadieId == scrobble.TrackId);
                var user = this.DbContext.Users.FirstOrDefault(x => x.RoadieId == roadieUser.UserId);
                var userTrack = this.DbContext.UserTracks.FirstOrDefault(x => x.UserId == roadieUser.Id && x.TrackId == track.Id);
                var pl = new PlayActivityList
                {
                    Artist = new DataToken
                    {
                        Text = track.ReleaseMedia.Release.Artist.Name,
                        Value = track.ReleaseMedia.Release.Artist.RoadieId.ToString()
                    },
                    TrackArtist = track.TrackArtist == null ? null : new DataToken
                    {
                        Text = track.TrackArtist.Name,
                        Value = track.TrackArtist.RoadieId.ToString()
                    },
                    Release = new DataToken
                    {
                        Text = track.ReleaseMedia.Release.Title,
                        Value = track.ReleaseMedia.Release.RoadieId.ToString()
                    },
                    Track = TrackList.FromDataTrack(null,
                                                                         track,
                                                                         track.ReleaseMedia.MediaNumber,
                                                                         track.ReleaseMedia.Release,
                                                                         track.ReleaseMedia.Release.Artist,
                                                                         track.TrackArtist,
                                                                         this.HttpContext.BaseUrl,
                                                                         this.MakeTrackThumbnailImage(track.RoadieId),
                                                                         this.MakeReleaseThumbnailImage(track.ReleaseMedia.Release.RoadieId),
                                                                         this.MakeArtistThumbnailImage(track.ReleaseMedia.Release.Artist.RoadieId),
                                                                         this.MakeArtistThumbnailImage(track.TrackArtist == null ? null : (Guid?)track.TrackArtist.RoadieId)),
                    User = new DataToken
                    {
                        Text = roadieUser.UserName,
                        Value = roadieUser.UserId.ToString()
                    },
                    ArtistThumbnail = this.MakeArtistThumbnailImage(track.TrackArtist != null ? track.TrackArtist.RoadieId : track.ReleaseMedia.Release.Artist.RoadieId),
                    PlayedDateDateTime = scrobble.TimePlayed,
                    IsNowPlaying = isNowPlaying,
                    Rating = track.Rating,
                    ReleasePlayUrl = $"{ this.HttpContext.BaseUrl }/play/release/{ track.ReleaseMedia.Release.RoadieId}",
                    ReleaseThumbnail = this.MakeReleaseThumbnailImage(track.ReleaseMedia.Release.RoadieId),
                    TrackPlayUrl = $"{ this.HttpContext.BaseUrl }/play/track/{ track.RoadieId}.mp3",
                    UserRating = userTrack?.Rating,
                    UserThumbnail = this.MakeUserThumbnailImage(roadieUser.UserId)
                };
                try
                {
                    await this.PlayActivityHub.Clients.All.SendAsync("SendActivity", pl);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex);
                }
            }
        }
    }
}