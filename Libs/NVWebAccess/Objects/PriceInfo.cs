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
    public class PriceInfo : WebSvcResponseShell<PriceInfoData>
    {
        public override PriceInfoData Data { get; set; }

        /// <summary>
        /// Ermittelt eine Preis für einen Artikel
        /// </summary>
        /// <param name="ArticleId">Die ArtikelID</param>
        /// <returns>PriceInfo-Objekt</returns>
        public static PriceInfo GetPriceInfo(WebSvcConnect svc, int CustomerId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity)
        {
            try
            {
                var nuvPriceInfo = svc.GetPriceInfo(CustomerId, ArticleId, definableAttribute1, definableAttribute2, Quantity);
                if (nuvPriceInfo.Status == 0)
                {
                    var DataObject = PriceInfoData.FromDC(nuvPriceInfo);

                    // Wichtig um die Qualität der PreisInfo-Abfrage zu ermitteln ...
                    DataObject.Quantity = Quantity;

                    // Staffelpreise übertragen ...
                    DataObject.ScalePriceInfos.Clear();

                    foreach (var Item in nuvPriceInfo.oPriceScaleInfoList)
                        DataObject.ScalePriceInfos.Add(PriceScaleData.FromDC(Item));

                    // Aufsteigend Sortieren...
                    DataObject.ScalePriceInfos.Sort((ScalePriceInfo1, ScalePriceInfo2) => ScalePriceInfo1.QuantityFrom.CompareTo(ScalePriceInfo2.QuantityFrom));

                    return new PriceInfo()
                    {
                        State = WebSvcResult.Ok,
                        Data = DataObject
                    };
                }
                else
                    return new PriceInfo()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvPriceInfo.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));

                return new PriceInfo()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public PriceInfo()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }
}