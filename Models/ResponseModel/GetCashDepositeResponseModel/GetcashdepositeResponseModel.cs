namespace Project_Redmil_MVC.Models.ResponseModel.GetCashDepositeResponseModel
{
    public class GetcashdepositeResponseModel
    {
        public int Id { get; set; }
        public string ModeName { get; set; }
        public string Title { get; set; }
        public string Statement { get; set; }
        public string Step1 { get; set; }
        public string Step2 { get; set; }
        public string Step3 { get; set; }
        public string Step4 { get; set; }
        public double Processingcharge { get; set; }
        public string Logo { get; set; }
        public string AMLLetter { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSECode { get; set; }
        public bool Status { get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime ReqDate { get; set; }
        public bool ProcessingchargeType { get; set; }
    }
}
