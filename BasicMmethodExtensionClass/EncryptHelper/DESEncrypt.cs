using System;
using System.Security.Cryptography;
using System.Text;

namespace BasicMmethodExtensionClass.EncryptHelper
{
    /// <summary>
    /// DES����/�����ࡣ
    /// </summary>
    public class DesEncrypt
    {
        private static readonly string KEY = "pengze0902";

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, KEY);
        }
        /// <summary> 
        /// �������� 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(Text);
            var bKey = Encoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }



        /// <summary>
        /// ����
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(string text)
        {
            return Decrypt(text, KEY);
        }

        /// <summary> 
        /// �������� 
        /// </summary> 
        /// <param name="text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x;
            for (x = 0; x < len; x++)
            {
                var i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            var bKey = Encoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }


        //// <summary>
        /// ȡ��MD5���ܴ�
        /// </summary>
        /// <param name="input">Դ�����ַ���</param>
        /// <returns>�����ַ���</returns>
        public static string Md5Hash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (var b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }



        /// <summary>
        /// ����ǩ��ʵ��
        /// </summary>
        /// <param name="hashToSign">׼��ǩ��������</param>
        /// <param name="dsaKeyInfo">��Կ��Ϣ</param>
        /// <param name="hashAlg">Hash�㷨����</param>
        /// <returns></returns>
        public static byte[] DsaSignHash(byte[] hashToSign, DSAParameters dsaKeyInfo, string hashAlg)
        {
            if (string.IsNullOrEmpty(hashAlg))
            {
                throw new ArgumentNullException(hashAlg);
            }
            byte[] sig;
            try
            {
                using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
                {
                    dsa.ImportParameters(dsaKeyInfo);
                    DSASignatureFormatter dsaFormatter = new DSASignatureFormatter(dsa);
                    dsaFormatter.SetHashAlgorithm(hashAlg);
                    sig = dsaFormatter.CreateSignature(hashToSign);
                }
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(e.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sig;
        }

        /// <summary>
        /// ��֤ǩ������ʵ��
        /// </summary>
        /// <param name="valueToSign">��ǩ����ԭʼ����</param>
        /// <param name="signedHashValue">ǩ��ֵ</param>
        /// <param name="dsaKeyInfo">˽Կ��Ϣ</param>
        /// <param name="hashAlg">Hash�㷨����</param>
        /// <returns></returns>
        public static bool DsaVerifyHash(byte[] valueToSign, byte[] signedHashValue, DSAParameters dsaKeyInfo, string hashAlg)
        {
            if (string.IsNullOrEmpty(hashAlg))
            {
                throw new ArgumentNullException(hashAlg);
            }
            bool verified;
            try
            {
                using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
                {
                    dsa.ImportParameters(dsaKeyInfo);
                    DSASignatureDeformatter dsaFormatter = new DSASignatureDeformatter(dsa);
                    dsaFormatter.SetHashAlgorithm(hashAlg);
                    verified = dsaFormatter.VerifySignature(valueToSign, signedHashValue);
                }
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException(e.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return verified;
        }

    }
}
