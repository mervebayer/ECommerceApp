namespace ECommerceApp.API.Models
{
    public class TestEmailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = "ECommerceApp Test Email";
        public string? Message { get; set; }
    }
}
