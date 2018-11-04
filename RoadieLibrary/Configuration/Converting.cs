﻿using System;

namespace Roadie.Library.Configuration
{
    [Serializable]
    public class Converting
    {
        public string APEConvertCommand { get; set; }
        public bool DoDeleteAfter { get; set; }
        public string M4AConvertCommand { get; set; }
        public string OGGConvertCommand { get; set; }
    }
}