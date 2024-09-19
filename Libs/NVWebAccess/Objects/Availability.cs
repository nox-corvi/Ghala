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
    public enum ExternalStockStatusEnum
    {
        /// <summary>
        /// externe Bestandsinformationen beinhaltet
        /// </summary>
        ExternalStockContained = 1,

        /// <summary>
        /// externe Bestandsinformation kann via  abgerufen werden
        /// </summary>
        OnRequest = 2,

        /// <summary>
        /// keine externen Bestandsinformationen abrufbar
        /// </summary>
        NoExternalStockInformation = 9,

        /// <summary>
        /// Fehler
        /// </summary>
        Error = 90
    }


    public enum ExternalStockResponseStatusEnum
    {

    }

    public class Availability : WebSvcResponseShell<AvailabilityData>
    {
        public override AvailabilityData Data { get; set; }

        /// <summary>
        /// Ermittelt die Bestände für einen Artikel
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static Availability GetAvailability(WebSvcConnect svc, short Stock, string ArticleId, decimal definableAttribute1, decimal definableAttribute2)
        {
            try
            {
                var nuvArticle = Article.GetArticleById(svc, ArticleId);
                if (nuvArticle.State != WebSvcResult.Ok)
                    return new Availability()
                    {
                        State = nuvArticle.State,
                        Message = nuvArticle.Message
                    };

                var Data = new AvailabilityData();
                Data.Stock = Stock;

                var nuvAvailability = svc.GetAvailability(nuvArticle.Data.ArticleId, definableAttribute1, definableAttribute2, Stock);
                if (nuvAvailability.Status == 0)
                {
                    Data.QuantityAvailable = nuvAvailability.decQuantityAvailable.GetValueOrDefault(0);
                    Data.QuantityOrdered = nuvAvailability.decQuantityOrdered.GetValueOrDefault(0);
                    Data.QuantityReserved = nuvAvailability.decQuantityReserved.GetValueOrDefault(0);
                    Data.QuantityStockLevel = nuvAvailability.decQuantityStockLevel.GetValueOrDefault(0);
                    Data.ExternalStock = nuvAvailability.decExternalStock.GetValueOrDefault(0);
                    Data.ExternalStockSupplierId = (int)nuvAvailability.lngExternalStockSupplierID.GetValueOrDefault(0);
                    Data.ExternalStockStatus = (ExternalStockStatusEnum)nuvAvailability.shtExternalStockStatus.GetValueOrDefault(0);
                }
                else
                    return new Availability()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvAvailability.Messages),
                    };


                return new Availability()
                {
                    State = WebSvcResult.Ok,
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Availability()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Availability()
            : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class AvailabilityData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Das verwendeten Lager
        /// </summary>
        public short Stock { get; set; } = -1;

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

        /// <summary>
        /// Der Status des externen Lagers
        /// </summary>
        public ExternalStockStatusEnum  ExternalStockStatus { get; set; } = 0;

        /// <summary>
        /// Die Hauptlieferantennummer
        /// </summary>
        public int ExternalStockSupplierId { get; set; } = 0;

        /// <summary>
        /// Der externe Bestand - falls verfügbar
        /// </summary>
        public decimal ExternalStock { get; set; } = 0m;

        /// <summary>
        /// Die Mengeneinheit
        /// </summary>
        public string QuantityUnit { get; set; } = "";
    }
}