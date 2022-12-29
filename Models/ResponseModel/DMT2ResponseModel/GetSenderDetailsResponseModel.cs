namespace Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel
{
    public class GetSenderDetailsResponseModel:BaseBillResponseModel
    {
        public List<Data> data { get; set; }
        public List<AdditionalInfo> additionalInfo { get; set; }

        public class Data
        {
            public int SenderId { get; set; }
            public string SenderMobile { get; set; }
            public string SenderName { get; set; }
            public double MonthlyLimit { get; set; }
            public double UsedLimit { get; set; }
        }


        public class AdditionalInfo
        {
            public int SenderId { get; set; }
            public string BeneficiaryName { get; set; }
            public int BenificiaryId { get; set; }
            public string AccountNumber { get; set; }
            public string IFSCCode { get; set; }
            public string BankName { get; set; }
            public string Status { get; set; }
        }

    }
}
