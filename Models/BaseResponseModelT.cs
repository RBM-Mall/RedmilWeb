namespace Project_Redmil_MVC.Models
{
    public class BaseResponseModelT<T>
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        
    }

    public class BaseLICResponseModelT<T>
    {
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public T bill_fetch { get; set; }

        //public string CustomerName { get; set; }
        //public string Netamount { get; set; }
        //public string Duedatefromto { get; set; }
        //public int status { get; set; }
    }
    public class BaseResonseModelDMTSenderT<T>
    {
        
        public string Statuscode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public T AdData { get; set; }


    }

}
