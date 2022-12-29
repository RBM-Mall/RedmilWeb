namespace Project_Redmil_MVC.Models.ResponseModel.LICIndiaBillResponseModel
{
    public class PayLICIndiaBillResponseModel:BaseResponseModel
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public int ServiceId { get; set; }
        public int OpId { get; set; }
        public double Amount { get; set; }
        public double Comm { get; set; }
        public bool CommType { get; set; }
        public double CommAount { get; set; }
        public double Surcharge { get; set; }
        public bool SurType { get; set; }
        public double SurAmount { get; set; }
        public double Gst { get; set; }
        public double Tds { get; set; }
        public double Rewards { get; set; }
        public double Cost { get; set; }
        public double OpBalane { get; set; }
        public double ClBalane { get; set; }
        public string PMode { get; set; }
        public int ApiId { get; set; }
        public string Status { get; set; }
        public DateTime Editdate { get; set; }
        public DateTime Reqdate { get; set; }
        public object TransType { get; set; }
        public object SenderName { get; set; }
        public object TQuery { get; set; }
        public int DirectId { get; set; }
        public double DirectCommission { get; set; }
        public bool DirectCommissionType { get; set; }
        public double DirectCommissionAmount { get; set; }
        public int InDirectId { get; set; }
        public double InDirectCommission { get; set; }
        public bool InDirectCommissionType { get; set; }
        public double InDirectCommissionAmount { get; set; }
        public int SuperInDirectId { get; set; }
        public double SuperInDirectCommission { get; set; }
        public bool SuperInDirectCommissionType { get; set; }
        public double SuperInDirectCommissionAmount { get; set; }
        public string CaNumber { get; set; }
        public string Ad1 { get; set; }
        public string Ad2 { get; set; }
        public string Ad3 { get; set; }
        public string ReferenceId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string BillNo { get; set; }
        public double BillAmount { get; set; }
        public double BillNetAmount { get; set; }
        public DateTime Billdate { get; set; }
        public bool AcceptPayment { get; set; }
        public bool AcceptPartPayment { get; set; }
        public DateTime DueFrom { get; set; }
        public DateTime DueTo { get; set; }
        public string ValidationId { get; set; }
        public string BillId { get; set; }
        public string Mobileno { get; set; }
        public string ResponseMsg { get; set; }
        public string CustomerName { get; set; }
    }
}
