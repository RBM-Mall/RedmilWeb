using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Exchange.WebServices.Data;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Controllers.UserDashoard;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.AepsRequestModel;
using Project_Redmil_MVC.Models.RequestModel.LiveFaceLiveNess;
using Project_Redmil_MVC.Models.RequestModel.RatesandRoiRequestmodel;
using Project_Redmil_MVC.Models.RequestModel.SelfHelp;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.AepsResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.PssbookSuccesResponseModal;
using Project_Redmil_MVC.Models.ResponseModel.RatesandRoi;
using Project_Redmil_MVC.Models.ResponseModel.SelfHelpResponseModel;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Project_Redmil_MVC.Models.AddressResponseModel;
using static System.Net.Mime.MediaTypeNames;

namespace Project_Redmil_MVC.Controllers
{

    public class AepsController : Controller
    {
        public readonly string Baseurl;
        public readonly IConfiguration _config;
        public string finalResponse2 = string.Empty;
        public string Latitude = "28.6262498";
        public string Longitude = "77.3734622";
        public string Address = "Redmil Business Mall Headquarters,4th floor,Tower A,The Corenthum towers, Sector 62,Noida, Uttar Pradesh-201301";
        public string pinCode = "201301";
        public AepsController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }


        public IActionResult AEPS()
        {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {

                return RedirectToAction("ErrorForLogin", "Error");

            }
            dynamic obj = GetBalance();
            ViewBag.MobileNo = HttpContext.Session.GetString("Mobile").ToString();
            ViewBag.Balance = obj.Value[0].MainBal;
            return View(); 
        }

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


