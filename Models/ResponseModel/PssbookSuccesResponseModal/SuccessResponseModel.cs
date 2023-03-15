namespace Project_Redmil_MVC.Models.ResponseModel.PssbookSuccesResponseModal
{
    public class SuccessResponseModel
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public int ServiceId { get; set; }
        public int OpId { get; set; }
        public int ApiId { get; set; }
        public int ModeId { get; set; }
        public int BankId { get; set; }
        public string BeniName { get; set; }
        public string Account { get; set; }
        public string IFSC { get; set; }
        public double Surcharge { get; set; }
        public bool SurType { get; set; }
        public string Request { get; set; }
        public string  Response { get; set; }
        public string Status { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime ReqDate { get; set; }
        public double Amount { get; set; }
        public string bankReferenceNo { get; set; }
        public string Remark { get; set; }
        public bool VoiceStatus { get; set; }
        public int LanguageId { get; set; }
        public string VoiceMessage { get; set; }
    }
}
