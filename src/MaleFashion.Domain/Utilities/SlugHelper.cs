using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MaleFashion.Domain.Utilities
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            var slug = input.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            return slug.Trim('-');
        }
    }
}
