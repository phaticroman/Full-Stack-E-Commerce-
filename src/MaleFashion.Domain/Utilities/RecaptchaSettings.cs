using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Utilities
{
    public class RecaptchaSettings
    {
        public string SiteKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}
