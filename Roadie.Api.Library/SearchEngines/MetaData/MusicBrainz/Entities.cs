﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Roadie.Library.MetaData.MusicBrainz
{
    public class AttributeValues
    {
    }

    [Serializable]
    public class CoverArtArchive
    {
        public bool artwork { get; set; }
        public bool back { get; set; }
        public int? count { get; set; }

        public bool darkened { get; set; }
        public bool front { get; set; }
    }

    public class Label
    {
        public List<Alias> aliases { get; set; }
        public string disambiguation { get; set; }
        public string id { get; set; }

        [JsonProperty(PropertyName = "label-code")]
        public int? labelcode { get; set; }

        public string name { get; set; }

        [JsonProperty(PropertyName = "sort-name")]
        public string sortname { get; set; }

        public NameAndCount[] tags { get; set; }
        public NameAndCount[] genres { get; set; }
    }

    [Serializable]
    public class LabelInfo
    {
        [JsonProperty(PropertyName = "catalog-number")]
        public string catalognumber { get; set; }

        public Label label { get; set; }
    }


    [Serializable]
    public class Medium
    {
        public object format { get; set; }
        public int? position { get; set; }
        public string title { get; set; }

        [JsonProperty(PropertyName = "track-count")]
        public short? trackcount { get; set; }
        [JsonProperty(PropertyName = "track-offset")]
        public int? trackoffset { get; set; }

        public List<Track> tracks { get; set; }
    }

    [Serializable]
    public class Recording
    {
        public List<Alias> aliases { get; set; }
        public string disambiguation { get; set; }
        public string id { get; set; }
        public int? length { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public NameAndCount[] tags { get; set; }
        public NameAndCount[] genres { get; set; }
    }

    [Serializable]
    public class Relation
    {
        public List<object> attributes { get; set; }
        public AttributeValues attributevalues { get; set; }
        public object begin { get; set; }
        public string direction { get; set; }
        public object end { get; set; }
        public bool ended { get; set; }
        public string sourcecredit { get; set; }
        public string targetcredit { get; set; }
        public string targettype { get; set; }
        public string type { get; set; }
        public string typeid { get; set; }
        public MbUrl url { get; set; }
    }

    [Serializable]
    public class RepositoryRelease
    {
        public int Id { get; set; }
        public string ArtistMbId { get; set; }
        public Release Release { get; set; }
    }

    [DebuggerDisplay("title: {title}, date: {date}")]
    [Serializable]
    public class Release
    {
        public List<object> aliases { get; set; }
        public string asin { get; set; }
        public string barcode { get; set; }
        public string country { get; set; }

        [JsonProperty(PropertyName = "cover-art-archive")]
        public CoverArtArchive coverartarchive { get; set; }

        public string coverThumbnailUrl { get; set; }
        public string date { get; set; }
        public string disambiguation { get; set; }
        public string id { get; set; }
        public List<string> imageUrls { get; set; }

        [JsonProperty(PropertyName = "label-info")]
        public List<LabelInfo> labelinfo { get; set; }

        public List<Medium> media { get; set; }
        public string packaging { get; set; }
        public string quality { get; set; }
        public List<Relation> relations { get; set; }

        [JsonProperty(PropertyName = "release-events")]
        public List<ReleaseEvents> releaseevents { get; set; }
        [JsonProperty(PropertyName = "release-group")]
        public ReleaseGroup releasegroup { get; set; }
        public string status { get; set; }
        [JsonProperty(PropertyName = "text-representation")]
        public TextRepresentation textrepresentation { get; set; }
        public string title { get; set; }
        [JsonProperty("artist-credits")]
        public Artist[] artistcredits { get; set; }
    }

    [Serializable]
    public class ReleaseBrowseResult
    {
        [JsonProperty(PropertyName = "release-count")]
        public int? releasecount { get; set; }

        [JsonProperty(PropertyName = "release-offset")]
        public int? releaseoffset { get; set; }

        public List<Release> releases { get; set; }
    }

    [Serializable]
    public class ReleaseEvents
    {
        public Area area { get; set; }

        public string date { get; set; }
    }

    [Serializable]
    public class ReleaseGroup
    {
        public List<object> aliases { get; set; }
        public string disambiguation { get; set; }
        [JsonProperty("first-release-date")]
        public string firstreleasedate { get; set; }
        public string id { get; set; }
        [JsonProperty("primary-type")]
        public string primarytype { get; set; }
        public List<object> secondarytypes { get; set; }
        public string title { get; set; }
        public NameAndCount[] tags { get; set; }
        public NameAndCount[] genres { get; set; }
    }

    [Serializable]
    public class TextRepresentation
    {
        public string language { get; set; }
        public string script { get; set; }
    }

    [Serializable]
    public class Track
    {
        public string id { get; set; }
        public int? length { get; set; }

        public string number { get; set; }
        public string position { get; set; }
        public Recording recording { get; set; }

        public string title { get; set; }

    }

    [Serializable]
    public class MbUrl
    {
        public string id { get; set; }
        public string resource { get; set; }
    }
}