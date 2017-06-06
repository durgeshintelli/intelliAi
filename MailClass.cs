using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Catalog;

namespace BusinessProcess
{
    /// <summary>
    /// This class used for mail sending
    /// </summary>
    public class MailClass
    {
        #region  Main Class Declaration
        //Creating the object of MainClass
        MainCommon MainClass = new MainCommon();
        readonly string smtpserver;
        readonly SmtpClient smptClient;
        readonly string mailUserID;
        #endregion
        /// <summary>
        /// constructor of mail class
        /// </summary>
        public MailClass()
        {
            //
            // TODO: Add constructor logic here
            //
            //smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
            //smptClient = new System.Net.Mail.SmtpClient(smtpserver);
            //            smptClient.Credentials = new System.Net.NetworkCredential("noreply@intellinetsystem.com", "Sita@2014");

            //smtpserver, mailUserId, mailPwd
            DataTable dt = MainClass.DbCon.GetTable("select * from tbl_setting", "mailsetting");
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                smtpserver = Convert.ToString(dr["smtpserver"]);
                smptClient = new System.Net.Mail.SmtpClient(smtpserver);
                if (dt.Columns.Contains("mailPwd") && !string.IsNullOrWhiteSpace(Convert.ToString(dr["mailPwd"])))
                {
                    smptClient.UseDefaultCredentials = false;
                    mailUserID = dr["mailUserId"].ToString();
                    smptClient.Credentials = new System.Net.NetworkCredential(Convert.ToString(dr["mailUserId"]), Convert.ToString(dr["mailPwd"]));

                }
            }
            else
                smptClient = new System.Net.Mail.SmtpClient(smtpserver);
        }
        public MailClass(string smptserver, int port, string userid, string password)
        {
            smptClient = new System.Net.Mail.SmtpClient(smptserver);
            smptClient.Port = port > 0 ? port : 25;
            smptClient.UseDefaultCredentials = false;
            mailUserID = userid;
            smptClient.Credentials = new System.Net.NetworkCredential(userid, password);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mail"></param>
        /// <param name="To"></param>
        /// <returns></returns>
        public string SendMails(MailTemplateProp Mail, string To)
        {
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;

                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                {
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                }
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);

                SendMail.To.Add(To);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = true;
                SendMail.Subject = Mail.Subject;
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);

