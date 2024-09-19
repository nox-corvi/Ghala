using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;


namespace NVWebAccess
{
    public class PriceInfoData : IWebSvcResponseSeed
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

        public decimal definableAttribute1 { get; set; }

        public decimal definableAttribute2 { get; set; }

        /// <summary>
        /// Die Menge für welche der Preis ermittelt wurde.
        /// </summary>
        public decimal Quantity { get; set; } = 0m;

        /// <summary>
        /// Preis aus Preisgruppe Brutto
        /// </summary>
        public decimal BasePriceGross { get; set; } = 0m;

        /// <summary>
        /// Preis aus Preisgruppe Netto
        /// </summary>
        public decimal BasePriceNet { get; set; } = 0m;

        /// <summary>
        /// Preis aus Preisgruppe Steuer
        /// </summary>
        public decimal BasePriceTax { get; set; } = 0m;

        /// <summary>
        /// Kalkulierter Preis Brutto;
        /// </summary>
        public decimal PriceGross { get; set; } = 0m;

        /// <summary>
        /// Kalkulierter Preis Netto
        /// </summary>
        public decimal PriceNet { get; set; } = 0m;

        /// <summary>
        /// Kalkulierter Preis Steuer
        /// </summary>
        public decimal PriceTax { get; set; } = 0m;

        /// <summary>
        /// Der Steuerschlüssel
        /// </summary>
        public string TaxCode { get; set; } = "";

        /// <summary>
        /// VK Pro
        /// </summary>
        public int SalesPricePerUnit { get; set; } = 0;

        /// <summary>
        /// Währung
        /// </summary>
        public string Currency { get; set; } = "";

        /// <summary>
        /// Preisgruppe
        /// </summary>
        public int PriceGroup { get; set; } = -1;

        /// <summary>
        /// Die Mengeneinheit
        /// </summary>
        public string QuantityUnit { get; set; } = "";

        /// <summary>
        /// Falls vorhanden, eine Liste mit den Staffelpreisen
        /// </summary>
        public List<PriceScaleData> ScalePriceInfos = new List<PriceScaleData>();

        public static PriceInfoData FromDC(dcPriceInfo nuvPriceInfo)
        {
            return new PriceInfoData()
            {
                CustomerId = (int)nuvPriceInfo.lngCustomerID.GetValueOrDefault(0),
                ArticleId = NZ(nuvPriceInfo.sArticleID),
                definableAttribute1 = nuvPriceInfo.decK78_DefinableAttribute1Value.GetValueOrDefault(0m),
                definableAttribute2 = nuvPriceInfo.decK78_DefinableAttribute2Value.GetValueOrDefault(0m),
                BasePriceGross = nuvPriceInfo.decBasePriceGross.GetValueOrDefault(0m),
                BasePriceNet = nuvPriceInfo.decBasePriceNet.GetValueOrDefault(0m),
                BasePriceTax = nuvPriceInfo.decBasePriceTax.GetValueOrDefault(0m),
                PriceGross = nuvPriceInfo.decPriceGross.GetValueOrDefault(0m),
                PriceNet = nuvPriceInfo.decPriceNet.GetValueOrDefault(0m),
                PriceTax = nuvPriceInfo.decPriceTax.GetValueOrDefault(0m),
                TaxCode = NZ(nuvPriceInfo.sTaxCode),
                SalesPricePerUnit = (int)nuvPriceInfo.lngSalesPriceUnit.GetValueOrDefault(0),
                Currency = NZ(nuvPriceInfo.sCurrencyCode),
                PriceGroup = (int)nuvPriceInfo.shtPriceGroup.GetValueOrDefault(0),
                QuantityUnit = NZ(nuvPriceInfo.sQuantityUnit),
            };
        }
    }
}