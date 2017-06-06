using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiAiSDK.Model;

namespace MusaService
{
    [ServiceContract]
    public interface IService1
    {
        
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTestResponse")]
        AIResponse GetTestResponse();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]

     #region ResponseMessage
    /// <summary>
    /// This Class Is Used To Send General Response.
    /// </summary>
    public class ResponseMessage
    {
        public string Msg { get; set; }
        public bool Status { get; set; }
        public string OrderNo { get; set; }
        // public PropertyCollection Attributs { get; set; }
        //public List<Dictionary<string, string>> Attributs { get; set; }
        public bool IsUniqueException { get; set; }
    }

    #endregion

    # region AI


    public class AIResponse
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Lang { get; set; }
        public Result Result { get; set; }
        public Status Status { get; set; }
        public string SessionId { get; set; }
        public fulfillment fulfillment { get; set; }
        public bool IsError
        {
            get
            {
                if (Status != null && Status.Code.HasValue && Status.Code >= 400)
                {
                    return true;
                }

                return false;
            }
        }
    }
    public class Status
    {
        public int? Code { get; set; }
        public string ErrorType { get; set; }
        public string ErrorDetails { get; set; }
        public string ErrorID { get; set; }
        public Status()
        {
        }
    }
    
     public class fulfillment
    {
        public string speech { get; set; }
    }

    public class Result
    {
        String action;
        public Boolean ActionIncomplete { get; set; }
        public String Action
        {
            get
            {
                if (string.IsNullOrEmpty(action))
                {
                    return string.Empty;
                }
                return action;
            }
            set
            {
                action = value;
            }
        }

        public Dictionary<string, object> Parameters { get; set; }
        public AIOutputContext[] Contexts { get; set; }
        public Metadata Metadata { get; set; }
        public String ResolvedQuery { get; set; }
        public Fulfillment Fulfillment { get; set; }
        public string Source { get; set; }
        public float Score { get; set; }
        public bool HasParameters
        {
            get
            {
                return Parameters != null && Parameters.Count > 0;
            }
        }

        public string GetStringParameter(string name, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (Parameters.ContainsKey(name))
            {
                return Parameters[name].ToString();
            }

            return defaultValue;
        }

        public int GetIntParameter(string name, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (Parameters.ContainsKey(name))
            {
                var parameterValue = Parameters[name].ToString();
                int result;
                if (int.TryParse(parameterValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                float floatResult;
                if (float.TryParse(parameterValue, NumberStyles.Float, CultureInfo.InvariantCulture, out floatResult))
                {
                    result = Convert.ToInt32(floatResult);
                    return result;
                }
            }

            return defaultValue;
        }

        public float GetFloatParameter(string name, float defaultValue = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (Parameters.ContainsKey(name))
            {
                var parameterValue = Parameters[name].ToString();
                float result;
                if (float.TryParse(parameterValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        public JObject GetJsonParameter(string name, JObject defaultValue = null)
        {
            if (string.IsNullOrEmpty("name"))
            {
                throw new ArgumentNullException(name);
            }

            if (Parameters.ContainsKey(name))
            {
                var parameter = Parameters[name] as JObject;
                if (parameter != null)
                {
                    return parameter;
                }
            }

            return defaultValue;
        }

        public AIOutputContext GetContext(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name must be not empty", name);
            }

            return Contexts.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public Result()
        {
        }
    }

   #endregion
    
    /// <summary>
    /// This class is used to parse datatable and dataset to JSON
    /// </summary>
    public static class JSONDataTable
    {
        //Parse DataTable To JSON
        public static string GetJSONString(this DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            return serializer.Serialize(table.ToDictionary());
        }
        //Parse DataSet To JSON
        public static string GetJSONString(this DataSet data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(data.ToDictionary());
        }
        public static string ToJson(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
        static Dictionary<string, object> ToDictionary(this DataTable table)
        {
            return new Dictionary<string, object>
        {
            { table.TableName, table.RowsToDictionary() }
        };
        }
        static object RowsToDictionary(this DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().ToArray();
            return table.Rows.Cast<DataRow>().Select(r => columns.ToDictionary(c => c.ColumnName, c => r[c]));
        }

        static Dictionary<string, object> ToDictionary(this DataSet data)
        {
            return data.Tables.Cast<DataTable>().ToDictionary(t => t.TableName, t => t.RowsToDictionary());
        }

        public static List<string> GetComboHTML(this List<ComboData> ilist)
        {
            //select (new  ComboData { ID = s.ID, Text = s.UOMName })).OrderBy(d => d.Text).ToList();
            List<string> abc = (from s in ilist select string.Concat(";", s.ID, ":", s.Text)).ToList();
            abc.Insert(0, ":All");
            return abc;

        }

        public static List<string> GetAutoCompleteString(this List<ComboData> ilist)
        {
            //select (new  ComboData { ID = s.ID, Text = s.UOMName })).OrderBy(d => d.Text).ToList();
            return (from s in ilist select s.Text).Distinct().ToList();
        }
    }

    public class ComboData
    {
        public decimal ID { get; set; }
        public string Text { get; set; }
    }

}
