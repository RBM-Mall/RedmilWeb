﻿using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Controllers.BankingServicesController.DMT2._0Controller;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.SelfHelp;
//using Project_Redmil_MVC.Controllers.BankingServicesController.DMT2;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.HowToWork;
using Project_Redmil_MVC.Models.ResponseModel.ResponseForSurcharge;
using Project_Redmil_MVC.Models.ResponseModel.SelfHelpResponseModel;

namespace Project_Redmil_MVC.Controllers
{
    public class WalletController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;

        public WalletController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }


        public IActionResult Wallet()
       {
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> gBRM = new List<GetBalanceResponseModel>();
            getBalanceRequestModel.Userid = HttpContext.Session.GetString("Id").ToString();
            try
            {
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("Getbalance", Checksum.checksumKey, getBalanceRequestModel.Userid);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                var client = new RestClient("https://api.redmilbusinessmall.com/api/Getbalance");
                //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else
                {
                    var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                    if (deserialize.Statuscode == "TXN" && deserialize != null)
                    {
                        var statusCode = deserialize.Statuscode;

                        if (statusCode == "TXN")
                        {
                            var data = deserialize.Data;
                            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
                            lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                            try
                            {
                                foreach (var i in lstdata)
                                {
                                    gBRM.Add(new GetBalanceResponseModel
                                    {
                                        MainBal = i.MainBal,
                                        AdBal = i.AdBal,
                                        TotalIncentives = i.TotalIncentives,
                                        BReward = i.BReward,
                                        Reward = i.Reward,
                                        WalletAmount = (i.AdBal + i.MainBal),
                                        REReward = i.REReward,
                                        //TotalIncentives=i.TotalIncentives
                                        //WalletAmount = string.Format("{0:0.00}", i.AdBal + i.MainBal).ToString()
                                    });
                                }
                                return View(gBRM);
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogRequestModel requestModel1 = new ExceptionLogRequestModel();
                                requestModel1.ExceptionMessage = ex;
                                requestModel1.Data = getBalanceRequestModel;
                                var clientN1 = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                                var requestN1 = new RestRequest(Method.POST);
                                requestN1.AddHeader("Content-Type", "application/json");
                                var jsonN1 = JsonConvert.SerializeObject(requestModel1);
                                requestN1.AddJsonBody(jsonN1);
                                IRestResponse responseN1 = clientN1.Execute(requestN1);
                                var resultN1 = responseN1.Content;
                                return RedirectToAction("ErrorForExceptionLog", "Error");
                            }
                        }
                        return View(gBRM);
                    }
                    else if (deserialize.Statuscode == "ERR")
                    {
                        return View(gBRM);
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
                requestModel1.Data = getBalanceRequestModel;
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


        #region FOr Tour Guide
        public IActionResult TourPackage(string ContestId)
        {

            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
       
            try
            {
                getBalanceRequestModel.ContestId = ContestId;
                getBalanceRequestModel.Userid= HttpContext.Session.GetString("Id").ToString();
                #region Checksum (GetBalance|Unique Key|UserId)
                string input = Checksum.MakeChecksumString("AvailRewardContest", Checksum.checksumKey, getBalanceRequestModel.Userid,getBalanceRequestModel.ContestId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                getBalanceRequestModel.checksum = CheckSum;
                //API URL Has been changed by Siddhartha Sir
                var client = new RestClient("https://proapitest5.redmilbusinessmall.com/api/AvailRewardContest");
                //var client = new RestClient($"{Baseurl}{ApiName.Getbalance}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var lstdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }

                else if (lstdata.Statuscode == "ERR")
                {
                    return Json(lstdata);
                }
                else if (lstdata.Statuscode == "TXN")
                {
                    return Json(lstdata);

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
                requestModel1.Data = getBalanceRequestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return RedirectToAction("ErrorForExceptionLog", "Error");
            }
            return Json("");
        }

        #endregion
        #region FOr Tour Guide
        public IActionResult HowToWork(string CategoryId)
        {

            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();

            try
            {
                getBalanceRequestModel.CategoryId = CategoryId;
            
                var client = new RestClient($"{Baseurl}{ApiName.ViewHowToUseCategoryWise}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(getBalanceRequestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var lstdata = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                 var New_Data=lstdata.Data;
                var data = JsonConvert.DeserializeObject<List<HowToWork>>(JsonConvert.SerializeObject(New_Data));
                //var deserialize_sur = JsonConvert.DeserializeObject<BaseResponseModelT<List<HowToWork>>>(result);
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("ErrorForExceptionLog", "Error");
                }
                else if (lstdata.Statuscode == "ERR")
                {
                    return Json(lstdata);
                }
                else if (lstdata.Statuscode == "TXN")
                {
                    return Json(new { Statuscode = "TXN", data });

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
                requestModel1.Data = getBalanceRequestModel;
                var client = new RestClient("https://api.redmilbusinessmall.com/api/WebPortalExceptionLog");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel1);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                return RedirectToAction("ErrorForExceptionLog", "Error");
            }
            return Json("");
        }

        #endregion

    }
}
