namespace Project_Redmil_MVC.Models.RequestModel.LICIndiaBillRequestModel
{
    public class PayLICIndiaBillRequestModel
    {
        public string? Userid { get; set; }
        public string? Token { get; set; }
        public string? canumber { get; set; }
        public string? checksum { get; set; }
        public string? amount { get; set; }
        public string? Wallet { get; set; }
        public string? ad1 { get; set; }
        public string? ad2 { get; set; }
        public string? ad3 { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
        public object? bill_fetch { get; set; }
       // public string? mode { get; set; }
    }
}
