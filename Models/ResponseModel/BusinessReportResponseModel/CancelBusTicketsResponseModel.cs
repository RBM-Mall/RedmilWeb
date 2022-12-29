namespace Project_Redmil_MVC.Models.ResponseModel.BusinessReportResponseModel
{
    public class CancelBusTicketsResponseModel
    {
        public string cancellationCharge { get; set; }
        public string refundAmount { get; set; }
        public string refundServiceTax { get; set; }
        public string serviceTaxOnCancellationCharge { get; set; }
        public string tin { get; set; }
    }
}
