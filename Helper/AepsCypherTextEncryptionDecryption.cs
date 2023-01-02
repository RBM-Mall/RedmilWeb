using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace Project_Redmil_MVC.Helper
{
    public sealed class AepsCypherTextEncryptionDecryption
    {

        protected RijndaelManaged myRijndael;

        //private static string encryptionKey = "Ef6d2GRq98";
        private static string encryptionKey = "Ef6d2GRq98";
        //private static string initialisationVector = "26744a68b53dd87bb395584c00f7290a";

        // Singleton pattern used here with ensured thread safety
        protected static readonly AepsCypherTextEncryptionDecryption _instance = new AepsCypherTextEncryptionDecryption();
        public static AepsCypherTextEncryptionDecryption Instance
        {
            get { return _instance; }
        }

        public AepsCypherTextEncryptionDecryption()
        {

        }

        public string YBLEncrypt(string textToDecrypt, string key)

        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,

                Padding = PaddingMode.PKCS7,

                //

                KeySize = 0x80,

                BlockSize = 0x80
            };

            //   byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] encryptedData = Encoding.UTF8.GetBytes(textToDecrypt);

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

            byte[] keyBytes = new byte[0x10];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length)

            {

                len = keyBytes.Length;

            }

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            rijndaelCipher.IV = keyBytes;

            byte[] plainText = rijndaelCipher.CreateEncryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Convert.ToBase64String(plainText, 0, plainText.Length);

        }


        public string YBLDecrypt(string textToDecrypt, string key)

        {

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.PKCS7;

            //

            rijndaelCipher.KeySize = 0x80;

            rijndaelCipher.BlockSize = 0x80;

            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

            byte[] keyBytes = new byte[0x10];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length)

            {
                len = keyBytes.Length;
            }

            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        #region Encryption for Aadhar Number
        public string EncryptionAadharNumber(string Uid)
        {
            string pubB64 = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCK8xP3JO4exQPIB2eDpAVXasM3YOoZN405HuaSjr1FVE0Z++jKrVhTiOYqiXX7ksChmoEt4uim+tWK/1SNpMyD/nl4SsQjkG0zRJr+kfP4owDnQdZRDPpLZABI2X5O6o5bgwPsxY3UfuenwrKc1/FQRITfaTp7nyoX956EZ9v4dQIDAQAB";
            string text = Uid;
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] publicKeyBytes = Convert.FromBase64String(pubB64);

            var keyLengthBits = 1024;  // need to know length of public key in advance!
            byte[] exponent = new byte[3];
            byte[] modulus = new byte[keyLengthBits / 8];
            Array.Copy(publicKeyBytes, publicKeyBytes.Length - exponent.Length, exponent, 0, exponent.Length);
            Array.Copy(publicKeyBytes, publicKeyBytes.Length - exponent.Length - 2 - modulus.Length, modulus, 0, modulus.Length);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters rsaKeyInfo = rsa.ExportParameters(false);
            rsaKeyInfo.Modulus = modulus;
            rsaKeyInfo.Exponent = exponent;
            rsa.ImportParameters(rsaKeyInfo);

            byte[] encrypted = rsa.Encrypt(textBytes, RSAEncryptionPadding.Pkcs1);
            string data = (Convert.ToBase64String(encrypted));
            return data;
        }

        #endregion
        //public string DecryptText(string encryptedString)
        //{
        //    using (myRijndael = new RijndaelManaged())
        //    {

        //        myRijndael.Key = HexStringToByte(encryptionKey);
        //        myRijndael.IV = HexStringToByte(initialisationVector);
        //        myRijndael.Mode = CipherMode.CBC;
        //        myRijndael.Padding = PaddingMode.PKCS7;

        //        Byte[] ourEnc = Convert.FromBase64String(encryptedString);
        //        string ourDec = DecryptStringFromBytes(ourEnc, myRijndael.Key, myRijndael.IV);

        //        return ourDec;
        //    }
        //}
        ////    private String Encrypt(String text, String key) throws Exception
        ////    {
        ////        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        ////    byte[] keyBytes = new byte[16];
        ////    byte[] b = key.getBytes("UTF-8");
        ////    int len = b.length;
        ////    if (len > keyBytes.length) len = keyBytes.length;
        ////    System.arraycopy(b, 0, keyBytes, 0, len);
        ////    SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
        ////    IvParameterSpec ivSpec = new IvParameterSpec(keyBytes);
        ////    cipher.init(Cipher.ENCRYPT_MODE, keySpec, ivSpec);
        ////    byte[] results = cipher.doFinal(text.getBytes("UTF-8"));
        ////    String s = getString(org.apache.commons.codec.binary.Base64.encodeBase64(results));
        ////    return s;
        ////}

        //public string EncryptText(string plainText)
        //{
        //    string PlainText = JsonConvert.DeserializeObject(plainText).ToString();
        //    using (myRijndael = new RijndaelManaged())
        //    {

        //        myRijndael.Key = HexStringToByte(encryptionKey);
        //        myRijndael.IV = HexStringToByte(initialisationVector);
        //        myRijndael.Mode = CipherMode.CBC;
        //        myRijndael.Padding = PaddingMode.PKCS7;

        //        byte[] encrypted = EncryptStringToBytes(plainText, myRijndael.Key, myRijndael.IV);
        //        string encString = Convert.ToBase64String(encrypted);

        //        return encString;
        //    }
        //}


        //protected byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments. 
        //    if (plainText == null || plainText.Length <= 0)
        //        throw new ArgumentNullException("plainText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    byte[] encrypted;
        //    // Create an RijndaelManaged object 
        //    // with the specified key and IV. 
        //    using (RijndaelManaged rijAlg = new RijndaelManaged())
        //    {
        //        rijAlg.Key = Key;
        //        rijAlg.IV = IV;

        //        // Create a decrytor to perform the stream transform.
        //        ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

        //        // Create the streams used for encryption. 
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {

        //                    //Write all data to the stream.
        //                    swEncrypt.Write(plainText);
        //                }
        //                encrypted = msEncrypt.ToArray();
        //            }
        //        }
        //    }


        //    // Return the encrypted bytes from the memory stream. 
        //    return encrypted;

        //}

        //protected string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments. 
        //    if (cipherText == null || cipherText.Length <= 0)
        //        throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("Key");

        //    // Declare the string used to hold 
        //    // the decrypted text. 
        //    string plaintext = null;

        //    // Create an RijndaelManaged object 
        //    // with the specified key and IV. 
        //    using (RijndaelManaged rijAlg = new RijndaelManaged())
        //    {
        //        rijAlg.Key = Key;
        //        rijAlg.IV = IV;

        //        // Create a decrytor to perform the stream transform.
        //        ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        //        // Create the streams used for decryption. 
        //        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {

        //                    // Read the decrypted bytes from the decrypting stream 
        //                    // and place them in a string.
        //                    plaintext = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }

        //    }

        //    return plaintext;

        //}

        //public static void GenerateKeyAndIV()
        //{
        //    // This code is only here for an example
        //    RijndaelManaged myRijndaelManaged = new RijndaelManaged();
        //    myRijndaelManaged.Mode = CipherMode.CBC;
        //    myRijndaelManaged.Padding = PaddingMode.PKCS7;

        //    myRijndaelManaged.GenerateIV();
        //    myRijndaelManaged.GenerateKey();
        //    string newKey = ByteArrayToHexString(myRijndaelManaged.Key);
        //    string newinitVector = ByteArrayToHexString(myRijndaelManaged.IV);
        //}

        //protected static byte[] HexStringToByte(string hexString)
        //{
        //    try
        //    {
        //        int bytesCount = (hexString.Length) / 2;
        //        byte[] bytes = new byte[bytesCount];
        //        for (int x = 0; x < bytesCount; ++x)
        //        {
        //            bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
        //        }
        //        return bytes;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public static string ByteArrayToHexString(byte[] ba)
        //{
        //    StringBuilder hex = new StringBuilder(ba.Length * 2);
        //    foreach (byte b in ba)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}

        //public static string Decrypt(string textToDecrypt, string key)

        //{

        //    RijndaelManaged rijndaelCipher = new RijndaelManaged();

        //    rijndaelCipher.Mode = CipherMode.CBC;

        //    rijndaelCipher.Padding = PaddingMode.PKCS7;

        //    //

        //    rijndaelCipher.KeySize = 0x80;

        //    rijndaelCipher.BlockSize = 0x80;

        //    byte[] encryptedData = Convert.FromBase64String(textToDecrypt);

        //    byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

        //    byte[] keyBytes = new byte[0x10];

        //    int len = pwdBytes.Length;

        //    if (len > keyBytes.Length)

        //    {
        //        len = keyBytes.Length;
        //    }

        //    Array.Copy(pwdBytes, keyBytes, len);
        //    rijndaelCipher.Key = keyBytes;
        //    rijndaelCipher.IV = keyBytes;
        //    byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        //    return Encoding.UTF8.GetString(plainText);
        //}

    }

}
