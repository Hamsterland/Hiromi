using System.Collections.Generic;
using System.Linq;
using Hiromi.Data.Models.Tags;

namespace Hiromi.Services.Tags
{
    public static class TagUtilities
    {
        private const int MaxTagNameLength = 50;
        private const int MaxTagsPerUser = 25;
        
        public static bool IsWithinNameLimit(string name)
        {
            return name.Length <= MaxTagNameLength;
        }

        public static bool IsWithinMaxTagLimit(IEnumerable<TagSummary> tags)
        {
            return tags.Count() <= MaxTagsPerUser;
        }
    }
}