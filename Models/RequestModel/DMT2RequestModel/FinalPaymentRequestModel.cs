namespace Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel
{
    public class FinalPaymentRequestModel
    {
        public string UserId { get; set; }
        public string CustomerMobileNo { get; set; }
        public string CustomerName { get; set; }
        public string BeneIFSCCode { get; set; }
        public string BeneAccountNo { get; set; }
        public string BeneName { get; set; }
        public string Amount { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankName { get; set; }
        public string SenderId { get; set; }
        public string BeneficiaryId { get; set; }
        public string Checksum { get; set; }
        public string Wallet { get; set; }
    }
}
