using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project_Redmil_MVC.CommonHelper;
using Project_Redmil_MVC.Models;
using Project_Redmil_MVC.Models.RequestModel;
using Project_Redmil_MVC.Models.RequestModel.DMT2RequestModel;
using Project_Redmil_MVC.Models.ResponseModel;
using Project_Redmil_MVC.Models.ResponseModel.DMT2ResponseModel;

namespace Project_Redmil_MVC.Controllers.BankingServicesController.DMT2._0Controller
{
    public class DMT2DashboardController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public string SenderNAME;
        public List<GetSenderDetailsResponseModel.AdditionalInfo> lstDataList;

        public DMT2DashboardController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }

        [HttpGet]
        public IActionResult DMTDashboard()
        {
            string Rdata = ViewBag.Data;
            if (Convert.ToInt32(HttpContext.Session.GetString("Id")) <= 0)
            {
                return RedirectToAction("ErrorForLogin", "Error");
            }
            //if (TempData["IdNames"] != null)
            //{
            //    var userdata = TempData["IdNames"];
            //    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<GetSenderStatusResponseModel>>>(userdata.ToString());
            //}

            var balance = GetBalance();
            ViewBag.balance = balance;
            GetSenderDetailsRequestModel requestModel = new GetSenderDetailsRequestModel();
            try
            {
                requestModel.SenderMobile = SenderMobile();
                requestModel.SenderId = SenderId();
                requestModel.UserId = "2084";

                #region Checksum (senderdetails|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("senderdetails", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.SenderDetails}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseBillResponseModelNew>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var data = deserialize.Data;
                    var adData = deserialize.AdData;

                    List<GetSenderDetailsResponseModel.Data> datalist1 = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.Data>>(data.ToString());
                    SenderNAME = datalist1.FirstOrDefault().SenderName;
                    lstDataList = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.AdditionalInfo>>(adData.ToString());
                    GetSenderDetailsResponseModel mymodel = new GetSenderDetailsResponseModel();
                    mymodel.data = datalist1;
                    mymodel.additionalInfo = lstDataList;
                    return View(mymodel);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return View();
                }
                else
                {
                    return View();
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
            return View();
        }

        #region AddBeneficiary
        [HttpPost]

        public JsonResult AddBeneficiary(string accName, string accountNum, string IFSC, string Bank)
        {
            AddBeneficiaryDetailsRequestModel requestModel = new AddBeneficiaryDetailsRequestModel();
            try
            {
                requestModel.BeneficiaryName = accName;
                requestModel.AccountNumber = accountNum;
                requestModel.SenderId = SenderId();
                requestModel.UserId = "2084";
                requestModel.BankName = Bank;
                requestModel.IFSCCode = IFSC;
                #region Checksum (addbeneficiarydetails|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("addbeneficiarydetails", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.AddBeneficiarydDetails}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<AddBeneficiaryDetailsResponseModel>>>(response.Content);
                if (deserializ.Statuscode == "TXN" && deserializ != null)
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                }
                else if (deserializ.Statuscode == "ERR")
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
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

        #region GetDetailsById
        [HttpPost]
        public JsonResult GetDetailsById(int? id)
        {

            GetSenderDetailsRequestModel requestModel = new GetSenderDetailsRequestModel();
            try
            {
                requestModel.SenderMobile = SenderMobile();
                requestModel.SenderId = SenderId();
                requestModel.UserId = "2084";

                #region Checksum (senderdetails|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("senderdetails", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.SenderDetails}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseBillResponseModelNew>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var data = deserialize.Data;
                    var data22 = deserialize.AdData;

                    List<GetSenderDetailsResponseModel.Data> datalist1 = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.Data>>(data.ToString());
                    lstDataList = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.AdditionalInfo>>(data22.ToString());

                    var a = lstDataList.Where(x => x.BenificiaryId == id).ToList();
                    return Json(a);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
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

        #region DeleteBeneficiery
        [HttpPost]
        public JsonResult DeleteBeneficiery(string id)
        {

            DeleteBeneficiaryRequestModel requestModel = new DeleteBeneficiaryRequestModel();
            try
            {
                requestModel.BeneficiaryId = id;
                requestModel.UserId = "2084";

                #region Checksum (deletebeneficiarydetails|Unique Key|UserId|ServiceId)

                string input = Checksum.MakeChecksumString("deletebeneficiarydetails", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);

                #endregion
                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.DeleteBeneficiaryDetails}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    return Json(deserialize);
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json(deserialize);
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


        #region VerifyDetails
        [HttpPost]
        public JsonResult VerifyDetails(string mobile)
        {
            FinoBankChargesRequestModel requestModel = new FinoBankChargesRequestModel();
            try
            {
                requestModel.SenderMobile = SenderMobile();
                requestModel.UserId = "2084";
                requestModel.PaymentMode = "AccountVerification";
                requestModel.PaymentAmount = "1";

                #region Checksum (finobankcharges| Unique Key|UserId|ServiceId)
                string input = Checksum.MakeChecksumString("finobankcharges", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.FinoBankCharges}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<FinoBankChargesResponseModel>>>(response.Content);
                if (deserializ.Statuscode == "TXN" && deserializ != null)
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                }
                else if (deserializ.Statuscode == "ERR")
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
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


        #region FinalVerifyBeneficiaryAccount
        [HttpPost]
        public JsonResult VerifyBeneficiaryAccount(int id)
        {
            //Get Details Of Beneficiery
            var idData = GetDetailsById(id);
            var jsonValue = idData.Value;
            var listData = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.AdditionalInfo>>(JsonConvert.SerializeObject(jsonValue)).ToList();

            BeneficiaryAccountVerificationRequestModel requestModel = new BeneficiaryAccountVerificationRequestModel();
            try
            {
                requestModel.UserId = "2084";
                requestModel.AccountNumber = listData.FirstOrDefault().AccountNumber;
                requestModel.Amount = "1";
                requestModel.BankName = listData.FirstOrDefault().BankName;
                requestModel.IFSCCode = listData.FirstOrDefault().IFSCCode;
                requestModel.BeneficiaryName = listData.FirstOrDefault().BeneficiaryName;
                requestModel.SenderId = SenderId();
                requestModel.SenderMobile = SenderMobile();
                requestModel.SenderName = "Faisal Siddiqui ";
                requestModel.BeneficiaryId = listData.FirstOrDefault().BenificiaryId.ToString();
                requestModel.BeneIFSCCode = listData.FirstOrDefault().IFSCCode;
                requestModel.BeneName = listData.FirstOrDefault().BeneficiaryName;
                requestModel.CustomerMobileNo = requestModel.SenderMobile;
                requestModel.CustomerName = requestModel.SenderName;
                requestModel.BeneAccountNo = requestModel.AccountNumber;
                requestModel.Wallet = true;

                #region Checksum (beneficiaryaccountverification| Unique Key|UserId)
                string input = Checksum.MakeChecksumString("beneficiaryaccountverification", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.CheckSum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.BeneficiaryAccountVerification}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;
                var deserialize = JsonConvert.DeserializeObject<BaseBillResponseModelNew>(response.Content);
                if (deserialize.Statuscode == "TXN")
                {
                    var data = deserialize.Data;
                    var data22 = deserialize.AdData;
                    List<BeneficiaryAccountVerificationResponseModel.Data> datalist1 = JsonConvert.DeserializeObject<List<BeneficiaryAccountVerificationResponseModel.Data>>(data.ToString());
                    BeneficiaryAccountVerificationResponseModel.AdData datalist2 = JsonConvert.DeserializeObject<BeneficiaryAccountVerificationResponseModel.AdData>(data22.ToString());
                    return Json(new
                    {
                        data = datalist1,
                        additionalInfo = datalist2
                    });
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    return Json(new BaseResponseModel() { Statuscode = deserialize.Statuscode, Message = deserialize.Message });
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

        #region GetBalance
        [HttpPost]
        public string GetBalance()
        {
            GetBalanceRequestModel getBalanceRequestModel = new GetBalanceRequestModel();
            List<GetBalanceResponseModel> lstdata = new List<GetBalanceResponseModel>();
            try
            {
                getBalanceRequestModel.Userid = "2084";
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
                var deserialize = JsonConvert.DeserializeObject<BaseResponseModel>(response.Content);
                if (deserialize.Statuscode == "TXN" && deserialize != null)
                {
                    var data = deserialize.Data;
                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    return lstdata.FirstOrDefault().MainBal.ToString();
                }
                else if (deserialize.Statuscode == "ERR")
                {
                    var data = deserialize.Data;
                    lstdata = JsonConvert.DeserializeObject<List<GetBalanceResponseModel>>(JsonConvert.SerializeObject(data)).ToList();
                    return lstdata.FirstOrDefault().MainBal.ToString();
                }
                else
                {

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
            }
            return lstdata.FirstOrDefault().MainBal.ToString();
        }

        #endregion


        #region FinalPayment
        [HttpPost]
        public JsonResult FinalPayment(int id, string amount, string mode, string payment)
        {
            var idData = GetDetailsById(id);
            var jsonValue = idData.Value;
            var listData = JsonConvert.DeserializeObject<List<GetSenderDetailsResponseModel.AdditionalInfo>>(JsonConvert.SerializeObject(jsonValue)).ToList();

            FinalPaymentRequestModel requestModel = new FinalPaymentRequestModel();
            try
            {
                requestModel.UserId = "2084";
                requestModel.CustomerMobileNo = SenderMobile();
                requestModel.CustomerName = SenderName();
                requestModel.SenderId = SenderId();
                requestModel.AccountNumber = listData.FirstOrDefault().AccountNumber;
                requestModel.BankName = listData.FirstOrDefault().BankName;
                requestModel.BeneAccountNo = listData.FirstOrDefault().AccountNumber;
                requestModel.BeneficiaryId = listData.FirstOrDefault().BenificiaryId.ToString();
                requestModel.BeneIFSCCode = listData.FirstOrDefault().IFSCCode;
                requestModel.BeneName = listData.FirstOrDefault().BeneficiaryName;
                requestModel.SenderName = requestModel.CustomerName;
                requestModel.SenderMobile = requestModel.CustomerMobileNo;
                requestModel.BeneficiaryName = requestModel.BeneName;
                requestModel.IFSCCode = requestModel.BeneIFSCCode;
                requestModel.Amount = amount;
                requestModel.Wallet = payment;
                #region Checksum (processneftrequest|Unique Key|UserId)
                if (!string.IsNullOrEmpty(mode) && mode.Equals("NEFT"))
                {
                    string input = Checksum.MakeChecksumString("processneftrequest", Checksum.checksumKey, requestModel.UserId);
                    string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                    requestModel.Checksum = CheckSum;

                    var client = new RestClient($"{Baseurl}{ApiName.ProcessNEFTRequest}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;
                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<FinalPaymentNEFTResponseModel>>>(response.Content);
                    if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
                    }
                    else
                    {
                        return Json("");
                    }
                }
                #endregion

                #region Checksum (processimpsrequest| Unique Key|UserId)
                if (!string.IsNullOrEmpty(mode) && mode.Equals("IMPS"))
                {
                    string input = Checksum.MakeChecksumString("processimpsrequest", Checksum.checksumKey, requestModel.UserId);
                    string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                    requestModel.Checksum = CheckSum;

                    var client = new RestClient($"{Baseurl}{ApiName.ProcessIMPSRequest}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    var json = JsonConvert.SerializeObject(requestModel);
                    request.AddJsonBody(json);
                    IRestResponse response = client.Execute(request);
                    var result = response.Content;

                    var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<FinalPaymentIMPSResponseModel>>>(response.Content);

                    if (deserializ.Statuscode == "TXN" && deserializ != null)
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                    }
                    else if (deserializ.Statuscode == "ERR")
                    {
                        return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
                    }
                    else
                    {
                        return Json("");
                    }
                }
                #endregion
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


        #region VerifyDetails
        [HttpPost]
        public JsonResult BankChargesDetails(string amount, string mode)
        {
            FinoBankChargesRequestModel requestModel = new FinoBankChargesRequestModel();
            try
            {
                requestModel.SenderMobile = SenderMobile();
                requestModel.UserId = "2084";
                if (mode.Equals("IMPS"))
                {
                    requestModel.PaymentMode = "IMPS";
                }
                if (mode.Equals("NEFT"))
                {
                    requestModel.PaymentMode = "NEFT";
                }

                requestModel.PaymentAmount = amount;

                #region Checksum (finobankcharges| Unique Key|UserId|ServiceId)
                string input = Checksum.MakeChecksumString("finobankcharges", Checksum.checksumKey, requestModel.UserId);
                string CheckSum = Checksum.ConvertStringToSCH512Hash(input);
                #endregion

                requestModel.Checksum = CheckSum;
                var client = new RestClient($"{Baseurl}{ApiName.FinoBankCharges}");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(requestModel);
                request.AddJsonBody(json);
                IRestResponse response = client.Execute(request);
                var result = response.Content;

                var deserializ = JsonConvert.DeserializeObject<BaseResponseModelT<List<FinoBankChargesResponseModel>>>(response.Content);
                if (deserializ.Statuscode == "TXN" && deserializ != null)
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message, Data = deserializ.Data.FirstOrDefault() });
                }
                else if (deserializ.Statuscode == "ERR")
                {
                    return Json(new BaseResponseModel() { Statuscode = deserializ.Statuscode, Message = deserializ.Message });
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


        public string SenderId()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderId")))
            {
                string ID = HttpContext.Session.GetString("SenderId");
                return ID;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderIdNewUser")))
            {
                string ID = HttpContext.Session.GetString("SenderIdNewUser");
                return ID;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RecentSenderId")))
            {
                string ID = HttpContext.Session.GetString("RecentSenderId");
                return ID;
            }
            return "";

        }
        public string SenderMobile()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderMobile")))
            {
                string mobile = HttpContext.Session.GetString("SenderMobile");
                return mobile;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderMobileNewUser")))
            {
                string mobile = HttpContext.Session.GetString("SenderMobileNewUser");
                return mobile;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RecentSenderMobile")))
            {
                string mobile = HttpContext.Session.GetString("RecentSenderMobile");
                return mobile;
            }
            return "";

        }

        public string SenderName()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderName")))
            {
                string name = HttpContext.Session.GetString("SenderName");
                return name;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SenderNameNewUser")))
            {
                string name = HttpContext.Session.GetString("SenderNameNewUser");
                return name;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RecentSenderName")))
            {
                string name = HttpContext.Session.GetString("RecentSenderName");
                return name;
            }
            return "";

        }
    }
}
