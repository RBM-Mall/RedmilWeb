namespace Project_Redmil_MVC.Models
{
    public class BaseResponseModel
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        
        //public object AdditionalInfo { get; internal set; }
    }

    public class BaseResponseModel2
    {
        public string? Key { get; set; }
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    public class BaseBillResponseModel
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? AdditionalInfo { get; set; }
    }
    public class BaseBillResponseModelNew
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? AdData { get; set; }
    }
    public class OrderBrandingBaseResponseModel
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public string? DeliveryCharges { get; set; }
        public object? Materials { get; set; }

    }
    public class GetSubscriptionPlanBaseResponseModel
    {

        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? AddOn { get; set; }
        public string? Pdfurl { get; set; }

    }

    public class BaseLICResponseModel
    {
        public string? Statuscode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? BillFetch { get; set; }

    }

    public class Bank
    {
        public string BankName1 { get; set; }
        public string BankLogo1 { get; set; }
        public string BankName2 { get; set; }
        public string BankLogo2 { get; set; }
        public string BankName3 { get; set; }
        public string BankLogo3 { get; set; }

        //List<SelfHelpResponseModel>? selfHelps { get; set; }
    }

    public class Kyc
    {
        public string KycStatus { get; set; }
        public string ExceptionKYCBankName { get; set; }
        public string ExceptionKYCRoutingStatus { get; set; }
    }

    public class BaseAepsRoutingDetailsResponseModel
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Bank>? Bank { get; set; }
        public List<Kyc>? Kyc { get; set; }
    }
    public class DepartmentOnCalllistBaseResponseModel
    {

        public string Statuscode { get; set; }
        public string Message { get; set; }
        public string To_time { get; set; }
        public string From_time { get; set; }
        public object Data { get; set; }
    }

    //public class SelfHelpResponseModel
    //{
    //    public int Id { get; set; }
    //    public string Question { get; set; }
    //    public string Answer { get; set; }
    //    public bool Status { get; set; }
    //    public DateTime Reqdate { get; set; }
    //}
}
