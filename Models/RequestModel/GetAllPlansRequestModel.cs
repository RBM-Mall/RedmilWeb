namespace Project_Redmil_MVC.Models.RequestModel
{
    public class GetAllPlansRequestModel
    {
        public string Userid { get; set; }
        public string checksum { get; set; }
        public string ServiceId { get; set; }

        public string MobileNo { get; set; }
        public string Circle { get; set; }
        public string OpName { get; set; }
    }
}
