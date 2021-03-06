﻿using Microsoft.AspNetCore.Http;
using Roadie.Library;
using Roadie.Library.Identity;
using Roadie.Library.Models;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roadie.Api.Services
{
    public interface IGenreService
    {
        Task<OperationResult<Genre>> ById(Library.Models.Users.User roadieUser, Guid id, IEnumerable<string> includes = null);

        Task<OperationResult<bool>> Delete(Library.Identity.User user, Guid id);

        Task<PagedResult<GenreList>> List(Library.Models.Users.User roadieUser, PagedRequest request, bool? doRandomize = false);

        Task<OperationResult<Image>> SetGenreImageByUrl(Library.Models.Users.User user, Guid id, string imageUrl);

        Task<OperationResult<bool>> UpdateGenre(Library.Models.Users.User user, Genre model);

        Task<OperationResult<Image>> UploadGenreImage(Library.Models.Users.User user, Guid id, IFormFile file);
    }
}