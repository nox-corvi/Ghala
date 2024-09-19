using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using NVWebAccessSvc;
using System.Security;
using Nox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Channels;

namespace NVWebAccess;

public class WebSvcConnect 
    : IDisposable
{
    public enum WacState
    {
        None,
        Created,
        Authenticated
    }

    private ILogger _logger;
    private IConfiguration _configuration;

    private string _Username = "";
    private SecureString _Password = new SecureString();

    private WebAccessContractClient _WacClient;
    private WacState _WacState = WacState.None;
    private string _WacAuthToken = "";

    #region Properties
    public ILogger Logger { get => _logger; }
    public IConfiguration Configuration { get => _configuration; }

    /// <summary>
    /// Gibt den verwendeten Mandanten zurück
    /// </summary>
    public string BusinessUnit { get; } = "";

    /// <summary>
    /// Gibt die verwendete Sprache zurück.
    /// </summary>
    public string Language { get; } = "";

    /// <summary>
    /// Gibt die verwendete Währung zurück oder legt Sie fest
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Beschänkt die Anzahl der Ergebnisse pro Seite
    /// </summary>
    public int PageSize { get; set; } = 200;

    /// <summary>
    /// Beschränkt die maximale Anzahl der Ergebnisse 
    /// </summary>
    public int MaxRowCount { get; set; } = 1000;

    /// <summary>
    /// Gibt den Authentifizierungstoken zurück
    /// </summary>
    public string AuthToken
    {
        get
        {
            return _WacAuthToken;
        }
    }

    public string Username { get => _Username; }
    #endregion

    /// <summary>
    /// Versucht sich mit dem WebService zu verbinden. Die Eigenschaft State zeigt abschließend den Status an.
    /// </summary>
    public void Connect()
    {
        switch (_WacState)
        {
            case WacState.None:
                BasicHttpBinding httpBinding = new BasicHttpBinding();
                httpBinding.MaxReceivedMessageSize = Int32.MaxValue;
                httpBinding.MaxBufferSize = Int32.MaxValue;
                httpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                

                _WacClient = new WebAccessContractClient(new BasicHttpBinding()
                {
                    MaxReceivedMessageSize = Int32.MaxValue,
                    MaxBufferSize = Int32.MaxValue,
                    Security = new BasicHttpSecurity()
                    {
                        Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                        Transport = new HttpTransportSecurity()
                        {
                            ClientCredentialType = HttpClientCredentialType.None
                        }
                    },
                }, 
                new EndpointAddress(_configuration["nvwebaccess:url"] ?? throw new ArgumentNullException("url must not be null")));

                _WacState = WacState.Created;

                // next iteration
                Connect();

                break;
            case WacState.Created:
                string wcTmpAuthToken = _WacClient.Authenticate(_Username, _Password.ToString(), BusinessUnit, Language);
                if (wcTmpAuthToken != "")
                {
                    _WacAuthToken = wcTmpAuthToken;
                    _WacState = WacState.Authenticated;
                }

                // be correct, next iteration
                Connect();

                break;
            case WacState.Authenticated:
                // well, *thumbsup*
                break;
            default:
                throw new ArgumentOutOfRangeException("Status liefert einen unerwarteten Wert zurück");
        }
    }

    /// <summary>
    /// Schließt die Verbindung. Die Eigenschaft State zeigt abschließend den Status an.
    /// </summary>
    public void Close()
    {
        switch (_WacState)
        {
            case WacState.Authenticated:
                try
                {
                    _WacClient?.Close();
                }
                finally
                {
                    _WacState = WacState.None;
                }
                break;
            case WacState.Created:
                _WacState = WacState.None;

                break;
        }

        _WacAuthToken = "";
    }

    #region Executes
    public bool exec_NextEDIImportNow(string sJobSchedId)
    {
        Connect();
        return _WacClient.K78_NextEDIImportNow(_WacAuthToken, sJobSchedId);
    }
    #endregion

    #region Queries
    internal dcArticle GetArticleByID(string ArticleId)
    {
        Connect();
        return _WacClient.GetArticleByID(_WacAuthToken, ArticleId);
    }

    internal List<dcArticle> GetAllArticles(int PageIndex, int PageSize = -1)
    {
        Connect();
        return _WacClient.GetAllArticles(_WacAuthToken, PageIndex, PageSize == -1 ? this.PageSize : PageSize, 0);
    }

    internal List<dcArticle> GetArticlesByGroupId(string GroupId, int PageIndex, int PageSize = -1)
    {
        Connect();
        return _WacClient.GetArticlesByArticleGroupID(_WacAuthToken, GroupId, PageIndex, PageSize == -1 ? this.PageSize : PageSize, 0);
    }

    public dcSalesMaster GetSalesOfferByADABRef(string ADABRef)
    {
        Connect();
        return _WacClient.K78_GetSalesOfferByADABRef(_WacAuthToken, ADABRef);
    }

    internal dcCustomer GetCustomerById(int CustomerId)
    {
        Connect();
        return _WacClient.GetCustomerByID(_WacAuthToken, CustomerId);
    }

    internal dcK78_CompanyNewsList GetCompanyNewsByTopicality(int TopicalityInDays)
    {
        Connect();
        return _WacClient.K78_GetNewsByTopicality(_WacAuthToken, TopicalityInDays);
    }

    internal dcPrice GetPrice(long CustomerId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity)
    {
        var Time = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            Connect();
            return _WacClient.K78_GetPriceQuery(_WacAuthToken, new dcPriceQuery()
            {
                lngCustomerID = CustomerId,
                sArticleID = ArticleId,
                decK78_DefinableAttribute1Value = definableAttribute1, 
                decK78_DefinableAttribute2Value = definableAttribute2,
                decQuantity = Quantity
            });
        }
        finally
        {
           // Global.Logger.LogDebug($"NVTime? NVGetPrice: {Time.ElapsedMilliseconds.ToString()}ms", Log4.Log4LevelEnum.Debug);
        }
    }

    internal dcPriceInfo GetPriceInfo(long CustomerId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity)
    {
        /* 2017-03-21 OG, Änderung von GetPriceInfo -> GetPriceInfoQuery
         * Bei VK Pro > 1, wird der Preis für die Menge [VKPro], und nicht Pro 1 gezogen, 
         * daher wird die Menge über ein QueryObject übergeben ...
         */
        Connect();
        return _WacClient.K78_GetPriceInfoQuery(_WacAuthToken, new dcPriceQuery()
        {
            lngCustomerID = CustomerId,
            sArticleID = ArticleId,
            decK78_DefinableAttribute1Value = definableAttribute1,
            decK78_DefinableAttribute2Value = definableAttribute2, 
            decQuantity = Quantity
        });
    }
    public dcK78_StockInfo GetStockInfo(string ArticleId, decimal definableAttribute1, decimal definableAttribute2, short Storage)
    {
        Connect();
        return _WacClient.K78_GetStockInfo(_WacAuthToken, ArticleId, String.Empty, Storage, String.Empty, DateTime.Now, definableAttribute1, definableAttribute2);
    }

    public dcK78_StockInfo GetAvailability(string ArticleId, decimal definableAttribute1, decimal definableAttribute2, short Storage)
    {
        Connect();
        return _WacClient.K78_GetStockInfo(_WacAuthToken, ArticleId, String.Empty, Storage, String.Empty, DateTime.Now, definableAttribute1, definableAttribute2);
    }

    public dcK78_ExternalStockInfo GetExternalAvailability(string ArticleId, decimal Quantity)
    {
        Connect();
        return _WacClient.K78_GetExternalStockInfo(_WacAuthToken, ArticleId, Quantity);
    }

    internal List<dcK78_StockInfo> GetStockInfo(string ArticleId, decimal definableAttribute1, decimal definableAttribute2, List<short> Storages)
    {
        Connect();
        var Result = new List<dcK78_StockInfo>();
        foreach (var Item in Storages)
            Result.Add(_WacClient.K78_GetStockInfo(_WacAuthToken, ArticleId, String.Empty, Item, String.Empty, DateTime.Now, definableAttribute1, definableAttribute2));

        return Result;
    }

    internal dcContact GetContactById(long ContactId)
    {
        Connect();
        return _WacClient.GetContactByID(_WacAuthToken, ContactId);
    }

    internal dcContactList GetContactByCustomer(long CustomerId)
    {
        Connect();
        return _WacClient.GetContactsByCustomerID(_WacAuthToken, CustomerId, 0, 0, 0);
    }

    internal dcContact CreateContact(int CustomerId, string FormOfAddress, string LastName, string FirstName)
    {
        Connect();
        return _WacClient.CreateContact(_WacAuthToken, CustomerId, FormOfAddress, FirstName, LastName);
    }

    internal dcContact SaveContact(int CustomerId, dcContact oContact)
    {
        Connect();
        return _WacClient.SaveContact(_WacAuthToken, oContact);
    }

    internal dcPayConnectionList GetPayConnectionsByCustomer(long CustomerId)
    {
        Connect();
        return _WacClient.GetPayConnectionsByCustomerID(_WacAuthToken, CustomerId);
    }

    internal dcSalesMaster GetSalesOrderById(string OrderId)
    {
        Connect();
        return _WacClient.GetSalesOrderByID(_WacAuthToken, long.Parse(OrderId));
    }

    internal List<dcSalesMaster> GetSalesOrdersByCustomerId(int CustomerId, int Days, int PageIndex, int PageSize = -1)
    {
        Connect();
        return _WacClient.GetSalesOrdersByCustomerID(_WacAuthToken, (long)CustomerId, Days, PageIndex, PageSize == -1 ? this.PageSize : PageSize, MaxRowCount);
    }

    public dcCart CreateCart(int WebShopId, int CustomerId, string Username, string ExternalCartId)
    {
        Connect();
        return _WacClient.CreateCart(_WacAuthToken, (short)WebShopId, CustomerId, _Username, ExternalCartId);
    }

    public dcCart ChangeCartOrderText(int CartId, string OrderText)
    {
        Connect();
        return _WacClient.K78_ChangeCartOrderText(_WacAuthToken, CartId, OrderText);
    }

    public dcCart ChangeCartMessageToMerchant(int CartId, string MessageToMerchant)
    {
        Connect();
        return _WacClient.K78_ChangeCartMessageToMerchant(_WacAuthToken, CartId, MessageToMerchant);
    }

    public dcCart ChangeCartCostCenterNo(int CartId, string CostCenterNo)
    {
        Connect();
        return _WacClient.K78_ChangeCartCostCenterNo(_WacAuthToken, CartId, CostCenterNo);
    }

    public dcCart ChangeCartContact(int CartId, long ContactId)
    {
        Connect();
        return _WacClient.K78_ChangeCartContact(_WacAuthToken, CartId, ContactId);
    }

    public dcCart ChangeCartIgnoreMasterDiscount(int CartId, bool IgnoreMasterDiscount)
    {
        Connect();
        return _WacClient.K78_ChangeCartIgnoreMasterDiscount(_WacAuthToken, CartId, IgnoreMasterDiscount);
    }

    public dcCart SubmitCart(int WebShopId, int CartId, string Username)
    {
        Connect();
        return _WacClient.SubmitCart(_WacAuthToken, (int)CartId, Username);
    }

    public dcCart SaveCart(dcCart Cart)
    {
        Connect();
        return _WacClient.SaveCart(_WacAuthToken, Cart);
    }

    public dcCart ChangeCartDetailAddDiscountAmount(int CartId, int ItemId, string DiscountText, decimal DiscountAmount)
    {
        Connect();
        return _WacClient.ChangeCartDetailAddDiscountAmount(_WacAuthToken, (int)CartId, (short)ItemId, DiscountText, DiscountAmount);
    }

    public dcCart ChangeCartDetailAddDiscountPercent(int CartId, int ItemId, string DiscountText, decimal DiscountAmount)
    {
        Connect();
        return _WacClient.ChangeCartDetailAddDiscountPercent(_WacAuthToken, (int)CartId, (short)ItemId, DiscountText, DiscountAmount);
    }

    public dcCart ChangeCartDeliveryAddress(int CartId, string Company1, string Company2, string Company3, string Company4, string Street, string ZipCode, string City, string CountryCode, string Country)
    {
        Connect();
        return _WacClient.ChangeCartDeliveryAddress(_WacAuthToken, (int)CartId, Company1, Company2, Company3, Company4, Street, ZipCode, City, CountryCode, Country);
    }

    public dcCart ChangeCartPaymentCondition(int CartId, int PaymentTypeId, int PaymentConditionId)
    {
        Connect();
        return _WacClient.ChangeCartPaymentCondition(_WacAuthToken, (int)CartId, (short)PaymentTypeId, (short)PaymentConditionId);
    }

    public dcCart ChangeCartShippingCondition(int CartId, int ShippingConditionId, decimal ShippingCosts)
    {
        Connect();
        return _WacClient.ChangeCartShippingCondition(_WacAuthToken, (int)CartId, (short)ShippingConditionId, ShippingCosts);
    }

    public dcCart ChangeCartInvoiceAddress(int CartId, string Company1, string Company2, string Company3, string Company4, string Street, string ZipCode, string City, string CountryCode, string Country)
    {
        Connect();
        return _WacClient.ChangeCartInvoiceAddress(_WacAuthToken, (int)CartId, Company1, Company2, Company3, Company4, Street, ZipCode, City, CountryCode, Country);
    }

    public dcCart CreateCartDetailFromArticle(int CartId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity, string QuantityUnit, string ExternalItemId)
    {
        Connect();
        return _WacClient.K78_CreateCartDetailFromArticle(_WacAuthToken, (int)CartId, ArticleId, Quantity, QuantityUnit, ExternalItemId,definableAttribute1, definableAttribute2);
    }

    //public dcCart ChangeCartDetailAddDiscountAmount(int CartId, int ItemId, string DiscountText, decimal DiscountAmount)
    //{
    //    Connect();
    //    return _WacClient.ChangeCartDetailAddDiscountAmount(_WacAuthToken, (int)CartId, (short)ItemId, DiscountText, DiscountAmount);
    //}

    //public dcCart ChangeCartDetailAddDiscountPercent(int CartId, int ItemId, string DiscountText, decimal DiscountPercent)
    //{
    //    Connect();
    //    return _WacClient.ChangeCartDetailAddDiscountPercent(_WacAuthToken, (int)CartId, (short)ItemId, DiscountText, DiscountPercent);
    //}

    public dcCart ChangeCartDetailQuantity(int CartId, int ItemId, decimal Quantity)
    {
        Connect();
        return _WacClient.ChangeCartDetailQuantity(_WacAuthToken, (int)CartId, (short)ItemId, Quantity);
    }

    public dcCart ChangeCartDetailQuantityUnit(int CartId, int ItemId, string QuantityUnit)
    {
        Connect();
        return _WacClient.ChangeCartDetailQuantityUnit(_WacAuthToken, (int)CartId, (short)ItemId, QuantityUnit);
    }

    public dcCart ChangeCartDetailText(int CartId, int ItemId, string Text)
    {
        Connect();
        return _WacClient.ChangeCartDetailText(_WacAuthToken, (int)CartId, (short)ItemId, Text);
    }

    public dcCart SetCartDiscount1(int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
    {
        Connect();
        return _WacClient.K78_SetCartDiscount1(_WacAuthToken, (int)CartId, DiscountText, DiscountPercent, DiscountAmount);
    }

    public dcCart SetCartDiscount2(int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
    {
        Connect();
        return _WacClient.K78_SetCartDiscount2(_WacAuthToken, (int)CartId, DiscountText, DiscountPercent, DiscountAmount);
    }

    public dcCart SetCartDiscount3(int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
    {
        Connect();
        return _WacClient.K78_SetCartDiscount3(_WacAuthToken, (int)CartId, DiscountText, DiscountPercent, DiscountAmount);
    }

    public dcSalesDocument GetSalesDocumentById(int DocumentId, string DocumentType)
    {
        Connect();
        return _WacClient.GetSalesDocumentByID(_WacAuthToken, DocumentType, DocumentId);
    }

    public dcDocument GetPrintedSalesDocument(dcSalesDocument SalesDocument)
    {
        Connect();
        return _WacClient.GetPrintedSalesDocument(_WacAuthToken, SalesDocument);
    }

    public dcDocument GetArchiveDocumentById(string ReferenceId)
    {
        Connect();
        return _WacClient.GetArchiveDocumentByID(_WacAuthToken, ReferenceId);
    }

    public dcCart DeleteCartDetail(int CartId, int ItemId)
    {
        Connect();
        return _WacClient.DeleteCartDetail(_WacAuthToken, (int)CartId, (short)ItemId);
    }

    public dcCart SetPriceToCartDetail(int CartId, dcCartDetail Detail, dcPrice Price)
    {
        Connect();
        return _WacClient.SetPriceToCartDetail(_WacAuthToken, Detail, Price);
    }

    public dcUser CreateUser(int WebShopId, int CustomerId, int ContactId, string Email, string Username, string Password)
    {
        Connect();
        return _WacClient.CreateUser(_WacAuthToken, (short)WebShopId, CustomerId, ContactId, Email, Username, Password);
    }

    public dcUser DeleteUser(dcUser o)
    {
        Connect();
        return _WacClient.DeleteUser(_WacAuthToken, o);
    }

    public dcUser GetUserByEmail(int WebShopId, string Email)
    {
        Connect();
        return _WacClient.GetUserByEmail(_WacAuthToken, (short)WebShopId, Email);
    }

    public dcUser GetUserByName(int WebShopId, string Name)
    {
        Connect();
        return _WacClient.GetUserByName(_WacAuthToken, (short)WebShopId, Name);
    }

    public dcUser GetUserByAccount(int WebShopId, string Account)
    {
        Connect();
        return _WacClient.GetUserByName(_WacAuthToken, (short)WebShopId, Account);
    }

    public dcUserList GetUsersByCustomerId(int WebShopId, int CustomerId)
    {
        Connect();
        return _WacClient.GetUsersByCustomerID(_WacAuthToken, (short)WebShopId, CustomerId);
    }

    public dcUser SaveUser(dcUser o)
    {
        Connect();
        return _WacClient.SaveUser(_WacAuthToken, o);
    }

    public bool ValidateUser(int WebShopId, string Username, string Password)
    {
        Connect();
        return _WacClient.ValidateUser(_WacAuthToken, (short)WebShopId, Username, Password);
    }
    #endregion

    public WebSvcConnect()
        : this(Global.Logger, Global.Configuration)
    {

    }

    public WebSvcConnect(ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _Username = _configuration["nvwebaccess:username"] ?? "";

        string pass = _configuration["nvwebaccess:password"] ?? "";
        foreach (var Item in pass)
            _Password.AppendChar(Item);

        BusinessUnit = _configuration["nvwebaccess:businessunit"] ?? "";
        Language = _configuration["nvwebaccess:language"] ?? "";
    }

    public void Dispose()
    {
        Close();
        ((IDisposable)_WacClient)?.Dispose();
    }
}
