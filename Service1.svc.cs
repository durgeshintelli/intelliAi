using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using Catalog;
using Catalog.WebArch;
using System.IO;
using System.Net.Mail;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.ServiceModel.Web;
using BusinessProcess.Datalayer;
using BusinessProcess;
using Catalog.Dataset;
using System.Web.Script.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using ApiAiSDK.Model;
using ApiAiSDK;
using NUnit.Framework;
using Newtonsoft.Json;


namespace MusaService
{
    public class Service1 : IService1
    {
        private const string ACCESS_TOKEN = "a7487a128bf54f599a084c067cdfd5ce";

        # region common parameter & Consructor
        MainCommon maincls;
        static Common c;
        Service1()
        {
            maincls = new MainCommon();
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(decimal value)
        {
            int mag = (int)Math.Log(Convert.ToDouble(value), 1024);
            decimal adjustedSize = (decimal)value / (1 << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        #endregion

        # region Data Value

        /// <summary>
        /// Date details for figure search
        /// </summary>
        /// <param name="index"></param>
        /// <param name="countryid"></param>
        /// <returns></returns>
        public List<GetDate> GetDateValue(int index, string countryid,int flag)
        {
            List<GetDate> objDate = new List<GetDate>();
            try
            {
                var cid_en = DecryptAes(countryid);

                int cid = Convert.ToInt32(cid_en);

                DataTable dt;
                CommanParametrs cps = new CommanParametrs();
                if (flag == 1)
                {
                    cps.Add(new CommanParameter("@index", index, DbType.Int32));
                    cps.Add(new CommanParameter("@CountryID", cid, DbType.Int32));
                    dt = maincls.DbCon.GetProcTable("Proc_GetModelDateMobile", cps, "tbl_categoryMapping");
                }
                else
                {
                    cps.Add(new CommanParameter("@index", index, DbType.Int32));
                    cps.Add(new CommanParameter("@CountryID", cid, DbType.Int32));
                    dt = maincls.DbCon.GetProcTable("Proc_GetModelDate", cps, "tbl_categoryMapping");
                }
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        GetDate gd = new GetDate();
                        gd.Date = Convert.ToString(dr["SYEAR"]);
                        objDate.Add(gd);
                        //date = Convert.ToString(dt.Rows[0][0]);
                    }
                    return objDate.ToList();
                }
                else
                {
                    return objDate.ToList();
                }

            }
            catch (Exception ex)
            {
                return objDate.ToList();
            }
        }
        # endregion

        # region View
        public List<Authentication> GetAndroidImeiAvailability(string imei)
        {
            string imei_en = DecryptAes(imei);
            List<Authentication> imeiList = new List<Authentication>();
            try
            {


                CommanParameter cp;
                CommanParametrs cps = new CommanParametrs();

                //Assign parameter value
                cp = new CommanParameter("@IMEI", imei_en, DbType.String);
                cps.Add(cp);
                DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETMOBILEIMEINO", cps, "GetImeiAvailability");
                foreach (DataRow dr in dtrequest.Rows)
                {
                    Authentication auth = new Authentication();
                    auth.ImeiCount = Convert.ToInt32(dr["TOTAL"].ToString());
                    auth.userid = Convert.ToInt32(dr["USERID"].ToString());
                    auth.Active = Convert.ToInt32(dr["ACTIVE"].ToString());
                    auth.AdminActive = Convert.ToInt32(dr["ADMINACTIVE"].ToString());
                    auth.enddate = Convert.ToString(dr["ENDDATE"]) == "" ? Convert.ToString(DateTime.Now) : Convert.ToString(dr["ENDDATE"]);
                    imeiList.Add(auth);
                }
                return imeiList.ToList();
            }
            catch (Exception ex)
            {
                return imeiList.ToList();
            }
        }

        public List<Authentication> GetImeiAvailability(string imei)
        {
            string imei_en = DecryptAes(imei);
            List<Authentication> imeiList = new List<Authentication>();
            CommanParameter cp;
            CommanParametrs cps = new CommanParametrs();

            //Assign parameter value
            cp = new CommanParameter("@IMEI", imei_en, DbType.String);
            cps.Add(cp);
            // DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETMOBILEIMEINO", cps, "GetImeiAvailability");
            DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETIPHONEMOBILEIMEINO", cps, "GetImeiAvailability");// for iphone
            foreach (DataRow dr in dtrequest.Rows)
            {
                Authentication auth = new Authentication();
                auth.ImeiCount = Convert.ToInt32(dr["TOTAL"].ToString());
                auth.userid = Convert.ToInt32(dr["USERID"].ToString());
                auth.Active = Convert.ToInt32(dr["ACTIVE"].ToString());
                auth.AdminActive = Convert.ToInt32(dr["ADMINACTIVE"].ToString());
                auth.enddate = Convert.ToString(dr["ENDDATE"]) == "" ? Convert.ToString(DateTime.Now) : Convert.ToString(dr["ENDDATE"]);
                imeiList.Add(auth);
            }
            return imeiList.ToList();
        }

        public List<DealerAvailable> GetAndroidDealerAvailability(string DEALERCODE)
        {
            string dealercode_en = DecryptAes(DEALERCODE);

            List<DealerAvailable> imeiList = new List<DealerAvailable>();
            CommanParameter cp;
            CommanParametrs cps = new CommanParametrs();
            Int32 cc = 0;
            //Assign parameter value
            cp = new CommanParameter("@DEALERCODE", dealercode_en, DbType.String);
            cps.Add(cp);
            DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETMOBDEALERINFO", cps, "GetDealerAvailable");
            //DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETIPHONEMOBDEALERINFO", cps, "GetDealerAvailable");
            if (dtrequest.Rows.Count == 0)
            {
                DealerAvailable auth = new DealerAvailable();
                auth.status = false;
                imeiList.Add(auth);
            }
            foreach (DataRow dr in dtrequest.Rows)
            {
                DealerAvailable auth = new DealerAvailable();
                auth.maxpermit = dr["MAXPERMIT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAXPERMIT"]);
                string imei = dr["IMEI"] == DBNull.Value ? "" : Convert.ToString(dr["IMEI"]);
                auth.ActiveCount = dr["ACTIVECOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACTIVECOUNT"]);
                auth.status = true;
                if (imei == "")
                {

                }
                else
                {
                    //auth.ImeiTotal = dtrequest.Rows.Count;
                    cc += 1;
                    auth.ImeiTotal = cc;
                }
                imeiList.Add(auth);

            }
            return imeiList;
        }

        public List<DealerAvailable> GetDealerAvailability(string DEALERCODE)
        {
            string dealercode_en = DecryptAes(DEALERCODE);

            List<DealerAvailable> imeiList = new List<DealerAvailable>();
            CommanParameter cp;
            CommanParametrs cps = new CommanParametrs();
            Int32 cc = 0;
            //Assign parameter value
            cp = new CommanParameter("@DEALERCODE", dealercode_en, DbType.String);
            cps.Add(cp);
            //DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETMOBDEALERINFO", cps, "GetDealerAvailable");
            DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_GETIPHONEMOBDEALERINFO", cps, "GetDealerAvailable");
            if (dtrequest.Rows.Count == 0)
            {
                DealerAvailable auth = new DealerAvailable();
                auth.status = false;
                imeiList.Add(auth);
            }
            foreach (DataRow dr in dtrequest.Rows)
            {
                DealerAvailable auth = new DealerAvailable();
                auth.maxpermit = dr["MAXPERMIT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAXPERMIT"]);
                string imei = dr["IMEI"] == DBNull.Value ? "" : Convert.ToString(dr["IMEI"]);
                auth.ActiveCount = dr["ACTIVECOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACTIVECOUNT"]);
                auth.status = true;
                if (imei == "")
                {

                }
                else
                {
                    //auth.ImeiTotal = dtrequest.Rows.Count;
                    cc += 1;
                    auth.ImeiTotal = cc;
                }
                imeiList.Add(auth);

            }
            return imeiList;
        }

        public List<activecolumninfo> GetOemAuthorization(string DEALERCODE)
        {
            string dealercode_en = DecryptAes(DEALERCODE);

            List<activecolumninfo> imeiList = new List<activecolumninfo>();
            CommanParameter cp;
            CommanParametrs cps = new CommanParametrs();

            //Assign parameter value
            cp = new CommanParameter("@DEALERCODE", dealercode_en, DbType.String);
            cps.Add(cp);
            DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_OEMUSERAUTENTICATION", cps, "GetOemAuthorization");
            foreach (DataRow dr in dtrequest.Rows)
            {
                activecolumninfo auth = new activecolumninfo();
                auth.status = true;

                imeiList.Add(auth);
            }
            return imeiList;
        }

        public string MessageVerify()
        {
            string result = string.Empty;
            //IllustrationPartList(15, 1982, 4848, "19-Jan-2015", 0, 1,3);
            //AlternatePartList("Part", "11192", "", "2", "8751", 1, 1, 3, "", "");
            //AddOrderIllustrationData("Part", "108705", "", "2", 1, 1, 3, "", "");
            //GetShipInfoDetails("700078");
            // SavePdf("701380", 14, 4);
            //getDiscountConfig(3, 3804, 14, 1, "USD");
            //decimal tt = 192.5M;
            //getDiscountConfig(1, tt, 14, 1, "USD");
            //Addpart(1, "000016632P04", 1, "0", 0);
            //addreview("Part", "1859", "", "1", 1, 1, 3, "", "");
            //ModuleRight(1);
            //GetShipInfoDetailsshipwise("700228");
            //DistributorId(51,2);
            //partList(14, "14-May-2015", -1, -1, "P836998", "", 1, 1, 1);
            // GetShipInfoDetailsshipwise("700298");
            // ModuleRight(10);
            //DiscountConfig(70);
            //SaveOrderType("PO_NO:1212", 4, "11-Jun-2015", "1111",1, 111, 1, 0, 0, 7, 0, "test", 3, "lon09999", 1,70,18);
            //OrderReports("701280", 1286);
            //Addpart(1, "af505055g", 1, "", 1);
            //UpdatePart(1, "af505055g", 1, "", 1,3);
            // ChangePassword(9, "12345", "123456", "123456");
            //GetModelSearchData(1, "45", 9, 1, 1, 1);

            //GetAggrigateData(1854, 1);
            // PartPrice(1, "000016632P04", 1, "0", 0,14);
            // PartPriceAll(1, "01104M06126", 1, "0", 0, 2);
            //IllustrationPartList(1030, 1432, 2863, "30-Jul-2015", 0, 1, 1);
            //partList(1030, "30-Jul-2015", 767, 3372, "", "", 1, 1, 1);
            //AccidentalAssemblyOrder(2710, 1432, 1, 1, 1030);
            //Addpart(1, "0310DAV00010N", 1, "", 0);
            //AddOrderIllustrationData("Part", "99102", "", "2", 1, 1, 1, "", "");
            //VinSearch(1030, "F6A47729", "BA14ZM0191", 1);
            // VinSearchAggregate(1432, 1);
            //VinSearchAssembly(1454, "03-Aug-2015", 1432, 759, 1);
            //partList(1030, "30-Jul-2015", -1, -1, "", "bolt", 1, 1, 1);
            //partApplicabilityList(1030, "6301AAB00031N", 1);
            // ModuleRight(1);
            // GetCheckoutDescription(1);

            //List<OrderDetailProp> pdata = new List<OrderDetailProp>();
            //OrderDetailProp op = new OrderDetailProp();
            //op.ID = 0;
            //op.PartId = 21;
            //op.Price = Convert.ToDecimal(32);
            //op.Qty = 4;
            //op.Amount = Convert.ToDecimal(128);
            //op.TaxAmount = 0;
            //pdata.Add(op);
            //SaveOrderTable(0, 0, 1, "Ramesh", "Bhagat", "Ramesh Motors co.", "Street no.7", "ADD2", "ADD3", "756765767", "1", "1", "1", "122015", "info@intellinetsystem.com", 0, 3, "HOUSTON", 0, "", "CustAttrbutes=PO_No:", 1, "17-May-2016", 1, 0, 0, 0, 0, 2, 0, "test11", 0, "INT001", 1, 1, 0, pdata);
            ////GetOemAuthorization("HONDA2015");
            //GetDealerAvailability("supriya1");
            //GetAndroidDealerAvailability("MFS01");
            //GetOemAuthorization("HONDA2015");
            //partList(2, "03-Dec-2015", -1, -1, "", "Bolt", 1, 1, 1);
            //FillAccessoriChild(241, 1, 1);
            //AddOrderIllustrationData("Part", "12693", "", "1", 1, 1, 1, "", "");
            //AccidentalAssemblyOrder(941, 52, 1, 1, 2);
            //GetImeiAvailability("6C8C88B0-A9ED-4CF6-B1EC-F259927735D3");
            //AddOrderIllustrationData("Part", "438", "", "2", 1, 1, 1, "", "");
            //partApplicabilityList(2, "000013", 1);
            //VinSearch(2, "N8MA00450", "", 1);
            //LoadCategoryImage(396);

            // Mahind & Mahindra Test

            // ModuleRight(6);   
            //IllustrationPartList(13761, 3007, 6488, "29-Feb-2016", 0, 1, 0); 
            // AddOrderIllustrationData("Part", "26009", "", "1", 1, 1, 0, "", "");
            //partList(13761, "29-Feb-2016", -1, -1, "", "Bolt", 1, 1, 1);
            //string ss="501,69,498,30,496,28,491,23,485,18,475,12,465,5,452,-1,438,-7,421,-12,403,-17,380,-22,357,-25,333,-26,307,-26,284,-24,256,-18,232,-13,204,-4,180,6,157,16,133,31,110,49,88,67,68,84,49,104,32,124,19,145,7,167,-1,187,-6,210,-9,231,-10,252,-9,275,-7,295,-1,318,5,339,13,357,22,375,34,396,47,414,59,431,75,447,90,463,107,477,125,493,144,503,167,513,188,522,211,528,234,530,260,531,286,527,314,521,342,511,364,507,390,493,413,481,435,467,456,452,475,436,492,418,507,401,521,383,533,365,544,343,554,321,564,300,571,280,578,257,582,236,586,216,587,195,588,178,585,157,581,135,575,114,565,94,555,74,537,56,488,26,463,17,463,17";
            //AccidentalViewAssembly(ss, 144, true, 1, "-1","", "3007", "29-Feb-2016", 1);
            // AccidentalAssemblyOrder(101, 2, 1, 1, 13761);
            // partApplicabilityList(13761, "0103AP0", 1);
            return result;
        }

        public List<ValidateUser> ValidateUser(string sUserName, string sPassword)
        {
            
            //DeviceLogout("WzBhQlqbjbd+xWMa7TwJzA==\n", "5zZwAPLGH+koedlKTsqRvA==\n");
            //OrderMasterListNew(4, 1, 1, "16-Jun-2012", "26-Jun-2016");
            // partList(14, "16-Jun-2016", 1822, -1, "", "", 1, 1, 3);
            // VinSearch(3, "12345075869", "", 1);
            //fillDistAddress("Mk00oWwVGWmqIt5tdyF1mg==\n");
            // DeviceInformation("6.0", "motorola", "titan_umtsds", "motorola", "XT1068", "", "359296059349747", "5", "QUiMMIngSBWiTippOkS4kw==\n", "3IajHpxqntrEIvKAsLKI8w==\n", "manoj", "sain", "eO_oor8nJ1c:APA91bGqL5gEjH3AobnaPj36kp9yoJceeBeUSq8YpZ9cW6VeF16k-EtALOLYe9iwk7-reBVCXsLj8RbkXwH-ApgJEDy2-HFVhI2WUfNUlkNFg5Cc5egyJjdzhaekw6ukbOTZdXtXOClE", "2.3");
            //AlternatePartList("Part", "e2U6j07DT9bmyWCZW6MSew==\n", "", "aNQbioBOdrQewtbn5Zfzcw==\n", "0", "XFosZQvhjoOPaR5Gkr7Dsg==\n", 1, "XFosZQvhjoOPaR5Gkr7Dsg==\n", "", "");
            //List<Partinfo> Plist=new List<Partinfo> ();
            //Partinfo pi=new Partinfo ();
            //pi.PartNo="007208042C1";
            //Plist.Add(pi);
            //Partinfo pii=new Partinfo ();
            //pii.PartNo="000013900P04";
            //Plist.Add(pii);
            //UpdatePart("XFosZQvhjoOPaR5Gkr7Dsg==\n", Plist, "XFosZQvhjoOPaR5Gkr7Dsg==\n", "", 0, "XFosZQvhjoOPaR5Gkr7Dsg==\n");
            //GetAndroidDealerAvailability("3IajHpxqntrEIvKAsLKI8w==\n");
            //GetAndroidImeiAvailability("Ndk2fB3GEj3ztHqNDqWiTg==\n");
            //DeviceDeActivate("Ndk2fB3GEj3ztHqNDqWiTg==\n");
            //partApplicabilityList("qTU8qV8M6GVwjC3bdz574Q==\n", "DsN7S15oBG\/2a\/XkPgZmMw==\n", 1);
            // IllustrationPartList("dyWMm5RF6z3xZhQ83y6ZTg==\n", 2313, 34127, "1996", 0, 1, "XFosZQvhjoOPaR5Gkr7Dsg==\n"); 
           // ModuleRight("XFosZQvhjoOPaR5Gkr7Dsg==\n");
            //LoadCategoryImage(616);
           // partList("WzBhQlqbjbd+xWMa7TwJzA==\n", "2014", -1, -1, "TKET3ilunrbfkjfwNpqWTg==\n", "", "XFosZQvhjoOPaR5Gkr7Dsg==\n", 2, "XFosZQvhjoOPaR5Gkr7Dsg==\n");
            //AutoCompletePartNo("WzBhQlqbjbd+xWMa7TwJzA==\n", 1, "2016", "8040", "-1", "-1", "XFosZQvhjoOPaR5Gkr7Dsg==\n");
            //GetDateValue(947, "XFosZQvhjoOPaR5Gkr7Dsg==\n", 1);

            //partApplicabilityList("dyWMm5RF6z3xZhQ83y6ZTg==\n", "4+I+oG0owta12a8qI3xK7w==\n", 1);
           // GetAndroidDealerAvailability("Seg2noPHeViYEzdvqXQSxA==\n");
           // CategoryFillType(true, "WzBhQlqbjbd+xWMa7TwJzA==\n", 1, "XFosZQvhjoOPaR5Gkr7Dsg==\n");
            //LoadCategoryTypeImage(3);
            string name = DecryptAes("Mk00oWwVGWmqIt5tdyF1mg==\n", "tR7nR6wZHGjYMCuV");
           


            string username_en = DecryptAes(sUserName);
            string password_en = DecryptAes(sPassword);

            List<ValidateUser> objvalidate = new List<ValidateUser>();
            string password = Global.EncryptCompSpecific(password_en, "ABF482", true);
            try
            {
                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();

                cps.Add(new Catalog.WebArch.CommanParameter("@username", username_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@password", password, System.Data.DbType.String));
                string sql = @"select id from tbl_user where upper(loginId)=upper(@username) and Password=@password and Inactive=0";
                DataTable dt = maincls.DbCon.GetTable(sql, cps, "tbl_user");
                foreach (DataRow dr in dt.Rows)
                {
                    ValidateUser lnf = new ValidateUser();
                    lnf.ID = Convert.ToInt32(dr["ID"]);
                    objvalidate.Add(lnf);
                }
                if (dt.Rows.Count == 0)
                {
                    ValidateUser lnf = new ValidateUser();
                    lnf.ID = 0;
                    objvalidate.Add(lnf);
                }
                return objvalidate.ToList();



            }
            catch (Exception ex)
            {
                throw new Exception("validateUser", ex);
            }
        }


        public string EncryptFormat(string message)
        {
            string result = Global.EncryptCompSpecific(message, "ABF482", true);
            return result;
        }

        public List<UserInfo> UserDetails(string ID)
        {
            var id_en = DecryptAes(ID);
            Int32 ids = Convert.ToInt32(id_en);

            string code = string.Empty;
            List<UserInfo> pUser = new List<UserInfo>();
            try
            {
                UserData objUser = new UserData();
                DataTable dtUser = objUser.GetSelectedUser(ids);

                Int32 feedback_no = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select ISNULL(FeedbackNo,1)FeedbackNo from tbl_setting"));
                string admin_email = Convert.ToString(maincls.DbCon.ExecuteScaler("select FeedbackEmailID from tbl_setting"));
                code = Convert.ToString(maincls.DbCon.ExecuteScaler("select code from tbl_Distributor where UserID=" + ids));
                if (code == "")
                    code = Convert.ToString(maincls.DbCon.ExecuteScaler("select CAdditional1 from tbl_Company"));
                string countryname = Convert.ToString(maincls.DbCon.ExecuteScaler("select CountryName from tbl_Country where ID=" + dtUser.Rows[0]["COUNTRYID"]));
                string currency = Convert.ToString(maincls.DbCon.ExecuteScaler("select c.CurrencySymbol from tbl_Currency c inner join tbl_Country ct on ct.CurrencyID=c.ID where ct.ID=" + dtUser.Rows[0]["COUNTRYID"]));
                Int32 currencyid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select c.ID from tbl_Currency c inner join tbl_Country ct on ct.CurrencyID=c.ID where ct.ID=" + dtUser.Rows[0]["COUNTRYID"]));
                foreach (DataRow dr in dtUser.Rows)
                {
                    UserInfo us = new UserInfo();
                    us.Email = Convert.ToString(dr["EMAIL"]);
                    String user_name = dr["UFNAME"] + " " + dr["ULNAME"];
                    us.Name = user_name;
                    us.AdminEmail = admin_email;
                    us.FeedBackNo = Convert.ToString(feedback_no.ToString());
                    us.RoleId = Convert.ToInt32(dr["ROLEID"]);
                    us.CountryId = Convert.ToInt32(dr["COUNTRYID"]);
                    us.Country = countryname;
                    us.Currency = currency;
                    us.CurrencyId = currencyid == 0 ? 0 : currencyid;
                    us.Code = code;
                    us.UserType = Convert.ToInt32(dr["USERTYPE"]);
                    pUser.Add(us);
                }

                if (dtUser.Rows.Count > 0)
                {
                    UserInfo ua = new UserInfo();
                    ua.Address = dtUser.Rows[0][0] == DBNull.Value ? "" : Convert.ToString(dtUser.Rows[0][0]);
                    ua.AdminEmail = dtUser.Rows[0][1] == DBNull.Value ? "" : Convert.ToString(dtUser.Rows[0][1]);
                    ua.Country = dtUser.Rows[0][2] == DBNull.Value ? "" : Convert.ToString(dtUser.Rows[0][2]);
                    ua.FeedBackNo = dtUser.Rows[0][3] == DBNull.Value ? "" : Convert.ToString(dtUser.Rows[0][3]);
                    ua.CurrencyId = dtUser.Rows[0][4] == DBNull.Value ? 0 : Convert.ToInt32(dtUser.Rows[0][4]);

                }

                return pUser;
            }
            catch (Exception ex)
            {
                //throw new Exception("VarifyUser", ex);
                return pUser;
            }
        }

        public List<RightInfo> ModuleRight(string RoleId)
        {
            List<RightInfo> rightList = new List<RightInfo>();
            try
            {
                var roleid_en = DecryptAes(RoleId);
                Int32 roleids = Convert.ToInt32(roleid_en);

                Int32 rightid = 460;
                DataTable dtRights1 = new DataTable();
                DataTable dtRights = maincls.DbCon.GetTable("select r.rightname,rr.hasright from tbl_right r inner join tbl_roleright rr on r.id=rr.rightid where r.parentrightid=" + rightid + " and rr.roleid=1 and r.id not in (537,538)  order by r.id desc", "temp");//and hasright=1
                int iid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where roleid=" + roleids));
                if (iid == 1 || iid == -1)
                {
                    DataSet ds = new DataSet();
                    DataTable dt;
                    DataRow dr;
                    DataColumn idCoulumn;
                    DataColumn nameCoulumn;
                    int i = 0;

                    dt = new DataTable();
                    idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Boolean"));//MUSA,Arctic Cat
                   // idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Decimal"));//MFS, Mahindra & Mahindra
                    nameCoulumn = new DataColumn("RIGHTNAME", Type.GetType("System.String"));

                    dt.Columns.Add(idCoulumn);
                    dt.Columns.Add(nameCoulumn);

                    dr = dt.NewRow();
                    dr["HASRIGHT"] = 1;
                    dr["RIGHTNAME"] = "Checkout";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["HASRIGHT"] = 1;
                    dr["RIGHTNAME"] = "Order Cart";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["HASRIGHT"] = 0;
                    dr["RIGHTNAME"] = "Parts Return";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["HASRIGHT"] = 1;
                    dr["RIGHTNAME"] = "Show Price";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["HASRIGHT"] = 0;
                    dr["RIGHTNAME"] = "Manage Orders";
                    dt.Rows.Add(dr);

                    ds.Tables.Add(dt);
                    dtRights1 = dt.Copy();
                    //dtRights1 = maincls.DbCon.GetTable("select rightname,CAST(1 as bit) as hasright from tbl_right where parentrightid=523", "temp");
                }
                else if (iid == 3)
                {
                    DataSet ds = new DataSet();
                    DataTable dt;
                    DataRow dr;
                    DataColumn idCoulumn;
                    DataColumn nameCoulumn;
                    int i = 0;

                    dt = new DataTable();
                    idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Boolean"));//MUSA, Arctic cat
                    //idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Decimal"));//MFS and MAhindra & Mahindra
                    nameCoulumn = new DataColumn("RIGHTNAME", Type.GetType("System.String"));

                    dt.Columns.Add(idCoulumn);
                    dt.Columns.Add(nameCoulumn);

                    dr = dt.NewRow();
                    dr["RIGHTNAME"] = "Checkout";
                    dr["HASRIGHT"] = 1;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["RIGHTNAME"] = "Order Cart";
                    dr["HASRIGHT"] = 1;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["RIGHTNAME"] = "Parts Return";
                    dr["HASRIGHT"] = 1;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["RIGHTNAME"] = "Show Price";
                    dr["HASRIGHT"] = 1;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["RIGHTNAME"] = "Manage Orders";
                    dr["HASRIGHT"] = 1;
                    dt.Rows.Add(dr);

                    ds.Tables.Add(dt);
                    dtRights1 = dt.Copy();
                    //dtRights1 = maincls.DbCon.GetTable("select rightname,CAST(1 as bit) as hasright from tbl_right where parentrightid=523", "temp");
                }
                //else if (iid == 0)// created for Mahindra & Mahindra
                //{
                //    DataSet ds = new DataSet();
                //    DataTable dt;
                //    DataRow dr;
                //    DataColumn idCoulumn;
                //    DataColumn nameCoulumn;
                //    int i = 0;

               //    dt = new DataTable();
                //   // idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Boolean"));//MUSA
                //     idCoulumn = new DataColumn("HASRIGHT", Type.GetType("System.Decimal"));//MFS and MAhindra & Mahindra
                //    nameCoulumn = new DataColumn("RIGHTNAME", Type.GetType("System.String"));

               //    dt.Columns.Add(idCoulumn);
                //    dt.Columns.Add(nameCoulumn);

               //    dr = dt.NewRow();
                //    dr["HASRIGHT"] = 0;
                //    dr["RIGHTNAME"] = "Checkout";
                //    dt.Rows.Add(dr);

               //    dr = dt.NewRow();
                //    dr["HASRIGHT"] = 1;
                //    dr["RIGHTNAME"] = "Order Cart";
                //    dt.Rows.Add(dr);

               //    dr = dt.NewRow();
                //    dr["HASRIGHT"] = 0;
                //    dr["RIGHTNAME"] = "Parts Return";
                //    dt.Rows.Add(dr);

               //    dr = dt.NewRow();
                //    dr["HASRIGHT"] = 0;
                //    dr["RIGHTNAME"] = "Show Price";
                //    dt.Rows.Add(dr);

               //    dr = dt.NewRow();
                //    dr["HASRIGHT"] = 0;
                //    dr["RIGHTNAME"] = "Manage Orders";
                //    dt.Rows.Add(dr);

               //    ds.Tables.Add(dt);
                //    dtRights1 = dt.Copy();

               //}
                else
                {
                    dtRights1 = maincls.DbCon.GetTable("select r.rightname,rr.hasright from tbl_right r inner join tbl_roleright rr on r.id=rr.rightid where r.parentrightid=523 and rr.roleid=" + roleids + " order by r.id desc", "temp");

                    if (dtRights1.Rows.Count == 0)
                    {
                        dtRights1 = maincls.DbCon.GetTable("select distinct r.rightname,rr.hasright from tbl_right r inner join tbl_roleright rr on r.id=rr.rightid where r.parentrightid=523 and rr.hasright=1 ", "temp");//and rr.hasright=1
                    }
                }
                DataTable dtRights2 = maincls.DbCon.GetTable("select r.rightname,rr.hasright from tbl_right r inner join tbl_roleright rr on r.id=rr.rightid where r.parentrightid=" + rightid + " and rr.roleid=1 and r.id in (537,538)  order by r.id asc", "temp");//and hasright=1

                DataTable dtall = dtRights.Copy();
                dtall.Merge(dtRights1);
                dtall.Merge(dtRights2);
                //foreach (DataRow dr in dtall.Rows)
                //{
                //    RightInfo rinfo = new RightInfo();
                //    rinfo.RightName = Convert.ToString(dr["rightname"]).Trim();
                //    rinfo.Rights = Convert.ToInt32(dr["hasright"]);

                //    rightList.Add(rinfo);
                //}
                //return rightList.ToList();
                foreach (DataRow dr in dtall.Rows)
                {

                    RightInfo rinfo = new RightInfo();
                    rinfo.RightName = Convert.ToString(dr["rightname"]).Trim();
                    rinfo.Rights = Convert.ToInt32(dr["hasright"]);
                    rightList.Add(rinfo);//MUSA Only without if condition

                    //if (rinfo.RightName == "Accessories Module" || rinfo.RightName == "Accidental Repair Feature" || rinfo.RightName == "Checkout" || rinfo.RightName == "Parts Return" || rinfo.RightName == "Manage Orders")//NOVA
                    //{
                    //    if (rinfo.RightName == "Accessories Module")
                    //    {
                    //        rinfo.RightName = "Accessories Module";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else if (rinfo.RightName == "Accidental Repair Feature")
                    //    {
                    //        rinfo.RightName = "Accidental Repair Feature";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else if (rinfo.RightName == "Checkout")
                    //    {
                    //        rinfo.RightName = "Checkout";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else if (rinfo.RightName == "Parts Return")
                    //    {
                    //        rinfo.RightName = "Parts Return";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else if (rinfo.RightName == "Manage Orders")
                    //    {
                    //        rinfo.RightName = "Manage Orders";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else { }
                    //}
                    //else
                    //{
                    //    rightList.Add(rinfo);//MUSA Only without if condition
                    //}
                    //if (rinfo.RightName == "Accidental Repair Feature" || rinfo.RightName == "Parts Return")// Arctic Cat
                    //{
                    //    if (rinfo.RightName == "Accidental Repair Feature")
                    //    {
                    //        rinfo.RightName = "Accidental Repair Feature";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }

                    //    else if (rinfo.RightName == "Parts Return")
                    //    {
                    //        rinfo.RightName = "Parts Return";
                    //        rinfo.Rights = 0;
                    //        rightList.Add(rinfo);
                    //    }
                    //    else { }
                    //}
                    //else
                    //{
                    //    rightList.Add(rinfo);//MUSA Only without if condition
                    //}
                }
                return rightList.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("ModuleRight", ex);
            }
        }

        public List<UserInfo> DistributorId(string id, string usertype)
        {
            List<UserInfo> pUser = new List<UserInfo>();
            try
            {
                var id_en = DecryptAes(id);
                Int32 ids = Convert.ToInt32(id_en);

                var usertype_en = DecryptAes(usertype);
                Int32 usertypes = Convert.ToInt32(usertype_en);

                Catalog.WebArch.CommanParametrs cps1 = new Catalog.WebArch.CommanParametrs();
                cps1.Add(new Catalog.WebArch.CommanParameter("@UserID", ids, System.Data.DbType.Int32));
                if (usertypes == 2)
                {
                    int idd1 = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select createdby from tbl_user where Id=@UserID", cps1));
                    ids = idd1;
                }

                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                cps.Add(new Catalog.WebArch.CommanParameter("@UserID", ids, System.Data.DbType.Int32));
                int idd = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select id from tbl_distributor where UserId=@UserID", cps));
                string code = Convert.ToString(maincls.DbCon.ExecuteScaler("select Code from tbl_distributor where UserId=@UserID", cps));
                UserInfo ui = new UserInfo();
                ui.Code = code;
                ui.RoleId = idd;
                pUser.Add(ui);

                return pUser.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("validateUser", ex);
            }
        }

        public List<LanguageInfo> LanguageList()
        {
            List<LanguageInfo> plist = new List<LanguageInfo>();
            LanguageData lngData = new LanguageData();
            //Creating datatable object and storing Language records into datatable object.
            DataTable dtlng = lngData.LanguageList();
            foreach (DataRow dr in dtlng.Rows)
            {
                LanguageInfo lnf = new LanguageInfo();
                string[] str = Convert.ToString(dr["LanguageCode"]).Split(' ');
                lnf.ID = Convert.ToInt32(str[0]);
                lnf.LanguageCode = Convert.ToString(str[1]);
                lnf.LanguageName = Convert.ToString(dr["Language"]);
                plist.Add(lnf);
            }
            return plist;

        }

        public List<ValidateUser> DeviceInformation(string release, string brand, string device, string manufacturer, string model, string mobileno, string imei, string size, string userid, string actcode, string fname, string lname, string gcmid, string AppVersion)
        {
            List<ValidateUser> objvalidate = new List<ValidateUser>();
            int isvalid = 0;
            string actcode_en = "";
            string mobile_en = "";

            try
            {
                string userid_en = DecryptAes(userid);
                if (actcode != "")
                    actcode_en = DecryptAes(actcode);

                if (mobileno != "")
                    mobile_en = DecryptAes(mobileno);


                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                cps.Add(new Catalog.WebArch.CommanParameter("@ID", DBNull.Value, System.Data.DbType.UInt32));
                cps.Add(new Catalog.WebArch.CommanParameter("@Release", release, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Brand", brand, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Device", device, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Manufacturer", manufacturer, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Model", model, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@MobileNo", mobile_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@IMEI", imei, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@ScreenSize", size, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Userid", userid_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@firstname", fname, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@LastName", lname, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@ActivationCode", actcode_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@GcmId", gcmid, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@AppVersion", AppVersion, System.Data.DbType.String));
                isvalid = maincls.DbCon.ExecuteProcNonQuery("PROC_DEVICEINFOSAVE", cps);
                ValidateUser valuser = new ValidateUser();
                valuser.ID = isvalid;

                objvalidate.Add(valuser);
                return objvalidate.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DeviceInformation", ex);
            }
        }

        public List<ValidateUser> DeviceLogout(string userid, string imei)
        {
            List<ValidateUser> objvalidate = new List<ValidateUser>();
            int isvalid = 0;
            try
            {
                string userid_en = DecryptAes(userid);
                string imei_en = DecryptAes(imei);

                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                cps.Add(new Catalog.WebArch.CommanParameter("@Userid", userid_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@imei", imei_en, System.Data.DbType.String));
                isvalid = maincls.DbCon.ExecuteProcNonQuery("PROC_DEVICELOGPERIODSAVE", cps);

                ValidateUser valuser = new ValidateUser();
                valuser.ID = isvalid;

                objvalidate.Add(valuser);
                return objvalidate.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DeviceLogout", ex);
            }
        }

        public List<ValidateUser> DeviceDeActivate(string imei)
        {
            List<ValidateUser> objvalidate = new List<ValidateUser>();
            int isvalid = 0;
            try
            {
                string imei_en = DecryptAes(imei);

                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                cps.Add(new Catalog.WebArch.CommanParameter("@IMEI", imei_en, System.Data.DbType.String));
                isvalid = maincls.DbCon.ExecuteProcNonQuery("PROC_DEVICEDEACTIVATE", cps);

                ValidateUser valuser = new ValidateUser();
                valuser.ID = isvalid;

                objvalidate.Add(valuser);
                return objvalidate.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("DeviceDeActivate", ex);
            }
        }

        public List<TestInfo> testdata(string aa)
        {
            List<TestInfo> obj = new List<TestInfo>();
            TestInfo tt = new TestInfo();
            tt.No = aa;
            obj.Add(tt);
            return obj.ToList();

        }

        public List<ValidateUser> ImageQuery(byte[] byteImage, string description, string userid)
        {
            List<ValidateUser> objvalidate = new List<ValidateUser>();
            int isvalid = 0;
            try
            {
                string userid_en = DecryptAes(userid);
                string desc_en = DecryptAes(description);

                Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                cps.Add(new Catalog.WebArch.CommanParameter("@Image", byteImage, System.Data.DbType.Binary));
                cps.Add(new Catalog.WebArch.CommanParameter("@Description", desc_en, System.Data.DbType.String));
                cps.Add(new Catalog.WebArch.CommanParameter("@Userid", userid_en, System.Data.DbType.String));
                isvalid = maincls.DbCon.ExecuteProcNonQuery("PROC_IMAGEQUERYSAVE", cps);

                ValidateUser valuser = new ValidateUser();
                valuser.ID = isvalid;

                objvalidate.Add(valuser);
                return objvalidate.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageQuery", ex);
            }
        }

        public List<ValidateUser> ValidateCountry(string UserId)
        {
            var userid_en = DecryptAes(UserId);
            Int32 ids = Convert.ToInt32(userid_en);



            List<ValidateUser> objvalidate = new List<ValidateUser>();
            int CountryId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + ids));
            ValidateUser valuser = new ValidateUser();
            valuser.ID = CountryId;

            objvalidate.Add(valuser);
            return objvalidate.ToList();
        }

        public List<MessageResult> ForgetPassword(string uname, string email)
        {
            string uname_en = DecryptAes(uname);
            string email_en = DecryptAes(email);

            List<MessageResult> obmessage = new List<MessageResult>();
            //Creating Object Of User Property Class
            UserProp uprop = new UserProp();
            // SettingBus SetBus = new SettingBus();
            SettingData setData = new SettingData();
            DataTable dtsetting = setData.List();
            //Assign the Mail ID to Property 
            uprop.Email = email_en.Trim().ToUpper();
            //Assign the User Name to Property 
            uprop.LoginID = uname_en.ToUpper().Trim();
            //Creating Object of Business Class
            //UserBus Ubus = new UserBus();
            UserData userData = new UserData();
            //Finding the Enter Mail ID is Valid Or not
            DataTable dtUser = userData.FindUserFromMailID(uprop);
            //Checking the Mail ID is Correct or not
            //Mail ID is Valid than Execute
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                //Generating New Password
                string strPassword = System.Guid.NewGuid().ToString().Substring(0, 6);
                if (dtsetting != null)
                {
                    //Checkingthe Setting is there 
                    if (dtsetting.Rows.Count > 0)
                    {
                        //Check that Min And Max Length is Define Or Not
                        if ((!Convert.IsDBNull(dtsetting.Rows[0]["PassMinlength"])) && (!Convert.IsDBNull(dtsetting.Rows[0]["PassMaxlength"])))
                        {
                            if (Convert.ToInt32(dtsetting.Rows[0]["PassMinLength"]) > 0 && (Convert.ToInt32(dtsetting.Rows[0]["PassMaxLength"]) > 0))
                            {
                                int iMinLength = Convert.ToInt32(dtsetting.Rows[0]["PassMinLength"]);
                                strPassword = System.Guid.NewGuid().ToString().Substring(0, iMinLength);
                            }
                        }
                    }
                }
                UserProp userProp = new UserProp();//Creating New Object
                userProp.ID = Convert.ToInt32(dtUser.Rows[0]["ID"]);
                userProp = (UserProp)userData.Get(userProp.ID);
                //Assigning New Password To save
                userProp.Password = Global.Encrypt(strPassword, "ABF482", true);
                //Save the Data
                int iStatus = userData.Update(userProp);
                dtUser = userData.GetSelectedUser(Convert.ToInt32(dtUser.Rows[0]["ID"]));
                if (iStatus > 0)
                {
                    DataTable dtUserPassword = userData.FindMaxRevision(Convert.ToInt32(dtUser.Rows[0]["ID"]));
                    int iRevision = 0;
                    if (dtUserPassword != null)
                    {
                        //If Number of Rows is greater than 0
                        if (dtUserPassword.Rows.Count > 0)
                        {
                            iRevision = Convert.ToInt32(dtUserPassword.Rows[0]["Revision"]);
                            iRevision += 1;//Increase Revision by 1
                        }
                    }
                    if (iRevision == 0) iRevision = 1;
                    userData.SaveintoUserPassword(Convert.ToInt32(dtUser.Rows[0]["ID"]), iRevision, strPassword, Convert.ToInt32(dtUser.Rows[0]["ID"]));
                    // MailingBus mbus = new MailingBus();
                    MailingData mdata = new MailingData();
                    MailingProp mpro = new MailingProp();
                    //User Creation transaction
                    mpro.TransactionType = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FP"]);
                    mpro = (MailingProp)mdata.GetTranMailing(mpro);
                    if (mpro.MailTemplateID > 0)
                    {
                        // MailTemplateBus mtbus = new MailTemplateBus();
                        MailTemplateData mtdata = new MailTemplateData();
                        MailTemplateProp mtprop = new MailTemplateProp();
                        mtprop.ID = mpro.MailTemplateID;
                        Int32 iid = mtprop.ID;
                        mtprop = (MailTemplateProp)mtdata.Get(iid);
                        string strtemp = System.Web.HttpContext.Current.Server.HtmlDecode(mtprop.Template);
                        if (dtUser.Rows.Count > 0)
                        {
                            foreach (DataColumn dc in dtUser.Columns)
                            {
                                if (dc.Caption.ToLower() == "password")
                                {
                                    strtemp = strtemp.Replace("{" + dc.Caption.ToLower() + "}", Global.Decrypt(dtUser.Rows[0][dc].ToString(), "ABF482", true));
                                }
                                else
                                    strtemp = strtemp.Replace("{" + dc.Caption.ToLower() + "}", dtUser.Rows[0][dc].ToString());
                            }
                            mtprop.Template = strtemp;
                            MailClass objMail = new MailClass();
                            objMail.SendMails(mtprop, email.Trim());
                        }
                    }
                }
                MessageResult msg = new MessageResult();
                msg.Message = "Password Has Been Send To Your Email ID";
                obmessage.Add(msg);
                return obmessage.ToList();
            }
            else
            {
                MessageResult msg = new MessageResult();
                msg.Message = "Please Enter Correct User Name And EMail ID";
                obmessage.Add(msg);
                return obmessage.ToList();


            }

        }

        public List<CurrencyInfo> CountryCurrency()
        {
            List<CurrencyInfo> pUser = new List<CurrencyInfo>();
            try
            {
                string sqlquery = @"select cc.currencysymbol from tbl_setting s inner join tbl_country c on c.id=s.DefCountryID
                                inner join tbl_currency cc on c.currencyid=cc.id";
                DataTable dt = maincls.DbCon.GetTable(sqlquery, "tbl_setting");
                foreach (DataRow dr in dt.Rows)
                {
                    CurrencyInfo us = new CurrencyInfo();
                    us.Currency = dr["CURRENCYSYMBOL"].ToString();
                    pUser.Add(us);
                }
                return pUser.ToList();
            }
            catch { throw; }
        }

        public List<activecolumninfo> ActiveColumn(int iType, int lngid)
        {
            List<activecolumninfo> plist = new List<activecolumninfo>();
            ColumnActiveBus colactivebus = new ColumnActiveBus();
            //getting column which is set by admin module
            DataTable dtActiveCols = colactivebus.GetActiveColumn(0, iType, lngid);
            Boolean tt = dtActiveCols.Select("COLUMNNAME like 'PRICE%'").Length > 0;

            activecolumninfo rinfo = new activecolumninfo();
            rinfo.status = tt;
            plist.Add(rinfo);

            return plist;
        }

        public List<CategoryInfo> CategoryFill(string UserID, int CateTypeID, int LngId, string countryid)
        {

            List<CategoryInfo> category = new List<CategoryInfo>();
            try
            {
                var userid_en = DecryptAes(UserID);
                Int32 userids = Convert.ToInt32(userid_en);

                var country_en = DecryptAes(countryid);
                Int32 countrys = Convert.ToInt32(country_en);

                //System.Web.HttpContext.Current.Session["LngId"] = LngId;

                CategoryViewData objcatdata = new CategoryViewData();
                int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));
                if (UserType == 0)
                    UserType = 1;

                //int countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserID));

                DataTable dtCategory = objcatdata.FillCategory(userids, UserType, countrys, Convert.ToString(CateTypeID), LngId, 0);
                //DataTable dtCategory = objcatdata.FillCategory(UserID, UserType, countryid, Convert.ToString(CateTypeID), LngId);

                foreach (DataRow dr in dtCategory.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.CategoryId = Convert.ToInt32(dr["CategoryId"]);
                    catinfo.ImagePath = "ImageHandler.ashx?ID=" + catinfo.Id + "&Type=1&width=75&height=75&isBinary=1";
                    category.Add(catinfo);

                }
                return category.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryFill", ex);
            }
        }

        public List<CategoryInfo> CategoryVariant(int Id, string CountryID, string StartDate, int LngId)
        {

            List<CategoryInfo> category = new List<CategoryInfo>();
            try
            {
                var country_en = DecryptAes(CountryID);
                Int32 countrys = Convert.ToInt32(country_en);

                CategoryViewData objcatdata = new CategoryViewData();
                DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA
                //string check_date = "01-Jan-" + StartDate; // Arctic cat
               // DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

                DataTable dtCategory = objcatdata.FillCategoryCountryModel(Id, countrys, dtSearchDate, LngId, 0);
                // DataTable dtCategory = objcatdata.FillCategoryCountryModel(2837, 1, dtSearchDate, 1,0);

                foreach (DataRow dr in dtCategory.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.CategoryId = Convert.ToInt32(dr["CategoryId"]);
                    catinfo.ImagePath = "ImageHandler.ashx?ID=" + catinfo.Id + "&Type=1&width=75&height=75&isBinary=1";
                    category.Add(catinfo);

                }
                return category.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryFill", ex);
            }
        }

        public List<CategoryInfo> CategoryAggregate(int Id, string StartDate, int LngId)
        {

            List<CategoryInfo> category = new List<CategoryInfo>();
            try
            {
                // System.Web.HttpContext.Current.Session["ModelId"] = Id;
                CategoryViewData objcatdata = new CategoryViewData();
                DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA
                //string check_date = "01-Jan-" + StartDate; // Arctic cat
                //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

                DataTable dtCategory = objcatdata.FillCatModelWithOutCountry(Id, dtSearchDate, LngId, 0);
                foreach (DataRow dr in dtCategory.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.CategoryId = Convert.ToInt32(dr["CategoryId"]);
                    catinfo.ImagePath = "ImageHandler.ashx?ID=" + catinfo.Id + "&Type=1&width=75&height=75&isBinary=1";
                    category.Add(catinfo);

                }
                return category.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryFill", ex);
            }
        }

        public List<VinAssemblyInfo> Assembly(int Id, bool blnCondi, string StartDate, int LngId)
        {

            List<VinAssemblyInfo> assembly = new List<VinAssemblyInfo>();
            try
            {
                CategoryViewData objcatdata = new CategoryViewData();
                 DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA
               // string check_date = "01-Jan-" + StartDate; // Arctic cat
                //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

                DataTable dtCategory = objcatdata.FillCategoryAssembly(Id, Convert.ToBoolean(MainCommon.FigNoWithModelCode), dtSearchDate, LngId, 0);

                foreach (DataRow dr in dtCategory.Rows)
                {
                    VinAssemblyInfo catinfo = new VinAssemblyInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.AMId = Convert.ToInt32(dr["AMId"]);
                    catinfo.FigNo = Convert.ToString(dr["FigNo"]);
                    catinfo.ImagePath = "ImageHandler.ashx?ID=" + catinfo.Id + "&Type=1&width=75&height=75&isBinary=1";
                    assembly.Add(catinfo);

                }
                return assembly.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryFill", ex);
            }
        }

        public List<CategoryInfo> CategoryFillType(bool blnAccessLvl, string UserID, int LngId, string countryid)
        {

            List<CategoryInfo> category = new List<CategoryInfo>();
            try
            {
                var userid_en = DecryptAes(UserID);
                Int32 userids = Convert.ToInt32(userid_en);

                var country_en = DecryptAes(countryid);
                Int32 countrys = Convert.ToInt32(country_en);

                CategoryViewData objcatdata = new CategoryViewData();

                int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));
                if (UserType == 0)
                    UserType = 1;

                //int countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserID));

                DataTable dtCategory = objcatdata.FillCategoryType(blnAccessLvl, userids, UserType, countrys, LngId, 0);

                foreach (DataRow dr in dtCategory.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.ImagePath = "ImageHandler.ashx?ID=" + catinfo.Id + "&Type=1&width=75&height=75&isBinary=1";
                    category.Add(catinfo);

                }
                return category.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryFill", ex);
            }
        }

        public List<IllustrationInfo> IllustrationPartList(string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, string UserType)
        {
            List<IllustrationInfo> plist = new List<IllustrationInfo>();
            try
            {
                var userid_en = DecryptAes(UserId);
                Int32 userids = Convert.ToInt32(userid_en);

                var usertype_en = DecryptAes(UserType);
                Int32 usertypes = Convert.ToInt32(usertype_en);

                // if (ModelID == 0) ModelID = Convert.ToInt32("0" + Convert.ToString(System.Web.HttpContext.Current.Session["modleid"]));
                DataTable dtPart = GetIllustrationPartDetails(ModelID, AssemblyID, StartDate, SrVINNO, Lngid, userids, usertypes);

                // int i = 0;
                foreach (DataRow dr in dtPart.Rows)
                {
                    IllustrationInfo ilinfo = new IllustrationInfo();
                    ilinfo.partNumber = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                    ilinfo.description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                    ilinfo.Servicable = Convert.ToString(dr["Serviceable"]) == "" ? "" : Convert.ToString(dr["Serviceable"]);
                    ilinfo.startdate = Convert.ToString(dr["StartDate"]) == "" ? "" : Convert.ToString(dr["StartDate"]);
                    ilinfo.history = Convert.ToString(dr["History"]) == "" ? "" : Convert.ToString(dr["History"]);
                    ilinfo.quantity = Convert.ToInt32(dr["Qty"]) == 0 ? 0 : Convert.ToInt32(dr["Qty"]);
                    ilinfo.Remark = Convert.ToString(dr["Remark"]) == "" ? "" : Convert.ToString(dr["Remark"]);
                    ilinfo.Asspartmodid = Convert.ToString(dr["ASSPARTMODID"]) == "" ? "" : Convert.ToString(dr["ASSPARTMODID"]);
                    ilinfo.Partid = Convert.ToString(dr["PARTID"]) == "" ? "" : Convert.ToString(dr["PARTID"]);
                    ilinfo.Asseblypartid = Convert.ToString(dr["ASSEMBLYPARTID"]) == "" ? "" : Convert.ToString(dr["ASSEMBLYPARTID"]);
                    ilinfo.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                    ilinfo.Price2 = Convert.ToString(dr["PRICE2"]) == "" ? "" : Convert.ToString(dr["PRICE2"]);
                    ilinfo.RefNo = Convert.ToString(dr["REFNO"]) == "" ? "" : Convert.ToString(dr["REFNO"]);
                    ilinfo.aGroupID = dr["AGROUPID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AGROUPID"]);
                    ilinfo.ParentPartId = dr["PARENTPARTID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PARENTPARTID"]);
                    ilinfo.AlternateDescription = "";
                    plist.Add(ilinfo);

                }

                return plist.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("IllustrationPartList", ex);
            }

        }

        public DataTable GetIllustrationPartDetails(Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 LngId, Int32 UserId, Int32 UserType)
        {
            Int32 Lngid = 1;

            Int32 iCountryId = 1;
            if (UserId > 0)
                iCountryId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserId));

            decimal decVId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("Select rootmapid From tbl_Categorymapping where Id=" + ModelID));

            //string check_date = "01-Jan-" + StartDate;// Arctic Cat
            //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic Cat

            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA

            DataTable dt;
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@ModelID", ModelID, DbType.Int32));
            cps.Add(new CommanParameter("@CountryID", iCountryId, DbType.Int32));
            cps.Add(new CommanParameter("@AssemblyID", AssemblyID, DbType.Int32));
            cps.Add(new CommanParameter("@SEDate", dtSearchDate, DbType.Date));
            cps.Add(new CommanParameter("@SrVINNO", SrVINNO, DbType.Int32));
            cps.Add(new CommanParameter("@LngId", Lngid, DbType.Int32, 1));
            cps.Add(new CommanParameter("@uType", UserType, DbType.Int32, 0));

            dt = maincls.DbCon.GetProcTable("PROC_ILLGETASSPARTS", cps, "PartDetails");

            string sAssParts = "-1";
            //checks whether any history rows exist for this model and assembly
            Catalog.WebArch.CommanParametrs cpHistCount = new Catalog.WebArch.CommanParametrs();
            cpHistCount.Add(new Catalog.WebArch.CommanParameter("@ModelID", ModelID));
            cpHistCount.Add(new Catalog.WebArch.CommanParameter("@AssemblyID", AssemblyID));

            object count = maincls.DbCon.ExecuteScaler("PROC_ILLGETHISTCOUNT", cpHistCount, "Pcount");
            if (Convert.ToInt32(count) > 0)
            {
                //Get Update Part History
                foreach (DataRow dr in dt.Rows)
                {
                    Catalog.WebArch.CommanParametrs cpHistAssCount = new Catalog.WebArch.CommanParametrs();
                    cpHistAssCount.Add(new Catalog.WebArch.CommanParameter("@ModelID", ModelID));
                    cpHistAssCount.Add(new Catalog.WebArch.CommanParameter("@AssemblyID", AssemblyID));
                    cpHistAssCount.Add(new Catalog.WebArch.CommanParameter("@PPartID", dr["partid"].ToString()));
                    cpHistAssCount.Add(new Catalog.WebArch.CommanParameter("@AssPartID", dr["AssemblypartId"].ToString()));

                    object icnt = maincls.DbCon.ExecuteScaler("PROC_ILLGETASSPARTHISTCOUNT", cpHistAssCount, "Pcount");

                    if (Convert.ToInt32(icnt) > 0)
                    {    //for History
                        //Updatepart(dr, stDate);
                        //Creating Object Of History Class
                        clsHistory objHistory = new clsHistory();
                        DataRow drUpdate = GetUpdatedPart(ModelID, Convert.ToDecimal(dr["ASSEMBLYPARTID"]), dtSearchDate, Convert.ToInt32(dr["QTY"]), UserId);
                        if (drUpdate != null)
                        {
                            //Updating rows
                            //added for updating history referal part id
                            DataRow[] arrdrReferalPart = dt.Select("" + dr["PARTID"].ToString() + "=ParentPartID");
                            if (arrdrReferalPart.Length > 0)
                            {
                                foreach (DataRow drReferalPart in arrdrReferalPart)
                                {
                                    drReferalPart["ParentPartID"] = drUpdate["PartID"];
                                }
                                dt.AcceptChanges();
                            }
                            //end of code
                            dr["PARTID"] = drUpdate["PartID"];
                            dr["PartNo"] = drUpdate["PartNo"];
                            dr["Description"] = drUpdate["Description"];
                            dr["startdate"] = drUpdate["startdate"];
                            dr["enddate"] = drUpdate["enddate"];
                            dr["SERVICEABLE"] = drUpdate["SERVICEABLE"];
                            dr["price1"] = drUpdate["price1"];
                            dr["remark"] = drUpdate["remark"];
                            dr["history"] = drUpdate["history"];
                            dr["NatoNo"] = drUpdate["NatoNo"];
                            string strimgid = maincls.DbCon.GetID("select PARTID from tbl_PartAttachment where PartId=" + Convert.ToInt32(drUpdate["PartID"]), "PARTID");
                            if (strimgid != "-1")
                                dr["PIMGID"] = Convert.ToInt32(strimgid);
                            else
                                dr["PIMGID"] = DBNull.Value;
                            dr.AcceptChanges();
                        }
                        else
                        {
                            if (System.Web.HttpContext.Current.Session["SrVINNO"] != null)
                                dr.Delete();
                        }
                    }
                }
            }

            //accepting all changes
            dt.AcceptChanges();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (Convert.ToDateTime(dr["StartDate"]) > dtSearchDate)
            //    {
            //        dr.Delete();
            //    }
            //    //If end date is available and it is less then selected date then delete
            //    else if (dr["EndDate"] != DBNull.Value)
            //    {
            //        if (Convert.ToDateTime(dr["EndDate"]) < dtSearchDate)
            //        {
            //            dr.Delete();
            //        }
            //    }
            //}
            //accepting all changes
            dt.AcceptChanges();


            foreach (DataRow dr in dt.Rows)
            {
                sAssParts += "," + dr["assemblypartid"].ToString();
            }

            //HttpContext.Current.Session["dtAssemblyPart"] = dt;

            //Fill all the Dataset tables that will be helpful in the Designing the Assembly Image
            c = new Common(AssemblyID, ModelID, dtSearchDate, sAssParts, iCountryId, 1); //1 fro iLngId




            string strDisId = "-1";
            object objdis = 1;
            string strcond = string.Empty;
            DateTime dteSeldate = Convert.ToDateTime(dtSearchDate);
            string strSelDate = MainClass.GetDateString(dteSeldate);
            if (Convert.ToInt32(UserType) == 0 || Convert.ToInt32(UserType) == 2)
            {
                if (Convert.ToInt32(UserType) == 2)
                {
                    string strch = "select ID from tbl_distributor where userid in (select createdby from tbl_user where id=" + Convert.ToString(UserId) + ")";
                    strDisId = maincls.DbCon.GetID(strch, "ID");
                }
                else
                    strDisId = maincls.DbCon.GetID("select ID from tbl_distributor where UserID=" + Convert.ToString(UserId) + "", "ID");
                objdis = maincls.DbCon.ExecuteScaler("select vehicleaccess from tbl_distributor where id='" + Convert.ToString(strDisId) + "'");
            }
            else
                strDisId = Convert.ToString(UserId); //OEM UserID.
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string Modelnames = string.Empty;
                    if (objdis != DBNull.Value)
                    {
                        int strPartId = Convert.ToInt32(dr["PartID"].ToString());
                        if (Convert.ToString(dr["History"].ToString()) == "H")//|| Convert.ToString(dr["History"].ToString()) == "U")
                        {
                            //int iPartID = getBasePart(strPartId, Convert.ToInt32(Request.QueryString["ID"]));
                            int iPartID = Convert.ToInt32(maincls.DbCon.ExecuteScaler("Select PartId from tbl_AssemblyPart where ID=" + Convert.ToString(dr["ASSEMBLYPARTID"])));
                            strPartId = iPartID;
                        }

                        Catalog.WebArch.CommanParametrs cps1 = new Catalog.WebArch.CommanParametrs();
                        cps1.Add(new Catalog.WebArch.CommanParameter("@PartID", strPartId));
                        cps1.Add(new Catalog.WebArch.CommanParameter("@HPartID", Convert.ToInt32(dr["PartID"].ToString())));
                        cps1.Add(new Catalog.WebArch.CommanParameter("@AssemblyID", Convert.ToInt32(AssemblyID)));

                        cps1.Add(new Catalog.WebArch.CommanParameter("@DistVehAceess", Convert.ToInt32(objdis)));
                        cps1.Add(new Catalog.WebArch.CommanParameter("@CountryID", Convert.ToInt32(iCountryId)));

                        cps1.Add(new Catalog.WebArch.CommanParameter("@DistributorID", Convert.ToInt32(strDisId)));
                        cps1.Add(new Catalog.WebArch.CommanParameter("@StartDate", dteSeldate, DbType.Date));

                        cps1.Add(new Catalog.WebArch.CommanParameter("@History", Convert.ToString(dr["History"].ToString())));
                        cps1.Add(new Catalog.WebArch.CommanParameter("@UserType", Convert.ToInt32(UserType)));

                        cps1.Add(new Catalog.WebArch.CommanParameter("@ModelID", Convert.ToInt32(decVId)));


                        DataTable dtModel = maincls.DbCon.GetProcTable("PROC_ILLGETAPPMODEL", cps1, "AppModels");
                        //putting all model in string
                        //Getting Distinct ModelName using linq.
                        var distinctModels = (from row in dtModel.AsEnumerable()
                                              select row.Field<string>("ModelName")).Distinct();
                        Modelnames = String.Join(",", distinctModels.ToArray());
                        dr["Applicable"] = Modelnames.Trim();

                    }
                }
                dt.AcceptChanges();
            }

            return dt;
        }


        public DataRow GetUpdatedPart(decimal VariantId, decimal AssemblyPartId, DateTime PStartDate, int iPqty, Int32 UserId)
        {
            int iLngId = 1;
            Int32 UCountryID = 1;
            UCountryID = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserId));

            CommanParametrs cpUpPart = new CommanParametrs();
            cpUpPart.Add(new CommanParameter("@Pqty", iPqty));
            cpUpPart.Add(new CommanParameter("@CountryID", UCountryID));
            cpUpPart.Add(new CommanParameter("@ModelID", VariantId));
            cpUpPart.Add(new CommanParameter("@AssemblyPartID", AssemblyPartId));
            cpUpPart.Add(new CommanParameter("@SEDate", PStartDate, DbType.Date));
            cpUpPart.Add(new CommanParameter("@LngId", iLngId, DbType.Int32, 1));

            DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ILLGETUPDATEPART", cpUpPart, "tpart");
            int icnt = 0;
            if (dtPart.Rows.Count > 0)
            {
                foreach (DataRow dr in dtPart.Rows)
                {
                    if (dtPart.Rows.Count > 1) dr["History"] = "H"; else dr["History"] = "U";
                    if (Convert.ToInt16(dr["Interchangeable"] == DBNull.Value ? 0 : dr["Interchangeable"]) == 4)
                    {
                        DataRow[] drs = dtPart.Select("PartId=" + dr["PPartId"]);
                        if (drs.Length > 0) drs[0]["StartDate"] = dr["StartDate"];
                        else
                        {
                            //PStartDate = (dr["EndDate"] == DBNull.Value ? PStartDate : Convert.ToDateTime(dr["EndDate"])).AddDays(1);
                            PStartDate = (dr["EndDate"] == DBNull.Value ? PStartDate : Convert.ToDateTime(dr["EndDate"]));
                            DataRow drr = GetUpdatedPart(VariantId, AssemblyPartId, PStartDate, iPqty, UserId);
                            return drr;
                        }
                        dr.Delete();
                    }
                    icnt++;
                    if (icnt > 1) break;
                }
                dtPart.AcceptChanges();

                return dtPart.Rows[0];
            }
            return null;
        }

        public List<AutoPartInfo> AutoCompletePartNo(string UserID, int LngId, string StartDate, string searchvalue, string vehicle, string model, string countryid)
        {
            List<AutoPartInfo> autoPartInfo = new List<AutoPartInfo>();

            var userid_en = DecryptAes(UserID);
            Int32 userids = Convert.ToInt32(userid_en);

            var country_en = DecryptAes(countryid);
            Int32 countrys = Convert.ToInt32(country_en);

            int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));

            if (UserType == 0)
                UserType = 1;

            //int countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserID));
            Boolean blnAccessLvl = false;
            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA
            //string check_date = "01-Jan-" + StartDate; // Arctic cat
            //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

            string smodelid = getCurrentVariantIds(UserType, userids, countrys, blnAccessLvl, LngId, dtSearchDate);
            try
            {
                PartData objSearch = new PartData();
                DataTable dtpart = objSearch.GetPartDetail(searchvalue, smodelid, vehicle, model);
                int i = 0;
                foreach (DataRow dr in dtpart.Rows)
                {
                    AutoPartInfo ainfo = new AutoPartInfo();
                    ainfo.Id = Convert.ToInt32(dr["Id"]);
                    ainfo.PartNo = Convert.ToString(dr["PartNo"]);
                    autoPartInfo.Add(ainfo);
                    if (i++ > 500) break;
                }

                return autoPartInfo.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("AutoCompletePartNo", ex);
            }

        }

        private string getCurrentVariantIds(int UserType, int UserID, int UCountryID, Boolean iblnAccessLvl, int LngId, DateTime idteSelDate)
        {
            string strModelIDs = "-1";
            try
            {
                PartSearch ObjPartSearchData = new PartSearch();
                int iUserType = UserType;
                int iUserID = UserID;
                int iUCountryID = UCountryID;
                Boolean blnAccessLvl = iblnAccessLvl;
                int iLngId = LngId;
                DateTime dteSelDate = idteSelDate;
                DataTable dtVehicles = ObjPartSearchData.PartSearchFillModel(blnAccessLvl, iUserID, iUserType, iUCountryID, iLngId);

                string strVehicleIDs = "-1";
                foreach (DataRow dr in dtVehicles.Rows)
                {
                    strVehicleIDs += "," + dr["ID"];
                }
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@VehicleIDs", strVehicleIDs, DbType.String));
                cps.Add(new CommanParameter("@SelDate", dteSelDate, DbType.Date));
                cps.Add(new CommanParameter("@CountryID", iUCountryID, DbType.Int32));
                cps.Add(new CommanParameter("@AllVarints", false, DbType.Boolean, 0));

                DataTable dtVariants = maincls.DbCon.GetProcTable("PROC_PARTSEARCHgETvARAINTIDS", cps, "tmp");
                if (dtVariants.Rows.Count > 0)
                    strModelIDs = Convert.ToString(dtVariants.Rows[0][0]);
                return strModelIDs;
            }
            catch (Exception ex)
            { throw new Exception("getCurrentVariantIds", ex); }
            //return strModelIDs;
        }

        public List<AutoDescriptionInfo> AutoCompleteDescription(string UserID, int LngId, string StartDate, string description, string vehicle, string model, string countryid)
        {
            List<AutoDescriptionInfo> autoPartInfo = new List<AutoDescriptionInfo>();

            var userid_en = DecryptAes(UserID);
            Int32 userids = Convert.ToInt32(userid_en);

            var country_en = DecryptAes(countryid);
            Int32 countrys = Convert.ToInt32(country_en);

            int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));
            if (UserType == 0)
                UserType = 1;

            //int countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserID));
            Boolean blnAccessLvl = false;
            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate); // MUSA
            //string check_date = "01-Jan-" + StartDate; // Arctic cat
            //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

            string smodelid = getCurrentVariantIds(UserType, userids, countrys, blnAccessLvl, LngId, dtSearchDate);
            try
            {
                PartData objSearch = new PartData();
                DataTable dtpart = objSearch.GetPartDescription(description, smodelid, vehicle, model);
                int i = 0;
                foreach (DataRow dr in dtpart.Rows)
                {
                    AutoDescriptionInfo ainfo = new AutoDescriptionInfo();
                    //ainfo.Id = Convert.ToInt32(dr["Id"]);
                    ainfo.Description = Convert.ToString(dr["Description"]);
                    autoPartInfo.Add(ainfo);
                    if (i++ > 2000) break;
                }

                return autoPartInfo.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("AutoCompleteDescription", ex);
            }

        }

        public List<CategoryInfo> FillVehicle(bool blnAccessLvl, string UserID, int LngId, string countryid)
        {
            List<CategoryInfo> vehicle = new List<CategoryInfo>();

            try
            {
                var userid_en = DecryptAes(UserID);
                Int32 userids = Convert.ToInt32(userid_en);

                var country_en = DecryptAes(countryid);
                Int32 countrys = Convert.ToInt32(country_en);

                PartSearch objSearch = new PartSearch();
                int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));
                if (UserType == 0)
                    UserType = 1;

                //int countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserID));
                DataTable dtVehicle = objSearch.PartSearchFillModel(blnAccessLvl, userids, UserType, countrys, LngId);
                DataView dv = dtVehicle.DefaultView;
                dv.Sort = "CategoryName asc";
                DataTable dtt = dv.ToTable();
                foreach (DataRow dr in dtt.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.Priority = dr["Priority"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Priority"]);
                    vehicle.Add(catinfo);

                }

                return vehicle.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("FillVehicle", ex);
            }

        }

        public List<CategoryInfo> FillModel(string VehicleIDs, string SelDate, string CountryID, int LngId)
        {
            List<CategoryInfo> model = new List<CategoryInfo>();

            try
            {
                var country_en = DecryptAes(CountryID);
                Int32 countrys = Convert.ToInt32(country_en);

                PartSearch objSearch = new PartSearch();
                DateTime dtSearchDate = SelDate == "" ? DateTime.Today : Convert.ToDateTime(SelDate); // MUSA
                //string check_date = "01-Jan-" + SelDate;// Arctic Cat
                //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic Cat

                DataTable dtVehicle = objSearch.PartSearchFillvariants(VehicleIDs, dtSearchDate, countrys, LngId);
                DataView dv = dtVehicle.DefaultView;
                dv.Sort = "CategoryName asc";
                DataTable dtt = dv.ToTable();
                foreach (DataRow dr in dtt.Rows)
                {
                    CategoryInfo catinfo = new CategoryInfo();
                    catinfo.Id = Convert.ToInt32(dr["Id"]);
                    catinfo.CategoryName = Convert.ToString(dr["CategoryName"]);
                    catinfo.Priority = dr["Priority"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Priority"]);
                    // catinfo.Priority = Convert.ToInt32(dr["Priority"]);
                    model.Add(catinfo);

                }

                return model.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("FillModel", ex);
            }

        }

        public List<PartInfo> partList(string UserId, string StartDate, Int32 Model, Int32 Variant, string Partno, string Description, string iCountryId, Int32 pagesize, string UserType)
        {
            //,int pagesize
            List<PartInfo> plist = new List<PartInfo>();
            string partno_en = string.Empty;
            try
            {
                var userid_en = DecryptAes(UserId);
                Int32 userids = Convert.ToInt32(userid_en);
                if (Partno != "")
                {
                    partno_en = DecryptAes(Partno);
                }


                var country_en = DecryptAes(iCountryId);
                Int32 countrys = Convert.ToInt32(country_en);

                var usertype_en = DecryptAes(UserType);
                Int32 usertypes = Convert.ToInt32(usertype_en);


                // Int32 iCountryId = 1;
                Int32 iVehicleId = 0;
                Int32 iVariantId = 0;
                string strVehicleIDs = "-1";
                string sVariants = "-1";

                if (Model > 0)
                {
                    iVehicleId = Model;
                    strVehicleIDs = strVehicleIDs + "," + Convert.ToString(Model);
                }
                else
                {
                    List<ModelInfo> mlist = ModelList(userids);
                    foreach (ModelInfo mModel in mlist)
                    {
                        strVehicleIDs = strVehicleIDs + "," + Convert.ToString(mModel.Id);
                    }
                }
                if (Variant > 0)
                    iVariantId = Variant;

                DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate);//MUSA
                //string check_date = "01-Jan-" + StartDate; // Arctic cat
                //DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat

                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@VehicleIDs", strVehicleIDs, DbType.String));
                cps.Add(new CommanParameter("@SelDate", dtSearchDate, DbType.Date));
                cps.Add(new CommanParameter("@CountryID", countrys, DbType.Int32));

                if (string.IsNullOrEmpty(partno_en.Trim()) && string.IsNullOrEmpty(Description.Trim()))
                    cps.Add(new CommanParameter("@AllVarints", false, DbType.Boolean, 0));
                else
                    cps.Add(new CommanParameter("@AllVarints", true, DbType.Boolean, 0));

               // DataTable dtVariants = maincls.DbCon.GetProcTable("PROC_PARTSEARCHgETvARAINTIDS", cps, "tmp");//MFS, Mahindra & Mahindra
                 DataTable dtVariants = maincls.DbCon.GetProcTable("PROC_PARTVARAINTIDSMOB", cps, "tmp");//MUSA
                if (dtVariants.Rows.Count > 0)
                    sVariants = Convert.ToString(dtVariants.Rows[0][0]);

                DataTable dtPart = GetPartSearch(countrys, iVehicleId, sVariants, iVariantId, partno_en, Description, StartDate, pagesize, usertypes);

                int i = 0;
                foreach (DataRow dr in dtPart.Rows)
                {
                    PartInfo pinf = new PartInfo();
                    pinf.ID = Convert.ToString(dr["ID"]);
                    // pinf.total = dr["COUNTTOTAL"] != DBNull.Value ? Convert.ToInt32(dr["COUNTTOTAL"]) : pinf.total;
                    pinf.total = dr["TOTALROWS"] != DBNull.Value ? Convert.ToInt32(dr["TOTALROWS"]) : pinf.total;
                    pinf.AssemblyID = Convert.ToString(dr["AssemblyID"]) == "" ? "" : Convert.ToString(dr["AssemblyID"]);
                    pinf.PmapID = Convert.ToString(dr["PmapID"]) == "" ? "" : Convert.ToString(dr["PmapID"]);
                    pinf.CatmapID = Convert.ToString(dr["CatmapID"]) == "" ? "" : Convert.ToString(dr["CatmapID"]);
                    pinf.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                    pinf.Price2 = Convert.ToString(dr["PRICE2"]) == "" ? "" : Convert.ToString(dr["PRICE2"]);
                    pinf.Price3 = Convert.ToString(dr["PARTPRICE"]) == "" ? "" : Convert.ToString(dr["PARTPRICE"]);
                    pinf.Model = Convert.ToString(dr["MODEL"]) == "" ? "" : Convert.ToString(dr["MODEL"]);
                    pinf.Vehicle = Convert.ToString(dr["VEHICLE"]) == "" ? "" : Convert.ToString(dr["VEHICLE"]);
                    pinf.partNumber = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                    pinf.description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                    pinf.AssemblyName = Convert.ToString(dr["ASSEMBLYNAME"]) == "" ? "" : Convert.ToString(dr["ASSEMBLYNAME"]);
                    pinf.GroupNo = Convert.ToString(dr["GROUPNO"]) == "" ? "" : Convert.ToString(dr["GROUPNO"]);
                    pinf.NS = Convert.ToString(dr["NS"]) == "" ? "" : Convert.ToString(dr["NS"]);
                    pinf.startdate = Convert.ToString(dr["StartDate"]) == "" ? "" : Convert.ToString(dr["StartDate"]);
                    pinf.enddate = Convert.ToString(dr["EndDate"]) == "" ? "" : Convert.ToString(dr["EndDate"]);
                    pinf.history = Convert.ToString(dr["History"]) == "" ? "" : Convert.ToString(dr["History"]);
                    pinf.quantity = Convert.ToInt32(dr["Qty"]) == 0 ? 0 : Convert.ToInt32(dr["Qty"]);
                    pinf.PartId = Convert.ToInt32(dr["PARTID"]) == 0 ? 0 : Convert.ToInt32(dr["PARTID"]);
                    pinf.AGroupId = dr["AGROUPID"] != DBNull.Value ? Convert.ToInt32(dr["AGROUPID"]) : 0;
                    plist.Add(pinf);
                    // if (i++ > 48) break;
                }

                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("partList", ex);
            }

        }

        public List<ModelInfo> ModelList(Int32 UserId)
        {
            List<ModelInfo> mlist = new List<ModelInfo>();
            try
            {


                int UCountryID = 1;
                int UserType = 0;
                Boolean blnAccessLvl = true;
                if (UserId > 0)
                {
                    UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select UserType from tbl_user where id=" + UserId));
                }
                if (UserType != -1)
                    //set the countryId of logging user
                    UCountryID = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserId));
                else
                    //set the default countryid from setting detail
                    UCountryID = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select DefCountryId from tbl_setting"));
                //When User is Distributor or distributor user
                if (Convert.ToInt32(UserType) == 0 || Convert.ToInt32(UserType) == 2)
                {
                    //get userid from session
                    int iUserID = Convert.ToInt32(UserId);
                    //store id,distributor code ,vehicle access into datatable.
                    //Creating objects of comman parameters
                    CommanParametrs objcps = new CommanParametrs();
                    CommanParameter objcp;

                    //Assign Perameter value
                    objcp = new CommanParameter("@ID", iUserID, DbType.Int32);
                    objcps.Add(objcp);

                    DataTable dtCode = maincls.DbCon.GetProcTable("PROC_USERGETCODE", objcps, "tmp");
                    //Check datatable is not empty.
                    if (dtCode != null && dtCode.Rows.Count > 0)
                    {
                        //set VehicleAccess into session
                        blnAccessLvl = Convert.ToBoolean(dtCode.Rows[0]["VehicleAccess"]);
                        //get countryId of distributor
                        object objDetailDist = maincls.DbCon.ExecuteScaler("Select CountryID from tbl_User where ID=(select UserID from tbl_Distributor where ID=" + Convert.ToInt32(dtCode.Rows[0]["ID"]) + ")");
                        //When objDetailDist variable contains countryid
                        if (objDetailDist != null)
                        {
                            //Set the countryid into session
                            UCountryID = Convert.ToInt32(objDetailDist.ToString());// as par Sitaram Sir
                        }
                    }
                }
                DataTable dtVehicles = PartSearchFillModel(UserId, UserType, UCountryID, blnAccessLvl);
                foreach (DataRow dr in dtVehicles.Rows)
                {
                    ModelInfo pinf = new ModelInfo();
                    pinf.ModelName = Convert.ToString(dr["CategoryName"]);
                    pinf.Id = Convert.ToInt32(dr["ID"]);
                    mlist.Add(pinf);
                }

                return mlist;
            }
            catch (Exception ex)
            {
                throw new Exception("ModelList", ex);
            }

        }

        private DataTable PartSearchFillModel(Int32 UserId, Int32 UserType, Int32 UCountryID, bool blnAccessLvl)
        {
            DataTable dtVehicles = new DataTable();
            // Getting User Type And ID from the Session
            int iUserType = Convert.ToInt32(UserType);
            int iUserID = Convert.ToInt32(UserId);
            int iUCountryID = Convert.ToInt32(UCountryID);
            Int32 LngId = 1;
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@blnAccessLvl", blnAccessLvl, DbType.Boolean));
            cps.Add(new CommanParameter("@UserID", iUserID, DbType.Int32));
            cps.Add(new CommanParameter("@UserType", iUserType, DbType.Int32, 0));
            cps.Add(new CommanParameter("@CountryID", iUCountryID, DbType.Int32));
            cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            dtVehicles = maincls.DbCon.GetProcTable("PROC_PARTSEARCHFILLMODEL", cps, "tbl_category");
            // Adding Dummy row.
            DataRow drd = dtVehicles.NewRow();
            drd["ID"] = -1;
            drd["CategoryName"] = "Select Model";
            dtVehicles.Rows.InsertAt(drd, 0);
            return dtVehicles;
        }

        private DataTable GetPartSearch(Int32 iCountryId, Int32 iVehicleId, string sVariants, Int32 iVariantId, string Partno, string Description, string StartDate, int pagesize, int usertype)
        {
            
            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate);// MUSA
            //string check_date = "01-Jan-" + StartDate; // Arctic cat
           // DateTime dtSearchDate = Convert.ToDateTime(check_date);// Arctic cat
            string sPartNo = Partno.Trim().ToUpper() == "" ? null : Partno.Trim().ToUpper();
            string sPartname = Description.Trim().ToUpper() == "" ? null : Description.Trim().ToUpper();
            string sFigNo = null;
            Boolean bPeculier = false;
            Boolean bNewPart = false;
            string sSrVinNo = null;
            Int32 LngId = 1;
            DataTable dtPartTmp;
            DataTable dtParts;
            // Scroll size in Part search
            int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@CountryID", iCountryId, DbType.Int32));
            cps.Add(new CommanParameter("@PmapID", iVehicleId, DbType.Int32));
            cps.Add(new CommanParameter("@VID", sVariants, DbType.String));
            cps.Add(new CommanParameter("@StartDate", dtSearchDate, DbType.Date));
            cps.Add(new CommanParameter("@PartNo", sPartNo, DbType.String));
            cps.Add(new CommanParameter("@PartName", sPartname, DbType.AnsiString));
            cps.Add(new CommanParameter("@FigNo", sFigNo, DbType.String));
            cps.Add(new CommanParameter("@Peculier", bPeculier, DbType.Boolean));
            cps.Add(new CommanParameter("@NewPart", bNewPart, DbType.Boolean));
            cps.Add(new CommanParameter("@chModel", iVariantId, DbType.Int32));
            cps.Add(new CommanParameter("@SrVinNo", sSrVinNo, DbType.String));
            cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            cps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32));
            cps.Add(new CommanParameter("@Page", pagesize, DbType.Int32));
            cps.Add(new CommanParameter("@uType", usertype, DbType.Int32, 0));


            //dtPartTmp = maincls.DbCon.GetProcTable("PROC_PARTSEARCHWOHISTORYMOB", cps, "tbl_part");
            dtPartTmp = maincls.DbCon.GetProcTable("PROC_PARTSEARCHWOHISTORY", cps, "tbl_part");
            dtParts = dtPartTmp.Clone();
            dtPartTmp.Select("INTERCHANGEABLE <> 4 OR interchangeable is null").CopyToDataTable(dtParts, LoadOption.OverwriteChanges);
            dtPartTmp.Clear();
            DataTable dtUParts = null;
            if (sPartNo != null || sPartname != null)
            {
                cps.Clear();
                cps.Add(new CommanParameter("@CountryID", iCountryId, DbType.Int32));
                cps.Add(new CommanParameter("@PmapID", iVehicleId, DbType.Int32));
                cps.Add(new CommanParameter("@VID", sVariants, DbType.String));
                cps.Add(new CommanParameter("@StartDate", dtSearchDate, DbType.Date));
                cps.Add(new CommanParameter("@PartNo", sPartNo, DbType.String));
                cps.Add(new CommanParameter("@PartName", sPartname, DbType.String));
                cps.Add(new CommanParameter("@FigNo", sFigNo, DbType.String));
                cps.Add(new CommanParameter("@Peculier", bPeculier, DbType.Boolean));
                cps.Add(new CommanParameter("@chModel", iVariantId, DbType.Int32));
                cps.Add(new CommanParameter("@SrVinNo", sSrVinNo, DbType.String));
                //cps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32)); //Add for MFS & Mahindra & Mahindra
                //cps.Add(new CommanParameter("@Page", pagesize, DbType.Int32)); //Add for MFS & Mahindra & Mahindra
              //  dtUParts = maincls.DbCon.GetProcTable("PROC_PARTSEARCHWHISTORY", cps, "PartSearch");//MFS & Mahindra & Mahindra
                dtUParts = maincls.DbCon.GetProcTable("PROC_PARTSEARCHWHISTORYMOB", cps, "PartSearch");//MUSA
                dtPartTmp = dtUParts.Clone();
                dtPartTmp.Columns["ID"].DataType = dtParts.Columns["ID"].DataType;
                dtPartTmp.Columns["ASSPARTMODID"].DataType = dtParts.Columns["ASSPARTMODID"].DataType;
                dtPartTmp.Columns["QTY"].DataType = dtParts.Columns["QTY"].DataType;
                dtUParts.Select("ID<>0").CopyToDataTable(dtPartTmp, LoadOption.OverwriteChanges);
                dtUParts.Dispose();
                //deleting same part for two models of same name.
                foreach (DataRow dr in dtPartTmp.Rows)
                {
                    DataRow[] drDup = dtParts.Select("PartID=" + dr["PartID"].ToString() + " And AssemblyID=" + dr["AssemblyID"].ToString() + " And MODEL='" + dr["MODEL"].ToString().Replace("'", "''") + "'");
                    if (drDup != null && drDup.Length > 0)
                        dr.Delete();
                }
                dtPartTmp.AcceptChanges();
                // Merging Base Parts and Intermediate Parts.
                dtParts.Merge(dtPartTmp);

                dtPartTmp.Clear();
                dtPartTmp = dtParts.Clone();
                dtParts.Select("CatInactive=0").CopyToDataTable(dtPartTmp, LoadOption.OverwriteChanges);
                dtParts.Clear();
                dtParts = dtPartTmp;

                dtParts.DefaultView.Sort = "PartNo,AssemblyName,CatMapID,StartDate";
                dtParts.AcceptChanges();
            }
            return dtParts;
        }

        public List<RequestCategory> RequestTypeData(Int32 RequestType)
        {
            List<RequestCategory> plist = new List<RequestCategory>();
            try
            {
                Int32 NeedRequestType = 1;
                CommanParameter cp;
                CommanParametrs cps = new CommanParametrs();

                //Assign parameter value
                cp = new CommanParameter("@ID", 0, DbType.Int32);
                cps.Add(cp);

                if (NeedRequestType == 1)
                    cp = new CommanParameter("@RequestType", RequestType, DbType.Int32, 0);
                else
                    cp = new CommanParameter("@RequestType", RequestType, DbType.Int32);
                cps.Add(cp);
                DataTable dtrequest = maincls.DbCon.GetProcTable("PROC_REQUESTCATEGORYLIST", cps, "Request category");

                //checking that any Assembly if exist or not 
                foreach (DataRow dr in dtrequest.Rows)
                {
                    RequestCategory req = new RequestCategory();
                    req.Id = Convert.ToInt32(dr["Id"]);
                    req.RequestType = Convert.ToInt32(dr["RequestType"]) == 0 ? 0 : Convert.ToInt32(dr["RequestType"]);
                    req.RequestDesc = Convert.ToString(dr["RequestDesc"]) == "" ? "" : Convert.ToString(dr["RequestDesc"]);

                    plist.Add(req);
                }
                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("RequestTypeData", ex);
            }
        }

        public List<AccidentalViewInfo> AccidentalViewType(int iModelID, int LngId)
        {

            List<AccidentalViewInfo> accTypeCategory = new List<AccidentalViewInfo>();
            try
            {
                AccidentalRepairViewData accViewData = new AccidentalRepairViewData();
                DataTable dtViewType = accViewData.GetVehicleViewType(iModelID, LngId);

                foreach (DataRow dr in dtViewType.Rows)
                {
                    AccidentalViewInfo avinfo = new AccidentalViewInfo();
                    avinfo.ID = Convert.ToInt32(dr["ID"].ToString());
                    avinfo.ViewType = Convert.ToString(dr["VIEWTYPE"]) == "" ? "" : Convert.ToString(dr["VIEWTYPE"]);
                    avinfo.ViewTypeMapId = Convert.ToInt32(dr["VIEWTYPEMAPID"]) == 0 ? 0 : Convert.ToInt32(dr["VIEWTYPEMAPID"]);
                    accTypeCategory.Add(avinfo);
                }
                //if (dtViewType.Rows.Count == 0)
                //{
                //    AccidentalViewInfo avinfo = new AccidentalViewInfo();
                //    avinfo.ID = 0;
                //    accTypeCategory.Add(avinfo);
                //}
                return accTypeCategory.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("AccidentalViewType", ex);
            }
        }

        public List<AccidentalImageInfo> AccidentalImage(int iModelID)
        {

            List<AccidentalImageInfo> accTypeCategory = new List<AccidentalImageInfo>();
            try
            {
                AccidentalRepairViewData accViewData = new AccidentalRepairViewData();
                DataTable dtImage = accViewData.FillImages(iModelID);

                foreach (DataRow dr in dtImage.Rows)
                {
                    AccidentalImageInfo aimageinfo = new AccidentalImageInfo();
                    aimageinfo.ID = Convert.ToInt32(dr["ID"]) == 0 ? 0 : Convert.ToInt32(dr["ID"]);
                    aimageinfo.ID1 = Convert.ToInt32(dr["ID1"]) == 0 ? 0 : Convert.ToInt32(dr["ID1"]);
                    aimageinfo.Priority = Convert.ToInt32(dr["PRIORITY"]) == 0 ? 0 : Convert.ToInt32(dr["PRIORITY"]);
                    accTypeCategory.Add(aimageinfo);
                }
                if (dtImage.Rows.Count == 0)
                {
                    AccidentalImageInfo aimageinfo = new AccidentalImageInfo();
                    aimageinfo.ID = 0;
                    aimageinfo.ID1 = 0;
                    aimageinfo.Priority = 0;
                    accTypeCategory.Add(aimageinfo);
                }
                return accTypeCategory.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("AccidentalImage", ex);
            }
        }

        public List<AccidentalInfo> AccidentalViewAssembly(string strarrwords, int iid, bool blnRemove, decimal zooms, string strAssID, string chassisno, string vid, string currentDate, int lngid)
        {
            List<AccidentalInfo> accinfo = new List<AccidentalInfo>();
            string[] longwords = strarrwords.Split(',');


            DataTable dtFilter = Accidentalvinsearch(chassisno, vid);
            string strAssemblyID = string.Empty;
            DataTable dtAssembly = null;
            DataTable dtNewAssemblies = null;
            int iModelID = -1;
            string strNotAss = string.Empty;
            string strAssembliesID = string.Empty;
            string strCond = string.Empty;
            string SelDate = MainClass.GetDateString(Convert.ToDateTime(currentDate));
            AccidentalRepairViewData accviewdata = new AccidentalRepairViewData();

            iModelID = Convert.ToInt32(vid);

            dsWCImage dsImage1 = new dsWCImage();

            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@VehicleID", iid, DbType.Int32));
            //Fill the table to get Assembly Area Detail.
            maincls.DbCon.FillProcTable("PROC_ACCRPRMAPFillAssAreaDTL", cps, dsImage1.Area, Catalog.PreviousRecord.Clear);
            //fill the table to get AssemblyArea,AssemblyArea point.
            maincls.DbCon.FillProcTable("PROC_ACCRPRMAPFILLASSAREAPOINT", cps, dsImage1.AreaPoint, Catalog.PreviousRecord.Clear);
            //fill the table to get Catalogue View AssemblyArea.
            maincls.DbCon.FillProcTable("PROC_ACCRPRMAPFILLASSAREA", cps, dsImage1.MapArea, PreviousRecord.Clear);

            string strid = GetAllPartsPrint(longwords, zooms, dsImage1);
            // Getting the current Assembly's datatable from the session.
            string[] strId = strAssID.Split(',');

            //if (HttpContext.Current.Session["Assemblies"] != null)
            //    dtAssembly = (DataTable)HttpContext.Current.Session["Assemblies"];

            if (dtAssembly != null && dtAssembly.Rows.Count > 0)
            {
                // if row found then delete it.
                if (strId.Length > 1)
                {
                    foreach (string sid in strId)
                    {
                        DataRow[] drArr = dtAssembly.Select("ID =" + sid);
                        if (drArr != null && drArr.Length > 0)
                            drArr[0].Delete();
                        dtAssembly.AcceptChanges();
                    }
                }
            }
            if (dtAssembly == null)
            {
                if (blnRemove == false)
                {
                    //Add By Amit
                    if (strid == "") //if return null or empty then message popup for no record found.
                    {
                        return null;
                        //return;
                    }
                }
            }

            char[] arrchSpr = { ',' };
            if (!string.IsNullOrEmpty(strid))
                strid = strid.Trim(arrchSpr);

            string strConds = string.Empty;
            if (MainCommon.FigNoWithModelCode)
                strConds = "Yes";
            else
                strConds = "No";


            //Filling Datatable for new assemblies.
            //Added by Durgesh against the Procedure PROC_ACCRPRMAPFILLASSEMBLY
            CommanParametrs cpss = new CommanParametrs();
            cpss.Add(new CommanParameter("@AssemblyID", strAssID, DbType.String));
            cpss.Add(new CommanParameter("@ModelID", iModelID, DbType.String));
            cpss.Add(new CommanParameter("@Date", Convert.ToDateTime(currentDate), DbType.Date));
            cpss.Add(new CommanParameter("@ID", strid, DbType.String));
            cpss.Add(new CommanParameter("@Cond", strCond, DbType.String, null));
            cpss.Add(new CommanParameter("@LngId", lngid, DbType.Int32, 1));
            dtNewAssemblies = maincls.DbCon.GetProcTable("PROC_ACCRPRMAPFILLASSEMBLYM", cpss, "tbl_Assembly");
            //dtNewAssemblies = accviewdata.FillAssemblyDetails(strAssID, iModelID.ToString(), Convert.ToDateTime(currentDate), strid, strConds); // MainClass.DbCon.GetTable(strAssembliesID, "NewAss");

            // if Already some selected Assemblies exists then...
            if (dtAssembly != null && dtAssembly.Rows.Count > 0)
            {
                // Updating the Selected Assembly's datatable.
                if (dtNewAssemblies != null)
                {//Adding Assembly Name in Data Table.
                    if (dtNewAssemblies.Rows.Count > 0)
                    {
                        foreach (DataRow drTemp in dtNewAssemblies.Rows)
                        {
                            DataRow[] arrdrAss = dtAssembly.Select("ID =" + drTemp["ID"].ToString());
                            if (arrdrAss != null && arrdrAss.Length > 0) continue;
                            // Adding row in the old table.
                            DataRow drNew = dtAssembly.NewRow();
                            drNew["ID"] = drTemp["ID"];
                            drNew["CATEGORYNAME"] = drTemp["CATEGORYNAME"];
                            drNew["FigNo"] = drTemp["FigNo"];
                            drNew["AMID"] = drTemp["AMID"];
                            drNew["ATTACHID"] = drTemp["ATTACHID"];
                            drNew["NOTEID"] = drTemp["NOTEID"];
                            dtAssembly.Rows.Add(drNew); //Merging datarow in assembly datatable.
                        }
                    }
                    //else
                    //{
                    //    return "";
                    //}
                }
            }
            else
                dtAssembly = dtNewAssemblies;

            //DataTable dt = dtAssembly.Clone();

            if (!string.IsNullOrEmpty(chassisno))
            {
                if (dtFilter != null && dtFilter.Rows.Count > 0)
                {

                    var resass = from s in dtFilter.AsEnumerable()
                                 join t in dtAssembly.AsEnumerable() on s.Field<decimal>("ID") equals t.Field<decimal>("ID")
                                 select t;
                    dtAssembly = resass.CopyToDataTable();
                    //dtAssembly.Select("ID in("+ String.Join(",",resass.ToArray()) + ")").CopyToDataTable(dt,LoadOption.OverwriteChanges);
                }
                else
                {
                    dtAssembly.Clear();
                    dtAssembly.TableName = "tbl_Assembly";
                    return accinfo;

                }
            }

            dtAssembly.TableName = "tbl_Assembly";

            // Assigining the updated datatable in the session variable and binding it to the list view control.

            // HttpContext.Current.Session["Assemblies"] = dtAssembly;
            if (dtAssembly.Rows.Count > 0)
            {
                foreach (DataRow dr in dtAssembly.Rows)
                {
                    AccidentalInfo ai = new AccidentalInfo();
                    ai.ID = Convert.ToInt32(dr["ID"].ToString());
                    ai.CategoryName = Convert.ToString(dr["CATEGORYNAME"]) == "" ? "" : Convert.ToString(dr["CATEGORYNAME"]);
                    ai.AssemblyName = Convert.ToString(dr["ASSEMBLYNAME"]) == "" ? "" : Convert.ToString(dr["ASSEMBLYNAME"]);
                    ai.FigNo = Convert.ToString(dr["FIGNO"]) == "" ? "" : Convert.ToString(dr["FIGNO"]);
                    ai.AMID = Convert.ToInt32(dr["AMID"]) == 0 ? 0 : Convert.ToInt32(dr["AMID"]);
                    accinfo.Add(ai);
                }
            }

            return accinfo.ToList();
        }

        public DataTable Accidentalvinsearch(string chassisno, string vid)
        {
            // Getting model id related to the VinNo. and EngineNo.
            string strChassisSql = @"Select CategoryMapID,StartDate,SBOMID From tbl_chassismaster 
         Where rtrim(ChassisNo) ='" + chassisno.Trim().ToUpper() + @"' And
         rtrim(CategoryMapId) ='" + vid.Trim().ToUpper() + "'";


            DataTable dtVin = maincls.DbCon.GetTable(strChassisSql, "dtVin");
            DataTable dtNewAssemblies = null;
            AccidentalRepairViewData accviewdata = new AccidentalRepairViewData();
            if (dtVin.Rows.Count > 0)
            {
                HttpContext.Current.Session["SelDate"] = dtVin.Rows[0]["StartDate"].ToString();

                if (dtVin.Rows[0]["SBOMID"].ToString() != "")
                {
                    string str = @" select AssemblyId ID from tbl_CatAssMapping where ID in (select CatAssMapID from tbl_sbomdetail where SBOMID='" + dtVin.Rows[0]["SBOMID"].ToString() + "')";
                    dtNewAssemblies = maincls.DbCon.GetTable(str, "dtAsm");
                }
                else
                {
                    string str = @" select AssemblyId ID from tbl_CatAssMapping where CategoryMapID in (select ID from tbl_CategoryMapping where  PMapID='" + vid + "') and StartDate<='" + dtVin.Rows[0]["StartDate"].ToString() + "'";
                    dtNewAssemblies = maincls.DbCon.GetTable(str, "dtAsm");
                }

            }
            else
            {
                //HttpContext.Current.Session["SelDate"] = DateTime.Now;
            }

            return dtNewAssemblies;
        }

        private string GetAllPartsPrint(string[] strarrwords, decimal zomes, dsWCImage dsImage1)
        {
            dsWCImage ds = new dsWCImage();
            dsWCImage.AreaRow currentAreaRow;
            currentAreaRow = ds.Area.AddAreaRow(0, 0, 0, 0); //Adding New Row with values in Area Table and assigning that value in CurrentAreaRow object of dataset table.
            currentAreaRow.MAXX = int.MinValue;
            currentAreaRow.MAXY = int.MinValue;
            currentAreaRow.MINX = int.MaxValue;
            currentAreaRow.MINY = int.MaxValue;
            ds.AreaPoint.Clear();
            //Adding the points one by one in the dataset using the AddPoint function defines in the Accident class
            for (int i = 0; i < strarrwords.Length - 1; i += 2)
            {
                try
                {
                    int ix = Convert.ToInt32(Convert.ToInt32(strarrwords[i]) / Convert.ToDecimal(zomes));
                    int iy = Convert.ToInt32(Convert.ToInt32(strarrwords[i + 1]) / Convert.ToDecimal(zomes));
                    if (ix > currentAreaRow.MAXX) currentAreaRow.MAXX = ix;
                    if (iy > currentAreaRow.MAXY) currentAreaRow.MAXY = iy;
                    if (ix < currentAreaRow.MINX) currentAreaRow.MINX = ix;
                    if (iy < currentAreaRow.MINY) currentAreaRow.MINY = iy;
                    ds.AreaPoint.AddAreaPointRow(ix, iy, currentAreaRow); ; //this function add point in dataset one by one.
                    //pp[j] = new System.Drawing.Point(ix, iy);
                    //j++;
                }
                catch { }
            }

            //System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            //gp.AddPolygon(pp); 


            string strcatviewimgassid = "";
            //This loop contineos to get area of Selected Area.
            foreach (dsWCImage.AreaRow ar in dsImage1.Area)
            {
                //array for select area point
                dsWCImage.AreaPointRow[] arrard = (dsWCImage.AreaPointRow[])dsImage1.AreaPoint.Select("AreaID=" + ar["ID"]);
                if (ar.RowState == DataRowState.Deleted) continue;
                if (ar.IsMAXXNull() || ar.IsMINXNull() || ar.IsMAXYNull() || ar.IsMINYNull()) continue;

                //for each area of dataset in comming above arrayid.
                foreach (dsWCImage.AreaPointRow apr in arrard)
                {
                    if (apr.X >= ar.MINX && apr.X <= ar.MAXX && apr.Y >= ar.MINY && apr.Y <= ar.MAXY)
                    {
                        //gp.IsVisible(apr.X,apr.Y))
                        //this function check the point for selected area
                        if (IsPointInSideForAll(apr.X, apr.Y, currentAreaRow.ID, currentAreaRow))
                        {
                            decimal decareaid = Convert.ToDecimal(ar.ID);
                            DataRow[] arrdrcat = dsImage1.MapArea.Select("AreaID=" + decareaid);
                            foreach (DataRow drrow in arrdrcat)
                            {
                                if (!strcatviewimgassid.Contains("," + drrow["MapID"].ToString() + ","))
                                    strcatviewimgassid += "," + drrow["MapID"];
                            }
                            break;
                        }
                    }
                }
            }
            strcatviewimgassid += GetAllPartsPrintData(dsImage1, currentAreaRow);
            return strcatviewimgassid; //returns partid.
        }

        public bool IsPointInSideForAll(int x, int y, Decimal areaID, dsWCImage.AreaRow currentAreaRow)
        {
            //getting areapoint rows for particlar area areaid
            //DataRow[] arrdrs = ds.AreaPoint.Select("AreaID=" + areaID);
            DataRow[] arrdrs = currentAreaRow.GetChildRows("Area_AreaPoint");
            //if (arrdrs == null || arrdrs.Length == 0) return false;
            //dsWCImage.AreaPointRow[] arrareapt = (dsWCImage.AreaPointRow)currentAreaRow.GetChildRows("Area_AreaPoint");
            dsWCImage.AreaPointRow[] arrareapt = (dsWCImage.AreaPointRow[])arrdrs; //array of area point.
            bool blncontains = false;

            //This loop check the area point with comming points in function to return true/false. 
            for (int i = 0, j = arrareapt.Length - 1; i < arrareapt.Length; j = i++)
            {

                if (((arrareapt[i].X <= x && x < arrareapt[j].X) || (arrareapt[j].X <= x && x < arrareapt[i].X))
                    && y < (arrareapt[j].Y - arrareapt[i].Y) * (x - arrareapt[i].X) / (arrareapt[j].X - arrareapt[i].X) + arrareapt[i].Y)
                {
                    blncontains = !blncontains;
                    //break;
                }
            }
            return blncontains; //returns the bool value.
        }

        private string GetAllPartsPrintData(dsWCImage dsImage1, dsWCImage.AreaRow currentAreaRow)
        {
            string strcatviewimgassid = "";
            //This loop contineous to get area of Selected Area.
            foreach (dsWCImage.AreaRow ar in dsImage1.Area)
            {
                if (ar.RowState == DataRowState.Deleted) continue;
                if (ar.IsMAXXNull() || ar.IsMINXNull() || ar.IsMAXYNull() || ar.IsMINYNull()) continue;
                //Getting child row of commin current area row.
                foreach (dsWCImage.AreaPointRow apr in currentAreaRow.GetChildRows("Area_AreaPoint"))
                {
                    if (apr.X >= ar.MINX && apr.X <= ar.MAXX && apr.Y >= ar.MINY && apr.Y <= ar.MAXY)
                    {
                        //this function check the points for selected area
                        if (IsPointInSideFordata(apr.X, apr.Y, ar.ID, dsImage1))
                        {
                            decimal decareaid = Convert.ToDecimal(ar.ID);
                            //Getting image assembly ID.
                            DataRow[] arrdrcat = dsImage1.MapArea.Select("AreaID=" + decareaid);
                            foreach (DataRow drrow in arrdrcat)
                            {
                                if (!strcatviewimgassid.Contains("," + drrow["MapID"].ToString() + ","))
                                    strcatviewimgassid += "," + drrow["MapID"];
                            }
                            break;
                        }
                    }
                }
            }
            return strcatviewimgassid; //returns partid.
        }

        public bool IsPointInSideFordata(int x, int y, Decimal areaID, dsWCImage dsImage1)
        {
            //getting areapoint rows for particlar area areaid
            DataRow[] arrdrs = dsImage1.AreaPoint.Select("AreaID=" + areaID);
            if (arrdrs == null || arrdrs.Length == 0) return false;

            dsWCImage.AreaPointRow[] arrareapt = (dsWCImage.AreaPointRow[])arrdrs; //object of area point
            bool blncontains = false;
            //This loop check the area point with comming points in function to return true/false. 
            for (int i = 0, j = arrareapt.Length - 1; i < arrareapt.Length; j = i++)
            {
                if (((arrareapt[i].X <= x && x < arrareapt[j].X) || (arrareapt[j].X <= x && x < arrareapt[i].X))
                        && y < (arrareapt[j].Y - arrareapt[i].Y) * (x - arrareapt[i].X) / (arrareapt[j].X - arrareapt[i].X) + arrareapt[i].Y)
                {
                    blncontains = !blncontains;

                }
                //if (((arrareapt[i].X <= x && x < arrareapt[j].X) || (arrareapt[j].X <= x && x < arrareapt[i].X))

            }
            return blncontains;//returns the bool value.
        }

        public List<AccidentalOrderInfo> AccidentalAssemblyOrder(int ID, int ModelId, string iCountryID, int LngId, string UserID)
        {
            List<AccidentalOrderInfo> accorderinfo = new List<AccidentalOrderInfo>();

            var userid_en = DecryptAes(UserID);
            Int32 userids = Convert.ToInt32(userid_en);

            var country_en = DecryptAes(iCountryID);
            Int32 countrys = Convert.ToInt32(country_en);




            int UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select usertype from tbl_user where ID=" + userids));
            if (UserType == 0)
                UserType = 1;
            AddToOrderListData objdata = new AddToOrderListData();
            // DataTable dtOrder = objdata.FillGrd(ID, ModelId, iCountryID, LngId, UserType);
            CommanParametrs Cps = new CommanParametrs();
            Cps.Add(new CommanParameter("@ID", ID, DbType.Int32));
            Cps.Add(new CommanParameter("@ModelId", ModelId, DbType.Int32));
            Cps.Add(new CommanParameter("@CountryID", countrys, DbType.Int32));
            Cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            Cps.Add(new CommanParameter("@uType", UserType, DbType.Int32, 0));
            DataTable dtOrder = maincls.DbCon.GetProcTable("PROC_ADDTOORDERLIST1", Cps, "tbl_part");
            foreach (DataRow dr in dtOrder.Rows)
            {
                AccidentalOrderInfo accorder = new AccidentalOrderInfo();
                accorder.ID = Convert.ToInt32(dr["ID"]);
                accorder.AssemblyID = Convert.ToInt32(dr["ASSEMBLYID"]) == 0 ? 0 : Convert.ToInt32(dr["ASSEMBLYID"]);
                accorder.PartNo = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                accorder.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                accorder.NQTY = Convert.ToInt32(dr["NQTY"]) == 0 ? 0 : Convert.ToInt32(dr["NQTY"]);
                accorder.Price = Convert.ToString(dr["PRICE1"]) == "0" ? "0" : Convert.ToString(dr["PRICE1"]);
                accorder.aGroupID = Convert.ToString(dr["AGROUPID"]) == "" ? "0" : Convert.ToString(dr["AGROUPID"]);//MUSA
                //accorder.aGroupID ="0";//Mahindra & Mahindra
                accorder.Servicable = Convert.ToString(dr["SERVICEABLE"]) == "" ? "" : Convert.ToString(dr["SERVICEABLE"]);//"S"; 
                accorderinfo.Add(accorder);
            }
            return accorderinfo.ToList();
        }

        public class ProcessImage
        {

            //Defining lid value to empty
            public string lid = string.Empty;

            public ProcessImage()
            {
                ////Put the Assembly Image in Bitimage image object
                //Bitmap orignalImage = (Bitmap)HttpContext.Current.Session["AssImage"];
                //Bitmap coloredImage = (Bitmap)HttpContext.Current.Session["AssOrImage"];
                //decimal zoomFactor = Convert.ToDecimal(Request.QueryString["factor"]);

                //Color partSelectionColor = (Color)HttpContext.Current.Session["AssPartHigh"];
                //bool reDrawLabel = Convert.ToBoolean(HttpContext.Current.Session["Redrawlabel"]);

                //Bitmap bitMapImage = new Bitmap(20, 20); ;
                //string strAssembly = ConfigurationManager.AppSettings["ASSEMBLY"];


                ////Calling the constructort of Assembly Class
                //if (strAssembly.ToUpper() == "PERODUA")
                //{
                //    AssemblyP a = new AssemblyP(orignalImage, coloredImage, zoomFactor, partSelectionColor, reDrawLabel);
                //    //check whether image is clicked or the Datagrid button is clicked 
                //    //if the button is clicked call the GetImageByAssPartID function
                //    try
                //    {
                //        if (Request.QueryString["Type"] == "Part")
                //        {

                //            if (Convert.ToString(Request.QueryString["PartId"]) == "-999")
                //            {
                //                Session["AssPartId"] = null;
                //            }
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID(Request.QueryString["PartId"]);
                //            lid = Convert.ToString(Request.QueryString["PartId"]);
                //            Session["PreLink"] = lid;
                //            if (lid == string.Empty) lid = "-1";
                //        }
                //        //check whether a lebel is clicked  
                //        else
                //        {
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                //            lid = Convert.ToString(a.GetLabelText());
                //        }
                //    }
                //    catch
                //    {
                //        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                //        lid = Convert.ToString(a.GetLabelText());
                //    }

                //}
                //else if (strAssembly.ToUpper() == "MAHINDRA")
                //{
                //    AssemblyM a = new AssemblyM(orignalImage, zoomFactor);
                //    //check whether image is clicked or the Datagrid button is clicked 
                //    //if the button is clicked call the GetImageByAssPartID function
                //    try
                //    {
                //        if (Request.QueryString["Type"] == "Part")
                //        {

                //            if (Convert.ToString(Request.QueryString["PartId"]) == "-999")
                //            {
                //                Session["AssPartId"] = null;
                //            }
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID(Request.QueryString["PartId"]);


                //            lid = Convert.ToString(Request.QueryString["PartId"]);
                //            Session["PreLink"] = lid;
                //            if (lid == string.Empty) lid = "-1";
                //        }
                //        //check whether a lebel is clicked  
                //        else
                //        {
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                //            lid = Convert.ToString(a.GetLabelText());
                //        }
                //    }
                //    catch
                //    {
                //        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                //        lid = Convert.ToString(a.GetLabelText());
                //    }

                //}
                //else if (strAssembly.ToUpper() == "NAVISTAR")
                //{
                //    AssemblyN a = new AssemblyN(orignalImage, zoomFactor);

                //    //check whether image is clicked or the Datagrid button is clicked 
                //    //if the button is clicked call the GetImageByAssPartID function
                //    try
                //    {
                //        if (Request.QueryString["Type"] == "Part")
                //        {

                //            if (Convert.ToString(Request.QueryString["PartId"]) == "-999")
                //            {
                //                Session["AssPartId"] = null;
                //            }
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID(Request.QueryString["PartId"]);


                //            lid = Convert.ToString(Request.QueryString["PartId"]);
                //            Session["PreLink"] = lid;
                //            if (lid == string.Empty) lid = "-1";
                //        }
                //        //check whether a lebel is clicked  
                //        else
                //        {
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                //            lid = Convert.ToString(a.GetLabelText());
                //        }
                //    }
                //    catch
                //    {
                //        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                //        lid = Convert.ToString(a.GetLabelText());
                //    }

                //}
                //else if (strAssembly.ToUpper() == "MUSA")
                //{
                //    AssemblyMUSA a = new AssemblyMUSA(orignalImage, coloredImage, zoomFactor, partSelectionColor, reDrawLabel);
                //    // a.updatelabelwith();
                //    //check whether image is clicked or the Datagrid button is clicked 
                //    //if the button is clicked call the GetImageByAssPartID function
                //    try
                //    {
                //        if (Request.QueryString["Type"] == "Part")
                //        {

                //            if (Convert.ToString(Request.QueryString["PartId"]) == "-999")
                //            {
                //                Session["AssPartId"] = null;
                //            }
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID(Request.QueryString["PartId"]);


                //            lid = Convert.ToString(Request.QueryString["PartId"]);
                //            Session["PreLink"] = lid;
                //            if (lid == string.Empty) lid = "-1";
                //        }
                //        //check whether a lebel is clicked  
                //        else
                //        {
                //            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                //            lid = Convert.ToString(a.GetLabelText());
                //        }
                //    }
                //    catch
                //    {
                //        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                //        lid = Convert.ToString(a.GetLabelText());
                //    }
                //}
                ///*Code Start For Croping Image*/
                //if (Request.QueryString["CType"] == "Crop")
                //{
                //    string sval = Request.QueryString["h"];

                //    if (strAssembly.ToUpper() == "PERODUA")
                //    {
                //        AssemblyP a = new AssemblyP(orignalImage, coloredImage, zoomFactor, partSelectionColor, reDrawLabel);

                //        SaveCroppedImage((Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0)), sval);
                //    }
                //    if (strAssembly.ToUpper() == "MAHINDRA")
                //    {
                //        AssemblyM a = new AssemblyM(orignalImage, zoomFactor);
                //        SaveCroppedImage((Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0)), sval);
                //    }
                //    if (strAssembly.ToUpper() == "NAVISTAR")
                //    {
                //        AssemblyN a = new AssemblyN(orignalImage, zoomFactor);
                //        SaveCroppedImage((Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0)), sval);
                //    }
                //    if (strAssembly.ToUpper() == "MUSA")
                //    {
                //        AssemblyMUSA a = new AssemblyMUSA(orignalImage, coloredImage, zoomFactor, partSelectionColor, reDrawLabel);
                //        SaveCroppedImage((Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0)), sval);
                //    }


                //    return;
                //}
                ///*End Croping*/

                //ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                //EncoderParameters Params = new EncoderParameters(1);
                //Params.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                //Response.ContentType = Info[1].MimeType;

                ////bitMapImage.Save(Response.OutputStream, ImageFormat.Png);
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                //{
                //    bitMapImage.Save(ms, Info[1], Params);
                //    //bitMapImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                //    ms.WriteTo(Context.Response.OutputStream);
                //}

            }


            public byte[] getImageArray(decimal zoomFactor, byte[] imagedata, byte[] cImagedata, Color partSelColor, bool reDrawLabel, string partId)
            {
                Bitmap orignalImage = GetImage(imagedata);
                Bitmap coloredImage = GetImage(cImagedata);
                Bitmap bitMapImage = new Bitmap(20, 20); ;
                string strAssembly = ConfigurationManager.AppSettings["ASSEMBLY"];

                //Calling the constructort of Assembly Class
                if (strAssembly.ToUpper() == "PERODUA")
                {
                    AssemblyP a = new AssemblyP(orignalImage, coloredImage, zoomFactor, partSelColor, partSelColor, reDrawLabel);
                    //check whether image is clicked or the Datagrid button is clicked 
                    //if the button is clicked call the GetImageByAssPartID function
                    try
                    {
                        if (!string.IsNullOrEmpty(partId))
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID(partId);
                        }
                        //check whether a lebel is clicked  
                        else
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                        }
                    }
                    catch
                    {
                        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                    }

                }
                else if (strAssembly.ToUpper() == "MAHINDRA")
                {
                    AssemblyM a = new AssemblyM(orignalImage, coloredImage, zoomFactor, partSelColor, partSelColor, reDrawLabel);
                    //check whether image is clicked or the Datagrid button is clicked 
                    //if the button is clicked call the GetImageByAssPartID function
                    try
                    {
                        if (!string.IsNullOrEmpty(partId))
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID(partId);
                        }
                        //check whether a lebel is clicked  
                        else
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                        }
                    }
                    catch
                    {
                        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                    }

                }
                else if (strAssembly.ToUpper() == "NAVISTAR")
                {
                    AssemblyN a = new AssemblyN(orignalImage, coloredImage, zoomFactor, partSelColor, partSelColor, reDrawLabel);

                    //check whether image is clicked or the Datagrid button is clicked 
                    //if the button is clicked call the GetImageByAssPartID function
                    try
                    {
                        if (!string.IsNullOrEmpty(partId))
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID(partId);
                        }
                        //check whether a lebel is clicked  
                        else
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                        }
                    }
                    catch
                    {
                        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                    }

                }
                else if (strAssembly.ToUpper() == "MUSA")
                {
                    AssemblyMUSA a = new AssemblyMUSA(orignalImage, coloredImage, zoomFactor, partSelColor, partSelColor, reDrawLabel);
                    // a.updatelabelwith();
                    //check whether image is clicked or the Datagrid button is clicked 
                    //if the button is clicked call the GetImageByAssPartID function
                    try
                    {
                        if (!string.IsNullOrEmpty(partId))
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID(partId);
                        }
                        //check whether a lebel is clicked  
                        else
                        {
                            bitMapImage = (Bitmap)a.GetImageByAssPartID("99999999");
                        }
                    }
                    catch
                    {
                        bitMapImage = (Bitmap)a.GetNewImage(Convert.ToInt32(0), Convert.ToInt32(0));
                    }
                }

                ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters Params = new EncoderParameters(1);
                Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //bitMapImage.Save(Response.OutputStream, ImageFormat.Png);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    bitMapImage.Save(ms, Info[1], Params);
                    return ms.ToArray();
                }

            }

            private Bitmap GetImage(byte[] imagedata)
            {
                ImageConverter ic = new ImageConverter();
                System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(imagedata);
                return new Bitmap(img);


            }
        }

        public bool ThumbnailCallback()
        {
            return true;
        }

        private byte[] getImgArray(byte[] ImgBytes, bool isThumbnail)
        {
            byte[] bytBuffer;
            MemoryStream stmMemo = new MemoryStream(ImgBytes, true);
            stmMemo.Write(ImgBytes, 0, ImgBytes.Length);

            System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            // create an image object, using the filename we just retrieved
            System.Drawing.Image image = System.Drawing.Image.FromStream(stmMemo);
            System.Drawing.Image thumbnailImage = null;
            if (isThumbnail)
                thumbnailImage = image.GetThumbnailImage(image.Width / 3, image.Height / 3, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            else
                thumbnailImage = image.GetThumbnailImage(image.Width, image.Height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnailImage);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            if (isThumbnail)
                graphic.DrawImage(image, 0, 0, image.Width / 3, image.Height / 3);
            else
                graphic.DrawImage(image, 0, 0, image.Width, image.Height);

            graphic.Dispose();

            Info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            MemoryStream ms = new MemoryStream();
            thumbnailImage.Save(ms, Info[1], encoderParameters);//System.Drawing.Imaging.ImageFormat.Gif);//
            if (Convert.ToString(ConfigurationManager.AppSettings["saveimg"]).ToUpper() == "YES") thumbnailImage.Save(@"c:\testimg\image.tif", Info[1], encoderParameters);

            bytBuffer = (ms.ToArray() == null ? null : (byte[])ms.ToArray());
            thumbnailImage.Dispose();

            return bytBuffer;
        }

        private byte[] getImgArrayLarge(byte[] ImgBytes, bool isThumbnail)
        {
            byte[] bytBuffer;
            MemoryStream stmMemo = new MemoryStream(ImgBytes, true);
            stmMemo.Write(ImgBytes, 0, ImgBytes.Length);

            System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            // create an image object, using the filename we just retrieved
            System.Drawing.Image image = System.Drawing.Image.FromStream(stmMemo);
            System.Drawing.Image thumbnailImage = null;
            if (isThumbnail)
                thumbnailImage = image.GetThumbnailImage(image.Width, image.Height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            else
                thumbnailImage = image.GetThumbnailImage(image.Width, image.Height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnailImage);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            if (isThumbnail)
                graphic.DrawImage(image, 0, 0, image.Width, image.Height);
            else
                graphic.DrawImage(image, 0, 0, image.Width, image.Height);

            graphic.Dispose();

            Info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            MemoryStream ms = new MemoryStream();
            thumbnailImage.Save(ms, Info[1], encoderParameters);//System.Drawing.Imaging.ImageFormat.Gif);//
            if (Convert.ToString(ConfigurationManager.AppSettings["saveimg"]).ToUpper() == "YES") thumbnailImage.Save(@"c:\testimg\image.tif", Info[1], encoderParameters);

            bytBuffer = (ms.ToArray() == null ? null : (byte[])ms.ToArray());
            thumbnailImage.Dispose();

            return bytBuffer;
        }

        public bool IsPointInSide(int x, int y, Decimal areaID)
        {
            //getting areapoint rows for particlar area areaid
            DataRow[] drs = c.dataSetCatalogD1.PartArea.FindByID(areaID).GetChildRows("AreaAreaPoints");
            if (drs == null || drs.Length == 0) return false;
            DatasetCommon.PartAreaPointRow[] t = (DatasetCommon.PartAreaPointRow[])drs;
            bool contains = false;
            for (int i = 0, j = t.Length - 1; i < t.Length; j = i++)
            {
                if (((t[i].XPoint <= x && x < t[j].XPoint) || (t[j].XPoint <= x && x < t[i].XPoint))
                    && y < (t[j].YPoint - t[i].YPoint) * (x - t[i].XPoint) / (t[j].XPoint - t[i].XPoint) + t[i].YPoint)
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public string GetAssId(int xPoint, int yPoint, decimal zoomFactor, string ModelId, string sType)
        {
            c.LabelIds = "-1"; c.SelectedIds = "-1";

            IllustrationData illustrationData = new IllustrationData();

            //getting x and y corrdinate
            int x1 = xPoint;
            int y1 = yPoint;
            //getting zoom value
            decimal fact = zoomFactor;
            x1 = Convert.ToInt32(x1 / fact);
            y1 = Convert.ToInt32(y1 / fact);
            int lblFact = Convert.ToInt32(20 / fact);
            bool lclicked = false;
            //string lid = "";
            //getting all labels
            DataSetImg.LabelsDataTable labels = c.dsImage.Labels;// (DataSetImg.LabelsDataTable)HttpContext.Current.Session["Labels"];
            try
            {
                DataRow[] arrdrrLables = null;
                if (ConfigurationManager.AppSettings["ASSEMBLY"].ToString().ToUpper() == "PERODUA" || ConfigurationManager.AppSettings["ASSEMBLY"].ToString().ToUpper() == "MUSA")
                    arrdrrLables = labels.Select("" + x1 + " >= XPoint And " + x1 + "<= XPoint + LWidth And " + y1 + " >= YPoint And " + y1 + "<= YPoint + LHeight");
                else
                    arrdrrLables = labels.Select("" + x1 + " >= XPoint And " + x1 + "<= XPoint + (LWIDTH/ " + fact + " ) And " + y1 + " >= YPoint And " + y1 + "<= YPoint + (LWIDTH/ " + fact + " )");

                foreach (DataRow drr in arrdrrLables)
                {
                    //setting label click(lclick) true
                    lclicked = true;
                    c.LabelIds = Convert.ToString(drr["Text"]);
                    c.SelectedIds = Convert.ToString(drr["Assemblypartid"]);
                }

                //if label is not clicked 
                if (!lclicked)
                {
                    DataTable dtAssRef = c.dsImage.Tables["AssReference"];// ((DataSetImg)HttpContext.Current.Session["dsImage"]).Tables["AssReference"];
                    if (dtAssRef.Rows.Count > 0)
                    {
                        DataRow[] arrdrrRef = dtAssRef.Select("" + x1 + " >= XPoint And " + x1 + "<= XPoint + LWidth And " + y1 + " >= YPoint And " + y1 + "<= YPoint + LHeight");
                        foreach (DataRow drrow in arrdrrRef)
                        {
                            decimal assid = Convert.ToDecimal(drrow["RAssemblyid"]);
                            decimal assPartid = Convert.ToDecimal(drrow["AssemblyPartid"]);
                            //getting datetime from session
                            DateTime dteSeldate = Convert.ToDateTime(HttpContext.Current.Session["SelDate"]);
                            string strSelDate = MainClass.GetDateString(dteSeldate);
                            ////creating object of comman parameter class
                            //Catalog.WebArch.CommanParametrs cps = new Catalog.WebArch.CommanParametrs();
                            ////Assign value in comman comman parameter
                            //cps.Add(new Catalog.WebArch.CommanParameter("@ModelID", Convert.ToInt32(ModelId)));
                            //cps.Add(new Catalog.WebArch.CommanParameter("@SEDate", dteSeldate, DbType.Date));
                            //cps.Add(new Catalog.WebArch.CommanParameter("@AssID", assid, DbType.Int32));
                            //object objAggID = MainClass.DbCon.ExecuteScaler("PROC_ILLGETAGGID", cps, "PAggID");
                            object objAggID = illustrationData.getApplicable(Convert.ToInt32(ModelId), assid, dteSeldate);
                            if (objAggID == null)//checking for null value
                            {
                                //displaying error message when no record found.
                                //return Resources.Resource.NoRecordFoundAlertMessage;
                                var res = new { id = 0, asspartid = 0, msg = "No Record Found." };
                                return res.ToJson();
                            }
                            else
                                HttpContext.Current.Session["AggregateID"] = objAggID;

                            string strCond = string.Empty;
                            if (sType == "Basket" || sType == "Basket" || sType == "Part")
                                strCond = "YES";

                            ////Creating object of comman parameter
                            //Catalog.WebArch.CommanParametrs cpars = new Catalog.WebArch.CommanParametrs();
                            ////Assigning value in comman parameter
                            //cpars.Add(new Catalog.WebArch.CommanParameter("@ModelID", Convert.ToInt32(ModelId)));
                            //cpars.Add(new Catalog.WebArch.CommanParameter("@SEDate", dteSeldate, DbType.Date));
                            //cpars.Add(new Catalog.WebArch.CommanParameter("@AssID", assid));
                            //cpars.Add(new Catalog.WebArch.CommanParameter("@AggID", Convert.ToString(HttpContext.Current.Session["AggregateID"])));
                            //cpars.Add(new Catalog.WebArch.CommanParameter("@PageType", strCond, DbType.String, string.Empty));
                            //DataTable dtAss = MainClass.DbCon.GetProcTable("PROC_ILLGETNAVASSREFCLICK", cpars, "Assembly");
                            DataTable dtAss = illustrationData.RefAssemblyData(Convert.ToInt32(ModelId), Convert.ToInt32(objAggID), assid, dteSeldate, strCond);
                            // setting current Assemblies of the Aggregate in the session.
                            HttpContext.Current.Session["Assemblys"] = dtAss;
                            var s = new { id = assid, asspartid = assPartid, msg = "", LebelText = "" };

                            return s.ToJson();//  assid.ToString() + "|" + assPartid.ToString();
                        }
                    }

                    //getting dataset                
                    decimal assPartID = -1;
                    DataRow[] arrdrrPoints = c.dataSetCatalogD1.PartArea.Select("MaxX > " + x1 + " And MinX < " + x1 + " And MaxY > " + y1 + " And MinY < " + y1 + "");
                    foreach (DatasetCommon.PartAreaRow dr in arrdrrPoints)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;
                        if (dr.IsMaxXNull() || dr.IsMinXNull() || dr.IsMaxYNull() || dr.IsMinYNull()) continue;
                        //finding x and y exist in area or not
                        if (IsPointInSide(x1, y1, Convert.ToDecimal(dr["ID"])))
                        {
                            //if exist then getting areaid
                            decimal areaid = Convert.ToDecimal(dr["ID"]);
                            //getting all assemblypart against that area
                            DataRow[] drs = c.dataSetCatalogD1.AssPartArea.Select("PartAreaID=" + areaid);
                            foreach (DataRow drP in drs)
                            {
                                //setting id into variable
                                assPartID = Convert.ToDecimal(drP["AssemblyPartID"]);
                                c.LabelIds = c.dataSetCatalogD1.AssemblyPart.FindByID(assPartID).LabelText;
                                c.SelectedIds = Convert.ToString(assPartID);

                                lclicked = true;
                                break;
                            }
                        }
                    }

                    //For unselect the parts in Image
                    if (lclicked == false)
                    {
                        try
                        {
                            c.LabelIds = "";
                            c.SelectedIds = "-1";
                        }
                        catch { }
                    }
                }
            }
            catch { }

            //setting link value from session
            string strprelink = Convert.ToString(HttpContext.Current.Session["PreLink"]);

            //DataTable dtkit = MainClass.DbCon.GetTable("Select distinct childid from tbl_asskitmapping where parentid in (" + c.SelectedIds + ") order by 1", "tblkit");
            DataTable dtkit = illustrationData.GetChildPartIds(c.SelectedIds);
            bool isKit = dtkit.Rows.Count > 0 ? true : false;
            foreach (DataRow dr in dtkit.Rows)
            {
                c.SelectedIds += "," + Convert.ToString(dr["childid"]);
                var reslbl = from l in c.dsImage.Labels.AsEnumerable()
                             where l.Field<decimal>("AssemblyPartID").Equals(Convert.ToDecimal(dr["childid"]))
                             select new { labeltext = l.Field<string>("Text") };
                foreach (var lbl in reslbl)
                {
                    c.LabelIds += "," + lbl.labeltext;
                }
            }
            var resf = new { id = 0, AsspartId = c.SelectedIds, msg = "", LebelText = c.LabelIds };

            return c.LabelIds;// resf.ToJson(); //lid;
        }

        private byte[] getBImgArray(string sname)
        {
            Bitmap bmp = new Bitmap(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Images/blank_img.jpg"));
            DrawString(bmp, sname);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            //  bmp.Dispose();
            Byte[] bytes = ms.ToArray();


            return bytes;
        }

        private void DrawString(Bitmap bmp, string printname)
        {
            StringFormat format =
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(3, 3, 147, 147);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            string name = DesignText(printname);
            g.DrawString(name.ToUpper(), new System.Drawing.Font("Arial", 10, FontStyle.Bold), new SolidBrush(System.Drawing.ColorTranslator.FromHtml("#004f63")), rec, format);// Convert.ToInt64(width * (0.10)), Convert.ToInt64(height * 0.2)


        }

        private string DesignText(string text)
        {
            string name = string.Empty;
            string[] totalstring = text.Split(' ');
            if (totalstring.Count() > 1)
            {
                for (int i = 0; i < totalstring.Count(); i++)
                {
                    int f1 = totalstring[i].Count();
                    if (f1 > 10)
                    {
                        name += totalstring[i] + (totalstring.Count() - 1 > i ? "\n" : "");
                    }
                    else
                    {
                        if (totalstring.Count() - 1 > i)
                        {
                            if (totalstring[i + 1].Count() + f1 > 10)
                            {
                                name += totalstring[i] + (totalstring.Count() - 1 > i ? "\n" : "");
                            }
                            else
                            {
                                name += totalstring[i] + " ";
                            }
                        }
                        else
                        {
                            name += totalstring[i];
                        }
                    }
                }
            }
            else
            {
                name = text;
            }
            return name;
        }

        public List<CategoryImage> getThumbnail(int Id)
        {

            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string strSql = @"Select AImage from tbl_Assembly where id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                CategoryImage cm = new CategoryImage();
                cm.Id = Id;
                // categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                cm.ImageByte = objImage == null ? null : getImgArray((byte[])objImage, true);

                catImage.Add(cm);
                /*
                byte[] bytBuffer;
                bytBuffer = (byte[])objImage;
                MemoryStream stmMemo = new MemoryStream(bytBuffer, true);
                stmMemo.Write(bytBuffer, 0, bytBuffer.Length);

                System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
                Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                // create an image object, using the filename we just retrieved
                System.Drawing.Image image = System.Drawing.Image.FromStream(stmMemo);


                // create the actual thumbnail image
                System.Drawing.Image thumbnailImage = image.GetThumbnailImage(image.Width / 3, image.Height / 3, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnailImage);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.DrawImage(image, 0, 0, image.Width / 3, image.Height / 3);
                graphic.Dispose();

                Info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //thumbnailImage.Save(System.Web.HttpContext.Current.Response.OutputStream, Info[1], encoderParameters);



                MemoryStream ms = new MemoryStream();
                thumbnailImage.Save(ms, Info[1], encoderParameters);//System.Drawing.Imaging.ImageFormat.Gif);//
                categoryImage.ImageByte = (ms.ToArray() == null ? null : (byte[])ms.ToArray());
                thumbnailImage.Dispose();
                */

            }
            catch (Exception ex)
            {
                throw new Exception("LoadAssemblyImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> LoadAssemblyImage(Int32 Id)
        {
            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string strSql = @"Select AImage from tbl_Assembly where id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);


                //DataTable dtPart = GetIllustrationPartDetails(-1, Id, "29-Jul-2014", 0, 1, 1030);             
                //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                if (objImage != null)
                {
                    CategoryImage cm = new CategoryImage();
                    cm.Id = Id;
                    int userid = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                        LngId = Convert.ToInt32("0" + Convert.ToString(HttpContext.Current.Session["LngId"])),
                        ModelId = Convert.ToInt32("0" + Convert.ToString(HttpContext.Current.Session["ModelId"])),
                        AssemblyId = Id, srVinNo = 0;
                    string StartDate = DateTime.Now.ToString("dd-MMM-yyyy");
                    if (ModelId > 0)
                    {
                        //GetIllustrationPartDetails(ModelId, AssemblyId, StartDate, srVinNo, LngId == 0 ? 1 : LngId, userid);
                        //List<IllustrationInfo> plist=
                        //IllustrationPartList(userid, ModelId, AssemblyId, StartDate, srVinNo, LngId, 0);
                        ProcessImage pimage = new ProcessImage();
                        byte[] bytBuffer = pimage.getImageArray(1, (byte[])objImage, (byte[])objImage, Color.Red, true, "-1,69833");
                        cm.ImageByte = objImage == null ? null : getImgArray(bytBuffer, true);
                    }
                    else
                        cm.ImageByte = objImage == null ? null : getImgArray((byte[])objImage, true);
                    catImage.Add(cm);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("LoadAssemblyImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> LoadAccidentalImage(Int32 Id)
        {
            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string strSql = @"Select ViewImage from tbl_CategoryViewImage where ID=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                CategoryImage cm = new CategoryImage();
                cm.Id = Id;
                //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                cm.ImageByte = objImage == null ? null : getImgArray((byte[])objImage, false);
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("LoadAccidentalImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> LoadCategoryTypeImage(Int32 Id)
        {
            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string strSql = @"Select CatTypeImage from tbl_categorytype where id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                CategoryImage cm = new CategoryImage();
                cm.Id = Id;
                //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                cm.ImageByte = objImage == null ? null : getImgArray((byte[])objImage, false);
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("LoadCategoryImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> getAcessoryCategoryImage(int Id)
        {

            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string strSql = @"select CATM.AIMAGE from tbl_CatAccTypeMapping CATM left outer join tbl_AccType A on CATM.AccTypeID=A.ID where CATM.ID=@Id order by A.Priority";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);

                CategoryImage cm = new CategoryImage();
                cm.Id = Id;
                //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                cm.ImageByte = objImage == null ? null : getImgArray((byte[])objImage, false);
                /*
                byte[] bytBuffer;
                bytBuffer = (byte[])objImage;
                MemoryStream stmMemo = new MemoryStream(bytBuffer, true);
                stmMemo.Write(bytBuffer, 0, bytBuffer.Length);

                System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
                Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                // create an image object, using the filename we just retrieved
                System.Drawing.Image image = System.Drawing.Image.FromStream(stmMemo);


                // create the actual thumbnail image
                System.Drawing.Image thumbnailImage = image.GetThumbnailImage(image.Width / 3, image.Height / 3, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnailImage);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.DrawImage(image, 0, 0, image.Width / 3, image.Height / 3);
                graphic.Dispose();

                Info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters;
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //thumbnailImage.Save(System.Web.HttpContext.Current.Response.OutputStream, Info[1], encoderParameters);



                MemoryStream ms = new MemoryStream();
                thumbnailImage.Save(ms, Info[1], encoderParameters);//System.Drawing.Imaging.ImageFormat.Gif);//
                categoryImage.ImageByte = (ms.ToArray() == null ? null : (byte[])ms.ToArray());
                thumbnailImage.Dispose();
                */
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("LoadAssemblyImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> getAcessoryCategoryPartImage(int Id)
        {

            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                // string strSql = @"select PAttachment from tbl_PartAttachment where Id=@Id and ATTACHMENTTYPE=4";
                string strSql = @"select pa.PAttachment from tbl_PartAttachment pa left outer join tbl_part p on pa.PartID=p.ID where pa.ATTACHMENTTYPE=4 and pa.IsDefault=0 and p.ID=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                CategoryImage cm = new CategoryImage();
                cm.Id = Id;
                //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                cm.ImageByte = objImage == null ? null : getImgArrayLarge((byte[])objImage, false);

                //byte[] bytBuffer;
                //bytBuffer = (byte[])objImage;
                //MemoryStream stmMemo = new MemoryStream(bytBuffer, true);
                //stmMemo.Write(bytBuffer, 0, bytBuffer.Length);

                //System.Drawing.Imaging.ImageCodecInfo[] Info = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                //System.Drawing.Imaging.EncoderParameters Params = new System.Drawing.Imaging.EncoderParameters(1);
                //Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //// create an image object, using the filename we just retrieved
                //System.Drawing.Image image = System.Drawing.Image.FromStream(stmMemo);


                //// create the actual thumbnail image
                //System.Drawing.Image thumbnailImage = image.GetThumbnailImage(image.Width / 1, image.Height / 1, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                //System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnailImage);

                //graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //graphic.SmoothingMode = SmoothingMode.HighQuality;
                //graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //graphic.CompositingQuality = CompositingQuality.HighQuality;

                //graphic.DrawImage(image, 0, 0, image.Width / 1, image.Height / 1);
                //graphic.Dispose();

                //Info = ImageCodecInfo.GetImageEncoders();
                //EncoderParameters encoderParameters;
                //encoderParameters = new EncoderParameters(1);
                //encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                //MemoryStream ms = new MemoryStream();
                //thumbnailImage.Save(ms, Info[1], encoderParameters);//System.Drawing.Imaging.ImageFormat.Gif);//
                //categoryImage.ImageByte = (ms.ToArray() == null ? null : (byte[])ms.ToArray());
                //thumbnailImage.Dispose();

                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("getAcessoryCategoryPartImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> GetAssemblyHotsPotImage(Int32 AssPartId, string keycode, decimal zoomfactor, string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, bool onImage = false, string device = "iphone")
        {

            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                var userid_en = DecryptAes(UserId);
                Int32 userids = Convert.ToInt32(userid_en);

                string strSql = @"Select AImage from tbl_Assembly where id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", AssemblyID, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                byte[] imageData = objImage == null ? null : (byte[])objImage;
                CategoryImage cm = new CategoryImage();
                cm.Id = AssemblyID;
                // categoryImage.ImageByte = imageData;
                DataTable dtPart = GetIllustrationPartDetails(ModelID, AssemblyID, StartDate, SrVINNO, Lngid, userids, 0);

                string val = "-1";
                if (!onImage)
                {
                    DataRow[] drs = dtPart.Select("ASSEMBLYPARTID=" + AssPartId);
                    foreach (DataRow dr in drs)
                        val += "," + Convert.ToString(drs[0]["LABELTEXT"]);
                }
                else
                {
                    string[] spoints = null;
                    int x = 0, y = 0;
                    if (!string.IsNullOrEmpty(keycode) || keycode.Length > 0)
                    {
                        spoints = keycode.Split(',');
                        x = Convert.ToInt16(spoints[0]) * 3; y = Convert.ToInt16(spoints[1]) * 3;
                    }

                    val = GetAssId(x, y, zoomfactor, ModelID.ToString(), "Part");
                }

                // categoryImage.ImageByte = pimage.getImageArray(zoomfactor, imageData, Color.Red, true, val);
                bool reDrawLabel = Convert.ToBoolean(ConfigurationManager.AppSettings["Redrawlabel"]);

                byte[] bytBuffer;
                ProcessImage pimage = new ProcessImage();
                bytBuffer = pimage.getImageArray(zoomfactor, imageData, c.cImageData, Color.Red, reDrawLabel, val);

                if (device == "iphone")
                {
                    cm.ImageByte = getImgArray(bytBuffer, Convert.ToString(ConfigurationManager.AppSettings["ASSEMBLY"]) == "MUSA" ? true : false);
                }
                else
                {
                    cm.ImageByte = getImgArrayLarge(bytBuffer, Convert.ToString(ConfigurationManager.AppSettings["ASSEMBLY"]) == "MUSA" ? true : false);
                }
                string[] sids = c.SelectedIds.Split(',');

                cm.AssPartId = AssPartId;// Convert.ToInt32(sids[sids.Length - 1]);
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("GetAssemblyHotsPotImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> GetAndroidAssemblyHotsPotImage(Int32 AssPartId, string keycode, decimal zoomfactor, string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, bool onImage = false, string device = "4")
        {

            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                var userid_en = DecryptAes(UserId);
                Int32 userids = Convert.ToInt32(userid_en);

                string strSql = @"Select AImage from tbl_Assembly where id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", AssemblyID, DbType.Int32));
                object objImage = maincls.DbCon.ExecuteScaler(strSql, cps);
                byte[] imageData = objImage == null ? null : (byte[])objImage;
                CategoryImage cm = new CategoryImage();
                cm.Id = AssemblyID;
                // categoryImage.ImageByte = imageData;
                DataTable dtPart = GetIllustrationPartDetails(ModelID, AssemblyID, StartDate, SrVINNO, Lngid, userids, 0);

                string val = "-1";
                if (!onImage)
                {
                    DataRow[] drs = dtPart.Select("ASSEMBLYPARTID=" + AssPartId);
                    foreach (DataRow dr in drs)
                        val += "," + Convert.ToString(drs[0]["LABELTEXT"]);
                }
                else
                {
                    string[] spoints = null;
                    int x = 0, y = 0;
                    if (!string.IsNullOrEmpty(keycode) || keycode.Length > 0)
                    {
                        spoints = keycode.Split(',');
                        x = Convert.ToInt16(spoints[0]) * 3; y = Convert.ToInt16(spoints[1]) * 3;
                    }

                    val = GetAssId(x, y, zoomfactor, ModelID.ToString(), "Part");
                }

                // categoryImage.ImageByte = pimage.getImageArray(zoomfactor, imageData, Color.Red, true, val);
                bool reDrawLabel = Convert.ToBoolean(ConfigurationManager.AppSettings["Redrawlabel"]);

                byte[] bytBuffer;
                ProcessImage pimage = new ProcessImage();
                bytBuffer = pimage.getImageArray(zoomfactor, imageData, c.cImageData, Color.Red, reDrawLabel, val);

                if (Convert.ToInt32(device) < 6.0)
                {
                    cm.ImageByte = getImgArray(bytBuffer, Convert.ToString(ConfigurationManager.AppSettings["ASSEMBLY"]) == "MUSA" ? true : false);
                }
                else
                {
                    cm.ImageByte = getImgArrayLarge(bytBuffer, Convert.ToString(ConfigurationManager.AppSettings["ASSEMBLY"]) == "MUSA" ? true : false);
                }
                string[] sids = c.SelectedIds.Split(',');

                cm.AssPartId = AssPartId;// Convert.ToInt32(sids[sids.Length - 1]);
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("GetAndroidAssemblyHotsPotImage", ex);
            }
            return catImage.ToList();
        }

        public List<CategoryImage> LoadCategoryImage(Int32 Id)
        {
            List<CategoryImage> catImage = new List<CategoryImage>();
            try
            {
                string name = string.Empty;
                string strSql = @"Select CatMImage,c.categoryname from tbl_categorymapping cm inner join tbl_category c on c.id=cm.categoryid where cm.id=@Id";
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@Id", Id, DbType.Int32));
                DataTable dtimage = maincls.DbCon.GetTable(strSql, cps, "tblimg");
                DataRow dr = null;
                CategoryImage cm = new CategoryImage();
                if (dtimage.Rows.Count > 0)
                {
                    dr = dtimage.Rows[0];
                    cm.Id = Id;
                    //categoryImage.ImageByte = objImage == null ? null : (byte[])objImage;
                    cm.ImageByte = dr["CatMImage"] == DBNull.Value ? getBImgArray(Convert.ToString(dr["categoryname"])) : getImgArray((byte[])dr["CatMImage"], false);
                }
                else
                {
                    cm.Id = 0;
                    cm.ImageByte = null;
                }
                catImage.Add(cm);
            }
            catch (Exception ex)
            {
                throw new Exception("LoadCategoryImage", ex);
            }
            return catImage.ToList();
        }

        public List<CountryInfo> CountryList()
        {
            List<CountryInfo> couinfo = new List<CountryInfo>();
            CountryModelData countrymodeldata = new CountryModelData();

            DataTable dtCountry = countrymodeldata.GetCountryDetail();
            foreach (DataRow dr in dtCountry.Rows)
            {
                CountryInfo cinfo = new CountryInfo();
                cinfo.ID = Convert.ToInt32(dr["ID"]);
                cinfo.CountryName = Convert.ToString(dr["COUNTRYNAME"]) == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]);
                cinfo.CurrencySymbol = Convert.ToString(dr["CURRENCYSYMBOL"]) == "" ? "" : Convert.ToString(dr["CURRENCYSYMBOL"]);
                couinfo.Add(cinfo);
            }

            return couinfo.ToList();
        }

        public List<PartInfo> partHistory(decimal ID, string modelid, string startdate, string UserId, int LngId, String type)
        {
            Int32 iCountryId = 1;

            var userid_en = DecryptAes(UserId);
            Int32 userids = Convert.ToInt32(userid_en);

            decimal decMId = 0;
            decimal decAPId = 0;
            decimal pId = 0;
            decimal decAId = 0;
            DateTime DteSearch;
            DateTime DteAP;
            int ipQty = 0;
            decimal decVId = 0;
            List<PartInfo> plist = new List<PartInfo>();
            try
            {
                if (userids > 0)
                    iCountryId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + userids));
                decAPId = ID;
                pId = Convert.ToDecimal(maincls.DbCon.ExecuteScaler("Select PartId From tbl_Assemblypart where Id=" + decAPId.ToString()));
                decAId = Convert.ToDecimal(maincls.DbCon.ExecuteScaler("Select AssemblyId From tbl_Assemblypart where Id=" + decAPId.ToString()));
                if (!string.IsNullOrEmpty(modelid))
                    decMId = Convert.ToDecimal(modelid);
                else if (modelid != null && Convert.ToString(modelid) != "" && Convert.ToString(modelid) != string.Empty)
                    decMId = Convert.ToDecimal(modelid);

                if (decMId > 0 && decAPId > 0)
                    DteAP = Convert.ToDateTime(maincls.DbCon.ExecuteScaler("Select StartDate From tbl_AssPartModel apm inner join tbl_Assemblypart ap on apm.assemblypartId=ap.id where  ap.ID=" + decAPId + " And apm.CategoryMapID=" + decMId).ToString());
                else if (pId > 0)
                    DteAP = Convert.ToDateTime(maincls.DbCon.ExecuteScaler("Select StartDate From tbl_Part p inner join tbl_Countrypart cp on p.id=cp.partid and cp.countryid=" + iCountryId + " where  p.ID=" + pId).ToString());

                ipQty = Convert.ToInt32(maincls.DbCon.ExecuteScaler("Select apm.Qty From tbl_AssPartModel apm inner join tbl_Assemblypart ap on apm.assemblypartId=ap.id  where ap.Id=" + decAPId));

                decVId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("Select rootmapid From tbl_Categorymapping where Id=" + decMId));

                if (startdate != null)
                    DteSearch = DateTime.Today.Date;
                else
                    DteSearch = DateTime.Today.Date;
                //this instance for histiry class
                clsHistory objHistory = new clsHistory();
                DataTable dtnew = getdata(decMId, decAPId, pId, DteSearch, ipQty, iCountryId);

                if (dtnew.Rows.Count == 0)
                {
                    if (decMId > 0)
                        dtnew = maincls.DbCon.GetTable(@"Select distinct '' As NSC,(case cp.nonServiceable when 0 then '-' else 'NS' end) As SERVICEABLE,
                                                p.Id As PARTID, apm.STARTDATE , apm.ENDDATE, PARTNO, lp.DESCRIPTION,
                                                cpd.PRICE1, CPD.Price2, apm.QTY, p.CHANGEABLE, p.REMARKs as remark,
                                                '' AS PARTHISTORYID,'' AS FILENAME,'','' AS INTERIMAGE 
                                                From tbl_part p Inner join tbl_Assemblypart ap on p.id=ap.partid
                                                inner join tbl_AssPartModel apm on ap.id=apm.AssemblyPartID           
                                                inner join (SELECT c.ID, NVL(cl.DESCRIPTION, c.DESCRIPTION) AS DESCRIPTION,NVL(cl.ALTERNATEDESCRIPTION, c.ALTERNATEDESCRIPTION) AS ALTERNATEDESCRIPTION
	                                            FROM tbl_PART c LEFT JOIN tbl_PARTlng cl ON c.ID = cl.PARTID and cl.languageid=" + LngId + @")  lp on lp.id=P.id	                                                      
                                                inner join tbl_countrypart cp on p.id=cp.partid and cp.countryid=" + iCountryId + @" 
                                                inner join (
                                                select CountryPartID,Price1,Price2 from tbl_countrypartdtl CPDtl
                                                Inner Join (Select Max(ID) as MaxID from tbl_countrypartdtl group by CountryPartID ) CPDM 
                                                On CPDtl.ID= cpdm.MaxID ) CPD On CP.ID=CPD.CountryPartID 
                                                where ap.Id=" + decAPId + " And p.id=" + pId + " And Apm.CategoryMapID=" + decMId.ToString(), "Temp");

                    else

                        dtnew = maincls.DbCon.GetTable(@"Select distinct '' As NSC,(case cp.nonServiceable when 0 then '-' else 'NS' end) As SERVICEABLE, 
                                                p.Id As PARTID, cp.STARTDATE , cp.ENDDATE, PARTNO, lp.DESCRIPTION,
                                                0 As PRICE1,0 As Price2, " + ipQty + @" as QTY, p.CHANGEABLE, p.remarks as REMARK,
                                                '' AS PARTHISTORYID,'' AS FILENAME,'','' AS INTERIMAGE
                                                From tbl_Part p inner join tbl_countrypart cp on p.id=cp.partid
                                                inner join (SELECT c.ID, NVL(cl.DESCRIPTION, c.DESCRIPTION) AS DESCRIPTION,NVL(cl.ALTERNATEDESCRIPTION, c.ALTERNATEDESCRIPTION) AS ALTERNATEDESCRIPTION
	                                            FROM tbl_PART c LEFT JOIN tbl_PARTlng cl ON c.ID = cl.PARTID and cl.languageid=" + LngId + @")  lp on lp.id=P.id
                                                and cp.countryid=" + iCountryId + @"
                                                where p.id=" + pId, "Temp");
                }

                foreach (DataRow dr in dtnew.Rows)
                {
                    PartInfo pinf = new PartInfo();
                    pinf.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                    pinf.partNumber = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                    pinf.description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                    pinf.startdate = Convert.ToString(dr["StartDate"]) == "" ? "" : Convert.ToString(dr["StartDate"]);
                    pinf.enddate = Convert.ToString(dr["EndDate"]) == "" ? "" : Convert.ToString(dr["EndDate"]);
                    pinf.history = Convert.ToString(dr["PartHistoryID"]) == "" ? "" : Convert.ToString(dr["PartHistoryID"]);
                    pinf.quantity = Convert.ToInt32(dr["Qty"]) == 0 ? 0 : Convert.ToInt32(dr["Qty"]);
                    pinf.NS = Convert.ToString(dr["Serviceable"]) == "" ? "" : Convert.ToString(dr["Serviceable"]);
                    pinf.Remark = Convert.ToString(dr["Remark"]) == "" ? "" : Convert.ToString(dr["Remark"]);
                    pinf.PartID = Convert.ToString(dr["PartID"]) == "" ? "" : Convert.ToString(dr["PartID"]);
                    pinf.Interchangeable = Convert.ToString(dr["Interchangeable"]) == "" ? "" : Convert.ToString(dr["Interchangeable"]);
                    plist.Add(pinf);

                }

                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("partHistory", ex);
            }
        }

        public DataTable getdata(decimal VariantId, decimal AssemblyPartId, decimal PartId, DateTime PStartDate, int ipQty, int iCountryId)
        {
            int iLngId = 1;
            DateTime dtdisstdate;
            bool bpinterch = true;
            bool bninterch = true;
            bool bcpDisuse = false;
            int ircnt = 0;
            int idscnt = 0;
            CommanParametrs cpUpPart = new CommanParametrs();
            cpUpPart.Add(new CommanParameter("@CountryID", iCountryId, DbType.Int32));
            cpUpPart.Add(new CommanParameter("@iPqty", ipQty, DbType.Int32));
            cpUpPart.Add(new CommanParameter("@VariantID", VariantId, DbType.Int32));
            cpUpPart.Add(new CommanParameter("@AssemblyPartID", AssemblyPartId, DbType.Int32));
            CommanParameter cpPartid = new CommanParameter("@IPartId", PartId, DbType.Int32);
            cpUpPart.Add(cpPartid);
            cpUpPart.Add(new CommanParameter("@LngId", iLngId, DbType.Int32, 1));

            DataTable dtcurr = maincls.DbCon.GetProcTable("PROC_HISTORY_CURRENT", cpUpPart, "tcurr");

            if (dtcurr.Rows.Count > 0)
            {
                if (Convert.ToInt16(dtcurr.Rows[0]["N_INTRCHNG"] == DBNull.Value ? 0 : dtcurr.Rows[0]["N_INTRCHNG"]) == 7) bninterch = false;
                if (Convert.ToInt16(dtcurr.Rows[0]["Interchangeable"] == DBNull.Value ? 0 : dtcurr.Rows[0]["Interchangeable"]) == 4) bcpDisuse = true;

                if (PStartDate != Convert.ToDateTime(dtcurr.Rows[0]["StartDate"])) PStartDate = Convert.ToDateTime(dtcurr.Rows[0]["StartDate"]);
            }

            cpUpPart.Remove(cpPartid);
            cpUpPart.Add(new CommanParameter("@StartDate", PStartDate, DbType.Date));

            DataTable dtprv = maincls.DbCon.GetProcTable("PROC_HISTORY_PREVIOUS", cpUpPart, "tprv");
            foreach (DataRow dr in dtprv.Rows)
            {
                ircnt += 1;
                if (Convert.ToInt32(dr["ppartid"] == DBNull.Value ? 0 : dr["ppartid"]) == PartId)
                {
                    if (bcpDisuse)
                    {

                        if (dtcurr.Rows.Count > 0) dtcurr.Rows[0]["startdate"] = dr["StartDate"];
                    }

                    if (Convert.ToInt16(dr["Interchangeable"]) == 3)
                    {
                        bpinterch = false;
                        dr.Delete();
                    }
                    else if (Convert.ToInt16(dr["Interchangeable"]) == 4)
                    {
                        idscnt += 1;
                        if (dtcurr.Rows.Count > 0) dtcurr.Rows[0]["startdate"] = dr["StartDate"];
                        dr.Delete();
                    }
                    continue;
                }
                if (!bpinterch) { dr.Delete(); continue; }
                else
                {
                    if (dr["Interchangeable"] == DBNull.Value) continue;
                    if (Convert.ToInt16(dr["Interchangeable"]) == 1 || Convert.ToInt16(dr["Interchangeable"]) == 2) continue;
                    if (Convert.ToInt16(dr["Interchangeable"]) == 3)
                    {
                        dr.Delete();
                        bpinterch = false;
                    }
                    else if (Convert.ToInt16(dr["Interchangeable"]) == 4)
                    {
                        idscnt += 1;
                        if (bcpDisuse)
                        {
                            if (dtcurr.Rows.Count > 0) dtcurr.Rows[0]["startdate"] = dr["StartDate"];
                        }
                        else
                        {
                            DataRow[] drs = dtprv.Select("PartId=" + dr["PPartId"].ToString());
                            if (drs.Length > 0)
                                drs[0]["StartDate"] = dr["StartDate"];
                        }
                        dr.Delete();
                    }
                }


                if (ircnt != idscnt) bcpDisuse = false;

            }
            dtprv.AcceptChanges();
            //=========== for Next =========
            DataTable dtnext = dtcurr.Clone();
            if (bninterch)
            {
                dtnext = maincls.DbCon.GetProcTable("PROC_HISTORY_NEXT", cpUpPart, "tnext");

                foreach (DataRow dr in dtnext.Rows)
                {
                    if (!bninterch) { dr.Delete(); continue; }
                    else
                    {
                        if (Convert.ToInt16(dr["N_INTRCHNG"] == DBNull.Value ? 0 : dr["N_INTRCHNG"]) == 7)
                        {
                            bninterch = false;
                            continue;
                        }
                        if (Convert.ToInt16(dr["Interchangeable"] == DBNull.Value ? 0 : dr["Interchangeable"]) == 4)
                        {
                            dtdisstdate = Convert.ToDateTime(dr["StartDate"]);
                            dr.Delete();
                            continue;
                        }
                    }
                }
                dtnext.AcceptChanges();
            }
            dtcurr.AcceptChanges();
            DataTable dtfinal = dtprv.Clone();
            dtfinal.Merge(dtprv);
            dtfinal.Merge(dtcurr);
            dtfinal.Merge(dtnext);
            dtfinal.DefaultView.Sort = "startdate Asc";
            return dtfinal.DefaultView.ToTable();
        }

        public List<PasswordInfo> ChangePassword(string userid, string Oldpass, string Newpass, string Confpass)
        {
            List<PasswordInfo> passinfo = new List<PasswordInfo>();

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            var oldpass_en = DecryptAes(Oldpass);
            var newpass_en = DecryptAes(Newpass);
            var confpass_en = DecryptAes(Confpass);

            PasswordInfo pinfo = new PasswordInfo();
            //Variable declaration
            string strPass;
            int iStatus;
            //Creating object of user business and property class
            // UserBus userbus = new UserBus();
            UserData userData = new UserData();
            UserProp userprop = new UserProp();
            //     Checking the Last Three Password is not Same.

            //Assign id to property
            userprop.ID = userids;
            //Getting property of assigned id
            userprop = (UserProp)userData.Get(userids);
            //Check for old password
            strPass = userprop.Password;
            if (oldpass_en != Global.DecryptCompSpecific(strPass, "ABF482", true))
            {
                //Set the focus.
                pinfo.ChangeResult = "Invalid Old Password.";
                passinfo.Add(pinfo);
                return passinfo.ToList();
            }
            //Finding the Records From tbl_UserPAssword Table
            DataTable dtUserPassword = userData.FindMaxRevision(userids);
            int iCount = 0;
            int iRevision = 0;
            //Encrypt the password.
            string strNewPassword = Global.EncryptCompSpecific(newpass_en, "ABF482", true);
            if (dtUserPassword != null)
            {
                //If Number of Rows is greater than 0
                if (dtUserPassword.Rows.Count > 0)
                {
                    iRevision = Convert.ToInt32(dtUserPassword.Rows[0]["Revision"]);
                    iRevision += 1;//Increase Revision by 1
                    foreach (DataRow dr in dtUserPassword.Rows)
                    {
                        //Check only 3 Last Password
                        if (iCount <= 2)
                        {
                            if (strNewPassword == Convert.ToString(dr["Password"]))
                            {
                                //Show the message.

                                pinfo.ChangeResult = "Please Enter Different Password.";
                                passinfo.Add(pinfo);
                                return passinfo.ToList();

                            }
                            //Increment the iCount by 1.
                            iCount += 1;
                        }
                    }
                }
            }

            //Assign encrypted password to property
            userprop.Password = Global.EncryptCompSpecific(newpass_en, "ABF482", true);
            //Save password and check save status
            iStatus = userData.Update(userprop);
            if (iStatus < 1)
            {
                //Show the message.

                pinfo.ChangeResult = "Password Change Was Unsuccessful.";
                passinfo.Add(pinfo);
                return passinfo.ToList();

            }
            else
                if (iRevision == 0) iRevision = 1;
            //Save the new password.
            userData.SaveintoUserPassword(userids, iRevision, strNewPassword, userids);
            //Display tha message.

            pinfo.ChangeResult = "Password Changed Successfully.";
            passinfo.Add(pinfo);
            return passinfo.ToList();

        }

        public List<AcessoryPartInfo> AcessaryPartCategorySearch(string cid, string userid, Int32 AcctypeId, Int32 AccId, Int32 iLngId, string StartDate)
        {
            List<AcessoryPartInfo> api = new List<AcessoryPartInfo>();

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            int UserType = 0;
            if (userids > 0)
            {
                UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select UserType from tbl_user where id=" + userids));

            }
            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate);

            AccessoriesPartBus accpBus = new AccessoriesPartBus();
            //Create the Object of AccessoriesPartData Data Class to get the data from database.
            AccessoriesPartData accpData = new AccessoriesPartData();
            //Getting the Parts from the database and store into datatable.
            DataTable dtParts = accpBus.AccPartsCategorySearch(UserType, cids, userids, AcctypeId, AccId, iLngId, dtSearchDate);

            foreach (DataRow dr in dtParts.Rows)
            {
                AcessoryPartInfo pinfo = new AcessoryPartInfo();

                pinfo.ID = Convert.ToInt32(dr["ID"]);
                pinfo.VehicleId = Convert.ToInt32(dr["VEHICLEID"]) == 0 ? 0 : Convert.ToInt32(dr["VEHICLEID"]);
                pinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                pinfo.description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                pinfo.Vehicle = Convert.ToString(dr["VEHICLE"]) == "" ? "" : Convert.ToString(dr["VEHICLE"]);
                pinfo.AccTypeName = Convert.ToString(dr["ACCTYPENAME"]) == "" ? "" : Convert.ToString(dr["ACCTYPENAME"]);
                pinfo.Price = Convert.ToInt32(dr["PRICE1"]) == 0 ? 0 : Convert.ToInt32(dr["PRICE1"]);
                pinfo.BrandName = Convert.ToString(dr["BRANDNAME"]) == "" ? "" : Convert.ToString(dr["BRANDNAME"]);
                pinfo.ColorName = Convert.ToString(dr["COLORNAME"]) == "" ? "" : Convert.ToString(dr["COLORNAME"]);
                pinfo.ColorId = dr["COLORID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COLORID"]);
                pinfo.Image = Convert.ToString(dr["IMAGE"]) == "" ? "" : Convert.ToString(dr["IMAGE"]);
                pinfo.VariantId = dr["VARIANTID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["VARIANTID"]);
                pinfo.Qty = dr["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QTY"]);
                pinfo.AccTypeId = dr["ACCTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACCTYPEID"]);
                pinfo.CategoryTypeId = dr["CATEGORYTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CATEGORYTYPEID"]);

                api.Add(pinfo);
            }
            return api.ToList();

        }

        public List<AcessoryPartInfo> FillAccItem(string iPartID, string StartDate, string cid, Int32 lngid, string usertype)
        {
            List<AcessoryPartInfo> apd = new List<AcessoryPartInfo>();

            var pid_en = DecryptAes(iPartID);
            Int32 pids = Convert.ToInt32(pid_en);

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            var usertype_en = DecryptAes(usertype);
            Int32 usertypes = Convert.ToInt32(usertype_en);

            AccessoryDesciptionViewData accDescViewData = new AccessoryDesciptionViewData();

            DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate);
            string strSql = string.Empty;
            //Getting the DataTable from database on the base of query.
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@PartID", pids, DbType.Int32, 0));
            cps.Add(new CommanParameter("@CountryID", cids, DbType.Int32));
            cps.Add(new CommanParameter("@SelDate", dtSearchDate, DbType.Date));
            cps.Add(new CommanParameter("@LngId", lngid, DbType.Int32, 1));
            cps.Add(new CommanParameter("@uType", usertypes, DbType.Int32, 0));
            DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ACCDESCVIEWBINDCONTROL", cps, "BindControlData");

            foreach (DataRow dr in dtPart.Rows)
            {
                AcessoryPartInfo pinfo = new AcessoryPartInfo();

                pinfo.ID = Convert.ToInt32(dr["ID"]);
                pinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                pinfo.partName = Convert.ToString(dr["PARTNAME"]) == "" ? "" : Convert.ToString(dr["PARTNAME"]);
                pinfo.description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                pinfo.Price = Convert.ToDecimal(dr["PRICE1"]) == 0 ? 0 : Convert.ToDecimal(dr["PRICE1"]);
                pinfo.BrandName = dr["BRANDNAME"].ToString() == "" ? "" : dr["BRANDNAME"].ToString();
                pinfo.LogoThumb = dr["LOGOTHUMBIL"].ToString() == "" ? "" : dr["LOGOTHUMBIL"].ToString();



                apd.Add(pinfo);
            }
            return apd.ToList();

        }

        public List<FillAcessType> FillAccessoriType(Int32 lngid)
        {
            List<FillAcessType> faty = new List<FillAcessType>();
            AccessoryCategoryViewData accCatViewData = new AccessoryCategoryViewData();
            DataTable dtCateType = accCatViewData.CatSearchFillAccType(lngid);
            foreach (DataRow dr in dtCateType.Rows)
            {
                FillAcessType fainfo = new FillAcessType();
                fainfo.id = Convert.ToInt32(dr["ID"]);
                fainfo.AcctypeName = Convert.ToString(dr["ACCTYPENAME"]) == "" ? "" : Convert.ToString(dr["ACCTYPENAME"]);
                fainfo.Acctypeid = dr["ACCTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACCTYPEID"]);
                fainfo.Priority = dr["PRIORITY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PRIORITY"]);
                fainfo.Inactive = dr["INACTIVE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["INACTIVE"]);
                faty.Add(fainfo);
            }
            return faty.ToList();
        }

        public List<FillAcessChild> FillAccessoriChild(Int32 AccId, Int32 LngId, string cid)
        {
            List<FillAcessChild> fac = new List<FillAcessChild>();

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            AccessoriesListingData acclistingdata = new AccessoriesListingData();
            DataTable dtCateType = acclistingdata.CatSearchFillListView(AccId, LngId, cids);
            foreach (DataRow dr in dtCateType.Rows)
            {
                FillAcessChild fainfo = new FillAcessChild();
                fainfo.id = Convert.ToInt32(dr["ID"]);
                fainfo.acessorypartid = dr["ACCESSORYPARTID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACCESSORYPARTID"]);
                fainfo.partNumber = dr["PARTNO"] == DBNull.Value ? "" : Convert.ToString(dr["PARTNO"]);
                fainfo.description = dr["DESCRIPTION"] == DBNull.Value ? "" : Convert.ToString(dr["DESCRIPTION"]);
                fainfo.inactive = dr["INACTIVE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["INACTIVE"]);
                fainfo.price = dr["PRICE1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PRICE1"]);
                fainfo.attachmenttype = dr["ATTACHMENTTYPE"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ATTACHMENTTYPE"]);
                fac.Add(fainfo);
            }
            return fac.ToList();
        }

        public List<AcessoryPartInfo> GetAcessoryPartList(string cid, string userid, Int32 lngid, Int32 pagesize)
        {

            List<AcessoryPartInfo> api = new List<AcessoryPartInfo>();

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            int UserType = 0;
            if (userids > 0)
            {
                UserType = 1;// Convert.ToInt32(maincls.DbCon.ExecuteScaler("select UserType from tbl_user where id=" + userid));

            }

            AccessoriesPartBus accpBus = new AccessoriesPartBus();
            //Create the Object of AccessoriesPartData Data Class to get the data from database.
            AccessoriesPartData accpData = new AccessoriesPartData();
            //Getting the Parts from the database and store into datatable.


            // Scroll size in Part search
            int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);

            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@UserType", UserType, DbType.Int32, 0));
            cps.Add(new CommanParameter("@CountryID", cids));
            cps.Add(new CommanParameter("@UserID", userids));
            cps.Add(new CommanParameter("@LngId", lngid, DbType.Int32, 1));
            cps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32));
            cps.Add(new CommanParameter("@Page", pagesize, DbType.Int32));
            //DataTable dtParts= maincls.DbCon.GetProcTable("PROC_ACCPARTGETACCESSORYPARTS", cps, "temp");
            DataTable dtParts = maincls.DbCon.GetProcTable("PROC_ACCPARTGETACCESPARTSMOB", cps, "temp");

            //DataTable dtParts = accpBus.GetAccessoryParts(UserType, cid, userid, lngid);
            foreach (DataRow dr in dtParts.Rows)
            {
                AcessoryPartInfo pinfo = new AcessoryPartInfo();

                pinfo.ID = Convert.ToInt32(dr["ID"]);
                pinfo.total = dr["COUNTTOTAL"] != DBNull.Value ? Convert.ToInt32(dr["COUNTTOTAL"]) : pinfo.total;
                pinfo.VehicleId = Convert.ToInt32(dr["VEHICLEID"]);
                pinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                pinfo.description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                pinfo.Vehicle = Convert.ToString(dr["VEHICLE"]) == "" ? "" : Convert.ToString(dr["VEHICLE"]);
                pinfo.AccTypeName = Convert.ToString(dr["ACCTYPENAME"]) == "" ? "" : Convert.ToString(dr["ACCTYPENAME"]);
                pinfo.Price = Convert.ToDecimal(dr["PRICE1"]) == 0 ? 0 : Convert.ToDecimal(dr["PRICE1"]);
                pinfo.BrandName = Convert.ToString(dr["BRANDNAME"]) == "" ? "" : Convert.ToString(dr["BRANDNAME"]);
                pinfo.ColorName = Convert.ToString(dr["COLORNAME"]) == "" ? "" : Convert.ToString(dr["COLORNAME"]);
                pinfo.ColorId = dr["COLORID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COLORID"]);
                pinfo.Image = Convert.ToString(dr["IMAGE"]) == "" ? "" : Convert.ToString(dr["IMAGE"]);
                pinfo.VariantId = dr["VARIANTID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["VARIANTID"]);
                pinfo.Qty = dr["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QTY"]);
                pinfo.AccTypeId = dr["ACCTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACCTYPEID"]);
                pinfo.CategoryTypeId = dr["CATEGORYTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CATEGORYTYPEID"]);



                api.Add(pinfo);
            }
            return api.ToList();
        }

        public List<AcessoryPartInfo> GetAcessoryPartListSearch(string details, string cid, string userid, Int32 lngid)
        {
            //, Int32 pagesize
            string partno = "", description = "";
            List<AcessoryPartInfo> api = new List<AcessoryPartInfo>();

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            string details_en = DecryptAes(details);


            int UserType = 0;
            if (userids > 0)
            {
                UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select UserType from tbl_user where id=" + userids));

            }

            AccessoriesPartBus accpBus = new AccessoriesPartBus();
            //Create the Object of AccessoriesPartData Data Class to get the data from database.
            AccessoriesPartData accpData = new AccessoriesPartData();

            DataTable dt = accpData.GetAccPartNo(details_en);
            if (dt.Rows.Count > 0)
            {
                partno = details_en;
            }
            else
            {
                description = details_en;
            }
            //Getting the Parts from the database and store into datatable.
            DataTable dtParts = accpBus.GetAccessoryParts(UserType, cids, userids, lngid);

            // Scroll size in Part search
            //  int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);

            //  CommanParametrs cps = new CommanParametrs();
            //  cps.Add(new CommanParameter("@UserType", UserType, DbType.Int32, 0));
            //  cps.Add(new CommanParameter("@CountryID", cid));
            //  cps.Add(new CommanParameter("@UserID", UserType));
            //  cps.Add(new CommanParameter("@LngId", lngid, DbType.Int32, 1));
            //  cps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32));
            //  cps.Add(new CommanParameter("@Page", pagesize, DbType.Int32));
            //// DataTable dtParts= maincls.DbCon.GetProcTable("PROC_ACCPARTGETACCESSORYPARTS", cps, "temp");
            //  DataTable dtParts = maincls.DbCon.GetProcTable("PROC_ACCPARTGETACCESPARTSMOB", cps, "temp");

            DataTable dtpClon = dtParts.Clone();
            if (partno.Trim() != "")
            {
                dtParts.Select("PartNo like '%" + partno + "%'").CopyToDataTable(dtpClon, LoadOption.OverwriteChanges);
            }
            else
            {

                dtParts.Select("Description like '%" + description + "%'").CopyToDataTable(dtpClon, LoadOption.OverwriteChanges);
            }

            foreach (DataRow dr in dtpClon.Rows)
            {
                AcessoryPartInfo pinfo = new AcessoryPartInfo();

                pinfo.ID = Convert.ToInt32(dr["ID"]);
                pinfo.VehicleId = Convert.ToInt32(dr["VEHICLEID"]);
                pinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                pinfo.description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                pinfo.Vehicle = Convert.ToString(dr["VEHICLE"]) == "" ? "" : Convert.ToString(dr["VEHICLE"]);
                pinfo.AccTypeName = Convert.ToString(dr["ACCTYPENAME"]) == "" ? "" : Convert.ToString(dr["ACCTYPENAME"]);
                pinfo.Price = Convert.ToInt32(dr["PRICE1"]) == 0 ? 0 : Convert.ToInt32(dr["PRICE1"]);
                pinfo.BrandName = Convert.ToString(dr["BRANDNAME"]) == "" ? "" : Convert.ToString(dr["BRANDNAME"]);
                pinfo.ColorName = Convert.ToString(dr["COLORNAME"]) == "" ? "" : Convert.ToString(dr["COLORNAME"]);
                pinfo.ColorId = dr["COLORID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["COLORID"]);
                pinfo.Image = Convert.ToString(dr["IMAGE"]) == "" ? "" : Convert.ToString(dr["IMAGE"]);
                pinfo.VariantId = dr["VARIANTID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["VARIANTID"]);
                pinfo.Qty = dr["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QTY"]);
                pinfo.AccTypeId = dr["ACCTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ACCTYPEID"]);
                pinfo.CategoryTypeId = dr["CATEGORYTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CATEGORYTYPEID"]);



                api.Add(pinfo);
            }
            return api.ToList();
        }
        # endregion

        # region VIN SEARCH

        public List<VinInfo> VinSearch(string UserId, string VinNo, string EngineNo, string countryid)
        {
            List<VinInfo> plist = new List<VinInfo>();
            try
            {
                var userid_en = DecryptAes(UserId);
                Int32 userids = Convert.ToInt32(userid_en);

                var cid_en = DecryptAes(countryid);
                Int32 cids = Convert.ToInt32(cid_en);

                // Int32 countryid = 1;
                Int32 UserType = 0;
                Int32 LngId = 1;
                string strChassisNo = string.Empty;
                string strEngineNo = string.Empty;
                string strChassisSql = string.Empty;
                DateTime dtSearchDate = DateTime.Today;
                DataTable dtChassisNo = new DataTable();
                // Getting the bool value as define in web config for engine no. text Req. or not.
                string strMANDTENGINE = ConfigurationManager.AppSettings["MANDTENGINE"];

                object objisSbomMandatory = maincls.DbCon.ExecuteScaler("Select ISSBOMMANDATORY from tbl_setting");
                if (objisSbomMandatory == null) objisSbomMandatory = false;

                if (userids > 0)
                {
                    UserType = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select UserType from tbl_user where id=" + userids));
                    //countryid = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + UserId));
                }
                if (!string.IsNullOrEmpty(VinNo))
                {
                    strChassisNo = VinNo.Trim();
                    strChassisNo = strChassisNo.Replace("'", "''");
                    // correcting Engine No.
                    strEngineNo = EngineNo.Trim();
                    strEngineNo = strEngineNo.Replace("'", "''");
                    dtChassisNo = FillChassisSearch(strChassisNo.ToUpper(), strEngineNo.ToUpper(), Convert.ToBoolean(strMANDTENGINE), Convert.ToBoolean(objisSbomMandatory), userids, 1, cids, LngId);
                }
                //checking that any VIN if exist or not 
                if (dtChassisNo.Rows.Count > 0)
                {
                    VinInfo pinf = new VinInfo();
                    pinf.Id = Convert.ToInt32(dtChassisNo.Rows[0]["Id"]);
                    if (Convert.ToBoolean(objisSbomMandatory)) pinf.SbomId = Convert.ToInt32(dtChassisNo.Rows[0]["SbomId"]);
                    pinf.SbomCode = Convert.ToString(dtChassisNo.Rows[0]["SBOMCode"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["SBOMCode"]);
                    pinf.Description = Convert.ToString(dtChassisNo.Rows[0]["Description"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["Description"]);
                    pinf.EngineNo = Convert.ToString(dtChassisNo.Rows[0]["Engineno"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["Engineno"]);
                    string[] dt = Convert.ToString(dtChassisNo.Rows[0]["StartDate"]).Split(' ');
                    pinf.ProductionDate = Convert.ToString(dt[0]);//Convert.ToString(dtChassisNo.Rows[0]["StartDate"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["StartDate"]);
                    //if (Convert.ToBoolean(dtSetting.Rows[0]["ISSBOMMANDATORY"]) == true)
                    //{
                    //    int istartPos = Convert.ToInt32(ConfigurationManager.AppSettings["SRSTARTPOS"].ToString());
                    //    pinf.ChassisNo = Convert.ToString(dtChassisNo.Rows[0]["ChassisNo"]).Substring(istartPos, 8);
                    //}
                    //else
                    pinf.ChassisNo = Convert.ToString(dtChassisNo.Rows[0]["ChassisNo"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["ChassisNo"]);
                    pinf.ModelName = Convert.ToString(dtChassisNo.Rows[0]["Model"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["Model"]);
                    pinf.Vehicle = Convert.ToString(dtChassisNo.Rows[0]["Vehicle"]) == "" ? "" : Convert.ToString(dtChassisNo.Rows[0]["Vehicle"]);
                    IDataReader dr = maincls.DbCon.GetReader("select cm.CatMImage from tbl_CategoryMapping cm join tbl_Category c on cm.CategoryID=c.ID where cm.ID=" + dtChassisNo.Rows[0]["ID"].ToString());

                    while (dr.Read())
                    {
                        if (dr["CatMImage"] is DBNull)
                        {
                            pinf.ImageByte = null;
                        }
                        else
                        {
                            byte[] bytBuffer;

                            bytBuffer = (byte[])dr["CatMImage"];
                            // pinf.ImageByte = bytBuffer;
                            pinf.ImageByte = bytBuffer == null ? null : getImgArray(bytBuffer, false);
                        }
                    }


                    plist.Add(pinf);
                }
                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("vinSearch", ex);
            }

        }

        public List<CategoryInfo> VinSearchAggregate(Int32 VariantId, Int32 LngId)
        {
            List<CategoryInfo> plist = new List<CategoryInfo>();
            try
            {
                object objiSLastLevel = maincls.DbCon.ExecuteScaler("select count(cm.id) from tbl_categorymapping cm inner join tbl_categorymapping cmv on cm.pmapid=cmv.id where cmv.pmapid=" + VariantId);
                if (Convert.ToInt16(objiSLastLevel) > 0) objiSLastLevel = 0; else objiSLastLevel = 1;

                DataTable dtAggregate = VINCatFillCategoryType(VariantId, LngId);
                //checking that any Aggregate if exist or not 
                foreach (DataRow dr in dtAggregate.Rows)
                {
                    CategoryInfo pinf = new CategoryInfo();
                    pinf.Id = Convert.ToInt32(dr["Id"]);
                    pinf.CategoryId = Convert.ToInt32(dr["CategoryId"]);
                    pinf.CategoryName = Convert.ToString(dr["CategoryName"]) == "" ? "" : Convert.ToString(dr["CategoryName"]);
                    pinf.iSLatLevel = Convert.ToBoolean(objiSLastLevel);
                    pinf.ImagePath = "ImageHandler.ashx?ID=" + pinf.Id + "&Type=1&width=75&height=75&isBinary=1";
                    plist.Add(pinf);
                }
                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("vinSearchAggregate", ex);
            }
        }

        public List<VinAssemblyInfo> VinSearchAssembly(int AggregateID, string StartDate, int ModelID, int SBOMID, Int32 LngId)
        {
            List<VinAssemblyInfo> plist = new List<VinAssemblyInfo>();
            try
            {
                DateTime dtSearchDate = StartDate == "" ? DateTime.Today : Convert.ToDateTime(StartDate);
                object objisSbomMandatory = maincls.DbCon.ExecuteScaler("Select ISSBOMMANDATORY from tbl_setting");
                if (objisSbomMandatory == null) objisSbomMandatory = false;

                DataTable dtAssembly = VINCatFillCategory(Convert.ToBoolean(MainCommon.FigNoWithModelCode), Convert.ToBoolean(objisSbomMandatory), AggregateID, dtSearchDate, ModelID, SBOMID, LngId);
                //checking that any Assembly if exist or not 
                foreach (DataRow dr in dtAssembly.Rows)
                {
                    VinAssemblyInfo pinf = new VinAssemblyInfo();
                    pinf.Id = Convert.ToInt32(dr["Id"]);
                    pinf.AMId = Convert.ToInt32(dr["AMId"]);
                    pinf.CategoryName = Convert.ToString(dr["CategoryName"]) == "" ? "" : Convert.ToString(dr["CategoryName"]);
                    pinf.FigNo = Convert.ToString(dr["FigNo"]) == "" ? "" : Convert.ToString(dr["FigNo"]);
                    pinf.ImagePath = "ImageHandler.ashx?ID=" + pinf.Id + "&Type=0&width=75&height=75&isBinary=1";
                    plist.Add(pinf);
                }
                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("vinSearchAssembly", ex);
            }
        }

        public DataTable VINCatFillCategory(bool BlnFigno, bool BlnSBOMMAN, int AGGID, DateTime SELDATE, int MODELID, int SBOMID, int LngId)
        {
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@BlnFigno", BlnFigno, DbType.Boolean));
            cps.Add(new CommanParameter("@BlnSBOMMAN", BlnSBOMMAN, DbType.Boolean));
            cps.Add(new CommanParameter("@AGGID", AGGID, DbType.Int32));
            cps.Add(new CommanParameter("@SELDATE", SELDATE, DbType.DateTime));
            cps.Add(new CommanParameter("@MODELID", MODELID, DbType.Int32));
            cps.Add(new CommanParameter("@SBOMID", SBOMID, DbType.Int32));
            cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            return maincls.DbCon.GetProcTable("PROC_VINCATFILLCATEGORY", cps, "tbl_chassismaster");
        }

        public DataTable VINCatFillCategoryType(int Index, int LngId)
        {
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@INDEX", Index, DbType.Int32));
            cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            return maincls.DbCon.GetProcTable("PROC_VINCATFILLCATEGORYTYPE", cps, "tbl_Category");
        }

        public DataTable FillChassisSearch(string strVIN, string strEngine, bool MANDTENGINE, bool ISSBOMMAN, int UserID, int UserType, int CountryID, int LngId)
        {
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@strVIN", strVIN, DbType.String));
            cps.Add(new CommanParameter("@strEngine", strEngine, DbType.String));
            cps.Add(new CommanParameter("@MANDTENGINE", MANDTENGINE, DbType.Int32, 0));
            cps.Add(new CommanParameter("@ISSBOMMAN", ISSBOMMAN, DbType.Int32, 0));
            cps.Add(new CommanParameter("@UserID", UserID, DbType.Int32));
            cps.Add(new CommanParameter("@UserType", UserType, DbType.Int32, 0));
            cps.Add(new CommanParameter("@CountryID", CountryID, DbType.Int32));
            cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            return maincls.DbCon.GetProcTable("PROC_CHASSISSEARCH_NEW", cps, "tbl_chassismaster");
        }
        # endregion

        #region CHECKOUT

        public List<CheckOutNotes> GetCheckoutDescription(Int32 ID)
        {
            List<CheckOutNotes> chkobject = new List<CheckOutNotes>();
            string result = string.Empty;
            // create new object for commanParameters class
            CommanParametrs objcps = new CommanParametrs();
            // create instance for commanparameter class
            CommanParameter objcp;
            //creating Object of CommanParameter class and asining value of ID
            objcp = new CommanParameter("@ID", ID, DbType.Decimal);
            //Add Value into cps object
            objcps.Add(objcp);
            // Select query assign in new string object
            string strSQL = "SELECT ID,Note FROM tbl_CheckOutNote WHERE ID=isnull(@ID,ID)";
            //Getting Record into Table from DataBase
            DataTable dtcheckoutprop = maincls.DbCon.GetTable(strSQL, objcps, "tbl_CheckOutNote");

            string strsmarty = "select * from TBL_ADDRESSCONFIGURATIONMASTER where MethodID=4";
            DataTable dtsmarty = maincls.DbCon.GetTable(strsmarty, objcps, "TBL_ADDRESSCONFIGURATIONMASTER");

            string url = dtsmarty.Rows[0]["MethodURL"].ToString();
            string authkey = dtsmarty.Rows[0]["AuthKey"].ToString();
            string authToken = dtsmarty.Rows[0]["AuthToken"].ToString();
            if (dtcheckoutprop.Rows.Count > 0)
            {
                CheckOutNotes chk = new CheckOutNotes();
                result = Convert.ToString(dtcheckoutprop.Rows[0]["Note"]) == "" ? "" : Convert.ToString(dtcheckoutprop.Rows[0]["Note"]);
                chk.Note = result;
                chk.SmartyUrl = url + "?auth-id=" + authkey + "&auth-token=" + authToken;
                chkobject.Add(chk);
            }
            return chkobject.ToList();
        }

        public List<OrderType> getOrderType(bool inactive, Int32 lngId)
        {
            int inact = 0;
            if (inactive == false)
                inact = 0;
            else
                inact = 1;

            List<OrderType> otypedata = new List<OrderType>();
            OrderTypeData otype = new OrderTypeData();
            string strSQL = "Select OT.ID,ISNULL(OTL.OTYPE,OT.OType)  OType,ISNULL(OTL.OTDesc,OT.OTDesc) OTDesc,OT.OTMOV, '0' as SRNO,OTP.Priority from tbl_OrderType OT Left join TBL_ORDERTYPELNG OTL ON OTL.OTYPEID=OT.ID AND OTL.LANGUAGEID=" + lngId + " left join tbl_OTypeProp OTP on OTP.OTypeId=ot.ID where OT.id >0 and OT.Inactive =" + inact + " Order By OTP.Priority asc";
            DataTable ordertype = maincls.DbCon.GetTable(strSQL, "tbl_OrderType");
            foreach (DataRow dr in ordertype.Rows)
            {
                OrderType otypeinfo = new OrderType();
                otypeinfo.Id = Convert.ToInt32(dr["Id"]);
                otypeinfo.Type = Convert.ToString(dr["OType"]) == "" ? "" : Convert.ToString(dr["OType"]);
                otypeinfo.Description = Convert.ToString(dr["OTDesc"]) == "" ? "" : Convert.ToString(dr["OTDesc"]);
                otypeinfo.MOV = Convert.ToInt32(dr["OTMOV"]) == 0 ? 0 : Convert.ToInt32(dr["OTMOV"]);
                otypeinfo.Priority = dr["PRIORITY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PRIORITY"]);
                otypedata.Add(otypeinfo);
            }

            return otypedata.ToList();
        }

        public List<OrderType> getOrderTypeList(bool inactive, Int32 lngId, Int32 cid)
        {
            int inact = 0;
            if (inactive == false)
                inact = 0;
            else
                inact = 1;

            List<OrderType> otypedata = new List<OrderType>();
            OrderTypeData otype = new OrderTypeData();
            string factor_query = "select cr.CFactor from tbl_Currency cr inner join tbl_country c on c.CurrencyId=cr.id where c.id=" + cid;
            DataTable factor = maincls.DbCon.GetTable(factor_query, "tbl_Currency");
            decimal val = Convert.ToDecimal(factor.Rows[0][0]) == 0 ? 0 : Convert.ToDecimal(factor.Rows[0][0]);

            string strSQL = "Select OT.ID,ISNULL(OTL.OTYPE,OT.OType)  OType,ISNULL(OTL.OTDesc,OT.OTDesc) OTDesc,OT.OTMOV, '0' as SRNO,OTP.Priority from tbl_OrderType OT Left join TBL_ORDERTYPELNG OTL ON OTL.OTYPEID=OT.ID AND OTL.LANGUAGEID=" + lngId + " left join tbl_OTypeProp OTP on OTP.OTypeId=ot.ID where OT.id >0 and OT.Inactive =" + inact + " Order By OTP.Priority asc";
            DataTable ordertype = maincls.DbCon.GetTable(strSQL, "tbl_OrderType");
            foreach (DataRow dr in ordertype.Rows)
            {
                OrderType otypeinfo = new OrderType();
                otypeinfo.Id = Convert.ToInt32(dr["Id"]);
                otypeinfo.Type = Convert.ToString(dr["OType"]) == "" ? "" : Convert.ToString(dr["OType"]);
                otypeinfo.Description = Convert.ToString(dr["OTDesc"]) == "" ? "" : Convert.ToString(dr["OTDesc"]);
                decimal mov_val = Convert.ToDecimal(dr["OTMOV"]) == 0 ? 0 : Convert.ToDecimal(dr["OTMOV"]);
                decimal act_val = Convert.ToDecimal(mov_val * val);
                otypeinfo.MOV = Math.Round(act_val, 2);
                otypeinfo.Priority = dr["PRIORITY"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PRIORITY"]);
                otypedata.Add(otypeinfo);
            }

            return otypedata.ToList();
        }

        public List<OrderTypeAttr> OrderTypeAttributes(Int32 OrderTypeID)
        {
            List<OrderTypeAttr> otypedata = new List<OrderTypeAttr>();
            OTypeAttributeMapData objOTypeAttrData = new OTypeAttributeMapData();
            DataTable dtOtypeAttr = objOTypeAttrData.AttributesPriorityList(OrderTypeID);
            foreach (DataRow dr in dtOtypeAttr.Rows)
            {
                OrderTypeAttr otypeinfo = new OrderTypeAttr();
                otypeinfo.Id = Convert.ToInt32(dr["Id"]);
                otypeinfo.OTypeCaption = Convert.ToString(dr["Caption"]) == "" ? "" : Convert.ToString(dr["Caption"]);
                otypeinfo.AttributeType = Convert.ToBoolean(dr["AttributeType"]);
                otypeinfo.ValidationType = Convert.ToBoolean(dr["ValidationType"]);
                otypeinfo.OTypeAttributes = Convert.ToString(dr["OTypeAttributes"]) == "" ? "" : Convert.ToString(dr["OTypeAttributes"]);

                otypedata.Add(otypeinfo);
            }

            return otypedata.ToList();
        }

        public List<DistributorBillAddress> fillDistAddress(string DistributorId)
        {
            List<DistributorBillAddress> distbilladd = new List<DistributorBillAddress>();
            DistributorData distributordata = new DistributorData();

            var distid_en = DecryptAes(DistributorId);

            int userid = distributordata.GetUserID(Convert.ToInt32(distid_en));

            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@ID", distid_en, DbType.Int32));
            // Getting the distributor record.
            DataTable dtDistributor = maincls.DbCon.GetProcTable("PROC_DISTRIBUTORGETWITHUSER", cps, "Distributor");
            foreach (DataRow dr in dtDistributor.Rows)
            {
                DistributorBillAddress distinfo = new DistributorBillAddress();
                distinfo.FistName = Convert.ToString(dr["UFNAME"]) == "" ? "" : Convert.ToString(dr["UFNAME"]);
                distinfo.LastName = Convert.ToString(dr["ULNAME"]) == "" ? "" : Convert.ToString(dr["ULNAME"]);
                distinfo.Company = Convert.ToString(dr["NAME"]) == "" ? "" : Convert.ToString(dr["NAME"]);
                distinfo.Address = Convert.ToString(dr["ADDRESS"]) == "" ? "" : Convert.ToString(dr["ADDRESS"]);
                distinfo.Email = Convert.ToString(dr["EMAIL"]) == "" ? "" : Convert.ToString(dr["EMAIL"]);
                distinfo.Phone = Convert.ToString(dr["PHONE"]) == "" ? "" : Convert.ToString(dr["PHONE"]);
                distinfo.CountryName = Convert.ToString(dr["COUNTRYNAME"]) == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]);
                distinfo.StateName = Convert.ToString(dr["STATENAME"]) == "" ? "" : Convert.ToString(dr["STATENAME"]);
                distinfo.CityName = Convert.ToString(dr["CITYNAME"]) == "" ? "" : Convert.ToString(dr["CITYNAME"]);
                distinfo.CountryId = Convert.ToInt32(dr["COUNTRYID"]);
                distinfo.StateId = Convert.ToInt32(dr["STATEID"]);
                distinfo.CityId = Convert.ToInt32(dr["CITYID"]);
                distbilladd.Add(distinfo);
            }
            return distbilladd.ToList();
        }

        public List<ShipAddress> GetShippingAddress(string DistributorId)
        {
            List<ShipAddress> distbilladd = new List<ShipAddress>();
            AddressData objAddressData = new AddressData();

            var distid_en = DecryptAes(DistributorId);

            //Creating Object Of CommanParametrs Class
            CommanParametrs cps = new CommanParametrs();
            CommanParameter cp;
            cp = new CommanParameter("@ID", 0, DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@DistributorId", Convert.ToInt32(distid_en), DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@FromDate", "", DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@ToDate", "", DbType.String);
            cps.Add(cp);
            DataTable dtshipAddress = maincls.DbCon.GetProcTable("PROC_GETDISTSHIPADDRESS", cps, "tbl_OrderShippingAddress");
            Int32 i = 1;
            foreach (DataRow dr in dtshipAddress.Rows)
            {
                ShipAddress distinfo = new ShipAddress();
                distinfo.ID = Convert.ToInt32(dr["ID"]);
                distinfo.FistName = Convert.ToString(dr["FNAME"]) == "" ? "" : Convert.ToString(dr["FNAME"]);
                distinfo.LastName = Convert.ToString(dr["LNAME"]) == "" ? "" : Convert.ToString(dr["LNAME"]);
                distinfo.Company = Convert.ToString(dr["COMPANY"]) == "" ? "" : Convert.ToString(dr["COMPANY"]);
                distinfo.Address1 = Convert.ToString(dr["ADDRESS1"]) == "" ? "" : Convert.ToString(dr["ADDRESS1"]);
                distinfo.Address2 = Convert.ToString(dr["ADDRESS2"]) == "" ? "" : Convert.ToString(dr["ADDRESS2"]);
                distinfo.Address3 = Convert.ToString(dr["ADDRESS3"]) == "" ? "" : Convert.ToString(dr["ADDRESS3"]);
                distinfo.CountryName = Convert.ToString(dr["COUNTRYNAME"]) == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]);
                distinfo.StateName = Convert.ToString(dr["STATENAME"]) == "" ? "" : Convert.ToString(dr["STATENAME"]);
                distinfo.CityName = Convert.ToString(dr["CITYNAME"]) == "" ? "" : Convert.ToString(dr["CITYNAME"]);
                distinfo.Phone = Convert.ToString(dr["PHONE"]) == "" ? "" : Convert.ToString(dr["PHONE"]);
                distinfo.Email = Convert.ToString(dr["EMAIL"]) == "" ? "" : Convert.ToString(dr["EMAIL"]);
                distinfo.CountryId = Convert.ToInt32(dr["COUNTRYID"]);
                distinfo.StateId = Convert.ToInt32(dr["STATEID"]);
                distinfo.CityId = Convert.ToInt32(dr["CITYID"]);
                distinfo.PostalCode = Convert.ToString(dr["PINCODE"]) == "" ? "" : Convert.ToString(dr["PINCODE"]);
                distinfo.OrderNo = Convert.ToString(dr["ORDERID"]) == "" ? "" : Convert.ToString(dr["ORDERID"]);
                distinfo.DistributorId = Convert.ToInt32(dr["DISTRIBUTORID"]);

                distbilladd.Add(distinfo);
                if (i++ > 4) break;
            }
            return distbilladd.ToList();
        }

        public List<CarrierMethod> getShipMethod(bool inactive, string userid)
        {
            List<CarrierMethod> carmetobj = new List<CarrierMethod>();

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            CarrierMethodData objCarrierData = new CarrierMethodData();
            CommanParametrs objcps = new CommanParametrs();
            // assign new instance for commanparameter class
            CommanParameter objcp;
            //creating Object of CommanParameter class and asining value of ID
            objcp = new CommanParameter("@DealerID", userids, DbType.Decimal);
            //Add value into cps object
            objcps.Add(objcp);
            //Return DataTable of CarrierMethod
            DataTable dtCarrierMethod = maincls.DbCon.GetProcTable("PROC_CARRIERMETHODLIST", objcps, "tbl_CarrierMethod");
            // DataTable dtCarrierMethod = objCarrierData.List();
            foreach (DataRow dr in dtCarrierMethod.Rows)
            {
                CarrierMethod cm = new CarrierMethod();
                cm.ID = Convert.ToInt32(dr["ID"]);
                cm.CarrierName = Convert.ToString(dr["CARRIERNAME"]) == "" ? "" : Convert.ToString(dr["CARRIERNAME"]);
                cm.MethodName = Convert.ToString(dr["METHODNAME"]) == "" ? "" : Convert.ToString(dr["METHODNAME"]);

                carmetobj.Add(cm);
            }
            return carmetobj.ToList();
        }

        public List<Country> GetCountry()
        {
            List<Country> objCountry = new List<Country>();
            CountryData countryData = new CountryData();
            DataTable dt = countryData.CountryList();
            foreach (DataRow dr in dt.Rows)
            {
                Country country = new Country();
                country.ID = Convert.ToInt32(dr["ID"]);
                country.CountryName = Convert.ToString(dr["COUNTRYNAME"]).Trim() == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]).Trim();
                objCountry.Add(country);
            }

            return objCountry.ToList();

        }

        public List<State> GetState(Int32 countryid)
        {
            List<State> objState = new List<State>();
            StateData statedata = new StateData();
            DataTable dt = statedata.GetStateDetail(countryid);
            foreach (DataRow dr in dt.Rows)
            {
                State state = new State();
                state.ID = Convert.ToInt32(dr["ID"]);
                state.StateName = Convert.ToString(dr["STATENAME"]).Trim() == "" ? "" : Convert.ToString(dr["STATENAME"]).Trim();
                objState.Add(state);
            }

            return objState.ToList();

        }

        public List<City> GetCity(Int32 stateid, Int32 countryid)
        {
            List<City> objCity = new List<City>();
            string strQuery = @"SELECT C.CITYNAME,C.ID,R.REGIONNAME FROM TBL_CITY C
                            INNER JOIN TBL_REGION R ON C.REGIONID=R.ID 
                            WHERE COUNTRYID=" + countryid + " AND STATEID=" + stateid;

            DataTable dt = maincls.DbCon.GetTable(strQuery, "TBL_CITY");
            foreach (DataRow dr in dt.Rows)
            {
                City city = new City();
                city.ID = Convert.ToInt32(dr["ID"]);
                city.CityName = Convert.ToString(dr["CITYNAME"]).Trim() == "" ? "" : Convert.ToString(dr["CITYNAME"]).Trim();

                objCity.Add(city);
            }

            return objCity.ToList();

        }

        public List<City> GetCityName(string city, decimal stateid, decimal countryid)
        {
            List<City> objCity = new List<City>();
            DataTable dtCity = maincls.DbCon.GetTable("Select ID,CityName from tbl_city where CityName like '" + city + "%' and StateID=" + stateid + " and countryid=" + countryid + "  and CityName is not null", "tbl_OrderShippingAddress");
            foreach (DataRow dr in dtCity.Rows)
            {
                City cityobj = new City();
                cityobj.ID = Convert.ToInt32(dr["ID"]);
                cityobj.CityName = Convert.ToString(dr["CITYNAME"]).Trim() == "" ? "" : Convert.ToString(dr["CITYNAME"]).Trim();

                objCity.Add(cityobj);
            }

            return objCity.ToList();
        }

        //public List<ResultInfo> GetOrderNumber()
        //{
        //    List<ResultInfo> obj = new List<ResultInfo>();
        //    OrderMasterData objOrderMasterData = new OrderMasterData();
        //    string result = objOrderMasterData.GetNewOrderNo();
        //    ResultInfo rinfo = new ResultInfo();
        //    rinfo.OrderNo = result;
        //    obj.Add(rinfo);
        //    return obj.ToList();
        //}

        public Int32 SaveShippingAddress(Int32 shippingid, Int32 distributorid, string fname, string lname, string company, string address1, string address2, string address3, string phone, string countryid, string stateid, string cityid, string pincode, string email, Int32 addresstype, Int32 userid, string cityname, Int32 orderid)
        {
            OrderMasterData objOrderMasterData = new OrderMasterData();



            Int32 shippingaddressid = 0;
            if (cityid == "0")
            {
                CityData cdata = new CityData();
                Int32 regionid = 1;
                CommanParametrs cps1 = new CommanParametrs();
                CommanParameter cp1;
                cp1 = new CommanParameter("@ID", DBNull.Value, DbType.Int32);
                cps1.Add(cp1);
                cp1 = new CommanParameter("@CountryID", countryid, DbType.Int32);
                cps1.Add(cp1);
                cp1 = new CommanParameter("@StateID", stateid, DbType.Int32);
                cps1.Add(cp1);
                cp1 = new CommanParameter("@CityName", cityname, DbType.String);
                cps1.Add(cp1);
                cp1 = new CommanParameter("@REGIONID", regionid, DbType.Int32);
                cps1.Add(cp1);
                cp1 = new CommanParameter("@USERID", userid, DbType.Int32);
                cps1.Add(cp1);
                // inserting record in the database and returning no of rows affected.
                cityid = Convert.ToString(maincls.DbCon.ExecuteProcNonQuery("PROC_CITYSAVE", cps1));

            }
            CommanParametrs cps = new CommanParametrs();
            // create instance for commanparameter class
            CommanParameter cp;
            cp = new CommanParameter("@ID", shippingid, DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@DistributorID", distributorid, DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@FName", fname, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@LName", lname, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@Company", company, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@ADDRESS1", address1, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@ADDRESS2", address2, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@ADDRESS3", address3, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@PHONE", phone, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@COUNTRYID", countryid, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@STATEID", stateid, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@CITYID", cityid, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@PINCODE", pincode, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@Email", email, DbType.String);
            cps.Add(cp);
            cp = new CommanParameter("@OrderID", orderid, DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@AddressType", addresstype, DbType.Int32, 0);
            cps.Add(cp);
            cp = new CommanParameter("@USERID", userid, DbType.Decimal);
            cps.Add(cp);
            shippingaddressid = maincls.DbCon.ExecuteProcNonQuery("PROC_ORDERSHIPPINGSAVE", cps);
            //GetdataInfo ig = new GetdataInfo();
            //ig.No = shippingaddressid;
            ////ig.OrderNo = orderid;
            //obj.Add(ig);
            return shippingaddressid;
        }

        public Int32 SaveOrderType(string CustAttrbutes, Int32 DelearId, string OrderDate, string OrderNo, Int32 OTypeId, decimal Amount, Int32 Status, decimal TaxAmt, decimal DisAmt, Int32 ShippingMethodId, decimal ShippingCost, string OrderNote, Int32 ShipID, string AccountNo, Int32 Currency, decimal DiscountId, decimal Discount)
        {
            Int32 iid = 0;
            List<GetInfo> obj = new List<GetInfo>();
            string scols = string.Empty;
            string scolvalues = string.Empty;
            if (CustAttrbutes.Length > 0)
            {
                string[] sAttributos = CustAttrbutes.Split('|');

                string[] sCols = new string[sAttributos.Count()];
                string[] sColValues = new string[sAttributos.Count()];

                int i = 0;
                foreach (string sAttrib in sAttributos)
                {
                    sCols[i] = sAttrib.Split(':')[0];
                    sColValues[i] = "'" + (sAttributos[i].Split(':')[1]).Replace("'", "''") + "'";
                    i++;
                }


                foreach (string scol in sCols)
                {
                    scols += "," + scol;
                }

                foreach (string scolvalue in sColValues)
                {
                    scolvalues += "," + scolvalue;
                }
            }
            string sQuery = string.Empty;
            string squery1 = string.Empty;
            string ofor = "M";
            if (Discount > 0)
            {
                squery1 = @"select Discount from tbl_DisConfig where Id=" + DiscountId;
                //Add value into cps object
                decimal dDiscount = Convert.ToDecimal(maincls.DbCon.ExecuteScaler(squery1));

                sQuery = @"Insert into tbl_OMst (DelearId, OrderDate, OrderNo, OrderTypeId, OrderAmount, OrderStatus, TaxAmount, DisAmount, ShippingMethodId, ShippingCost, OrderNote,ShipAddId, AccountNo,DiscountID,Discount  " + scols + @",CurrencyID,Ofrom)
                        values(" + DelearId + ",'" + OrderDate + "','" + OrderNo + "'," +
                                OTypeId + "," + Amount + "," + Status + "," + TaxAmt + "," +
                                DisAmt + "," + ShippingMethodId + "," + ShippingCost + ",'" + OrderNote.Replace("'", "''") + "'," + ShipID + ",'" + AccountNo + "'," + DiscountId + "," + dDiscount + "" + scolvalues + "," + Currency + ",'" + ofor + "')";
            }
            else
            {
                sQuery = @"Insert into tbl_OMst (DelearId, OrderDate, OrderNo, OrderTypeId, OrderAmount, OrderStatus, TaxAmount, DisAmount, ShippingMethodId, ShippingCost, OrderNote,ShipAddId, AccountNo  " + scols + @",CurrencyID,Ofrom)
                        values(" + DelearId + ",'" + OrderDate + "','" + OrderNo + "'," +
                               OTypeId + "," + Amount + "," + Status + "," + TaxAmt + "," +
                               DisAmt + "," + ShippingMethodId + "," + ShippingCost + ",'" + OrderNote.Replace("'", "''") + "'," + ShipID + ",'" + AccountNo + "'" + scolvalues + "," + Currency + ",'" + ofor + "')";
            }
            int exc = maincls.DbCon.ExecuteNonQuery(sQuery);
            if (exc > 0)
            {
                squery1 = @"select max(id) from tbl_omst where DelearId=" + DelearId;
                iid = Convert.ToInt32(maincls.DbCon.ExecuteScaler(squery1));

                return iid;
            }
            return iid;
        }

        public ResponseMessage SaveOrderTable(int orderfor, Int32 shippingid, string distributorid, string fname, string lname, string company, string address1, string address2, string address3, string phone, string countryid, string stateid, string cityid, string pincode, string email, Int32 addresstype, string userid, string cityname, string orderid, string Ordertypename, string CustAttrbutes, string DelearId, string OrderDate, Int32 OTypeId, string Amount, Int32 Status, decimal TaxAmt, decimal DisAmt, Int32 ShippingMethodId, decimal ShippingCost, string OrderNote, Int32 ShipID, string AccountNo, string Currency, string DiscountId, string Discount, List<OrderDetailProp> pdata)
        {
            // decrypt data

            var distid_en = DecryptAes(distributorid);
            Int32 distids = Convert.ToInt32(distid_en);

            string email_en = DecryptAes(email);

            var userid_en = DecryptAes(userid);
            Int32 userids = Convert.ToInt32(userid_en);

            var orderid_en = DecryptAes(orderid);
            Int32 orderids = Convert.ToInt32(orderid_en);

            var dealerid_en = DecryptAes(DelearId);
            Int32 dealerids = Convert.ToInt32(dealerid_en);

            var cur_en = DecryptAes(Currency);
            Int32 currencys = Convert.ToInt32(cur_en);

            var disid_en = DecryptAes(DiscountId);
            decimal discountids = Convert.ToDecimal(disid_en);

            var dis_en = DecryptAes(Discount);
            decimal discounts = Convert.ToDecimal(dis_en);

            var amt_en = DecryptAes(Amount);
            decimal amounts = Convert.ToDecimal(amt_en);


            if (orderfor == 0)
            {
                PropertyCollection prop = new PropertyCollection();
                try
                {
                    Int32 OrderIds = 0;
                    string OrderNo = string.Empty;
                    Int32 shippingaddressid = shippingid;

                    if (shippingid == 0)
                    {
                        shippingaddressid = SaveShippingAddress(shippingid, distids, fname, lname, company, address1, address2, address3, phone, countryid, stateid, cityid, pincode, email_en, addresstype, userids, cityname, orderids);
                    }

                    OrderMasterData objOrderMasterData = new OrderMasterData();
                    //AddressData objBus = new AddressData();
                    OrderMasterProp OrderProp = new OrderMasterProp();
                    if (orderfor == 0)
                        OrderNo = objOrderMasterData.GetNewOrderNo();
                    else
                        OrderNo = Ordertypename + objOrderMasterData.GetNewOrderNo();

                    OrderIds = SaveOrderType(CustAttrbutes, dealerids, OrderDate, OrderNo, OTypeId, amounts, Status, TaxAmt, DisAmt, ShippingMethodId, ShippingCost, OrderNote, shippingaddressid, AccountNo, currencys, discountids, discounts);
                    string msg = "Order No.: " + OrderNo + " Submitted Successfully";

                    if (OrderIds > 0)
                    {
                        int res = -1;
                        //OrderProp.OrderItem = getorderitem();
                        foreach (OrderDetailProp makes in pdata)
                        {
                            //Save Order Details
                            makes.OrderId = OrderIds;
                            OrderDetailData objOrder = new OrderDetailData();
                            res = objOrder.Insert(makes);
                            if (res < 0)
                            {
                                OrderProp.ID = OrderIds;
                                objOrderMasterData.Delete(OrderProp);
                                break;
                            }
                        }
                        if (res > 0)
                        {
                            prop.Add("OrderNo", OrderNo);
                            DateTime odt = new DateTime();
                            string dateFormat = Convert.ToString("MM/dd/yyyy");//Convert.ToString(HttpContext.Current.Session["GlobleDateCSFormat"]);
                            odt = OrderProp.OrderDate;
                            prop.Add("OrderDate", odt.ToString(dateFormat));
                            prop.Add("ShippingAddId", shippingaddressid);

                            if (!GenerateXML(OrderIds))
                                msg = "Record Not Saved due to some server error"; // Only for MUSA

                            //Return the success insert response

                            return new ResponseMessage { Status = true, OrderNo = OrderNo, Msg = msg };
                        }
                        else
                        {
                            return new ResponseMessage { Status = false, OrderNo = "", Msg = "Unable To Order Process Due To Server Response.\n Please Retry." };
                        }
                    }

                    else
                    {
                        //Return the success insert response
                        return new ResponseMessage { Status = false, OrderNo = "", Msg = "Unable To Order Process Due To Server Response.\n Please Retry." };
                    }
                }
                catch (Exception ex)
                {
                    //Return the error response
                    return new ResponseMessage { Status = false, OrderNo = ex.Message, Msg = "Unable To Order Process Due To Server Response.\n Please Retry." };//Catalog.ErrNumber.GetError(ex)
                }
            }
            else
            {
                PropertyCollection prop = new PropertyCollection();
                string msg = "";
                string sOrderid = string.Empty;
                Int32 OrderIds = 0;
                string OrderNo = string.Empty;
                Int32 shippingaddressid = shippingid;
                string orderall = string.Empty;
                try
                {

                    //Create object for FitemData
                    OrderMasterData objOrderMasterData = new OrderMasterData();
                    OrderMasterProp OrderProp = new OrderMasterProp();
                    AddressData objBus = new AddressData();
                    ////Getting DataTable of ordered Parts from session 
                    DataTable dtOrderList = new DataTable();
                    //List<OrderDetailProp> olist = new List<OrderDetailProp>();
                    foreach (OrderDetailProp par in pdata)
                    {
                        par.VendorId = Convert.ToInt32(maincls.DbCon.ExecuteScaler(@"select VendorID from tbl_part where ID=" + par.PartId));

                    }
                    // dtOrderList.Columns.Add("OrderNo");
                    var query = (from s in pdata
                                 group s by new { s.VendorId } into g
                                 select new
                                 {
                                     VendorId = g.Key.VendorId
                                 }).ToList();
                    int res = -1;
                    foreach (var vid in query)
                    {
                        var items = (from li in pdata
                                     where li.VendorId.Equals(vid.VendorId)
                                     select li).ToList<OrderDetailProp>();

                        if (shippingid == 0)
                        {
                            shippingaddressid = SaveShippingAddress(shippingid, distids, fname, lname, company, address1, address2, address3, phone, countryid, stateid, cityid, pincode, email_en, addresstype, userids, cityname, orderids);
                            shippingid = shippingaddressid;
                        }
                        OrderNo = Ordertypename + objOrderMasterData.GetNewOrderNo();
                        //Save Order Type

                        OrderIds = SaveOrderType(CustAttrbutes, dealerids, OrderDate, OrderNo, OTypeId, amounts, Status, TaxAmt, DisAmt, ShippingMethodId, ShippingCost, OrderNote, shippingaddressid, AccountNo, currencys, discountids, discounts);
                        // msg = "Order No.: " + orderall + " Submitted Successfully";
                        if (OrderIds > 0)
                        {
                            res = -1;

                            //OrderProp.OrderItem = getorderitem();
                            foreach (OrderDetailProp makes in items)
                            {
                                //Save Order Details
                                makes.OrderId = OrderIds;
                                makes.ModelName = OrderNo;
                                OrderDetailData objOrder = new OrderDetailData();
                                res = objOrder.Insert(makes);

                                if (res < 0)
                                {
                                    sOrderid += ", " + OrderIds;
                                    foreach (string oid in sOrderid.Split(','))
                                    {
                                        if (string.IsNullOrEmpty(oid)) continue;
                                        OrderProp.ID = Convert.ToInt32(OrderIds);
                                        objOrderMasterData.Delete(OrderProp);

                                    }
                                    break;
                                }
                                //DataRow[] drs = dtOrderList.Select("ID=" + makes.PartId);
                                //foreach (DataRow dr in drs)
                                //    dr["OrderNo"] = OrderProp.OrderNo;

                            }

                            if (res > 0)
                            {
                                if (string.IsNullOrEmpty(orderall))
                                {
                                    sOrderid += OrderIds;
                                    orderall += OrderNo;//OrderProp.OrderNo;

                                }
                                else
                                {
                                    sOrderid += ", " + OrderIds;
                                    orderall += ", " + OrderNo;// OrderProp.OrderNo;

                                }

                                // orderall += OrderNo;


                                if (!GenerateXML(OrderIds))
                                    msg = "Record Not Saved due to some server error";
                            }
                            else
                            {

                            }
                        }
                        if (res < 0)
                        {
                            break;
                        }
                        //}
                    }

                    if (res > 0)
                    {
                        prop.Add("OrderNo", OrderNo);
                        DateTime odt = new DateTime();
                        string dateFormat = Convert.ToString("MM/dd/yyyy");//Convert.ToString(HttpContext.Current.Session["GlobleDateCSFormat"]);
                        odt = OrderProp.OrderDate;
                        prop.Add("OrderDate", odt.ToString(dateFormat));
                        prop.Add("ShippingAddId", OrderProp.ShippingAddress.ID);

                        // HttpContext.Current.Session["AccOrderList"] = dtOrderList;

                    }
                    else
                    {
                        return new ResponseMessage { Status = false, OrderNo = "", Msg = "Unable To Order Process Due To Server Response.\n Please Retry." };
                    }

                }
                catch (Exception ex)
                {
                    //Return the error response
                    msg = "No Vendor Available, Please Contact OEM Administrator";
                    return new ResponseMessage { Status = false, OrderNo = "", Msg = msg };
                }
                // orderall = orderall.Remove(orderall.Length-1);
                //orderall = orderall.Substring(0, orderall.Length - 1);

                return new ResponseMessage { Status = true, OrderNo = orderall, Msg = "Order No.: " + orderall + " Submitted Successfully" };
            }


        }

        private void buildorderdetail(List<PartDatainfo> pdata)
        {
            List<OrderDetailProp> olist = new List<OrderDetailProp>();

            foreach (PartDatainfo par in pdata)
            {
                OrderDetailProp orddtl = new OrderDetailProp();
                orddtl.ID = Convert.ToInt32(par.ID);
                orddtl.PartId = Convert.ToInt32(par.PartId);
                orddtl.Price = Convert.ToDecimal(par.Price);
                orddtl.Qty = Convert.ToInt32(par.Quantity);
                orddtl.Amount = Convert.ToDecimal(par.Amount);
                orddtl.TaxAmount = Convert.ToDecimal(par.TaxAmount);
                orddtl.VendorId = Convert.ToInt32(maincls.DbCon.ExecuteScaler(@"select VendorID from tbl_part where ID=" + orddtl.PartId));
                olist.Add(orddtl);
            }
            //OrderProp.OrderItem = olist;
        }

        private bool GenerateXML(int orderId)
        {
            try
            {
                OrderListData objOLData = new OrderListData();
                DataSet dsVal = new DataSet();

                dsVal = objOLData.GetDataForXML(orderId.ToString());
                dsVal.DataSetName = "Order";
                dsVal.Tables[0].TableName = "order_header";
                dsVal.Tables[1].TableName = "billing_information";
                dsVal.Tables[2].TableName = "billing_address";
                dsVal.Tables[3].TableName = "shipping_address";
                dsVal.Tables[4].TableName = "house_account";
                dsVal.Tables[5].TableName = "purchase_order";
                dsVal.Tables[6].TableName = "order_item";


                //get the ftp details
                FtpDetailData ftpDetailData = new FtpDetailData();
                FtpDetailProp ftpDetailProp = new FtpDetailProp();
                ftpDetailProp.FtpUsedFor = (int)FTPFolders.Ordered_XML;
                DataTable dtFtp = ftpDetailData.List(ftpDetailProp);
                string[] ftpdetail = null;
                if (dtFtp != null && dtFtp.Rows.Count > 0)
                    ftpdetail = new string[] { Convert.ToString(dtFtp.Rows[0]["HostName"]), Convert.ToString(dtFtp.Rows[0]["UserName"]), Convert.ToString(dtFtp.Rows[0]["password"]), Convert.ToString(dtFtp.Rows[0]["subfolder"]) };

                return maincls.WriteXML(dsVal, System.Web.HttpContext.Current.Server.MapPath("~/Order_files/"), ftpdetail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private List<xmlInfo> GenerateXML(int orderId)
        //{
        //    List<xmlInfo> objxml = new List<xmlInfo>();
        //    OrderListData objOLData = new OrderListData();
        //    DataSet dsVal = new DataSet();

        //    dsVal = objOLData.GetDataForXML(orderId.ToString());
        //    dsVal.DataSetName = "Order";
        //    dsVal.Tables[0].TableName = "order_header";
        //    dsVal.Tables[1].TableName = "billing_information";
        //    dsVal.Tables[2].TableName = "billing_address";
        //    dsVal.Tables[3].TableName = "shipping_address";
        //    dsVal.Tables[4].TableName = "house_account";
        //    dsVal.Tables[5].TableName = "purchase_order";
        //    dsVal.Tables[6].TableName = "order_item";


        //    //get the ftp details
        //    FtpDetailData ftpDetailData = new FtpDetailData();
        //    FtpDetailProp ftpDetailProp = new FtpDetailProp();
        //    ftpDetailProp.FtpUsedFor = (int)FTPFolders.Ordered_XML;
        //    DataTable dtFtp = ftpDetailData.List(ftpDetailProp);
        //    string[] ftpdetail = null;
        //    if (dtFtp != null && dtFtp.Rows.Count > 0)
        //        ftpdetail = new string[] { Convert.ToString(dtFtp.Rows[0]["HostName"]), Convert.ToString(dtFtp.Rows[0]["UserName"]), Convert.ToString(dtFtp.Rows[0]["password"]), Convert.ToString(dtFtp.Rows[0]["subfolder"]) };

        //    bool tt = maincls.WriteXML(dsVal, System.Web.HttpContext.Current.Server.MapPath("~/Order_files/"), ftpdetail);
        //    xmlInfo xm = new xmlInfo();
        //    xm.status = tt;
        //    objxml.Add(xm);
        //    return objxml.ToList();
        //}

        public List<OrderMasterInfo> OrderMasterList(string distributorid, Int32 lngid)
        {
            List<OrderMasterInfo> orderobj = new List<OrderMasterInfo>();

            var distid_en = DecryptAes(distributorid);
            Int32 ids = Convert.ToInt32(distid_en);

            // create new object for commanParameters class
            CommanParametrs objcps = new CommanParametrs();
            // create instance for commanparameter class
            objcps.Add(new CommanParameter("@ID", 0, DbType.Decimal));
            objcps.Add(new CommanParameter("@OrderTypeId", 0, DbType.Decimal));
            objcps.Add(new CommanParameter("@DealerId", ids, DbType.Decimal));
            objcps.Add(new CommanParameter("@ShippingMethodId", null, DbType.Decimal));
            objcps.Add(new CommanParameter("@LngId", lngid, DbType.Int32));
            // returning the list of OrderType.
            DataTable dt = maincls.DbCon.GetProcTable("Proc_OrderMasterList", objcps, "tbl_OrderMst");
            DataView dv = dt.DefaultView;
            dv.Sort = "ID desc";
            DataTable dtt = dv.ToTable();
            foreach (DataRow dr in dtt.Rows)
            {
                OrderMasterInfo omonfo = new OrderMasterInfo();
                omonfo.Id = Convert.ToInt32(dr["ID"]);
                omonfo.DealerId = Convert.ToInt32(dr["DELEARID"]);
                omonfo.DealerName = Convert.ToString(dr["DEALERNAME"]) == "" ? "" : Convert.ToString(dr["DEALERNAME"]);
                omonfo.DealerCode = Convert.ToString(dr["DEALERCODE"]) == "" ? "" : Convert.ToString(dr["DEALERCODE"]);
                omonfo.OrderTypeId = Convert.ToInt32(dr["ORDERTYPEID"]);
                omonfo.OderType = Convert.ToString(dr["ORDERTYPE"]) == "" ? "" : Convert.ToString(dr["ORDERTYPE"]);
                omonfo.OrderAmount = Convert.ToDecimal(dr["ORDERAMOUNT"]);
                omonfo.OrderNo = Convert.ToString(dr["ORDERNO"]) == "" ? "" : Convert.ToString(dr["ORDERNO"]);
                omonfo.OrderDate = Convert.ToString(dr["ORDERDATE"]) == "" ? "" : Convert.ToString(dr["ORDERDATE"]);
                omonfo.OrderStatus = Convert.ToInt32(dr["ORDERSTATUS"]);
                omonfo.ShippingMethodId = Convert.ToInt32(dr["SHIPPINGMETHODID"]);
                omonfo.MethodName = Convert.ToString(dr["METHODNAME"]) == "" ? "" : Convert.ToString(dr["METHODNAME"]);
                omonfo.CurrencySymbol = Convert.ToString(dr["CURRENCYSYMBOL"]) == "" ? " " : Convert.ToString(dr["CURRENCYSYMBOL"]);
                omonfo.CarrierId = Convert.ToInt32(dr["CARRIERID"]);

                orderobj.Add(omonfo);


            }
            return orderobj.ToList();
        }

        public List<OrderMasterInfo> OrderMasterListNew(string distributorid, Int32 lngid, Int32 page, string fromdate, string todate)
        {
            var distid_en = DecryptAes(distributorid);
            Int32 ids = Convert.ToInt32(distid_en);

            List<OrderMasterInfo> orderobj = new List<OrderMasterInfo>();

            int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);
            // create new object for commanParameters class
            CommanParametrs objcps = new CommanParametrs();
            // create instance for commanparameter class
            objcps.Add(new CommanParameter("@ID", 0, DbType.Decimal));
            objcps.Add(new CommanParameter("@OrderTypeId", 0, DbType.Decimal));
            objcps.Add(new CommanParameter("@DealerId", ids, DbType.Decimal));
            objcps.Add(new CommanParameter("@ShippingMethodId", null, DbType.Decimal));
            if (fromdate != "")
                objcps.Add(new CommanParameter("@FDate", Convert.ToDateTime(fromdate), DbType.DateTime, DBNull.Value));
            if (todate != "")
                objcps.Add(new CommanParameter("@TDate", Convert.ToDateTime(todate), DbType.DateTime, DBNull.Value));
            objcps.Add(new CommanParameter("@Page", page, DbType.Int32));
            objcps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32));
            // returning the list of OrderType.
            DataTable dt = maincls.DbCon.GetProcTable("Proc_OrderMasterListNew", objcps, "tbl_OrderMst");
            DataView dv = dt.DefaultView;
            dv.Sort = "ID desc";
            DataTable dtt = dv.ToTable();
            foreach (DataRow dr in dtt.Rows)
            {
                OrderMasterInfo omonfo = new OrderMasterInfo();
                omonfo.Id = Convert.ToInt32(dr["ID"]);
                omonfo.TotalRecords = Convert.ToInt32(dr["TotalRows"]);
                omonfo.DealerId = Convert.ToInt32(dr["DELEARID"]);
                omonfo.DealerName = Convert.ToString(dr["DEALERNAME"]) == "" ? "" : Convert.ToString(dr["DEALERNAME"]);
                omonfo.DealerCode = Convert.ToString(dr["DEALERCODE"]) == "" ? "" : Convert.ToString(dr["DEALERCODE"]);
                omonfo.OrderTypeId = Convert.ToInt32(dr["ORDERTYPEID"]);
                omonfo.OderType = Convert.ToString(dr["ORDERTYPE"]) == "" ? "" : Convert.ToString(dr["ORDERTYPE"]);
                omonfo.OrderAmount = Convert.ToDecimal(dr["ORDERAMOUNT"]);
                omonfo.OrderNo = Convert.ToString(dr["ORDERNO"]) == "" ? "" : Convert.ToString(dr["ORDERNO"]);
                omonfo.OrderDate = Convert.ToString(dr["ORDERDATE"]) == "" ? "" : Convert.ToString(dr["ORDERDATE"]);
                omonfo.OrderStatus = Convert.ToInt32(dr["ORDERSTATUS"]);
                omonfo.ShippingMethodId = Convert.ToInt32(dr["SHIPPINGMETHODID"]);
                omonfo.MethodName = Convert.ToString(dr["METHODNAME"]) == "" ? "" : Convert.ToString(dr["METHODNAME"]);
                omonfo.CurrencySymbol = Convert.ToString(dr["CURRENCYSYMBOL"]) == "" ? " " : Convert.ToString(dr["CURRENCYSYMBOL"]);
                omonfo.CarrierId = Convert.ToInt32(dr["CARRIERID"]);

                orderobj.Add(omonfo);


            }
            return orderobj.ToList();
        }

        public List<UserInfo> DistributorInfoLoad(string userid)
        {
            List<UserInfo> uinfoobj = new List<UserInfo>();

            var userid_en = DecryptAes(userid);
            Int32 ids = Convert.ToInt32(userid_en);

            CommanParametrs objcps = new CommanParametrs();
            CommanParameter objcp;

            objcp = new CommanParameter("@ID", ids, DbType.Int32);
            objcps.Add(objcp);
            DataTable dtUser = maincls.DbCon.GetProcTable("PROC_USERGET", objcps, "tbl_User");

            //Declareation of Object of User Property class
            UserProp obUserGetjprop = new UserProp();
            //Store Id of User into ID of User Property Class
            obUserGetjprop.ID = ids;
            //checking if records in table more than zero   
            if (dtUser.Rows.Count > 0)
            {
                UserInfo ui = new UserInfo();
                //User First Name store into User First Name property of User Property class
                string UFName = Convert.ToString(dtUser.Rows[0]["UFName"]);
                //User Last Name store into User Last Name property of User Property class
                string ULName = Convert.ToString(dtUser.Rows[0]["ULName"]);
                ui.Name = UFName + " " + ULName;
                ui.Code = Convert.ToString(dtUser.Rows[0]["LOGINID"]);
                //Password store into Address property of User Property class
                string Address = Convert.ToString(dtUser.Rows[0]["Address"]);
                ui.Address = Address;

                //CountryId store into CountryId property of User Property class
                if (dtUser.Rows[0]["CountryId"] != DBNull.Value)
                {
                    Int32 CountryId = Convert.ToInt32(dtUser.Rows[0]["CountryId"]);
                    CountryData cd = new CountryData();
                    DataTable dtcunlist = cd.CountryList();
                    ui.Country = Convert.ToString(dtcunlist.Select("ID=" + CountryId)[0].ItemArray[2]);

                }
                if (dtUser.Rows[0]["StateId"] != DBNull.Value)
                {
                    //StateId store into StateId property of User Property class
                    Int32 StateId = Convert.ToInt32(dtUser.Rows[0]["StateId"]);
                    StateData sd = new StateData();
                    //Asigning the Country Id to CountryId of StateProp class Property
                    Int32 CountryId = Convert.ToInt32(dtUser.Rows[0]["CountryId"]);
                    //Binding combobox with data table
                    DataTable dtStatelist = sd.GetStateDetail(CountryId);
                    ui.State = Convert.ToString(dtStatelist.Select("ID=" + StateId)[0].ItemArray[1]);
                }
                if (dtUser.Rows[0]["CityId"] != DBNull.Value)
                {
                    //CityId store into CityId property of User Property class
                    Int32 CityId = Convert.ToInt32(dtUser.Rows[0]["CityId"]);
                    CityData cd = new CityData();
                    //Binding combobox with data table
                    DataTable dtCityList = cd.CityList();
                    ui.City = Convert.ToString(dtCityList.Select("ID=" + CityId)[0].ItemArray[2]);
                }

                uinfoobj.Add(ui);

            }
            else
                //if there is no record exist than return null
                return null;
            return uinfoobj.ToList();
        }

        public List<UserInfo> DistributorContactPerson(string userid)
        {
            var userid_en = DecryptAes(userid);
            Int32 ids = Convert.ToInt32(userid_en);

            List<UserInfo> uinfoobj = new List<UserInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@DistributorID", ids, DbType.Int32));
            // Gettting the record of the distributor contact person whose id is sent.
            DataTable dtDcp = maincls.DbCon.GetProcTable("PROC_DISCONTPERSONLIST", cps, "DistContPer");
            if (dtDcp.Rows.Count > 0)
            {
                UserInfo ui = new UserInfo();
                string UFName = Convert.ToString(dtDcp.Rows[0]["FIRSTNAME"]) == "" ? "" : Convert.ToString(dtDcp.Rows[0]["FIRSTNAME"]);
                //User Last Name store into User Last Name property of User Property class
                string ULName = Convert.ToString(dtDcp.Rows[0]["LASTNAME"]) == "" ? "" : Convert.ToString(dtDcp.Rows[0]["LASTNAME"]);
                ui.Name = UFName + " " + ULName;
                TitleData td = new TitleData();
                Int32 titleid = Convert.ToInt32(dtDcp.Rows[0]["TITLEID"]);
                DataTable dttitlelist = td.Titlelist();
                ui.Title = Convert.ToString(dttitlelist.Select("ID=" + titleid)[0].ItemArray[2]) == "" ? "" : Convert.ToString(dttitlelist.Select("ID=" + titleid)[0].ItemArray[2]);
                ui.Email = Convert.ToString(dtDcp.Rows[0]["EMAIL"]) == "" ? "" : Convert.ToString(dtDcp.Rows[0]["EMAIL"]);
                ui.Contact = Convert.ToString(dtDcp.Rows[0]["MOBILENO"]) == "" ? "" : Convert.ToString(dtDcp.Rows[0]["MOBILENO"]);
                uinfoobj.Add(ui);
            }
            return uinfoobj.ToList();
        }

        public List<OrderInfo> GetOrderDetailList(string OrderId)
        {
            var oid_en = DecryptAes(OrderId);
            Int32 oids = Convert.ToInt32(oid_en);

            List<OrderInfo> objorder = new List<OrderInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@OrderId", oids, DbType.Decimal));
            DataTable dtDcp = maincls.DbCon.GetProcTable("PROC_OMDETAILLIST", cps, "tbl_OrderDtl");
            foreach (DataRow dr in dtDcp.Rows)
            {
                OrderInfo orderinfo = new OrderInfo();
                orderinfo.Id = Convert.ToInt32(dr["ID"]);
                orderinfo.PartId = Convert.ToInt32(dr["PARTID"]);
                orderinfo.PartNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                orderinfo.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                orderinfo.Price = Convert.ToDecimal(dr["PRICE"]) == 0 ? 0 : Convert.ToDecimal(dr["PRICE"]);
                orderinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                orderinfo.ShipQty = Convert.ToInt32(dr["SHIPQTY"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPQTY"]);
                orderinfo.BackOrderQty = Convert.ToInt32(dr["BACKORDERQTY"]) == 0 ? 0 : Convert.ToInt32(dr["BACKORDERQTY"]);
                orderinfo.Amount = Convert.ToDecimal(dr["AMOUNT"]) == 0 ? 0 : Convert.ToDecimal(dr["AMOUNT"]);
                orderinfo.TaxAmount = Convert.ToDecimal(dr["TAXAMOUNT"]) == 0 ? 0 : Convert.ToDecimal(dr["TAXAMOUNT"]);
                orderinfo.CurrencySymbol = Convert.ToString(dr["CURRENCYSYMBOL"]) == "" ? " " : Convert.ToString(dr["CURRENCYSYMBOL"]);
                objorder.Add(orderinfo);
            }
            return objorder.ToList();

        }

        public List<OrderReports> OrderReports(string OrderNo, string OrderId)
        {
            string OrderNo_en = DecryptAes(OrderNo);

            var oid_en = DecryptAes(OrderId);
            Int32 oids = Convert.ToInt32(oid_en);

            List<OrderReports> objorder = new List<OrderReports>();
            OrderListData objListDetail = new OrderListData();
            DataTable dtdetails = objListDetail.GetOrderReports(OrderNo_en, oids);

            foreach (DataRow dr in dtdetails.Rows)
            {
                OrderReports objReports = new OrderReports();
                objReports.Id = Convert.ToInt32(dr["ID"]);
                objReports.OrderNo = Convert.ToString(dr["ORDERNO"]) == "" ? "" : Convert.ToString(dr["ORDERNO"]);
                objReports.OrderDate = Convert.ToString(dr["ORDERDATE"]) == "" ? "" : Convert.ToString(dr["ORDERDATE"]);
                objReports.DealerId = Convert.ToInt32(dr["DELEARID"]);
                objReports.OtDesk = Convert.ToString(dr["OTDESC"]) == "" ? "" : Convert.ToString(dr["OTDESC"]);
                objReports.OtDeskId = Convert.ToInt32(dr["OTDESCID"]);
                objReports.OrderStatus = Convert.ToInt32(dr["ORDERSTATUS"]);
                objReports.Code = Convert.ToString(dr["CODE"]) == "" ? "" : Convert.ToString(dr["CODE"]);
                objReports.CompanyName = Convert.ToString(dr["COMPANYNAME"]) == "" ? "" : Convert.ToString(dr["COMPANYNAME"]);
                objReports.Ufname = Convert.ToString(dr["UFNAME"]) == "" ? "" : Convert.ToString(dr["UFNAME"]);
                objReports.Ulname = Convert.ToString(dr["ULNAME"]) == "" ? "" : Convert.ToString(dr["ULNAME"]);
                objReports.ShippingRemarks = Convert.ToString(dr["SHIPPINGREMARKS"]) == "" ? "" : Convert.ToString(dr["SHIPPINGREMARKS"]);
                objReports.ShippingMethodId = Convert.ToInt32(dr["SHIPPINGMETHODID"]);
                objReports.Note = Convert.ToString(dr["NOTE"]) == "" ? "" : Convert.ToString(dr["NOTE"]);
                objReports.Otype = Convert.ToString(dr["OTYPE"]) == "" ? "" : Convert.ToString(dr["OTYPE"]);
                objReports.OrderType = Convert.ToString(dr["ORDERTYPE"]) == "" ? "" : Convert.ToString(dr["ORDERTYPE"]);
                objReports.ShipAddId = Convert.ToInt32(dr["SHIPADDID"]);
                objReports.PendingSince = Convert.ToInt32(dr["PENDINGSINCE"]);
                objReports.OrderValue = Convert.ToString(Math.Round(Convert.ToDecimal(dr["ORDERVALUE"]), 2)) == "" ? "" : Convert.ToString(Math.Round(Convert.ToDecimal(dr["ORDERVALUE"]), 2));
                objReports.Percentage = Convert.ToString(dr["DISPERCENTAGE"]) == "N/A" ? "" : Convert.ToString(dr["DISPERCENTAGE"]);
                objReports.NetValue = Convert.ToString(Math.Round(Convert.ToDecimal(dr["NETVALUE"]), 2)) == "" ? objReports.OrderValue : Convert.ToString(Math.Round(Convert.ToDecimal(dr["NETVALUE"]), 2));
                objReports.DiscountValue = Convert.ToString(dr["DISCOUNT"]) == "N/A" ? "N/A" : Convert.ToString(Math.Round(Convert.ToDecimal(dr["DISCOUNT"]), 2)) + " (" + objReports.Percentage + "% )";
                objReports.PONO = Convert.ToString(dr["PO_NO"]) == "" ? "" : Convert.ToString(dr["PO_NO"]);
                string sql = @"select MethodName from tbl_CarrierMethod where ID=" + objReports.ShippingMethodId;
                DataTable dt = maincls.DbCon.GetTable(sql, "tbl_CarrierMethod");
                foreach (DataRow dr1 in dt.Rows)
                {

                    objReports.MethodName = dr1["METHODNAME"].ToString();

                }
                objorder.Add(objReports);
            }
            return objorder.ToList();
        }

        public List<ShipAddress> GetShipOrderDetails(string ID)
        {
            var id_en = DecryptAes(ID);
            Int32 ids = Convert.ToInt32(id_en);


            List<ShipAddress> objshipaddress = new List<ShipAddress>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@ID", ids, DbType.Int32));
            DataTable dt = maincls.DbCon.GetProcTable("PROC_GETORDERSHIPPINGDETAILS", cps, "tbl_OrderShippingAddress");
            foreach (DataRow dr in dt.Rows)
            {
                ShipAddress distinfo = new ShipAddress();
                distinfo.ID = Convert.ToInt32(dr["ADDRESSID"]);
                distinfo.FistName = Convert.ToString(dr["FNAME"]) == "" ? "" : Convert.ToString(dr["FNAME"]);
                distinfo.LastName = Convert.ToString(dr["LNAME"]) == "" ? "" : Convert.ToString(dr["LNAME"]);
                distinfo.Company = Convert.ToString(dr["SHIPPINGCOMPANY"]) == "" ? "" : Convert.ToString(dr["SHIPPINGCOMPANY"]);
                distinfo.Address1 = Convert.ToString(dr["SADDRESS1"]) == "" ? "" : Convert.ToString(dr["SADDRESS1"]);
                distinfo.Address2 = Convert.ToString(dr["SADDRESS2"]) == "" ? "" : Convert.ToString(dr["SADDRESS2"]);
                distinfo.Address3 = Convert.ToString(dr["SADDRESS3"]) == "" ? "" : Convert.ToString(dr["SADDRESS3"]);
                distinfo.CountryName = Convert.ToString(dr["SCOUNTRY"]) == "" ? "" : Convert.ToString(dr["SCOUNTRY"]);
                distinfo.StateName = Convert.ToString(dr["SSTATE"]) == "" ? "" : Convert.ToString(dr["SSTATE"]);
                distinfo.CityName = Convert.ToString(dr["SCITY"]) == "" ? "" : Convert.ToString(dr["SCITY"]);
                distinfo.Phone = Convert.ToString(dr["SPHONE"]) == "" ? "" : Convert.ToString(dr["SPHONE"]);
                distinfo.Email = Convert.ToString(dr["SEMAIL"]) == "" ? "" : Convert.ToString(dr["SEMAIL"]);
                distinfo.PostalCode = Convert.ToString(dr["SPINCODE"]) == "" ? "" : Convert.ToString(dr["SPINCODE"]);
                objshipaddress.Add(distinfo);
            }
            return objshipaddress.ToList();
        }

        public List<OrderQuantity> GetOrderDetailListCount(string OrderId)
        {
            var oid_en = DecryptAes(OrderId);
            Int32 oids = Convert.ToInt32(oid_en);

            List<OrderQuantity> mlist = new List<OrderQuantity>();
            CommanParametrs objcps = new CommanParametrs();
            objcps.Add(new CommanParameter("@OrderId", oids, DbType.Decimal));

            // returning the list of OrderType.
            DataTable dt = maincls.DbCon.GetProcTable("PROC_OMDETAILLIST", objcps, "tbl_OrderDtl");
            foreach (DataRow dr in dt.Rows)
            {
                OrderQuantity oq = new OrderQuantity();
                oq.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                oq.ShipQty = Convert.ToInt32(dr["SHIPQTY"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPQTY"]);
                oq.BackOrderQty = Convert.ToInt32(dr["BACKORDERQTY"]) == 0 ? 0 : Convert.ToInt32(dr["BACKORDERQTY"]);
                mlist.Add(oq);
            }
            return mlist.ToList();
        }

        public List<Partordinfo> GetShipInfoDetails(string OrderNo)
        {
            List<Partordinfo> objpart = new List<Partordinfo>();
            try
            {
                string OrderNo_en = DecryptAes(OrderNo);

                OrderListData objListDetail = new OrderListData();
                DataTable dtdetails = objListDetail.GetShipInfoDetails(OrderNo_en);

                //var part = (from p in dtdetails.AsEnumerable()
                //            select new
                //            {
                //                PARTID = p.Field<decimal>("PARTID"),
                //                PARTNO = p.Field<String>("PARTNO"),
                //                DESCRIPTION = p.Field<String>("DESCRIPTION"),
                //                QTY = p.Field<decimal>("TOTALORDERQUANTITY"),
                //                INVOICENO = p.Field<string>("INVOICENO"),
                //                INVOICEDATE = p.Field<DateTime>("INVOICEDATE"),
                //                SHIPID = p.Field<decimal>("SHIPID"),
                //                ATTID = p.Field<decimal>("ATTID")
                //            }).Distinct();

                //var q = from p in part
                //        select new
                //        {
                //            p.PARTNO,
                //            p.DESCRIPTION,
                //            p.QTY,
                //            p.SHIPID,
                //            p.INVOICENO,
                //            p.INVOICEDATE,
                //            p.ATTID,
                //            Shipment = from s in dtdetails.AsEnumerable()
                //                       where s.Field<decimal>("PARTID").Equals(p.PARTID)
                //                       select new { SHIPID = s.Field<decimal>("SHIPID"), SHIPNO = s.Field<String>("SHIPNO"), SHIPDATE = s.Field<DateTime>("SHIPDATE"), METHODNAME = s.Field<String>("METHODNAME"), SHIPQTY = s.Field<decimal>("SHIPQTY"), SHIPINV = s.Field<String>("INVOICENO"), SHIPINVDATE = s.Field<DateTime>("INVOICEDATE"), ATTID = s.Field<decimal>("ATTID") },
                //            BALQTY = from s in dtdetails.AsEnumerable()
                //                     where s.Field<decimal>("PARTID").Equals(p.PARTID)
                //                     group s by s.Field<decimal>("PARTID") into g
                //                     select new { BALQTY = p.QTY - g.Sum(x => x.Field<decimal>("SHIPQTY")) }.BALQTY


                //        };
                foreach (DataRow dr in dtdetails.Rows)
                {

                    Partordinfo pord = new Partordinfo();
                    pord.ID = Convert.ToInt32(dr["ID"]);
                    pord.ATTID = Convert.ToInt32(dr["ATTID"]) == 0 ? 0 : Convert.ToInt32(dr["ATTID"]);
                    pord.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                    pord.InvoiceDate = Convert.ToString(dr["INVOICEDATE"]) == "" ? "" : Convert.ToString(dr["INVOICEDATE"]);
                    pord.InvoiceNo = Convert.ToString(dr["INVOICENO"]) == "" ? "" : Convert.ToString(dr["INVOICENO"]);
                    pord.MethodName = Convert.ToString(dr["METHODNAME"]) == "" ? "" : Convert.ToString(dr["METHODNAME"]);
                    pord.PartId = Convert.ToInt32(dr["PARTID"]) == 0 ? 0 : Convert.ToInt32(dr["PARTID"]);
                    pord.PartNo = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                    pord.QTY = Convert.ToInt32(dr["TOTALORDERQUANTITY"]) == 0 ? 0 : Convert.ToInt32(dr["TOTALORDERQUANTITY"]);
                    pord.ShapOrderNo = Convert.ToString(dr["SAP_ORDERNO"]) == "" ? "" : Convert.ToString(dr["SAP_ORDERNO"]);
                    pord.ShipDate = Convert.ToString(dr["SHIPDATE"]) == "" ? "" : Convert.ToString(dr["SHIPDATE"]);
                    pord.SHIPID = Convert.ToInt32(dr["SHIPID"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPID"]);
                    pord.ShipMethodid = Convert.ToInt32(dr["SHIPMETHODID"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPMETHODID"]);
                    pord.ShipNo = Convert.ToString(dr["SHIPNO"]) == "" ? "" : Convert.ToString(dr["SHIPNO"]);
                    pord.ShipQTY = Convert.ToInt32(dr["SHIPQTY"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPQTY"]);
                    pord.BALQTY = pord.QTY - pord.ShipQTY;

                    objpart.Add(pord);
                }

                return objpart.ToList();

            }
            catch { throw; }
        }

        public List<Partordinfo> GetShipInfoDetailsshipwise(string OrderNo)
        {
            List<Partordinfo> objpart = new List<Partordinfo>();
            try
            {
                string OrderNo_en = DecryptAes(OrderNo);
                OrderListData objListDetail = new OrderListData();
                DataTable dtdetails = objListDetail.GetShipInfoDetails(OrderNo_en);

                //var part = (from p in dtdetails.AsEnumerable()
                //            select new
                //            {
                //                //QTY = p.Field<decimal>("TOTALORDERQUANTITY"),
                //                SHIPNO = p.Field<String>("SHIPNO"),
                //                METHODNAME = p.Field<String>("METHODNAME"),
                //                SHIPDATE = p.Field<DateTime>("SHIPDATE"),
                //                SHIPID = p.Field<decimal>("SHIPID"),
                //                INVOICENO = p.Field<string>("INVOICENO"),
                //                INVOICEDATE = p.Field<DateTime>("INVOICEDATE"),
                //                ATTID = p.Field<decimal>("ATTID"),
                //                SHAPORDERNO = p.Field<string>("SAP_OrderNo") == null ? "False" : p.Field<string>("SAP_OrderNo")
                //            }).Distinct();

                //var q = from p in part
                //        select new
                //        {
                //            //p.PARTNO,
                //            //p.DESCRIPTION,
                //            p.SHIPNO,
                //            p.SHIPID,
                //            p.METHODNAME,
                //            p.SHIPDATE,
                //            p.INVOICENO,
                //            p.INVOICEDATE,
                //            p.ATTID,
                //            p.SHAPORDERNO,
                //            Shipment = from s in dtdetails.AsEnumerable()
                //                       where s.Field<decimal>("SHIPID").Equals(p.SHIPID)
                //                       select new { PARTNO = s.Field<String>("PARTNO"), DESCRIPTION = s.Field<String>("DESCRIPTION"), SHIPQTY = s.Field<decimal>("SHIPQTY") },
                //            QTY = from s in dtdetails.AsEnumerable()
                //                  where s.Field<decimal>("SHIPID").Equals(p.SHIPID)
                //                  group s by s.Field<decimal>("PARTID") into g
                //                  select new { BALQTY = g.Sum(x => x.Field<decimal>("SHIPQTY")) }.BALQTY
                //        };
                foreach (DataRow dr in dtdetails.Rows)
                {

                    Partordinfo pord = new Partordinfo();
                    pord.ID = Convert.ToInt32(dr["ID"]);
                    pord.ATTID = Convert.ToInt32(dr["ATTID"]) == 0 ? 0 : Convert.ToInt32(dr["ATTID"]);
                    pord.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                    pord.InvoiceDate = Convert.ToString(dr["INVOICEDATE"]) == "" ? "" : Convert.ToString(dr["INVOICEDATE"]);
                    pord.InvoiceNo = Convert.ToString(dr["INVOICENO"]) == "" ? "" : Convert.ToString(dr["INVOICENO"]);
                    pord.MethodName = Convert.ToString(dr["METHODNAME"]) == "" ? "" : Convert.ToString(dr["METHODNAME"]);
                    pord.PartId = Convert.ToInt32(dr["PARTID"]) == 0 ? 0 : Convert.ToInt32(dr["PARTID"]);
                    pord.PartNo = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                    pord.QTY = Convert.ToInt32(dr["TOTALORDERQUANTITY"]) == 0 ? 0 : Convert.ToInt32(dr["TOTALORDERQUANTITY"]);
                    pord.ShapOrderNo = Convert.ToString(dr["SAP_ORDERNO"]) == "" ? "" : Convert.ToString(dr["SAP_ORDERNO"]);
                    pord.ShipDate = Convert.ToString(dr["SHIPDATE"]) == "" ? "" : Convert.ToString(dr["SHIPDATE"]);
                    pord.SHIPID = Convert.ToInt32(dr["SHIPID"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPID"]);
                    pord.ShipMethodid = Convert.ToInt32(dr["SHIPMETHODID"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPMETHODID"]);
                    pord.ShipNo = Convert.ToString(dr["SHIPNO"]) == "" ? "" : Convert.ToString(dr["SHIPNO"]);
                    pord.ShipQTY = Convert.ToInt32(dr["SHIPQTY"]) == 0 ? 0 : Convert.ToInt32(dr["SHIPQTY"]);
                    pord.BALQTY = pord.QTY - pord.ShipQTY;

                    objpart.Add(pord);
                }
                return objpart.ToList();

            }
            catch { throw; }
        }

        /// <summary>
        ///for  testing
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public string test(string OrderNo)
        {

            try
            {
                OrderListData objListDetail = new OrderListData();
                DataTable dtdetails = objListDetail.GetShipInfoDetails(OrderNo);

                var part = (from p in dtdetails.AsEnumerable()
                            select new
                            {
                                //QTY = p.Field<decimal>("TOTALORDERQUANTITY"),
                                SHIPNO = p.Field<String>("SHIPNO"),
                                METHODNAME = p.Field<String>("METHODNAME"),
                                SHIPDATE = p.Field<DateTime>("SHIPDATE"),
                                SHIPID = p.Field<decimal>("SHIPID"),
                                INVOICENO = p.Field<string>("INVOICENO"),
                                INVOICEDATE = p.Field<DateTime>("INVOICEDATE"),
                                ATTID = p.Field<decimal>("ATTID"),
                                SHAPORDERNO = p.Field<string>("SAP_OrderNo") == null ? "False" : p.Field<string>("SAP_OrderNo")
                            }).Distinct();

                var q = from p in part
                        select new
                        {
                            //p.PARTNO,
                            //p.DESCRIPTION,
                            p.SHIPNO,
                            p.SHIPID,
                            p.METHODNAME,
                            p.SHIPDATE,
                            p.INVOICENO,
                            p.INVOICEDATE,
                            p.ATTID,
                            p.SHAPORDERNO,
                            Shipment = from s in dtdetails.AsEnumerable()
                                       where s.Field<decimal>("SHIPID").Equals(p.SHIPID)
                                       select new { PARTNO = s.Field<String>("PARTNO"), DESCRIPTION = s.Field<String>("DESCRIPTION"), SHIPQTY = s.Field<decimal>("SHIPQTY") },
                            QTY = from s in dtdetails.AsEnumerable()
                                  where s.Field<decimal>("SHIPID").Equals(p.SHIPID)
                                  group s by s.Field<decimal>("PARTID") into g
                                  select new { BALQTY = g.Sum(x => x.Field<decimal>("SHIPQTY")) }.BALQTY
                        };
                return q.ToJson();

            }
            catch { throw; }
        }

        #endregion

        # region Inventory View


        public List<ComboData> GetPartDesInvPool(string partno)
        {
            string partno_en = DecryptAes(partno);

            InventoryPoolingData invPool = new InventoryPoolingData();
            List<ComboData> lst = (from s in invPool.GetSerchPartNo(partno_en).Rows.Cast<DataRow>() select (new ComboData { ID = s.Field<decimal>("ID"), Text = s.Field<string>("PARTNO") })).OrderBy(p => p.Text).ToList();
            if (lst.Count == 0)
                lst = (from s in invPool.GetSerchddesc(partno_en).Rows.Cast<DataRow>() select (new ComboData { ID = s.Field<decimal>("ID"), Text = s.Field<string>("DESCRIPTION") })).OrderBy(p => p.Text).ToList();
            return lst.ToList();
        }

        public List<InventoryListInfo> GetInventoryList(string Id)
        {
            var id_en = DecryptAes(Id);
            Int32 ids = Convert.ToInt32(id_en);
            List<InventoryListInfo> objInvInfo = new List<InventoryListInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@Id", ids, DbType.Int32));
            // DataTable dtDcp = maincls.DbCon.GetProcTable("Proc_GetInventoryView", cps, "tbl_InvDtl");
            DataTable dtDcp = maincls.DbCon.GetProcTable("Proc_GetInventoryViewMob", cps, "tbl_InvDtl");
            foreach (DataRow dr in dtDcp.Rows)
            {
                InventoryListInfo Invinfo = new InventoryListInfo();
                Invinfo.Id = Convert.ToInt32(dr["id"]);
                Invinfo.Partno = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                Invinfo.Description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                Invinfo.Uploaddate = Convert.ToString(dr["UPLOADDATE"]) == "" ? "" : Convert.ToString(dr["UPLOADDATE"]);
                Invinfo.UPLOADDays = Convert.ToInt32(dr["UploadedDays"]) == 0 ? 0 : Convert.ToInt32(dr["UploadedDays"]);
                Invinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                Invinfo.DealerName = Convert.ToString(dr["DEALERNAME"]) == "" ? "" : Convert.ToString(dr["DEALERNAME"]);
                Invinfo.ContactPerson = Convert.ToString(dr["CONTACTPERSION"]) == "" ? "" : Convert.ToString(dr["CONTACTPERSION"]);
                Invinfo.EMAIL = Convert.ToString(dr["Email"]) == "" ? "" : Convert.ToString(dr["Email"]);
                Invinfo.ContactNo = Convert.ToString(dr["CONTACTNO"]) == "" ? "" : Convert.ToString(dr["CONTACTNO"]);
                Invinfo.Latitude = Convert.ToString(dr["LATITUDE"]) == "" ? "" : Convert.ToString(dr["LATITUDE"]);
                Invinfo.Longitude = Convert.ToString(dr["LONGITUDE"]) == "" ? "" : Convert.ToString(dr["LONGITUDE"]);
                Invinfo.CountryName = Convert.ToString(dr["COUNTRYNAME"]) == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]);
                Invinfo.StateName = Convert.ToString(dr["STATENAME"]) == "" ? "" : Convert.ToString(dr["STATENAME"]);
                Invinfo.CityName = Convert.ToString(dr["CITYNAME"]) == "" ? "" : Convert.ToString(dr["CITYNAME"]);
                Invinfo.Distance = "0" + " miles";
                objInvInfo.Add(Invinfo);
            }
            return objInvInfo.ToList();

        }

        public List<InventoryListInfo> GetInventory(string Id, double lat, double lon)
        {
            var id_en = DecryptAes(Id);
            Int32 ids = Convert.ToInt32(id_en);

            List<InventoryListInfo> objInvInfo = new List<InventoryListInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@Id", ids, DbType.Int32));
            // DataTable dtDcp = maincls.DbCon.GetProcTable("Proc_GetInventoryView", cps, "tbl_InvDtl");
            DataTable dtDcp = maincls.DbCon.GetProcTable("Proc_GetInventoryViewMob", cps, "tbl_InvDtl");
            foreach (DataRow dr in dtDcp.Rows)
            {
                InventoryListInfo Invinfo = new InventoryListInfo();
                Invinfo.Id = Convert.ToInt32(dr["id"]);
                Invinfo.Partno = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                Invinfo.Description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                Invinfo.Uploaddate = Convert.ToString(dr["UPLOADDATE"]) == "" ? "" : Convert.ToString(dr["UPLOADDATE"]);
                Invinfo.UPLOADDays = Convert.ToInt32(dr["UploadedDays"]) == 0 ? 0 : Convert.ToInt32(dr["UploadedDays"]);
                Invinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                Invinfo.DealerName = Convert.ToString(dr["DEALERNAME"]) == "" ? "" : Convert.ToString(dr["DEALERNAME"]);
                Invinfo.ContactPerson = Convert.ToString(dr["CONTACTPERSION"]) == "" ? "" : Convert.ToString(dr["CONTACTPERSION"]);
                Invinfo.EMAIL = Convert.ToString(dr["Email"]) == "" ? "" : Convert.ToString(dr["Email"]);
                Invinfo.ContactNo = Convert.ToString(dr["CONTACTNO"]) == "" ? "" : Convert.ToString(dr["CONTACTNO"]);
                Invinfo.Latitude = Convert.ToString(dr["LATITUDE"]) == "" ? "" : Convert.ToString(dr["LATITUDE"]);
                Invinfo.Longitude = Convert.ToString(dr["LONGITUDE"]) == "" ? "" : Convert.ToString(dr["LONGITUDE"]);
                Invinfo.CountryName = Convert.ToString(dr["COUNTRYNAME"]) == "" ? "" : Convert.ToString(dr["COUNTRYNAME"]);
                Invinfo.StateName = Convert.ToString(dr["STATENAME"]) == "" ? "" : Convert.ToString(dr["STATENAME"]);
                Invinfo.CityName = Convert.ToString(dr["CITYNAME"]) == "" ? "" : Convert.ToString(dr["CITYNAME"]);
                if (lat == 0 || lon == 0 || Invinfo.Latitude == "0.0000000" || Invinfo.Longitude == "0.0000000")
                {
                    Invinfo.Distance = "";
                }
                else
                {
                    double dist = Calculate(lat, lon, Convert.ToDouble(Invinfo.Latitude), Convert.ToDouble(Invinfo.Longitude));
                    Invinfo.Distance = Convert.ToString(dist.ToString("F")) + " miles";
                }
                objInvInfo.Add(Invinfo);
            }
            return objInvInfo.ToList();

        }

        //next api for distance

        public static double Calculate(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var sLatitudeRadians = sLatitude * (Math.PI / 180.0);
            var sLongitudeRadians = sLongitude * (Math.PI / 180.0);
            var eLatitudeRadians = eLatitude * (Math.PI / 180.0);
            var eLongitudeRadians = eLongitude * (Math.PI / 180.0);

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Using 3956 as the number of miles around the earth
            var result2 = 3956.0 * 2.0 * Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));
            // var result2 = 3958.76 * 2.0 * Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return result2;
        }

        public List<InventryStatus> InventoryStatistics()
        {
            List<InventryStatus> objinventory = new List<InventryStatus>();
            //parts count
            InventoryPoolingData objUserbus = new InventoryPoolingData();
            DataTable distinctValues = objUserbus.GetPartCount();
            decimal partscnt = Convert.ToDecimal(distinctValues.Rows[0][0].ToString());
            //dealer count
            DataTable dt = maincls.DbCon.GetProcTable("PROC_DISTIPLOADLIST", "Distributor");
            Int32 ss = dt.Rows.Count;
            InventryStatus instatus = new InventryStatus();
            instatus.DealerCount = Convert.ToInt32(ss) == 0 ? 0 : Convert.ToInt32(ss);
            instatus.PartCount = Convert.ToInt32(partscnt) == 0 ? 0 : Convert.ToInt32(partscnt);
            objinventory.Add(instatus);
            return objinventory.ToList();
        }


        # endregion

        # region Order List

        public List<AddPart> Addpart(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0)
        {
            var Qty_en = DecryptAes(Qty);
            Int32 qtys = Convert.ToInt32(Qty_en);

            var cid_en = DecryptAes(CountryID);
            Int32 cids = Convert.ToInt32(cid_en);

            string partno_en = DecryptAes(PartNo);


            string agroupid = string.Empty;
            string PartCategoryID = string.Empty;
            string pno = partno_en.ToUpper();
            List<AddPart> objAddPart = new List<AddPart>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@PartNo", pno, DbType.String));
            cps.Add(new CommanParameter("@Qty", qtys, DbType.Int32));
            cps.Add(new CommanParameter("@CountryID", cids, DbType.Int32));
            DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ORDERLISTADDPART", cps, "AddPart");
            if (dtPart.Rows.Count > 0)
            {
                string slq = @"select agroupid from tbl_part where partno=" + "'" + partno_en + "'";
                DataTable dt = maincls.DbCon.GetTable(slq, "tbl_part");
                if (dt.Rows.Count > 0)
                {
                    agroupid = dt.Rows[0]["AGROUPID"].ToString();
                }
                else { agroupid = "0"; }
            }
            AddPart addpart = new AddPart();
            try
            {
                bool blStatus = false;
                if (dtPart != null)
                {
                    //If Part Is Not in Database then Pop Up Message
                    if (dtPart.Rows.Count <= 0)
                    {
                        //Show the message.

                        //return Resources.Resource.ThePartNumberYouHaveSpecifiedIsNotAValidPartNoAlertMessage;

                        addpart.Message = "Not Vaild";
                        addpart.ID = 0;
                        addpart.Description = "";
                        addpart.Partno = "";
                        addpart.Available = "";
                        addpart.NS = 0;
                        addpart.Qty = 0;
                        addpart.PartPrice = 0;
                        addpart.AGroupId = "AGroupId";
                        objAddPart.Add(addpart);
                        return objAddPart.ToList();
                    }
                    else
                    {
                        try
                        {
                            if (otype == 0)
                            {
                                PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                if (PartCategoryID == "1")
                                {
                                    addpart.Message = "accessory parts";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;
                                    addpart.PartPrice = 0;
                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                                //  return Resources.Resource.PleaseOrderTheAccessoryPartsFromtheAccessoryPartSection;
                            }
                            else
                            {
                                PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                if (PartCategoryID == "2")
                                {
                                    addpart.Message = "spare parts";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;
                                    addpart.PartPrice = 0;
                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                            }

                            //Checking that Part is Serviceable or Non Serviceable
                            if (Convert.ToInt32(dtPart.Rows[0]["NonServiceable"]) == 1 || Convert.ToBoolean(dtPart.Rows[0]["NonServiceable"]) == true)
                            {
                                //Show the message.
                                // return Resources.Resource.ThePartNumberYouHaveSpecifiedIsANonServiceablePartAlertMessage;
                                addpart.Message = "NonServiceable";
                                addpart.ID = 0;
                                addpart.Description = "";
                                addpart.Partno = "";
                                addpart.Available = "";
                                addpart.NS = 0;
                                addpart.Qty = 0;
                                addpart.PartPrice = 0;
                                addpart.AGroupId = "AGroupId";
                                objAddPart.Add(addpart);
                                return objAddPart.ToList();
                            }
                        }
                        catch { }

                    }


                    if (blStatus == true)
                    {
                        //Show the message
                        //return Resources.Resource.PartQuantitySuccessfullyUpdatedInOrderListAlertMessage;
                        addpart.Message = "Update";
                        addpart.ID = 0;
                        addpart.Description = "";
                        addpart.Partno = "";
                        addpart.Available = "";
                        addpart.NS = 0;
                        addpart.Qty = 0;
                        addpart.PartPrice = 0;
                        addpart.AGroupId = "AGroupId";
                        objAddPart.Add(addpart);
                        return objAddPart.ToList();
                    }
                    else
                    {
                        //Show the message
                        //return Resources.Resource.PartSuccessfullyAddedToOrderListAlertMessage;
                        foreach (DataRow dr in dtPart.Rows)
                        {
                            addpart.Message = "Success";
                            addpart.ID = Convert.ToInt32(dr["ID"]);
                            addpart.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                            addpart.Partno = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                            addpart.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                            addpart.NS = Convert.ToInt32(dr["NONSERVICEABLE"]) == 0 ? 0 : Convert.ToInt32(dr["NONSERVICEABLE"]);
                            addpart.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                            addpart.PartPrice = Convert.ToInt32(dr["PARTPRICE"]) == 0 ? 0 : Convert.ToInt32(dr["PARTPRICE"]);
                            addpart.AGroupId = agroupid == "" ? "0" : agroupid;
                            objAddPart.Add(addpart);
                        }
                        return objAddPart.ToList();
                    }
                }
                else
                {
                    //Show the message
                    // return Resources.Resource.PleaseEnterAValidPartNoAlertMessage;
                    addpart.Message = "Valid";
                    addpart.ID = 0;
                    addpart.Description = "";
                    addpart.Partno = "";
                    addpart.Available = "";
                    addpart.NS = 0;
                    addpart.Qty = 0;
                    addpart.PartPrice = 0;
                    addpart.AGroupId = "AGroupId";
                    objAddPart.Add(addpart);
                    return objAddPart.ToList();
                }

            }
            catch
            {
                //return Resources.Resource.ThePartNumberYouHaveSpecifiedIsNotAValidPartNoAlertMessage;
                addpart.Message = "Invalid";
                addpart.ID = 0;
                addpart.Description = "";
                addpart.Partno = "";
                addpart.Available = "";
                addpart.NS = 0;
                addpart.Qty = 0;
                addpart.PartPrice = 0;
                addpart.AGroupId = "AGroupId";
                objAddPart.Add(addpart);
                return objAddPart.ToList();
            }


        }


        public List<AddPart> UpdatePart(string Qty, List<Partinfo> Plist, string CountryID, string hfOrderType, int otype = 0, string usertype = "0")
        {
            var Qty_en = DecryptAes(Qty);
            Int32 qtys = Convert.ToInt32(Qty_en);

            var cid_en = DecryptAes(CountryID);
            Int32 cid = Convert.ToInt32(cid_en);

            var utpe_en = DecryptAes(usertype);
            Int32 utype = Convert.ToInt32(utpe_en);

            string agroupid = string.Empty;

            List<AddPart> objAddPart = new List<AddPart>();

            foreach (Partinfo makes in Plist)
            {
                string pno = makes.PartNo;
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@PartNo", pno, DbType.String));
                cps.Add(new CommanParameter("@Qty", qtys, DbType.Int32));
                cps.Add(new CommanParameter("@CountryID", cid, DbType.Int32));
                DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ORDERLISTADDPART", cps, "AddPart");
                if (dtPart.Rows.Count > 0)
                {
                    string slq = @"select agroupid from tbl_part where partno=" + "'" + pno + "'";
                    DataTable dt = maincls.DbCon.GetTable(slq, "tbl_part");
                    if (dt.Rows.Count > 0)
                    {
                        agroupid = dt.Rows[0]["AGROUPID"].ToString();
                    }
                    else { agroupid = "0"; }
                }

                if (dtPart == null || dtPart.Rows.Count == 0) continue;
                AddPart addpart = new AddPart();
                DataRow dr = dtPart.Rows[0];
                //if (dtPart.Rows[0]["AVAILABLE"] == "In Stock" || dtPart.Rows[0]["AVAILABLE"] == "Limited Availability" || dtPart.Rows[0]["AVAILABLE"] == "")
                // {

                //addpart.Message = "";
                //addpart.ID = Convert.ToInt32(dr["ID"]);
                //addpart.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                //addpart.Partno = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                //addpart.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                // addpart.NS = Convert.ToInt32(dr["NONSERVICEABLE"]) == 0 ? 0 : Convert.ToInt32(dr["NONSERVICEABLE"]);
                //addpart.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                //addpart.PartPrice = Convert.ToDecimal(dr["PARTPRICE"]) == 0 ? 0 : Convert.ToDecimal(dr["PARTPRICE"]);
                //addpart.AGroupId = agroupid == "" ? "0" : agroupid;
                //objAddPart.Add(addpart);
                // }
                // else
                // {

                string part = dtPart.Rows[0]["ID"].ToString();
                DataTable dtt = addreview("Part", part, "", "1", cid, 1, utype, "", "");
                if (dtt.Rows.Count == 0)
                {
                    addpart.Message = part;
                    addpart.NS = 1;
                    addpart.Partno = pno;
                    objAddPart.Add(addpart);
                }
                else
                {
                    addpart.Message = part;
                    addpart.ID = Convert.ToInt32(dtt.Rows[0]["ID"]);
                    addpart.Partno = Convert.ToString(dtt.Rows[0]["PARTNO"]) == "" ? "" : Convert.ToString(dtt.Rows[0]["PARTNO"]);
                    addpart.Description = Convert.ToString(dtt.Rows[0]["DESCRIPTION"]) == "" ? "" : Convert.ToString(dtt.Rows[0]["DESCRIPTION"]);
                    addpart.Available = Convert.ToString(dtt.Rows[0]["AVAILABLE"]) == "" ? "" : Convert.ToString(dtt.Rows[0]["AVAILABLE"]);
                    addpart.PartPrice = Convert.ToDecimal(dtt.Rows[0]["PRICE1"]) == 0 ? 0 : Convert.ToDecimal(dtt.Rows[0]["PRICE1"]);
                    addpart.Qty = Convert.ToInt32(dtt.Rows[0]["QTY"]) == 0 ? 0 : Convert.ToInt32(dtt.Rows[0]["QTY"]);
                    addpart.AGroupId = Convert.ToString(dtt.Rows[0]["AGROUPID"]) == "" ? "0" : Convert.ToString(dtt.Rows[0]["AGROUPID"]);//MUSA
                    //addpart.AGroupId = ""; //MFS
                    addpart.NS = Convert.ToInt32(dr["NONSERVICEABLE"]) == 0 ? 0 : Convert.ToInt32(dr["NONSERVICEABLE"]);
                    objAddPart.Add(addpart);
                }

            }
            return objAddPart.ToList();
        }

        public DataTable addreview(string rType, string PID, string ID, string Qty, Int32 cid, Int32 lngid, Int32 usertype, string MId, string decAId)
        {
            Catalog.MainCommon MainClass = Catalog.MainCommon.Instance();
            List<AlternatePartInfo> objAlternate = new List<AlternatePartInfo>();
            //Set the string variable as empty.
            string strSql = string.Empty;
            //Get the datetime into datetime object.
            DateTime dteSeldate = DateTime.Now;

            DataTable dtOrderList = null;

            object strSelDate = dteSeldate;
            if (rType == "Part")
            {
                //Get the partID into string type variable 'arrPartID'.
                string arrPartID = Convert.ToString(PID);
                if (arrPartID.IndexOf(',') == 0)
                    arrPartID = arrPartID.Remove(0, 1);
                //Split the part ID.
                string[] strarrPID = arrPartID.Split(',');
                //Get the quantity into string type variable 'arrPartQty'.
                string arrPartQty = Convert.ToString(Qty);
                //Split the part quantity.
                string[] strarrPQty = arrPartQty.Split(',');
                string strpartid = string.Empty;
                for (int i = 0; i < strarrPID.Length; i++)
                {
                    if (dtOrderList != null)
                    {
                        //Get the ID into datarow.
                        DataRow[] arrdr = dtOrderList.Select("ID=" + strarrPID[i]);
                        //Check the datarow length.
                        if (arrdr.Length > 0)
                        {
                            //Get the datarow value into another datarow.
                            arrdr[0]["NQTY"] = Convert.ToInt32(arrdr[0]["NQTY"]) + Convert.ToInt32(strarrPQty[i]);
                            continue;
                        }
                    }
                    //creating an instance of AddToOrderListData
                    AddToOrderListData ObjAddToLData = new AddToOrderListData();
                    // Updating part Qty. if null then 1
                    if (strarrPQty[i] == "null")
                    {
                        strarrPQty[i] = "1";
                    }
                    // Calling Procedure to fill datatable.
                    DataTable dtPD = FillGrdPart(Convert.ToInt32(strarrPQty[i]), Convert.ToString(strarrPID[i]), cid, lngid, usertype);
                    //DataTable dtPD = MainClass.DbCon.GetTable(strSql, "tbl_Part");
                    if (dtOrderList == null) dtOrderList = dtPD.Clone();
                    //Merge the datatable into another table.
                    dtOrderList.Merge(dtPD);
                }
            }
            else
            {
                //Creating an instence of AddToOrderListData class.
                AddToOrderListData ObjAddToLData = new AddToOrderListData();
                // Calling Procedure to fill datatable.
                dtOrderList = ObjAddToLData.FillGrd(Convert.ToInt32(ID), Convert.ToInt32(MId), cid, lngid, usertype);
            }
            //Add some addtional columns in order list
            DataTable dtpart = dtOrderList;
            //Set the string variable as empty.
            string strModelid = string.Empty;
            string strAssID = string.Empty;
            decimal decMId;
            //Getting ModelID and Assembly IDs in different cases.
            if (Convert.ToString(rType) != "Part")
            {
                //Get the modelID into another variable.
                strModelid = "," + MId;
                //Get the ID into another variable.
                strAssID = "," + ID;
                //Get the value into object.
                object count = MainClass.DbCon.ExecuteScaler(@"select count(*) from tbl_parttree where VariantID in (0" + strModelid + ") And  AssemblyID in (0" + strAssID + ")");
                //Check the count for greater than zero.
                if (Convert.ToInt32(count) > 0)
                {
                    //Checking part history for each row of order list data table
                    foreach (DataRow dr in dtpart.Rows)
                    {
                        //Get the modelID or decMID.
                        decMId = Convert.ToDecimal(rType == "Part" ? Convert.ToString(dr["MODELID"]) : Convert.ToString(MId));

                        string sSql = @"select count(*) from tbl_parttree pt
                        Where partid=" + dr["ID"].ToString() + @"
                        And VariantID=" + decMId.ToString() + " And  AssemblyID= " + dr["AssemblyId"].ToString() + @"
                        And AssemblypartId=" + Convert.ToString(dr["AssemblypartId"]);
                        //Return the integer value and assign to another interger.
                        int icnt = Convert.ToInt32(MainClass.DbCon.ExecuteScaler(sSql));
                        //Check the icnt value for greater than zero.
                        if (icnt > 0)
                        {
                            //Create the new object for clsHistory.
                            clsHistory objHistory = new clsHistory();
                            //Get the datarow value for updation on the basis of parameters.
                            DataRow drUpdate = GetUpdatedPart(decMId, Convert.ToDecimal(dr["ASSEMBLYPARTID"]), dteSeldate, Convert.ToInt32(dr["NQTY"]), usertype);
                            //Check the drUpdate for not null.
                            if (drUpdate != null)
                            {
                                //Updating rows
                                dr["ID"] = drUpdate["PartID"];
                                dr["PartNo"] = drUpdate["PartNo"];
                                dr["Description"] = drUpdate["Description"];
                                dr["startdate"] = drUpdate["startdate"];
                                dr["enddate"] = drUpdate["enddate"];
                                dr["MOQTY"] = Convert.ToInt32(drUpdate["MOQTY"]) == 0 ? 0 : Convert.ToInt32(drUpdate["MOQTY"]);
                                dr["MPQTY"] = Convert.ToInt32(drUpdate["MPQTY"]) == 0 ? 0 : Convert.ToInt32(drUpdate["MPQTY"]);
                                dr["price1"] = drUpdate["price1"];
                                dr["Status"] = drUpdate["Status"];
                                //Save the changes.
                                dr.AcceptChanges();
                            }
                            else
                            {
                                if (HttpContext.Current.Session["SrVINNO"] != null)
                                    //Delete the datarow.
                                    dr.Delete();
                            }
                        }
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            //Check for type value.
                            if (rType == "Basket" || rType == "PartSearch" || rType == "Assembly")
                            {
                                if (Convert.ToDateTime(dr["StartDate"]) > Convert.ToDateTime(strSelDate))
                                {
                                    //Delete the datarow.
                                    dr.Delete();
                                }
                                //Check for end date
                                else if (dr["enddate"] != DBNull.Value)
                                    //Check if end date is less than selecetd date with session date
                                    if (Convert.ToDateTime(dr["enddate"]) < Convert.ToDateTime(strSelDate))
                                    {
                                        //Delete the datarow.
                                        dr.Delete();
                                    }
                            }
                        }
                    }
                    //Save the Changes.
                    dtpart.AcceptChanges();
                }
                else
                {
                    if (rType != "Part")
                    {
                        foreach (DataRow dr in dtpart.Select("StartDate>" + MainClass.GetDateString(Convert.ToDateTime(strSelDate)) + " or Enddate <" + MainClass.GetDateString(Convert.ToDateTime(strSelDate))))
                        {
                            //Delete the datarow.
                            dr.Delete();
                        }
                        //Save the changes.
                        dtpart.AcceptChanges();
                    }
                }
            }
            //For Mahindra Logan-Verito
            //-----------------------------------------------------------------------
            //if (Convert.ToString(HttpContext.Current.Session["SrVINNO"]) != "" && Convert.ToString(HttpContext.Current.Session["ISBOMREQUIRED"]) == "1")
            //{
            //    //Find the Part mapped with this BOM ID // decAId insted of HttpContext.Current.Session["Id"].ToString()
            //    DataTable dtRpoVin = MainClass.DbCon.GetTable(@"select PARTID from TBL_SBOMPARTDETAILS where Assemblyid='" + decAId + "' and SBOMID='" + HttpContext.Current.Session["SBOMID"].ToString() + "'", "ABC");

            //    //Add the Primary Key attribute on this table
            //    DataColumn[] keyColumns = new DataColumn[1];
            //    keyColumns[0] = dtRpoVin.Columns["PARTID"];
            //    dtRpoVin.PrimaryKey = keyColumns;
            //    dtRpoVin.AcceptChanges();
            //    foreach (DataRow dr in dtpart.Rows)
            //    {
            //        //find whether the part exist or not
            //        DataRow drc = dtRpoVin.Rows.Find(dr["ID"]);
            //        //if does not exist delete
            //        if (drc == null)
            //        {
            //            dr.Delete();
            //        }
            //    }
            //    //accepting all changes
            //    dtpart.AcceptChanges();
            //}
            //-----------------------------------------------------------------------

            // code for merging the rows for similar AssemblyPartID.
            DataTable dtTmp = dtpart.Clone(); // creating temporary table.
            //Geting order list from session 
            DataTable dtOrder = new DataTable();// (DataSetCommon.OrderListDataTable)HttpContext.Current.Session["OrderList"];
            //Updating qty for each record
            foreach (DataRow drt in dtpart.Rows)
            {
                DataRow[] arrDrPart = null;
                DataRow[] arrDrTmp = dtTmp.Select("ID=" + drt["ID"].ToString());
                // check for assembly part already exists or not.
                if (dtTmp.Rows.Count <= 0 || arrDrTmp.Length <= 0)
                {
                    //Add the new row into datatable.
                    DataRow drNew = dtTmp.NewRow();
                    // checking how many rows exist in the main table for the current assemblypart.
                    arrDrPart = dtpart.Select("ID=" + drt["ID"].ToString());
                    // copying values of the current row in the new row
                    for (int iCount = 0; iCount < dtpart.Columns.Count; iCount++)
                    {
                        // Removing ModelID from the New Row.
                        if (dtpart.Columns[iCount].ColumnName.ToUpper() != "MODELID")
                            drNew[iCount] = drt[iCount];
                    }
                    // if more than one rows exist in the main table then
                    //setting Existing,New and Total Qty
                    if (arrDrPart.Length > 1)
                    {
                        //Initialize the variable as zero.
                        int iNQty = 0;
                        //Updating qty.
                        foreach (DataRow drs in arrDrPart)
                        {
                            iNQty += (drs["NQty"] == DBNull.Value) ? 0 : Convert.ToInt32(drs["NQty"]);
                        }
                        //Assign the variable into another datarow.
                        drNew["NQty"] = iNQty;
                    }
                    //if (dtOrder != null)
                    //{

                    //    DataRow[] arrdrselect;
                    //    arrdrselect = dtOrder.Select("Id=" + drt["ID"]);
                    //    //filling the quantity.
                    //    if (arrdrselect.Length > 0)
                    //    {
                    //        //Loop for Get the Qty of Part
                    //        foreach (DataRow drPart in arrdrselect)
                    //        {
                    //            //Checking that Qty is not null
                    //            if (drNew["Qty"] != null && drPart["Qty"] != null)
                    //                drNew["Qty"] = Convert.ToInt32(drNew["Qty"]) + Convert.ToInt32(drPart["Qty"]);
                    //        }
                    //    }
                    //}
                    //// Adding this row in the new table.
                    //dtTmp.Rows.Add(drNew);
                }
            }
            //dtpart.Clear(); // clearing the part table.
            // dtpart = dtTmp; // 
            // HttpContext.Current.Session["OrdPartViewState"] = dtpart;
            DataTable dtCloned = dtpart.Clone();
            dtCloned.Columns["NQTY"].DataType = typeof(string);

            foreach (DataRow row in dtpart.Rows)
            {
                dtCloned.ImportRow(row);
            }
            foreach (DataRow dr in dtCloned.Rows)
            {
                AlternatePartInfo apinfo = new AlternatePartInfo();
                apinfo.ID = Convert.ToInt32(dr["ID"]);
                apinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                apinfo.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                apinfo.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                apinfo.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                apinfo.Price2 = Convert.ToString(dr["PRICE2"]) == "" ? "" : Convert.ToString(dr["PRICE2"]);
                apinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                 apinfo.aGroupID = Convert.ToString(dr["AGROUPID"]) == "" ? "" : Convert.ToString(dr["AGROUPID"]);//MUSA
                //apinfo.aGroupID = "";// MFS
                objAlternate.Add(apinfo);
            }

            return dtCloned;

        }


        public List<AddPart> CheckAlternateParts(string PartNo, string ptype, string utype, string countryid)
        {
            try
            {
                string partno_en = DecryptAes(PartNo);

                var utupe_en = DecryptAes(utype);
                Int32 utypes = Convert.ToInt32(utupe_en);

                var cid_en = DecryptAes(countryid);
                Int32 cid = Convert.ToInt32(cid_en);


                List<AddPart> objAddPart = new List<AddPart>();
                OrderListData objListDetail = new OrderListData();
                //Getting DataTable of Alternate Parts
                DataTable dtAlternatePartsList = objListDetail.GetAlternateParts(partno_en, ptype, utypes, cid);
                if (dtAlternatePartsList.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtAlternatePartsList.Rows)
                    {
                        AddPart addpart = new AddPart();
                        addpart.ID = Convert.ToInt32(dr["ID"]);
                        addpart.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                        addpart.Partno = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                        addpart.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                        addpart.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                        addpart.PartPrice = Convert.ToInt32(dr["PRICE1"]) == 0 ? 0 : Convert.ToInt32(dr["PRICE1"]);
                        addpart.Price2 = Convert.ToInt32(dr["PRICE2"]) == 0 ? 0 : Convert.ToInt32(dr["PRICE2"]);
                        objAddPart.Add(addpart);
                    }
                }
                else
                {
                    AddPart addpart = new AddPart();
                    addpart.ID = 0;
                    addpart.Description = "";
                    addpart.Partno = "";
                    addpart.Available = "";
                    addpart.Qty = 0;
                    addpart.PartPrice = 0;
                    addpart.Price2 = 0;
                    objAddPart.Add(addpart);
                }
                return objAddPart.ToList();
            }
            catch { throw; }
        }

        # endregion

        # region Literature List

        public List<Literature> LiteratureList(int ID, int assid, string description, int parentnode)
        {
            List<Literature> objLiterature = new List<Literature>();
            AttachmentData objattachment = new AttachmentData();
            CommanParametrs Cps = new CommanParametrs();
            CommanParameter cp;
            cp = new CommanParameter("@ID", ID, DbType.Int32);
            Cps.Add(cp);
            cp = new CommanParameter("@pnode", parentnode, DbType.Int32);
            Cps.Add(cp);
            cp = new CommanParameter("@AssemblyID", assid, DbType.Int32);
            Cps.Add(cp);
            cp = new CommanParameter("@Description", description, DbType.String);
            Cps.Add(cp);
            DataTable dt = maincls.DbCon.GetProcTable("PROC_LITERATURESEARCHLIST", Cps, "tbl_AssemblyAttachment");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Literature literature = new Literature();
                    literature.ID = Convert.ToInt32(dr["ID"]) == 0 ? 0 : Convert.ToInt32(dr["ID"]);
                    literature.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                    literature.ActSize = Convert.ToInt64(dr["FSIZE"]) == 0 ? 0 : Convert.ToInt64(dr["FSIZE"]);
                    literature.Size = SizeSuffix(Convert.ToInt64(dr["FSIZE"])) == "" ? "" : SizeSuffix(Convert.ToInt64(dr["FSIZE"]));
                    literature.Type = Convert.ToInt32(dr["ATTACHMENTTYPE"]) == 0 ? 0 : Convert.ToInt32(dr["ATTACHMENTTYPE"]);
                    literature.Date = Convert.ToString(dr["CREATEDDATE"]) == "" ? "" : Convert.ToString(dr["CREATEDDATE"]);
                    objLiterature.Add(literature);
                }
            }
            //else
            // {
            //Literature literature = new Literature();
            //literature.ID = 0;
            //literature.Description = "";
            //literature.ActSize = 0;
            //literature.Size = "";
            //literature.Type = 0;
            //literature.Date = "";
            //objLiterature.Add(literature);
            // }
            return objLiterature.ToList();
        }

        public List<LiteratureMap> GetMapModelsDetails(Int32 ID)
        {
            try
            {
                List<LiteratureMap> objLiterature = new List<LiteratureMap>();
                DataTable dt;
                CommanParametrs objcps = new CommanParametrs();
                //Initialization of Object of CommanParameter class
                CommanParameter objcp;
                //creating Object of CommanParameter class and asining value of Id
                objcp = new CommanParameter("@ID", ID, DbType.Int32);
                objcps.Add(objcp);

                dt = maincls.DbCon.GetProcTable("PROC_GETATTACHMENTMAPPING", objcps, "tbl_attachments");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        LiteratureMap lmap = new LiteratureMap();
                        lmap.TypeName = Convert.ToString(dr["LEVELNAME"]) == "" ? "" : Convert.ToString(dr["LEVELNAME"]);
                        lmap.Name = Convert.ToString(dr["ASSEMBLYNAME"]) == "" ? "" : Convert.ToString(dr["ASSEMBLYNAME"]);
                        if (lmap.TypeName == "VARIANT")
                        { lmap.TypeName = "MODEL"; }
                        else if (lmap.TypeName == "ASSEMBLY")
                        { lmap.TypeName = "GROUP"; }
                        else if (lmap.TypeName == "MODEL")
                        { lmap.TypeName = "SERIES"; }
                        else if (lmap.TypeName == "AGGREGATE")
                        { lmap.TypeName = "AGGREGATE"; }

                        objLiterature.Add(lmap);
                    }
                }
                return objLiterature.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        # endregion

        # region Part Return

        public List<InventryPartReturnInfo> GetPartReturnListNew(string distId, Int32 page)
        {
            List<InventryPartReturnInfo> objIPRinfo = new List<InventryPartReturnInfo>();
            try
            {
                var distid_en = DecryptAes(distId);
                Int32 distids = Convert.ToInt32(distid_en);


                int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);
                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@UserId", distids, DbType.Decimal));
                cps.Add(new CommanParameter("@Page", page, DbType.Decimal));
                cps.Add(new CommanParameter("@RecsPerPage", scrollsize, DbType.Decimal));
                DataTable dt = maincls.DbCon.GetProcTable("PROC_VIEWPARTDETAILLISTNEW", cps, "tbl_ReturnMst");
                DataView dv = dt.DefaultView;
                dv.Sort = "ID DESC";
                dt = dv.ToTable();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        InventryPartReturnInfo iprinfo = new InventryPartReturnInfo();
                        iprinfo.TotalRecords = Convert.ToInt32(dr["TotalRows"]);
                        iprinfo.id = Convert.ToInt32(dr["ID"]);
                        iprinfo.returnno = Convert.ToString(dr["ReturnNo"]) == "" ? "" : Convert.ToString(dr["ReturnNo"]);
                        iprinfo.SNo = Convert.ToInt32("1");
                        iprinfo.returndate = Convert.ToString(dr["RDate"]) == "" ? "" : Convert.ToString(dr["RDate"]);
                        iprinfo.invoiceno = Convert.ToString(dr["InvoiceNo"]) == "" ? "" : Convert.ToString(dr["InvoiceNo"]);
                        iprinfo.invoicedate2 = Convert.ToString(dr["IDate"]) == "" ? "" : Convert.ToString(dr["IDate"]);
                        iprinfo.status = Convert.ToString(dr["Status"]) == "" ? "" : Convert.ToString(dr["Status"]);
                        iprinfo.Intstatus = Convert.ToInt32(dr["intstatus"]);
                        iprinfo.returntype = Convert.ToString(dr["ReturnType"]) == "" ? "" : Convert.ToString(dr["ReturnType"]);
                        iprinfo.Calltag = Convert.ToBoolean(dr["callTag"]);
                        iprinfo.calltagno = Convert.ToString(dr["CALLTAGNO"]) == "" ? "0" : Convert.ToString(dr["CALLTAGNO"]);
                        iprinfo.Diffdays = Convert.ToString(dr["diffdays"]) == "" ? "0" : Convert.ToString(dr["diffdays"]);
                        Int32 fstatus = Convert.ToInt32(dr["FinalStatus"]);
                        if (fstatus == 0)
                        {
                            iprinfo.finalstatus = "RO SUBMITTED";
                        }
                        else if (fstatus == 1)
                        {
                            iprinfo.finalstatus = "RO PROCESSED";
                        }
                        else if (fstatus == 2)
                        {
                            iprinfo.finalstatus = "MATERIAL RECEIPT";
                        }
                        else
                        {
                            iprinfo.finalstatus = "NA";
                        }
                        iprinfo.closedcount = Convert.ToDecimal(dr["CLOSEDCOUNT"]);
                        objIPRinfo.Add(iprinfo);
                    }
                }

                return objIPRinfo.ToList();
            }
            catch (Exception ex)
            {
                return objIPRinfo.ToList();
            }

        }

        public List<InventryPartReturnInfo> GetPartReturnList(string distId)
        {
            var distid_en = DecryptAes(distId);
            Int32 distids = Convert.ToInt32(distid_en);
            List<InventryPartReturnInfo> objIPRinfo = new List<InventryPartReturnInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@UserId", distids, DbType.Decimal));
            DataTable dt = maincls.DbCon.GetProcTable("PROC_VIEWPARTDETAILLIST", cps, "tbl_ReturnMst");
            DataView dv = dt.DefaultView;
            dv.Sort = "ID DESC";
            dt = dv.ToTable();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    InventryPartReturnInfo iprinfo = new InventryPartReturnInfo();
                    iprinfo.id = Convert.ToInt32(dr["ID"]);
                    iprinfo.returnno = Convert.ToString(dr["ReturnNo"]) == "" ? "" : Convert.ToString(dr["ReturnNo"]);
                    iprinfo.SNo = Convert.ToInt32("1");
                    iprinfo.returndate = Convert.ToString(dr["RDate"]) == "" ? "" : Convert.ToString(dr["RDate"]);
                    iprinfo.invoiceno = Convert.ToString(dr["InvoiceNo"]) == "" ? "" : Convert.ToString(dr["InvoiceNo"]);
                    iprinfo.invoicedate2 = Convert.ToString(dr["IDate"]) == "" ? "" : Convert.ToString(dr["IDate"]);
                    iprinfo.status = Convert.ToString(dr["Status"]) == "" ? "" : Convert.ToString(dr["Status"]);
                    iprinfo.Intstatus = Convert.ToInt32(dr["intstatus"]);
                    iprinfo.returntype = Convert.ToString(dr["ReturnType"]) == "" ? "" : Convert.ToString(dr["ReturnType"]);
                    iprinfo.Calltag = Convert.ToBoolean(dr["callTag"]);
                    iprinfo.calltagno = Convert.ToString(dr["CALLTAGNO"]) == "" ? "0" : Convert.ToString(dr["CALLTAGNO"]);
                    iprinfo.Diffdays = Convert.ToString(dr["diffdays"]) == "" ? "0" : Convert.ToString(dr["diffdays"]);
                    Int32 fstatus = Convert.ToInt32(dr["FinalStatus"]);
                    if (fstatus == 0)
                    {
                        iprinfo.finalstatus = "RO SUBMITTED";
                    }
                    else if (fstatus == 1)
                    {
                        iprinfo.finalstatus = "RO PROCESSED";
                    }
                    else if (fstatus == 2)
                    {
                        iprinfo.finalstatus = "MATERIAL RECEIPT";
                    }
                    else
                    {
                        iprinfo.finalstatus = "NA";
                    }
                    iprinfo.closedcount = Convert.ToDecimal(dr["CLOSEDCOUNT"]);
                    objIPRinfo.Add(iprinfo);
                }
            }
            //else
            //{
            //    InventryPartReturnInfo iprinfo = new InventryPartReturnInfo();
            //    iprinfo.id = 0;
            //    objIPRinfo.Add(iprinfo);

            //}
            return objIPRinfo.ToList();

        }

        public List<inventoryChildPartReturnInfo> GetPartReturnChildDataList(string ID)
        {
            var id_en = DecryptAes(ID);
            Int32 ids = Convert.ToInt32(id_en);

            Int32 totalqty = 0;
            decimal amt = 0;
            List<inventoryChildPartReturnInfo> objPRCDInfo = new List<inventoryChildPartReturnInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@ID", ids, DbType.Decimal));
            DataTable dt = maincls.DbCon.GetProcTable("PROC_RETURNDTLLIST", cps, "TBL_RETURNREASON");
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    inventoryChildPartReturnInfo objICPRInfo = new inventoryChildPartReturnInfo();
                    objICPRInfo.ID = Convert.ToInt32(dr["ID"]);
                    objICPRInfo.ReturnID = Convert.ToInt32(dr["ReturnID"]) == 0 ? 0 : Convert.ToInt32(dr["ReturnID"]);
                    objICPRInfo.ReasonId = Convert.ToInt32(dr["ReasonId"]) == 0 ? 0 : Convert.ToInt32(dr["ReasonId"]);
                    objICPRInfo.Reason = Convert.ToString(dr["Reason"]) == "" ? "" : Convert.ToString(dr["Reason"]);
                    objICPRInfo.PartID = Convert.ToInt32(dr["PartID"]) == 0 ? 0 : Convert.ToInt32(dr["PartID"]);
                    objICPRInfo.PartNo = Convert.ToString(dr["PartNo"]).Trim() == "" ? "" : Convert.ToString(dr["PartNo"]).Trim();
                    objICPRInfo.Remarks = Convert.ToString(dr["Remarks"]).Trim() == "" ? "" : Convert.ToString(dr["Remarks"]).Trim();
                    objICPRInfo.shipDtlid = dr["shipDtlid"] == DBNull.Value ? 0 : Convert.ToInt32(dr["shipDtlid"]);
                    objICPRInfo.RPrice = Convert.ToDecimal(dr["RPrice"]) == 0 ? 0 : Convert.ToDecimal(dr["RPrice"]);
                    objICPRInfo.InvoiceNo = Convert.ToString(dr["InvoiceNo"]) == "" ? "" : Convert.ToString(dr["InvoiceNo"]);
                    objICPRInfo.InvoiceDate = Convert.ToString(dr["InvoiceDate"]) == "" ? "" : Convert.ToString(dr["InvoiceDate"]);
                    objICPRInfo.AppQty = Convert.ToInt32(dr["AppQty"]) == 0 ? 0 : Convert.ToInt32(dr["AppQty"]);
                    objICPRInfo.AppRemarks = Convert.ToString(dr["AppRemarks"]) == "" ? "" : Convert.ToString(dr["AppRemarks"]);
                    objICPRInfo.appchk = Convert.ToInt32(dr["appchk"]) == 0 ? 0 : Convert.ToInt32(dr["appchk"]);
                    objICPRInfo.POSNo = Convert.ToString(dr["Line No"]) == "" ? "" : Convert.ToString(dr["Line No"]);
                    objICPRInfo.AcceptQty = Convert.ToInt32(dr["AcceptQty"]) == 0 ? 0 : Convert.ToInt32(dr["AcceptQty"]);
                    objICPRInfo.Description = Convert.ToString(dr["DESCRIPTION"]).Trim() == "" ? "" : Convert.ToString(dr["DESCRIPTION"]).Trim();
                    objICPRInfo.AppRemarks = Convert.ToString(dr["AppRemarks"]) == "" ? "" : Convert.ToString(dr["AppRemarks"]);
                    objICPRInfo.Quantity = Convert.ToInt32(dr["Quantity"]) == 0 ? 0 : Convert.ToInt32(dr["Quantity"]);
                    objICPRInfo.Price = Convert.ToDecimal(dr["PRICE"]) == 0 ? 0 : Convert.ToDecimal(dr["PRICE"]);
                    objICPRInfo.TotalPrice = Convert.ToDecimal(objICPRInfo.Quantity * objICPRInfo.RPrice) == 0 ? 0 : Convert.ToDecimal(objICPRInfo.Quantity * objICPRInfo.RPrice);
                    Int32 apstatus = Convert.ToInt32(dr["AppStatus"]) == 0 ? 0 : Convert.ToInt32(dr["AppStatus"]);
                    if (apstatus == 0)
                    { objICPRInfo.AppStatus = "Pending"; }
                    else if (apstatus == 1)
                    { objICPRInfo.AppStatus = "In Process"; }
                    else if (apstatus == 2)
                    { objICPRInfo.AppStatus = "Fully Accepted"; }
                    else if (apstatus == 3)
                    { objICPRInfo.AppStatus = "Rejected"; }
                    else if (apstatus == 4)
                    { objICPRInfo.AppStatus = "Cancelled"; }
                    else
                    { objICPRInfo.AppStatus = "Partially Accepted"; }
                    amt += objICPRInfo.TotalPrice;
                    totalqty += objICPRInfo.Quantity;
                    objICPRInfo.TotalAmount = amt;
                    objICPRInfo.TotalQuantity = totalqty;
                    objPRCDInfo.Add(objICPRInfo);
                }

            }
            //else
            //{
            //    inventoryChildPartReturnInfo objICPRInfo = new inventoryChildPartReturnInfo();
            //    objICPRInfo.ID = 0;
            //    objPRCDInfo.Add(objICPRInfo);

            //}
            return objPRCDInfo.ToList();

        }

        public List<Statistics> GetViewStats(string userID)
        {
            var id_en = DecryptAes(userID);
            Int32 uids = Convert.ToInt32(id_en);

            List<Statistics> onjstats = new List<Statistics>();
            for (int i = 0; i <= 6; i++)
            {
                CommanParameter cp;
                CommanParametrs cps = new CommanParametrs();
                cp = new CommanParameter("@ID", uids, DbType.Int32);
                cps.Add(cp);
                cp = new CommanParameter("@Status", i, DbType.Int32);
                cps.Add(cp);
                DataTable dt = maincls.DbCon.GetProcTable("Proc_GetViewRRStats", cps, "Request");
                Statistics stats = new Statistics();
                stats.ID = Convert.ToInt32(dt.Rows[0][0]);
                onjstats.Add(stats);
            }
            return onjstats.ToList();
        }

        public List<CallTagStatus> CallTagGeneration(string ReturnNo, string Priority, string DistributorId)
        {
            var id_en = DecryptAes(DistributorId);
            Int32 uids = Convert.ToInt32(id_en);

            string result = string.Empty;
            string getresult = string.Empty;
            List<CallTagStatus> objstatus = new List<CallTagStatus>();
            CallTagStatus em = new CallTagStatus();
            try
            {
                if (Convert.ToInt32(Priority) == 0)
                {
                    result = "A Request For Call Tag Is Already Under Process.";
                    em.Message = result;
                    em.Status = "";
                    objstatus.Add(em);
                    return objstatus.ToList();
                }
                getresult = GetCallTagDetail(ReturnNo, uids);
                if (getresult == null || getresult == "")
                {
                    result = "Call Tag Not Generated, Please Call To Administrator.";
                    em.Message = result;
                    em.Status = "";
                    objstatus.Add(em);
                    return objstatus.ToList();
                }
                else
                {
                    //result = result;// "Your Request For Call Tag Has Been Processed Successfully.<br/><br/>Please Note FedEx Confirmation No. As Under<br/><br/> <b>Confirmation No. :" + getresult + "</b><br/><br/>Note : FedEx Representative Will Come With Call Tag.";
                    em.Message = getresult;
                    em.Status = getresult;
                    objstatus.Add(em);
                    return objstatus.ToList();
                }
            }
            catch (Exception ex)
            {
                result = "Call Tag Not Generated, Please Call To Administrator.";
                em.Message = result;
                em.Status = "";
                objstatus.Add(em);
                return objstatus.ToList();
            }

        }

        public string GetCallTagDetail(string ReturnNo, Int32 DistributorId)
        {
            Catalog.MainCommon MainClass = Catalog.MainCommon.Instance();
            CourierConfigAdminData objOrderMaster = new CourierConfigAdminData();
            DataTable dt = null;
            string dealer = string.Empty, phone = string.Empty, address = string.Empty, city = string.Empty, state = string.Empty, pincode = string.Empty, country = string.Empty;
            string compname = string.Empty, cphone = string.Empty, caddress = string.Empty, ccity = string.Empty, cstate = string.Empty, cpincode = string.Empty, ccountry = string.Empty;
            string ApiKey = string.Empty, Password = string.Empty, MeterNo = string.Empty, AccNo = string.Empty, Url = string.Empty;
            string strval = string.Empty;
            bool ProxyAllow = false;
            string ProxyURL = string.Empty, ProxyUser = string.Empty, ProxyPassword = string.Empty;
            DataTable dtProxy = MainClass.DbCon.GetTable("select ProxyAllow,ProxyURL,ProxyUser,ProxyPassword from tbl_setting", "tbl_setting");
            if (dtProxy.Rows.Count > 0)
            {
                ProxyAllow = Convert.ToBoolean(dtProxy.Rows[0][0]);
                ProxyURL = Convert.ToString(dtProxy.Rows[0][1]);
                ProxyUser = Convert.ToString(dtProxy.Rows[0][2]);
                ProxyPassword = Convert.ToString(dtProxy.Rows[0][3]);
            }
            dt = objOrderMaster.GetListForTrack(Convert.ToInt32(Catalog.AddressMethod.FedEx), Convert.ToInt32(Catalog.ShippingMethod.Return_Label));

            if (dt.Rows.Count > 0)
            {
                ApiKey = Convert.ToString(dt.Rows[0]["APIKey"]);
                Password = Convert.ToString(dt.Rows[0]["Password"]);
                MeterNo = Convert.ToString(dt.Rows[0]["MeterNo"]);
                AccNo = Convert.ToString(dt.Rows[0]["AccountNo"]);
                Url = Convert.ToString(dt.Rows[0]["ServiceURL"]);

                compname = Convert.ToString(dt.Rows[0]["CompanyName"]);
                cphone = Convert.ToString(dt.Rows[0]["PhoneNo"]);
                caddress = Convert.ToString(dt.Rows[0]["Address"]);
                ccity = Convert.ToString(dt.Rows[0]["CityName"]);
                cstate = Convert.ToString(dt.Rows[0]["StateAlias"]);
                cpincode = Convert.ToString(dt.Rows[0]["PinCode"]);
                ccountry = Convert.ToString(dt.Rows[0]["CountryName"]);


                FedexCallTagGround.CallTagConfig objCallTag = new FedexCallTagGround.CallTagConfig(ApiKey, Password, MeterNo, AccNo, Url, ProxyAllow, ProxyURL, ProxyUser, ProxyPassword);

                string sql = @" Select dist.ID, dist.Name, osa.Address1 as Address, osa.PinCode, osa.Phone, c.CountryCode,s.StateAlias,ct.CityName 
                        From tbl_Distributor dist 
                        inner join tbl_ReturnMst rmt on dist.ID = rmt.DealerId
                        inner join tbl_OrderShippingAddress osa on dist.ID = osa.DistributorID and rmt.PKAddID = osa.ID
                        inner join tbl_Country c on osa.CountryID=c.id
                        inner join tbl_state s on osa.StateID=s.id
                        inner join tbl_city ct on osa.CityID=ct.id
                        where dist.ID='" + DistributorId + "' and upper(ltrim(rtrim(rmt.ReturnNo)))='" + ReturnNo.Trim().ToUpper() + "'";


                DataTable dtDealer = MainClass.DbCon.GetTable(sql, "tbl_DistributorAddress");

                if (dtDealer.Rows.Count > 0)
                {
                    dealer = Convert.ToString(dtDealer.Rows[0]["Name"]);
                    phone = Convert.ToString(dtDealer.Rows[0]["Phone"]);
                    address = Convert.ToString(dtDealer.Rows[0]["Address"]);
                    city = Convert.ToString(dtDealer.Rows[0]["CityName"]);
                    state = Convert.ToString(dtDealer.Rows[0]["StateAlias"]);
                    pincode = Convert.ToString(dtDealer.Rows[0]["PinCode"]);
                    country = Convert.ToString(dtDealer.Rows[0]["CountryCode"]);

                    //ConfirmationNo = objFedexTag.TrackResult("MAHINDRA USA, INC.", "9015551212", "9020 Jackrabbit Road, Suite 600", "Houston", "TX", "77095", "US", "Joe", "3305551234", "1234 Main Street Suite 200", "Akron", "OH", "44333", "US", "212121212");


                    strval = objCallTag.TrackResult(compname, cphone, caddress, ccity.Trim(), cstate, cpincode, ccountry, dealer, phone, address, city.Trim(), state, pincode, country, ReturnNo);

                    if (strval.Length > 500)
                        strval = strval.Substring(0, 500);

                    decimal vInsert = MainClass.DbCon.ExecuteNonQuery("Insert into tbl_CallTagLog(DistID,URL,MeterNo,AccountNo,IPAddress, CreatedDate, ErrMessage) values(" + DistributorId + ",'" + Url + "','" + MeterNo + "','" + AccNo + "','" + HttpContext.Current.Request.UserHostAddress + "',getdate(),'" + strval + "')");

                    decimal n;
                    bool isNumeric = decimal.TryParse(strval, out n);
                    if (isNumeric)
                    {
                        decimal i = MainClass.DbCon.ExecuteNonQuery("update tbl_ReturnMst set ChangedDate=getdate() , callTagNo='" + strval + "' where ReturnNo='" + ReturnNo + "' ");
                    }

                }
            }
            return strval;
        }

        # endregion

        # region Part Applicability

        public List<PartApplicableInfo> partApplicabilityList(string UserId, string Partno, Int32 pagesize)
        {
            var UserId_en = DecryptAes(UserId);
            Int32 uids = Convert.ToInt32(UserId_en);

            string Partno_en = DecryptAes(Partno);

            List<PartApplicableInfo> plist = new List<PartApplicableInfo>();
            try
            {
                Int32 iCountryId = 1;

                if (uids > 0)
                    iCountryId = Convert.ToInt32(maincls.DbCon.ExecuteScaler("select CountryId from tbl_user where id=" + uids));

                // Scroll size in Part search
                int scrollsize = Convert.ToInt32(ConfigurationManager.AppSettings["partscrollsize"]);

                CommanParametrs cps = new CommanParametrs();
                CommanParameter cp;
                cp = new CommanParameter("@PartCategoryID", 1, DbType.Int32);
                cps.Add(cp);
                cp = new CommanParameter("@PartNo", Partno_en, DbType.String);
                cps.Add(cp);
                cp = new CommanParameter("@CountryID", iCountryId, DbType.Int32);
                cps.Add(cp);
                cp = new CommanParameter("@Page", pagesize, DbType.Int32);
                cps.Add(cp);
                cp = new CommanParameter("@RecsPerPage", scrollsize, DbType.Int32);
                cps.Add(cp);


                //DataTable dtVariants = maincls.DbCon.GetProcTable("PROC_PARTSEARCH", cps, "tmp");
                DataTable dtVariants = maincls.DbCon.GetProcTable("PROC_PARTSEARCHMOB", cps, "tmp");

                //int i = 0;
                foreach (DataRow dr in dtVariants.Rows)
                {
                    PartApplicableInfo pinf = new PartApplicableInfo();
                    pinf.ID = Convert.ToString(dr["PARTID"]) == "" ? "" : Convert.ToString(dr["PARTID"]);
                    pinf.total = dr["COUNTTOTAL"] != DBNull.Value ? Convert.ToInt32(dr["COUNTTOTAL"]) : pinf.total;
                    pinf.partNumber = Convert.ToString(dr["partno"]) == "" ? "" : Convert.ToString(dr["partno"]);
                    pinf.description = Convert.ToString(dr["description"]) == "" ? "" : Convert.ToString(dr["description"]);
                    pinf.servicable = Convert.ToString(dr["nonserviceable"]) == "0" ? "NS" : "S";
                    pinf.qty = Convert.ToString(dr["moqty"]) == "0" ? "0" : Convert.ToString(dr["moqty"]);
                    Int32 pid = Convert.ToInt32(dr["PARTID"]) == 0 ? 0 : Convert.ToInt32(dr["PARTID"]);
                    pinf.Applicables = PartApplicability(pid, iCountryId);

                    //if (pinf.Applicables.Count > 0)
                    //{
                        plist.Add(pinf);

                   // }
                }

                return plist.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("partApplicabilityList", ex);
            }

        }



        public List<Applicable> PartApplicability(Int32 PartId, Int32 iCountryId)
        {
            List<Applicable> partlist = new List<Applicable>();
            try
            {
                //Creating objects of comman parameters
                CommanParametrs objcps = new CommanParametrs();
                CommanParameter objcp;

                //Assign Perameter value
                objcp = new CommanParameter("@PartID", PartId, DbType.Int32);
                objcps.Add(objcp);
                objcp = new CommanParameter("@CountryID", iCountryId, DbType.Int32);
                objcps.Add(objcp);
                DataTable dtpartapplicability = maincls.DbCon.GetProcTable("PROC_PARTVARIANTLIST", objcps, "tmp");
                foreach (DataRow dr in dtpartapplicability.Rows)
                {
                    Applicable pinfo = new Applicable();
                    pinfo.Vehicle = Convert.ToString(dr["Vehicle"]) == "" ? "" : Convert.ToString(dr["Vehicle"]);
                    pinfo.VehicleId = Convert.ToInt32(dr["VehicleID"]) == 0 ? 0 : Convert.ToInt32(dr["VehicleID"]);
                    pinfo.Variant = Convert.ToString(dr["Variant"]) == "" ? "" : Convert.ToString(dr["Variant"]);
                    pinfo.quantity = Convert.ToInt32(dr["Qty"]) == 0 ? 0 : Convert.ToInt32(dr["Qty"]);
                    pinfo.startdate = Convert.ToString(dr["StartDate"]) == "" ? "" : Convert.ToString(dr["StartDate"]);
                    pinfo.enddate = Convert.ToString(dr["EndDate"]) == "" ? "" : Convert.ToString(dr["EndDate"]);
                    partlist.Add(pinfo);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("PartApplicability", ex);
            }
            return partlist;
        }

        #endregion

        # region Alternate Parts


        public List<AlternatePartInfo> GetAlternateParts(string PartNo, string ptype, string utype, string countryid)
        {
            var ut_en = DecryptAes(utype);
            Int32 usertype = Convert.ToInt32(ut_en);

            string Partno_en = DecryptAes(PartNo);

            var cId_en = DecryptAes(countryid);
            Int32 cid = Convert.ToInt32(cId_en);

            List<AlternatePartInfo> objAlternate = new List<AlternatePartInfo>();
            OrderListData objListDetail = new OrderListData();

            //Getting DataTable of Alternate Parts
            DataTable dtAlternatePartsList = objListDetail.GetAlternateParts(Partno_en, ptype, usertype, cid);
            if (dtAlternatePartsList.Rows.Count > 0)
            {
                foreach (DataRow dr in dtAlternatePartsList.Rows)
                {
                    AlternatePartInfo apinfo = new AlternatePartInfo();
                    apinfo.ID = Convert.ToInt32(dr["ID"]);
                    apinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                    apinfo.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                    apinfo.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                    apinfo.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                    apinfo.Price2 = Convert.ToString(dr["PRICE2"]) == "" ? "" : Convert.ToString(dr["PRICE2"]);
                    apinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                    objAlternate.Add(apinfo);
                }
            }
            return objAlternate.ToList();
        }


        public List<AlternatePartInfo> AddOrderIllustrationData(string rType, string PID, string ID, string Qty, string cid, Int32 lngid, string usertype, string MId, string decAId)
        {
            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            string pid_en = DecryptAes(PID);
            string qty_en = DecryptAes(Qty);

            var ut_en = DecryptAes(usertype);
            Int32 utype = Convert.ToInt32(ut_en);

            Catalog.MainCommon MainClass = Catalog.MainCommon.Instance();
            List<AlternatePartInfo> objAlternate = new List<AlternatePartInfo>();
            //Set the string variable as empty.
            string strSql = string.Empty;
            //Get the datetime into datetime object.
            DateTime dteSeldate = DateTime.Now;

            DataTable dtOrderList = null;

            object strSelDate = dteSeldate;
            if (rType == "Part")
            {
                //Get the partID into string type variable 'arrPartID'.
                string arrPartID = Convert.ToString(pid_en);
                if (arrPartID.IndexOf(',') == 0)
                    arrPartID = arrPartID.Remove(0, 1);
                //Split the part ID.
                string[] strarrPID = arrPartID.Split(',');
                //Get the quantity into string type variable 'arrPartQty'.
                string arrPartQty = Convert.ToString(qty_en);
                //Split the part quantity.
                string[] strarrPQty = arrPartQty.Split(',');
                string strpartid = string.Empty;
                for (int i = 0; i < strarrPID.Length; i++)
                {
                    if (dtOrderList != null)
                    {
                        //Get the ID into datarow.
                        DataRow[] arrdr = dtOrderList.Select("ID=" + strarrPID[i]);
                        //Check the datarow length.
                        if (arrdr.Length > 0)
                        {
                            //Get the datarow value into another datarow.
                            arrdr[0]["NQTY"] = Convert.ToInt32(arrdr[0]["NQTY"]) + Convert.ToInt32(strarrPQty[i]);
                            continue;
                        }
                    }
                    //creating an instance of AddToOrderListData
                    AddToOrderListData ObjAddToLData = new AddToOrderListData();
                    // Updating part Qty. if null then 1
                    if (strarrPQty[i] == "null")
                    {
                        strarrPQty[i] = "1";
                    }
                    // Calling Procedure to fill datatable.
                    DataTable dtPD = FillGrdPart(Convert.ToInt32(strarrPQty[i]), Convert.ToString(strarrPID[i]), cids, lngid, utype);
                    //DataTable dtPD = MainClass.DbCon.GetTable(strSql, "tbl_Part");
                    if (dtOrderList == null) dtOrderList = dtPD.Clone();
                    //Merge the datatable into another table.
                    dtOrderList.Merge(dtPD);
                }
            }
            else
            {
                //Creating an instence of AddToOrderListData class.
                AddToOrderListData ObjAddToLData = new AddToOrderListData();
                // Calling Procedure to fill datatable.
                dtOrderList = ObjAddToLData.FillGrd(Convert.ToInt32(ID), Convert.ToInt32(MId), cids, lngid, utype);
            }
            //Add some addtional columns in order list
            DataTable dtpart = dtOrderList;
            //Set the string variable as empty.
            string strModelid = string.Empty;
            string strAssID = string.Empty;
            decimal decMId;
            //Getting ModelID and Assembly IDs in different cases.
            if (Convert.ToString(rType) != "Part")
            {
                //Get the modelID into another variable.
                strModelid = "," + MId;
                //Get the ID into another variable.
                strAssID = "," + ID;
                //Get the value into object.
                object count = MainClass.DbCon.ExecuteScaler(@"select count(*) from tbl_parttree where VariantID in (0" + strModelid + ") And  AssemblyID in (0" + strAssID + ")");
                //Check the count for greater than zero.
                if (Convert.ToInt32(count) > 0)
                {
                    //Checking part history for each row of order list data table
                    foreach (DataRow dr in dtpart.Rows)
                    {
                        //Get the modelID or decMID.
                        decMId = Convert.ToDecimal(rType == "Part" ? Convert.ToString(dr["MODELID"]) : Convert.ToString(MId));

                        string sSql = @"select count(*) from tbl_parttree pt
                        Where partid=" + dr["ID"].ToString() + @"
                        And VariantID=" + decMId.ToString() + " And  AssemblyID= " + dr["AssemblyId"].ToString() + @"
                        And AssemblypartId=" + Convert.ToString(dr["AssemblypartId"]);
                        //Return the integer value and assign to another interger.
                        int icnt = Convert.ToInt32(MainClass.DbCon.ExecuteScaler(sSql));
                        //Check the icnt value for greater than zero.
                        if (icnt > 0)
                        {
                            //Create the new object for clsHistory.
                            clsHistory objHistory = new clsHistory();
                            //Get the datarow value for updation on the basis of parameters.
                            DataRow drUpdate = GetUpdatedPart(decMId, Convert.ToDecimal(dr["ASSEMBLYPARTID"]), dteSeldate, Convert.ToInt32(dr["NQTY"]), utype);
                            //Check the drUpdate for not null.
                            if (drUpdate != null)
                            {
                                //Updating rows
                                dr["ID"] = drUpdate["PartID"];
                                dr["PartNo"] = drUpdate["PartNo"];
                                dr["Description"] = drUpdate["Description"];
                                dr["startdate"] = drUpdate["startdate"];
                                dr["enddate"] = drUpdate["enddate"];
                                dr["MOQTY"] = Convert.ToInt32(drUpdate["MOQTY"]) == 0 ? 0 : Convert.ToInt32(drUpdate["MOQTY"]);
                                dr["MPQTY"] = Convert.ToInt32(drUpdate["MPQTY"]) == 0 ? 0 : Convert.ToInt32(drUpdate["MPQTY"]);
                                dr["price1"] = drUpdate["price1"];
                                dr["Status"] = drUpdate["Status"];
                                //Save the changes.
                                dr.AcceptChanges();
                            }
                            else
                            {
                                if (HttpContext.Current.Session["SrVINNO"] != null)
                                    //Delete the datarow.
                                    dr.Delete();
                            }
                        }
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            //Check for type value.
                            if (rType == "Basket" || rType == "PartSearch" || rType == "Assembly")
                            {
                                if (Convert.ToDateTime(dr["StartDate"]) > Convert.ToDateTime(strSelDate))
                                {
                                    //Delete the datarow.
                                    dr.Delete();
                                }
                                //Check for end date
                                else if (dr["enddate"] != DBNull.Value)
                                    //Check if end date is less than selecetd date with session date
                                    if (Convert.ToDateTime(dr["enddate"]) < Convert.ToDateTime(strSelDate))
                                    {
                                        //Delete the datarow.
                                        dr.Delete();
                                    }
                            }
                        }
                    }
                    //Save the Changes.
                    dtpart.AcceptChanges();
                }
                else
                {
                    if (rType != "Part")
                    {
                        foreach (DataRow dr in dtpart.Select("StartDate>" + MainClass.GetDateString(Convert.ToDateTime(strSelDate)) + " or Enddate <" + MainClass.GetDateString(Convert.ToDateTime(strSelDate))))
                        {
                            //Delete the datarow.
                            dr.Delete();
                        }
                        //Save the changes.
                        dtpart.AcceptChanges();
                    }
                }
            }
            //For Mahindra Logan-Verito
            //-----------------------------------------------------------------------
            //if (Convert.ToString(HttpContext.Current.Session["SrVINNO"]) != "" && Convert.ToString(HttpContext.Current.Session["ISBOMREQUIRED"]) == "1")
            //{
            //    //Find the Part mapped with this BOM ID // decAId insted of HttpContext.Current.Session["Id"].ToString()
            //    DataTable dtRpoVin = MainClass.DbCon.GetTable(@"select PARTID from TBL_SBOMPARTDETAILS where Assemblyid='" + decAId + "' and SBOMID='" + HttpContext.Current.Session["SBOMID"].ToString() + "'", "ABC");

            //    //Add the Primary Key attribute on this table
            //    DataColumn[] keyColumns = new DataColumn[1];
            //    keyColumns[0] = dtRpoVin.Columns["PARTID"];
            //    dtRpoVin.PrimaryKey = keyColumns;
            //    dtRpoVin.AcceptChanges();
            //    foreach (DataRow dr in dtpart.Rows)
            //    {
            //        //find whether the part exist or not
            //        DataRow drc = dtRpoVin.Rows.Find(dr["ID"]);
            //        //if does not exist delete
            //        if (drc == null)
            //        {
            //            dr.Delete();
            //        }
            //    }
            //    //accepting all changes
            //    dtpart.AcceptChanges();
            //}
            //-----------------------------------------------------------------------

            // code for merging the rows for similar AssemblyPartID.
            DataTable dtTmp = dtpart.Clone(); // creating temporary table.
            //Geting order list from session 
            DataTable dtOrder = new DataTable();// (DataSetCommon.OrderListDataTable)HttpContext.Current.Session["OrderList"];
            //Updating qty for each record
            foreach (DataRow drt in dtpart.Rows)
            {
                DataRow[] arrDrPart = null;
                DataRow[] arrDrTmp = dtTmp.Select("ID=" + drt["ID"].ToString());
                // check for assembly part already exists or not.
                if (dtTmp.Rows.Count <= 0 || arrDrTmp.Length <= 0)
                {
                    //Add the new row into datatable.
                    DataRow drNew = dtTmp.NewRow();
                    // checking how many rows exist in the main table for the current assemblypart.
                    arrDrPart = dtpart.Select("ID=" + drt["ID"].ToString());
                    // copying values of the current row in the new row
                    for (int iCount = 0; iCount < dtpart.Columns.Count; iCount++)
                    {
                        // Removing ModelID from the New Row.
                        if (dtpart.Columns[iCount].ColumnName.ToUpper() != "MODELID")
                            drNew[iCount] = drt[iCount];
                    }
                    // if more than one rows exist in the main table then
                    //setting Existing,New and Total Qty
                    if (arrDrPart.Length > 1)
                    {
                        //Initialize the variable as zero.
                        int iNQty = 0;
                        //Updating qty.
                        foreach (DataRow drs in arrDrPart)
                        {
                            iNQty += (drs["NQty"] == DBNull.Value) ? 0 : Convert.ToInt32(drs["NQty"]);
                        }
                        //Assign the variable into another datarow.
                        drNew["NQty"] = iNQty;
                    }
                    //if (dtOrder != null)
                    //{

                    //    DataRow[] arrdrselect;
                    //    arrdrselect = dtOrder.Select("Id=" + drt["ID"]);
                    //    //filling the quantity.
                    //    if (arrdrselect.Length > 0)
                    //    {
                    //        //Loop for Get the Qty of Part
                    //        foreach (DataRow drPart in arrdrselect)
                    //        {
                    //            //Checking that Qty is not null
                    //            if (drNew["Qty"] != null && drPart["Qty"] != null)
                    //                drNew["Qty"] = Convert.ToInt32(drNew["Qty"]) + Convert.ToInt32(drPart["Qty"]);
                    //        }
                    //    }
                    //}
                    //// Adding this row in the new table.
                    //dtTmp.Rows.Add(drNew);
                }
            }
            //dtpart.Clear(); // clearing the part table.
            // dtpart = dtTmp; // 
            // HttpContext.Current.Session["OrdPartViewState"] = dtpart;
            DataTable dtCloned = dtpart.Clone();
            dtCloned.Columns["NQTY"].DataType = typeof(string);

            foreach (DataRow row in dtpart.Rows)
            {
                dtCloned.ImportRow(row);
            }
            foreach (DataRow dr in dtCloned.Rows)
            {
                AlternatePartInfo apinfo = new AlternatePartInfo();
                apinfo.ID = Convert.ToInt32(dr["ID"]);
                apinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                apinfo.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                apinfo.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);// MUSA
                // apinfo.Available = "null";//Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);//mahindra & mahindra
                apinfo.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                apinfo.Price2 = Convert.ToString(dr["PRICE2"]) == "" ? "" : Convert.ToString(dr["PRICE2"]);
                apinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                apinfo.aGroupID = Convert.ToString(dr["AGROUPID"]) == "" ? "0" : Convert.ToString(dr["AGROUPID"]);//MUSA
                //apinfo.aGroupID = "0";//MFS
                objAlternate.Add(apinfo);
            }

            return objAlternate.ToList();

        }


        public List<AlternatePartInfo> AlternatePartList(string rType, string PID, string ID, string Qty, string GroupId, string cid, Int32 lngid, string usertype, string MId, string decAId)
        {
            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            string pid_en = DecryptAes(PID);
            string qty_en = DecryptAes(Qty);

            var ut_en = DecryptAes(usertype);
            Int32 utype = Convert.ToInt32(ut_en);

            Catalog.MainCommon MainClass = Catalog.MainCommon.Instance();
            List<AlternatePartInfo> objAlternate = new List<AlternatePartInfo>();
            //Set the string variable as empty.
            string strSql = string.Empty;
            //Get the datetime into datetime object.
            DateTime dteSeldate = DateTime.Now;
            //if (Convert.ToString(EndDate) != String.Empty && Convert.ToString(EndDate) != null)
            //    //Get the enddate into dteseldate object. 
            //    dteSeldate = Convert.ToDateTime(EndDate);
            //else
            //    //Get the seldate into dteseldate. 
            //    dteSeldate = Convert.ToDateTime(HttpContext.Current.Session["SelDate"]);

            DataTable dtOrderList = null;
            //strSelDate = MainClass.GetDateString(dteSeldate);
            object strSelDate = dteSeldate;
            if (rType == "Part")
            {
                //Get the partID into string type variable 'arrPartID'.
                string arrPartID = Convert.ToString(pid_en);
                if (arrPartID.IndexOf(',') == 0)
                    arrPartID = arrPartID.Remove(0, 1);
                //Split the part ID.
                string[] strarrPID = arrPartID.Split(',');
                //Get the quantity into string type variable 'arrPartQty'.
                string arrPartQty = Convert.ToString(qty_en);
                //Split the part quantity.
                string[] strarrPQty = arrPartQty.Split(',');
                string strpartid = string.Empty;
                for (int i = 0; i < strarrPID.Length; i++)
                {
                    if (dtOrderList != null)
                    {
                        //Get the ID into datarow.
                        DataRow[] arrdr = dtOrderList.Select("ID=" + strarrPID[i]);
                        //Check the datarow length.
                        if (arrdr.Length > 0)
                        {
                            //Get the datarow value into another datarow.
                            arrdr[0]["NQTY"] = Convert.ToInt32(arrdr[0]["NQTY"]) + Convert.ToInt32(strarrPQty[i]);
                            continue;
                        }
                    }
                    //creating an instance of AddToOrderListData
                    AddToOrderListData ObjAddToLData = new AddToOrderListData();
                    // Updating part Qty. if null then 1
                    if (strarrPQty[i] == "null")
                    {
                        strarrPQty[i] = "1";
                    }
                    // Calling Procedure to fill datatable.
                    DataTable dtPD = FillAlternatePart(Convert.ToInt32(strarrPQty[i]), Convert.ToString(strarrPID[i]), cids, lngid, Convert.ToInt32(GroupId), utype);
                    //DataTable dtPD = MainClass.DbCon.GetTable(strSql, "tbl_Part");
                    if (dtOrderList == null) dtOrderList = dtPD.Clone();
                    //Merge the datatable into another table.
                    dtOrderList.Merge(dtPD);
                }
            }
            else
            {
                //Creating an instence of AddToOrderListData class.
                AddToOrderListData ObjAddToLData = new AddToOrderListData();
                // Calling Procedure to fill datatable.
                dtOrderList = ObjAddToLData.FillGrdAlternatePart(Convert.ToInt32(ID), Convert.ToInt32(MId), cids, lngid, utype, Convert.ToInt32(GroupId));
            }
            //Add some addtional columns in order list
            DataTable dtpart = dtOrderList;
            //Set the string variable as empty.
            string strModelid = string.Empty;
            string strAssID = string.Empty;
            decimal decMId;
            //Getting ModelID and Assembly IDs in different cases.
            if (Convert.ToString(rType) != "Part")
            {
                //Get the modelID into another variable.
                strModelid = "," + MId;
                //Get the ID into another variable.
                strAssID = "," + ID;
                //Get the value into object.
                object count = MainClass.DbCon.ExecuteScaler(@"select count(*) from tbl_parttree where VariantID in (0" + strModelid + ") And  AssemblyID in (0" + strAssID + ")");
                //Check the count for greater than zero.
                if (Convert.ToInt32(count) > 0)
                {
                    //Checking part history for each row of order list data table
                    foreach (DataRow dr in dtpart.Rows)
                    {
                        //Get the modelID or decMID.
                        decMId = Convert.ToDecimal(rType == "Part" ? Convert.ToString(dr["MODELID"]) : Convert.ToString(MId));

                        string sSql = @"select count(*) from tbl_parttree pt
                        Where partid=" + dr["ID"].ToString() + @"
                        And VariantID=" + decMId.ToString() + " And  AssemblyID= " + dr["AssemblyId"].ToString() + @"
                        And AssemblypartId=" + Convert.ToString(dr["AssemblypartId"]);
                        //Return the integer value and assign to another interger.
                        int icnt = Convert.ToInt32(MainClass.DbCon.ExecuteScaler(sSql));
                        //Check the icnt value for greater than zero.
                        if (icnt > 0)
                        {
                            //Create the new object for clsHistory.
                            clsHistory objHistory = new clsHistory();
                            //Get the datarow value for updation on the basis of parameters.
                            DataRow drUpdate = objHistory.GetUpdatedPart(decMId, Convert.ToDecimal(dr["ASSEMBLYPARTID"]), dteSeldate, Convert.ToInt32(dr["NQTY"]));
                            //Check the drUpdate for not null.
                            if (drUpdate != null)
                            {
                                //Updating rows
                                dr["ID"] = drUpdate["PartID"];
                                dr["PartNo"] = drUpdate["PartNo"];
                                dr["Description"] = drUpdate["Description"];
                                dr["startdate"] = drUpdate["startdate"];
                                dr["enddate"] = drUpdate["enddate"];
                                dr["MOQTY"] = drUpdate["MOQTY"];
                                dr["MPQTY"] = drUpdate["MPQTY"];
                                dr["price1"] = drUpdate["price1"];
                                dr["Status"] = drUpdate["Status"];
                                //Save the changes.
                                dr.AcceptChanges();
                            }
                            else
                            {
                                if (HttpContext.Current.Session["SrVINNO"] != null)
                                    //Delete the datarow.
                                    dr.Delete();
                            }
                        }
                        if (dr.RowState != DataRowState.Deleted)
                        {
                            //Check for type value.
                            if (rType == "Basket" || rType == "PartSearch" || rType == "Assembly")
                            {
                                if (Convert.ToDateTime(dr["StartDate"]) > Convert.ToDateTime(strSelDate))
                                {
                                    //Delete the datarow.
                                    dr.Delete();
                                }
                                //Check for end date
                                else if (dr["enddate"] != DBNull.Value)
                                    //Check if end date is less than selecetd date with session date
                                    if (Convert.ToDateTime(dr["enddate"]) < Convert.ToDateTime(strSelDate))
                                    {
                                        //Delete the datarow.
                                        dr.Delete();
                                    }
                            }
                        }
                    }
                    //Save the Changes.
                    dtpart.AcceptChanges();
                }
                else
                {
                    if (rType != "Part")
                    {
                        foreach (DataRow dr in dtpart.Select("StartDate>" + MainClass.GetDateString(Convert.ToDateTime(strSelDate)) + " or Enddate <" + MainClass.GetDateString(Convert.ToDateTime(strSelDate))))
                        {
                            //Delete the datarow.
                            dr.Delete();
                        }
                        //Save the changes.
                        dtpart.AcceptChanges();
                    }
                }
            }
            //For Mahindra Logan-Verito
            //-----------------------------------------------------------------------
            //if (Convert.ToString(HttpContext.Current.Session["SrVINNO"]) != "" && Convert.ToString(HttpContext.Current.Session["ISBOMREQUIRED"]) == "1")
            //{
            //    //Find the Part mapped with this BOM ID // decAId insted of HttpContext.Current.Session["Id"].ToString()
            //    DataTable dtRpoVin = MainClass.DbCon.GetTable(@"select PARTID from TBL_SBOMPARTDETAILS where Assemblyid='" + decAId + "' and SBOMID='" + HttpContext.Current.Session["SBOMID"].ToString() + "'", "ABC");

            //    //Add the Primary Key attribute on this table
            //    DataColumn[] keyColumns = new DataColumn[1];
            //    keyColumns[0] = dtRpoVin.Columns["PARTID"];
            //    dtRpoVin.PrimaryKey = keyColumns;
            //    dtRpoVin.AcceptChanges();
            //    foreach (DataRow dr in dtpart.Rows)
            //    {
            //        //find whether the part exist or not
            //        DataRow drc = dtRpoVin.Rows.Find(dr["ID"]);
            //        //if does not exist delete
            //        if (drc == null)
            //        {
            //            dr.Delete();
            //        }
            //    }
            //    //accepting all changes
            //    dtpart.AcceptChanges();
            //}
            //-----------------------------------------------------------------------

            // code for merging the rows for similar AssemblyPartID.
            DataTable dtTmp = dtpart.Clone(); // creating temporary table.
            //Geting order list from session 
            DataTable dtOrder = new DataTable();//(DataSetCommon.OrderListDataTable)HttpContext.Current.Session["OrderList"];
            //Updating qty for each record
            foreach (DataRow drt in dtpart.Rows)
            {
                DataRow[] arrDrPart = null;
                DataRow[] arrDrTmp = dtTmp.Select("ID=" + drt["ID"].ToString());
                // check for assembly part already exists or not.
                if (dtTmp.Rows.Count <= 0 || arrDrTmp.Length <= 0)
                {
                    //Add the new row into datatable.
                    DataRow drNew = dtTmp.NewRow();
                    // checking how many rows exist in the main table for the current assemblypart.
                    arrDrPart = dtpart.Select("ID=" + drt["ID"].ToString());
                    // copying values of the current row in the new row
                    for (int iCount = 0; iCount < dtpart.Columns.Count; iCount++)
                    {
                        // Removing ModelID from the New Row.
                        if (dtpart.Columns[iCount].ColumnName.ToUpper() != "MODELID")
                            drNew[iCount] = drt[iCount];
                    }
                    // if more than one rows exist in the main table then
                    //setting Existing,New and Total Qty
                    if (arrDrPart.Length > 1)
                    {
                        //Initialize the variable as zero.
                        int iNQty = 0;
                        //Updating qty.
                        foreach (DataRow drs in arrDrPart)
                        {
                            iNQty += (drs["NQty"] == DBNull.Value) ? 0 : Convert.ToInt32(drs["NQty"]);
                        }
                        //Assign the variable into another datarow.
                        drNew["NQty"] = iNQty;
                    }
                    //if (dtOrder != null)
                    //{

                    //    DataRow[] arrdrselect;
                    //    arrdrselect = dtOrder.Select("Id=" + drt["ID"]);
                    //    //filling the quantity.
                    //    if (arrdrselect.Length > 0)
                    //    {
                    //        //Loop for Get the Qty of Part
                    //        foreach (DataRow drPart in arrdrselect)
                    //        {
                    //            //Checking that Qty is not null
                    //            if (drNew["Qty"] != null && drPart["Qty"] != null)
                    //                drNew["Qty"] = Convert.ToInt32(drNew["Qty"]) + Convert.ToInt32(drPart["Qty"]);
                    //        }
                    //    }
                    //}
                    //// Adding this row in the new table.
                    //dtTmp.Rows.Add(drNew);
                }
            }
            // dtpart.Clear(); // clearing the part table.
            // dtpart = dtTmp; // 
            //HttpContext.Current.Session["OrdPartViewState"] = dtpart;
            DataTable dtCloned = dtpart.Clone();
            dtCloned.Columns["NQTY"].DataType = typeof(string);

            foreach (DataRow row in dtpart.Rows)
            {
                dtCloned.ImportRow(row);
            }
            foreach (DataRow dr in dtCloned.Rows)
            {
                AlternatePartInfo apinfo = new AlternatePartInfo();
                apinfo.ID = Convert.ToInt32(dr["ID"]);
                apinfo.partNumber = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                apinfo.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                apinfo.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                apinfo.Price1 = Convert.ToString(dr["PRICE1"]) == "" ? "" : Convert.ToString(dr["PRICE1"]);
                apinfo.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                objAlternate.Add(apinfo);
            }
            return objAlternate.ToList();

        }

        public DataTable FillGrdPart(int iQty, string strID, int iCountryID, int LngId, int usertype)
        {
            CommanParametrs Cps = new CommanParametrs();
            Cps.Add(new CommanParameter("@Qty", iQty, DbType.Int32, 0));
            Cps.Add(new CommanParameter("@IDs", strID, DbType.String));
            Cps.Add(new CommanParameter("@CountryID", iCountryID, DbType.Int32));
            Cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            Cps.Add(new CommanParameter("@uType", usertype, DbType.Int32, 0));
            return maincls.DbCon.GetProcTable("PROC_ADDTOORDERLISTPART", Cps, "tbl_part");
        }



        public DataTable FillAlternatePart(int iQty, string strID, int iCountryID, int LngId, int GroupID, int usertype)
        {
            CommanParametrs Cps = new CommanParametrs();
            Cps.Add(new CommanParameter("@Qty", iQty, DbType.Int32, 0));
            Cps.Add(new CommanParameter("@IDs", strID, DbType.String));
            Cps.Add(new CommanParameter("@CountryID", iCountryID, DbType.Int32));
            Cps.Add(new CommanParameter("@LngId", LngId, DbType.Int32, 1));
            Cps.Add(new CommanParameter("@uType", usertype, DbType.Int32, 0));
            Cps.Add(new CommanParameter("@GroupID", GroupID));
            return maincls.DbCon.GetProcTable("PROC_ALTERNATEADDTOORDERLIST", Cps, "tbl_Alternatepart");
        }

        #endregion

        # region Track Shipment Order


        public List<ShipmentInfo> FillShipNoTrack(decimal OrderId)
        {
            List<ShipmentInfo> objshipment = new List<ShipmentInfo>();
            OrderMasterData ObjPartSearchData = new OrderMasterData();
            DataTable dtModels = ObjPartSearchData.GetShipNoForTrack(OrderId);
            if (dtModels.Rows.Count > 0)
            {
                foreach (DataRow dr in dtModels.Rows)
                {
                    ShipmentInfo ship = new ShipmentInfo();
                    ship.ID = Convert.ToInt32(dr["ID"]);
                    ship.ShipNo = Convert.ToString(dr["SHIPNO"]) == "" ? "" : Convert.ToString(dr["SHIPNO"]);
                    objshipment.Add(ship);
                }
            }

            return objshipment.ToList();
        }


        public List<TrackInfo> GetShipmentDetails(int CourierId, string ShipNo)
        {
            List<TrackInfo> trackinfo = new List<TrackInfo>();
            List<Config> configinfo = new List<Config>();
            CourierConfigAdminData objOrderMaster = new CourierConfigAdminData();
            DataTable dt = null;

            dt = objOrderMaster.GetListForTrack(CourierId, 1);

            string ApiKey, Password, MeterNo, AccNo, Url;

            ApiKey = Convert.ToString(dt.Rows[0]["APIKey"]);
            Password = Convert.ToString(dt.Rows[0]["Password"]);
            MeterNo = Convert.ToString(dt.Rows[0]["MeterNo"]);
            AccNo = Convert.ToString(dt.Rows[0]["AccountNo"]);
            Url = Convert.ToString(dt.Rows[0]["ServiceURL"]);


            DataSet dsVal = null;
            dt = null;
            bool ProxyAllow = false;
            string ProxyURL = string.Empty, ProxyUser = string.Empty, ProxyPassword = string.Empty;
            DataTable dtProxy = maincls.DbCon.GetTable("select ProxyAllow,ProxyURL,ProxyUser,ProxyPassword from tbl_setting", "tbl_setting");
            if (dtProxy.Rows.Count > 0)
            {
                ProxyAllow = Convert.ToBoolean(dtProxy.Rows[0][0]);
                ProxyURL = Convert.ToString(dtProxy.Rows[0][1]);
                ProxyUser = Convert.ToString(dtProxy.Rows[0][2]);
                ProxyPassword = Convert.ToString(dtProxy.Rows[0][3]);
            }
            TrackInfo track = new TrackInfo();
            //FedEx and UPS Courier Get Track Ship Order Details
            if (CourierId == 1)
            {

                FedExTrack.FedExConfig objFedx = new FedExTrack.FedExConfig(ShipNo, ApiKey, Password, MeterNo, AccNo, Url, ProxyAllow, ProxyURL, ProxyUser, ProxyPassword);
                //dsVal = new DataSet();
                dsVal = objFedx.TrackResult();


            }
            else
            {
                // UPS Tracker ship details.
               // UPSTrack.UPSConfig objUps = new UPSTrack.UPSConfig(ShipNo, ApiKey, Password, MeterNo, AccNo, Url, ProxyAllow, ProxyURL, ProxyUser, ProxyPassword);
                //dsVal = new DataSet();
              //  dsVal = objUps.GetUPPSInfo();



            }
            foreach (DataTable table in dsVal.Tables)
            {

                if (table.TableName == "Tbl_Error")
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        track.Error = Convert.ToString(dr["ErrorMsg"]) == "" ? "" : Convert.ToString(dr["ErrorMsg"]);

                    }
                }
                if (table.TableName == "TBL_CONFIG")
                {

                    foreach (DataRow dr in table.Rows)
                    {
                        Config con = new Config();
                        con.Date = Convert.ToString(dr["DATE"]) == "" ? "" : Convert.ToString(dr["DATE"]);
                        con.Activity = Convert.ToString(dr["ACTIVITY"]) == "" ? "" : Convert.ToString(dr["ACTIVITY"]);
                        con.Location = Convert.ToString(dr["LOCATION"]) == "" ? "" : Convert.ToString(dr["LOCATION"]);
                        configinfo.Add(con);
                    }
                    track.Config = configinfo;
                }
                if (table.TableName == "Address")
                {

                    foreach (DataRow dr in table.Rows)
                    {
                        track.ShipDate = Convert.ToString(dr["SHIPDATE"]) == "" ? "" : Convert.ToString(dr["SHIPDATE"]);
                        track.ShipAddress = Convert.ToString(dr["SHIPADDRESS"]) == "" ? "" : Convert.ToString(dr["SHIPADDRESS"]);
                        track.OriginLocation = Convert.ToString(dr["ORIGINLOCATION"]) == "" ? "" : Convert.ToString(dr["ORIGINLOCATION"]);
                        track.SignatureName = Convert.ToString(dr["SIGNATURENAME"]) == "" ? "" : Convert.ToString(dr["SIGNATURENAME"]);
                        track.ActualDelAddress = Convert.ToString(dr["ACTUALDELADDRESS"]) == "" ? "" : Convert.ToString(dr["ACTUALDELADDRESS"]);
                        track.ActualDelLocation = Convert.ToString(dr["ACTUALDELLOCATION"]) == "" ? "" : Convert.ToString(dr["ACTUALDELLOCATION"]);
                        track.ActualDate = Convert.ToString(dr["ACTUALDATE"]) == "" ? "" : Convert.ToString(dr["ACTUALDATE"]);
                        track.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                    }

                }


            }

            trackinfo.Add(track);
            return trackinfo.ToList();
        }


        # endregion

        # region Dealer Representative

        public string DealerRepresentative(string userid)
        {
            var uid_en = DecryptAes(userid);
            Int32 uid = Convert.ToInt32(uid_en);

            BusinessProcess.SubmitOrderBus submitOrderBuses = new BusinessProcess.SubmitOrderBus();
            DataTable parentuser = submitOrderBuses.GetParentDetails(uid);
            return parentuser.GetJSONString();
        }

        # endregion

        # region PDF Chckout Mail

        public string MessageDiscount(int id)
        {
            string type = string.Empty;
            string term = string.Empty;
            string frtype = string.Empty;
            string result = string.Empty;
            DiscountConfigData objDConfigData = new DiscountConfigData();
            //call the method of Datalayer
            DataTable dtDConfig = objDConfigData.DiscountConfigList(id);
            if (dtDConfig.Rows.Count > 0)
            {
                Boolean tt = Convert.ToBoolean(dtDConfig.Rows[0]["DsType"]);
                Boolean fre = Convert.ToBoolean(dtDConfig.Rows[0]["Freight"]);
                string temm1 = Convert.ToString(dtDConfig.Rows[0]["Term"]);
                decimal discount = Convert.ToDecimal(dtDConfig.Rows[0]["Discount"]);
                if (tt == true)
                    type = "Surcharge";
                else
                    type = "Discount";
                if (temm1 == "")
                    term = "";
                else
                    term = "Term: " + temm1;

                if (fre == true)
                    frtype = "Prepaid";
                else
                    frtype = "Collect";

                // result = "You Are Entitled To " + discount + " % " + type + " ( " + term + " Freight:- " + frtype + " )"; 
                result = discount + " % " + type + "; " + term + "; " + " Freight: " + frtype;

            }
            return result;
        }

        public List<EmailStatus> SavePdf(string orderno, string userid, string DistributorId)
        {
            var uid_en = DecryptAes(userid);
            Int32 uid = Convert.ToInt32(uid_en);

            var did_en = DecryptAes(DistributorId);
            Int32 did = Convert.ToInt32(did_en);

            string orderno_en = DecryptAes(orderno);

            List<EmailStatus> objstatus = new List<EmailStatus>();
            string mststus = "";
            try
            {
                string totalorder = orderno_en;
                string[] a = totalorder.Split(',');
                int ordercount = a.Length;
                for (Int32 k = 0; ordercount > k; k++)
                {
                    string sorder = a[k];
                    string query = "select ID from tbl_OMst where OrderNo=" + "'" + sorder + "'";
                    Int32 orderid = Convert.ToInt32(maincls.DbCon.ExecuteScaler(query));
                    OrderListData objListDetail = new OrderListData();
                    DataTable dtdetails = objListDetail.GetOrderReports(sorder, orderid);
                    string methodname = Convert.ToString(maincls.DbCon.ExecuteScaler("select MethodName from tbl_CarrierMethod where ID=" + Convert.ToInt32(dtdetails.Rows[0]["SHIPPINGMETHODID"])));
                    int shipid = Convert.ToInt32(dtdetails.Rows[0]["SHIPADDID"]);
                    StringBuilder html1 = new StringBuilder();
                    html1.Append("<div id='div_confirmorder'><table size='2'><tbody><tr><td style='height: 8px;' colspan='5'>");
                    html1.Append("<b style='font-size: 16px;'>Thank you for ordering from Mahindra USA INC..</b></td></tr><tr><td colspan='5'>");
                    html1.Append("<b>Your Order Reference Number is &nbsp;</b> <b style='color: red; font-size: 16px;'>" + sorder + "</b>");
                    html1.Append("</td></tr><tr><td style='height: 5px;' colspan='5'></td></tr><tr><td><b>Order Placed on</b>");
                    html1.Append("</td><td align='left' colspan='4'>: &nbsp;&nbsp;&nbsp;" + Convert.ToDateTime(dtdetails.Rows[0]["ORDERDATE"]).ToString("MM/dd/yyyy") + "</td></tr><tr><td><b>Shipping Method</b>");
                    html1.Append("</td><td align='left' colspan='4'>: &nbsp;&nbsp;&nbsp;" + methodname + "</td></tr><tr><td><b>Order Type</b>");
                    html1.Append("</td><td align='left' colspan='4'>: &nbsp;&nbsp;&nbsp;" + dtdetails.Rows[0]["OTYPE"].ToString() + " -" + dtdetails.Rows[0]["OTDESC"].ToString() + "</td></tr><tr><td><b>Pay With</b>");
                    html1.Append("</td><td align='left' colspan='4'>: &nbsp;&nbsp;&nbsp;" + dtdetails.Rows[0]["PO_NO"].ToString() + "</td></tr><tr><td><b>Special Instructions</b></td>");
                    html1.Append("<td align='left' colspan='4'>: &nbsp;&nbsp;&nbsp;" + dtdetails.Rows[0]["SHIPPINGREMARKS"].ToString() + "<br></td></tr></tbody></table></div>");


                    StringBuilder dbillto11 = new StringBuilder();
                    dbillto11.Append("<table><tbody><tr><td align='left' valign='middle'><b>Please Note:</b></td></tr><tr><td align='left' class='ship-con' valign='middle'>");
                    dbillto11.Append("<span id='spnNote1'>Attention Dealer: For Fuel Additives order, please use ZOIL order type. Thank you<br><br></span></td></tr>");
                    dbillto11.Append("<tr><td align='left' valign='middle' style='height: 7px;'></td></tr><tr><td align='left' valign='middle' style='height: 7px;'></td>");
                    dbillto11.Append("</tr><tr><td align='left' valign='middle'><b>Your Order Will Be Billed To</b></td></tr><tr><td align='left' class='ship-con' valign='middle'>");
                    dbillto11.Append("<div id='div_billedTo'><table width='100%' border='0' cellspacing='4' cellpadding='4'><tbody><tr><td align='left' valign='middle' style='padding: 5px;'>");
                    dbillto11.Append("<br>815 W. Industrial Blvd<br>Cleburne,TEXAS-76033<br>Phone No.: 817-645-4325<br>E-Mail: sameer.mishra@intellinetsystem.com</td>");
                    dbillto11.Append("</tr></tbody></table></div></td></tr><tr><td align='left' valign='middle' style='height: 7px;'></td></tr><tr><td align='left' valign='middle'>");
                    dbillto11.Append("<b>Your Order Will Be Shipped To</b></td></tr><tr><td align='left' class='ship-con' valign='middle'>");
                    dbillto11.Append("<div id='div_shippedTo'><table width='100%' border='0' cellspacing='4' cellpadding='4'><tbody><tr><td align='left' valign='middle'>4C Lonestar Ranch and Outdoors - Cleburne,<br>815 W. Industrial Blvd1,orai,<br>Cleburne1, TEXAS - 80000000<br>Phone No.: 817-645-4325<br>E-Mail: sameer.mishra@intellinetsystem.com</td></tr></tbody></table></div>");
                    dbillto11.Append("</td></tr></tbody></table>");

                    StringBuilder str11 = new StringBuilder();
                    str11.Append("<table><tr><td align='left'><b>Please Note:</b></td></tr><tr><td align='left'>Attention Dealer: For Fuel Additives order, please use ZOIL order type. Thank you<br><br></td></tr></table>");

                    CommanParametrs cps1 = new CommanParametrs();
                    cps1.Add(new CommanParameter("@ID", did, DbType.Int32));
                    DataTable dtbillingAddress = maincls.DbCon.GetProcTable("PROC_DISTRIBUTORGETWITHUSER", cps1, "Distributor");

                    StringBuilder str21 = new StringBuilder();
                    str21.Append("<table><tr><td align='left'><b>Your Order Will Be Billed To</b></td></tr><tr><td align='left'>");
                    str21.Append("<table width='100%' border='0' cellspacing='4' cellpadding='4'><tbody><tr><td align='left' valign='middle' style='padding: 5px;'>");
                    str21.Append("" + dtbillingAddress.Rows[0]["NAME"].ToString() + "<br>" + dtbillingAddress.Rows[0]["ADDRESS"].ToString() + "<br>" + dtbillingAddress.Rows[0]["CITYNAME"].ToString() + "," + dtbillingAddress.Rows[0]["STATENAME"].ToString() + "-" + dtbillingAddress.Rows[0]["PINNO"].ToString() + "<br>Phone No.: " + dtbillingAddress.Rows[0]["PHONE"].ToString() + "<br>");
                    str21.Append("E-Mail: " + dtbillingAddress.Rows[0]["EMAIL"].ToString() + "</td></tr></tbody></table></td></tr></table>");

                    CommanParametrs cps = new CommanParametrs();
                    cps.Add(new CommanParameter("@ID", shipid, DbType.Int32));
                    DataTable dtshippingaddress = maincls.DbCon.GetProcTable("PROC_GETORDERSHIPPINGDETAILS", cps, "tbl_OrderShippingAddress");
                    string address2 = dtshippingaddress.Rows[0]["SADDRESS2"].ToString() == "" ? "" : dtshippingaddress.Rows[0]["SADDRESS2"].ToString() + ",";
                    string address3 = dtshippingaddress.Rows[0]["SADDRESS3"].ToString() == "" ? "" : dtshippingaddress.Rows[0]["SADDRESS3"].ToString() + ",";

                    StringBuilder str31 = new StringBuilder();
                    str31.Append("<table><tr><td align='left'><b>Your Order Will Be Shipped To:</b></td></tr><tr><td align='left'>");
                    str31.Append("<table width='100%' border='0' cellspacing='4' cellpadding='4'><tbody><tr><td align='left' valign='middle'>");
                    str31.Append("" + dtshippingaddress.Rows[0]["SHIPPINGCOMPANY"].ToString() + ",<br>" + dtshippingaddress.Rows[0]["SADDRESS1"].ToString() + "," + address2 + "" + address3 + "<br>");
                    str31.Append("" + dtshippingaddress.Rows[0]["SCITY"].ToString() + ", " + dtshippingaddress.Rows[0]["SSTATE"].ToString() + " - " + dtshippingaddress.Rows[0]["SPINCODE"].ToString() + "<br>Phone No.: " + dtshippingaddress.Rows[0]["SPHONE"].ToString() + "<br>");
                    str31.Append("E-Mail: " + dtshippingaddress.Rows[0]["SEMAIL"].ToString() + "</td></tr></tbody></table></td></tr></table>");

                    //string filename = OrderNo;
                    string compName = "Mahindra USA INC.";

                    UserProp userprop = new UserProp();
                    UserBus userbus = new UserBus();
                    //set the userrpop id's value by assinging the session variable value.
                    userprop.ID = uid;//Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                    //get the user Information based on user ID.
                    userprop = (UserProp)userbus.Get(userprop);
                    //creating object
                    SettingBus settingbus = new SettingBus();
                    SettingProp settingProp = new SettingProp();
                    //Getting object of setting business.    
                    settingProp = (SettingProp)settingbus.Get(settingProp);
                    bool mailSend = Convert.ToBoolean(ConfigurationManager.AppSettings["mailSend"]);


                    iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 8);
                    iTextSharp.text.Font headf = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);
                    Document pdfDoc = new Document(iTextSharp.text.PageSize.A4, 10f, 10f, 20f, 20f);

                    CommanParametrs cps2 = new CommanParametrs();
                    cps2.Add(new CommanParameter("@OrderId", orderid, DbType.Decimal));
                    //DataTable dtPDFS = maincls.DbCon.GetProcTable("PROC_OMDETAILLIST", cps2, "tbl_OrderDtl");

                    DataTable dtPDFS = maincls.DbCon.GetProcTable("PROC_OMDETAILLISTMOB", cps2, "tbl_OrderDtl");
                    string currency = dtPDFS.Rows[0]["CURRENCYSYMBOL"].ToString();
                    //DataTable dtPDFS = (DataTable)HttpContext.Current.Session["OrderListPDF"];
                    DataTable dtPDF = new DataTable();
                    dtPDF.Columns.Add("SNO");
                    dtPDF.Columns.Add("partno");
                    dtPDF.Columns.Add("Description");
                    dtPDF.Columns.Add("Available");
                    dtPDF.Columns.Add("qty");
                    // dtPDF.Columns.Add("price1");
                    dtPDF.Columns.Add("price");
                    //dtPDF.Columns.Add("Total");
                    dtPDF.Columns.Add("amount");

                    foreach (DataRow dr in dtPDFS.Rows)
                    {
                        dtPDF.ImportRow(dr);
                    }

                    PdfPTable table = new PdfPTable(dtPDF.Columns.Count);
                    table.TotalWidth = pdfDoc.PageSize.Width - pdfDoc.LeftMargin;

                    float[] widths = new float[] { 1.7f, 3.3f, 7.5f, 2.9f, 1.1f, 2.5f, 3f };
                    table.SetWidths(widths);
                    table.WidthPercentage = 95;
                    PdfPCell cell = new PdfPCell(new Phrase("Products"));
                    cell.Colspan = dtPDF.Columns.Count;
                    cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "SR.NO." + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);
                    table.AddCell(cell);
                    cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "ORDER PART NUMBER" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);
                    table.AddCell(cell);
                    cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "DESCRIPTION" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER); //new PdfPCell(new Phrase(10f, Resources.Resource.DESCRIPTION.ToUpper(), headf));
                    table.AddCell(cell);
                    cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "AVAILABLE" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);
                    table.AddCell(cell);
                    cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "QTY" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);
                    table.AddCell(cell);
                    if (currency != null)
                        cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "UNIT PRICE" + "(" + currency + ")" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);
                    else
                        cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "UNIT PRICE" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);

                    table.AddCell(cell);

                    if (currency != null)
                        cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "TOTAL" + "(" + currency + ")" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);

                    else
                        cell = TableCell("<table size=1  ><tr><td align='center'><b>" + "TOTAL" + "</b></td></tr></table>", 10f, PdfPCell.ALIGN_CENTER);

                    table.AddCell(cell);
                    int ind = 0;
                    decimal gt = 0;
                    string avil = string.Empty;

                    foreach (DataRow r in dtPDF.Rows)
                    {
                        if (dtPDF.Rows.Count > 0)
                        {
                            ind += 1;
                            PdfPCell ctb = TableCell("<table size=1 ><tr><td>" + ind.ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                            table.AddCell(ctb);
                            PdfPCell ctb1 = TableCell("<table size=1 ><tr><td>" + r[1].ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                            table.AddCell(ctb1);
                            PdfPCell ctb2 = TableCell("<table size=1 ><tr><td>" + r[2].ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                            table.AddCell(ctb2);
                            if (r[3].ToString() == "0")
                            { avil = " "; }
                            else
                            { avil = r[3].ToString(); }
                            PdfPCell ctb3 = TableCell("<table size=1 ><tr><td align='center'>" + avil + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                            table.AddCell(ctb3);
                            PdfPCell ctb4 = TableCell("<table size=1 ><tr><td align='right'>" + r[4].ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                            table.AddCell(ctb4);
                            decimal prices = Convert.ToDecimal(r[5]);
                            PdfPCell ctb5 = TableCell("<table size=1 ><tr><td align='right'>" + Math.Round(prices, 2).ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                            //table.AddCell(new Phrase(10f, r[5].ToString(), font5));
                            table.AddCell(ctb5);
                            int Qty = Convert.ToInt32(r[4].ToString());
                            decimal price = Convert.ToDecimal(r[5].ToString());
                            PdfPCell ctb6 = TableCell("<table size=1 ><tr><td align='right'>" + Math.Round(Qty * price, 2).ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                            //  table.AddCell(new Phrase(10f,Math.Round(Qty*price,2).ToString(), font5));
                            table.AddCell(ctb6);
                            gt += Math.Round(Qty * price, 2);
                        }
                    }




                    string htmlText = html1.ToString();
                    string dbillto = dbillto11.ToString();
                    string str1 = str11.ToString();
                    string str2 = str21.ToString();
                    string str3 = str31.ToString();


                    htmlText = htmlText.ToString().Replace("\r", "");
                    htmlText = htmlText.Replace("\n", "").Replace("  ", "");
                    string htmlText1 = "<table><tr><td></td><td><h3><b>" + "Order Confirmation" + "</b></h3></td><td></td></tr></table>";
                    string OrderHeading = "<table size='1'><tr><td><h3><b>" + "Ordered Items" + "</b></h3></td><td></td><td></td></tr></table>";


                    iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();

                    List<IElement> htmlarraylist1 = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(htmlText1), styles);
                    List<IElement> htmlarraylist = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(htmlText), styles);
                    List<IElement> dbillto1 = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(dbillto), styles);
                    List<IElement> OrderHeading1 = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(OrderHeading), styles);

                    //add the collection to the document  

                    Paragraph mypara = new Paragraph();//make an emtphy paragraph as "holder"  
                    mypara.IndentationLeft = 10;
                    mypara.InsertRange(0, htmlarraylist1);
                    mypara.InsertRange(1, htmlarraylist);
                    mypara.InsertRange(2, OrderHeading1);
                    Paragraph mypara1 = new Paragraph();
                    mypara1.IndentationLeft = 10;
                    mypara1.InsertRange(0, dbillto1);

                    string path = HttpContext.Current.Server.MapPath(@"~/Order_files");
                    PdfWriter docWriter = PdfWriter.GetInstance(pdfDoc, new FileStream(path + "/" + sorder + ".pdf", FileMode.Create));
                    PdfWriterEvents writerEvent = new PdfWriterEvents(compName, 300, 400, 55);
                    docWriter.PageEvent = writerEvent;


                    PdfPTable table3 = new PdfPTable(1);


                    PdfPCell c3 = TableCell(str1, 10f, PdfPCell.ALIGN_TOP);
                    float[] widths3 = new float[] { 22f };
                    table3.SetWidths(widths3);
                    table3.WidthPercentage = 100;
                    table3.KeepTogether = true;
                    c3.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    c3.Border = PdfPCell.NO_BORDER;
                    table3.AddCell(c3);



                    PdfPTable table4 = new PdfPTable(1);
                    PdfPCell c4 = TableCell(str2, 10f, PdfPCell.ALIGN_TOP);
                    float[] widths2 = new float[] { 22f };
                    table4.SetWidths(widths2);
                    table4.WidthPercentage = 100;
                    table4.KeepTogether = true;
                    c4.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    c4.Border = PdfPCell.NO_BORDER;
                    table4.AddCell(c4);



                    PdfPTable table5 = new PdfPTable(1);
                    PdfPCell c5 = TableCell(str3, 10f, PdfPCell.ALIGN_TOP);
                    float[] widths5 = new float[] { 22f };
                    table5.SetWidths(widths5);
                    table5.WidthPercentage = 100;
                    table5.KeepTogether = true;
                    c5.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    c5.Border = PdfPCell.NO_BORDER;
                    table5.AddCell(c5);

                    int discountid = Convert.ToInt32(dtdetails.Rows[0]["DISCOUNTID"]);
                    string dismessage = string.Empty;
                    if (discountid > 0)
                    {
                        dismessage = MessageDiscount(discountid);

                    }



                    PdfPTable table2 = new PdfPTable(2);
                    table2.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_RIGHT;
                    PdfPCell c1 = TableCell("<table ><tr><td align='right'>" + "Grand Total" + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);// new PdfPCell(new Phrase());
                    PdfPCell c2 = TableCell("<table ><tr><td align='right'>" + gt.ToString() + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);

                    float[] widths1 = new float[] { 19f, 3f };
                    table2.SetWidths(widths1);
                    table2.WidthPercentage = 95;
                    table2.AddCell(c1);
                    table2.AddCell(c2);

                    //Discount & Percentage Row
                    PdfPTable table6 = new PdfPTable(2);
                    table6.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_RIGHT;
                    PdfPCell dis = new PdfPCell();
                    PdfPCell distotal = new PdfPCell();
                    decimal discount = 0, discountper = 0;
                    string dstype = dtdetails.Rows[0]["DSTYPE"] == DBNull.Value ? Convert.ToString("") : Convert.ToString(dtdetails.Rows[0]["DSTYPE"]);
                    if (dstype != "")
                    {
                        discount = Math.Round(Convert.ToDecimal(dtdetails.Rows[0]["DISCOUNT"]), 2);
                        discountper = Convert.ToDecimal(dtdetails.Rows[0]["DISPERCENTAGE"]);
                    }
                    if (dstype == "True")
                    {
                        dis = TableCell("<table ><tr><td align='right'>" + "Surcharge " + discountper + " %" + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                        distotal = TableCell("<table ><tr><td align='right'>" + discount + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);

                    }
                    else if (dstype == "False")
                    {
                        string disc = Convert.ToString(discountper).Replace("-", "");
                        dis = TableCell("<table ><tr><td align='right'>" + "Discount " + disc + " %" + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                        distotal = TableCell("<table ><tr><td align='right'>" + discount + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);

                    }
                    else
                    { }

                    //Net Total
                    PdfPTable table7 = new PdfPTable(2);
                    table6.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_RIGHT;
                    PdfPCell net1 = new PdfPCell();
                    PdfPCell nettotal1 = new PdfPCell();
                    decimal netval = Math.Round(Convert.ToDecimal(dtdetails.Rows[0]["NETVALUE"]), 2);
                    if (dstype == "True" || dstype == "False")
                    {
                        net1 = TableCell("<table ><tr><td align='right'>" + "Net Total" + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);
                        nettotal1 = TableCell("<table ><tr><td align='right'>" + netval + "</td></tr></table>", 10f, PdfPCell.ALIGN_RIGHT);

                    }


                    if (dstype == "True" || dstype == "False")
                    {
                        float[] widths20 = new float[] { 19f, 3f };
                        table6.SetWidths(widths20);
                        table6.WidthPercentage = 95;
                        table6.AddCell(dis);
                        table6.AddCell(distotal);

                        float[] widths21 = new float[] { 19f, 3f };
                        table7.SetWidths(widths21);
                        table7.WidthPercentage = 95;
                        table7.AddCell(net1);
                        table7.AddCell(nettotal1);
                    }


                    //Discount/Surcharge Message
                    PdfPTable table8 = new PdfPTable(2);
                    // table8.DefaultCell.VerticalAlignment = PdfPCell.ALIGN_RIGHT;
                    //PdfPCell disimg = new PdfPCell();
                    //PdfPCell dismsg = new PdfPCell();
                    table8.WidthPercentage = 100;
                    table8.SetWidths(new float[] { 5f, 95f });
                    PdfPCell icell = new PdfPCell();

                    //if (dstype == "True")
                    //{
                    //    icell = ImageCell("~/Images/angry.png", 50f, PdfPCell.ALIGN_RIGHT);

                    //    //disimg = TableCell("<table ><tr><td align='left'><img src='~/Images/angry.png' /></td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                    //    //dismsg = TableCell("<table ><tr><td align='left'>" + dismessage + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);

                    //}
                    //else if (dstype == "False")
                    //{
                    //    icell = ImageCell("~/Images/smile.png", 50f, PdfPCell.ALIGN_RIGHT);

                    //    //disimg = TableCell("<table ><tr><td align='left'><img src='~/Images/smile.png' /></td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                    //    //dismsg = TableCell("<table ><tr><td align='left'>" + dismessage + "</td></tr></table>", 10f, PdfPCell.ALIGN_LEFT);
                    //}
                    // else
                    //{}



                    if (dstype == "True" || dstype == "False")
                    {
                        //float[] widths23 = new float[] { 19f, 3f };
                        // table8.SetWidths(widths23);
                        // table8.WidthPercentage = 100;
                        // table8.AddCell(dis);
                        //table8.AddCell(distotal);
                        icell.Border = PdfPCell.NO_BORDER;
                        table8.AddCell(icell);
                        PdfPCell c6 = TableCell(dismessage, 10f, PdfPCell.ALIGN_MIDDLE);

                        c6.VerticalAlignment = PdfPCell.ALIGN_LEFT;
                        c6.Border = PdfPCell.NO_BORDER;
                        table8.AddCell(c6);


                    }


                    pdfDoc.Open();
                    pdfDoc.Add(mypara);
                    pdfDoc.Add(table);
                    pdfDoc.Add(table2);
                    if (dstype == "True" || dstype == "False")
                    {
                        pdfDoc.Add(table6);
                        pdfDoc.Add(table7);
                        pdfDoc.Add(table8);
                    }
                    pdfDoc.Add(table3);
                    pdfDoc.Add(table4);
                    pdfDoc.Add(table5);

                    pdfDoc.Close();

                    if (mailSend)
                    {
                        MailClass objMail = new MailClass();
                        DataTable dtUser = userbus.FindUserFromMailID(userprop);
                        MailingBus mbus = new MailingBus();
                        MailingProp mpro = new MailingProp();
                        //User Creation transaction
                        mpro.TransactionType = Convert.ToInt32(ConfigurationManager.AppSettings["OC"]);

                        mpro = (MailingProp)mbus.GetTranMailing(mpro);
                        if (mpro.MailTemplateID > 0)
                        {
                            MailTemplateBus mtbus = new MailTemplateBus();
                            MailTemplateProp mtprop = new MailTemplateProp();
                            mtprop.ID = mpro.MailTemplateID;
                            mtprop = (MailTemplateProp)mtbus.Get(mtprop);
                            mtprop.Subject = mtprop.Subject + "#" + sorder;
                            string strtemp = HttpContext.Current.Server.HtmlDecode(mtprop.Template);
                            if (dtUser.Rows.Count > 0)
                            {
                                foreach (DataColumn dc in dtUser.Columns)
                                {
                                    if (dc.Caption.ToLower() == "password")
                                    {
                                        strtemp = strtemp.Replace("{" + dc.Caption.ToLower() + "}", Global.Decrypt(dtUser.Rows[0][dc].ToString(), "ABF482", true));
                                    }
                                    else
                                        strtemp = strtemp.Replace("{" + dc.Caption.ToLower() + "}", dtUser.Rows[0][dc].ToString());
                                }
                                mtprop.Template = strtemp;

                                string strdir = HttpContext.Current.Server.MapPath(@"~/Order_files/");
                                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(strdir + sorder + ".pdf");
                                try
                                {


                                    mststus = objMail.SendOrderMails(mtprop, userprop.Email, settingProp.SpareEmailID, attach);
                                    if (mststus == "1")
                                    {
                                        mststus = "Email Sent Successfully";
                                    }
                                    if (mststus == "0" || mststus == "")
                                    {
                                        mststus = "Mail Sending Failed.";
                                    }
                                    else
                                    {
                                        string[] ids = mststus.Split(',');

                                        for (int i = 0; i < ids.Length - 1; i++)
                                        {
                                            mststus += "Mail Sending Failed." + ":" + ids[i] + "\n";
                                        }
                                    }




                                }


                                catch (Exception ex)
                                {
                                    ex.Data.Clear();
                                    attach.Dispose();
                                    if (File.Exists(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf"))
                                    {
                                        File.Delete(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf");

                                    }

                                }
                                finally
                                {
                                    attach.Dispose();
                                    if (File.Exists(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf"))
                                    {
                                        File.Delete(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf");

                                    }
                                }
                            }
                        }
                    }

                    if (File.Exists(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf"))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(@"~/Order_files/") + sorder + ".pdf");

                    }
                    EmailStatus em = new EmailStatus();
                    em.Status = mststus;
                    objstatus.Add(em);
                }
                return objstatus.ToList();
            }
            catch (Exception ex)
            {
                EmailStatus em = new EmailStatus();
                em.Status = mststus;
                objstatus.Add(em);
                return objstatus.ToList();
            }
        }

        private static PdfPCell ImageCell(string path, float scale, int align)
        {
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath(path));
            image.ScalePercent(scale);
            image.CompressionLevel = 1;

            PdfPCell cell = new PdfPCell(image);
            // cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            cell.HorizontalAlignment = align;
            cell.PaddingLeft = 2f;
            cell.PaddingBottom = 2f;
            cell.PaddingRight = 2f;
            cell.PaddingTop = 2f;
            return cell;
        }

        private static PdfPCell TableCell(string tbl, float scale, int align)
        {
            System.Collections.Generic.List<IElement> htmlarraylist = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(new StringReader(tbl), null);
            PdfPCell cell = new PdfPCell();
            for (int k = 0; k < htmlarraylist.Count; k++)
            {
                cell.AddElement(htmlarraylist[k]);

            }
            cell.VerticalAlignment = align;
            cell.HorizontalAlignment = PdfPCell.ALIGN_TOP;
            cell.PaddingLeft = 0f;
            cell.PaddingBottom = 0f;
            cell.PaddingRight = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }

        class PdfWriterEvents : IPdfPageEvent
        {
            string watermarkText = string.Empty;
            float fontSize = 55;
            float xPosition = 300;
            float yPosition = 400;
            public PdfWriterEvents(string watermark, float xpos, float ypos, float fts)
            {
                watermarkText = watermark;
                xPosition = xpos;
                yPosition = ypos;
                fontSize = fts;
            }

            public void OnOpenDocument(PdfWriter writer, Document document) { }
            public void OnCloseDocument(PdfWriter writer, Document document) { }
            public void OnStartPage(PdfWriter writer, Document document)
            {
                iTextSharp.text.Rectangle rect = writer.PageSize;
                float angle = 45f;
                try
                {
                    PdfContentByte under = writer.DirectContentUnder;
                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                    under.BeginText();
                    under.SetColorFill(BaseColor.LIGHT_GRAY);
                    under.SetFontAndSize(baseFont, fontSize);
                    under.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarkText, xPosition, yPosition, angle);
                    under.EndText();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }
            public void OnEndPage(PdfWriter writer, Document document) { }
            public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition) { }
            public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition) { }
            public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title) { }
            public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition) { }
            public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title) { }
            public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition) { }
            public void OnGenericTag(PdfWriter writer, Document document, iTextSharp.text.Rectangle rect, String text) { }

        }

        # endregion

        # region Discount

        //public List<DiscountInfo> getDiscountConfig(int OrderTypeID, decimal OPrice, Int32 userid, Int32 cid, string currency)
        //{
        //    Int32 icnt = 0;
        //    List<DiscountInfo> objDiscount = new List<DiscountInfo>();

        //    string result = string.Empty;
        //    List<DiscountConfigProp> lsdconfig = new List<DiscountConfigProp>();

        //    int delId = userid;
        //    int countryId = cid;

        //    //object creation
        //    DiscountConfigData objDConfigData = new DiscountConfigData();
        //    DiscountConfigProp oDConfigProp = new DiscountConfigProp();
        //    oDConfigProp.OrderTypeId = OrderTypeID;

        //    //call the method of Datalayer
        //    DataTable dtDConfig = objDConfigData.DiscountConfigList(delId, OrderTypeID, OPrice, countryId);

        //    //object creation
        //    foreach (DataRow dr in dtDConfig.Rows)
        //    {
        //        DiscountInfo disinfo = new DiscountInfo();
        //        oDConfigProp = new DiscountConfigProp();
        //        oDConfigProp.ID = Convert.ToInt32(dr["Id"]);
        //        oDConfigProp.OrderTypeId = Convert.ToInt32(dr["OrderTypeId"]);
        //        oDConfigProp.DSType = Convert.ToBoolean(dr["DSType"]);
        //        oDConfigProp.FPrice = Convert.ToDecimal(dr["FPrice"]);
        //        oDConfigProp.TPrice = Convert.ToDecimal(dr["TPrice"]);
        //        oDConfigProp.Discount = Convert.ToDecimal(dr["Discount"]);
        //        oDConfigProp.Term = Convert.ToString(dr["Term"]);
        //        oDConfigProp.Freight = Convert.ToBoolean(dr["Freight"]);
        //        oDConfigProp.Inactive = Convert.ToBoolean(dr["Inactive"]);
        //        oDConfigProp.StartDate = Convert.ToDateTime(dr["StartDate"]);
        //        oDConfigProp.EndDate = Convert.ToDateTime(dr["EndDate"]);
        //        //lsdconfig.Add(oDConfigProp);

        //        Int32 oDisId = 0;
        //        decimal oDiscount = 0;
        //        string oDsType = string.Empty;
        //        string dsType = "Discount";
        //        string frType = "Collect";
        //        if (oDConfigProp.Freight == true)
        //            frType = "Prepaid";
        //        if (oDConfigProp.DSType == true)
        //            dsType = "Surcharge";

        //        decimal fP = oDConfigProp.FPrice;
        //        decimal tP = oDConfigProp.TPrice;
        //        decimal orm = OPrice;
        //        StringBuilder htmldiscount = new StringBuilder();
        //        if (icnt == 0 & oDConfigProp.DSType == false)
        //        {
        //            oDisId = oDConfigProp.ID;
        //            oDiscount = oDConfigProp.Discount;
        //            oDsType = dsType;
        //            if (orm >= fP && tP <= orm)
        //            {
        //                // htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                // htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />");
        //                htmldiscount.Append("Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "Term:-" + oDConfigProp.Term) + " Freight:-" + frType + "");
        //                // htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                if (dsType == "Discount")
        //                {
        //                    disinfo.Percentage = oDConfigProp.Discount;
        //                }
        //                else
        //                {
        //                    disinfo.Surcharge = oDConfigProp.Discount;
        //                }
        //            }
        //            else if (orm >= fP && orm <= tP)
        //            {
        //                //htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                // htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />");
        //                htmldiscount.Append("Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "Term:-" + oDConfigProp.Term) + " Freight:-" + frType + ")");
        //                // htmldiscount.Append("<div><img src='Images/smile.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                if (dsType == "Discount")
        //                {
        //                    disinfo.Percentage = oDConfigProp.Discount;
        //                }
        //                else
        //                {
        //                    disinfo.Surcharge = oDConfigProp.Discount;
        //                }
        //            }
        //            else
        //            {
        //                htmldiscount.Append("Shop for " + currency + " " + Convert.ToDecimal(oDConfigProp.FPrice - OPrice).ToString("F") + " more & upgrade to " + oDConfigProp.Discount + " % " + dsType + "");
        //                if (dsType == "Discount")
        //                {
        //                    disinfo.Percentage = oDConfigProp.Discount;
        //                }
        //                else
        //                {
        //                    disinfo.Surcharge = oDConfigProp.Discount;
        //                }
        //            }

        //        }
        //        else if (icnt == 0 & oDConfigProp.DSType == true)
        //        {
        //            oDisId = oDConfigProp.ID;
        //            oDiscount = oDConfigProp.Discount;
        //            oDsType = dsType;
        //            if (orm >= fP && tP <= orm)
        //            {
        //                //htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                //htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />");
        //                htmldiscount.Append("Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "Term:-" + oDConfigProp.Term) + " Freight:-" + frType + ")");
        //                // htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                if (dsType == "Discount")
        //                {
        //                    disinfo.Percentage = oDConfigProp.Discount;
        //                }
        //                else
        //                {
        //                    disinfo.Surcharge = oDConfigProp.Discount;
        //                }
        //            }
        //            else if (orm >= fP && orm <= tP)
        //            {
        //                //htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + "") + " <b>Freight:-</b>" + frType + ")</div>");
        //                //htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />");
        //                htmldiscount.Append("Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "Term:-" + oDConfigProp.Term) + " Freight:-" + frType + ")");
        //                // htmldiscount.Append("<div><img src='Images/angry.png' alt='' style='margin-bottom:-4px;' />&nbsp;Your are entitled to " + oDConfigProp.Discount + " % " + dsType + " (" + (oDConfigProp.Term == "" ? "" : "<b>Term:-</b>" + oDConfigProp.Term) + " <b>Freight:-</b>" + frType + ")</div>");
        //                if (dsType == "Discount")
        //                {
        //                    disinfo.Percentage = oDConfigProp.Discount;
        //                }
        //                else
        //                {
        //                    disinfo.Surcharge = oDConfigProp.Discount;
        //                }
        //            }
        //        }
        //        else if (icnt == 1 & oDConfigProp.DSType == false)
        //        {
        //            htmldiscount.Append("Shop for " + currency + " " + Convert.ToDecimal(oDConfigProp.FPrice - OPrice).ToString("F") + " more & upgrade to " + oDConfigProp.Discount + " % " + dsType + "");
        //            if (dsType == "Discount")
        //            {
        //                disinfo.Percentage = oDConfigProp.Discount;
        //            }
        //            else
        //            {
        //                disinfo.Surcharge = oDConfigProp.Discount;
        //            }
        //        }
        //        icnt++;
        //        disinfo.Message = htmldiscount.ToString();
        //        objDiscount.Add(disinfo);

        //    }
        //    if (dtDConfig.Rows.Count == 0)
        //    {
        //        DiscountInfo disinfo = new DiscountInfo();
        //        disinfo.Message = "";
        //        objDiscount.Add(disinfo);
        //    }

        //    return objDiscount.ToList();
        //}


        public List<DiscountConfig> DiscountConfig(Int32 ID)
        {
            string type = string.Empty;
            string term = string.Empty;
            string frtype = string.Empty;
            List<DiscountConfig> objDiscountConfig = new List<DiscountConfig>();

            List<DiscountConfigProp> lsdconfig = new List<DiscountConfigProp>();

            //object creation
            DiscountConfigData objDConfigData = new DiscountConfigData();
            DiscountConfigProp oDConfigProp = new DiscountConfigProp();

            //call the method of Datalayer
            DataTable dtDConfig = objDConfigData.DiscountConfigList(ID);

            if (dtDConfig.Rows.Count > 0)
            {
                foreach (DataRow dr in dtDConfig.Rows)
                {
                    DiscountConfig disconfig = new DiscountConfig();
                    disconfig.ID = Convert.ToInt32(dr["Id"]);
                    disconfig.OrderTypeId = Convert.ToInt32(dr["OrderTypeId"]);
                    disconfig.DsType = Convert.ToBoolean(dr["DSType"]);
                    disconfig.FPrice = Convert.ToDecimal(dr["FPrice"]);
                    disconfig.TPrice = Convert.ToDecimal(dr["TPrice"]);
                    disconfig.Discount = Convert.ToDecimal(dr["Discount"]) == 0 ? 0 : Convert.ToDecimal(dr["Discount"]);
                    disconfig.Term = Convert.ToString(dr["Term"]);
                    disconfig.Freight = Convert.ToBoolean(dr["Freight"]);
                    disconfig.Inactive = Convert.ToBoolean(dr["Inactive"]);
                    disconfig.StartDate = Convert.ToString(dr["StartDate"]);
                    disconfig.EndDate = Convert.ToString(dr["EndDate"]);
                    disconfig.SubOrderType = Convert.ToString(dr["SubOrderType"]);
                    if (disconfig.DsType == true)
                        type = "Surcharge";
                    else
                        type = "Discount";
                    if (disconfig.Term == "")
                        term = "";
                    else
                        term = "Term: " + disconfig.Term;

                    if (disconfig.Freight == true)
                        frtype = "Prepaid";
                    else
                        frtype = "Collect";

                    // disconfig.Message = "You Are Entitled To " + disconfig.Discount + " % " + type + " ( " + term + " Freight:- " + frtype + " )";
                    disconfig.Message = disconfig.Discount + " % " + type + "; " + term + "; " + " Freight: " + frtype;

                    objDiscountConfig.Add(disconfig);
                }
            }

            return objDiscountConfig.ToList();
        }

        public List<DiscountInfo> GetDiscount(Int32 OrderTypeID, string OPrice, string userid, string cid)
        {
            var price_en = DecryptAes(OPrice);
            decimal prce = Convert.ToDecimal(price_en);

            var uid_en = DecryptAes(userid);
            Int32 uid = Convert.ToInt32(uid_en);

            var cid_en = DecryptAes(cid);
            Int32 cids = Convert.ToInt32(cid_en);

            List<DiscountInfo> objDiscount = new List<DiscountInfo>();
            DiscountConfigData objDConfigData = new DiscountConfigData();
            DataTable dt = getDiscountList(uid, OrderTypeID, prce, cids); //jDConfigData.getDiscountList(userid, OrderTypeID, OPrice, cid);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DiscountInfo dinfo = new DiscountInfo();
                    dinfo.ID = Convert.ToInt32(dr["ID"]);
                    dinfo.Message = Convert.ToString(dr["CDISCOUNT"]) == "" ? "" : Convert.ToString(dr["CDISCOUNT"]);
                    dinfo.DsType = Convert.ToString(dr["DSTYPE"]) == "" ? "" : Convert.ToString(dr["DSTYPE"]);
                    dinfo.Priority = Convert.ToInt32(dr["ISDEFAULT"]);
                    objDiscount.Add(dinfo);
                }
            }

            return objDiscount.ToList();
        }


        public DataTable getDiscountList(int DealerId, int OTypeId, decimal OPrice, int cntryId)
        {
            CommanParametrs cps = new CommanParametrs();
            CommanParameter cp;
            cp = new CommanParameter("@DealerID", DealerId, DbType.Int32);
            cps.Add(cp);
            cp = new CommanParameter("@OTYPEID", OTypeId, DbType.Int32);
            cps.Add(cp);
            cp = new CommanParameter("@OPRICE", OPrice, DbType.Decimal);
            cps.Add(cp);
            cp = new CommanParameter("@CountryId", cntryId, DbType.Int32);
            cps.Add(cp);
            return maincls.DbCon.GetProcTable("PROC_GETDISCOUNTLISTMOB", cps, "TBL_DISCONFIG");
        }

        #endregion

        # region Mobile Maintenance Alert Message

        public List<AlertInfo> AlertMessage()
        {
            List<AlertInfo> ainfo = new List<AlertInfo>();
            string sql = @"select * from TBL_MobMaintenance where AppType in (1,2,3)";
            DataTable dt = maincls.DbCon.GetTable(sql, "TBL_MobMaintenance");
            AlertInfo ai = new AlertInfo();
            if (dt.Rows.Count > 0)
            {

                ai.Message = Convert.ToString(dt.Rows[0]["Message"]) == "" ? "" : Convert.ToString(dt.Rows[0]["Message"]);
                ai.Active = Convert.ToInt32(dt.Rows[0]["Status"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["Status"]);
                ainfo.Add(ai);
            }
            else
            {
                ai.Message = "";
                ai.Active = 0;
                ainfo.Add(ai);
            }
            return ainfo.ToList();
        }
        # endregion

        # region Model search

        public List<ModelSearchInfo> GetModelSearchData(int SerchType, string SearchStr, string UserID, string UserType, string CountryID, int LngId)
        {
            var usertype_en = DecryptAes(UserType);
            Int32 usertypes = Convert.ToInt32(usertype_en);

            var uid_en = DecryptAes(UserID);
            Int32 uid = Convert.ToInt32(uid_en);

            var cid_en = DecryptAes(CountryID);
            Int32 cids = Convert.ToInt32(cid_en);

            List<ModelSearchInfo> objModelSearch = new List<ModelSearchInfo>();
            try
            {
                CategoryViewData ObjCategoryViewData = new CategoryViewData();
                DataTable modeldata = new DataTable();
                modeldata = ObjCategoryViewData.GetModelSearchData(uid, usertypes, cids, LngId, SerchType, SearchStr);
                if (modeldata.Rows.Count > 0)
                {
                    foreach (DataRow dr in modeldata.Rows)
                    {
                        ModelSearchInfo msinfo = new ModelSearchInfo();
                        msinfo.ModelId = Convert.ToInt32(dr["MODELID"]);
                        msinfo.VehicleId = Convert.ToInt32(dr["VEHICLEID"]);
                        msinfo.TypeId = Convert.ToInt32(dr["TYPEID"]) == 0 ? 0 : Convert.ToInt32(dr["TYPEID"]);
                        msinfo.ModelName = Convert.ToString(dr["MODELNAME"]) == "" ? "" : Convert.ToString(dr["MODELNAME"]);
                        msinfo.Series = Convert.ToString(dr["SERIES"]) == "" ? "" : Convert.ToString(dr["SERIES"]);
                        msinfo.TypeName = Convert.ToString(dr["TYPENAME"]) == "" ? "" : Convert.ToString(dr["TYPENAME"]);
                        objModelSearch.Add(msinfo);
                    }
                }
            }
            catch (Exception ex)
            {
                return objModelSearch.ToList();
            }

            return objModelSearch.ToList();
        }

        public List<AggregateInfo> GetAggrigateData(decimal ModelId, int LngId)
        {
            List<AggregateInfo> objAggregate = new List<AggregateInfo>();
            CategoryViewData ObjCategoryViewData = new CategoryViewData();
            DataTable dtAggregate = ObjCategoryViewData.GetModelSearchAggrigate(ModelId, LngId);
            if (dtAggregate.Rows.Count > 0)
            {
                foreach (DataRow dr in dtAggregate.Rows)
                {
                    AggregateInfo aginfo = new AggregateInfo();
                    aginfo.ID = Convert.ToInt32(dr["ID"]);
                    aginfo.CategoryId = Convert.ToInt32(dr["CATEGORYID"]);
                    aginfo.CategoryName = Convert.ToString(dr["CATEGORYNAME"]) == "" ? "" : Convert.ToString(dr["CATEGORYNAME"]);
                    objAggregate.Add(aginfo);
                }
            }

            return objAggregate.ToList();
        }

        #endregion

        # region Price Check

        public List<PartPriceInfo> PartPrice(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0, string userid = "0")
        {
            string pnos = DecryptAes(PartNo);

            var uid_en = DecryptAes(userid);
            Int32 uid = Convert.ToInt32(uid_en);

            var cid_en = DecryptAes(CountryID);
            Int32 cids = Convert.ToInt32(cid_en);

            var qty_en = DecryptAes(Qty);
            Int32 qtys = Convert.ToInt32(qty_en);

            string agroupid = string.Empty;
            string PartCategoryID = string.Empty;
            string pno = pnos.ToUpper();
            List<PartPriceInfo> objAddPart = new List<PartPriceInfo>();
            CommanParametrs cps = new CommanParametrs();
            cps.Add(new CommanParameter("@PartNo", pno, DbType.String));
            cps.Add(new CommanParameter("@Qty", qtys, DbType.Int32));
            cps.Add(new CommanParameter("@CountryID", cids, DbType.Int32));
            DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ORDERLISTADDPART", cps, "AddPart");
            if (dtPart.Rows.Count > 0)
            {
                string slq = @"select agroupid from tbl_part where partno=" + "'" + pnos + "'";
                DataTable dt = maincls.DbCon.GetTable(slq, "tbl_part");
                if (dt.Rows.Count > 0)
                {
                    agroupid = dt.Rows[0]["AGROUPID"].ToString();
                }
                else { agroupid = "0"; }
            }
            dtPart = AddColumn(dtPart, uid, cids);
            PartPriceInfo addpart = new PartPriceInfo();
            try
            {
                bool blStatus = false;
                if (dtPart != null)
                {
                    //If Part Is Not in Database then Pop Up Message
                    if (dtPart.Rows.Count <= 0)
                    {
                        //Show the message.

                        //return Resources.Resource.ThePartNumberYouHaveSpecifiedIsNotAValidPartNoAlertMessage;

                        addpart.Message = "Not Vaild";
                        addpart.ID = 0;
                        addpart.Description = "";
                        addpart.Partno = "";
                        addpart.Available = "";
                        addpart.NS = 0;
                        addpart.Qty = 0;
                        addpart.AGroupId = "AGroupId";
                        objAddPart.Add(addpart);
                        return objAddPart.ToList();
                    }
                    else
                    {
                        try
                        {
                            if (otype == 0)
                            {
                                PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                if (PartCategoryID == "1")
                                {
                                    addpart.Message = "accessory parts";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;

                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                                //  return Resources.Resource.PleaseOrderTheAccessoryPartsFromtheAccessoryPartSection;
                            }
                            else
                            {
                                PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                if (PartCategoryID == "2")
                                {
                                    addpart.Message = "spare parts";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;

                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                            }

                            //Checking that Part is Serviceable or Non Serviceable
                            if (Convert.ToInt32(dtPart.Rows[0]["NonServiceable"]) == 1 || Convert.ToBoolean(dtPart.Rows[0]["NonServiceable"]) == true)
                            {
                                //Show the message.
                                // return Resources.Resource.ThePartNumberYouHaveSpecifiedIsANonServiceablePartAlertMessage;
                                addpart.Message = "NonServiceable";
                                addpart.ID = 0;
                                addpart.Description = "";
                                addpart.Partno = "";
                                addpart.Available = "";
                                addpart.NS = 0;
                                addpart.Qty = 0;

                                addpart.AGroupId = "AGroupId";
                                objAddPart.Add(addpart);
                                return objAddPart.ToList();
                            }
                        }
                        catch { }

                    }


                    if (blStatus == true)
                    {
                        //Show the message
                        //return Resources.Resource.PartQuantitySuccessfullyUpdatedInOrderListAlertMessage;
                        addpart.Message = "Update";
                        addpart.ID = 0;
                        addpart.Description = "";
                        addpart.Partno = "";
                        addpart.Available = "";
                        addpart.NS = 0;
                        addpart.Qty = 0;

                        addpart.AGroupId = "AGroupId";
                        objAddPart.Add(addpart);
                        return objAddPart.ToList();
                    }
                    else
                    {
                        //Show the message
                        //return Resources.Resource.PartSuccessfullyAddedToOrderListAlertMessage;
                        foreach (DataRow dr in dtPart.Rows)
                        {
                            addpart.Message = "Success";
                            addpart.ID = Convert.ToInt32(dr["ID"]);
                            addpart.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                            addpart.Partno = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                            addpart.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                            addpart.NS = Convert.ToInt32(dr["NONSERVICEABLE"]) == 0 ? 0 : Convert.ToInt32(dr["NONSERVICEABLE"]);
                            addpart.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                            addpart.Dist_Cost = Convert.ToString(dr["PRICE1"]) == "0" ? "0" : Convert.ToString(dr["PRICE1"]);
                            addpart.Dist_Msrp = Convert.ToString(dr["PRICE2"]) == "0" ? "0" : Convert.ToString(dr["PRICE2"]);
                            addpart.Cost = Convert.ToString(dr["PRICE3"]) == "0" ? "0" : Convert.ToString(dr["PRICE3"]);
                            addpart.Msrp = Convert.ToString(dr["PRICE4"]) == "0" ? "0" : Convert.ToString(dr["PRICE4"]);
                            addpart.AGroupId = agroupid == "" ? "0" : agroupid;
                            objAddPart.Add(addpart);
                        }
                        return objAddPart.ToList();
                    }
                }
                else
                {
                    //Show the message
                    // return Resources.Resource.PleaseEnterAValidPartNoAlertMessage;
                    addpart.Message = "Valid";
                    addpart.ID = 0;
                    addpart.Description = "";
                    addpart.Partno = "";
                    addpart.Available = "";
                    addpart.NS = 0;
                    addpart.Qty = 0;

                    addpart.AGroupId = "AGroupId";
                    objAddPart.Add(addpart);
                    return objAddPart.ToList();
                }

            }
            catch
            {
                addpart.Message = "Invalid";
                addpart.ID = 0;
                addpart.Description = "";
                addpart.Partno = "";
                addpart.Available = "";
                addpart.NS = 0;
                addpart.Qty = 0;

                addpart.AGroupId = "AGroupId";
                objAddPart.Add(addpart);
                return objAddPart.ToList();
            }


        }

        public List<PartPriceInfo> PartPriceAll(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0, string userid = "0")
        {
            string pnos = DecryptAes(PartNo);

            var uid_en = DecryptAes(userid);
            Int32 uid = Convert.ToInt32(uid_en);

            var cid_en = DecryptAes(CountryID);
            Int32 cids = Convert.ToInt32(cid_en);

            var qty_en = DecryptAes(Qty);
            Int32 qtys = Convert.ToInt32(qty_en);

            List<PartPriceInfo> objAddPart = new List<PartPriceInfo>();
            try
            {
                string countryname = string.Empty;
                string currency = string.Empty;
                string agroupid = string.Empty;
                string PartCategoryID = string.Empty;
                string pno = pnos.ToUpper();

                CommanParametrs cps = new CommanParametrs();
                cps.Add(new CommanParameter("@PartNo", pno, DbType.String));
                cps.Add(new CommanParameter("@Qty", qtys, DbType.Int32));
                cps.Add(new CommanParameter("@CountryID", cids, DbType.Int32));
                DataTable dtPart = maincls.DbCon.GetProcTable("PROC_ORDERLISTADDPART", cps, "AddPart");
                if (dtPart.Rows.Count > 0)
                {
                    string slq = @"select agroupid from tbl_part where partno=" + "'" + pnos + "'";

                    DataTable dt = maincls.DbCon.GetTable(slq, "tbl_part");
                    if (dt.Rows.Count > 0)
                    {
                        agroupid = Convert.ToString(dt.Rows[0]["AGROUPID"]);
                    }
                    else { agroupid = "0"; }
                }
                else
                { agroupid = "0"; }
                string getCountry = @"select c.ID,c.CountryName,cu.CurrencySymbol from tbl_country c left join tbl_currency cu on cu.id=c.currencyid inner join tbl_countrypart cp on cp.countryid=c.id where cp.partid=" + dtPart.Rows[0]["ID"];
                DataTable dtCountry = maincls.DbCon.GetTable(getCountry, "tbl_country");
                if (dtCountry.Rows.Count > 0)
                {

                    for (Int32 j = 0; j < dtCountry.Rows.Count; j++)
                    {
                        cids = Convert.ToInt32(dtCountry.Rows[j]["ID"]);
                        countryname = Convert.ToString(dtCountry.Rows[j]["COUNTRYNAME"]).Trim();
                        currency = Convert.ToString(dtCountry.Rows[j]["CURRENCYSYMBOL"]).Trim();

                        dtPart = AddColumn(dtPart, uid, cids);
                        PartPriceInfo addpart = new PartPriceInfo();
                        try
                        {
                            bool blStatus = false;
                            if (dtPart != null)
                            {
                                //If Part Is Not in Database then Pop Up Message
                                if (dtPart.Rows.Count <= 0)
                                {
                                    //Show the message.

                                    //return Resources.Resource.ThePartNumberYouHaveSpecifiedIsNotAValidPartNoAlertMessage;

                                    addpart.Message = "Not Vaild";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;
                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                                else
                                {
                                    try
                                    {
                                        if (otype == 0)
                                        {
                                            PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                            if (PartCategoryID == "1")
                                            {
                                                addpart.Message = "accessory parts";
                                                addpart.ID = 0;
                                                addpart.Description = "";
                                                addpart.Partno = "";
                                                addpart.Available = "";
                                                addpart.NS = 0;
                                                addpart.Qty = 0;

                                                addpart.AGroupId = "AGroupId";
                                                objAddPart.Add(addpart);
                                                return objAddPart.ToList();
                                            }
                                            //  return Resources.Resource.PleaseOrderTheAccessoryPartsFromtheAccessoryPartSection;
                                        }
                                        else
                                        {
                                            PartCategoryID = dtPart.Rows[0]["PartCategoryID"].ToString();
                                            if (PartCategoryID == "2")
                                            {
                                                addpart.Message = "spare parts";
                                                addpart.ID = 0;
                                                addpart.Description = "";
                                                addpart.Partno = "";
                                                addpart.Available = "";
                                                addpart.NS = 0;
                                                addpart.Qty = 0;

                                                addpart.AGroupId = "AGroupId";
                                                objAddPart.Add(addpart);
                                                return objAddPart.ToList();
                                            }
                                        }

                                        //Checking that Part is Serviceable or Non Serviceable
                                        if (Convert.ToInt32(dtPart.Rows[0]["NonServiceable"]) == 1 || Convert.ToBoolean(dtPart.Rows[0]["NonServiceable"]) == true)
                                        {
                                            //Show the message.
                                            // return Resources.Resource.ThePartNumberYouHaveSpecifiedIsANonServiceablePartAlertMessage;
                                            addpart.Message = "NonServiceable";
                                            addpart.ID = 0;
                                            addpart.Description = "";
                                            addpart.Partno = "";
                                            addpart.Available = "";
                                            addpart.NS = 0;
                                            addpart.Qty = 0;

                                            addpart.AGroupId = "AGroupId";
                                            objAddPart.Add(addpart);
                                            return objAddPart.ToList();
                                        }
                                        else { }
                                    }
                                    catch
                                    {
                                        // PartPriceInfo addpart = new PartPriceInfo();
                                        addpart.Message = "Invalid";
                                        addpart.ID = 0;
                                        addpart.Description = "";
                                        addpart.Partno = "";
                                        addpart.Available = "";
                                        addpart.NS = 0;
                                        addpart.Qty = 0;

                                        addpart.AGroupId = "AGroupId";
                                        objAddPart.Add(addpart);
                                        return objAddPart.ToList();
                                    }

                                }


                                if (blStatus == true)
                                {
                                    //Show the message
                                    //return Resources.Resource.PartQuantitySuccessfullyUpdatedInOrderListAlertMessage;
                                    addpart.Message = "Update";
                                    addpart.ID = 0;
                                    addpart.Description = "";
                                    addpart.Partno = "";
                                    addpart.Available = "";
                                    addpart.NS = 0;
                                    addpart.Qty = 0;

                                    addpart.AGroupId = "AGroupId";
                                    objAddPart.Add(addpart);
                                    return objAddPart.ToList();
                                }
                                else
                                {
                                    //Show the message
                                    //return Resources.Resource.PartSuccessfullyAddedToOrderListAlertMessage;
                                    foreach (DataRow dr in dtPart.Rows)
                                    {
                                        addpart.Message = "Success";
                                        addpart.ID = Convert.ToInt32(dr["ID"]);
                                        addpart.Description = Convert.ToString(dr["DESCRIPTION"]) == "" ? "" : Convert.ToString(dr["DESCRIPTION"]);
                                        addpart.Partno = Convert.ToString(dr["PARTNO"]) == "" ? "" : Convert.ToString(dr["PARTNO"]);
                                        addpart.Available = Convert.ToString(dr["AVAILABLE"]) == "" ? "" : Convert.ToString(dr["AVAILABLE"]);
                                        addpart.NS = Convert.ToInt32(dr["NONSERVICEABLE"]) == 0 ? 0 : Convert.ToInt32(dr["NONSERVICEABLE"]);
                                        addpart.Qty = Convert.ToInt32(dr["QTY"]) == 0 ? 0 : Convert.ToInt32(dr["QTY"]);
                                        addpart.Dist_Cost = Convert.ToString(dr["PRICE1"]) == "0" ? "0" : Convert.ToString(dr["PRICE1"]);
                                        addpart.Dist_Msrp = Convert.ToString(dr["PRICE2"]) == "0" ? "0" : Convert.ToString(dr["PRICE2"]);
                                        addpart.Cost = Convert.ToString(dr["PRICE3"]) == "0" ? "0" : Convert.ToString(dr["PRICE3"]);
                                        addpart.Msrp = Convert.ToString(dr["PRICE4"]) == "0" ? "0" : Convert.ToString(dr["PRICE4"]);
                                        addpart.AGroupId = agroupid == "" ? "0" : agroupid;
                                        addpart.CountryName = countryname;
                                        addpart.Currency = currency;
                                        objAddPart.Add(addpart);
                                    }
                                    //return objAddPart.ToList();
                                }
                            }
                            else
                            {
                                //Show the message
                                // return Resources.Resource.PleaseEnterAValidPartNoAlertMessage;
                                addpart.Message = "Valid";
                                addpart.ID = 0;
                                addpart.Description = "";
                                addpart.Partno = "";
                                addpart.Available = "";
                                addpart.NS = 0;
                                addpart.Qty = 0;

                                addpart.AGroupId = "AGroupId";
                                objAddPart.Add(addpart);
                                return objAddPart.ToList();
                            }

                        }
                        catch
                        {
                            addpart.Message = "Invalid";
                            addpart.ID = 0;
                            addpart.Description = "";
                            addpart.Partno = "";
                            addpart.Available = "";
                            addpart.NS = 0;
                            addpart.Qty = 0;

                            addpart.AGroupId = "AGroupId";
                            objAddPart.Add(addpart);
                            return objAddPart.ToList();
                        }
                    }

                }
                else
                {
                    PartPriceInfo addpart = new PartPriceInfo();
                    addpart.Message = "Invalid";
                    addpart.ID = 0;
                    addpart.Description = "";
                    addpart.Partno = "";
                    addpart.Available = "";
                    addpart.NS = 0;
                    addpart.Qty = 0;

                    addpart.AGroupId = "AGroupId";
                    objAddPart.Add(addpart);
                    return objAddPart.ToList();
                }
            }
            catch (Exception ex)
            {
                PartPriceInfo addpart = new PartPriceInfo();
                addpart.Message = "Invalid";
                addpart.ID = 0;
                addpart.Description = "";
                addpart.Partno = "";
                addpart.Available = "";
                addpart.NS = 0;
                addpart.Qty = 0;

                addpart.AGroupId = "AGroupId";
                objAddPart.Add(addpart);
                return objAddPart.ToList();
            }

            return objAddPart.ToList();
        }

        public DataTable AddColumn(DataTable dt, int userid, int CountryID)
        {
            OrderListData orderlistdata = new OrderListData();
            string str = @"select d.id as UserID from tbl_distributor d
                        inner join tbl_user u on d.userid=u.id where usertype=0 and d.userid=" + userid + "";
            //Assign the value as integer.
            int DistID = Convert.ToInt32(maincls.DbCon.ExecuteScaler(str));
            //Adding the Column into Datatable
            if (!dt.Columns.Contains("PRICE1"))
                dt.Columns.Add("PRICE1");
            if (!dt.Columns.Contains("PRICE2"))
                dt.Columns.Add("PRICE2");

            if (!dt.Columns.Contains("PRICE3"))
                dt.Columns.Add("PRICE3");
            if (!dt.Columns.Contains("PRICE4"))
                dt.Columns.Add("PRICE4");

            if (!dt.Columns.Contains("TOTAL1"))
                dt.Columns.Add("TOTAL1", typeof(System.Decimal));
            if (!dt.Columns.Contains("TOTAL2"))
                dt.Columns.Add("TOTAL2", typeof(System.Decimal));

            if (!dt.Columns.Contains("TOTAL3"))
                dt.Columns.Add("TOTAL3", typeof(System.Decimal));
            if (!dt.Columns.Contains("TOTAL4"))
                dt.Columns.Add("TOTAL4", typeof(System.Decimal));
            if (!dt.Columns.Contains("STATUS"))
                dt.Columns.Add("STATUS");
            if (!dt.Columns.Contains("INSTOCK"))
                dt.Columns.Add("INSTOCK");
            foreach (DataRow dr in dt.Rows)
            {
                int iCount = 0;
                int iPartID = Convert.ToInt32(dr["ID"]);//Assign the PartID

                //Filling the DataTable 
                DataTable dtPartPriceDetail = orderlistdata.GetPriceDetail(iPartID, CountryID); //MainClass.DbCon.GetTable(strSql, "partPrice");
                object objINStock = maincls.DbCon.ExecuteScaler("select Partsource from tbl_part where ID= " + iPartID);
                //object objINStock = MainClass.DbCon.ExecuteScaler("select INStock from tbl_partDistributor where DistributorID=" + DistID + " and PartID= " + iPartID);
                dr["INSTOCK"] = Convert.ToString(objINStock) == "" ? 0 : Convert.ToInt32(Convert.ToString(objINStock));
                //If DataTable is not null
                if (dtPartPriceDetail != null)
                {
                    //If no. of Rows is more than 0 in Datatable
                    if (dtPartPriceDetail.Rows.Count > 0)
                    {
                        //Assign the Value into Main Table
                        for (int iCoun = 0; iCoun < 4; iCoun++)
                        {
                            //If Price is there then execute
                            if (!Convert.IsDBNull(dtPartPriceDetail.Rows[iCount]["Price" + (iCoun + 1)]))
                            {
                                try
                                {
                                    dr["Price" + (iCoun + 1)] = (Convert.ToDecimal(dtPartPriceDetail.Rows[iCount]["Price" + (iCoun + 1)])).ToString("0.00");
                                }
                                catch
                                {
                                    dr["Price" + (iCoun + 1)] = Convert.ToDouble(dtPartPriceDetail.Rows[iCount]["Price" + (iCoun + 1)]);
                                }
                                try
                                {
                                    dr["Total" + (iCoun + 1)] = (Convert.ToDecimal(dtPartPriceDetail.Rows[iCount]["Price" + (iCoun + 1)]) * Convert.ToInt32(dr["QTY"])).ToString("0.00");
                                }
                                catch
                                {
                                    dr["Total" + (iCoun + 1)] = Convert.ToDouble(dtPartPriceDetail.Rows[iCount]["Price" + (iCoun + 1)]) * Convert.ToDouble(dr["QTY"]); ;
                                }
                                //Save the Changes
                                dt.AcceptChanges();
                            }
                            else
                            {
                                dr["Price" + (iCoun + 1)] = 0;
                                dr["Total" + (iCoun + 1)] = 0;
                            }

                        }
                    }
                    else
                    {
                        for (int iCoun = 0; iCoun < 2; iCoun++)
                        {
                            dr["Price" + (iCoun + 1)] = 0;
                            dr["Total" + (iCoun + 1)] = 0;
                        }
                    }
                }
                iCount += 1;
                //DataTable dtPartStatus = new DataTable();
                object objPartStatus = new object();
                try
                {
                    string strPartstatus = @"select Partstatus from tbl_part where id=" + iPartID;
                    //dtPartStatus = MainClass.DbCon.GetTable(strPartstatus, "tbl_Part");
                    objPartStatus = maincls.DbCon.ExecuteScaler(strPartstatus);
                    dr["STATUS"] = dtPartPriceDetail.Rows[0]["Status"].ToString();
                }
                catch
                {

                    if (objPartStatus != null)
                    {
                        if (Convert.ToInt32(objPartStatus) == 4)
                            dr["STATUS"] = "Images/RED.GIF";
                        else
                            dr["STATUS"] = "Images/GREEN.GIF";
                    }
                    else
                        dr["STATUS"] = "Images/GREEN.GIF";
                }
            }
            //Save the Changes.
            dt.AcceptChanges();
            return dt;//Retuning the Datatable
        }
        # endregion

        # region Notification for enable-disable

        //enable disable notification by device id
        public List<CardInfo> NotificationUpdate(Int32 id, string imei)
        {
            string imeis = DecryptAes(imei);
            List<CardInfo> objInfo = new List<CardInfo>();
            try
            {
                CommanParameter cp;
                CommanParametrs cps = new CommanParametrs();

                cp = new CommanParameter("@IMEI", imeis, DbType.String);
                cps.Add(cp);
                cp = new CommanParameter("@Notification", id, DbType.Boolean);
                cps.Add(cp);
                int i = maincls.DbCon.ExecuteProcNonQuery("PROC_NOTIFICATIONUPDATE", cps);
                if (i > 0)
                {
                    CardInfo cinfo = new CardInfo();
                    cinfo.Status = 1;
                    cinfo.Message = "Success";
                    objInfo.Add(cinfo);
                }
                else
                {
                    CardInfo cinfo = new CardInfo();
                    cinfo.Status = 0;
                    cinfo.Message = "Failed";
                    objInfo.Add(cinfo);
                }

                return objInfo.ToList();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                CardInfo cinfo = new CardInfo();
                cinfo.Status = 0;
                cinfo.Message = "Failed";
                objInfo.Add(cinfo);
                return objInfo.ToList();
            }
        }

        # endregion

        # region feedback type

        public List<FeedbackTypeInfo> FeedbackType()
        {
            List<FeedbackTypeInfo> objFeedback = new List<FeedbackTypeInfo>();
            DataSet ds = new DataSet();
            DataTable dt;
            DataRow dr;
            DataColumn idCoulumn;
            DataColumn nameCoulumn;
            dt = new DataTable();
            idCoulumn = new DataColumn("ID", Type.GetType("System.Decimal"));
            nameCoulumn = new DataColumn("NAME", Type.GetType("System.String"));

            dt.Columns.Add(idCoulumn);
            dt.Columns.Add(nameCoulumn);

            dr = dt.NewRow();
            dr["ID"] = 0;
            dr["NAME"] = "Information";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 1;
            dr["NAME"] = "Issue";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 2;
            dr["NAME"] = "Request";
            dt.Rows.Add(dr);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt.Rows)
                {
                    FeedbackTypeInfo fti = new FeedbackTypeInfo();
                    fti.ID = Convert.ToInt32(dr1["ID"]);
                    fti.Name = Convert.ToString(dr1["NAME"]);
                    objFeedback.Add(fti);
                }
                return objFeedback.ToList();
            }
            else
            {
                return objFeedback.ToList();
            }
        }

        #endregion

        # region Encryption with AES 128


        public RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public String DecryptAes(String encryptedText, String key = "tR7nR6wZHGjYMCuV")
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            string rr = Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
            return rr;
        }

        public byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public String EncryptAes(String plainText, String key = "tR7nR6wZHGjYMCuV")
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }



        #endregion

        #region Splash service for Key  value AES 128

        public List<AesKey> Splash()
        {
            List<AesKey> objSeed = new List<AesKey>();
            try
            {
                AesKey ak = new AesKey();
                ak.Seed = "tR7nR6wZHGjYMCuV";
                objSeed.Add(ak);
                return objSeed.ToList();
            }
            catch (Exception ex)
            {
                return objSeed.ToList();
            }
        }

        #endregion

        # region AI function

        public AIResponse GetTestResponse()
        {
            var testObject = new
            {
                id = "2d2d947b-6ccd-4615-8f16-59b8bfc0fa6b",
                timestamp = "2017-07-13T11:03:43.023Z",
                result = new
                {
                    source = "agent",
                    resolvedQuery = "test params 1.23",
                    speech = "",
                    action = "test_params",
                    parameters = new
                    {
                        number = "1.23",
                        integer = "17",
                        str = "string value",
                        complex_param = new { nested_key = "nested_value" }
                    },
                    metadata = new
                    {
                        intentId = "46a278fb-0ffc-4748-aa9a-5563d89199ee",
                        intentName = "test params"
                    }
                },
                status = new { code = 200, errorType = "success" }
            };

            var jsonText = JsonConvert.SerializeObject(testObject);
            return JsonConvert.DeserializeObject<AIResponse>(jsonText);
        }

        # endregion

    }
}
