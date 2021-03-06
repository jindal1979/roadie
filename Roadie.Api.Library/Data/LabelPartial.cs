﻿using Roadie.Library.Configuration;
using Roadie.Library.Extensions;
using Roadie.Library.Utility;
using System;
using System.IO;

namespace Roadie.Library.Data
{
    public partial class Label
    {
        public string CacheKey => CacheUrn(RoadieId);

        public string CacheRegion => CacheRegionUrn(RoadieId);

        public string Etag
        {
            get
            {
                return HashHelper.CreateMD5($"{ RoadieId }{ LastUpdated }");
            }
        }

        /// <summary>
        ///     Returns a full file path to the Label Image
        /// </summary>
        public string PathToImage(IRoadieSettings configuration, bool makeFolderIfNotExist = false)
        {
            var folder = FolderPathHelper.LabelPath(configuration, SortNameValue);
            if (!Directory.Exists(folder) && makeFolderIfNotExist)
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, $"{ SortNameValue.ToFileNameFriendly() } [{ Id }].jpg");
        }

        public bool IsValid => !string.IsNullOrEmpty(Name);

        public static string CacheRegionUrn(Guid Id)
        {
            return string.Format("urn:label:{0}", Id);
        }

        public static string CacheUrn(Guid Id)
        {
            return $"urn:label_by_id:{Id}";
        }

        public override string ToString()
        {
            return $"Id [{Id}], Name [{Name}], RoadieId [{RoadieId}]";
        }
    }
}