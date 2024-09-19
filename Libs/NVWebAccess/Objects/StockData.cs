using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;


namespace NVWebAccess
{
    public class StockData: IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Die Liste der im StockInfoObject verwendeten Lager
        /// </summary>
        public List<short> Stocks { get; set; } = new List<short>();

        /// <summary>
        /// Die verfügbare Menge
        /// </summary>
        public decimal QuantityAvailable { get; set; } = 0m;

        /// <summary>
        /// Die Bestellte Menge
        /// </summary>
        public decimal QuantityOrdered { get; set; } = 0m;

        /// <summary>
        /// Die reservierte Menge
        /// </summary>
        public decimal QuantityReserved { get; set; } = 0m;

        /// <summary>
        /// Die Menge am Lager
        /// </summary>
        public decimal QuantityStockLevel { get; set; } = 0m;
    }

}