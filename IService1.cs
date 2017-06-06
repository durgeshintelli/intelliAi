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
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ValidateUser/{sUserName}/{sPassword}")]
        List<ValidateUser> ValidateUser(string sUserName, string sPassword);

        //[OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ValidateUser")]
        //List<ValidateUser> ValidateUser(string sUserName, string sPassword);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetImeiAvailability")]
        List<Authentication> GetImeiAvailability(string imei);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetAndroidImeiAvailability")]
        List<Authentication> GetAndroidImeiAvailability(string imei);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetAndroidDealerAvailability")]
        List<DealerAvailable> GetAndroidDealerAvailability(string DEALERCODE);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetDealerAvailability")]
        List<DealerAvailable> GetDealerAvailability(string DEALERCODE);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetOemAuthorization")]
        List<activecolumninfo> GetOemAuthorization(string DEALERCODE);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "UserDetails")]
        List<UserInfo> UserDetails(string ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ModuleRight")]
        List<RightInfo> ModuleRight(string RoleId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "DistributorId")]
        List<UserInfo> DistributorId(string id, string usertype);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LanguageList")]
        List<LanguageInfo> LanguageList();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DeviceInformation")]
        List<ValidateUser> DeviceInformation(string release, string brand, string device, string manufacturer, string model, string mobileno, string imei, string size, string userid, string actcode, string fname, string lname, string gcmid, string AppVersion);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DeviceLogout")]
        List<ValidateUser> DeviceLogout(string userid, string imei);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DeviceDeActivate")]
        List<ValidateUser> DeviceDeActivate(string imei);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "ImageQuery")]
        List<ValidateUser> ImageQuery(byte[] byteImage, string description, string userid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "ValidateCountry")]
        List<ValidateUser> ValidateCountry(string UserId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "ForgetPassword")]
        List<MessageResult> ForgetPassword(string uname, string email);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CountryCurrency")]
        List<CurrencyInfo> CountryCurrency();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "ActiveColumn")]
        List<activecolumninfo> ActiveColumn(int iType, int lngid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CategoryFill")]
        List<CategoryInfo> CategoryFill(string UserID, int CateTypeID, int LngId, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CategoryVariant")]
        List<CategoryInfo> CategoryVariant(int Id, string CountryID, string StartDate, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CategoryAggregate")]
        List<CategoryInfo> CategoryAggregate(int Id, string StartDate, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "Assembly")]
        List<VinAssemblyInfo> Assembly(int Id, bool blnCondi, string StartDate, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CategoryFillType")]
        List<CategoryInfo> CategoryFillType(bool blnAccessLvl, string UserID, int LngId, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "IllustrationPartList")]
        List<IllustrationInfo> IllustrationPartList(string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, string UserType);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AutoCompletePartNo")]
        List<AutoPartInfo> AutoCompletePartNo(string UserID, int LngId, string StartDate, string searchvalue, string vehicle, string model, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AutoCompleteDescription")]
        List<AutoDescriptionInfo> AutoCompleteDescription(string UserID, int LngId, string StartDate, string description, string vehicle, string model, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillVehicle")]
        List<CategoryInfo> FillVehicle(bool blnAccessLvl, string UserID, int LngId, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillModel")]
        List<CategoryInfo> FillModel(string VehicleIDs, string SelDate, string CountryID, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "partList")]
        List<PartInfo> partList(string UserId, string StartDate, Int32 Model, Int32 Variant, string Partno, string Description, string iCountryId, Int32 pagesize, string UserType);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "RequestTypeData")]
        List<RequestCategory> RequestTypeData(Int32 RequestType);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AccidentalViewType")]
        List<AccidentalViewInfo> AccidentalViewType(int iModelID, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AccidentalImage")]
        List<AccidentalImageInfo> AccidentalImage(int iModelID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AccidentalViewAssembly")]
        List<AccidentalInfo> AccidentalViewAssembly(string strarrwords, int iid, bool blnRemove, decimal zooms, string strAssID, string chassisno, string vid, string currentDate, int lngid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AccidentalAssemblyOrder")]
        List<AccidentalOrderInfo> AccidentalAssemblyOrder(int ID, int ModelId, string iCountryID, int LngId, string UserID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LoadAssemblyImage")]
        List<CategoryImage> LoadAssemblyImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getThumbnail")]
        List<CategoryImage> getThumbnail(int Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LoadAccidentalImage")]
        List<CategoryImage> LoadAccidentalImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LoadCategoryTypeImage")]
        List<CategoryImage> LoadCategoryTypeImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getAcessoryCategoryImage")]
        List<CategoryImage> getAcessoryCategoryImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getAcessoryCategoryPartImage")]
        List<CategoryImage> getAcessoryCategoryPartImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAssemblyHotsPotImage")]
        List<CategoryImage> GetAssemblyHotsPotImage(Int32 AssPartId, string keycode, decimal zoomfactor, string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, bool onImage = false, string device = "iphone");

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAndroidAssemblyHotsPotImage")]
        List<CategoryImage> GetAndroidAssemblyHotsPotImage(Int32 AssPartId, string keycode, decimal zoomfactor, string UserId, Int32 ModelID, Int32 AssemblyID, string StartDate, Int32 SrVINNO, Int32 Lngid, bool onImage = false, string device = "4");


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LoadCategoryImage")]
        List<CategoryImage> LoadCategoryImage(Int32 Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CountryList")]
        List<CountryInfo> CountryList();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "ChangePassword")]
        List<PasswordInfo> ChangePassword(string userid, string Oldpass, string Newpass, string Confpass);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AcessaryPartCategorySearch")]
        List<AcessoryPartInfo> AcessaryPartCategorySearch(string cid, string userid, Int32 AcctypeId, Int32 AccId, Int32 iLngId, string StartDate);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillAccessoriType")]
        List<FillAcessType> FillAccessoriType(Int32 lngid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillAccessoriChild")]
        List<FillAcessChild> FillAccessoriChild(Int32 AccId, Int32 LngId, string cid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAcessoryPartList")]
        List<AcessoryPartInfo> GetAcessoryPartList(string cid, string userid, Int32 lngid, Int32 pagesize);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAcessoryPartListSearch")]
        List<AcessoryPartInfo> GetAcessoryPartListSearch(string details, string cid, string userid, Int32 lngid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetCheckoutDescription")]
        List<CheckOutNotes> GetCheckoutDescription(Int32 ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getOrderType")]
        List<OrderType> getOrderType(bool inactive, Int32 lngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "OrderTypeAttributes")]
        List<OrderTypeAttr> OrderTypeAttributes(Int32 OrderTypeID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "fillDistAddress")]
        List<DistributorBillAddress> fillDistAddress(string DistributorId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetShippingAddress")]
        List<ShipAddress> GetShippingAddress(string DistributorId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getShipMethod")]
        List<CarrierMethod> getShipMethod(bool inactive, string userid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetCountry")]
        List<Country> GetCountry();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetState")]
        List<State> GetState(Int32 countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetCity")]
        List<City> GetCity(Int32 stateid, Int32 countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetCityName")]
        List<City> GetCityName(string city, decimal stateid, decimal countryid);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "OrderMasterList")]
        List<OrderMasterInfo> OrderMasterList(string distributorid, Int32 lngid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DistributorInfoLoad")]
        List<UserInfo> DistributorInfoLoad(string userid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DistributorContactPerson")]
        List<UserInfo> DistributorContactPerson(string userid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetOrderDetailList")]
        List<OrderInfo> GetOrderDetailList(string OrderId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "OrderReports")]
        List<OrderReports> OrderReports(string OrderNo, string OrderId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetShipOrderDetails")]
        List<ShipAddress> GetShipOrderDetails(string ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetOrderDetailListCount")]
        List<OrderQuantity> GetOrderDetailListCount(string OrderId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "Addpart")]
        List<AddPart> Addpart(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CheckAlternateParts")]
        List<AddPart> CheckAlternateParts(string PartNo, string ptype, string utype, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LiteratureList")]
        List<Literature> LiteratureList(int ID, int assid, string description, int parentnode);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetMapModelsDetails")]
        List<LiteratureMap> GetMapModelsDetails(Int32 ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "partHistory")]
        List<PartInfo> partHistory(decimal ID, string modelid, string startdate, string UserId, int LngId, String type);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPartReturnList")]
        List<InventryPartReturnInfo> GetPartReturnList(string distId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPartReturnChildDataList")]
        List<inventoryChildPartReturnInfo> GetPartReturnChildDataList(string ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "partApplicabilityList")]
        List<PartApplicableInfo> partApplicabilityList(string UserId, string Partno, Int32 pagesize);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillAccItem")]
        List<AcessoryPartInfo> FillAccItem(string iPartID, string StartDate, string cid, Int32 lngid, string usertype);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAlternateParts")]
        List<AlternatePartInfo> GetAlternateParts(string PartNo, string ptype, string utype, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AddOrderIllustrationData")]
        List<AlternatePartInfo> AddOrderIllustrationData(string rType, string PID, string ID, string Qty, string cid, Int32 lngid, string usertype, string MId, string decAId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AlternatePartList")]
        List<AlternatePartInfo> AlternatePartList(string rType, string PID, string ID, string Qty, string GroupId, string cid, Int32 lngid, string usertype, string MId, string decAId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FillShipNoTrack")]
        List<ShipmentInfo> FillShipNoTrack(decimal OrderId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetShipmentDetails")]
        List<TrackInfo> GetShipmentDetails(int CourierId, string ShipNo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DealerRepresentative")]
        string DealerRepresentative(string userid);

        //[OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetOrderNumber")]
        //List<ResultInfo> GetOrderNumber();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SaveShippingAddress")]
        Int32 SaveShippingAddress(Int32 shippingid, Int32 distributorid, string fname, string lname, string company, string address1, string address2, string address3, string phone, string countryid, string stateid, string cityid, string pincode, string email, Int32 addresstype, Int32 userid, string cityname, Int32 orderid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SaveOrderType")]
        Int32 SaveOrderType(string CustAttrbutes, Int32 DelearId, string OrderDate, string OrderNo, Int32 OTypeId, decimal Amount, Int32 Status, decimal TaxAmt, decimal DisAmt, Int32 ShippingMethodId, decimal ShippingCost, string OrderNote, Int32 ShipID, string AccountNo, Int32 Currency, decimal DiscountId, decimal Discount);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SaveOrderTable")]
        ResponseMessage SaveOrderTable(int orderfor, Int32 shippingid, string distributorid, string fname, string lname, string company, string address1, string address2, string address3, string phone, string countryid, string stateid, string cityid, string pincode, string email, Int32 addresstype, string userid, string cityname, string orderid, string Ordertypename, string CustAttrbutes, string DelearId, string OrderDate, Int32 OTypeId, string Amount, Int32 Status, decimal TaxAmt, decimal DisAmt, Int32 ShippingMethodId, decimal ShippingCost, string OrderNote, Int32 ShipID, string AccountNo, string Currency, string DiscountId, string Discount, List<OrderDetailProp> pdata);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetShipInfoDetails")]
        List<Partordinfo> GetShipInfoDetails(string OrderNo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetShipInfoDetailsshipwise")]
        List<Partordinfo> GetShipInfoDetailsshipwise(string OrderNo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "test")]
        string test(string OrderNo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetViewStats")]
        List<Statistics> GetViewStats(string userID);

        //[OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SavePdf")]
        //string SavePdf(string html1, string filename, string compName, string dbilltoo, string str11, string str21, string str31, Int32 userid, string currency, string partdata);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "testdata")]
        List<TestInfo> testdata(string aa);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetInventoryList")]
        List<InventoryListInfo> GetInventoryList(string Id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetInventory")]
        List<InventoryListInfo> GetInventory(string Id, double lat, double lon);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPartDesInvPool")]
        List<ComboData> GetPartDesInvPool(string partno);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "InventoryStatistics")]
        List<InventryStatus> InventoryStatistics();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SavePdf")]
        List<EmailStatus> SavePdf(string orderno, string userid, string DistributorId);

        //[OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "SavePdfAndroid")]
        //List<EmailStatus> SavePdfAndroid(string orderno, Int32 userid, Int32 DistributorId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "CallTagGeneration")]
        List<CallTagStatus> CallTagGeneration(string ReturnNo, string Priority, string DistributorId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetDiscount")]
        List<DiscountInfo> GetDiscount(Int32 OrderTypeID, string OPrice, string userid, string cid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "UpdatePart")]
        List<AddPart> UpdatePart(string Qty, List<Partinfo> Plist, string CountryID, string hfOrderType, int otype = 0, string usertype = "3");

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "addreview")]
        DataTable addreview(string rType, string PID, string ID, string Qty, Int32 cid, Int32 lngid, Int32 usertype, string MId, string decAId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "AlertMessage")]
        List<AlertInfo> AlertMessage();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "EncryptFormat")]
        string EncryptFormat(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "DiscountConfig")]
        List<DiscountConfig> DiscountConfig(Int32 ID);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetModelSearchData")]
        List<ModelSearchInfo> GetModelSearchData(int SerchType, string SearchStr, string UserID, string UserType, string CountryID, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetAggrigateData")]
        List<AggregateInfo> GetAggrigateData(decimal ModelId, int LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "PartPrice")]
        List<PartPriceInfo> PartPrice(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0, string userid = "0");

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "PartPriceAll")]
        List<PartPriceInfo> PartPriceAll(string Qty, string PartNo, string CountryID, string hfOrderType, int otype = 0, string userid = "0");

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "VinSearch")]
        List<VinInfo> VinSearch(string UserId, string VinNo, string EngineNo, string countryid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "VinSearchAggregate")]
        List<CategoryInfo> VinSearchAggregate(Int32 VariantId, Int32 LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "VinSearchAssembly")]
        List<VinAssemblyInfo> VinSearchAssembly(int AggregateID, string StartDate, int ModelID, int SBOMID, Int32 LngId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "NotificationUpdate")]
        List<CardInfo> NotificationUpdate(Int32 id, string imei);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "FeedbackType")]
        List<FeedbackTypeInfo> FeedbackType();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "OrderMasterListNew")]
        List<OrderMasterInfo> OrderMasterListNew(string distributorid, Int32 lngid, Int32 page, string fromdate, string todate);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetPartReturnListNew")]
        List<InventryPartReturnInfo> GetPartReturnListNew(string distId, Int32 page);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "Splash")]
        List<AesKey> Splash();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "getOrderTypeList")]
        List<OrderType> getOrderTypeList(bool inactive, Int32 lngId, Int32 cid);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetDateValue")]
        List<GetDate> GetDateValue(int index, string countryid, int flag);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTestResponse")]
        AIResponse GetTestResponse();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]

    public class ValidateUser
    {
        Int32 id = 0;


        [DataMember]
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }


    }

    public class MessageResult
    {
        string msg = string.Empty;
        public string Message
        {
            get { return msg; }
            set { msg = value; }
        }
    }
    public class FillAcessChild
    {
        string pno = string.Empty;
        string pdesc = string.Empty;

        public Int32 id
        {
            get;
            set;
        }

        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }

        public string description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }
        public Int32 acessorypartid
        {
            get;
            set;
        }
        public Int32 inactive
        {
            get;
            set;
        }
        public Int32 price
        {
            get;
            set;
        }
        public Int32 attachmenttype
        {
            get;
            set;
        }
    }

    public class FillAcessType
    {
        string acssnme = string.Empty;
        public Int32 id
        {
            get;
            set;
        }
        public Int32 Acctypeid
        {
            get;
            set;
        }

        public Int32 Priority
        {
            get;
            set;
        }
        public Int32 Inactive
        {
            get;
            set;
        }

        public string AcctypeName
        {
            get { return acssnme; }
            set { acssnme = value; }
        }
    }
    public class BindModel
    {
        string catnme = string.Empty;
        public Int32 id
        {
            get;
            set;
        }

        public string CategoryName
        {
            get { return catnme; }
            set { catnme = value; }
        }
    }

    public class AcessoryPartDetails
    {
        string pno = string.Empty;
        string pdesc = string.Empty;

        public Int32 AccessoryPartId
        {
            get;
            set;
        }

        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }

        public string description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }
        public Int32 ChkPartAtt
        {
            get;
            set;
        }


    }

    public class AttachmentInfo
    {
        string pdesc = string.Empty;
        string psize = string.Empty;
        string path = string.Empty;

        public Int32 ID
        {
            get;
            set;
        }
        public string Description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }

        public string Size
        {
            get { return psize; }
            set { psize = value; }
        }
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }

    public class AcessoryPartInfo
    {
        string pno = string.Empty;
        string pdesc = string.Empty;
        string logo = string.Empty;
        string pname = string.Empty;
        string acctypename = string.Empty;
        string color = string.Empty;
        string image = string.Empty;
        string vehicle = string.Empty;
        Int32 totalcount = 0;

        public Int32 ID
        {
            get;
            set;
        }

        public int total
        {
            get { return totalcount; }
            set { totalcount = value; }
        }

        public Int32 VehicleId
        {
            get;
            set;
        }

        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }
        public string partName
        {
            get { return pname; }
            set { pname = value; }
        }
        public string description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }

        public string Vehicle
        {
            get { return vehicle; }
            set { vehicle = value; }
        }
        public string AccTypeName
        {
            get { return acctypename; }
            set { acctypename = value; }
        }
        public decimal Price
        {
            get;
            set;
        }
        public string BrandName
        {
            get;
            set;
        }

        public string ColorName
        {
            get { return color; }
            set { color = value; }
        }
        public Int32 ColorId
        {
            get;
            set;
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public Int32 VariantId
        {
            get;
            set;
        }
        public Int32 Qty
        {
            get;
            set;
        }
        public Int32 AccTypeId
        {
            get;
            set;
        }
        public Int32 CategoryTypeId
        {
            get;
            set;
        }
        public string LogoThumb
        {
            get { return logo; }
            set { logo = value; }
        }

    }

    public class PartInfo
    {
        string pno = string.Empty;
        string pdesc = string.Empty;

        Int32 agroupid = 0;
        Int32 partid = 0;
        Int32 pqty = 0;
        Int32 vid = 0;

        Int32 totalcount = 0;
        string assemblyname = string.Empty;
        string pstdate = string.Empty;
        string peddate = string.Empty;
        string pgrpname = string.Empty;
        string phu = string.Empty;
        string assid = string.Empty;
        string service = string.Empty;
        string remarks = string.Empty;
        string pid = string.Empty;
        string group = string.Empty;
        string ic = string.Empty;
        string id = string.Empty;
        string price1 = string.Empty;
        string price2 = string.Empty;
        string price3 = string.Empty;
        string price4 = string.Empty;
        string model = string.Empty;
        string vehicle = string.Empty;
        string variant = string.Empty;

        public int PartId
        {
            get { return partid; }
            set { partid = value; }
        }

        public int AGroupId
        {
            get { return agroupid; }
            set { agroupid = value; }
        }
        public string AssemblyName
        {
            get { return assemblyname; }
            set { assemblyname = value; }
        }
        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }
        public string description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }

        public int quantity
        {
            get { return pqty; }
            set { pqty = value; }
        }
        public int total
        {
            get { return totalcount; }
            set { totalcount = value; }
        }
        public string startdate
        {
            get { return pstdate; }
            set { pstdate = value; }
        }
        public string enddate
        {
            get { return peddate; }
            set { peddate = value; }
        }
        public string history
        {
            get { return phu; }
            set { phu = value; }
        }
        public string NS
        {
            get { return service; }
            set { service = value; }
        }
        public string GroupNo
        {
            get { return group; }
            set { group = value; }
        }
        public string Remark
        {
            get { return remarks; }
            set { remarks = value; }
        }
        public string PartID
        {
            get { return pid; }
            set { pid = value; }
        }
        public string Interchangeable
        {
            get { return ic; }
            set { ic = value; }
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string AssemblyID
        {
            get { return assid; }
            set { assid = value; }
        }
        public string PmapID
        {
            get;
            set;
        }
        public string CatmapID
        {
            get;
            set;
        }
        public string Price1
        {
            get { return price1; }
            set { price1 = value; }
        }
        public string Price2
        {
            get { return price2; }
            set { price2 = value; }
        }
        public string Price3
        {
            get { return price3; }
            set { price3 = value; }
        }
        public string Price4
        {
            get { return price4; }
            set { price4 = value; }
        }
        public string Model
        {
            get { return model; }
            set { model = value; }
        }
        public string Vehicle
        {
            get;
            set;
            //get { return vehicle; }
            //set { Vehicle = value; }
        }
        public Int32 VehicleId
        {
            get { return vid; }
            set { vid = value; }
        }
        public string Variant
        {
            get { return variant; }
            set { variant = value; }
        }
    }

    public class Applicable
    {

        public Int32 quantity
        {
            get;
            set;
        }
        public string startdate
        {
            get;
            set;
        }
        public string enddate
        {
            get;
            set;
        }

        public string Vehicle
        {
            get;
            set;
        }
        public Int32 VehicleId
        {
            get;
            set;
        }
        public string Variant
        {
            get;
            set;
        }
    }

    public class AccidentalOrderInfo
    {
        public Int32 ID
        {
            get;
            set;
        }
        public Int32 AssemblyID
        {
            get;
            set;
        }
        public string PartNo
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public Int32 NQTY
        {
            get;
            set;
        }
        public string Price
        {
            get;
            set;
        }
        public string aGroupID
        {
            get;
            set;
        }
        public string Servicable
        {
            get;
            set;
        }
    }

    public class AccidentalInfo
    {
        public int ID
        {
            get;
            set;
        }
        public String CategoryName
        {
            get;
            set;
        }
        public String AssemblyName
        {
            get;
            set;
        }
        public String FigNo
        {
            get;
            set;
        }
        public Int32 AMID
        {
            get;
            set;
        }

    }

    public class ModelInfo
    {
        string pModelName = string.Empty;
        Int32 pId = 0;
        public int Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public string ModelName
        {
            get { return pModelName; }
            set { pModelName = value; }
        }
    }
    public class PasswordInfo
    {
        public string ChangeResult
        {
            get;
            set;
        }
    }

    public class LanguageInfo
    {
        int id = 0;
        String lngcode = string.Empty;
        String lng = string.Empty;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public string LanguageCode
        {
            get { return lngcode; }
            set { lngcode = value; }
        }
        public string LanguageName
        {
            get { return lng; }
            set { lng = value; }
        }
    }

    public class CurrencyInfo
    {
        string currency = string.Empty;
        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }
    }

    public class OrderMasterInfo
    {
        Int32 id = 0;
        Int32 totalrecords = 0;
        Int32 dealerId = 0;
        string dealername = string.Empty;
        string dealercode = string.Empty;
        Int32 ordertypeid = 0;
        string odertype = string.Empty;
        decimal orderamount = 0;
        string orderno = string.Empty;
        Int32 orderstatus = 0;
        Int32 shippingmethodid = 0;
        string orderdate = string.Empty;
        string methodname = string.Empty;
        string currencysymbol = string.Empty;
        Int32 carrierid = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public int TotalRecords
        {
            get { return totalrecords; }
            set { totalrecords = value; }
        }
        public int DealerId
        {
            get { return dealerId; }
            set { dealerId = value; }
        }
        public int OrderTypeId
        {
            get { return ordertypeid; }
            set { ordertypeid = value; }
        }
        public int OrderStatus
        {
            get { return orderstatus; }
            set { orderstatus = value; }
        }
        public int ShippingMethodId
        {
            get { return shippingmethodid; }
            set { shippingmethodid = value; }
        }
        public int CarrierId
        {
            get { return carrierid; }
            set { carrierid = value; }
        }
        public string OrderDate
        {
            get { return orderdate; }
            set { orderdate = value; }
        }

        public string DealerName
        {
            get { return dealername; }
            set { dealername = value; }
        }
        public string DealerCode
        {
            get { return dealercode; }
            set { dealercode = value; }
        }
        public string OderType
        {
            get { return odertype; }
            set { odertype = value; }
        }
        public string OrderNo
        {
            get { return orderno; }
            set { orderno = value; }
        }
        public string MethodName
        {
            get { return methodname; }
            set { methodname = value; }
        }
        public decimal OrderAmount
        {
            get { return orderamount; }
            set { orderamount = value; }
        }
        public string CurrencySymbol
        {
            get { return currencysymbol; }
            set { currencysymbol = value; }
        }
    }

    public class UserInfo
    {
        string email = string.Empty;
        string name = string.Empty;
        string Adminemail = string.Empty;
        string feedbackno = string.Empty;
        string currency = string.Empty;
        string usercode = string.Empty;
        string address = string.Empty;
        string country = string.Empty;
        string state = string.Empty;
        string city = string.Empty;
        string contact = string.Empty;
        string title = string.Empty;
        Int32 currencyid = 0;

        public Int32 CurrencyId
        {
            get { return currencyid; }
            set { currencyid = value; }
        }
        public string Contact
        {
            get { return contact; }
            set { contact = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        public string State
        {
            get { return state; }
            set { state = value; }
        }
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public string Code
        {
            get { return usercode; }
            set { usercode = value; }
        }

        public string AdminEmail
        {
            get { return Adminemail; }
            set { Adminemail = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string FeedBackNo
        {
            get { return feedbackno; }
            set { feedbackno = value; }
        }
        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }
        public Int32 RoleId
        {
            get;
            set;
        }
        public Int32 CountryId
        {
            get;
            set;
        }
        public Int32 UserType
        {
            get;
            set;
        }
    }

    public class AutoPartInfo
    {
        string aPartno = string.Empty;
        Int32 pId = 0;
        public int Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public string PartNo
        {
            get { return aPartno; }
            set { aPartno = value; }
        }
    }

    public class RightInfo
    {
        public string RightName
        {
            get;
            set;
        }
        public Int32 Rights
        {
            get;
            set;
        }

    }
    public class SettingInfo
    {
        public Int32 NoofAttempt
        {
            get;
            set;
        }
        public Int32 DefCountryId
        {
            get;
            set;
        }
        public Int32 PassMaxlength
        {
            get;
            set;
        }
        public Int32 PassMinlength
        {
            get;
            set;
        }
        public string smtpServer
        {
            get;
            set;
        }
        public Int32 Zoomsize
        {
            get;
            set;
        }
        public Int32 Issobommandatory
        {
            get;
            set;
        }
        public Int32 FeedbackNo
        {
            get;
            set;
        }
        public string FeedbackEmailId
        {
            get;
            set;
        }
        public Int32 OrderNo
        {
            get;
            set;
        }

    }

    public class CountryInfo
    {
        public Int32 ID
        {
            get;
            set;
        }
        public string CountryName
        {
            get;
            set;
        }
        public string CurrencySymbol
        {
            get;
            set;
        }

    }

    public class AccidentalViewInfo
    {
        public string ViewType
        {
            get;
            set;
        }
        public Int32 ID
        {
            get;
            set;
        }
        public Int32 ViewTypeMapId
        {
            get;
            set;
        }

    }

    public class AccidentalImageInfo
    {
        public Int32 ID1
        {
            get;
            set;
        }
        public Int32 ID
        {
            get;
            set;
        }
        public Int32 Priority
        {
            get;
            set;
        }

    }

    public class AutoDescriptionInfo
    {
        string aDescription = string.Empty;
        Int32 pId = 0;
        public int Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public string Description
        {
            get { return aDescription; }
            set { aDescription = value; }
        }
    }

    public class VinInfo
    {
        Int32 pId = 0;
        Int32 pSbomId = 0;
        string pSbomCode = string.Empty;
        string pdesc = string.Empty;
        string pstdate;
        string pEngineNo = string.Empty;
        string pChassisNo = string.Empty;
        string pModelName = string.Empty;
        string pVehicle = string.Empty;
        byte[] bytImage;

        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 SbomId
        {
            get { return pSbomId; }
            set { pSbomId = value; }
        }
        public string SbomCode
        {
            get { return pSbomCode; }
            set { pSbomCode = value; }
        }
        public string Description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }
        public string ProductionDate
        {
            get { return pstdate; }
            set { pstdate = value; }
        }
        public string EngineNo
        {
            get { return pEngineNo; }
            set { pEngineNo = value; }
        }
        public string ChassisNo
        {
            get { return pChassisNo; }
            set { pChassisNo = value; }
        }
        public string ModelName
        {
            get { return pModelName; }
            set { pModelName = value; }
        }
        public string Vehicle
        {
            get { return pVehicle; }
            set { pVehicle = value; }
        }
        public byte[] ImageByte
        {
            set { bytImage = value; }
        }
        public string ImageString
        {
            get
            {
                if (bytImage == null)
                    return "0";
                else
                    return Convert.ToBase64String(bytImage);
            }
            set { ImageByte = Convert.FromBase64String(value); }
        }
    }

    public class CategoryInfo
    {
        Int32 pId = 0;
        Int32 pCategoryId = 0;
        Int32 cPriority = 0;
        string pCategoryName = string.Empty;
        bool piSLatLevel = false;
        string pimagePath = string.Empty;

        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 CategoryId
        {
            get { return pCategoryId; }
            set { pCategoryId = value; }
        }
        public string CategoryName
        {
            get { return pCategoryName; }
            set { pCategoryName = value; }
        }
        public bool iSLatLevel
        {
            get { return piSLatLevel; }
            set { piSLatLevel = value; }
        }
        public string ImagePath
        {
            get { return pimagePath; }
            set { pimagePath = value; }
        }
        public Int32 Priority
        {
            get { return cPriority; }
            set { cPriority = value; }
        }
    }
    public class RequestCategory
    {
        Int32 pId = 0;
        Int32 pRequestType = 0;
        string pRequestDesc = string.Empty;

        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }

        public Int32 RequestType
        {
            get { return pRequestType; }
            set { pRequestType = value; }
        }
        public string RequestDesc
        {
            get { return pRequestDesc; }
            set { pRequestDesc = value; }
        }
    }

    public class CategoryImage
    {
        byte[] bytImage;
        Int32 pId = 0;
        Int32 assPartId = 0;

        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 AssPartId
        {
            get { return assPartId; }
            set { assPartId = value; }
        }
        public byte[] ImageByte
        {
            set { bytImage = value; }
        }
        public string ImageString
        {
            get { return Convert.ToBase64String(bytImage); }
            set { bytImage = Convert.FromBase64String(value); }
        }


    }

    public class DealerAvailable
    {
        Int32 activecount = 0;
        public Int32 maxpermit
        {
            get;
            set;
        }
        public Int32 ImeiTotal
        {
            get;
            set;
        }
        public Boolean status
        {
            get;
            set;
        }
        public Int32 ActiveCount
        {
            get { return activecount; }
            set { activecount = value; }
        }
    }
    public class Authentication
    {
        public Int32 ImeiTotal
        {
            get;
            set;
        }
        public Int32 maxpermit
        {
            get;
            set;
        }
        public Int32 ImeiCount
        {
            get;
            set;
        }
        public Int32 userid
        {
            get;
            set;
        }
        public Int32 Active
        {
            get;
            set;
        }
        public Int32 AdminActive
        {
            get;
            set;
        }

        public string enddate
        {
            get;
            set;
        }
    }

    public class LoginAuthenticate
    {
        string pIMEI = string.Empty;
        public Int32 maxpermit
        {
            get;
            set;
        }
        public Int32 userid
        {
            get;
            set;
        }

        public string IMEI
        {
            get { return pIMEI; }
            set { pIMEI = value; }
        }
    }

    public class Country
    {
        Int32 id = 0;
        string cname = string.Empty;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public string CountryName
        {
            get { return cname; }
            set { cname = value; }
        }
    }

    public class State
    {
        Int32 id = 0;
        string sname = string.Empty;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public string StateName
        {
            get { return sname; }
            set { sname = value; }
        }
    }

    public class City
    {
        Int32 id = 0;
        string cname = string.Empty;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public string CityName
        {
            get { return cname; }
            set { cname = value; }
        }
    }

    public class CarrierMethod
    {
        Int32 id = 0;
        string carname = string.Empty;
        string metname = string.Empty;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public string CarrierName
        {
            get { return carname; }
            set { carname = value; }
        }
        public string MethodName
        {
            get { return metname; }
            set { metname = value; }
        }

    }

    public class ShipAddress
    {
        string fname = string.Empty;
        string lname = string.Empty;
        string company = string.Empty;
        string address1 = string.Empty;
        string address2 = string.Empty;
        string address3 = string.Empty;
        string pincode = string.Empty;
        string email = string.Empty;
        string phone = string.Empty;
        string couname = string.Empty;
        string stname = string.Empty;
        string ctname = string.Empty;
        string orderid = string.Empty;
        Int32 distid = 0;
        Int32 couid = 0;
        Int32 id = 0;
        Int32 stid = 0;
        Int32 ciid = 0;
        public Int32 DistributorId
        {
            get { return distid; }
            set { distid = value; }
        }
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 CountryId
        {
            get { return couid; }
            set { couid = value; }
        }
        public Int32 StateId
        {
            get { return stid; }
            set { stid = value; }
        }
        public Int32 CityId
        {
            get { return ciid; }
            set { ciid = value; }
        }
        public string FistName
        {
            get { return fname; }
            set { fname = value; }
        }
        public string LastName
        {
            get { return lname; }
            set { lname = value; }
        }
        public string Company
        {
            get { return company; }
            set { company = value; }
        }
        public string Address1
        {
            get { return address1; }
            set { address1 = value; }
        }
        public string Address2
        {
            get { return address2; }
            set { address2 = value; }
        }
        public string Address3
        {
            get { return address3; }
            set { address3 = value; }
        }
        public string PostalCode
        {
            get { return pincode; }
            set { pincode = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        public string CountryName
        {
            get { return couname; }
            set { couname = value; }
        }
        public string StateName
        {
            get { return stname; }
            set { stname = value; }
        }
        public string CityName
        {
            get { return ctname; }
            set { ctname = value; }
        }
        public string OrderNo
        {
            get { return orderid; }
            set { orderid = value; }
        }
    }

    public class DistributorBillAddress
    {
        string fname = string.Empty;
        string lname = string.Empty;
        string company = string.Empty;
        string address = string.Empty;
        string pincode = string.Empty;
        string email = string.Empty;
        string phone = string.Empty;
        string countryname = string.Empty;
        string statename = string.Empty;
        string cityname = string.Empty;
        Int32 couid = 0;
        Int32 stid = 0;
        Int32 ciid = 0;
        public Int32 CountryId
        {
            get { return couid; }
            set { couid = value; }
        }
        public Int32 StateId
        {
            get { return stid; }
            set { stid = value; }
        }
        public Int32 CityId
        {
            get { return ciid; }
            set { ciid = value; }
        }
        public string FistName
        {
            get { return fname; }
            set { fname = value; }
        }
        public string LastName
        {
            get { return lname; }
            set { lname = value; }
        }
        public string Company
        {
            get { return company; }
            set { company = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string PostalCode
        {
            get { return pincode; }
            set { pincode = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        public string CountryName
        {
            get { return countryname; }
            set { countryname = value; }
        }
        public string StateName
        {
            get { return statename; }
            set { statename = value; }
        }
        public string CityName
        {
            get { return cityname; }
            set { cityname = value; }
        }
    }

    public class OrderTypeAttr
    {
        Int32 pId = 0;
        string otypeattr = string.Empty;
        string otypecaption = string.Empty;
        Boolean attrtype = false;
        Boolean validtype = false;
        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 Priority
        {
            get { return pId; }
            set { pId = value; }
        }
        public string OTypeAttributes
        {
            get { return otypeattr; }
            set { otypeattr = value; }
        }
        public string OTypeCaption
        {
            get { return otypecaption; }
            set { otypecaption = value; }
        }

        public Boolean AttributeType
        {
            get { return attrtype; }
            set { attrtype = value; }
        }
        public Boolean ValidationType
        {
            get { return validtype; }
            set { validtype = value; }
        }

    }

    public class CheckOutNotes
    {
        string note = string.Empty;
        string smartyurl = string.Empty;
        public string Note
        {
            get { return note; }
            set { note = value; }
        }
        public string SmartyUrl
        {
            get { return smartyurl; }
            set { smartyurl = value; }
        }
    }

    public class OrderType
    {
        decimal pId = 0;
        Int32 pd = 0;
        string otype = string.Empty;
        string odesc = string.Empty;

        public Int32 Id
        {
            //get { return pId; }
            // set { pId = value; }
            get;
            set;
        }
        public decimal MOV
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 Priority
        {
            get { return pd; }
            set { pd = value; }
        }
        public string Type
        {
            get { return otype; }
            set { otype = value; }
        }
        public string Description
        {
            get { return odesc; }
            set { odesc = value; }
        }
    }

    public class pdf
    {
        string ppath = string.Empty;
        string ptext = string.Empty;
        string psize = string.Empty;
        string pdate = string.Empty;
        public string Path
        {
            get { return ppath; }
            set { ppath = value; }
        }
        public string Text
        {
            get { return ptext; }
            set { ptext = value; }
        }
        public string Size
        {
            get { return psize; }
            set { psize = value; }
        }
        public string Date
        {
            get { return pdate; }
            set { pdate = value; }
        }
    }

    public class VinAssemblyInfo
    {
        Int32 pId = 0;
        Int32 pAMId = 0;
        string pCategoryName = string.Empty;
        string pFigNo = string.Empty;
        bool piSLatLevel = false;
        string pimagePath = string.Empty;

        public Int32 Id
        {
            get { return pId; }
            set { pId = value; }
        }
        public Int32 AMId
        {
            get { return pAMId; }
            set { pAMId = value; }
        }
        public string CategoryName
        {
            get { return pCategoryName; }
            set { pCategoryName = value; }
        }
        public string FigNo
        {
            get { return pFigNo; }
            set { pFigNo = value; }
        }
        public bool iSLatLevel
        {
            get { return piSLatLevel; }
            set { piSLatLevel = value; }
        }
        public string ImagePath
        {
            get { return pimagePath; }
            set { pimagePath = value; }
        }
    }

    public class OrderQuantity
    {
        Int32 qty = 0;
        Int32 shipqty = 0;
        Int32 backorderqty = 0;

        public Int32 Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        public Int32 ShipQty
        {
            get { return shipqty; }
            set { shipqty = value; }
        }
        public Int32 BackOrderQty
        {
            get { return backorderqty; }
            set { backorderqty = value; }
        }
    }

    public class PartApplicableInfo
    {
        Int32 totalcount = 0;
        public PartApplicableInfo()
        {
            Applicables = new List<Applicable>();
        }
        public string partNumber
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }
        public string ID
        {
            get;
            set;
        }
        public string servicable
        {
            get;
            set;
        }
        public string qty
        {
            get;
            set;
        }
        public int total
        {
            get { return totalcount; }
            set { totalcount = value; }
        }
        public List<Applicable> Applicables { get; set; }




    }

    public class AlternatePartInfo
    {
        Int32 id = 0;
        Int32 qty = 0;
        string pno = string.Empty;
        string description = string.Empty;
        string available = string.Empty;
        string price1 = string.Empty;
        string price2 = string.Empty;
        string aGroupId = string.Empty;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }

        }
        public Int32 Qty
        {
            get { return qty; }
            set { qty = value; }

        }
        public string aGroupID
        {
            get { return aGroupId; }
            set { aGroupId = value; }
        }
        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Available
        {
            get { return available; }
            set { available = value; }
        }
        public string Price1
        {
            get { return price1; }
            set { price1 = value; }
        }
        public string Price2
        {
            get { return price2; }
            set { price2 = value; }
        }
    }

    public class IllustrationInfo
    {
        string pno = string.Empty;
        string pdesc = string.Empty;
        Int32 pqty = 0;
        Int32 agroupid = 0;
        Int32 parentpartid = 0;
        string pstdate = string.Empty;
        string peddate = string.Empty;
        string pgrpname = string.Empty;
        string phu = string.Empty;

        public string partNumber
        {
            get { return pno; }
            set { pno = value; }
        }
        public string description
        {
            get { return pdesc; }
            set { pdesc = value; }
        }

        public int quantity
        {
            get { return pqty; }
            set { pqty = value; }
        }
        public int aGroupID
        {
            get { return agroupid; }
            set { agroupid = value; }
        }
        public int ParentPartId
        {
            get { return parentpartid; }
            set { parentpartid = value; }
        }
        public string startdate
        {
            get { return pstdate; }
            set { pstdate = value; }
        }
        //public string enddate
        //{
        //    get { return peddate; }
        //    set { peddate = value; }
        //}
        public string history
        {
            get { return phu; }
            set { phu = value; }
        }
        public string Servicable
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }
        public string AlternateDescription
        {
            get;
            set;
        }
        public string Price1
        {
            get;
            set;
        }
        public string Price2
        {
            get;
            set;
        }
        public string Asspartmodid
        {
            get;
            set;
        }
        public string Partid
        {
            get;
            set;
        }
        public string Asseblypartid
        {
            get;
            set;
        }
        public string RefNo
        {
            get;
            set;
        }
    }

    public class activecolumninfo
    {
        public Boolean status
        {
            get;
            set;
        }
    }

    public class OrderReports
    {
        Int32 id = 0;
        Int32 dealerid = 0;
        Int32 otdescid = 0;
        Int32 orderstatus = 0;
        Int32 shippingmethodid = 0;
        Int32 shipaddid = 0;
        Int32 pendingsince = 0;
        string orderno = string.Empty;
        string orderdate = string.Empty;
        string otdesk = string.Empty;
        string code = string.Empty;
        string companyname = string.Empty;
        string ufname = string.Empty;
        string ulname = string.Empty;
        string shippingremarks = string.Empty;
        string note = string.Empty;
        string otype = string.Empty;
        string ordertype = string.Empty;
        string pono = string.Empty;
        string methodname = string.Empty;
        string ordervalue = string.Empty;
        string percentage = string.Empty;
        string netvalue = string.Empty;
        string discountvalue = string.Empty;

        public string DiscountValue
        {
            get { return discountvalue; }
            set { discountvalue = value; }
        }
        public string OrderValue
        {
            get { return ordervalue; }
            set { ordervalue = value; }
        }
        public string Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }
        public string NetValue
        {
            get { return netvalue; }
            set { netvalue = value; }
        }
        public Int32 Id
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 DealerId
        {
            get { return dealerid; }
            set { dealerid = value; }
        }
        public Int32 OtDeskId
        {
            get { return otdescid; }
            set { otdescid = value; }
        }
        public Int32 OrderStatus
        {
            get { return orderstatus; }
            set { orderstatus = value; }
        }
        public Int32 ShippingMethodId
        {
            get { return shippingmethodid; }
            set { shippingmethodid = value; }
        }
        public Int32 ShipAddId
        {
            get { return shipaddid; }
            set { shipaddid = value; }
        }
        public Int32 PendingSince
        {
            get { return pendingsince; }
            set { pendingsince = value; }
        }
        public string OrderNo
        {
            get { return orderno; }
            set { orderno = value; }
        }
        public string OrderDate
        {
            get { return orderdate; }
            set { orderdate = value; }
        }
        public string OtDesk
        {
            get { return otdesk; }
            set { otdesk = value; }
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string CompanyName
        {
            get { return companyname; }
            set { companyname = value; }
        }
        public string Ufname
        {
            get { return ufname; }
            set { ufname = value; }
        }
        public string Ulname
        {
            get { return ulname; }
            set { ulname = value; }
        }
        public string ShippingRemarks
        {
            get { return shippingremarks; }
            set { shippingremarks = value; }
        }
        public string Note
        {
            get { return note; }
            set { note = value; }
        }
        public string Otype
        {
            get { return otype; }
            set { otype = value; }
        }
        public string OrderType
        {
            get { return ordertype; }
            set { ordertype = value; }
        }
        public string PONO
        {
            get { return pono; }
            set { pono = value; }
        }
        public string MethodName
        {
            get { return methodname; }
            set { methodname = value; }
        }
    }

    public class OrderInfo
    {
        Int32 id = 0;
        Int32 partid = 0;
        decimal price = 0;
        Int32 qty = 0;
        Int32 shipqty = 0;
        Int32 backorderqty = 0;
        decimal amount = 0;
        decimal taxamount = 0;
        string partno = string.Empty;
        string description = string.Empty;
        string currencysymbol = string.Empty;

        public Int32 Id
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 PartId
        {
            get { return partid; }
            set { partid = value; }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        public Int32 Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        public Int32 ShipQty
        {
            get { return shipqty; }
            set { shipqty = value; }
        }
        public Int32 BackOrderQty
        {
            get { return backorderqty; }
            set { backorderqty = value; }
        }
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public decimal TaxAmount
        {
            get { return taxamount; }
            set { taxamount = value; }
        }
        public string PartNumber
        {
            get { return partno; }
            set { partno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string CurrencySymbol
        {
            get { return currencysymbol; }
            set { currencysymbol = value; }
        }

    }

    public class AddPart
    {
        Int32 Id = 0;
        Int32 Quantity = 0;
        Int32 NonServiceable = 0;
        decimal price = 0;
        decimal price1 = 0;
        decimal price2 = 0;
        string agroupid = string.Empty;
        string partno = string.Empty;
        string description = string.Empty;
        string available = string.Empty;
        string message = string.Empty;

        public Int32 ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public Int32 Qty
        {
            get { return Quantity; }
            set { Quantity = value; }
        }
        public Int32 NS
        {
            get { return NonServiceable; }
            set { NonServiceable = value; }
        }
        public decimal PartPrice
        {
            get { return price; }
            set { price = value; }
        }
        public decimal Price1
        {
            get { return price1; }
            set { price1 = value; }
        }
        public decimal Price2
        {
            get { return price2; }
            set { price2 = value; }
        }
        public string Partno
        {
            get { return partno; }
            set { partno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Available
        {
            get { return available; }
            set { available = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string AGroupId
        {
            get { return agroupid; }
            set { agroupid = value; }
        }

    }

    public class PartPriceInfo
    {
        Int32 Id = 0;
        Int32 Quantity = 0;
        Int32 NonServiceable = 0;
        string price4 = "0";
        string price3 = "0";
        string price1 = "0";
        string price2 = "0";
        string agroupid = string.Empty;
        string partno = string.Empty;
        string description = string.Empty;
        string available = string.Empty;
        string message = string.Empty;
        string countryname = string.Empty;
        string currency = string.Empty;

        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }
        public string CountryName
        {
            get { return countryname; }
            set { countryname = value; }
        }
        public Int32 ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public Int32 Qty
        {
            get { return Quantity; }
            set { Quantity = value; }
        }
        public Int32 NS
        {
            get { return NonServiceable; }
            set { NonServiceable = value; }
        }
        public string Dist_Cost
        {
            get { return price1; }
            set { price1 = value; }
        }
        public string Dist_Msrp
        {
            get { return price2; }
            set { price2 = value; }
        }
        public string Cost
        {
            get { return price3; }
            set { price3 = value; }
        }
        public string Msrp
        {
            get { return price4; }
            set { price4 = value; }
        }
        public string Partno
        {
            get { return partno; }
            set { partno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Available
        {
            get { return available; }
            set { available = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string AGroupId
        {
            get { return agroupid; }
            set { agroupid = value; }
        }

    }

    public class PartPriceAllInfo
    {
        Int32 Id = 0;
        Int32 Quantity = 0;
        Int32 NonServiceable = 0;

        string agroupid = string.Empty;
        string partno = string.Empty;
        string description = string.Empty;
        string available = string.Empty;
        string message = string.Empty;

        public Int32 ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public Int32 Qty
        {
            get { return Quantity; }
            set { Quantity = value; }
        }
        public Int32 NS
        {
            get { return NonServiceable; }
            set { NonServiceable = value; }
        }

        public string Partno
        {
            get { return partno; }
            set { partno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Available
        {
            get { return available; }
            set { available = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string AGroupId
        {
            get { return agroupid; }
            set { agroupid = value; }
        }

    }

    public class CountryPriceInfo
    {
        string price4 = "0";
        string price3 = "0";
        string price1 = "0";
        string price2 = "0";
        string countryname = string.Empty;
        public string CountryName
        {
            get { return countryname; }
            set { countryname = value; }
        }
        public string Dist_Cost
        {
            get { return price1; }
            set { price1 = value; }
        }
        public string Dist_Msrp
        {
            get { return price2; }
            set { price2 = value; }
        }
        public string Cost
        {
            get { return price3; }
            set { price3 = value; }
        }
        public string Msrp
        {
            get { return price4; }
            set { price4 = value; }
        }
    }

    public class Literature
    {
        Int32 Id = 0;
        Int32 type = 0;
        Int64 actsize = 0;
        string size = string.Empty;
        string description = string.Empty;
        string date = string.Empty;

        public Int32 ID
        {
            get { return Id; }
            set { Id = value; }
        }
        public Int64 ActSize
        {
            get { return actsize; }
            set { actsize = value; }
        }
        public Int32 Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
    }

    public class LiteratureMap
    {
        string typename = string.Empty;
        string name = string.Empty;
        public string TypeName
        {
            get { return typename; }
            set { typename = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class InventryPartReturnInfo
    {
        Int32 ID = 0;
        Int32 totalrecords = 0;
        string ReturnNo = string.Empty;
        Int32 SNO = 1;
        string RDate = string.Empty;
        string InvoiceNo = string.Empty;
        string IDate = string.Empty;
        string Status = string.Empty;
        Int32 intstatus = 0;
        string ReturnType = string.Empty;
        bool callTag = true;
        string CALLTAGNO = string.Empty;
        string diffdays = string.Empty;
        string FinalStatus = string.Empty;
        decimal CLOSEDCOUNT = 0;

        public Int32 id
        {
            get { return ID; }
            set { ID = value; }
        }

        public Int32 TotalRecords
        {
            get { return totalrecords; }
            set { totalrecords = value; }
        }
        public string returnno
        {
            get { return ReturnNo; }
            set { ReturnNo = value; }
        }
        public Int32 SNo
        {
            get { return SNO; }
            set { SNO = value; }
        }
        public string returndate
        {
            get { return RDate; }
            set { RDate = value; }
        }
        public string invoiceno
        {
            get { return InvoiceNo; }
            set { InvoiceNo = value; }
        }
        public string invoicedate2
        {
            get { return IDate; }
            set { IDate = value; }
        }
        public string status
        {
            get { return Status; }
            set { Status = value; }
        }
        public Int32 Intstatus
        {
            get { return intstatus; }
            set { intstatus = value; }
        }
        public string returntype
        {
            get { return ReturnType; }
            set { ReturnType = value; }
        }
        public bool Calltag
        {
            get { return callTag; }
            set { callTag = value; }
        }
        public string calltagno
        {
            get { return CALLTAGNO; }
            set { CALLTAGNO = value; }
        }
        public string Diffdays
        {
            get { return diffdays; }
            set { diffdays = value; }
        }
        public string finalstatus
        {
            get { return FinalStatus; }
            set { FinalStatus = value; }
        }
        public decimal closedcount
        {
            get { return CLOSEDCOUNT; }
            set { CLOSEDCOUNT = value; }
        }
    }

    public class inventoryChildPartReturnInfo
    {
        Int32 id = 0;
        Int32 RETURNID = 0;
        Int32 REASIONID = 0;
        string REASON = string.Empty;
        Int32 PARTID = 0;
        string PARTNO = string.Empty;
        Int32 QUANTITY = 0;
        string REMARKS = string.Empty;
        Int32 SHIPDTLID = 0;
        decimal RPRICE = 0;
        string INVOICENO = string.Empty;
        string INVOICEDATE = string.Empty;
        Int32 APPQTY = 0;

        string APPSTATUS = string.Empty;
        string APPREMARKS = string.Empty;
        string DESCRIPTION = string.Empty;
        decimal PRICE = 0;
        int APPCHK = 0;
        string LineNo = string.Empty;
        Int32 ACCEPTQTY = 0;
        decimal totalPrice = 0;
        Int32 totalquantity = 0;
        decimal totalamout = 0;

        public decimal TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
        public decimal TotalAmount
        {
            get { return totalamout; }
            set { totalamout = value; }
        }
        public Int32 TotalQuantity
        {
            get { return totalquantity; }
            set { totalquantity = value; }
        }
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 ReturnID
        {
            get { return RETURNID; }
            set { RETURNID = value; }
        }
        public Int32 ReasonId
        {
            get { return REASIONID; }
            set { REASIONID = value; }
        }
        public string Reason
        {
            get { return REASON; }
            set { REASON = value; }
        }
        public Int32 PartID
        {
            get { return PARTID; }
            set { PARTID = value; }
        }
        public string PartNo
        {
            get { return PARTNO; }
            set { PARTNO = value; }
        }
        public Int32 Quantity
        {
            get { return QUANTITY; }
            set { QUANTITY = value; }
        }
        public string Remarks
        {
            get { return REMARKS; }
            set { REMARKS = value; }
        }
        public Int32 shipDtlid
        {
            get { return SHIPDTLID; }
            set { SHIPDTLID = value; }
        }
        public decimal RPrice
        {
            get { return RPRICE; }
            set { RPRICE = value; }
        }
        public string InvoiceNo
        {
            get { return INVOICENO; }
            set { INVOICENO = value; }
        }
        public string InvoiceDate
        {
            get { return INVOICEDATE; }
            set { INVOICEDATE = value; }
        }
        public Int32 AppQty
        {
            get { return APPQTY; }
            set { APPQTY = value; }
        }
        public string AppStatus
        {
            get { return APPSTATUS; }
            set { APPSTATUS = value; }
        }
        public string AppRemarks
        {
            get { return APPREMARKS; }
            set { APPREMARKS = value; }
        }
        public string Description
        {
            get { return DESCRIPTION; }
            set { DESCRIPTION = value; }
        }
        public decimal Price
        {
            get { return PRICE; }
            set { PRICE = value; }
        }
        public Int32 appchk
        {
            get { return APPCHK; }
            set { APPCHK = value; }
        }
        public string POSNo
        {
            get { return LineNo; }
            set { LineNo = value; }
        }
        public Int32 AcceptQty
        {
            get { return ACCEPTQTY; }
            set { ACCEPTQTY = value; }
        }


    }

    public class ShipmentInfo
    {
        string shipno = string.Empty;

        public Int32 ID
        {
            get;
            set;
        }

        public string ShipNo
        {
            get { return shipno; }
            set { shipno = value; }
        }
    }

    public class ResultInfo
    {
        string orderno = string.Empty;

        public string OrderNo
        {
            get;
            set;
        }
    }

    public class GetInfo
    {
        Int32 no = 0;

        public Int32 No
        {
            get;
            set;
        }
    }

    public class TestInfo
    {
        string no = string.Empty;

        public string No
        {
            get;
            set;
        }
    }

    public class DiscountInfo
    {
        string message = string.Empty;
        Int32 id = 0;
        Int32 priority = 0;
        string dstype = string.Empty;

        public string DsType
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 Priority
        {
            get { return priority; }
            set { priority = value; }
        }

    }

    public class DiscountConfig
    {
        int id = 0;
        int ordertypeid = 0;
        bool dstype = false;
        decimal fprice = 0;
        decimal tprice = 0;
        decimal discount = 0;
        string term = string.Empty;
        bool freight = false;
        bool inactive = false;
        string startdate = string.Empty;
        string enddate = string.Empty;
        string subordertype = string.Empty;
        string message = string.Empty;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public int OrderTypeId
        { get { return ordertypeid; } set { ordertypeid = value; } }
        public bool DsType
        { get { return dstype; } set { dstype = value; } }
        public decimal FPrice
        { get { return fprice; } set { fprice = value; } }
        public decimal TPrice
        { get { return tprice; } set { tprice = value; } }
        public decimal Discount
        { get { return discount; } set { discount = value; } }
        public string Term
        { get { return term; } set { term = value; } }
        public bool Freight
        { get { return freight; } set { freight = value; } }
        public bool Inactive
        { get { return inactive; } set { inactive = value; } }
        public string StartDate
        { get { return startdate; } set { startdate = value; } }
        public string EndDate
        { get { return enddate; } set { enddate = value; } }
        public string SubOrderType
        { get { return subordertype; } set { subordertype = value; } }
        public string Message
        { get { return message; } set { message = value; } }

    }


    public class GetdataInfo
    {
        Int32 no = 0;

        public Int32 No
        {
            get;
            set;
        }

    }

    public class xmlInfo
    {
        public bool status
        {
            get;
            set;
        }
    }

    public class shipordwiseinfo
    {
        Int32 shipid = 0;
        Int32 attid = 0;
        Int32 qty = 0;
        string shipno = string.Empty;
        string methodname = string.Empty;
        string shipdate = string.Empty;
        string invoiceno = string.Empty;
        string invoicedate = string.Empty;
        string shaporderno = string.Empty;
        string shipment = string.Empty;

        public Int32 SHIPID
        {
            get { return shipid; }
            set { shipid = value; }
        }
        public Int32 ATTID
        {
            get { return attid; }
            set { attid = value; }
        }
        public Int32 QTY
        {
            get { return qty; }
            set { qty = value; }
        }
        public string Shipno
        {
            get { return shipno; }
            set { shipno = value; }
        }
        public string MethodName
        {
            get { return methodname; }
            set { methodname = value; }
        }
        public string ShipDate
        {
            get { return shipdate; }
            set { shipdate = value; }
        }
        public string InvoiceNo
        {
            get { return invoiceno; }
            set { invoiceno = value; }
        }
        public string InvoiceDate
        {
            get { return invoicedate; }
            set { invoicedate = value; }
        }
        public string ShapOrderNo
        {
            get { return shaporderno; }
            set { shaporderno = value; }
        }
        public string Shipment
        {
            get { return shipment; }
            set { shipment = value; }
        }

    }

    public class Partordinfo
    {
        Int32 id = 0;
        Int32 shipmethodid = 0;
        Int32 shipid = 0;
        Int32 attid = 0;
        Int32 qty = 0;
        Int32 balqty = 0;
        Int32 shipqty = 0;
        Int32 partid = 0;

        string description = string.Empty;
        string partno = string.Empty;
        string invoiceno = string.Empty;
        string invoicedate = string.Empty;
        string shipment = string.Empty;
        string methodname = string.Empty;
        string shipno = string.Empty;
        string shipdate = string.Empty;
        string shaporderno = string.Empty;
        public Int32 ShipMethodid
        {
            get { return shipmethodid; }
            set { shipmethodid = value; }
        }
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 SHIPID
        {
            get { return shipid; }
            set { shipid = value; }
        }
        public Int32 ATTID
        {
            get { return attid; }
            set { attid = value; }
        }
        public Int32 QTY
        {
            get { return qty; }
            set { qty = value; }
        }
        public Int32 BALQTY
        {
            get { return balqty; }
            set { balqty = value; }
        }
        public Int32 ShipQTY
        {
            get { return shipqty; }
            set { shipqty = value; }
        }
        public Int32 PartId
        {
            get { return partid; }
            set { partid = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string MethodName
        {
            get { return methodname; }
            set { methodname = value; }
        }
        public string ShipNo
        {
            get { return shipno; }
            set { shipno = value; }
        }
        public string PartNo
        {
            get { return partno; }
            set { partno = value; }
        }

        public string InvoiceNo
        {
            get { return invoiceno; }
            set { invoiceno = value; }
        }
        public string InvoiceDate
        {
            get { return invoicedate; }
            set { invoicedate = value; }
        }
        public string ShipDate
        {
            get { return shipdate; }
            set { shipdate = value; }
        }

        public string Shipment
        {
            get { return shipment; }
            set { shipment = value; }
        }
        public string ShapOrderNo
        {
            get { return shaporderno; }
            set { shaporderno = value; }
        }
    }

    public class TrackInfo
    {
        string error = string.Empty;
        string shipdate = string.Empty;
        string shipaddress = string.Empty;
        string originlocation = string.Empty;
        string signaturename = string.Empty;
        string actualdeladdress = string.Empty;
        string actualdellocation = string.Empty;
        string actualdate = string.Empty;
        string description = string.Empty;
        List<Config> config = new List<Config>();

        public List<Config> Config
        {
            get;
            set;
        }
        public string Error
        {
            get { return error; }
            set { error = value; }
        }
        public string ShipDate
        {
            get { return shipdate; }
            set { shipdate = value; }
        }
        public string ShipAddress
        {
            get { return shipaddress; }
            set { shipaddress = value; }
        }
        public string OriginLocation
        {
            get { return originlocation; }
            set { originlocation = value; }
        }
        public string SignatureName
        {
            get { return signaturename; }
            set { signaturename = value; }
        }
        public string ActualDelAddress
        {
            get { return actualdeladdress; }
            set { actualdeladdress = value; }
        }
        public string ActualDelLocation
        {
            get { return actualdellocation; }
            set { actualdellocation = value; }
        }
        public string ActualDate
        {
            get { return actualdate; }
            set { actualdate = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }

    public class Config
    {
        string date = string.Empty;
        string activity = string.Empty;
        string location = string.Empty;

        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public string Activity
        {
            get { return activity; }
            set { activity = value; }
        }
        public string Location
        {
            get { return location; }
            set { location = value; }
        }
    }

    public class Statistics
    {
        Int32 id = 0;
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
    }

    public class PartDatainfo
    {
        Int32 id = 0;
        Int32 orderid = 0;
        Int32 partid = 0;
        decimal price = 0;
        Int32 quantity = 0;
        decimal amount = 0;
        decimal taxamount = 0;

        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public Int32 OrderId
        {
            get { return orderid; }
            set { orderid = value; }
        }
        public Int32 PartId
        {
            get { return partid; }
            set { partid = value; }
        }
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }
        public Int32 Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public decimal TaxAmount
        {
            get { return taxamount; }
            set { taxamount = value; }
        }

    }

    public class InventoryListInfo
    {
        Int32 id = 0;
        string partno = string.Empty;
        string description = string.Empty;
        string UPLOADDATE = string.Empty;
        Int32 UploadedDays = 0;
        Int32 QTY = 0;
        string DEALERNAME = string.Empty;
        string CONTACTPERSON = string.Empty;
        string CONTACTNO = string.Empty;
        string Email = string.Empty;

        public Int32 Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Partno
        {
            get { return partno; }
            set { partno = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Uploaddate
        {
            get { return UPLOADDATE; }
            set { UPLOADDATE = value; }
        }
        public Int32 UPLOADDays
        {
            get { return UploadedDays; }
            set { UploadedDays = value; }
        }
        public Int32 Qty
        {
            get { return QTY; }
            set { QTY = value; }
        }
        public string DealerName
        {
            get { return DEALERNAME; }
            set { DEALERNAME = value; }
        }
        public string ContactPerson
        {
            get { return CONTACTPERSON; }
            set { CONTACTPERSON = value; }
        }
        public string ContactNo
        {
            get { return CONTACTNO; }
            set { CONTACTNO = value; }
        }
        public string EMAIL
        {
            get { return Email; }
            set { Email = value; }
        }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Distance { get; set; }
    }


    public class AlertInfo
    {
        string message = string.Empty;
        int active = 0;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public int Active
        {
            get { return active; }
            set { active = value; }
        }
    }


    public class InventryStatus
    {
        public Int32 DealerCount { get; set; }
        public Int32 PartCount { get; set; }
    }

    public class EmailStatus
    {
        string status = string.Empty;
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
    }

    public class CallTagStatus
    {
        string status = string.Empty;
        string message = string.Empty;
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }


    public class Partinfo
    {
        public string PartNo
        {
            get;
            set;
        }
    }


    public class ModelSearchInfo
    {
        int vehicleid = 0;
        int modelid = 0;
        int typeid = 0;
        string series = string.Empty;
        string modelname = string.Empty;
        string typename = string.Empty;

        public int VehicleId
        {
            get { return vehicleid; }
            set { vehicleid = value; }
        }
        public int ModelId
        {
            get { return modelid; }
            set { modelid = value; }
        }
        public int TypeId
        {
            get { return typeid; }
            set { typeid = value; }
        }
        public string Series
        {
            get { return series; }
            set { series = value; }
        }
        public string ModelName
        {
            get { return modelname; }
            set { modelname = value; }
        }
        public string TypeName
        {
            get { return typename; }
            set { typename = value; }
        }
    }

    public class AggregateInfo
    {
        int categoryid = 0;
        int id = 0;
        string categoryname = string.Empty;
        public int CategoryId
        {
            get { return categoryid; }
            set { categoryid = value; }
        }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string CategoryName
        {
            get { return categoryname; }
            set { categoryname = value; }
        }
    }

    public class CardInfo
    {
        int status = 0;
        string message = string.Empty;
        public Int32 Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class FeedbackTypeInfo
    {
        int id = 0;
        string name = string.Empty;
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

   # region get date

    public class GetDate
    {
        string date = string.Empty;
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
    }
# endregion

    # region AES 128 Key Value

    public class AesKey
    {
        string seed = string.Empty;
        public string Seed
        {
            get { return seed; }
            set { seed = value; }
        }
    }

    #endregion

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
