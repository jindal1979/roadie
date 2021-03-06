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
    public interface ILabelService
    {
        Task<OperationResult<Label>> ById(Library.Models.Users.User roadieUser, Guid id, IEnumerable<string> includes = null);

        Task<OperationResult<bool>> Delete(Library.Identity.User user, Guid id);

        Task<PagedResult<LabelList>> List(Library.Models.Users.User roadieUser, PagedRequest request, bool? doRandomize = false);

        Task<OperationResult<bool>> MergeLabelsIntoLabel(Library.Identity.User user, Guid intoLabelId, IEnumerable<Guid> labelIdsToMerge);

        Task<OperationResult<Image>> SetLabelImageByUrl(Library.Models.Users.User user, Guid id, string imageUrl);

        Task<OperationResult<bool>> UpdateLabel(Library.Models.Users.User user, Label label);

        Task<OperationResult<Image>> UploadLabelImage(Library.Models.Users.User user, Guid id, IFormFile file);
    }
}