                try
                {
                    smptClient.Send(SendMail);

                }
                catch (SmtpException ex)
                {

                    if (ex.StatusCode == SmtpStatusCode.MailboxBusy)
                    {
                        SendMail.IsBodyHtml = false;
                        smptClient.Send(SendMail);
                        return "Mail Sent";
                    }
                    else if (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
                    {
                        if (ex.InnerException is SmtpFailedRecipientsException)
                        {
                            return ((SmtpFailedRecipientsException)ex.InnerException).FailedRecipient;
                        }
                    }
                    else
                        return ex.Message;
                }
                return "Mail Sent";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                smptClient.Dispose();
            }
        }

        public string SendMails(MailTemplateProp Mail, string To, System.Net.Mail.Attachment pdfatt)
        {
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;
                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);
                SendMail.To.Add(To);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = true;
                SendMail.Subject = Mail.Subject;
                SendMail.Attachments.Add(pdfatt);
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                smptClient.Send(SendMail);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (pdfatt != null)
                    pdfatt.Dispose();
                smptClient.Dispose();
            }

        }

        public string SendMails(MailTemplateProp Mail, string To, System.Net.Mail.Attachment pdfatt, string UserMailID)
        {
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;
                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);
                SendMail.To.Add(To);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = true;
                SendMail.Subject = Mail.Subject;
                SendMail.Attachments.Add(pdfatt);
                SendMail.CC.Add(UserMailID);
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                smptClient.Send(SendMail);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (pdfatt != null)
                    pdfatt.Dispose();
                smptClient.Dispose();
            }
        }

        public string SendMails(MailTemplateProp Mail, string To, DataTable dtsource)
        {
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;

                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);
                SendMail.To.Add(To);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = true;
                SendMail.Subject = Mail.Subject;
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                smptClient.Send(SendMail);
                return "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                smptClient.Dispose();
            }
        }

        public string SendOrderMails(MailTemplateProp Mail, string To, string CC, System.Net.Mail.Attachment pdfatt)
        {
            string mstatus = "";
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;
                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);
                SendMail.To.Add(To);
                if (!string.IsNullOrEmpty(CC))
                    SendMail.CC.Add(CC);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = true;
                SendMail.Subject = Mail.Subject;
                SendMail.Attachments.Add(pdfatt);
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                try
                {
                    smptClient.Send(SendMail);
                    mstatus = "1";
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy ||
                            status == SmtpStatusCode.MailboxUnavailable)
                        {
                            // mstatus = "0";
                        }
                        else
                        {
                            mstatus += ex.InnerExceptions[i].FailedRecipient + ",";
                        }
                    }
                }
                catch (SmtpException ex)
                {

                    if (ex.StatusCode == SmtpStatusCode.MailboxBusy)
                    {
                        SendMail.IsBodyHtml = false;
                        smptClient.Send(SendMail);
                        mstatus = "1";
                    }
                    else if (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
                    {
                        if (ex.InnerException is SmtpFailedRecipientsException)
                        {
                            mstatus += ((SmtpFailedRecipientsException)ex.InnerException).FailedRecipient;
                        }

                    }
                    else
                        mstatus = ex.Message;
                }
            }
            catch (Exception ex)
            {
                mstatus = ex.Message;
            }
            finally
            {
                if (pdfatt != null)
                    pdfatt.Dispose();
                smptClient.Dispose();
            }
            return mstatus;
        }

        public string SendFeedbackMail(MailTemplateProp Mail, string To, System.Net.Mail.Attachment pdfatt)
        {
            try
            {
                //string smtpserver = MainClass.DbCon.ExecuteScaler("select smtpserver from tbl_setting where id=1").ToString();
                string strHTMLBody;
                // Create the HTML Message Body 
                strHTMLBody = Mail.Template;
                System.Net.Mail.MailMessage SendMail = new System.Net.Mail.MailMessage();
                if (!String.IsNullOrEmpty(mailUserID))
                    SendMail.From = new System.Net.Mail.MailAddress(mailUserID);
                else
                    SendMail.From = new System.Net.Mail.MailAddress(Mail.FromEmail);
                SendMail.To.Add(To);
                SendMail.Body = strHTMLBody;
                SendMail.IsBodyHtml = Mail.IsBodyHtml;
                SendMail.Subject = Mail.Subject;
                if (pdfatt != null)
                    SendMail.Attachments.Add(pdfatt);

                if (!String.IsNullOrWhiteSpace(Mail.BCCEmail))
                    SendMail.Bcc.Add(Mail.BCCEmail);

                if (!String.IsNullOrWhiteSpace(Mail.CCEmail))
                    SendMail.CC.Add(Mail.CCEmail);
                //SmtpClient smtp = new SmtpClient("75.126.222.146");
                //System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpserver);
                //smptClient.Send(SendMail);

                try
                {
                    smptClient.Send(SendMail);

                }
                catch (SmtpException ex)
                {

                    if (ex.StatusCode == SmtpStatusCode.MailboxBusy)
                    {
                        SendMail.IsBodyHtml = false;
                        smptClient.Send(SendMail);
                        return "Mail Sent";
                    }
                    else if (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
                    {
                        if (ex.InnerException is SmtpFailedRecipientsException)
                        {
                            return ((SmtpFailedRecipientsException)ex.InnerException).FailedRecipient;
                        }

                    }
                    else
                        return ex.Message;
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (pdfatt != null)
                    pdfatt.Dispose();
                smptClient.Dispose();

            }
            return "Mail Sent";
        }

    }
}
