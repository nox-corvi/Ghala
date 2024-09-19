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
    [Serializable]
    public class Article 
        : WebSvcResponseShell<ArticleData>
    {
        public override ArticleData Data { get; set; }

        /// <summary>
        /// Ermittelt eine Artikel anhand seiner Artikelnummer
        /// </summary>
        /// <param name="ArticleId">Artikelnummer</param>
        /// <returns>Article</returns>
        public static Article GetArticleById(WebSvcConnect svc, string ArticleId)
        {
            try
            {
                // enventa websvc call
                var nuvArticle = svc.GetArticleByID(ArticleId);
                if (nuvArticle.Status == 1)
                    return new Article()
                    {
                        State = WebSvcResult.Ok,
                        Data = ArticleData.FromDC(nuvArticle)
                    };
                else
                    return new Article()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvArticle.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Article()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        /// <summary>
        /// Gibt alle Artikel zurück
        /// </summary>
        /// <param name="PageIndex">Die Seite welche zurückgegeben werden soll</param>
        /// <returns>Article</returns>
        public static ArticleList GetAllArticles(WebSvcConnect svc, int PageIndex)
        {
            var Data = new ArticleDataList();
            try
            {
                // enventa websvc call
                var nuvArticle = svc.GetAllArticles(PageIndex, 128);

                foreach (var Item in nuvArticle)
                {
                    if (Item.Status == null)
                        Data.Add(ArticleData.FromDC(Item));
                    else
                        return new ArticleList()
                        {
                            State = WebSvcResult.Error,
                            Message = Helper.nuvMessageToString(Item.Messages)
                        };
                }

                return new ArticleList()
                {
                    State = WebSvcResult.Ok,
                    Data = Data,
                };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new ArticleList()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        /// <summary>
        /// Gibt alle Artikel einer Warengruppe zurück
        /// </summary>
        /// <param name="PageIndex">Die Seite welche zurückgegeben werden soll</param>
        /// <returns>Article</returns>
        public static ArticleList GetArticlesByGroupId(WebSvcConnect svc, string GroupId, int PageIndex)
        {
            var Data = new ArticleDataList();
            try
            {
                // enventa websvc call
                var nuvArticle = svc.GetArticlesByGroupId(GroupId, PageIndex);

                foreach (var Item in nuvArticle)
                {
                    if (Item.Status == null)
                        Data.Add(ArticleData.FromDC(Item));
                    else
                        return new ArticleList()
                        {
                            State = WebSvcResult.Error,
                            Message = Helper.nuvMessageToString(Item.Messages)
                        };                        
                }

                return new ArticleList()
                {
                    State = WebSvcResult.Ok,
                    Data = Data,
                };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new ArticleList()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

         public Article()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class ArticleList
        : WebSvcResponseShell<ArticleDataList>
    {
        private ArticleDataList _DataList = new ArticleDataList();

        public Guid ObjectId { get; } = Guid.NewGuid();

        public override ArticleDataList Data 
        { 
            get => _DataList;
            set => _DataList = value; 
        }

        public ArticleList()
            : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class ArticleDataList
        : List<ArticleData>, IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();
    }

    public class ArticleData : IWebSvcResponseSeed
    {
        /// <summary>
        /// Der Status eines Artikels in eNVenta
        /// </summary>
        public enum nuvArticleStateEnum
        {
            /// <summary>
            /// Normal
            /// </summary>
            normal = 1,
            /// <summary>
            /// Gesperrt
            /// </summary>
            locked = 2,
            /// <summary>
            /// Extern ausgelaufen
            /// </summary>
            runOutExt = 3,
            /// <summary>
            /// In Vorbereitung
            /// </summary>
            inPreparation = 4,
            /// <summary>
            /// Ausgelaufen Intern
            /// </summary>
            runOutInt = 5,
        }

        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// ArtikelID
        /// </summary>
        public string ArticleId { get; set; } = "";

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Kurzbeschreibung
        /// </summary>
        public string ShortDesc { get; set; } = "";

        /// <summary>
        /// EAN
        /// </summary>
        public string EAN { get; set; } = "";

        /// <summary>
        /// Menge pro Verpackung
        /// </summary>
        public decimal QuantityPerPackage { get; set; } = 0m;

        /// <summary>
        /// VK Pro
        /// </summary>
        public int SalesPricePerUnit { get; set; } = 0;

        /// <summary>
        /// Die Verpackungseinheit des Artikels
        /// </summary>
        public string QuantityUnitPackage { get; set; } = "";

        /// <summary>
        /// Die Verkaufseinheit des Artikels
        /// </summary>
        public string QuantityUnitSales { get; set; } = "";

        /// <summary>
        /// Mindestabnahmemenge durch Lieferkondition
        /// </summary>
        public decimal ECommPurchMinQty { get; set; } = 0;

        /// <summary>
        /// Der Status des Artikels
        /// </summary>
        public nuvArticleStateEnum State { get; set; } = nuvArticleStateEnum.normal;

        /// <summary>
        /// Ermittelt für einen eNVenta-Artikelstatus den Klartext
        /// </summary>
        /// <param name="blub">Der Artikelstatus</param>
        /// <returns>Der zugehörige Klartext</returns>
        public static string nuvArticleStatePlainText(nuvArticleStateEnum blub)
        {
            switch (blub)
            {
                case nuvArticleStateEnum.normal:
                    return "Artikel Ok";
                case nuvArticleStateEnum.locked:
                    return "Artikel gesperrt";
                case nuvArticleStateEnum.runOutExt:
                    return "Auslaufartikel (extern)";
                case nuvArticleStateEnum.inPreparation:
                    return "Artikel in vorbereitung";
                case nuvArticleStateEnum.runOutInt:
                    return "Auslaufartikel (intern)";
                default:
                    return "Status unbekannt";
            }
        }

        /// <summary>
        /// Konvertiert ein dcArticle-Objekt in ein 
        /// </summary>
        /// <param name="nuvArticle">ein NV dc-Article-Objekt</param>
        /// <returns>Ein ArticleInfoObject</returns>
        internal static ArticleData FromDC(dcArticle nuvArticle)
        {
            return new ArticleData()
            {
                ArticleId = nuvArticle.sArticleID.NZ(),
                Description = nuvArticle.sName.NZ(),
                EAN = nuvArticle.sEAN.NZ(),
                ShortDesc = nuvArticle.sK78_ARBEZ.NZ(),
                QuantityPerPackage = nuvArticle.decQuantityPackage.GetValueOrDefault(1),
                SalesPricePerUnit = (int)nuvArticle.lngSalesPriceUnit.GetValueOrDefault(1),

                QuantityUnitPackage = nuvArticle.sQuantityUnitPackage.NZ(),
                QuantityUnitSales = nuvArticle.sQuantityUnitSales.NZ(),

                ECommPurchMinQty = nuvArticle.decK78_ECommPurchMinQty.GetValueOrDefault(0),
                // im zweifelsfall ist der artikel verfügbar (?) ...
                State = (ArticleData.nuvArticleStateEnum)nuvArticle.shtStatus.GetValueOrDefault((int)ArticleData.nuvArticleStateEnum.normal)
            };
        }
    }
}