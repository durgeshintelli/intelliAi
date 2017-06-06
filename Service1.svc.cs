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
