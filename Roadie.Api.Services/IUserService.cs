﻿using Roadie.Library;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roadie.Api.Services
{
    public interface IUserService
    {
        Task<OperationResult<User>> ById(User user, Guid id, IEnumerable<string> includes, bool isAccountSettingsEdit = false);

        Task<PagedResult<UserList>> List(PagedRequest request);

        Task<OperationResult<bool>> DeleteAllBookmarks(User roadieUser);

        Task<OperationResult<bool>> SetArtistBookmark(Guid artistId, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetArtistDisliked(Guid artistId, User roadieUser, bool isDisliked);

        Task<OperationResult<bool>> SetArtistFavorite(Guid artistId, User roadieUser, bool isFavorite);

        Task<OperationResult<short>> SetArtistRating(Guid artistId, User roadieUser, short rating);

        Task<OperationResult<bool>> SetCollectionBookmark(Guid collectionId, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetLabelBookmark(Guid labelId, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetPlaylistBookmark(Guid playlistId, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetReleaseBookmark(Guid releaseid, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetReleaseDisliked(Guid releaseId, User roadieUser, bool isDisliked);

        Task<OperationResult<bool>> SetReleaseFavorite(Guid releaseId, User roadieUser, bool isFavorite);

        Task<OperationResult<short>> SetReleaseRating(Guid releaseId, User roadieUser, short rating);

        Task<OperationResult<bool>> SetTrackBookmark(Guid trackId, User roadieUser, bool isBookmarked);

        Task<OperationResult<bool>> SetTrackDisliked(Guid trackId, User roadieUser, bool isDisliked);

        Task<OperationResult<bool>> SetTrackFavorite(Guid releaseId, User roadieUser, bool isFavorite);

        Task<OperationResult<short>> SetTrackRating(Guid trackId, User roadieUser, short rating);

        Task<OperationResult<bool>> UpdateIntegrationGrant(Guid userId, string integrationName, string token);

        Task<OperationResult<bool>> UpdateProfile(User userPerformingUpdate, User userBeingUpdatedModel);
    }
}