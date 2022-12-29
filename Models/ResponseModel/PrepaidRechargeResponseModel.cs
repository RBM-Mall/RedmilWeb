namespace Project_Redmil_MVC.Models.ResponseModel
{
    public class PrepaidRechargeResponseModel:BaseResponseModel
    {
        public int Id { get; set; }
        public long Mobileno { get; set; }
        public double Amount { get; set; }
        public double OpBalane { get; set; }
        public string PMode { get; set; }
        public double ClBalane { get; set; }
        public DateTime Reqdate { get; set; }
        public string Status { get; set; }
        public string TRefId { get; set; }
        public string Response { get; set; }
        public string Status1 { get; set; }
    }
}
