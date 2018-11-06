﻿using Roadie.Library.Models.Releases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roadie.Library.Models
{
    [Serializable]
    public class CollectionRelease
    {
        public Release Release { get; set; }
        public int ListNumber { get; set; }
    }
}
