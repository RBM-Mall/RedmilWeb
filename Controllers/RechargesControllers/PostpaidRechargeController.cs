using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Helper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using RestSharp;
using System.Text.RegularExpressions;

namespace Project_Redmil_MVC.Controllers
{
    public class PostpaidRechargeController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public PostpaidRechargeController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        [HttpGet]
        public IActionResult PostpaidRecharge()
        {

            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            return View();
        }
        #region Postpaid Recharge

        [HttpPost]

        public JsonResult PostpaidRecharge(string Number, string Operator, string Circle, string Amount, string Payment)
        {
            if ((!string.IsNullOrEmpty(Number)) && (!string.IsNullOrEmpty(Operator)) && (!string.IsNullOrEmpty(Circle)) && (!string.IsNullOrEmpty(Amount)) && (!string.IsNullOrEmpty(Payment)))
            {
                PrepaidRechargeRequestModel prepaidRechargeRequestModel = new PrepaidRechargeRequestModel();
                try
                {
                    prepaidRechargeRequestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                    prepaidRechargeRequestModel.Token = "";
                    var op = ReplaceOperatorName(Operator);
                    var operaterData = GetOperatorList();
                    var responseOperators = operaterData.Value as List<ResponseOperator>;
                    prepaidRechargeRequestModel.OpId = responseOperators.Where(x => x.Operatorname == op).FirstOrDefault().Id.ToString();
                    prepaidRechargeRequestModel.ServiceId = responseOperators.Where(x => x.Operatorname == op).FirstOrDefault().ServiceId.ToString();
                    prepaidRechargeRequestModel.Mobileno = Number;
                    prepaidRechargeRequestModel.Mode = "App";
                    prepaidRechargeRequestModel.Amount = ToDigitsOnly(Amount);
                    prepaidRechargeRequestModel.Wallet = Payment;
                    #region Checksum (Recharge|Unique Key|UserId)
                    string input = Checksum.MakeChecksumString("Recharge", Checksum.checksumKey, prepaidRechargeRequestModel.Userid,
                        prepaidRechargeRequestModel.ServiceId.Trim(), prepaidRechargeRequestModel.OpId.Trim(), prepaidRechargeRequestModel.Mobileno.Trim(),
                        prepaidRechargeRequestModel.Mode, prepaidRechargeRequestModel.Amount.Trim(), prepaidRechargeRequestModel.Wallet);
                    string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                    #endregion
                    prepaidRechargeRequestModel.checksum = CheckSum;
                    //var client = new RestClient("https://api.redmilbusinessmall.com/api/Recharge");
                    var client = new RestClient($"{Baseurl}{ApiName.Recharge}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(prepaidRechargeRequestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    if (string.IsNullOrEmpty(result))
                    {
                        return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    else
                    {
                        var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                        var data = deserialize.Data;
                        if (deserialize.Statuscode == "TXN")
                        {
                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<PrepaidRechargeResponseModel>>>(response.Content);

                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                        }
                        else if (deserialize.Statuscode == "ERR")
                        {
                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<PrepaidRechargeResponseModel>>>(response.Content);

                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                        }
                        else
                        {
                            return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionLogRequestModel requestModel = new ExceptionLogRequestModel();
                    requestModel.ExceptionMessage = ex;
                    requestModel.Data = prepaidRechargeRequestModel;
                    var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
            }
            else
            {
                return Json("");
            }
        }

        #endregion

        #region GetMobileMNPDetails
        [HttpPost]
        public JsonResult GetMobileMNPDetails(string MobileNumber)
        {
            RequestModel1 requestModel = new RequestModel1();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.MobileNo = MobileNumber;
                #region Checksum (GetMobileMNPDetails|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("GetMobileMNPDetails", Checksum.checksumKey, requestModel.Userid, requestModel.MobileNo.Trim());
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                // var client = new RestClient("https://api.redmilbusinessmall.com/api/GetMobileMNPDetails");
                var client = new RestClient($"{Baseurl}{ApiName.GetMobileMNPDetails}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var datadeserialize = deserialize.Data;
                        var data = JsonConvert.DeserializeObject<ResponseOperatorDetails>(JsonConvert.SerializeObject(datadeserialize));
                        List<ResponseOperator> lstresponseOperator = new List<ResponseOperator>();
                        return Json(data);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }


            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = requestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion

        #region GetBalance
        [HttpPost]
        public JsonResult GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            try
            {
                getBalanceRequestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var data = deserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data));
                        List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                        lstdata = datalist.ToList();
                        return Json(lstdata);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return Json(deserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = getBalanceRequestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }

        #endregion


        private string ReplaceCircleName(string oldName)
        {
            switch (oldName)
            {

                case "Andhra Pradesh": return "AP";

                case "Assam": return "ASM";

                case "Bihar & Jharkhand": return "BIH";

                case "Chennai": return "CHE";

                case "Delhi": return "DEL";

                case "Gujrat": return "GUJ";

                case "Haryana": return "HAR";

                case "Himachal Pradesh": return "HP";

                case "J&K": return "JK";

                case "Karnataka": return "KK";

                case "Madhya Pradesh": return "MP";

                case "Maharashtra": return "MAH";
                case "Mumbai": return "MAH";

                case "NorthEast": return "NE";

                case "Orissa": return "ORI";

                case "Punjab": return "PUN";

                case "Rajasthan": return "RAJ";

                case "Tamilnadu": return "TN";

                case "Uttar Pradesh (East)": return "UPE";

                case "Uttar Pradesh (West)": return "UPW";

                case "Uttaranchal": return "UPW";

                case "WestBengal & AN Island": return "WB";

                case "All": return "ALL";

                default: return "ALL";
            }
        }

        private string ReplaceOperatorName(string oldName)
        {
            switch (oldName)
            {
                case "Vi": return "Vodafone Postpaid";
                case "Vodafone": return "Vodafone Postpaid";

                case "AIRTEL": return "Airtel Postpaid";
                case "Airtel": return "Airtel Postpaid";

                case "BSNL": return "BSNL Postpaid";

                case "Idea": return "Vodafone Postpaid";


                case "Jio": return "Jio Postpaid";
                case "Reliance Jio": return "Jio Postpaid";

                default: return "ALL";
            }
        }

        private string ToDigitsOnly(string input)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(input, "");
        }


        #region GetOperatorList
        [HttpPost]
        public JsonResult GetOperatorList()
        {
            var baseImg = "https://api.redmilbusinessmall.com";
            List<ResponseOperator> lstresponseOperator = new List<ResponseOperator>();
            RequestModel1 requestModel = new RequestModel1();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.ServiceId = "22";

                #region Checksum (addsender|Unique Key|UserId)

                string input = Checksum.MakeChecksumString("GetOperaterList", Checksum.checksumKey, requestModel.Userid, requestModel.ServiceId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion

                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/GetOperaterList");
                var client = new RestClient($"{Baseurl}{ApiName.GetOperaterList}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deseserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deseserialize.Statuscode == "TXN" && deseserialize != null)
                    {
                        var data = deseserialize.Data;
                        var datalist = JsonConvert.DeserializeObject<List<ResponseOperator>>(JsonConvert.SerializeObject(data));
                        if (datalist != null)
                        {

                            foreach (var item in datalist)
                            {
                                lstresponseOperator.Add(new ResponseOperator
                                {
                                    Id = item.Id,
                                    Operatorname = item.Operatorname,
                                    Opcode = item.Opcode,
                                    Img = baseImg + item.Img,
                                    ServiceId = item.ServiceId
                                });
                            }
                        }
                        return Json(lstresponseOperator);
                    }
                    else if (deseserialize.Statuscode == "ERR")
                    {
                        return Json(deseserialize);
                    }
                    else
                    {
                        return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }


            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = requestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion

    }
}
