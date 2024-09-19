using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
////using static UniLib.Helpers;
////using static UniLib.Log;
using NVWebAccessSvc;


namespace NVWebAccess
{
    public class PriceScaleData
    {
        /// <summary>
        /// Kalkulierter Nettopreis
        /// </summary>
        public decimal PriceNet { get; set; } = 0m;

        /// <summary>
        /// Kalkulierter Bruttopreis
        /// </summary>
        public decimal PriceGross { get; set; } = 0m;

        /// <summary>
        /// Kalkulierte Steuer
        /// </summary>
        public decimal PriceTax { get; set; } = 0m;

        /// <summary>
        /// Ab Menge
        /// </summary>
        public decimal QuantityFrom { get; set; } = 1m;

        /// <summary>
        /// VK Pro
        /// </summary>
        public int SalesPricePerUnit { get; set; } = 1;

        /// <summary>
        /// Mengeneinheit
        /// </summary>
        public string QuantityUnit { get; set; } = "";

        public static PriceScaleData FromDC(dcPriceScaleInfo nuvScalePriceInfo)
        {
            return new PriceScaleData()
            {
                PriceGross = nuvScalePriceInfo.decPriceGross.GetValueOrDefault(0m),
                PriceNet = nuvScalePriceInfo.decPriceNet.GetValueOrDefault(0m),
                PriceTax = nuvScalePriceInfo.decPriceTax.GetValueOrDefault(0m),
                QuantityFrom = nuvScalePriceInfo.decQuantityFrom.GetValueOrDefault(1),
                QuantityUnit = Strings.NZ(nuvScalePriceInfo.sQuantityUnit),
                SalesPricePerUnit = (int)nuvScalePriceInfo.lngSalesPriceUnit.GetValueOrDefault(1),
            };
        }
    }
}