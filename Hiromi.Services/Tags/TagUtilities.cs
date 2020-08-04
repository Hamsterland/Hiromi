using System;
using System.Collections.Generic;
using System.Linq;
using Hiromi.Data.Models.Tags;

namespace Hiromi.Services.Tags
{
    public class TagUtilities
    {
        private const int MAX_TAG_NAME_LENGTH = 50;
        private const int MAX_TAGS_PER_USER = 15;
        
        public static bool IsWithinNameLimit(string name)
        {
            return name.Length <= MAX_TAG_NAME_LENGTH;
        }

        public static bool IsWithinMaxTagLimit(IEnumerable<TagSummary> tags)
        {
            return tags.Count() <= MAX_TAGS_PER_USER;
        }
    }
}