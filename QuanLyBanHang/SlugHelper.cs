using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace QuanLyBanHang
{
    public static class SlugHelper
    {
        public static string GenerateSlug(this string text)
        {
            text = text.ToLower();
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-").Trim('-');
            return text;
        }
    }
}
