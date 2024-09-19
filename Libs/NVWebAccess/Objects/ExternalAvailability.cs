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
    public enum ExternalAvailabilityEnum
    { 
        /// <summary>
        /// extern verfügbar
        /// </summary>
        OnStock  = 1,

        /// <summary>
        /// extern Teilmenge verfügbar
        /// </summary>
        PartlyOnStock = 4,

        /// <summary>
        /// extern nicht verfügbar
        /// </summary>
        NotOnStock = 10,

        /// <summary>
        /// Fehler
        /// </summary>
        Error = 90
    }

    public class ExternalAvailability : WebSvcResponseShell<ExternalAvailabilityData>
    {
        public override ExternalAvailabilityData Data { get; set; }

        /// <summary>
        /// Ermittelt die Bestände für einen Artikel
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static ExternalAvailability GetExternalAvailability(WebSvcConnect svc, string ArticleId, decimal Quantity)
        {
            try
            {
                var nuvArticle = Article.GetArticleById(svc, ArticleId);
                if (nuvArticle.State != WebSvcResult.Ok)
                    return new ExternalAvailability()
                    {
                        State = nuvArticle.State,
                        Message = nuvArticle.Message
                    };

                var Data = new ExternalAvailabilityData();

                var nuvExternalAvailability = svc.GetExternalAvailability(nuvArticle.Data.ArticleId, Quantity);
                if (nuvExternalAvailability.Status == 0)
                {
                    Data.ExternalStockSupplierId = (int)nuvExternalAvailability.lngExternalStockSupplierID.GetValueOrDefault(0);
                    Data.ExternalAvailability = (ExternalAvailabilityEnum)nuvExternalAvailability.shtExternalStockInfo.GetValueOrDefault(0);
                }
                else
                    return new ExternalAvailability()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvExternalAvailability.Messages),
                    };

                return new ExternalAvailability()
                {
                    State = WebSvcResult.Ok,
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new ExternalAvailability()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public ExternalAvailability()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class ExternalAvailabilityData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();


        /// <summary>
        /// Die Hauptlieferantennummer 
        /// </summary>
        public int ExternalStockSupplierId { get; set; } = -1;


        /// <summary>
        /// Status der Verfügbarkeit
        /// </summary>
        public ExternalAvailabilityEnum ExternalAvailability { get; set; }
    }
}