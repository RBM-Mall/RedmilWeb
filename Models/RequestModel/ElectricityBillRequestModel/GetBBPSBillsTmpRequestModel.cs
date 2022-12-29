namespace Project_Redmil_MVC.Models.RequestModel.ElectricityBillRequestModel
{
    public class GetBBPSBillsTmpRequestModel
    {
        public string Userid { get; set; }
        public string Token { get; set; }
        public string checksum { get; set; }

        public string Mobileno { get; set; }

        public string Account { get; set; }

        public string BillerId { get; set; }
        public string InputParam1 { get; set; }

        public string InputParam2 { get; set; }

        public string ccf { get; set; }

        public string Ip_address { get; set; }
    }
}
