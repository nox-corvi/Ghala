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
    public class SalesDocument : WebSvcResponseShell<SalesDocumentData>
    {
        public override SalesDocumentData Data { get; set; }

        /// <summary>
        /// Ermittelt die Bestände für einen Artikel
        /// </summary>
        /// <param name="ArticleId"></param>
        /// <returns></returns>
        public static SalesDocument GetDocumentById(WebSvcConnect svc, int DocumentId, string DocumentType)
        {
            try
            {
                var nuvSalesDocument = svc.GetSalesDocumentById(DocumentId, DocumentType);
                if (nuvSalesDocument.Status == 0)
                    return new SalesDocument()
                    {
                        State = WebSvcResult.Ok,
                        Data = SalesDocumentData.FromDC(nuvSalesDocument)
                    };
                else
                    return new SalesDocument()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvSalesDocument.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new SalesDocument()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public SalesDocument()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class SalesDocumentData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public decimal TotalAmount { get; set; } = 0m;
        public decimal TotalTax { get; set; } = 0m;

        public DateTime? PrintDate { get; set; } = null;
        public DateTime? ValidityDate { get; set; } = null;

        public long DeliveryNoteId { get; set; } = -1;
        public long DocumentId { get; set; } = -1;
        public long InvoiceId { get; set; } = -1;
        public long OrderId { get; set; } = -1;
        public string Clerk { get; set; } = "";
        public string DeliveryNoteIdList { get; set; } = "";
        public string DocumentType { get; set; } = "";
        public string DocumentTypeDesc { get; set; } = "";
        public int StorageId { get; set; } = -1;

        public string OfferStatus { get; set; } = "";
        public string OrderStatus { get; set; } = "";
        public string OrderText { get; set; } = "";
        public string OrderType { get; set; } = "";
        public string ReferenceId { get; set; } = "";

        public static SalesDocumentData FromDC(dcSalesDocument nuvSalesDocument)
        {
            return new SalesDocumentData()
            {
                TotalAmount = nuvSalesDocument.decTotalAmount.GetValueOrDefault(0),
                TotalTax = nuvSalesDocument.decTotalTax.GetValueOrDefault(0),
                PrintDate = nuvSalesDocument.dtPrintDate,
                ValidityDate = nuvSalesDocument.dtValidityDate,
                DeliveryNoteId = nuvSalesDocument.lngDeliveryNoteID.GetValueOrDefault(-1),
                DocumentId = nuvSalesDocument.lngDocumentID.GetValueOrDefault(-1),
                InvoiceId = nuvSalesDocument.lngInvoiceID.GetValueOrDefault(-1),
                OrderId = nuvSalesDocument.lngOrderID.GetValueOrDefault(-1),
                //oDeliveryNoteIDList = nuvSalesDocument.oDeliveryNoteIDList,
                //oSalesMasterList = nuvSalesDocument.oSalesMasterList,
                Clerk = NZ(nuvSalesDocument.sClerk),
                DeliveryNoteIdList = NZ(nuvSalesDocument.sDeliveryNoteIDList),
                DocumentType = NZ(nuvSalesDocument.sDocumentType),
                DocumentTypeDesc = NZ(nuvSalesDocument.sDocumentTypeDesc),
                StorageId = nuvSalesDocument.shtStorageID.GetValueOrDefault(-1),
                OfferStatus = NZ(nuvSalesDocument.sOfferStatus),
                OrderStatus = NZ(nuvSalesDocument.sOrderStatus),
                OrderText = NZ(nuvSalesDocument.sOrderText),
                OrderType = NZ(nuvSalesDocument.sOrderType),
                ReferenceId = NZ(nuvSalesDocument.sReferenceID),
            };
        }

        /// <summary>
        /// Konviertiert ein CustomerInfoObject in ein dcCustomerObject
        /// </summary>
        /// <returns>ein dcCustomer Object</returns>
        public dcSalesDocument toDC()
        {
            return new dcSalesDocument()
            {
                decTotalAmount = TotalAmount,
                decTotalTax = TotalTax,
                dtPrintDate = PrintDate,
                dtValidityDate = ValidityDate,
                lngDeliveryNoteID = DeliveryNoteId,
                lngDocumentID = DocumentId,
                lngInvoiceID = InvoiceId,
                lngOrderID = OrderId,
                //oDeliveryNoteIDList = nuvSalesDocument.oDeliveryNoteIDList,
                //oSalesMasterList = nuvSalesDocument.oSalesMasterList,
                sClerk = Clerk,
                sDeliveryNoteIDList = DeliveryNoteIdList,
                sDocumentType = DocumentType,
                sDocumentTypeDesc = DocumentTypeDesc,
                shtStorageID = (short)StorageId,
                sOfferStatus = OfferStatus,
                sOrderStatus = OrderStatus,
                sOrderText = OrderText,
                sOrderType = OrderType,
                sReferenceID = ReferenceId,
            };
        }
    }
}
