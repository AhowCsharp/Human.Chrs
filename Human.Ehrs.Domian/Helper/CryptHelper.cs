using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Buffers.Text;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace LineTag.Core.Utility
{
    public static class CryptHelper
    {
        public static string StringToBase64(object data)
        {
            string text = data?.ToString();

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static string Base64ToString(object data)
        {
            string base64 = data?.ToString();

            if (string.IsNullOrEmpty(base64))
            {
                return string.Empty;
            }

            var buffer = new Span<byte>(new byte[base64.Length]);
            if (Convert.TryFromBase64String(base64, buffer, out _))
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(data.ToString()));
            }
            else
            {
                return string.Empty;
            }
        }

        #region OneWayHash 單向雜湊(目前MD5和SHA1都有被破解的可能)

        /// <summary>雜湊模式列舉</summary>
        public enum HashMethod
        {
            MD5,
            SHA1,
            SHA256
        }

        /// <summary>SHA256單向雜湊(可用於產生字串資料指紋)</summary>
        /// <param name="data">來源資料</param>
        /// <returns></returns>
        public static string OneWayHash(string data)
        {
            return OneWayHash(data, HashMethod.SHA256, "UTF-8");
        }

        /// <summary>單向雜湊(可用於產生字串資料指紋)</summary>
        /// <param name="hashmethod">雜湊模式</param>
        /// <param name="data">來源資料</param>
        /// <param name="encodingname">編碼名</param>
        /// <returns></returns>
        public static string OneWayHash(string data, HashMethod hashmethod, string encodingname)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            byte[] byHash = null;
            switch (hashmethod)
            {
                case HashMethod.MD5:
                    byHash = MD5.Create().ComputeHash(Encoding.GetEncoding(encodingname).GetBytes(data));
                    break;

                case HashMethod.SHA1:
                    byHash = SHA1.Create().ComputeHash(Encoding.GetEncoding(encodingname).GetBytes(data));
                    break;

                case HashMethod.SHA256:
                    byHash = SHA256.Create().ComputeHash(Encoding.GetEncoding(encodingname).GetBytes(data));
                    break;
            }

            return Convert.ToBase64String(byHash);
        }

        #endregion OneWayHash 單向雜湊(目前MD5和SHA1都有被破解的可能)

        /// <summary>
        /// 加鹽雜湊資料
        /// </summary>
        /// <param name="data">資料</param>
        /// <returns></returns>
        public static string SaltHash(string data)
        {
            byte[] salt;
            byte[] hashed;
            byte[] result = new byte[49];

            using (Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(data, 16, 1000, HashAlgorithmName.SHA256))
            {
                salt = rfc2898DeriveByte.Salt;
                hashed = rfc2898DeriveByte.GetBytes(32);
            }

            //從result[1] 塞 16 個，這代表index[0] = 0
            Buffer.BlockCopy(salt, 0, result, 1, 16);

            //從result[17] 塞 32 個，加總共49個
            Buffer.BlockCopy(hashed, 0, result, 17, 32);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 驗證雜湊資料
        /// </summary>
        /// <param name="hasheddata">雜湊後資料</param>
        /// <param name="data">要驗證的資料</param>
        /// <returns></returns>
        public static bool VerifySaltHash(string hasheddata, string data)
        {
            if (hasheddata == null)
            {
                return false;
            }

            if (data == null)
            {
                throw new ArgumentNullException("password is null");
            }

            byte[] check = Convert.FromBase64String(hasheddata);
            byte[] checkhashed;

            if (check[0] != 0 || check.Length != 49)
            {
                return false;
            }

            //從check index 1，塞16個byte到slat
            byte[] slat = new byte[16];
            Buffer.BlockCopy(check, 1, slat, 0, 16);

            //從check index 17，塞32個byte到hashed
            byte[] hashed = new byte[32];
            Buffer.BlockCopy(check, 17, hashed, 0, 32);

            //用data加入salt再次雜湊取得結果
            using (Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(data, slat, 1000, HashAlgorithmName.SHA256))
            {
                checkhashed = rfc2898DeriveByte.GetBytes(32);
            }

            //驗證結果是否相符
            return hashed.SequenceEqual(checkhashed);
        }
    }
}