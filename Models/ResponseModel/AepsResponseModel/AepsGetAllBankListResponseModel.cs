namespace Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel
{
    public class AepsGetAllBankListResponseModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public int IIN { get; set; }
        public int AcquirerId { get; set; }
        public string BankCode { get; set; }
        public bool Status { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? ReqDate { get; set; }
        public string BankLogo { get; set; }
    }
}
