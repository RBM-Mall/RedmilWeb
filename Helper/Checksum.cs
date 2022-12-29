namespace Project_Redmil_MVC.Helper
{
    public class Checksum
    {
        public const string checksumKey = "32977065309f4acaad68fdbc1f63db97";
        public static string MakeChecksumString(params string[] names)
        {
            string result = "";

            if (names.Count() == 4 && names[3].ToString().Trim() == "")
            {
                for (int i = 0; i < 3; i++)
                {   

                    result = result + "|" + names[i].ToString();
                }
            }
            else
            {
                for (int i = 0; i < names.Length; i++)
                {
                    result = result + "|" + names[i].ToString();
                }
            }
            
            if (result.Trim().Length > 1) 
            {
                result = result.Substring(1);
            }            

            return result;
        }

        public static string ConvertStringToSCH512Hash(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                // Convert to text

                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("x2"));
              return hashedInputStringBuilder.ToString();
            }
        }
    }
}
