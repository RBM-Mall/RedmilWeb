namespace Project_Redmil_MVC.Models.ResponseModel.LICIndiaBillResponseModel
{
    public class GetLICIndiaBillResponseModel : BaseLICResponseModel
    {
        public int response_code { get; set; }
        public bool status { get; set; }
        public string amount { get; set; }
        public string name { get; set; }
        public string duedate { get; set; }
        public BillFetch bill_fetch { get; set; }
        public string message { get; set; }
        
        public class BillFetch
        {
            public string CustomerName { get; set; }
            public string Netamount { get; set; }
            public string Duedatefromto { get; set; }
            public int status { get; set; }
        }





    }
}
