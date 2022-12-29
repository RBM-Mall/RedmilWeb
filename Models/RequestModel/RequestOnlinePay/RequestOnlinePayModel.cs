namespace Project_Redmil_MVC.Models.RequestOnline.RequestOnlinePay
{
    public class RequestOnlinePayModel
    {
        public string? Userid { get; set; }
        public string? Amount { get; set; }
        public string? checksum { get; set; }
        public string?  Mode { get; set; }
        public string? OrderId { get; set; }
        public string? Token { get; set; }
        public string? RequestData { get; set; }
        public string? PlanType { get; set; }
        public string? AddOnItems { get; set; }

    }
}
