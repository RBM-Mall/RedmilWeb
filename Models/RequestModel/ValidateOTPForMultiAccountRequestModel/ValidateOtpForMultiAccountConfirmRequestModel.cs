namespace Project_Redmil_MVC.Models.RequestModel.ValidateOTPForMultiAccountRequestModel
{
    public class ValidateOtpForMultiAccountConfirmRequestModel
    {
        public string Userid { get; set; }
        public string Mobile { get; set; }
        public string Otp { get; set; }
        public string checksum { get; set; }
    }
}
