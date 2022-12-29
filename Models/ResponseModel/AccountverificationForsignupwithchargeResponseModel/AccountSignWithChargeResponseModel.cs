namespace Project_Redmil_MVC.Models.ResponseModel.AccountverificationForsignupwithchargeResponseModel
{
    public class AccountSignWithChargeResponseModel:ResponseModel1
    {
        public int Id { get; set; }
        public long SenderMobile { get; set; }
        public string SenderName { get; set; }
        public double Surcharge { get; set; }
        public double Amount { get; set; }
        public string BeniName { get; set; }
        public string BeniNumber { get; set; }
        public string BankName { get; set; }
        public string Ifsc { get; set; }
        public string Account { get; set; }
        public string TransType { get; set; }
        public DateTime Reqdate { get; set; }
        public string Status { get; set; }
    }
}
