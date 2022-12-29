
namespace Project_Redmil_MVC.CommonHelper
{
    public class HelperMethod
    {
       // public readonly static IConfiguration _config;

       // public readonly static ILogger _logger;

       //public HelperMethod(ILogger<HelperMethod> logger, IConfiguration config)
       // {
       //     //logger = _logger;
       //     config = _config;
       // }
        public static string GetBaseURl(IConfiguration _config)
        {

            //string j = i;
            string usertype = _config.GetSection("ApiUrl").GetSection("BaseURlType").Value;
            string url = string.Empty;
            switch (usertype)
            {
                case "Live":
                    url = "https://api.redmilbusinessmall.com/api/";
                    break;

                case "1S":
                    url = "https://proapitest1.redmilbusinessmall.com/api/";
                    break;
                case "1U":
                    url = "http://uatapi1.redmilbusinessmall.com/api/";
                    break;
                case "1L":
                    url = "http://localhost:/api/";
                    break;

                case "2S":
                    url = "https://proapitest2.redmilbusinessmall.com/api/";
                    break;
                case "2U":
                    url = "http://uatapi2.redmilbusinessmall.com/api/";
                    break;
                case "2L":
                    url = "http://localhost:/api/";
                    break;

                case "3S":
                    url = "https://proapitest3.redmilbusinessmall.com/api/";
                    break;
                case "3U":
                    url = "http://uatapi3.redmilbusinessmall.com/api/";
                    break;
                case "3L": 
                    url = "http://localhost:/api/";
                    break;

                case "4S":
                    url = "https://proapitest4.redmilbusinessmall.com/api/";
                    break;
                case "4U":
                    url = "http://uatapi4.redmilbusinessmall.com/api/";
                    break;
                case "4L":
                    url = "http://localhost:/api/";
                    break;

                case "5S":
                    url = "https://proapitest5.redmilbusinessmall.com/api/";
                    break;
                case "5U":
                    url = "http://uatapi5.redmilbusinessmall.com/api/";
                    break;
                case "5L":
                    url = "http://localhost:50746/api/";
                    break;

                case "6S":
                    url = "https://proapitest6.redmilbusinessmall.com/api/";
                    break;
                case "6U":
                    url = "http://uatapi6.redmilbusinessmall.com/api/";
                    break;
                case "6L":
                    url = "http://localhost:/api/";
                    break;

                case "7S":
                    url = "https://proapitest7.redmilbusinessmall.com/api/";
                    break;
                case "7U":
                    url = "http://uatapi7.redmilbusinessmall.com/api/";
                    break;
                case "7L":
                    url = "http://localhost:/api/";
                    break;

                case "8S":
                    url = "https://proapitest8.redmilbusinessmall.com/api/";
                    break;
                case "8U":
                    url = "http://uatapi8.redmilbusinessmall.com/api/";
                    break;
                case "8L":
                    url = "http://localhost:/api/";
                    break;
                default:
                    url = "https://api.redmilbusinessmall.com/api/";
                    break;

            }
            return url;
        }
    }
}
