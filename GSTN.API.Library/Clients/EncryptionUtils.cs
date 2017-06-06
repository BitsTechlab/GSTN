using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Numerics;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;

namespace GSTN.API
{


	public class EncryptionUtils
	{

		public static X509Certificate2 getPublicKey()
		{
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
			X509Certificate2 cert2 = new X509Certificate2("Resources\\GSTN_public.cer");
			return cert2;
		}

		public static string HMAC_Encrypt(string message, string secret)
		{
			secret = secret ?? "";
			var encoding = new System.Text.ASCIIEncoding();
			byte[] keyByte = encoding.GetBytes(secret);
			byte[] messageBytes = encoding.GetBytes(message);
            using (var HMACSHA256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = HMACSHA256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }


		}
        public static string GenerateBase64HMAC(string msg, string key)
        {
            byte[] encodeMsg = Encoding.UTF8.GetBytes(msg);
            string base64Msg = Convert.ToBase64String(encodeMsg);
            byte[] EK = Encoding.UTF8.GetBytes(key);
            return GenerateHMAC(base64Msg, EK);
        }
        public static string GenerateHMAC(string Base64Msg, byte[] EK)
		{
            using (var HMACSHA256 = new HMACSHA256(EK))
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(Base64Msg);
                byte[] hashmessage = HMACSHA256.ComputeHash(data);
                return Convert.ToBase64String(hashmessage);
            }
		}
		public static string HMAC_Encrypt(byte[] EK)
		{
            using (var HMACSHA256 = new HMACSHA256())
            {
                byte[] hashmessage = HMACSHA256.ComputeHash(EK);
                return Convert.ToBase64String(hashmessage);
            }
		}

		public static string AesEncrypt(string plainText, byte[] keyBytes)
		{
			byte[] dataToEncrypt = UTF8Encoding.UTF8.GetBytes(plainText);
            return EncryptionUtils.AesEncrypt(dataToEncrypt, keyBytes);
		
		}

		public static string AesEncrypt(string plainText, string key)
		{
			byte[] data = UTF8Encoding.UTF8.GetBytes(plainText);
			byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(key);
			return EncryptionUtils.AesEncrypt(data, keyBytes);

		}


		public static string AesEncrypt(byte[] dataToEncrypt, byte[] keyBytes)
		{
            AesManaged tdes = new AesManaged();

            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keyBytes;
            // Encoding.ASCII.GetBytes(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

		public static byte[] AesDecrypt(string encryptedText, string key)
		{

			byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);

			return AesDecrypt(dataToDecrypt, keyBytes);

		}

		public static byte[] AesDecrypt(string encryptedText, byte[] keys)
		{
			byte[] dataToDecrypt = Convert.FromBase64String(encryptedText);
			return AesDecrypt(dataToDecrypt, keys);
		}



		public static byte[] AesDecrypt(byte[] dataToDecrypt, byte[] keys)
		{

			AesManaged tdes = new AesManaged();
			tdes.KeySize = 256;
			tdes.BlockSize = 128;
			tdes.Key = keys;
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;

			ICryptoTransform decrypt__1 = tdes.CreateDecryptor();
			byte[] deCipher = decrypt__1.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
			tdes.Clear();

			return deCipher;
		}




		public static string RSAEncrypt(string input)
		{
			byte[] bytesToBeEncrypted = Encoding.ASCII.GetBytes(input);
			return RsaEncrypt(bytesToBeEncrypted);
		}

		private static readonly byte[] Salt = new byte[] {
			10,
			20,
			30,
			40,
			50,
			60,
			70,
			80
		};
		public static byte[] CreateAesKey()
		{

			System.Security.Cryptography.AesCryptoServiceProvider crypto = new System.Security.Cryptography.AesCryptoServiceProvider();
			crypto.KeySize = 256;
			crypto.GenerateKey();
			byte[] key = crypto.Key;
			return key;
		}

		public static string RsaEncrypt(byte[] bytesToBeEncrypted)
		{
			X509Certificate2 certificate = getPublicKey();
			RSACryptoServiceProvider RSA = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            

			byte[] bytesEncrypted = RSA.Encrypt(bytesToBeEncrypted, false);

			string result = Convert.ToBase64String(bytesEncrypted);
			return result;
		}

        public static string sha256_hash(string value)
        {

            using (SHA256 hash = SHA256Managed.Create())
            {
                Byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                return Convert.ToBase64String(result);
            }

        }

        /// <summary>
        /// Generate Hash value from given string value
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A managed SHA 256 hash value</returns>
        public static byte[] Generatehash256(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA256Managed hashString = new SHA256Managed();
            hashValue = hashString.ComputeHash(message);
            return hashValue;
        }


        public static byte[] convertStringToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }
       

        public static string RsaEncryptBC(byte[] bytesToEncrypt)
        {

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            string certificateLocation = "Resources\\GSTN_G2B_SANDBOX_UAT_public.pem";
            string publicKey = File.ReadAllText(certificateLocation).Replace("RSA PUBLIC","PUBLIC");


            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (AsymmetricKeyParameter)new PemReader(txtreader).ReadObject();
                
                encryptEngine.Init(true, keyParameter);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;

        }

        public static byte[] CreateAesKeyBC()
        {
            SecureRandom random = new SecureRandom();
            byte[] keyBytes = new byte[32]; //32 Bytes = 256 Bits
            random.NextBytes(keyBytes);

            var key = ParameterUtilities.CreateKeyParameter("AES", keyBytes);
            return key.GetKey();
        }
        public byte[] Hash(string text, string key)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(key)));
            byte[] result = new byte[hmac.GetMacSize()];
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            hmac.BlockUpdate(bytes, 0, bytes.Length);
            hmac.DoFinal(result, 0);

            return result;
        }
        public static string GenerateBase64HMACBC(string msg, string key)
        {
            byte[] encodeMsg = Encoding.UTF8.GetBytes(msg);
            string base64Msg = Convert.ToBase64String(encodeMsg);
            byte[] EK = Encoding.UTF8.GetBytes(key);
            return GenerateHMACBC(base64Msg, EK);
        }
        public static string GenerateHMACBC(string Base64Msg, byte[] EK)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(EK));
            byte[] hashMessage = new byte[hmac.GetMacSize()];
            byte[] bytes = Encoding.UTF8.GetBytes(Base64Msg);

            hmac.BlockUpdate(bytes, 0, bytes.Length);
            hmac.DoFinal(hashMessage, 0);
            return Convert.ToBase64String(hashMessage);
        }
    }

}



