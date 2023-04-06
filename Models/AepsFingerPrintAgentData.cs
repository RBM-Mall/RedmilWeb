namespace Project_Redmil_MVC.Models
{
    public class AdditionalInfo
    {
        public List<Param> Param { get; set; }
    }

    public class Data
    {
        [JsonProperty("@type")]
        public string type { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class DeviceInfo
    {
        [JsonProperty("@dpId")]
        public string dpId { get; set; }

        [JsonProperty("@rdsId")]
        public string rdsId { get; set; }

        [JsonProperty("@rdsVer")]
        public string rdsVer { get; set; }

        [JsonProperty("@mi")]
        public string mi { get; set; }

        [JsonProperty("@mc")]
        public string mc { get; set; }

        [JsonProperty("@dc")]
        public string dc { get; set; }
        public AdditionalInfo additional_info { get; set; }
    }

    public class Param
    {
        [JsonProperty("@name")]
        public string name { get; set; }

        [JsonProperty("@value")]
        public object value { get; set; }
    }

    public class PidData
    {
        public Resp Resp { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
        public Skey Skey { get; set; }
        public string Hmac { get; set; }
        public Data Data { get; set; }
    }

    public class Resp
    {
        [JsonProperty("@errCode")]
        public string errCode { get; set; }

        [JsonProperty("@errInfo")]
        public string errInfo { get; set; }

        [JsonProperty("@fCount")]
        public string fCount { get; set; }

        [JsonProperty("@fType")]
        public string fType { get; set; }

        [JsonProperty("@nmPoints")]
        public string nmPoints { get; set; }

        [JsonProperty("@qScore")]
        public string qScore { get; set; }
    }

    public class Root
    {
        [JsonProperty("?xml")]
        public Xml xml { get; set; }
        public PidData PidData { get; set; }
    }

    public class Skey
    {
        [JsonProperty("@ci")]
        public string ci { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string version { get; set; }
    }




}
