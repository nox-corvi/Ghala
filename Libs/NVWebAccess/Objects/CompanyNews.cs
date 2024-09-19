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
    public class CompanyNews : WebSvcResponseShell<CompanyNewsCol>
    {
        public override CompanyNewsCol Data { get; set; } = new CompanyNewsCol();

        /// <summary>
        /// Ermittelt CompanyNews anhand der Gültigkeit
        /// </summary>
        /// <param name="CustomerId">Kundennummer</param>
        /// <returns>CustomerInfo</returns>
        public static CompanyNews GetNewsByTopicality(WebSvcConnect svc, int TopicalityInDays)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCompanyNews = svc.GetCompanyNewsByTopicality(TopicalityInDays);

                    var Result = new CompanyNews();
                    if (nuvCompanyNews != null)
                    {
                        return new CompanyNews()
                        {
                            State = WebSvcResult.Ok,
                            Data = CompanyNewsCol.FromDC(nuvCompanyNews),
                        };
                    }
                    else
                        return new CompanyNews()
                        {
                            State = WebSvcResult.NoResult,
                            Message = "no records found",
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new CompanyNews()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public CompanyNews()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    [Serializable()]
    public class CompanyNewsCol : List<CompanyNewsData>, IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public static CompanyNewsCol FromDC(dcK78_CompanyNewsList nuvCompanyNewsList)
        {
            var Result = new CompanyNewsCol();

            foreach (var Item in nuvCompanyNewsList)
                Result.Add(CompanyNewsData.FromDC(Item));

            return Result;
        }

    }

    public class CompanyNewsData : IWebSvcResponseSeed
    {
        public enum nuvCompanyNewStatus
        {
            None = 0,
            InPreparation = 1,
            Activated = 2,
            Deactivated = 3,
        }

        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Die NewsID
        /// </summary>
        public int NewsID { get; set; } = -1;

        /// <summary>
        /// Das Datum der Nachricht
        /// </summary>
        public DateTime? NewsDate { get; set; }

        /// <summary>
        /// Der Titel der Nachricht
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Die Nachricht per se
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Der Status der Nachricht
        /// </summary>
        public nuvCompanyNewStatus Status { get; set; } = nuvCompanyNewStatus.None;

        /// <summary>
        /// Das Änderungsdatum
        /// </summary>
        public DateTime? AlterationDate { get; set; }

        /// <summary>
        /// Das Datum des Newseintrages
        /// </summary>
        public DateTime? EntryDate { get; set; }

        public static CompanyNewsData FromDC(dcK78_CompanyNews nuvCompanyNews)
        {
            return new CompanyNewsData()
            {
                NewsID = (int)nuvCompanyNews.decNewsID,
                NewsDate = nuvCompanyNews.dtNewsDate,
                Title = NZ(nuvCompanyNews.sTitle),
                Message = NZ(nuvCompanyNews.sMessage),
                Status = (nuvCompanyNewStatus)nuvCompanyNews.shtStatus,
                AlterationDate = nuvCompanyNews.dtAlterationDate,
                EntryDate = nuvCompanyNews.dtEntryDate,
            };
        }
    }
}
