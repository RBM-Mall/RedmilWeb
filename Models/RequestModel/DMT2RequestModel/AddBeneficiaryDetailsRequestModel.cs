namespace Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel
{
    public class AddBeneficiaryDetailsRequestModel
    {
        public string SenderId { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankName { get; set; }
        public string Checksum { get; set; }
        public string UserId { get; set; }
    }

}
