namespace FarmManagementAPI.FarmManagement.Core.Entities
{
    public class MpesaConfiguration
    {
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;
        public string BusinessShortCode { get; set; } = string.Empty;
        public string Passkey { get; set; } = string.Empty;
        public string Environment { get; set; } = "Sandbox";
    }
}