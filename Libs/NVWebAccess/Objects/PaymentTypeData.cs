using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;

namespace NVWebAccess.Objects
{
    public class PaymentTypeData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Debitoren-KontoNr des Kunden
        /// </summary>
        public long AccountId { get; set; } = 0;

        /// <summary>
        /// Zahlungsverbindungsnummer
        /// </summary>
        public long PayConnectionId { get; set; } = 0;

        /// <summary>
        /// Bank-KontoNummer
        /// </summary>
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Bank-Kontonummer
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// Name der Bank
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// BIC
        /// </summary>
        public string BIC { get; set; }

        /// <summary>
        /// IBAN
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// Konvertiert ein dcPayConnection-Objekt in ein  PayConnectionObject
        /// </summary>
        /// <param name="nuvArticle">ein NV dc-Article-Objekt</param>
        /// <returns>Ein ArticleInfoObject</returns>
        internal static PayConnectionData FromDC(dcPayConnection nuvPayConnection)
        {
            return new PayConnectionData()
            {
                AccountId = nuvPayConnection.lngAccountID.GetValueOrDefault(0),
                PayConnectionId = nuvPayConnection.lngPayConnectionID.GetValueOrDefault(0),
                BankAccountNumber = NZ(nuvPayConnection.sBankAccountNumber),
                BankCode= NZ(nuvPayConnection.sBankCode),
                BankName= NZ(nuvPayConnection.sBankname),
                BIC = NZ(nuvPayConnection.sBIC),
                IBAN = NZ(nuvPayConnection.sIBAN),
            };
        }
    }
}
