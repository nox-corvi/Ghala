using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;

namespace NVWebAccess
{
    public class CustomerData : IWebSvcResponseSeed
    {
        /// <summary>
        /// Der Status eines Kunden in eNVenta
        /// </summary>
        public enum nuvCustomerStateEnum
        {
            /// <summary>
            /// Ok
            /// </summary>
            Ok = 0,
            /// <summary>
            /// Gesamtsperre
            /// </summary>
            LockedFull = 1,
            /// <summary>
            /// Liefersperre
            /// </summary>
            LockedDelivery = 2,
            /// <summary>
            /// Rechnungssperre
            /// </summary>
            LockedInvoice = 3
        }

        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Die Kundennummer
        /// </summary>
        public int CustomerId { get; set; } = 0;

        /// <summary>
        /// Firma 1
        /// </summary>
        public string Company1 { get; set; } = "";

        /// <summary>
        /// Firma 2
        /// </summary>
        public string Company2 { get; set; } = "";

        /// <summary>
        /// Postleitzahl
        /// </summary>
        public string ZipCode { get; set; } = "";

        /// <summary>
        /// PLZ Postfach
        /// </summary>
        public string PostBox { get; set; } = "";

        /// <summary>
        /// Strasse
        /// </summary>
        public string Street { get; set; } = "";

        /// <summary>
        /// Stadt
        /// </summary>
        public string City { get; set; } = "";
        
        /// <summary>
        /// Ländercode
        /// </summary>
        public string CountryCode { get; set; } = "";

        /// <summary>
        /// Land
        /// </summary>
        public string Country { get; set; } = "";

        /// <summary>
        /// Währung
        /// </summary>
        public string Currency { get; set; } = "";

        /// <summary>
        /// Kundengruppe
        /// </summary>
        public string CustomerGroup { get; set; } = "";

        /// <summary>
        /// Standard Lieferkondition
        /// </summary>
        public short DeliveryConditionId { get; set; } = -1;

        /// <summary>
        /// Standard Zahlungskondition
        /// </summary>
        public short PaymentConditionId { get; set; } = -1;

        /// <summary>
        /// Standard Versandart
        /// </summary>
        public short ShippingConditionId { get; set; } = -1;

        /// <summary>
        /// Der Status der Kunden
        /// </summary>
        public nuvCustomerStateEnum State { get; set; } = nuvCustomerStateEnum.Ok;

        /// <summary>
        /// Ermittelt für einen eNVenta-Kundenstatus den Klartext
        /// </summary>
        /// <param name="blub">Der Kundenstatus</param>
        /// <returns>Der zugehörige Klartext</returns>
        public static string nuvCustomerStatePlainText(nuvCustomerStateEnum blub)
        {
            switch (blub)
            {
                case nuvCustomerStateEnum.Ok:
                    return "Kunde Ok";
                case nuvCustomerStateEnum.LockedDelivery:
                    return "Kunde hat Liefersperre";
                case nuvCustomerStateEnum.LockedInvoice:
                    return "Kunde hat Rechungssperre";
                case nuvCustomerStateEnum.LockedFull:
                    return "Kunde ist gesperrt";
                default:
                    return "Status unbekannt";
            }
        }

        public static CustomerData FromDC(dcCustomer nuvCustomer)
        {
            return new CustomerData()
            {
                CustomerId = (int)nuvCustomer.lngCustomerID.Value,
                Company1 = NZ(nuvCustomer.sCompany1),
                Company2 = NZ(nuvCustomer.sCompany2),
                Street = NZ(nuvCustomer.sStreet),
                ZipCode = NZ(nuvCustomer.sZipCode),
                PostBox = NZ(nuvCustomer.sPostBox),
                City = NZ(nuvCustomer.sCity),
                CountryCode = NZ(nuvCustomer.sCountryCode),
                Country = NZ(nuvCustomer.sCountry),
                Currency = NZ(nuvCustomer.sCurrency),
                CustomerGroup = NZ(nuvCustomer.sCustomerGroup),
                DeliveryConditionId = nuvCustomer.shtDeliveryConditionID.GetValueOrDefault(-1),
                PaymentConditionId = nuvCustomer.shtPaymentConditionID.GetValueOrDefault(-1),
                ShippingConditionId = nuvCustomer.shtShippingConditionID.GetValueOrDefault(-1),
                State = (nuvCustomerStateEnum)nuvCustomer.shtK78_Block,
            };
        }

        /// <summary>
        /// Konviertiert ein CustomerInfoObject in ein dcCustomerObject
        /// </summary>
        /// <returns>ein dcCustomer Object</returns>
        public dcCustomer toDC()
        {
            return new dcCustomer()
            {
                lngCustomerID = CustomerId,
                sCompany1 = Company1,
                sCompany2 = Company2,
                sStreet = Street,
                sZipBox = ZipCode,
                sPostBox = PostBox,
                sCity = City,
                sCountryCode = CountryCode,
                sCountry = Country,
                sCurrency = Currency,
                sCustomerGroup = CustomerGroup,
                shtDeliveryConditionID = DeliveryConditionId,
                shtPaymentConditionID = PaymentConditionId,
                shtShippingConditionID=ShippingConditionId,
                shtK78_Block = (short)State,
            };
        }
    }
}