namespace Project_Redmil_MVC.Models.RequestModel.AepsRequestModel
{
    public class SendOTPForFingpayKYCRequestModel
    {
        public string UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MobileNo { get; set; }
        public string checksum { get; set; }
    }
}