        #region GetAepsKycDetailsNew
        //First API for Get KYC details
        public IActionResult GetAepsKycDetailsNew(string radioVal, string bankName,string later)
        {

            var baseUrl = "https://api.redmilbusinessmall.com";
            AepsKycDetailsNewRequestModel obj = new AepsKycDetailsNewRequestModel();

            AepsRoutingDetailsRequestModel aepsRoutingDetailsReq = new AepsRoutingDetailsRequestModel();
            AepsRoutingDetailsResponseModel aepsRoutingDetailsResp = new AepsRoutingDetailsResponseModel();

            Kyc kyc = new Kyc();
            Bank bank = new Bank();
            List<Bank> Data1 = new List<Bank>();

            List<AepsKycDetailsNewResponseModel> aepsResponses = new List<AepsKycDetailsNewResponseModel>();
            List<AepsKycDetailsStatusPendingResponseModel> aepsKycDetailsStatusPendingResponses = new List<AepsKycDetailsStatusPendingResponseModel>();

            try
            {
                //obj.UserId = "599851"; //prahsant Yadav
                obj.UserId = "2180"; //Rahul Sir
                //obj.UserId = "2084";
                //obj.UserId = "721842"; //Rishi Redmil



                #region Checksum (GetAepsKycDetailsNew|Unique Key|UserId|)
                //string input = Checksum.MakeChecksumString("ViewPayOutCategory", obj.UserId, obj.Token);
                string input = Checksum.MakeChecksumString("GetAepsKycDetailsNew", Checksum.checksumKey, obj.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                obj.checksum = CheckSum;


                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetAepsKycDetailsNew");
                //var client = new RestClient($"{Baseurl}{ApiName.GetAgentKycId}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(json);

                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var aepsKycData = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    var aepsKyc = aepsKycData.Data;
                    var aepsStatusCode = aepsKycData.Statuscode;
                    //string aepsKycStatus = string.Empty;i
                    if (aepsStatusCode == "TXN")
                    {
                        var aepsKycStatus = JsonConvert.DeserializeObject<List<AepsKycDetailsNewResponseModel>>(JsonConvert.SerializeObject(aepsKyc));

                        aepsResponses = aepsKycStatus.ToList();

                        try
                        {
                            if (aepsStatusCode == "TXN")
                            {
                                if (aepsKycStatus != null)
                                {
                                    //var aepsKycStatusData = aepsKycStatus.Where(x => x.Status == "Completed");
                                    if (aepsKycStatus.FirstOrDefault().Status == "Completed" || later=="1")
                                    {
                                        //aepsRoutingDetailsReq.UserId = "599851";
                                        aepsRoutingDetailsReq.UserId = "2180";
                                        //aepsRoutingDetailsReq.UserId = "2084";
                                        aepsRoutingDetailsReq.Token = "";
                                        if (radioVal == "Cash Withdrawal")
                                        {
                                            aepsRoutingDetailsReq.TransactionType = "01";
                                        }
                                        else if (radioVal == "Balance Enquiry")
                                        {
                                            aepsRoutingDetailsReq.TransactionType = "31";
                                        }
                                        else if (radioVal == "Mini Statement")
                                        {
                                            aepsRoutingDetailsReq.TransactionType = "07";
                                        }
                                        else
                                        {
                                            aepsRoutingDetailsReq.TransactionType = "Invalid TXN Type";
                                        }

                                        var client1 = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetAepsRoutingDetails");

                                        var request1 = new RestRequest(Method.POST);
                                        request1.AddHeader("Content-Type", "application/json");
                                        var json1 = JsonConvert.SerializeObject(aepsRoutingDetailsReq);
                                        request1.AddJsonBody(json1);

                                        IRestResponse response1 = client1.Execute(request1);
                                        var result1 = response1.Content;
                                        if (string.IsNullOrEmpty(result1))
                                        {
                                            return RedirectToAction("ErrorForExceptionLog", "Error");
                                        }
                                        else
                                        {
                                            var aepsKycData1 = JsonConvert.DeserializeObject<BaseAepsRoutingDetailsResponseModel>(response1.Content);
                                            var aepsKyc0 = aepsKycData1.Statuscode;
                                            var status = aepsKycData1.Status;
                                            var aepsKyc1 = aepsKycData1.Kyc;
                                            var aepsKyc2 = aepsKycData1.Bank;
                                            var aepsRoutingDetailsStatus1 = JsonConvert.DeserializeObject<List<Kyc>>(JsonConvert.SerializeObject(aepsKyc1));
                                            var aepsRoutingDetailsStatus2 = JsonConvert.DeserializeObject<List<Bank>>(JsonConvert.SerializeObject(aepsKyc2));
                                            if (aepsKycData1.Statuscode == "TXN")
                                            {
                                                if (aepsKycData1.Status == "Bridge")
                                                {
                                                    // status Birdge ayega to Bank select krna h.
                                                    if (aepsRoutingDetailsStatus2 != null)
                                                    {
                                                        foreach (var item in aepsRoutingDetailsStatus2)
                                                        {
                                                            Data1.Add(new Bank
                                                            {
                                                                BankName1 = item.BankName1,
                                                                BankLogo1 = baseUrl + item.BankLogo1,
                                                                BankName2 = item.BankName2,
                                                                BankLogo2 = baseUrl + item.BankLogo2,
                                                                BankName3 = item.BankName3,
                                                                BankLogo3 = baseUrl + item.BankLogo3

                                                            });
                                                        }

                                                        if (radioVal == "Cash Withdrawal")
                                                        {
                                                            radioVal = "cw";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Balance Enquiry")
                                                        {
                                                            radioVal = "be";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Mini Statement")
                                                        {
                                                            radioVal = "ms";
                                                            //return Json(radioVal);
                                                        }
                                                        return Json(new
                                                        {
                                                            data = Data1,
                                                            data2 = radioVal,
                                                            Status = "radio",
                                                        });
                                                    }
                                                }

                                                else if (aepsKycData1.Status == "Ratio")
                                                {
                                                    //Status Ratio aayega to direct Select Device
                                                    if (aepsRoutingDetailsStatus2 != null)
                                                    {
                                                        foreach (var item in aepsRoutingDetailsStatus2)
                                                        {
                                                            Data1.Add(new Bank
                                                            {
                                                                BankName1 = item.BankName1,
                                                                BankLogo1 = baseUrl + item.BankLogo1,
                                                            });
                                                        }
                                                        if (radioVal == "Cash Withdrawal")
                                                        {
                                                            radioVal = "cw";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Balance Enquiry")
                                                        {
                                                            radioVal = "be";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Mini Statement")
                                                        {
                                                            radioVal = "ms";
                                                            //return Json(radioVal);
                                                        }
                                                        return Json(new
                                                        {
                                                            data = Data1,
                                                            data2 = radioVal,
                                                            Status = status,
                                                        });
                                                    }
                                                }

                                                else if (aepsKycData1.Status == "Exception")
                                                {
                                                    //Status Exception aayega to direct Select Device
                                                    if (aepsRoutingDetailsStatus2 != null)
                                                    {
                                                        foreach (var item in aepsRoutingDetailsStatus2)
                                                        {
                                                            Data1.Add(new Bank
                                                            {
                                                                BankName1 = item.BankName1,
                                                                BankLogo1 = baseUrl + item.BankLogo1,
                                                            });
                                                        }

                                                        if (radioVal == "Cash Withdrawal")
                                                        {
                                                            radioVal = "cw";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Balance Enquiry")
                                                        {
                                                            radioVal = "be";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Mini Statement")
                                                        {
                                                            radioVal = "ms";
                                                            //return Json(radioVal);
                                                        }
                                                        return Json(new
                                                        {
                                                            data = Data1,
                                                            data2 = radioVal,
                                                            Status = status,
                                                        });
                                                    }
                                                }
                                            }

                                            //kyc = aepsRoutingDetailsStatus1.ToList();

                                            var bankName1 = aepsRoutingDetailsStatus2.FirstOrDefault().BankName1;
                                            var bankLogo1 = aepsRoutingDetailsStatus2.FirstOrDefault().BankLogo1;
                                            var bankName2 = aepsRoutingDetailsStatus2.FirstOrDefault().BankName2;
                                            var bankLogo2 = aepsRoutingDetailsStatus2.FirstOrDefault().BankLogo2;
                                            var bankName3 = aepsRoutingDetailsStatus2.FirstOrDefault().BankName3;
                                            var bankLogo3 = aepsRoutingDetailsStatus2.FirstOrDefault().BankLogo3;
                                            var kycStatus = aepsRoutingDetailsStatus1.FirstOrDefault().KycStatus;
                                            var exceptionKYCBankName = aepsKycData1.Kyc.FirstOrDefault().ExceptionKYCBankName;
                                            var exceptionKycRoutingStatus = aepsRoutingDetailsStatus1.FirstOrDefault().ExceptionKYCRoutingStatus;
                                            if (kycStatus.Equals("False") && exceptionKycRoutingStatus.Equals("True"))
                                            {
                                                if (radioVal == "Cash Withdrawal")
                                                {
                                                    var eBankName = exceptionKYCBankName;
                                                    radioVal = "cw";
                                                    return Json(new { data = eBankName, data2 = radioVal }); // To redirect to the page select device
                                                }
                                                else if (radioVal == "Balance Enquiry")
                                                {
                                                    var eBankName = exceptionKYCBankName;
                                                    radioVal = "be";
                                                    return Json(new { data = eBankName, data2 = radioVal }); // To redirect to the page select device
                                                }
                                                else if (radioVal == "Mini Statement")
                                                {
                                                    var eBankName = exceptionKYCBankName;
                                                    radioVal = "ms";
                                                    return Json(new { data = eBankName, data2 = radioVal }); // To redirect to the page select device
                                                }

                                                //return Json(new { data = eBankName, data2 = radioVal }); // To redirect to the page select device
                                                //else
                                                //{
                                                //    errMsg = "Invalid TXN Type";
                                                //}


                                                // for select device modal pop up

                                                else
                                                {
                                                    string? bankName11 = bankName1.ToString();
                                                    string? bankLogo11 = bankLogo1.ToString(); // select device me bank name1 and TT jayega kewal

                                                    //string? bankName12 = bankName2.ToString();
                                                    //string? bankLogo12 = bankLogo2.ToString();
                                                    //string? bankName13 = bankName2.ToString();
                                                    //string? bankLogo13 = bankLogo2.ToString();

                                                    if (aepsRoutingDetailsStatus2 != null)
                                                    {
                                                        foreach (var item in aepsRoutingDetailsStatus2)
                                                        {
                                                            Data1.Add(new Bank
                                                            {
                                                                BankName1 = item.BankName1,
                                                                BankLogo1 = item.BankLogo1
                                                                //BankName2 = item.BankName2,
                                                                //BankLogo2 = item.BankLogo2,
                                                                //BankName3 = item.BankName3,
                                                                //BankLogo3 = item.BankLogo3
                                                            });
                                                        }

                                                        if (radioVal == "Cash Withdrawal")
                                                        {
                                                            radioVal = "cw";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Balance Enquiry")
                                                        {
                                                            radioVal = "be";
                                                            //return Json(radioVal);
                                                        }
                                                        else if (radioVal == "Mini Statement")
                                                        {
                                                            radioVal = "ms";
                                                            //return Json(radioVal);
                                                        }

                                                        return Json(new { data = Data1, data2 = radioVal });
                                                    }

                                                    //else
                                                    //{
                                                    //    errMsg = "Invalid TXN Type";
                                                    //}

                                                }
                                            }

                                            

                                            //if(kycStatus.Equals("True") && exceptionKycRoutingDetails.Equals("True"))
                                            //{
                                            // Will do later.
                                            //}
                                        }

                                    }
                                    else if (aepsKycStatus.FirstOrDefault().Status == "Pending")
                                    {
                                        foreach (var item in aepsKycStatus)
                                        {
                                            aepsKycDetailsStatusPendingResponses.Add(new AepsKycDetailsStatusPendingResponseModel
                                            {
                                                Status = item.Status,
                                                BankLogo = baseUrl + item.BankLogo,
                                                BankName = item.BankName,
                                                //getBankResponseModels = GetBankRe,
                                                //getBalanceResponseModels=  lstdata,
                                            });
                                        }
                                        //return Json(aepsKycDetailsStatusPendingResponses);
                                        return Json(new
                                        {
                                            data = aepsKycDetailsStatusPendingResponses,
                                            data2 = radioVal,
                                            Status = "Pending",
                                        });
                                        //return Json(aepsKycStatus);
                                    }
                                }
                            }


                            else if (aepsStatusCode == "ERR")
                            {
                               
                            }
                            else
                            {
                                return RedirectToAction("ErrorForExceptionLog", "Error");
                            }

                        }
                        catch (Exception ex)
                        {
                            ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                            requestModel1.ExceptionMessage = ex;
                            requestModel1.Data = aepsRoutingDetailsReq;
                            var clientEx1 = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                            var requestEx1 = new RestRequest(Method.POST);
                            requestEx1.AddHeader("Content-Type", "application/json");
                            var jsonEx1 = JsonConvert.SerializeObject(requestModel1);
                            requestEx1.AddJsonBody(jsonEx1);
                            IRestResponse responseEx1 = client.Execute(requestEx1);
                            var resultEx1 = responseEx1.Content;
                            return RedirectToAction("ErrorForExceptionLog", "Error");
                        }
                    }
                    return View(); //aepsResponses
                }
            }

            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = obj;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
                return RedirectToAction("ErrorForExceptionLog", "Error");
            }
        }
        #endregion



        #region AaadharPicVerification
        // After Device Selection hitting below api
        public JsonResult AaadharPicVerification(string radioValDevice, string transMode, string device, string bank)
        {
            //var baseUrl = "https://api.redmilbusinessmall.com";
            AaadharPicVerificationRequestModal adharPicFaceBioRequest = new AaadharPicVerificationRequestModal();

            //AepsRoutingDetailsRequestModel aepsRoutingDetailsReq = new AepsRoutingDetailsRequestModel();
            //AepsRoutingDetailsResponseModel aepsRoutingDetailsResp = new AepsRoutingDetailsResponseModel();

            //Kyc kyc = new Kyc();
            //Bank bank = new Bank();
            //var Data1 = new List<Bank>();

            List<AaadharPicVerificationResponseModal> adharPicFaceBioResponse = new List<AaadharPicVerificationResponseModal>();
            try
            {
                adharPicFaceBioRequest.Userid = "2180"; // 2114, 721842

                #region Checksum (AaadharPicVerification|Unique Key|UserId|)
                //string input = Checksum.MakeChecksumString("ViewPayOutCategory", obj.UserId, obj.Token);
                string input = Checksum.MakeChecksumString("AaadharPicVerification", Checksum.checksumKey, adharPicFaceBioRequest.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                adharPicFaceBioRequest.checksum = CheckSum;

                if (radioValDevice != null)
                {
                    var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/AaadharPicVerification");
                    //var client = new RestClient($"{Baseurl}{ApiName.GetAgentKycId}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(adharPicFaceBioRequest);
                    request.AddJsonBody(json);

                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    if (string.IsNullOrEmpty(result))
                    {
                        return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    else
                    {
                        var adharPicFaceBioData = JsonConvert.DeserializeObject<AaadharPicVerificationResponseModal>(response.Content);
                        var adharPicFaceBioStatusCode = adharPicFaceBioData.Statuscode;
                        //var aepsStatusCode = aepsKycData.Statuscode;
                        //var aepsKycStatus = JsonConvert.DeserializeObject<List<AepsKycDetailsNewResponseModel>>(JsonConvert.SerializeObject(result));
                        //adharPicFaceBioResponse = adharPicFaceBioData.ToList();

                        if (adharPicFaceBioStatusCode == "TXN")
                        {
                            return Json(adharPicFaceBioData);
                        }

                        else if (adharPicFaceBioStatusCode == "SKP")
                        {
                            return Json(adharPicFaceBioData);
                        }

                        else if (adharPicFaceBioStatusCode == "ERR")
                        {
                            HttpContext.Session.SetString("TransactionMode", transMode);
                            HttpContext.Session.SetString("DeviceName", device);
                            HttpContext.Session.SetString("BankName", bank);
                            return Json(new { Result = "RedirectToCustomer", url = Url.Action("CustomerDetails", "Aeps") });
                        }
                        else
                        {
                            return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                        }
                    }
                }
                return Json("");
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                requestModel1.ExceptionMessage = ex;
                requestModel1.Data = adharPicFaceBioRequest;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                requestEx.AddJsonBody(json);
                IRestResponse response = client.Execute(requestEx);
                var result = response.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }


        }
        #endregion





        #region FaceLiveLiness
        public JsonResult FaceLiveLiness(string showimage,string transMode, string device,string bank,string bankLogo)
        {

            AepsFaceVerificationRequestModel requestobj = new AepsFaceVerificationRequestModel();
            try
            {
                //requestobj.Userid = HttpContext.Session.GetString("Id").ToString();
                //requestobj.Userid = "599851";
                requestobj.Userid = "2180";
                //requestobj.Userid = "721842";
                requestobj.FileName = showimage;
                var client = new RestClient($"{Baseurl}{ApiName.FaceLiveliNess}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestobj);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                    if (deserialize.Statuscode == "TXN" || deserialize.Statuscode == "SKP")
                    {
                        HttpContext.Session.SetString("TransactionMode", transMode);
                        HttpContext.Session.SetString("DeviceName", device);
                        HttpContext.Session.SetString("BankName", bank);
                        return Json(new { Result = "RedirectToCustomer", url = Url.Action("CustomerDetails", "Aeps")});
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
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestobj;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion








        #region fingurprintBiometric
        //public JsonResult fingurprintBiometric(fingurprintDataRequestModel Result, string radioValDevice, string latitude, string longitude)
        public JsonResult fingurprintBiometric(string customerName,string transactionMode, string insertedAmount, string customerMobile,string deBankName, string userselectedBankName, string customerAadharNumber,string customeramount,string iin,string deviceName, string aepsAgentId)
        {
            
            Dictionary<string, string> userVerificationWithPaytmData;
            var biometricData = TriggerMachine();
            string transactionValue = string.Empty;
            if (biometricData != null && biometricData != "")
            {
                if (transactionMode.Equals("be"))
                {
                    transactionValue = "31";
                }
                else if (transactionMode.Equals("ms"))
                {
                    transactionValue = "07";
                }
                else if (transactionMode.Equals("cw"))
                {
                    transactionValue = "01";
                }

                fingurprintDataRequestModel fingurprintDataRequest = new fingurprintDataRequestModel();
                //deBankName = "ICICI Bank";
                UserVerificationWithPaytmRequestModel userVerificationWithPaytmRequest = new UserVerificationWithPaytmRequestModel();
                //userVerificationWithPaytmRequest.userId = int.Parse(HttpContext.Session.GetString("Id"));
                //userVerificationWithPaytmRequest.userId = "599851"; //Prashant Yadav
                //userVerificationWithPaytmRequest.userId = "721842";//Rishi SIngh
                userVerificationWithPaytmRequest.userId = "2180";//Rahul Sir
                var unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                //string retailerTxnId = userVerificationWithPaytmRequest.userId + " " + DateTime.Now.ToString();
                string retailerTxnId = DateTime.Now.Ticks + "" + userVerificationWithPaytmRequest.userId;
                char ss = Convert.ToChar(34);
                if(deBankName=="ICICI Bank")
                {
                    userVerificationWithPaytmData = new Dictionary<string, string>()
                    {
                        ["RetailerTxnId"] = retailerTxnId,
                        //["RetailerTxnId"] = "7986158232599851",
                        //["AadharNumber"] = "275687902863",//Prashant Yadav
                        ["AadharNumber"] = customerAadharNumber,
                        //["AadharNumber"] = "289234714252",//Rahul Sir 
                        ["IIN"] = iin,
                        ["Amount"] = insertedAmount,
                        //["BiometricData"] = ss.ToString() + ss.ToString() + biometricData,
                        ["BiometricData"] = biometricData,
                        ["RetailerId"] = "RED7355558471",
                        //["Latitude"] = "28.5966336",
                        ["Latitude"] = "28.626264",
                        //["Longitude"] = "77.3914624",
                        ["Longitude"] = "77.3734324",
                        ["AcquirerInstitutationId"] = "1",
                        //["TxnTypeCode"] = "07",//Mini Statement
                        //["TxnTypeCode"] = "01",//Cash Withdrwal
                        //["TxnTypeCode"] = "31",//Balance Enquiry
                        ["TxnTypeCode"]= transactionValue,
                        ["AppId"] = "1",
                        ["AppVersion"] = "2.0.0",
                        ["CustomerConsent"] = "1",
                        ["IsVID"] = "False",
                        ["AuthType"] = "FMR-FIR",
                        ["Param1"] = "10.0.0.1",
                        //["Param2"] = "8527698920",//Prahsant Mobile
                        ["Param2"] = customerMobile,
                        //["Param2"] = "7355558471",//Rahul Sir Mobile
                        ["Param3"] = string.Empty,
                        ["Param4"] = string.Empty,
                        ["Param5"] = string.Empty,
                        ["Param6"] = string.Empty,
                        ["Param7"] = string.Empty,
                        ["Param8"] = string.Empty,
                        ["Param9"] = string.Empty,
                        ["Param10"] = string.Empty
                    };

                   // userVerificationWithPaytmData.Add("userid", "2180");
                }
                else
                {
                    userVerificationWithPaytmData = new Dictionary<string, string>()
                    {
                        ["RetailerTxnId"] = retailerTxnId,
                        //["RetailerTxnId"] = "7986158232599851",
                        //["AadharNumber"] = "275687902863",//Prashant Yadav
                        ["AadharNumber"] = customerAadharNumber,
                        //["AadharNumber"] = "289234714252",//Rahul Sir 
                        ["IIN"] = iin,
                        ["Amount"] = insertedAmount,
                        //["BiometricData"] = ss.ToString() + ss.ToString() + biometricData,
                        ["BiometricData"] = biometricData,
                        //["RetailerId"] = "RED7355558471",
                        ["RetailerId"] = "RED7355558471",
                        //["Latitude"] = "28.5966336",
                        ["Latitude"] = "28.626264",
                        //["Longitude"] = "77.3914624",
                        ["Longitude"] = "77.3734324",
                        ["AcquirerInstitutationId"] = "1",
                        ["TxnTypeCode"] = transactionValue,
                        //["TxnTypeCode"] = "31",//Balance Enquiry
                        //["TxnTypeCode"] = "07",//Mini Statement
                        //["TxnTypeCode"] = "01",//Cash Withdrwal
                        ["AppId"] = "1",
                        ["AppVersion"] = "2.0.0",
                        ["CustomerConsent"] = "1",
                        ["IsVID"] = "False",
                        ["AuthType"] = "FMR-FIR",
                        ["Is1FADone"] = "0",
                        ["Param1"] = "10.0.0.1",
                        //["Param2"] = "8527698920",//Prahsant Mobile
                        ["Param2"] = customerMobile,
                        //["Param2"] = "7355558471",//Rahul Sir Mobile
                        ["Param3"] = string.Empty,
                        ["Param4"] = string.Empty,
                        ["Param5"] = string.Empty,
                        ["Param6"] = string.Empty,
                        ["Param7"] = string.Empty,
                        ["Param8"] = string.Empty,
                        ["Param9"] = string.Empty,
                        ["Param10"] = string.Empty
                    };
                }
                //Dictionary<string, string> userVerificationWithPaytmData = new Dictionary<string, string>()
                

                string dictionaryData = JsonConvert.SerializeObject(userVerificationWithPaytmData);

                //Dictionary<string, string> userVerificationWithPaytmData2 = new Dictionary<string, string>()
                //{
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.RetailerTxnId, "retailerTxnId");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.AadharNumber, dd);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.IIN, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Amount, dd);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.RetailerId, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param1, "10.0.0.1");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param2, "8527698920");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.AcquirerInstitutationId, "1");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.AppId, "1");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.AppVersion, "2.0.0");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.CustomerConsent, "1");
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param3, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param4, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param5, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param6, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param7, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param8, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param9, String.Empty);
                //userVerificationWithPaytmData.Add(userVerificationWithPaytmRequest.Param10, String.Empty);
                //userVerificationWithPaytmData2.Add(userVerificationWithPaytmRequest.BiometricData, "");
                //userVerificationWithPaytmData2.Add(userVerificationWithPaytmRequest.Latitude, "");
                //userVerificationWithPaytmData2.Add(userVerificationWithPaytmRequest.Longitude, "");
                //userVerificationWithPaytmData2.Add(userVerificationWithPaytmRequest.TxnTypeCode, "Balance Enquiry");
                //userVerificationWithPaytmData2.Add(userVerificationWithPaytmRequest.IsVID, "False");
                //};

                //List<AaadharPicVerificationResponseModal> adharPicFaceBioResponse = new List<AaadharPicVerificationResponseModal>();


                //adharPicFaceBioRequest.Userid = "599851"; // 2114, 2180



                #region Checksum (AaadharPicVerification|Unique Key|UserId|)
                //string input = Checksum.MakeChecksumString("ViewPayOutCategory", obj.UserId, obj.Token);
                //string input = Checksum.MakeChecksumString("AaadharPicVerification", Checksum.checksumKey, adharPicFaceBioRequest.Userid);
                //string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                //adharPicFaceBioRequest.checksum = CheckSum;

                if (deviceName != null)
                {
                    var dictionaryData1 = JsonConvert.DeserializeObject(dictionaryData);
                    AepsCypherTextEncryptionDecryption crypto = AepsCypherTextEncryptionDecryption.Instance;
                    string encryptedText = crypto.YBLEncrypt(dictionaryData1.ToString(), "Ef6d2GRq98");  //For Encrypting Code
                    //string decryptText = crypto.YBLDecrypt(dictionaryData1.ToString(), "Ef6d2GRq98"); //For Decrypting Code

                    UserVerificationWithPaytmRequestModel1 requestModel = new UserVerificationWithPaytmRequestModel1();
                    MakeAepsTransactionRequestModel requestModelNew = new MakeAepsTransactionRequestModel();
                    try
                    {
                        
                        //requestModelNew.Userid = "599851";//PRASHANT YADAV
                        requestModelNew.Userid = "2180";
                        requestModelNew.EncData = encryptedText;
                        requestModelNew.PinCode = "201301";
                        requestModelNew.CustomerAadhaarNo = customerAadharNumber;
                        requestModelNew.CustomerMobileno = customerMobile;
                        requestModelNew.CustomerName = customerName;
                        requestModelNew.Mode = "App";
                        requestModelNew.BCName = "RAHUL KUSHWAHA";
                        //requestModelNew.BCName = "PRASHANT YADAV";
                        //requestModelNew.TxnType = "31";//Balance Enquriy
                        //requestModelNew.TxnType = "01";//Cash Withdrawal
                        //requestModelNew.TxnType = "07";//Mini Statement
                        //requestModelNew.AgentID = "RED7355558471";
                        requestModelNew.TxnType = transactionValue;
                        requestModelNew.AgentID = "RED7355558471";
                        //requestModelNew.AgentID = "RED8527698920";
                        requestModelNew.BCLocation = Address;
                        //requestModelNew.Device = "Mantra";
                        requestModelNew.Device = deviceName;
                        requestModelNew.Amount = insertedAmount;
                        requestModelNew.Token = "";
                        requestModelNew.RetailerTxnId = retailerTxnId;

                        //Commented For ICICI Bank Transactions
                        #region Checksum (MakeFingPayAepsTransactions|Unique Key|Input Fields)
                        string Input = Checksum.MakeChecksumString("MakeFingPayAepsTransactions", Checksum.checksumKey, requestModelNew.Userid.Trim(),
                         requestModelNew.Amount.Trim(), requestModelNew.TxnType.Trim(), requestModelNew.CustomerMobileno.Trim(), requestModelNew.Mode, requestModelNew.CustomerName
                         , requestModelNew.CustomerAadhaarNo.Trim(), requestModelNew.BCName.Trim(), requestModelNew.AgentID, requestModelNew.BCLocation, requestModelNew.Device);

                        string CheckSum = Checksum.ConvertStringToSCH512Hash(Input);
                        #endregion
                        requestModelNew.checksum = CheckSum;
                        //var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/MakeFingPayAepsTransactions");
                        var client = new RestClient("https://proapitest1.redmilbusinessmall.com/api/MakeFingPayAepsTransactions");

                        ////Commented For Yes Bank Transactions
                        //#region Checksum (MakeFingPayAepsTransactions|Unique Key|Input Fields)
                        //string Input = Checksum.MakeChecksumString("MakeAepsTransactions", Checksum.checksumKey, requestModelNew.Userid.Trim(),
                        // requestModelNew.Amount.Trim(), requestModelNew.TxnType.Trim(), requestModelNew.CustomerMobileno.Trim(), requestModelNew.Mode, requestModelNew.CustomerName
                        // , requestModelNew.CustomerAadhaarNo.Trim(), requestModelNew.BCName.Trim(), requestModelNew.AgentID, requestModelNew.BCLocation, requestModelNew.Device);

                        //string CheckSum = Checksum.ConvertStringToSCH512Hash(Input);
                        //#endregion
                        //requestModelNew.checksum = CheckSum;
                        //var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/MakeAepsTransactions");
                        //var client = new RestClient("https://proapitest1.redmilbusinessmall.com/api/MakeAepsTransactions");
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/json");
                        var json = JsonConvert.SerializeObject(requestModelNew);
                        request.AddJsonBody(json);
                        IRestResponse response = client.Execute(request);
                        var result = response.Content;
                        if (string.IsNullOrEmpty(result))
                        {
                            return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                        }
                        else
                        {
                            if (transactionValue == "07")
                            {
                                var deserializFinalResponse = JsonConvert.DeserializeObject<BaseResonseModelAepsFinalT<List<MakeAepsTransactionResponseModel>>>(response.Content);
                                
                                if (deserializFinalResponse.Statuscode == "TXN")
                                {
                                    var miniStatementData = deserializFinalResponse.Data.FirstOrDefault().MiniSTMTdata;
                                    var newminiStatementData = JsonConvert.DeserializeObject<List<MiniSTMTdata>>(miniStatementData);

                                    return Json(new BaseMakeAepsTransactionResponseModel() { MiniStatementData=newminiStatementData, Statuscode = deserializFinalResponse.Statuscode, Message = deserializFinalResponse.Message, Data = deserializFinalResponse.Data.FirstOrDefault() });
                                }
                                else if (deserializFinalResponse.Statuscode == "ERR")
                                {
                                    return Json(new BaseMakeAepsTransactionResponseModel() { Statuscode = deserializFinalResponse.Statuscode, Message = deserializFinalResponse.Message, Data = deserializFinalResponse.Data.FirstOrDefault() });
                                }
                                else
                                {
                                    return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                                }
                            }
                            else
                            {
                                var deserializFinalResponse = JsonConvert.DeserializeObject<BaseResonseModelAepsFinalT<List<MakeAepsTransactionResponseModel>>>(response.Content);
                                if (deserializFinalResponse.Statuscode == "TXN")
                                {
                                    return Json(new BaseMakeAepsTransactionResponseModel() { Statuscode = deserializFinalResponse.Statuscode, Message = deserializFinalResponse.Message, Data = deserializFinalResponse.Data.FirstOrDefault() });
                                }

                                else if (deserializFinalResponse.Statuscode == "ERR")
                                {
                                    return Json(new BaseMakeAepsTransactionResponseModel() { Statuscode = deserializFinalResponse.Statuscode, Message = deserializFinalResponse.Message, Data = deserializFinalResponse.Data.FirstOrDefault() });
                                }
                                else
                                {
                                    return Json(new { Result = "UnExpectedStatusCode", url = Url.Action("ErrorForExceptionLog", "Error") });
                                }
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                        requestModelEx.ExceptionMessage = ex;
                        requestModelEx.Data = requestModelNew;
                        var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                        var requestEx = new RestRequest(Method.POST);
                        requestEx.AddHeader("Content-Type", "application/json");
                        var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                        requestEx.AddJsonBody(jsonEx);
                        IRestResponse responseEx = clientEx.Execute(requestEx);
                        var resultEx = responseEx.Content;
                        return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                }
            }
            else
            {
                return Json(new { Result = "NotConnected", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
            return Json("");
        }
        #endregion

        

        //public IActionResult CheckFingerprintAvailability()
        //{
        //    string returnMessage = "";

        //    try
        //    {
        //        // Check the availability of fingerprint authentication.
        //        var ucvAvailability = Windows.Security.Credentials.UI.UserConsentVerifier.CheckAvailabilityAsync();

        //        switch (ucvAvailability)
        //        {
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.Available:
        //                returnMessage = "Fingerprint verification is available.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceBusy:
        //                returnMessage = "Biometric device is busy.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceNotPresent:
        //                returnMessage = "No biometric device found.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DisabledByPolicy:
        //                returnMessage = "Biometric verification is disabled by policy.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.NotConfiguredForUser:
        //                returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " +
        //                                "fingerprint database and try again.";
        //                break;
        //            default:
        //                returnMessage = "Fingerprints verification is currently unavailable.";
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnMessage = "Fingerprint authentication availability check failed: " + ex.ToString();
        //    }

        //    return returnMessage;
        //}

        #region CustomerDetails
        public IActionResult CustomerDetails()
        {
            var deviceName = DeviceName();
            var transactionMode = TransactionMode();
            var bankName = BankName();
            //ViewBag.BankList = new SelectList(AllBankList(), "Id", "BankName");
            return View();

        }
        #endregion





        #region Bank Details
        public JsonResult AllBankList(string BankNameSelected)
        {
            AepsGetAllBankListRequestModel requestModel = new AepsGetAllBankListRequestModel();
            List<AepsGetAllBankListResponseModel> Data1 = new List<AepsGetAllBankListResponseModel>();
            string CheckSum = string.Empty;
            try
            {
                //requestModel.UserId = "599851";
                requestModel.UserId = "2180";
                requestModel.Token = "";

                if(BankNameSelected.Equals("Yes Bank"))
                {
                    #region Checksum (UserId|Token|checksum)

                    string input = Checksum.MakeChecksumString("GetAllAepsBankDetails", Checksum.checksumKey, requestModel.UserId);
                    CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                    #endregion
                    requestModel.checksum = CheckSum;
                    var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetAllAepsBankDetails");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    if (string.IsNullOrEmpty(result))
                    {
                        //return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    else
                    {
                        var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                        if (deserialize.Statuscode == "TXN" && deserialize != null)
                        {
                            var datadeserialize = deserialize.Data;
                            var data = JsonConvert.DeserializeObject<List<AepsGetAllBankListResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    Data1.Add(new AepsGetAllBankListResponseModel
                                    {
                                        Id = item.Id,
                                        BankName = item.BankName,
                                        IIN = item.IIN,
                                        BankCode = item.BankCode,
                                    });
                                }

                            }
                            return Json(Data1);
                        }
                        else if (deserialize.Statuscode == "ERR")
                        {
                            return Json(deserialize);
                        }
                    }
                    return Json(Data1);

                }
                else if(BankNameSelected.Equals("ICICI Bank") || BankNameSelected.Equals("Fingpay"))
                {
                    #region Checksum (UserId|Token|checksum)

                    string input = Checksum.MakeChecksumString("GetFingpayAllAepsBankDetails", Checksum.checksumKey, requestModel.UserId);
                    CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                    #endregion

                    requestModel.checksum = CheckSum;
                    var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/GetFingpayAllAepsBankDetails");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    if (string.IsNullOrEmpty(result))
                    {
                        //return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                    }
                    else
                    {
                        var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                        if (deserialize.Statuscode == "TXN" && deserialize != null)
                        {
                            var datadeserialize = deserialize.Data;
                            var data = JsonConvert.DeserializeObject<List<AepsGetAllBankListResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                            if (data != null)
                            {
                                foreach (var item in data)
                                {
                                    Data1.Add(new AepsGetAllBankListResponseModel
                                    {
                                        Id = item.Id,
                                        BankName = item.BankName,
                                        IIN = item.IIN,
                                        BankCode = item.BankCode,
                                    });
                                }

                            }
                            return Json(Data1);
                        }
                        else if (deserialize.Statuscode == "ERR")
                        {
                            return Json(deserialize);
                        }
                    }
                    return Json(Data1);

                }
                return Json("");

               

                
            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = requestModel;
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion

        #region TriggerMachine
        public string TriggerMachine()
        {
            try
            {
                string completeUrl2 = "http://localhost:11100/rd/capture";
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(completeUrl2);
                request2.Method = "CAPTURE";
                request2.Credentials = CredentialCache.DefaultCredentials;
                StreamWriter writer = new StreamWriter(request2.GetRequestStream());
                string pidOptString = "<PidOptions><Opts fCount=\"1\" dType=\"P\" fType=\"2\" iCount=\"0\" pCount=\"0\" format=\"0\" pidVer=\"2.0\" timeout=\"20000\" otp=\"\" posh=\"LEFT_INDEX\" env=\"P\" wadh=\"\" /> <Demo></Demo> <CustOpts> <Param name=\"Param1\" value=\"\" /> </CustOpts> </PidOptions>";
                //string pidOptString = "<PidOptions ver = \"1.0\"><Opts env = \"P\" fCount = \"1\" fType = \"2\" format = \"1\" iCount = \"0\" iType = \"0\" pCount = \"0\" pType = \"0\" pidVer = \"2.0\" posh = \"UNKNOWN\" timeout = \"10000\" /></PidOptions>";
                writer.WriteLine(pidOptString);
                writer.Close();
                WebResponse response2 = default(WebResponse);
                response2 = request2.GetResponse();
                Stream str2 = response2.GetResponseStream();
                StreamReader sr2 = new StreamReader(str2);
                finalResponse2 = sr2.ReadToEnd();
                return finalResponse2;
                //XmlDocument xml = new XmlDocument();
                //finalResponse2 = finalResponse2.Replace("< ", "<").Replace(" >", ">").Replace("</ ", "</").Replace("? xml", "?xml");

                //xml.LoadXml(finalResponse2);
                
                //XmlNodeList xnList = xml.SelectNodes("/PidData/Data");
                //string X = string.Empty;

                //if (xnList != null)
                //{
                //    //if (xnList[0] != null)
                //    //{
                //    //if (xnList[0].Attributes["X"] != null)
                //    //{
                //    //X = db.NullToString(xnList[0].Attributes["dc"]?.InnerText);
                //    X = xnList[0].LastChild.InnerText;
                //    //}
                //    //}
                //}
                //return X;

            }
            catch (Exception ex)
            {
                //Console.ReadLine(ex);
            }
            return "";
        }
        #endregion

        #region getAddress
        public RootObject getAddress(string lat, string longg)
        {
            WebClient webClient = new WebClient();
            double latt = double.Parse(lat);
            //double latt = 28.6262498;
            double longgt = double.Parse(longg);
            //double longgt = 77.3734622;
            webClient.Headers.Add("user-agent", "Chrome/21 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            webClient.Headers.Add("Referer", "http://www.microsoft.com");
            var jsonData = webClient.DownloadData("http://nominatim.openstreetmap.org/reverse?format=json&lat=" + latt + "&lon=" + longgt);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
            RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));
            return rootObject;
        }
        #endregion


        //public string XMLRemoveData()
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(finalResponse2);
        //    string xx = string.Empty;
        //    foreach (XmlNode node in doc)
        //    {
        //        if (node.NodeType == XmlNodeType.XmlDeclaration)
        //        {
        //            xx= doc.RemoveChild(node).ToString();
        //        }
        //    }
        //    return xx;
        //}


        #region GetTrnsactionMode
        public string TransactionMode()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("TransactionMode")))
            {
                string ID = HttpContext.Session.GetString("TransactionMode");
                return ID;
            }
            
            return "";

        }

        #endregion

        #region GetDeviceName
        public string DeviceName()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("DeviceName")))
            {
                string mobile = HttpContext.Session.GetString("DeviceName");
                return mobile;
            }
            return "";

        }

        #endregion

        #region GetBankName
        public string BankName()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("BankName")))
            {
                string name = HttpContext.Session.GetString("BankName");
                return name;
            }
            return "";
        }
        #endregion

