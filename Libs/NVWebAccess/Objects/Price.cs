using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;
using Microsoft.Extensions.Logging;

namespace NVWebAccess
{
    public class Price : WebSvcResponseShell<PriceData>
    {
        public override PriceData Data { get; set; }

        /// <summary>
        /// Ermittelt eine Preis für einen Artikel
        /// </summary>
        /// <param name="ArticleId">Die ArtikelID</param>
        /// <returns>PriceInfo-Objekt</returns>
        public static Price GetPrice(WebSvcConnect svc, int CustomerId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity)
        {
            try
            {
                var nuvCustomer = NVWebAccess.Customer.GetCustomerById(svc, CustomerId);
                if (nuvCustomer.State != WebSvcResult.Ok)
                {
                    return new Price()
                    {
                        State = nuvCustomer.State,
                        Message = nuvCustomer.Message
                    };
                }
                else
                {
                    if (nuvCustomer.Data.State != CustomerData.nuvCustomerStateEnum.Ok)
                    {
                        // Ist der Kunde gesperrt?
                        if (nuvCustomer.State != WebSvcResult.Ok)
                        {
                            // dann werden auch keine Preise geliefert ...
                            return new Price()
                            {
                                State = WebSvcResult.NoResult,
                                Message = NVWebAccess.CustomerData.nuvCustomerStatePlainText(nuvCustomer.Data.State)
                            };
                        }
                    }
                }

                /* 2017-03-21 OG, Änderung von GetPriceInfo -> GetPriceInfoQuery
                 * Bei VK Pro > 1, wird der Preis für die Menge [VKPro], und nicht Pro 1 gezogen, 
                 * daher wird die Menge über ein QueryObject übergeben ...
                 */
                var nuvPrice = svc.GetPrice(CustomerId, ArticleId, definableAttribute1, definableAttribute2, Quantity);
                if (nuvPrice.Status == 0)
                {
                    var DataObject = PriceData.FromDC(nuvPrice);

                    // Wichtig um die Qualität der PreisInfo-Abfrage zu ermitteln ...
                    DataObject.Quantity = Quantity;
                    DataObject.ItemAmount = Math.Round(DataObject.Amount / Quantity, 5);
                    DataObject.ItemNetAmount = Math.Round(DataObject.NetAmount / Quantity, 5);
                    DataObject.ItemGrossAmount = Math.Round(DataObject.GrossAmount / Quantity, 5);
                    DataObject.ItemTaxAmount = Math.Round(DataObject.TaxAmount / Quantity, 5);

                    return new Price()
                    {
                        State = WebSvcResult.Ok,
                        Data = DataObject
                    };
                }
                else
                    return new Price()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvPrice.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Price()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Price()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class PriceDiscountDescription
    {
        /// <summary>
        /// Rabattbetrag
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// Rabattprozent
        /// </summary>
        public decimal Percent { get; set; } = 0;

        /// <summary>
        /// Rabattwert
        /// </summary>
        public decimal Value { get; set; } = 0;

        /// <summary>
        /// Rabattnummer
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// Rabattkennzeichen
        /// </summary>
        public string Flag { get; set; } = "";

        /// <summary>
        /// Rabatttext
        /// </summary>
        public string Text { get; set; } = "";

        /// <summary>
        /// Rabattart
        /// </summary>
        public string Type { get; set; } = "";
    }

    public class PriceData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Der zugehörige Kunde
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Der zugehörige Artikel
        /// </summary>
        public string ArticleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal definableAttribute1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal definableAttribute2 { get; set; }

        /// <summary>
        /// Die Menge für welche der Preis ermittelt wurde.
        /// </summary>
        public decimal Quantity { get; set; } = 0m;

        /// <summary>
        /// Die Mengeneinheit
        /// </summary>
        public string QuantityUnitSales { get; set; } = "";

        /// <summary>
        /// Verkaufseinheit
        /// </summary>
        public int SalesPriceUnit { get; set; } = 1;

        /// <summary>
        /// Der Betrag
        /// </summary>
        public decimal Amount { get; set; } = 0m;

        /// <summary>
        /// Brutto
        /// </summary>
        public decimal GrossAmount { get; set; } = 0m;

        /// <summary>
        /// Netto
        /// </summary>
        public decimal NetAmount { get; set; } = 0m;

        /// <summary>
        /// Betrag für ein Stück
        /// </summary>
        public decimal ItemAmount { get; set; } = 0m;

        /// <summary>
        /// Bruttobetrag für eine Einheit
        /// </summary>
        public decimal ItemGrossAmount { get; set; } = 0m;

        /// <summary>
        /// Nettobetrag für eine Einheit
        /// </summary>
        public decimal ItemNetAmount { get; set; } = 0m;

        /// <summary>
        /// Verkaufspreis
        /// </summary>
        public decimal SalesPrice { get; set; } = 0m;

        /// <summary>
        /// Steuerbetrag 
        /// </summary>
        public decimal TaxAmount { get; set; } = 0m;

        /// <summary>
        /// Steuersatz
        /// </summary>
        public decimal TaxRate { get; set; } = 0m;

        /// <summary>
        /// Steuerbetrag für eine Einheit
        /// </summary>
        public decimal ItemTaxAmount { get; set; } = 0m;

        /// <summary>
        /// Liste mit den für den Preis gültigen Rabatten und Zuschlägen
        /// </summary>
        public List<PriceDiscountDescription> Discount { get; private set; }

        public static PriceData FromDC(dcPrice nuvPrice)
        {
            var PriceDiscountList = new List<PriceDiscountDescription>();

            #region Rabatt übertragung
            var t = nuvPrice.GetType();

            for (int i = 1; i <= 6; i++)
            {
                var DiscountAmount = N<decimal>(t.GetProperty("decDiscountAmount" + i.ToString()).GetValue(nuvPrice, null));
                var DiscountPercent = N<decimal>(t.GetProperty($"decDiscountPercent{i.ToString()}").GetValue(nuvPrice, null));
                var DiscountValue = N<decimal>(t.GetProperty($"decDiscountValue{i.ToString()}FC").GetValue(nuvPrice, null));

                if ((DiscountAmount != 0) | (DiscountPercent != 0) | (DiscountValue != 0))
                    PriceDiscountList.Add(new PriceDiscountDescription()
                    {
                        Amount = DiscountAmount,
                        Percent = DiscountPercent,
                        Value = DiscountValue,
                        Id = N<int>(t.GetProperty($"lngDiscountID{i.ToString()}").GetValue(nuvPrice, null)),
                        Flag = NZ(t.GetProperty($"sDiscountFlag{i.ToString()}").GetValue(nuvPrice, null)),
                        Text = NZ(t.GetProperty($"sDiscountText{i.ToString()}").GetValue(nuvPrice, null)),
                        Type = NZ(t.GetProperty($"sDiscountType{i.ToString()}").GetValue(nuvPrice, null))
                    });
            }
            #endregion

            return new PriceData()
            {
                Amount = nuvPrice.decAmount.GetValueOrDefault(0m),
                GrossAmount = nuvPrice.decGrossAmountFC.GetValueOrDefault(0m),
                NetAmount = nuvPrice.decNetAmountFC.GetValueOrDefault(0m),
                Quantity = nuvPrice.decQuantity.GetValueOrDefault(1),
                QuantityUnitSales = NZ(nuvPrice.sQuantityUnitSales),
                
                SalesPrice = nuvPrice.decSalesPrice.GetValueOrDefault(0m),
                SalesPriceUnit = (int)nuvPrice.lngSalesPriceUnit.GetValueOrDefault(1),
                TaxAmount = nuvPrice.decTaxAmountFC.GetValueOrDefault(0m),
                TaxRate = nuvPrice.decTaxRate.GetValueOrDefault(0m),
                ArticleId = NZ(nuvPrice.sArticleID, ""),
                definableAttribute1 = nuvPrice.decK78_DefinableAttribute1Value.GetValueOrDefault(0m),
                definableAttribute2 = nuvPrice.decK78_DefinableAttribute2Value.GetValueOrDefault(0m),
                CustomerId = (int)nuvPrice.lngCustomerID.GetValueOrDefault(),
                Discount = PriceDiscountList,
            };
        }
    }
}