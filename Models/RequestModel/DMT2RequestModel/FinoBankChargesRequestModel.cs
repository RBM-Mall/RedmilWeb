namespace Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel
{
    public class FinoBankChargesRequestModel
    {
        public string SenderMobile { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentAmount { get; set; }
        public string Checksum { get; set; }
        public string UserId { get; set; }
    }
}
