namespace Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel
{
    public class GetBBPSBillsTmpResponseModel : BaseBillResponseModel
    {
        public Data data { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
        public Info info { get; set; }
        public class AdditionalInfo
        {
            public List<Info> info { get; set; }
        }

        public class Data
        {
            public string Billnumber { get; set; }
            public string Billperiod { get; set; }
            public string Duedate { get; set; }
            public string Customer_name { get; set; }
            public string Billamount { get; set; }
            public string ReqestNo { get; set; }
            public string Commission { get; set; }
            public string Surcharge { get; set; }
            public string billerResponseEnc { get; set; }
            public string inputParamsEnc { get; set; }
            public string additionalInfoEnc { get; set; }
            public List<AdditionalInfo> additionalInfo { get; set; }
            public List<Info> info { get; set; }
        }

        public class Info
        {
            public string infoName { get; set; }
            public string infoValue { get; set; }
        }
    }
}
