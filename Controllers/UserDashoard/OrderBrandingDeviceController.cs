using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.ResponseModel.OrderBrandingResponseModel;

namespace Project_Redmil_MVC.Controllers.UserDashoard
{
    public class OrderBrandingDeviceController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public OrderBrandingDeviceController(IConfiguration config)
        {
            _config = config;
            Baseurl = "https://api.redmilbusinessmall.com/api/"; HelperMethod.GetBaseURl(_config);

        }
        public IActionResult Index()
        {
            return View();
        }

        #region OrderBrandingDevice
        public IActionResult OrderBrandingDevice()
        {
           
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            else
            {
                try
                {
                    //var baseUrl = _config.GetSection("ApiUrl").GetSection("BaseUrl").Value;
                    var baseUrl = "https://api.redmilbusinessmall.com";
                    var datainsert = new List<OrderBrandingResponseModel>();
                    List<OrderBrandingResponseModel> lstresponse = new List<OrderBrandingResponseModel>();
                    var Materials = new List<OrderBrandingDeviceController>();
                    #region Checksum (addsender|Unique Key|UserId)
                    string input = Checksum.MakeChecksumString("ShowMaterial", Checksum.checksumKey);
                    string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                    #endregion
                    //var client = new RestClient("https://api.redmilbusinessmall.com/api/User/ValidateUser");
                    var client = new RestClient($"{Baseurl}{ApiName.ShowMaterial}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    if (string.IsNullOrEmpty(result))
                    {
                        return RedirectToAction("ErrorForExceptionLog", "Error");
                    }
                    else
                    {
                        var Orderbranddata = JsonConvert.DeserializeObject<OrderBrandingBaseResponseModel>(result);
                        if(Orderbranddata.Statuscode=="TXN" && Orderbranddata != null)
                        {
                            var data = Orderbranddata.Materials;
                            var datalist = JsonConvert.DeserializeObject<List<OrderBrandingResponseModel>>(JsonConvert.SerializeObject(data));
                            lstresponse = datalist.ToList();
                            //    if (!string.IsNullOrEmpty(Cat))
                            //    {
                            //        var a = baseUrl + lstresponse.Where(x => x.Title == Cat).FirstOrDefault().ImgLink;
                            //        //baseUrl + item.ImgLink
                            //        return Json(a);

                            //}
                            if (lstresponse != null)
                            {
                                foreach (var item in lstresponse)
                                {
                                    datainsert.Add(new OrderBrandingResponseModel
                                    {
                                        Id = item.Id,
                                        MaterialName = item.MaterialName,
                                        SOrder = item.SOrder,
                                        Usage = item.Usage,
                                        Amount = item.Amount,
                                        MaterialType = item.MaterialType,
                                        ImgSample = baseUrl + item.ImgSample,
                                        ImgThumbnail = baseUrl + item.ImgThumbnail

                                    });
                                }

                                return View(datainsert);

                            }
                            return View(lstresponse);
                        }
                        else if (Orderbranddata.Statuscode == "ERR")
                        {
                            return View(lstresponse);
                        }
                        else
                        {
                            return RedirectToAction("ErrorForExceptionLog", "Error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                    requestModel1.ExceptionMessage = ex;
                    requestModel1.Data = "";
                    var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel1);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
            }
        }

        #endregion

        [HttpPost]
        public JsonResult  OrdeBrandingDevice()
        {
            return Json("");   

        }
        public IActionResult Product()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                TempData["Message"] = "Please Login First";
                return RedirectToAction("Index", "Home");

            }
            return View();
        }
        public IActionResult Checkout(string materialname,string Quantity,string detail)
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                TempData["Message"] = "Please Login First";
                return RedirectToAction("Index", "Home");

            }
            return View();
        }
        [HttpPost]
        public JsonResult checkout(string materialname, string Quantity, string detail)
        {

            return Json("");
        }

    }
}
