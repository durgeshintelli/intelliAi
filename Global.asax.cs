using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MusaService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            string strconstr = Global.DecryptConn(ConfigurationManager.ConnectionStrings["IntelliCatalogueConnectionString"].ToString(), "ABF482", true);
            string strDType = Global.DecryptConn(ConfigurationManager.ConnectionStrings["IntelliCatalogueConnectionType"].ToString(), "ABF482", true);
            Catalog.MainCommon.constr = strconstr;
            Catalog.MainCommon._iDatabaseType = Convert.ToInt32(strDType);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            if (Response.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;

                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-10);

            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            //Session.Clear();
            //Session.Abandon();
            //Session.RemoveAll();
            //if (Response.Cookies["ASP.NET_SessionId"] != null)
            //{
            //    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;

            //    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-10);

            //}
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        #region encription or decryption
        /// <summary>
        /// Purpose: This function is used to encrypt string
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Encrypt(string strtoEncrypt, string strkey, bool bluseHashing)
        {
            if (ConfigurationManager.AppSettings["ASSEMBLY"].ToString().ToUpper() == "MAHINDRA")
            {
                return strtoEncrypt;
            }
            else
            {
                byte[] bytkeyArray;
                byte[] byttoEncryptArray = UTF8Encoding.UTF8.GetBytes(strtoEncrypt);

                if (bluseHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
                }
                else
                    bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = bytkeyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

                return Convert.ToBase64String(bytresultArray, 0, bytresultArray.Length);
            }
        }

        public static string EncryptConn(string strtoEncrypt, string strkey, bool bluseHashing)
        {
            byte[] bytkeyArray;
            byte[] byttoEncryptArray = UTF8Encoding.UTF8.GetBytes(strtoEncrypt);

            if (bluseHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
            }
            else
                bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = bytkeyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

            return Convert.ToBase64String(bytresultArray, 0, bytresultArray.Length);
        }
        /// <summary>
        /// Purpose: This function is used to decrypt encrypt string if Company is Mahindra then 
        /// we are not using encrept or decrypt method only in connection string we are using this..
        /// </summary>
        /// <param name="strtoDecrypt"></param>
        /// <param name="strkey"></param>
        /// <param name="bluseHashing"></param>
        /// <returns></returns>
        public static string Decrypt(string strtoDecrypt, string strkey, bool bluseHashing)
        {
            if (ConfigurationManager.AppSettings["ASSEMBLY"].ToString().ToUpper() == "MAHINDRA")
            {
                return strtoDecrypt;
            }
            else
            {
                byte[] bytkeyArray;
                byte[] byttoEncryptArray = Convert.FromBase64String(strtoDecrypt);

                if (bluseHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
                }
                else
                    bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = bytkeyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(bytresultArray);
            }
        }

        /// <summary>
        /// Purpose: This function is used to decrypt encrypt string
        /// </summary>
        /// <param name="strtoDecrypt"></param>
        /// <param name="strkey"></param>
        /// <param name="bluseHashing"></param>
        /// <returns></returns>
        public static string DecryptConn(string strtoDecrypt, string strkey, bool bluseHashing)
        {
            byte[] bytkeyArray;
            byte[] byttoEncryptArray = Convert.FromBase64String(strtoDecrypt);

            if (bluseHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
            }
            else
                bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = bytkeyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(bytresultArray);
        }

        /// <summary>
        /// Purpose: This function is used to encrypt string
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string EncryptCompSpecific(string strtoEncrypt, string strkey, bool bluseHashing)
        {
            if (ConfigurationManager.AppSettings["COMPANY"].ToString().ToUpper() == "MAHINDRA")
            {
                return strtoEncrypt;
            }
            else
            {
                byte[] bytkeyArray;
                byte[] byttoEncryptArray = UTF8Encoding.UTF8.GetBytes(strtoEncrypt);

                if (bluseHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
                }
                else
                    bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = bytkeyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

                return Convert.ToBase64String(bytresultArray, 0, bytresultArray.Length);
            }
        }

        /// <summary>
        /// Purpose: This function is used to decrypt encrypt string
        /// </summary>
        /// <param name="strtoDecrypt"></param>
        /// <param name="strkey"></param>
        /// <param name="bluseHashing"></param>
        /// <returns></returns>
        public static string DecryptCompSpecific(string strtoEncrypt, string strkey, bool bluseHashing)
        {
            if (ConfigurationManager.AppSettings["COMPANY"].ToString().ToUpper() == "MAHINDRA")
            {
                return strtoEncrypt;
            }
            else
            {

                byte[] bytkeyArray;
                byte[] byttoEncryptArray = Convert.FromBase64String(strtoEncrypt);

                if (bluseHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    bytkeyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strkey));
                }
                else
                    bytkeyArray = UTF8Encoding.UTF8.GetBytes(strkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = bytkeyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] bytresultArray = cTransform.TransformFinalBlock(byttoEncryptArray, 0, byttoEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(bytresultArray);
            }
        }
        #endregion encription or decryption
    }
}