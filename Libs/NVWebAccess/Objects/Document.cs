using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace NVWebAccess
{
    public class Document : WebSvcResponseShell<DocumentData>
    {
        public override DocumentData Data { get; set; }

        public static Document GetArchiveDocumentById(WebSvcConnect svc, string ReferenceId)
        {
            try
            {
                var nuvDocument = svc.GetArchiveDocumentById(ReferenceId);
                if (nuvDocument.Status == 0)
                    return new Document()
                    {
                        State = WebSvcResult.Ok,
                        Data = DocumentData.FromDC(nuvDocument)
                    };
                else
                    return new Document()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvDocument.Messages),
                    };

            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Document()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Document GetPrintedSalesDocument(WebSvcConnect svc, SalesDocument Document)
        {
            try
            {
                var nuvDocument = svc.GetPrintedSalesDocument(Document.Data.toDC());
                if (nuvDocument.Status == 0)
                    return new Document()
                    {
                        State = WebSvcResult.Ok,
                        Data = DocumentData.FromDC(nuvDocument)
                    };
                else
                    return new Document()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvDocument.Messages),
                    };

            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Document()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Document()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class DocumentData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public byte[] Data { get; set; } = null;

        public int Length { get; set; } = 0;

        public string DocumentId { get; set; } = "";

        public string Extension { get; set; } = "";

        public string MimeType { get; set; } = "";

        public string Name { get; set; } = "";

        public static DocumentData FromDC(dcDocument nuvSalesDocument)
        {
            return new DocumentData()
            {
                Data = nuvSalesDocument.binData,
                Length = (int)nuvSalesDocument.lngLength,
                DocumentId = NZ(nuvSalesDocument.sDocumentID),
                Extension = NZ(nuvSalesDocument.sExtension),
                MimeType = NZ(nuvSalesDocument.sMimeType),
                Name = NZ(nuvSalesDocument.sName)
            };
        }

        /// <summary>
        /// Konviertiert ein CustomerInfoObject in ein dcCustomerObject
        /// </summary>
        /// <returns>ein dcCustomer Object</returns>
        public dcDocument toDC()
        {
            return new dcDocument()
            {
                binData = Data,
                lngLength = Length, 
                sDocumentID = DocumentId, 
                sExtension = Extension,
                sMimeType = MimeType,
                sName = Name
            };
        }
    }
}
