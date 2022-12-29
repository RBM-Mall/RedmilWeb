namespace Project_Redmil_MVC.Models.ResponseModel.UpgradeSubscriptionResponseModel
{
    public class ApplyCoupanCodeResponseModel:BaseResponseModel
    {
        public string CouponCode { get; set; }
        public double DiscountedAmount { get; set; }
    }
}
