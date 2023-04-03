namespace Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel
{
    public class AepsOnboardingResponseModel:BaseResponseModel
    {
        public string MerchantKey { get; set; }
        public string IsOnboard { get; set; }
        public string IsKyc { get; set; }
    }
}
