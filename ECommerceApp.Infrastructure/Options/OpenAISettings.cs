using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Options
{
    public class OpenAISettings
    {
        public const string SectionName = "OpenAISettings";

        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4.1-mini";
        public string Endpoint { get; set; } = "https://api.openai.com/v1/responses";
    }
}
