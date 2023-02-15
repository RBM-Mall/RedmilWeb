using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Collections;
using RestSharp;
using Project_Redmil_MVC.Helper;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using System.Linq;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models.ResponseModel.SupportResponseModel;
using RestSharp.Deserializers;
using Project_Redmil_MVC.Models.ResponseModel.LoginValidateResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.LoginResponseModels;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models;
using System.Linq.Expressions;

namespace Project_Redmil_MVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }


        [HttpPost]
        #region Index
        public IActionResult Index(Login userNumber, string Mobile, string Mpin)
        {

            //var Id = HttpContext.Session.GetString("Id").ToString();
            //ViewBag.Id = Id;

            userNumber.Userid = "NA";
            try
            {
                #region Checksum (addsender|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("User/ValidateUser", Checksum.checksumKey, userNumber.Userid.ToString(), userNumber.Mobile);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion
                //var client = new RestClient("https://api.redmilbusinessmall.com/api/User/ValidateUser");
                var client = new RestClient($"{Baseurl}{ApiName.ValidateUser}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                userNumber.checksum = CheckSum;
                var json = JsonConvert.SerializeObject(userNumber);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var des = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    var data = des.Data;
                    var datalist = JsonConvert.DeserializeObject<List<ValidateResponseModel>>(JsonConvert.SerializeObject(data));
                    var UId = datalist.FirstOrDefault().Id;

                    if (des.Message.ToString() != null && des.Statuscode.ToString().Equals("OSS"))
                    {
                        return Json(des);
                    }
                    else if (!string.IsNullOrEmpty(des.Statuscode) && des.Statuscode == "TXN")
                    {
                        RequestMpinModel obj = new RequestMpinModel();
                        try
                        {
                            obj.IpAddress = GetIp();
                            obj.MacAddress = GetMacAddress(obj.IpAddress);
                            //obj.Version = "4.1.37";
                            obj.Version = "6.4.9";
                            obj.AppId = "faisal";
                            obj.Userid = "NA";
                            obj.Mobile = Mobile;
                            obj.Mpin = Mpin;
                            #region Checksum (addsender|Unique Key|UserId|)
                            string input1 = Checksum.MakeChecksumString("Mpin", Checksum.checksumKey, obj.Userid, obj.Mobile, obj.Mpin, obj.IpAddress, obj.MacAddress, obj.Version);
                            string CheckSum1 = Checksum.ConvertStringToSCH512Hash(input1);
                            #endregion
                            obj.checksum = CheckSum1;
                            //var client1 = new RestClient("https://api.redmilbusinessmall.com/api/Mpin");
                            var client1 = new RestClient($"{Baseurl}{ApiName.Mpin}");
                            var request1 = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            var json1 = JsonConvert.SerializeObject(obj);
                            request1.AddJsonBody(json1);
                            IRestResponse response1 = client1.Execute(request1);
                            var result1 = response1.Content;
                            var des1 = JsonConvert.DeserializeObject<BaseResponseModel>(response1.Content);
                            if (!string.IsNullOrEmpty(des1.Statuscode) && des1.Statuscode == "ERR")
                            {
                                return Json(des1);
                            }
                            else if (!string.IsNullOrEmpty(des1.Statuscode) && des1.Statuscode == "TXN")
                            {
                                List<ResponseMpinModel> mpinResponseModels = JsonConvert.DeserializeObject<List<ResponseMpinModel>>(JsonConvert.SerializeObject(des1.Data));
                                StoreSession(mpinResponseModels.FirstOrDefault());
                                //var u = HttpContext.Session.GetString("MobileNo");
                                if ((!string.IsNullOrEmpty(Convert.ToString(mpinResponseModels.FirstOrDefault().Id))))
                                {
                                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<ResponseMpinModel>>>(response1.Content);
                                    var getdata = deserializ.Data;
                                    HttpContext.Session.SetString("Name", getdata[0].Name);
                                    HttpContext.Session.SetString("Mobile", getdata[0].Mobileno);
                                    HttpContext.Session.SetString("Email", getdata[0].Emailid);
                                    HttpContext.Session.SetString("Mallname", getdata[0].Mallname);
                                    HttpContext.Session.SetString("Rolltype", getdata[0].Rolltype);
                                    HttpContext.Session.SetString("Id", getdata[0].Id.ToString());
                                    HttpContext.Session.SetString("Kycstatus", getdata[0].Kycstatus == true ? "1" : "0");
                                    //HttpContext.Session.SetString("Account", getdata[0].AccountNo.ToString());
                                    //HttpContext.Session.SetString("Ifsc", getdata[0].Ifsc.ToString());
                                    //HttpContext.Session.SetString("BankId", getdata[0].BankId.ToString());
                                    if (getdata.FirstOrDefault().AccountNo != null && getdata.FirstOrDefault().AccountNo != "")
                                    {
                                        HttpContext.Session.SetString("Account", Convert.ToString(mpinResponseModels.FirstOrDefault().AccountNo));
                                        var Account = HttpContext.Session.GetString("Account").ToString();
                                    }
                                    else
                                    {
                                        HttpContext.Session.SetString("Account", string.IsNullOrEmpty(mpinResponseModels.FirstOrDefault().AccountNo).ToString());
                                        TempData["Account"] = HttpContext.Session.GetString("Account").ToString();

                                    }

                                    if (getdata.FirstOrDefault().BankId != null && getdata.FirstOrDefault().BankId.ToString() != "")
                                    {
                                        HttpContext.Session.SetString("BankId", Convert.ToString(mpinResponseModels.FirstOrDefault().BankId));
                                        var BankId = HttpContext.Session.GetString("BankId").ToString();

                                    }
                                    if (getdata.FirstOrDefault().Ifsc != null && getdata.FirstOrDefault().Ifsc.ToString() != "")
                                    {
                                        HttpContext.Session.SetString("Ifsc", getdata[0].Ifsc.ToString());
                                        var Ifsc = HttpContext.Session.GetString("Ifsc").ToString();
                                    }
                                    var Mallname = HttpContext.Session.GetString("Mallname");
                                    var Name = HttpContext.Session.GetString("Name");
                                    var MobileNo = HttpContext.Session.GetString("Mobile").ToString();
                                    var Email = HttpContext.Session.GetString("Email");
                                    var Rolltype = HttpContext.Session.GetString("Rolltype");
                                    var UserId = HttpContext.Session.GetString("Id").ToString();
                                    TempData["Kycstatus"] = HttpContext.Session.GetString("Kycstatus").Trim();
                                    //TempData["Kycstatus"] = HttpContext.Session.GetString("Kycstatus").Trim()=="1"?true:false;
                                    //var Account = HttpContext.Session.GetString("Account").ToString();
                                    // var Ifsc = HttpContext.Session.GetString("Ifsc").ToString();
                                    //var BankId = HttpContext.Session.GetString("BankId").ToString();
                                    ViewBag.id = UserId;
                                    return Json(deserializ);
                                }
                            }
                            else if (!string.IsNullOrEmpty(des1.Statuscode) && des1.Statuscode == "UAB")
                            {
                                return Json(des1);
                            }

                            else if (!string.IsNullOrEmpty(des1.Statuscode) && des1.Statuscode == "ODL")
                            {
                                RequestOtpModel obj2 = new RequestOtpModel();
                                try
                                {
                                    obj2.Mobile = Mobile;
                                    string input2 = Checksum.MakeChecksumString("SendAnotherDeviceLoginOtp", Checksum.checksumKey, "NA", obj2.Mobile);
                                    string CheckSum2 = Checksum.ConvertStringToSCH512Hash(input2);
                                    obj2.checksum = CheckSum2;
                                    //var client2 = new RestClient("https://api.redmilbusinessmall.com/api/SendAnotherDeviceLoginOtp");
                                    var client2 = new RestClient($"{Baseurl}{ApiName.SendAnotherDeviceLoginOtp}");
                                    var request2 = new RestRequest(Method.POST);
                                    request.AddHeader("Content-Type", "application/json");
                                    var json2 = JsonConvert.SerializeObject(obj2);
                                    request2.AddJsonBody(json2);
                                    IRestResponse response2 = client2.Execute(request2);
                                    var result2 = response2.Content;
                                    if (string.IsNullOrEmpty(result2))
                                    {
                                        return RedirectToAction("ErrorForExceptionLog", "Error");
                                    }
                                    else
                                    {
                                        BaseResponseModel des2 = JsonConvert.DeserializeObject<BaseResponseModel>(response2.Content);
                                        if (des2.Statuscode == "ERR")
                                        {
                                            des2.Statuscode = "ODLERR";
                                            return Json(des2);
                                            //return some msg to identify there is some problem while sending otp please try again login and close the login popup.
                                        }
                                        des2.Statuscode = "ODLOSS";

                                        return Json(des2);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                                    requestModelEx.ExceptionMessage = ex;
                                    requestModelEx.Data = obj2;
                                    var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                                    var requestEx = new RestRequest(Method.POST);
                                    requestEx.AddHeader("Content-Type", "application/json");
                                    var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                                    requestEx.AddJsonBody(jsonEx);
                                    IRestResponse responseEx = clientEx.Execute(requestEx);
                                    var resultEx = responseEx.Content;
                                    return RedirectToAction("ErrorForExceptionLog", "Error");
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                            requestModelEx.ExceptionMessage = ex;
                            requestModelEx.Data = userNumber;
                            var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                            var requestEx = new RestRequest(Method.POST);
                            requestEx.AddHeader("Content-Type", "application/json");
                            var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                            requestEx.AddJsonBody(jsonEx);
                            IRestResponse responseEx = clientEx.Execute(requestEx);
                            var resultEx = responseEx.Content;
                            return RedirectToAction("ErrorForExceptionLog", "Error");
                        }
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
                requestModel1.Data = userNumber;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return RedirectToAction("ErrorForExceptionLog", "Error");
            }
            return View();
        }
        #endregion



        #region OtpVerfication
        [HttpPost]
        public JsonResult OtpVerfication(string Mobile, string AppId, String Otp)
        {
            RequestOtpModel obj3 = new RequestOtpModel();
            try
            {
                obj3.Mobile = Mobile;
                obj3.AppId = "faisal";
                obj3.Otp = Otp;
                string input3 = Checksum.MakeChecksumString("ValidateOTPAnotherDeviceLogin", Checksum.checksumKey, "NA", obj3.Mobile, obj3.Otp, obj3.AppId);
                string CheckSum3 = Checksum.ConvertStringToSCH512Hash(input3);
                obj3.checksum = CheckSum3;
                //var client3 = new RestClient("https://api.redmilbusinessmall.com/api/ValidateOTPAnotherDeviceLogin");
                var client3 = new RestClient($"{Baseurl}{ApiName.ValidateOTPAnotherDeviceLogin}");
                var request3 = new RestRequest(Method.POST);
                request3.AddHeader("Content-Type", "application/json");
                var json3 = JsonConvert.SerializeObject(obj3);
                request3.AddJsonBody(json3);
                IRestResponse response3 = client3.Execute(request3);
                var result3 = response3.Content;
                if (string.IsNullOrEmpty(result3))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    BaseResponseModel des3 = JsonConvert.DeserializeObject<BaseResponseModel>(response3.Content);
                    return Json(des3);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj3;
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




        #region SentOtpForResetMpin
        [HttpPost]
        public JsonResult SentOtpForResetMpin(string Mobile)
        {
            RequestOtpModel obj4 = new RequestOtpModel();
            try
            {
                obj4.Mobile = Mobile;
                string input4 = Checksum.MakeChecksumString("SentOtpForResetMpin", Checksum.checksumKey, "NA", obj4.Mobile);
                string CheckSum4 = Checksum.ConvertStringToSCH512Hash(input4);
                obj4.checksum = CheckSum4;
                //var client4 = new RestClient("https://api.redmilbusinessmall.com/api/SentOtpForResetMpin");
                var client4 = new RestClient($"{Baseurl}{ApiName.SentOtpForResetMpin}");
                var request4 = new RestRequest(Method.POST);
                request4.AddHeader("Content-Type", "application/json");
                var json4 = JsonConvert.SerializeObject(obj4);
                request4.AddJsonBody(json4);
                IRestResponse response4 = client4.Execute(request4);
                var result4 = response4.Content;
                if (string.IsNullOrEmpty(result4))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    BaseResponseModel des4 = JsonConvert.DeserializeObject<BaseResponseModel>(response4.Content);
                    return Json(des4);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj4;
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



        #region ChangesMpins
        public JsonResult ChangeMpins(string Mobile, string Mpin, string Otp)
        {
            RequestOtpModel obj5 = new RequestOtpModel();
            try
            {
                obj5.Mobile = Mobile;
                obj5.Mpin = Mpin;
                obj5.Otp = Otp;
                string input5 = Checksum.MakeChecksumString("ResetMpin", Checksum.checksumKey, "NA", obj5.Mobile, obj5.Mpin, obj5.Otp);
                string CheckSum5 = Checksum.ConvertStringToSCH512Hash(input5);
                obj5.checksum = CheckSum5;
                //var client5 = new RestClient("https://api.redmilbusinessmall.com/api/ResetMpin");
                var client5 = new RestClient($"{Baseurl}{ApiName.ResetMpin}");
                var request5 = new RestRequest(Method.POST);
                request5.AddHeader("Content-Type", "application/json");
                var json5 = JsonConvert.SerializeObject(obj5);
                request5.AddJsonBody(json5);
                IRestResponse response5 = client5.Execute(request5);
                var result5 = response5.Content;
                if (string.IsNullOrEmpty(result5))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    BaseResponseModel des5 = JsonConvert.DeserializeObject<BaseResponseModel>(response5.Content);
                    return Json(des5);
                }
                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj5;
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


        public string GetIp()
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddressesAsync(hostName);
            string gagan = ips.Result[1].ToString();
            //return "";
            //string ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string jaiswal = GetMacAddress(gagan);
            ViewBag.IPAddress = "Ip address is: " + gagan + " and Mac Address is " + jaiswal;
            return gagan;

        }
        public string GetMacAddress(string ipAddress)

        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            String sMacAddress = string.Empty;

            foreach (NetworkInterface adapter in nics)

            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();

                }

            }

            return sMacAddress;

        }
        public void StoreSession(ResponseMpinModel mpinResponseModel)
        {
            HttpContext.Session.SetString("Id", mpinResponseModel.Id.ToString());
            HttpContext.Session.SetString("ParentId", mpinResponseModel.ParentId.ToString() ?? "");
            HttpContext.Session.SetString("Name", mpinResponseModel.Name.ToString() ?? "");
            HttpContext.Session.SetString("MobileNo", mpinResponseModel.Mobileno ?? "");
            HttpContext.Session.SetString("Emailid", mpinResponseModel.Emailid ?? "");
            HttpContext.Session.SetString("Gender", mpinResponseModel.Gender ?? "");
            HttpContext.Session.SetString("DOB", mpinResponseModel.DOB ?? "");
            HttpContext.Session.SetString("Address", mpinResponseModel.Address ?? "");
            HttpContext.Session.SetString("Cityid", mpinResponseModel.CityId.ToString() ?? "");
            HttpContext.Session.SetString("StateId", mpinResponseModel.StateId.ToString() ?? "");
            HttpContext.Session.SetString("Pincode", mpinResponseModel.Pincode ?? "");
            HttpContext.Session.SetString("PanNo", mpinResponseModel.PanNo ?? "");
            HttpContext.Session.SetString("PanDoc", mpinResponseModel.PanDoc ?? "");
            HttpContext.Session.SetString("AadharDoc", mpinResponseModel.AadharDoc ?? "");
            HttpContext.Session.SetString("BankId", mpinResponseModel.BankId.ToString() ?? "");
            HttpContext.Session.SetString("Ifsc", mpinResponseModel.Ifsc ?? "");
            HttpContext.Session.SetString("AccountNo", mpinResponseModel.AccountNo ?? "");
            HttpContext.Session.SetString("Beneficiaryname", mpinResponseModel.Beneficiaryname ?? "");
            HttpContext.Session.SetString("Gstid", mpinResponseModel.Gstid.ToString() ?? "");
            HttpContext.Session.SetString("Esign", mpinResponseModel.Esign ?? "");
            HttpContext.Session.SetString("Rolltype", mpinResponseModel.Rolltype ?? "");
            HttpContext.Session.SetString("SchemeId", mpinResponseModel.SchemeId.ToString() ?? "");
            HttpContext.Session.SetString("Joiningdate", mpinResponseModel.Joiningdate ?? "");
            HttpContext.Session.SetString("Editdate", mpinResponseModel.Editdate ?? "");
            HttpContext.Session.SetString("Kycstatus", mpinResponseModel.Kycstatus.ToString() ?? "");
            HttpContext.Session.SetString("Status", mpinResponseModel.Status.ToString() ?? "");
            HttpContext.Session.SetString("Mallname", mpinResponseModel.Mallname ?? "");
            HttpContext.Session.SetString("ServiceCom", mpinResponseModel.ServiceCom.ToString() ?? "");
            HttpContext.Session.SetString("Gstatus", mpinResponseModel.Gstatus.ToString() ?? "");
            HttpContext.Session.SetString("CapAmt", mpinResponseModel.CapAmt.ToString() ?? "");
            HttpContext.Session.SetString("CapBal", mpinResponseModel.CapBal.ToString() ?? "");
            HttpContext.Session.SetString("BbpsId", mpinResponseModel.BbpsId ?? "");
            HttpContext.Session.SetString("GoldId", mpinResponseModel.GoldId ?? "");
            HttpContext.Session.SetString("PersonalStatus", mpinResponseModel.PersonalStatus.ToString() ?? "");
            HttpContext.Session.SetString("AppId", mpinResponseModel.AppId ?? "");
            HttpContext.Session.SetString("Insuance", mpinResponseModel.Insuance.ToString() ?? "");
            HttpContext.Session.SetString("InsuanceDate", mpinResponseModel.InsuanceDate?.ToString() ?? "");
            HttpContext.Session.SetString("WAPincode", mpinResponseModel.WAPincode?.ToString() ?? "");
            HttpContext.Session.SetString("AadharDocStatus", mpinResponseModel.AadharDocStatus ?? "");
            HttpContext.Session.SetString("PanDocStatus", mpinResponseModel.PanDocStatus ?? "");
            HttpContext.Session.SetString("EsignStatus", mpinResponseModel.EsignStatus.ToString() ?? "");
            HttpContext.Session.SetString("ProfilePic", mpinResponseModel.ProfilePic ?? "");
            HttpContext.Session.SetString("InDirectCommission", mpinResponseModel.InDirectCommission.ToString() ?? "");
            HttpContext.Session.SetString("ChangePassword", mpinResponseModel.ChangePassword.ToString() ?? "");
            HttpContext.Session.SetString("AppVersion", mpinResponseModel.AppVersion ?? "");
            HttpContext.Session.SetString("RealStatus", mpinResponseModel.RealStatus.ToString() ?? "");
            HttpContext.Session.SetString("BCAgentId", mpinResponseModel.BCAgentId?.ToString() ?? "");
            HttpContext.Session.SetString("AepsAgentId", mpinResponseModel.AepsAgentId?.ToString() ?? "");
            HttpContext.Session.SetString("AadharBackDoc", mpinResponseModel.AadharBackDoc ?? "");
            HttpContext.Session.SetString("AadharBackDocStatus", mpinResponseModel.AadharBackDocStatus ?? "");
            HttpContext.Session.SetString("PassbookOrChequeDoc", mpinResponseModel.PassbookOrChequeDoc ?? "");
            HttpContext.Session.SetString("PassbookOrChequeDocStatus", mpinResponseModel.PassbookOrChequeDocStatus ?? "");
            HttpContext.Session.SetString("Occupation", mpinResponseModel.Occupation ?? "");
            HttpContext.Session.SetString("Longitude", mpinResponseModel.Longitude ?? "");
            HttpContext.Session.SetString("Latitude", mpinResponseModel.Latitude ?? "");
            HttpContext.Session.SetString("BMappingDate", mpinResponseModel.BMappingDate?.ToString() ?? "");
            HttpContext.Session.SetString("MappingStatus", mpinResponseModel.MappingStatus.ToString() ?? "");
            HttpContext.Session.SetString("AadharStatus", mpinResponseModel.AadharStatus.ToString() ?? "");
            HttpContext.Session.SetString("AadharOtpKycStatus", mpinResponseModel.AadharOtpKycStatus?.ToString() ?? "");
            HttpContext.Session.SetString("OSAPinCode", mpinResponseModel.OSAPinCode ?? "");

            //var u = HttpContext.Session.GetString("Name");

        }
       
        
        #region ClearSession
        [HttpPost]

        public JsonResult ClearSession()
        {
            HttpContext.Session.Remove("Id");
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return Json(new BaseResponseModel() { Statuscode = "Success", Message = "Logout Successfuly", Data = "" });
            }
            return Json("");
        }
        #endregion


        #region SessionHandle
        [HttpPost]

        public JsonResult SessionHandle()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return Json(new BaseResponseModel() { Statuscode = "NotHasValue", Message = "", Data = "" });
            }
            else
            {
                return Json(new BaseResponseModel() { Statuscode = "HasValue", Message = "", Data = "" });
            }
        }
        #endregion

        #region Cookie
        [HttpPost]
        public JsonResult CookiesHandle(string Mobile, string Mpin)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            //Create a Cookie with a suitable Key and add the Cookie to Browser.
            Response.Cookies.Append("MobileNo", Mobile, option);
            Response.Cookies.Append("Mpin", Mpin, option);
            //Response.Cookies.Append("Mpin", mpinResponseModels.FirstOrDefault().Mobileno, option);
            string name = HttpContext.Request.Cookies["MobileNo"];
            string name1 = HttpContext.Request.Cookies["Mpin"];
            return Json(JsonConvert.SerializeObject(new ResCookiesModel() { Mobile = name, MPin = name1 }));
        }
        #endregion

    }
}