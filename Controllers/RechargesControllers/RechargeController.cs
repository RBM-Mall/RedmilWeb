using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.RequestModel;
using System.Data;
using System.Text.RegularExpressions;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Project_Redmil_MVC.Controllers.RechargesControllers
{
    public class RechargeController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public RechargeController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        [HttpGet]
        public IActionResult PrepaidRecharge()
        {

            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            return View();
        }

        [HttpPost]
        public JsonResult PrepaidRecharge(string Number, string Operator, string Circle, string Amount, string Payment)
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
                    //prepaidRechargeRequestModel.Mobileno = Number;
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
                        if (deserialize != null && deserialize.Statuscode == "TXN")
                        {
                            var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<PrepaidRechargeResponseModel>>>(response.Content);

                            return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                        }
                        else if (deserialize != null && deserialize.Statuscode == "ERR")
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


        #region GetOperatorList
        [HttpPost]
        public JsonResult GetOperatorList()
        {
            RequestModel1 requestModel = new RequestModel1();
            try
            {
                var baseImg = "https://api.redmilbusinessmall.com";
                List<ResponseOperator> lstresponseOperator = new List<ResponseOperator>();
                
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.ServiceId = "21";

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
                                    Img1 = baseImg + item.Img,
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
                        return Json("");
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
            }
            return Json("");
        }
        #endregion

        #region GetAllPlans

        [HttpPost]
        public JsonResult GetAllPlans(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            RequestModel1 requestModel = new RequestModel1();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                // var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                ////Create request with GET
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deseserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deseserialize.Statuscode == "TXN" && deseserialize != null)
                {
                    var data = deseserialize.Data;
                    var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                    lstGetAllPlans = datalist.ToList();
                    return Json(lstGetAllPlans);
                }
                else if (deseserialize.Statuscode == "ERR")
                {
                    return Json(deseserialize);
                }
                else
                {
                    return Json("");
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
            }

            return Json("");
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
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/GetMobileMNPDetails");
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
                ExceptionLogRequestModel requestModel2 = new ExceptionLogRequestModel();
                requestModel2.ExceptionMessage = ex;
                requestModel2.Data = requestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel2);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion
        #region GetAllPlansCombo
        [HttpPost]
        public JsonResult GetAllPlansCombo(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Combo Pack");
                        return Json(a);
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

        #region GetAllPlansInternetPack
        [HttpPost]
        public JsonResult GetAllPlansInternetPack(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Internet Pack");
                        return Json(a);
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
        #region GetAllPlansTopUp

        [HttpPost]
        public JsonResult GetAllPlansTopUp(string circle, string opname)
        {

            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Topup Plan");
                        return Json(a);
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


        #region GetAllPlansUnlimited
        [HttpPost]
        public JsonResult GetAllPlansUnlimited(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Unlimited Pack" || x.PlanName == "Unlimited Packs");
                        return Json(a);
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
        #endregion #region GetAllPlansUnlimited

        #region GetAllPlansVAS
        [HttpPost]
        public JsonResult GetAllPlansVAS(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {

                requestModel.Userid = "2084";
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "VAS" || x.PlanName == "Full Talktime" || x.PlanName == "Validity Extension");
                        // var deserialize = JsonConvert.DeserializeObject<BaseResponseModel<GetAllPlansResponseModel>>(response.Content);
                        return Json(a);
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

        #region GetAllPlansFullTalktime
        [HttpPost]
        public JsonResult GetAllPlansFullTalktime(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Full Talktime");
                        return Json(a);
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


        #region GetAllPlansInternationalRoaming
        [HttpPost]
        public JsonResult GetAllPlansInternationalRoaming(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "International Roaming");
                        return Json(a);
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


        #region GetAllPlansISD
        [HttpPost]
        public JsonResult GetAllPlansISD(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "ISD Plan" || x.PlanName == "Prime ISD Plan");
                        return Json(a);
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


        #region GetAllPlansSpecialRecharge
        [HttpPost]
        public JsonResult GetAllPlansSpecialRecharge(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Special Recharge");
                        // var deserialize = JsonConvert.DeserializeObject<BaseResponseModel<GetAllPlansResponseModel>>(response.Content);
                        return Json(a);
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


        #region GetAllPlansValidityExtension
        [HttpPost]
        public JsonResult GetAllPlansValidityExtension(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                // var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Validity Extention");
                        return Json(a);
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



        #region GetAllPlans4GDataVoucher
        [HttpPost]
        public JsonResult GetAllPlans4GDataVoucher(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "4G Data Voucher");
                        return Json(a);
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


        #region GetAllPlansJioPhone
        [HttpPost]
        public JsonResult GetAllPlansJioPhone(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Jio Phone All in One Plan");
                        return Json(a);
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

        #region GetAllPlansJioPhoneData
        [HttpPost]
        public JsonResult GetAllPlansJioPhoneData(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Jio Phone Data Add On");
                        return Json(a);
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

        #region GetAllPlansPrime1GBPlan
        [HttpPost]
        public JsonResult GetAllPlansPrime1GBPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime 1 GB Plan");
                        // var deserialize = JsonConvert.DeserializeObject<BaseResponseModel<GetAllPlansResponseModel>>(response.Content);
                        return Json(a);
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

        #region GetAllPlansPrime15GBPlan
        [HttpPost]
        public JsonResult GetAllPlansPrime15GBPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime 1.5 GB Plan");
                        // var deserialize = JsonConvert.DeserializeObject<BaseResponseModel<GetAllPlansResponseModel>>(response.Content);
                        return Json(a);
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

        #region GetAllPlansPrime2GBPlan
        [HttpPost]
        public JsonResult GetAllPlansPrime2GBPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime 2 GB Plan");
                        return Json(a);

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

        #region GetAllPlansPrime25GBPlan
        [HttpPost]
        public JsonResult GetAllPlansPrime25GBPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime 2.5 GB Plan");
                        return Json(a);
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

        #region GetAllPlansPrime3GBPlan
        [HttpPost]
        public JsonResult GetAllPlansPrime3GBPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime 3 GB Plan");
                        return Json(a);
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

        #region GetAllPlansNoLimitPlan
        [HttpPost]
        public JsonResult GetAllPlansNoLimitPlan(string circle, string opname)
        {
            List<GetAllPlansResponseModel> lstGetAllPlans = new List<GetAllPlansResponseModel>();
            GetAllPlansRequestModel requestModel = new GetAllPlansRequestModel();
            try
            {
                requestModel.Userid = HttpContext.Session.GetString("Id").ToString();
                requestModel.Circle = ReplaceCircleName(circle);
                requestModel.OpName = ReplaceOperatorName(opname);
                #region Checksum (JRIBrowsPlan|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("JRIBrowsPlan", Checksum.checksumKey, requestModel.Userid, requestModel.Circle + "|" + requestModel.OpName);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                requestModel.checksum = CheckSum;
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/JRIBrowsPlan");
                var client = new RestClient($"{Baseurl}{ApiName.JRIBrowsPlan}");
                //var client = new RestClient(baseUrl + "GetOperatorList");
                ////Create request with GET
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
                        var datalist = JsonConvert.DeserializeObject<List<GetAllPlansResponseModel>>(JsonConvert.SerializeObject(data));
                        lstGetAllPlans = datalist.ToList();
                        var a = lstGetAllPlans.Where(x => x.PlanName == "Prime No Daily limit Plan");
                        return Json(a);
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
                case "Vi": return "Vodafone";
                case "Vodafone": return "Vodafone";

                case "AIRTEL": return "Airtel";
                case "Airtel": return "Airtel";

                case "BSNL": return "BSNL";

                case "Idea": return "Vodafone";

                case "MTNL": return "MTNL";

                case "Jio": return "Reliance Jio";
                case "Reliance Jio": return "Reliance Jio";

                default: return "ALL";
            }
        }

        private string ToDigitsOnly(string input)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(input, "");
        }

    }

}
