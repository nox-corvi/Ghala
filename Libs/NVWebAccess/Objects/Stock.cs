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
    public class Stock : WebSvcResponseShell<StockData>
    {
        public override StockData Data { get; set; }

        /// <summary>
        /// Ermittelt die Bestände für einen Artikel
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static Stock GetStock(WebSvcConnect svc, short[] Stocks, string ArticleId, decimal definableAttribute1, decimal definableAttribute2)
        {
            try
            {
                var nuvArticle = Article.GetArticleById(svc, ArticleId);
                if (nuvArticle.State != WebSvcResult.Ok)
                    return new Stock()
                    {
                        State = nuvArticle.State,
                        Message = nuvArticle.Message
                    };

                var Data = new StockData();
                foreach (var StockId in Stocks)
                {
                    Data.Stocks = Stocks.ToList<short>();

                    var nuvStock = svc.GetStockInfo(nuvArticle.Data.ArticleId, definableAttribute1, definableAttribute2, StockId);
                    if (nuvStock.Status == 0)
                    {
                        Data.QuantityAvailable += nuvStock.decQuantityAvailable.GetValueOrDefault(0);
                        Data.QuantityOrdered += nuvStock.decQuantityOrdered.GetValueOrDefault(0);
                        Data.QuantityReserved += nuvStock.decQuantityReserved.GetValueOrDefault(0);
                        Data.QuantityStockLevel += nuvStock.decQuantityStockLevel.GetValueOrDefault(0);
                    }
                    else
                        return new Stock()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvStock.Messages),
                        };
                }

                return new Stock()
                {
                    State = WebSvcResult.Ok,
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Stock()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Stock()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }
}