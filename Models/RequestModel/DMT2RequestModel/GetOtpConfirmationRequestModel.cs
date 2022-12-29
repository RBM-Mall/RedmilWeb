namespace Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel
{
    public class GetOtpConfirmationRequestModel
    {
        public string OTP { get; set; }
        public string SenderMobile { get; set; }
        public string UserId { get; set; }
        public string Checksum { get; set; }

    }
}
