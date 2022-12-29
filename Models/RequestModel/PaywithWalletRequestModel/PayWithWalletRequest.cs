namespace Project_Redmil_MVC.Models.RequestModel.PaywithWalletRequestModel
{
    public class PayWithWalletRequest
    {
        public string? UserId { get; set; }
        public string? Amount { get; set; }
        public string? checksum { get; set; }
        public string? DiscountedAmount { get; set; }
        public string? CouponCode { get; set; }
        public string? PlanId { get; set; }
        public string? AddOnItems { get; set; }
        public string? Wallet { get; set; }
    }
}
