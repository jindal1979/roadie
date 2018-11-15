﻿using Roadie.Library.Models;
using Roadie.Library.Models.Pagination;
using Roadie.Library.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roadie.Api.Services
{
    public interface ILabelService
    {
        Task<PagedResult<LabelList>> List(User roadieUser, PagedRequest request, bool? doRandomize = false);
    }
}