using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Options
{
    public class AppUrlSettings
    {
        public const string SectionName = "AppUrlSettings";

        public string FrontendBaseUrl { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
    }
}
