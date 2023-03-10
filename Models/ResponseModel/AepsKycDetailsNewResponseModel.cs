namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class AepsKycDetailsNewResponseModel : Bank
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankLogo { get; set; }
        public AaadharPicVerificationResponseModal aaadharPics { get; set; }
    }
    public class AepsRoutingDetailsResponseModel
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public object Bank { get; set; }
        public object Kyc { get; set; }
    }

    public class AaadharPicVerificationResponseModal
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
    public class AepsKycDetailsStatusPendingResponseModel
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankLogo { get; set; }

    }
    }


