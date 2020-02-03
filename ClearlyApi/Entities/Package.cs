using System;
using ClearlyApi.Enums;

namespace ClearlyApi.Entities
{
    public class Package : PersistantObject
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int Price { get; set; }

        public PackageType Type { get; set; }
    }
}