        #region SelfHelp
        public JsonResult SelfHelp(string ServiceId)
        {

            try
            {
                SelfHelpRequestModel SelfRequest = new SelfHelpRequestModel();
                SelfRequest.Userid = "NA";
                SelfRequest.ServiceId = ServiceId;
                string input = Checksum.MakeChecksumString("ViewFQL", Checksum.checksumKey,
                    SelfRequest.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                SelfRequest.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.ViewFQL}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(SelfRequest);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return Json(new { Result = "EmptyResult", url = Url.Action("ErrorForExceptionLog", "Error") });
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<ResponseModel1>(response.Content);
                    var datadeserialize = deserialize.Data;
                    var data = JsonConvert.DeserializeObject<List<SelfHelpResponseModel>>(JsonConvert.SerializeObject(datadeserialize));
                    //var datadeserialize = deserialize.Data;
                    //var TranferData = JsonConvert.DeserializeObject<GetcashdepositeResponseModel>(JsonConvert.SerializeObject(datadeserialize));
                    return Json(data);
                }

            }
            catch (Exception ex)
            {
                ExceptionLogRequestModel requestModelEx = new ExceptionLogRequestModel();
                requestModelEx.ExceptionMessage = ex;
                requestModelEx.Data = "";
                var clientEx = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var requestEx = new RestRequest(Method.POST);
                requestEx.AddHeader("Content-Type", "application/json");
                var jsonEx = JsonConvert.SerializeObject(requestModelEx);
                requestEx.AddJsonBody(jsonEx);
                IRestResponse responseEx = clientEx.Execute(requestEx);
                var resultEx = responseEx.Content;
                return Json(new { Result = "RedirectToException", url = Url.Action("ErrorForExceptionLog", "Error") });
            }
        }
        #endregion


        #region Replace Service ID's
        private string ReplaceServiceID(string serviceId)
        {
            switch (serviceId)
            {
                //Redmil Business Mall
                case "1": return "Commercial loans";
                case "2": return "Home loan";
                case "3": return "Instant loan";

                // Banking Services
                case "4": return "Credit score";
                case "5": return "Fastag";
                case "6": return "Mutual Funds";

                case "7": return "Fixed Deposits";
                case "8": return "Gold Investment";
                case "9": return "Insurance";
                case "10": return "Real State Porta";

                case "11": return "CA Services";
                case "12": return "Software";
                case "13": return "Digital Marketing";

                case "14": return "Web Development";
                case "15": return "ISO Service";

                // Payment Services
                case "16": return "Copyright And Trademark";
                case "17": return "Flight Booking";
                case "18": return "Hotel Booking";
                case "19": return "Bus Booking";
                case "20": return "Car Booking";
                case "21": return "Mobile Prepaid";

                // Investment Services
                case "22": return "Mobile Postpaid";
                case "23": return "DTH Bill";

                // Banking Solution Services
                case "24": return "Electricity Bill";
                case "25": return "Gas Bill";


                // Recharge and Bill Payments
                case "26": return "Aadhar ATM(Aeps)";
                case "27": return "Money Transfer";
                case "28": return "Water Bill";
                case "29": return "Business Loan";
                case "30": return "Personal Loan";

                // Travel Services
                case "31": return "Credit";
                case "32": return "Debit";

                // Digital Distribution Services
                case "33": return "Cash Out";

                //Redmil Business Mall
                case "34": return "Default";
                case "35": return "UPI Services";
                case "36": return "Profile";

                // Banking Services
                case "37": return "Broadband Postpaid (BBPS)";
                case "38": return "Cabel TV (BBPS)";
                case "39": return "Fastag Recharge (BBPS)";

                case "40": return "Health Insurance (BBPS)";
                case "41": return "Landline Postpaid (BBPS)";
                case "42": return "Life Insurance (BBPS)";
                case "43": return "Loan Repayment (BBPS)";

                case "44": return "LPG Gas (BBPS)";
                case "45": return "Virtual Payment";
                case "46": return "";

                case "47": return "Micro ATM";
                case "48": return "";

                // Payment Services
                case "49": return "";
                case "50": return "";
                case "51": return "";
                case "52": return "Branding Material";
                case "53": return "Team Member joining";
                case "54": return "Sign Up";

                // Investment Services
                case "55": return "Credit Card (BBPS)";
                case "56": return "Hospital (BBPS)";

                // Banking Solution Services
                case "57": return "Housing Society (BBPS)";
                case "58": return "Insurance (BBPS)";


                // Recharge and Bill Payments
                case "59": return "Municipal Services (BBPS)";
                case "60": return "Municipal Taxes (BBPS)";
                case "61": return "Subscription (BBPS)";
                case "62": return "BBPS Bill Payment";
                case "63": return "BBPS Bill Payment No Surcharge";

                // Travel Services
                case "64": return "Redmil Offer";
                case "65": return "Prepaid Card";

                // Digital Distribution Services
                case "66": return "POS Payments";

                case "67": return "Credit Advance Wallet Limit";
                case "68": return "UTI Pan Card";

                // Digital Distribution Services
                case "69": return "HDFC Account Opening";
                case "70": return "SBI Credit Card";

                case "71": return "HDFC Current Account Opening";
                case "72": return "SMS Payment Link";
                case "73": return "Axis Bank Account Opening";
                case "74": return "Upstox Demate Account";

                case "75": return "Axis Bank Current Account Opening";
                case "76": return "Paytm Money Demat Account";
                case "77": return "Refer & Earn";

                case "78": return "Paytm Aeps";
                case "79": return "HDFC Bank Credit Card";

                // Payment Services
                case "80": return "Axis Bank Credit Card";
                case "81": return "Aadhar Pay";
                case "82": return "Buy Now Pay Later";
                case "83": return "Bajaj Finserv";
                case "84": return "Amazon Easy";
                case "85": return "Groww Demat Account";

                // Investment Services
                case "86": return "Axis Direct Demat Account";
                case "87": return "Kotak811 Saving Account";

                // Banking Solution Services
                case "88": return "";
                case "89": return "";


                // Recharge and Bill Payments
                case "90": return "LIC";
                case "91": return "Jupiter Savings Account";
                case "92": return "Equitas Bank Savings Account";
                case "93": return "Fi Money Savings Account";
                case "94": return "Niyo Bank Savings Account";

                // Travel Services
                case "95": return "Zest Money EMI Card";
                case "96": return "CITI Bank Credit Card";

                // Digital Distribution Services
                case "97": return "HDFC Securities Demat Account";

                case "98": return "Bajaj Finserv Fixed Deposits";
                case "99": return "ICICI UPI Services";

                // Digital Distribution Services
                case "100": return "Jiffy Demat Account";
                case "1001": return "Funds India Demat Account";

                default: return "All";
            }
        }
        #endregion


    }
}
