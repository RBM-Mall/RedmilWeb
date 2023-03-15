namespace Project_Redmil_MVC.Models.ResponseModel.UpgradeSubscriptionResponseModel
{

    public class UpgradeSubscriptionAddOnResponseModel:GetSubscriptionPlanBaseResponseModel
    {
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public string? Icon { get; set; }
        public double Price { get; set; }
        public DateTime ReqDate { get; set; }
        public string? Desc { get; set; }
        public string? PlanName { get; set; }
        public int PlanId { get; set; }
        public string? PlanType { get; set; }

        //public List<UpgradeSubscriptionDatumResponseModel> upgradeSubscriptions { get; set; }

    }
    public class UpgradeSubscriptionDatumResponseModel
    {
        public int Id { get; set; }
        public string? PlanName { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public double GST { get; set; }
        public double DirectComm { get; set; }
        public double InDirectComm { get; set; }
        public bool Status { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime ReqDate { get; set; }
        public string? Img { get; set; }
        public string? PlanType { get; set; }
        public string? SampleImg { get; set; }
        public string?  Subscribed{ get; set; }

    }
    

}
