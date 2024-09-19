
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccess;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NVWebAccess
{
    public enum EnhancedPosStatusEnum
    {
        None,
        DeliveryDateWillFollow,
        InClarification
    };

    public enum StatusEnum
    {
        unknown = 0,
        open = 1,
        released = 2,
        charge = 3,
        charged = 4,
        delivered = 5,
        canceled = 6
    }

    [Serializable()]
    public class SalesOrderMasterDetail : WebSvcResponseShell<SalesOrderMasterDetailData>
    {
        private static string MASTER_DETAIL_FL =
            @"k.BelegNr as OrderId, k.KundenNr as CustomerId, k.BelegArt as OrderType,  
            k.ErfassungsDatum as EntryDate, k.LieferDatum as DeliveryDate, 
            k.Bestellung as OrderText, k.BestellDatum as OrderDate, 
            k.lOrt as DeliveryCity, k.lFirma1 as DeliveryCompany1, k.lFirma2 as DeliveryCompany2, k.lLand as DeliveryCountry, k.lLaenderKz as DeliveryCountryCode, k.lStrasse as DeliveryStreet, k.lPlz as DeliveryZipCode, k.lKontakt as DeliveryContact, 
            k.Sperrung as Blocked, k.AngebotStatus as OfferStatus, k.ZBnummer as PaymentCondition, k.VAnummer as ShippingCondition, 
                
            CASE WHEN ((COALESCE(p.LieferscheinNr, 0) > 0) OR (COALESCE(p.RechnungsNr,0) > 0) OR (NOT p.CMemoId is NULL)) THEN p.Menge_Geliefert ELSE 0 END as QuantityDelivered, 
            p.Menge_Geliefert as QuantityDeliverable, p.Menge_Bestellt as QuantityOrdered, 
            p.LieferDatum as PosDeliveryDate, p.WunschDatum as RequestedDate, k.Bestellung as CustomerOrderId, p.LieferscheinNr as DeliveryNoteId, 
            p.BonNr as GoodsReceiptId, p.RechnungsNr as InvoiceId, p.PackListenNr as PackingListId, 
  
            p.SupplierId, p.ArtikelNr as ArticleId, p.Bezeichnung as ArticleName, a.K78_ARBEZ as ArticleShortDescription, p.fArtikelNr as CustomerArticleId,  
            p.PositionsNr as ItemId, p.FixPosNr as FixedItemId, p.CustFixedItemId as CustomerFixedItemId, p.PositionsArt as ItemType, 

            p.EinheitVk as QuantityUnit, p.Status, p.K78_EnhancedPosStatus as EnhancedPosStatus";

        private static string MASTER_DETAIL_CR = @"SELECT COUNT(*) as RC FROM ({0}) a;";

        private static string MASTER_DETAIL_SQL =
            @"SELECT
                {0}
            FROM
                AuftragsKopf k
            LEFT JOIN
                AuftragsPos p on
                k.KundenNr = p.KundenNr and
                k.BelegNr = p.BelegNr and k.BelegArt = p.BelegArt and k.BranchKey = p.BranchKey
            LEFT JOIN
                Artikel a on a.ArtikelNr = p.ArtikelNr and a.BranchKey = p.BranchKey 
            WHERE
                k.KundenNr = {1}
                and k.BelegArt< 'A' and p.Status != 6
                and (DATEADD(day, {2}, k.Erfassungsdatum) >= sysdatetime()) ";
        private static string PAGING = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";

        public override SalesOrderMasterDetailData Data { get; set; }
        public static List<SalesOrderMasterDetail> GetSalesOrderMasterDetailExt(WebSvcConnect svc, int CustomerId, int Days, string Where, SqlParameter[] Parameters, string Order)
        {
            try
            {
                List<SalesOrderMasterDetail> Result = new List<SalesOrderMasterDetail>();

                string AndWhere = NZ(Where).Trim().StartsWith("AND", StringComparison.InvariantCultureIgnoreCase) ? " " : Where.Equals("") ? " " : " AND ";
                string OrderBy = NZ(Order).Trim().StartsWith("ORDER BY", StringComparison.InvariantCultureIgnoreCase) ? Order : Order.Equals("") ? " " : " ORDER BY " + Order;

                using (var dbA = new SqlDbAccess(Global.Configuration.GetConnectionString("enventa") ?? ""))
                using (var h = dbA.GetReader(
                    string.Format(MASTER_DETAIL_SQL, MASTER_DETAIL_FL, CustomerId, Days) + AndWhere + OrderBy,
                    Parameters))
                    while (h.Read())
                        Result.Add(new SalesOrderMasterDetail()
                        {
                            State = WebSvcResult.Ok,
                            Data = SalesOrderMasterDetailData.FromDB(h)
                        });

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<SalesOrderMasterDetail>()
                {
                    new SalesOrderMasterDetail()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public static int GetSalesOrderMasterDetailExtCount(WebSvcConnect svc, int CustomerId, int Days, string Where, SqlParameter[] Parameters)
        {
            try
            {
                string AndWhere = NZ(Where).Trim().StartsWith("AND", StringComparison.InvariantCultureIgnoreCase) ? " " + Where : Where.Equals("") ? "" : " AND ";
                //string OrderBy = NZ(Order).Trim().StartsWith("ORDER BY", StringComparison.InvariantCultureIgnoreCase) ? Order : Order.Equals("") ? "" : " ORDER BY " + Order;

                using (var dbA = new SqlDbAccess(Global.Configuration.GetConnectionString("enventa") ?? ""))
                    return dbA.GetValue<int>(string.Format(MASTER_DETAIL_CR, string.Format(MASTER_DETAIL_SQL, MASTER_DETAIL_FL, CustomerId, Days) + AndWhere), Parameters);
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return -1;
            }
        }

        public static List<SalesOrderMasterDetail> GetSalesOrderMasterDetailExtPart(WebSvcConnect svc, int CustomerId, int Days, string Where, SqlParameter[] Parameters, string Order, int PageIndex, int PageSize)
        {
            try
            {
                List<SalesOrderMasterDetail> Result = new List<SalesOrderMasterDetail>();
                string AndWhere = NZ(Where).Trim().StartsWith("AND", StringComparison.InvariantCultureIgnoreCase) ? " " + Where : Where.Equals("") ? "" : " AND ";
                string OrderBy = NZ(Order).Trim().StartsWith("ORDER BY", StringComparison.InvariantCultureIgnoreCase) ? Order : Order.Equals("") ? "" : " ORDER BY " + Order;
                string SQL = string.Format(string.Format(MASTER_DETAIL_SQL, MASTER_DETAIL_FL, CustomerId, Days) + AndWhere + OrderBy + PAGING, PageIndex * PageSize, PageSize);

                Global.Logger.LogDebug(SQL);

                using (var dbA = new SqlDbAccess(Global.Configuration.GetConnectionString("enventa") ?? ""))
                using (var h = dbA.GetReader(SQL, Parameters))

                    while (h.Read())
                        Result.Add(new SalesOrderMasterDetail()
                        {
                            State = WebSvcResult.Ok,
                            Data = SalesOrderMasterDetailData.FromDB(h)
                        });

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<SalesOrderMasterDetail>()
                {
                    new SalesOrderMasterDetail()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public static List<SalesOrderMasterDetail> GetSalesOrderMasterByCustomerId(WebSvcConnect svc, int CustomerId, int Days) =>
            GetSalesOrderMasterDetailExt(svc, CustomerId, Days, "", null, "");

        public static int GetSalesOrderMasterByCustomerIdCount(WebSvcConnect svc, int CustomerId, int Days) =>
            GetSalesOrderMasterDetailExtCount(svc, CustomerId, Days, "", null);

        public static List<SalesOrderMasterDetail> GetSalesOrderMasterByCustomerIdPart(WebSvcConnect svc, int CustomerId, int Days, string Where, int PageIndex, int PageSize) =>
            GetSalesOrderMasterDetailExtPart(svc, CustomerId, Days, "", null, "", PageIndex, PageSize);


        public SalesOrderMasterDetail()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class SalesOrderMasterDetailData : WebSvcResponseSeed
    {

        #region Properties
        /// <summary>
        /// Auftragsnummer
        /// </summary>
        public string OrderId { get; set; } = "";

        /// <summary>
        /// KundenNr
        /// </summary>
        public int CustomerId { get; set; } = -1;

        /// <summary>
        /// BelegArt
        /// </summary>
        public string OrderType { get; set; } = "";

        /// <summary>
        /// Erfassungsdatum
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Lieferdatum
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Bestelltext
        /// </summary>
        public string OrderText { get; set; } = "";

        /// <summary>
        /// Bestelldatum
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Lieferanschrift - Firma 1
        /// </summary>
        public string DeliveryCompany1 { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Firma 2
        /// </summary>
        public string DeliveryCompany2 { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Straße
        /// </summary>
        public string DeliveryStreet { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Postleitzahl
        /// </summary>
        public string DeliveryZipCode { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Stadt
        /// </summary>
        public string DeliveryCity { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Ländercode
        /// </summary>
        public string DeliveryCountryCode { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Land
        /// </summary>
        public string DeliveryCountry { get; set; } = "";

        /// <summary>
        /// Lieferanschrift - Ansprechpartner
        /// </summary>
        public string DeliveryContact { get; set; } = "";

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
        /// Bestellte Menge
        /// </summary>
        public decimal QuantityOrdered { get; set; } = 0;

        /// <summary>
        /// Zu liefernde Menge
        /// </summary>
        public decimal QuantityDeliverable { get; set; } = 0;

        /// <summary>
        /// Gelieferte Menge
        /// </summary>
        public decimal QuantityDelivered { get; set; } = 0;

        /// <summary>
        /// Lieferdatum Position
        /// </summary>
        public DateTime? PosDeliveryDate { get; set; }

        /// <summary>
        /// Wunschlieferdatum
        /// </summary>
        public DateTime? RequestedDate { get; set; }

        /// <summary>
        /// Kundenbestellnummer
        /// </summary>
        public string CustomerOrderId { get; set; } = "";

        /// <summary>
        /// Lieferscheinnummer
        /// </summary>
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
        /// Packlistennummer
        /// </summary>
        [WebSvcAutoProcess(EnableFullText = true)]
        public string PackingListId { get; set; } = "";

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
        /// Boie Artikelkurzbezeichnung
        /// </summary>
        public string ArticleShortDescription { get; set; } = "";

        /// <summary>
        /// Kundenartikelnummer
        /// </summaryarP>
        public string CustomerArticleId { get; set; } = "";

        /// <summary>
        /// Positionsnummer
        /// </summary>
        public int ItemId { get; set; } = -1;

        /// <summary>
        /// Feste Positionsnummer
        /// </summary>
        public int FixedItemId { get; set; } = -1;

        /// <summary>
        /// Kundenpositionsnummer
        /// </summary>
        public int CustomerFixedItemId { get; set; } = 0;

        /// <summary>
        /// Positionsart
        /// </summary>
        public string ItemType { get; set; } = "";

        /// <summary>
        /// Mengeneinheit
        /// </summary>
        public string QuantityUnit { get; set; } = "";

        /// <summary>
        /// Objektstatus
        /// </summary>
        public StatusEnum Status { get; set; } = StatusEnum.unknown;

        /// <summary>
        /// Erweiterter Positionsstatus
        /// </summary>
        public EnhancedPosStatusEnum EnhancedPosStatus { get; set; } = EnhancedPosStatusEnum.None;
        #endregion

        public static SalesOrderMasterDetailData FromDB(SqlDataReader r) =>
            new SalesOrderMasterDetailData()
            {
                OrderId = r.GetValue(r.GetOrdinal("OrderId")).ToString(),
                CustomerId = N<int>(r.GetValue(r.GetOrdinal("CustomerId"))),
                OrderType = NZ(r.GetValue(r.GetOrdinal("OrderType"))),
                EntryDate = (DateTime)(r.GetValue(r.GetOrdinal("EntryDate"))),
                DeliveryDate = (DateTime)(r.GetValue(r.GetOrdinal("DeliveryDate"))),
                OrderText = NZ(r.GetValue(r.GetOrdinal("OrderText"))),
                OrderDate = N<DateTime?>((r.GetValue(r.GetOrdinal("OrderDate")))),
                DeliveryCompany1 = NZ(r.GetValue(r.GetOrdinal("DeliveryCompany1"))),
                DeliveryCompany2 = NZ(r.GetValue(r.GetOrdinal("DeliveryCompany1"))),
                DeliveryStreet = NZ(r.GetValue(r.GetOrdinal("DeliveryStreet"))),
                DeliveryZipCode = NZ(r.GetValue(r.GetOrdinal("DeliveryZipCode"))),
                DeliveryCity = NZ(r.GetValue(r.GetOrdinal("DeliveryCity"))),
                DeliveryCountryCode = NZ(r.GetValue(r.GetOrdinal("DeliveryCountryCode"))),
                DeliveryCountry = NZ(r.GetValue(r.GetOrdinal("DeliveryCountry"))),
                DeliveryContact = NZ(r.GetValue(r.GetOrdinal("DeliveryContact"))),
                QuantityDeliverable= N<decimal>(r.GetValue(r.GetOrdinal("QuantityDeliverable")), 0),
                QuantityDelivered = N<decimal>(r.GetValue(r.GetOrdinal("QuantityDelivered")), 0),
                QuantityOrdered = N<decimal>(r.GetValue(r.GetOrdinal("QuantityOrdered")), 0),
                PosDeliveryDate = N<DateTime?>(r.GetValue(r.GetOrdinal("PosDeliveryDate"))),
                RequestedDate = N<DateTime?>(r.GetValue(r.GetOrdinal("RequestedDate"))),
                CustomerOrderId = NZ(r.GetValue(r.GetOrdinal("CustomerOrderId")), ""),
                DeliveryNoteId = NZ(r.GetValue(r.GetOrdinal("DeliveryNoteId")), ""),
                GoodsReceiptId = NZ(r.GetValue(r.GetOrdinal("GoodsReceiptId")), ""),
                InvoiceId = NZ(r.GetValue(r.GetOrdinal("InvoiceId")), ""),
                PackingListId = NZ(r.GetValue(r.GetOrdinal("PackingListId")), ""),
                SupplierId = NZ(r.GetValue(r.GetOrdinal("SupplierId")), ""),
                ArticleId = NZ(r.GetValue(r.GetOrdinal("ArticleId"))),
                ArticleName = NZ(r.GetValue(r.GetOrdinal("ArticleName"))),
                ArticleShortDescription = NZ(r.GetValue(r.GetOrdinal("ArticleShortDescription"))),
                CustomerArticleId = NZ(r.GetValue(r.GetOrdinal("CustomerArticleId"))),
                ItemId = N<int>(r.GetValue(r.GetOrdinal("ItemId")), 0),
                FixedItemId = N<int>(r.GetValue(r.GetOrdinal("FixedItemId")), 0),
                CustomerFixedItemId = N<int>(r.GetValue(r.GetOrdinal("CustomerFixedItemId")), 0),
                ItemType = NZ(r.GetValue(r.GetOrdinal("ItemType"))),
                QuantityUnit = NZ(r.GetValue(r.GetOrdinal("QuantityUnit"))),
                Status = (StatusEnum)N<int>(r.GetValue(r.GetOrdinal("Status")), 0),
                EnhancedPosStatus = (EnhancedPosStatusEnum)N<int>(r.GetValue(r.GetOrdinal("EnhancedPosStatus")), 0)
            };
    }
}
