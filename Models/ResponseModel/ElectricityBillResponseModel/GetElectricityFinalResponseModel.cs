namespace Project_Redmil_MVC.Models.ResponseModel.ElectricityBillResponseModel
{
    public class GetElectricityFinalResponseModel
    {

        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string Account { get; set; }
        public object BeniName { get; set; }
        public long Mobileno { get; set; }
        public double Amount { get; set; }
        public double OpBalane { get; set; }
        public string PMode { get; set; }
        public double ClBalane { get; set; }
        public string Reqdate { get; set; }
        public string Status { get; set; }
        public string InputParam1 { get; set; }
        public string InputParam2 { get; set; }
        public double TotAmt { get; set; }
        public double ccf { get; set; }
        public string Request { get; set; }
        public string BillerName { get; set; }
        public string Response { get; set; }
    }

    

}
