namespace Project_Redmil_MVC.Models.RequestModel.BusinessReportRequestModel
{
    public class CancelBusTicketsRequestModel
    {
        public string Userid { get; set; }
        public string checksum { get; set; }
        public string seatsToCancel { get; set; }
        public string tin { get; set; }
    }
}
