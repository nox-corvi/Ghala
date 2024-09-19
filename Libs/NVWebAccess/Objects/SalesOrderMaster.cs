using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nox;
using System.Data;
using System.Data.SqlClient;
using static Nox.Helpers;
using NVWebAccessSvc;
using Nox.Data.SqlServer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace NVWebAccess
{
    public class SalesOrderMaster 
        : WebSvcResponseShell<SalesOrderMasterData>
    {
        public override SalesOrderMasterData Data { get; set; }

        public const string SQL_SALES_ORDER_HEAD = 
            @"SELECT
                k.BelegNr as OrderId, k.KundenNr as CustomerId, 
                k.ErfassungsDatum as EntryDate, k.BestellDatum as OrderDate, k.LieferDatum as DeliveryDate, 
                k.BelegArt as OrderType, k.Bestellung as OrderText, 
                k.Sperrung as Blocked, k.AngebotStatus as OfferStatus, k.ZBnummer as PaymentCondition, k.VAnummer as ShippingCondition,                
                k.lFirma1 as DeliveryCompany1, k.lFirma2 as DeliveryCompany2, k.lStrasse as DeliveryStreet, k.lPlz as DeliveryZipCode, k.lOrt as DeliveryCity, 
                k.lLaenderKz as DeliveryCountryCode, k.lLand as DeliveryCountry, k.lKontakt as DeliveryContact, 
    
                k.rKontakt as InvoiceContact,
                k.rFirma1 as InvoiceCompany1, k.rFirma2 as InvoiceCompany2, k.rStrasse as InvoiceStreet, k.rPlzPF as InvoiceZipBox, k.rPostfach as InvoicePostBox,
                k.rPlz as InvoiceZipCode, k.rOrt as InvoiceCity, 
                k.rLaenderKz as InvoiceCountryCode, k.rLand as InvoiceCountry, k.rKontakt as Purchaser
            FROM
                AuftragsKopf k
            WHERE ";
        public const string SQL_SALES_ORDER_POS =
            @"SELECT
                p.BelegNr as OrderId, p.BelegArt as OrderType,
                p.RechnungsNr as InvoiceId, p.FakturaDatum as InvoiceDate, p.LieferDatum as DeliveryDate, p.Wunschdatum as RequestedDate, 
                p.AlternativeItem, p.FaktorVk as FactorSalesPrice, p.PreisFaktor as PriceFactor, 
                p.Vk as Price, p.Menge_Geliefert as QuantityDelivered, p.Menge_Bestellt as QuantityOrdered, 
    
    

                p.SupplierId, p.ArtikelNr as ArticleId, p.Bezeichnung as ArticleName, a.K78_ARBEZ as ArticleShortDescription, p.fArtikelNr as CustomerArticleId,  
                p.PositionsNr as ItemId, p.FixPosNr as FixedItemId, p.CustFixedItemId as CustomerFixedItemId, p.PositionsArt as ItemType, 
                p.CustOrderId as CustomerOrderId, p.LieferscheinNr as DeliveryNoteId, p.BonNr as GoodsReceiptId,p.PackListenNr as PackingListId, 
                p.LagerNr as StorageId, p.PosText as ItemTExt, p.Steuerschluessel as TaxCode,
                p.VkPro as SalesPriceUnit, p.EinheitVk as QuantityUnit, p.Status, p.K78_EnhancedPosStatus as EnhancedPosStatus
            FROM
                AuftragsPos p 
            LEFT JOIN
                Artikel a on 
                a.ArtikelNr = p.ArtikelNr and a.BranchKey = p.BranchKey 
            WHERE ";

        public static SalesOrderMaster GetSalesOrderById(WebSvcConnect svc, string OrderId)
        {
            try
            {
                // enventa websvc call
                var nuvSalesMaster = svc.GetSalesOrderById(OrderId);
                if (nuvSalesMaster.Status == 1)
                    return new SalesOrderMaster()
                    {
                        State = WebSvcResult.Ok,
                        Data = SalesOrderMasterData.FromDC(nuvSalesMaster)
                    };
                else
                    return new SalesOrderMaster()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvSalesMaster.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new SalesOrderMaster()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static SalesOrderMaster GetDbDirectSalesOrderById(WebSvcConnect svc, string OrderId)
        {
            try
            {
                using (var dbA = new SqlDbAccess(Global.Configuration.GetConnectionString("enventa") ?? throw new ArgumentNullException("connection string must not be null")))
                {
                    using (var h = dbA.GetReader(SQL_SALES_ORDER_HEAD + "k.belegnr = @belegnr and k.branchkey = @branchkey",
                        new SqlParameter("belegnr", OrderId),
                        new SqlParameter("branchkey", svc.BusinessUnit)))
                    {
                        if (h.Read())
                        {
                            var Head = SalesOrderMasterData.FromDB(h);

                            using (var p = dbA.GetReader(SQL_SALES_ORDER_POS + "p.belegnr = @belegnr and p.branchkey = @branchkey",
                                new SqlParameter("belegnr", OrderId),
                                new SqlParameter("branchkey", svc.BusinessUnit)))
                                while (p.Read())
                                    Head.Positions.Add(SalesOrderDetailData.FromDB(p));

                            return new SalesOrderMaster()
                            {
                                State = WebSvcResult.Ok,
                                Data = Head
                            };
                        }
                        else
                            return new SalesOrderMaster()
                            {
                                State = WebSvcResult.NoResult,
                                Message = "",
                            };
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new SalesOrderMaster()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static List<SalesOrderMaster> GetSalesOrdersByCustomerId(WebSvcConnect svc, int CustomerId, int Days)
        {
            try
            {
                // enventa websvc call
                var Result = new List<SalesOrderMaster>(); int PageIndex = 0;

                var nuvSalesMaster = svc.GetSalesOrdersByCustomerId(CustomerId, Days, PageIndex, svc.PageSize);
                while (nuvSalesMaster.Count != 0)
                {
                    foreach (var nuvItem in nuvSalesMaster)
                        if (nuvItem.Status != 1)
                            Result.Add(new SalesOrderMaster()
                            {
                                State = WebSvcResult.Ok,
                                Data = SalesOrderMasterData.FromDC(nuvItem)
                            });
                        else
                            Result.Add(new SalesOrderMaster()
                            {
                                State = WebSvcResult.NoResult,
                                Message = Helper.nuvMessageToString(nuvItem.Messages),
                            });

                    nuvSalesMaster = svc.GetSalesOrdersByCustomerId(CustomerId, Days, ++PageIndex, svc.PageSize);
                }

                return Result;
            }
            catch (Exception ex)
            {
                //Log.LogWebError(ex);
                return new List<SalesOrderMaster>()
                {
                    new SalesOrderMaster()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public List<SalesOrderMaster> GetDbDirectSalesOrdersByCustomerId(WebSvcConnect svc, int CustomerId, int Days)
        {
            try
            {
                // enventa websvc call
                var Result = new List<SalesOrderMaster>();

                using (var dbA = new SqlDbAccess(Global.Configuration.GetConnectionString("enventa") ?? ""))
                {
                    using (var h = dbA.GetReader(SQL_SALES_ORDER_HEAD + $"k.kundennr = @customerid and k.branchkey = @branchkey and k.belegart < 'A' and ((DATEADD(day, {Days}, k.Erfassungsdatum)) >= sysdatetime())",
                        new SqlParameter("customerid", CustomerId),
                        new SqlParameter("branchkey", svc.BusinessUnit)))
                        while (h.Read())
                            Result.Add(new SalesOrderMaster()
                            {
                                State = WebSvcResult.Ok,
                                Data = SalesOrderMasterData.FromDB(h)
                            });
                    foreach (var Item in Result)
                        using (var p = dbA.GetReader(SQL_SALES_ORDER_POS + "p.belegnr = @orderid and p.belegart = @ordertype and p.branchkey = @branchkey",
                            new SqlParameter("orderid", Item.Data.OrderId),
                            new SqlParameter("ordertype", Item.Data.OrderType),
                            new SqlParameter("branchkey", svc.BusinessUnit)))
                            while (p.Read())
                                Item.Data.Positions.Add(SalesOrderDetailData.FromDB(p));
                }

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<SalesOrderMaster>()
                {
                    new SalesOrderMaster()
                    {
                        State = WebSvcResult.Error,
                                    Message = SerializeException(ex)
                    }
                };
            }
        }

        public List<SalesOrderMaster> GetSalesOrdersByCustomerIdPaged(WebSvcConnect svc, int CustomerId, int Days, int PageIndex, int PageSize)
        {
            try
            {
                // enventa websvc call
                var Result = new List<SalesOrderMaster>();

                var nuvSalesMaster = svc.GetSalesOrdersByCustomerId(CustomerId, Days, PageIndex, PageSize);
                foreach (var nuvItem in nuvSalesMaster)
                    if (nuvItem.Status != 1)
                        Result.Add(new SalesOrderMaster()
                        {
                            State = WebSvcResult.Ok,
                            Data = SalesOrderMasterData.FromDC(nuvItem)
                        });
                    else
                        Result.Add(new SalesOrderMaster()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvItem.Messages),
                        });

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<SalesOrderMaster>()
                {
                    new SalesOrderMaster()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public SalesOrderMaster()
            : base(Global.Logger, Global.Configuration)
        {
            
        }
    }

    public class SalesOrderMasterData 
        : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Lieferdatum
        /// </summary>
        public DateTime DeliveryDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Erfassungsdatum
        /// </summary>
        public DateTime EntryDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Bestelldatum
        /// </summary>
        public DateTime? OrderDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// KundenNr
        /// </summary>
        public int CustomerId { get; set; } = -1;

        /// <summary>
        /// Auftragsnummer
        /// </summary>
        public string OrderId { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Stadt
        /// </summary>
        public string DeliveryCity { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Firma 1
        /// </summary>
        public string DeliveryCompany1 { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Firma 2
        /// </summary>
        public string DeliveryCompany2 { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Ansprechpartner
        /// </summary>
        public string DeliveryContact { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Land
        /// </summary>
        public string DeliveryCountry { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Ländercode
        /// </summary>
        public string DeliveryCountryCode { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Straße
        /// </summary>
        public string DeliveryStreet { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Postleitzahl
        /// </summary>
        public string DeliveryZipCode { get; set; } = "";

        /// <summary>
        /// Gesperrt
        /// </summary>
        public bool Blocked { get; set; } = false;

        /// <summary>
        /// Angebotsstatus
        /// </summary>
        public int OfferStatus { get; set; } = -1;

        /// <summary>
        /// Zahlungsbedingung
        /// </summary>
        public int PaymentCondition { get; set; } = -1;

        /// <summary>
        /// Lieferbedingung
        /// </summary>
        public int ShippingCondition { get; set; } = -1;

        /// <summary>
        /// Rechnungsanschrift - Stadt
        /// </summary>
        public string InvoiceCity { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Firma 1
        /// </summary>
        public string InvoiceCompany1 { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Firma 2
        /// </summary>
        public string InvoiceCompany2 { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Ansprechpartner
        /// </summary>
        public string InvoiceContact { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Land
        /// </summary>
        public string InvoiceCountry { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Ländercode
        /// </summary>
        public string InvoiceCountryCode { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Postfach
        /// </summary>
        public string InvoicePostBox { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Straße
        /// </summary>
        public string InvoiceStreet { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Postleitzahl Postfach
        /// </summary>
        public string InvoiceZipBox { get; set; } = "";

        /// <summary>
        /// Rechnungsanschrift - Postleitzahl
        /// </summary>
        public string InvoiceZipCode { get; set; } = "";

        /// <summary>
        /// Bestelltext
        /// </summary>
        public string OrderText { get; set; } = "";

        /// <summary>
        /// Vorgangsart
        /// </summary>
        public string OrderType { get; set; } = "";

        /// <summary>
        /// Besteller
        /// </summary>
        public string Purchaser { get; set; } = "";

        /// <summary>
        /// Auftragsstatus
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// Auftragsstatus Text
        /// </summary>
        public string StatusText { get; set; } = "";

        /// <summary>
        /// Positionen
        /// </summary>
        public List<SalesOrderDetailData> Positions { get; set; } = new List<SalesOrderDetailData>();

        public static SalesOrderMasterData FromDC(dcSalesMaster nuvSalesMaster)
        {
            return new SalesOrderMasterData()
            {
                DeliveryDate = nuvSalesMaster.dtDeliveryDate.GetValueOrDefault(),
                EntryDate = nuvSalesMaster.dtEntryDate.GetValueOrDefault(),
                OrderDate = nuvSalesMaster.dtOrderDate.GetValueOrDefault(),
                CustomerId = (int)nuvSalesMaster.lngCustomerID,
                OrderId = nuvSalesMaster.lngOrderID.ToString(),
                DeliveryCity = NZ(nuvSalesMaster.sDCity),
                DeliveryCompany1 = NZ(nuvSalesMaster.sDCompany1),
                DeliveryCompany2 = NZ(nuvSalesMaster.sDCompany2),
                DeliveryContact = NZ(nuvSalesMaster.sDContact),
                DeliveryCountry = NZ(nuvSalesMaster.sDCountryCode),
                DeliveryCountryCode = NZ(nuvSalesMaster.sDCountryCode),
                DeliveryStreet = NZ(nuvSalesMaster.sDStreet),
                DeliveryZipCode = NZ(nuvSalesMaster.sDZipCode),
                Blocked = (bool)(nuvSalesMaster.shtBlocked.GetValueOrDefault(0) == 1),
                OfferStatus = nuvSalesMaster.shtOfferStatus.GetValueOrDefault(0),
                PaymentCondition = nuvSalesMaster.shtPaymentCondition.GetValueOrDefault(0),
                ShippingCondition = nuvSalesMaster.shtShippingCondition.GetValueOrDefault(0),
                InvoiceCity = NZ(nuvSalesMaster.sICity),
                InvoiceCompany1 = NZ(nuvSalesMaster.sICompany1),
                InvoiceCompany2 = NZ(nuvSalesMaster.sICompany2),
                InvoiceContact = NZ(nuvSalesMaster.sIContact),
                InvoiceCountry = NZ(nuvSalesMaster.sICountry),
                InvoiceCountryCode = NZ(nuvSalesMaster.sICountryCode),
                InvoicePostBox = NZ(nuvSalesMaster.sIPostBox),
                InvoiceStreet = NZ(nuvSalesMaster.sIStreet),
                InvoiceZipBox = NZ(nuvSalesMaster.sIZipBox),
                InvoiceZipCode = NZ(nuvSalesMaster.sIZipCode),
                OrderText = NZ(nuvSalesMaster.sOrderText),
                OrderType = NZ(nuvSalesMaster.sOrderType),
                Purchaser = NZ(nuvSalesMaster.sIContact),
                Status = 0,
                StatusText = "",

                Positions = SalesOrderDetailData.FromDC(nuvSalesMaster.oDetailList),
            };
        }

        public static SalesOrderMasterData FromDB(SqlDataReader R)
        {
            return new SalesOrderMasterData()
            {
                OrderId = R.GetInt64(R.GetOrdinal("OrderId")).ToString(),
                CustomerId = R.GetInt32(R.GetOrdinal("CustomerId")),
                EntryDate = N<DateTime>(R.GetValue(R.GetOrdinal("EntryDate")), DateTime.MinValue),
                OrderDate = N<DateTime>(R.GetValue(R.GetOrdinal("OrderDate")), DateTime.MinValue),
                DeliveryDate = N<DateTime>(R.GetValue(R.GetOrdinal("DeliveryDate")), DateTime.MinValue),
                OrderType = NZ(R.GetValue(R.GetOrdinal("OrderType"))),
                OrderText = NZ(R.GetValue(R.GetOrdinal("OrderText"))),

                Blocked = (bool)(N<int>(R.GetValue(R.GetOrdinal("Blocked")), 0) == 1),
                OfferStatus = N<int>(R.GetValue(R.GetOrdinal("OfferStatus")), 0),
                PaymentCondition = N<int>(R.GetValue(R.GetOrdinal("PaymentCondition")), 0),
                ShippingCondition = N<int>(R.GetValue(R.GetOrdinal("ShippingCondition")), 0),

                DeliveryCompany1 = NZ(R.GetValue(R.GetOrdinal("DeliveryCompany1"))),
                DeliveryCompany2 = NZ(R.GetValue(R.GetOrdinal("DeliveryCompany2"))),
                DeliveryStreet = NZ(R.GetValue(R.GetOrdinal("DeliveryStreet"))),
                DeliveryZipCode = NZ(R.GetValue(R.GetOrdinal("DeliveryZipCode"))),
                DeliveryCity = NZ(R.GetValue(R.GetOrdinal("DeliveryCity"))),
                DeliveryCountryCode = NZ(R.GetValue(R.GetOrdinal("DeliveryCountryCode"))),
                DeliveryCountry = NZ(R.GetValue(R.GetOrdinal("DeliveryCountry"))),
                DeliveryContact = NZ(R.GetValue(R.GetOrdinal("DeliveryContact"))),

                
                InvoiceCompany1 = NZ(R.GetValue(R.GetOrdinal("InvoiceCompany1"))),
                InvoiceCompany2 = NZ(R.GetValue(R.GetOrdinal("InvoiceCompany2"))),
                InvoiceContact = NZ(R.GetValue(R.GetOrdinal("InvoiceContact"))),                
                InvoiceStreet = NZ(R.GetValue(R.GetOrdinal("InvoiceStreet"))),
                InvoiceZipBox = NZ(R.GetValue(R.GetOrdinal("InvoiceZipBox"))),
                InvoicePostBox = NZ(R.GetValue(R.GetOrdinal("InvoicePostBox"))),
                InvoiceZipCode = NZ(R.GetValue(R.GetOrdinal("InvoiceZipCode"))),
                InvoiceCity = NZ(R.GetString(R.GetOrdinal("InvoiceCity"))),
                InvoiceCountryCode = NZ(R.GetValue(R.GetOrdinal("InvoiceCountryCode"))),
                InvoiceCountry = NZ(R.GetValue(R.GetOrdinal("InvoiceCountry"))),

                Purchaser = NZ(R.GetValue(R.GetOrdinal("Purchaser"))),
                Status = 0,
                StatusText = "",

                Positions = new List<SalesOrderDetailData>() { SalesOrderDetailData.FromDB(R) },
            };
        }
    }

    public class SalesOrderDetailData
    {
        /// <summary>
        /// Alternativposition
        /// </summary>
        [WebSvcAutoProcess()]
        public bool AlternativeItem { get; set; } = false;

        /// <summary>
        /// Teillieferung
        /// </summary>
        [WebSvcAutoProcess()]
        public bool PartialDelivery { get; set; } = false;

        /// <summary>
        /// Verhältnis Lager / Verkaufseinheit
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal FactorSalesPrice { get; set; } = 0;

        /// <summary>
        /// Positionsbetrag inkl. MwSt in Belegwährung
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal ItemGrossAmount { get; set; } = 0;

        /// <summary>
        /// Positionsbetrag excl. MwSt in Belegwährung
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal ItemNetAmount { get; set; } = 0;

        /// <summary>
        /// Steuerbetrag der Position
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal ItemTaxAmount { get; set; } = 0;

        /// <summary>
        /// Preisverhältnis der Verkaufseinheit zur Standardverkaufseinheit
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal PriceFactor { get; set; } = 0;

        /// <summary>
        /// Einzelpreis ohne Rabatte / Zuschläge
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Gelieferte Menge
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal QuantityDelivered { get; set; } = 0;

        /// <summary>
        /// Bestellte Menge
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal QuantityOrdered { get; set; } = 0;

        /// <summary>
        /// Steuersatz
        /// </summary>
        [WebSvcAutoProcess()]
        public decimal TaxRate { get; set; } = 0;

        /// <summary>
        /// Rechnungsdatum
        /// </summary>
        [WebSvcAutoProcess(StringTransform = "yyyyMMdd")]
        public DateTime InvoiceDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Lieferdatum
        /// </summary>
        [WebSvcAutoProcess(StringTransform = "yyyyMMdd")]
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Wunschlieferdatum
        /// </summary>
        [WebSvcAutoProcess(StringTransform = "yyyyMMdd")]
        public DateTime? RequestedDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Kundenbestellnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string CustomerOrderId { get; set; } = "";

        /// <summary>
        /// Lieferscheinnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string DeliveryNoteId { get; set; } = "";

        /// <summary>
        /// Wareneingangsnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string GoodsReceiptId { get; set; } = "";

        /// <summary>
        /// Rechnungsnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string InvoiceId { get; set; } = "";

        /// <summary>
        /// Belegnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string OrderId { get; set; } = "";

        /// <summary>
        /// Packlistennummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string PackingListId { get; set; } = "";

        /// <summary>
        /// Menge, für die der Verkaufspreis gilt (VK pro)
        /// </summary>
        [WebSvcAutoProcess()]
        public int SalesPriceUnit { get; set; } = 0;

        /// <summary>
        /// Lieferantennummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string SupplierId { get; set; } = "";

        /// <summary>
        /// Artikelnummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string ArticleId { get; set; } = "";

        /// <summary>
        /// Artikelbezeichnung
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string ArticleName { get; set; } = "";

        /// <summary>
        /// Feste Positionsnummer
        /// </summary>
        [WebSvcAutoProcess()]
        public int FixedItemId { get; set; } = -1;

        /// <summary>
        /// Positionsnummer
        /// </summary>
        [WebSvcAutoProcess()]
        public int ItemId { get; set; } = -1;

        /// <summary>
        /// Objektstatus
        /// </summary>
        [WebSvcAutoProcess()]
        public StatusEnum Status { get; set; } = 0;

        /// <summary>
        /// Hat Zusatzpositionen
        /// </summary>
        [WebSvcAutoProcess()]
        public bool HasAdditionalItems { get; set; } = false;

        /// <summary>
        /// Ist Zusatzposition
        /// </summary>
        [WebSvcAutoProcess()]
        public bool IsAditionalItem { get; set; } = false;

        /// <summary>
        /// Lagernummer
        /// </summary>
        [WebSvcAutoProcess()]
        public int StorageId { get; set; } = -1;

        /// <summary>
        /// Positionstext
        /// </summary>
        [WebSvcAutoProcess()]
        public string ItemText { get; set; } = "";

        /// <summary>
        /// Positionsart
        /// </summary>
        [WebSvcAutoProcess()]
        public string ItemType { get; set; } = "";

        /// <summary>
        /// Belegart
        /// </summary>
        [WebSvcAutoProcess()]
        public string OrderType { get; set; } = "";

        /// <summary>
        /// Mengeneinheit
        /// </summary>
        [WebSvcAutoProcess()]
        public string QuantityUnit { get; set; } = "";

        /// <summary>
        /// Steuerschlüssel
        /// </summary>
        [WebSvcAutoProcess()]
        public string TaxCode { get; set; } = "";

        /// <summary>
        /// Kundenpositionsnummer
        /// </summary>
        [WebSvcAutoProcess()]
        public int CustomerFixedItemId { get; set; } = 0;

        /// <summary>
        /// Boie Artikelkurzbezeichnung
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string ArticleShortDescription { get; set; } = "";

        /// <summary>
        /// Erweiterter Positionsstatus
        /// </summary>
        [WebSvcAutoProcess()]
        public EnhancedPosStatusEnum EnhancedPosStatus { get; set; } = 0;

        /// <summary>
        /// Kundenartikelnummer
        /// </summaryarP>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string CustomerArticleId { get; set; } = "";

        public static SalesOrderDetailData FromDC(dcSalesDetail nuvSalesDetail)
        {
            return new SalesOrderDetailData()
            {
                AlternativeItem = nuvSalesDetail.bAlternativeItem.GetValueOrDefault(),
                PartialDelivery = nuvSalesDetail.bPartialDelivery.GetValueOrDefault(),
                FactorSalesPrice = nuvSalesDetail.decFactorSalesPrice.GetValueOrDefault(0),
                ItemGrossAmount = nuvSalesDetail.decItemGrossAmountFC.GetValueOrDefault(0),
                ItemNetAmount = nuvSalesDetail.decItemNetAmountFC.GetValueOrDefault(0),
                ItemTaxAmount = nuvSalesDetail.decItemTaxAmountFC.GetValueOrDefault(0),
                PriceFactor = nuvSalesDetail.decPriceFactor.GetValueOrDefault(0),
                Price = nuvSalesDetail.decPriceFactor.GetValueOrDefault(0),
                QuantityDelivered = nuvSalesDetail.decQuantityDelivered.GetValueOrDefault(0),
                QuantityOrdered = nuvSalesDetail.decQuantityOrdered.GetValueOrDefault(0),
                TaxRate = nuvSalesDetail.decTaxRate.GetValueOrDefault(0),
                InvoiceDate = nuvSalesDetail.dtInvoiceDate.GetValueOrDefault(),
                DeliveryDate = nuvSalesDetail.dtDeliveryDate.GetValueOrDefault(),
                RequestedDate = nuvSalesDetail.dtRequestedDate.GetValueOrDefault(),
                CustomerOrderId = nuvSalesDetail.lngCustomerOrderID.GetValueOrDefault(-1).ToString(),
                DeliveryNoteId = nuvSalesDetail.lngDeliveryNoteID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                GoodsReceiptId = nuvSalesDetail.lngGoodsReceiptID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                InvoiceId = nuvSalesDetail.lngInvoiceID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                OrderId = nuvSalesDetail.lngOrderID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                PackingListId = nuvSalesDetail.lngPackingListID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                SalesPriceUnit = (int)nuvSalesDetail.lngSalesPriceUnit.GetValueOrDefault(0),
                SupplierId = nuvSalesDetail.lngSupplierID.GetValueOrDefault(-1).ToString().Replace("-1", ""),
                ArticleId = NZ(nuvSalesDetail.sArticleID),
                ArticleName = NZ(nuvSalesDetail.sArticleName),
                FixedItemId = nuvSalesDetail.shtFixedItemID.GetValueOrDefault(-1),
                ItemId = nuvSalesDetail.shtItemID.GetValueOrDefault(-1),
                Status = (StatusEnum)nuvSalesDetail.shtStatus.GetValueOrDefault(0),
                HasAdditionalItems = (bool)(nuvSalesDetail.shtHasAdditionalItems.GetValueOrDefault(0) == 1),
                IsAditionalItem = (bool)(nuvSalesDetail.shtHasAdditionalItems.GetValueOrDefault(0) == 1),
                StorageId = nuvSalesDetail.shtStorageID.GetValueOrDefault(0),
                ItemText = NZ(nuvSalesDetail.sItemText),
                ItemType = NZ(nuvSalesDetail.sItemType),
                OrderType = NZ(nuvSalesDetail.sOrderType),
                QuantityUnit = NZ(nuvSalesDetail.sQuantityUnit),
                TaxCode = NZ(nuvSalesDetail.sTaxCode),
                ArticleShortDescription = NZ(nuvSalesDetail.sK78_ARBEZ),
                CustomerArticleId = NZ(nuvSalesDetail.sCustomerArticleID),
                CustomerFixedItemId = nuvSalesDetail.shtCustFixedItemID.GetValueOrDefault(0),
                EnhancedPosStatus = (EnhancedPosStatusEnum)nuvSalesDetail.shtK78_EnhancedPostStatus.GetValueOrDefault(0),
            };
        }

        public static SalesOrderDetailData FromDB(SqlDataReader R)
        {
            return new SalesOrderDetailData()
            {
                OrderId = NZ(R.GetValue(R.GetOrdinal("OrderId")), ""),
                OrderType = NZ(R.GetValue(R.GetOrdinal("OrderType"))),

                InvoiceId = NZ(R.GetValue(R.GetOrdinal("InvoiceId")), ""),
                InvoiceDate = N<DateTime>(R.GetValue(R.GetOrdinal("InvoiceDate")), DateTime.MinValue),
                DeliveryDate = N<DateTime>(R.GetValue(R.GetOrdinal("DeliveryDate")), DateTime.MinValue),
                RequestedDate = N<DateTime>(R.GetValue(R.GetOrdinal("RequestedDate")), DateTime.MinValue),

                AlternativeItem = (bool)(N<int>(R.GetValue(R.GetOrdinal("AlternativeItem")), 0) == 1),
                PartialDelivery = false, // nicht im FS hinterlegt ...
                FactorSalesPrice = N<decimal>(R.GetValue(R.GetOrdinal("FactorSalesPrice")), 0),

                ItemGrossAmount = 0, //TODO: Berechnen
                ItemNetAmount = 0, //TODO: Berechnen
                ItemTaxAmount = 0, //TODO: Berechnen

                PriceFactor = N<decimal>(R.GetValue(R.GetOrdinal("PriceFactor")), 0),
                Price = N<decimal>(R.GetValue(R.GetOrdinal("Price"))),
                QuantityDelivered = N<decimal>(R.GetValue(R.GetOrdinal("QuantityDelivered")), 0),
                QuantityOrdered = N<decimal>(R.GetValue(R.GetOrdinal("QuantityOrdered")), 0),

                TaxRate = 0, // TODO: Berechnen

                SupplierId = NZ(R.GetValue(R.GetOrdinal("SupplierId")), ""),
                ArticleId = NZ(R.GetValue(R.GetOrdinal("ArticleId"))),
                ArticleName = NZ(R.GetValue(R.GetOrdinal("ArticleName"))),
                ArticleShortDescription = NZ(R.GetValue(R.GetOrdinal("ArticleShortDescription"))),
                CustomerArticleId = NZ(R.GetValue(R.GetOrdinal("CustomerArticleId"))),

                ItemId = N<int>(R.GetValue(R.GetOrdinal("ItemId")), 0),
                FixedItemId = N<int>(R.GetValue(R.GetOrdinal("FixedItemId")), 0),
                CustomerFixedItemId = N<int>(R.GetValue(R.GetOrdinal("CustomerFixedItemId")), 0),
                ItemType = NZ(R.GetValue(R.GetOrdinal("ItemType"))),

                CustomerOrderId = NZ(R.GetValue(R.GetOrdinal("CustomerOrderId")), ""),
                DeliveryNoteId = NZ(R.GetValue(R.GetOrdinal("DeliveryNoteId")), ""),
                GoodsReceiptId = NZ(R.GetValue(R.GetOrdinal("GoodsReceiptId")), ""),
                PackingListId = NZ(R.GetValue(R.GetOrdinal("PackingListId")), ""),

                StorageId = N<int>(R.GetValue(R.GetOrdinal("StorageId")), 0),
                ItemText = NZ(R.GetValue(R.GetOrdinal("ItemText"))),
                TaxCode = NZ(R.GetValue(R.GetOrdinal("TaxCode"))),

                SalesPriceUnit = N<int>(R.GetValue(R.GetOrdinal("SalesPriceUnit")), 0),
                QuantityUnit = NZ(R.GetValue(R.GetOrdinal("QuantityUnit"))),
                Status = (StatusEnum)N<int>(R.GetValue(R.GetOrdinal("Status")), 0),

                HasAdditionalItems = false, // nicht im FS hinterlegt ...
                IsAditionalItem = false, // nicht im FS hinterlegt ...
                
                EnhancedPosStatus = N<EnhancedPosStatusEnum>(R.GetValue(R.GetOrdinal("EnhancedPosStatus")), EnhancedPosStatusEnum.None),
            };
        }

        public static List<SalesOrderDetailData> FromDC(List<dcSalesDetail> nuvSalesDetail)
        {
            var Result = new List<SalesOrderDetailData>();

            foreach (var nuvItem in nuvSalesDetail)
                Result.Add(SalesOrderDetailData.FromDC(nuvItem));

            return Result;
        }
    }
}
