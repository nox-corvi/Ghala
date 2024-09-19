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
    public class Customer : WebSvcResponseShell<CustomerData>
    {
        public override CustomerData Data { get; set; }

        /// <summary>
        /// Ermittelt Kundendaten anhand seiner Kundennummer
        /// </summary>
        /// <param name="CustomerId">Kundennummer</param>
        /// <returns>CustomerInfo</returns>
        public static Customer GetCustomerById(WebSvcConnect svc, int CustomerId)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCustomer = svc.GetCustomerById(CustomerId);
                    if (nuvCustomer.Status == 1)
                        return new Customer()
                        {
                            State = WebSvcResult.Ok,
                            Data = CustomerData.FromDC(nuvCustomer),
                        };
                    else
                        return new Customer()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCustomer.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Customer()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Customer CreateCustomer(WebSvcConnect svc, string CustomerGroup, string Company1, string Company2, 
            string Street, string PostalCode, string Country, string CountryCode, int PaymentId)
        {
            return null;
        }


        public Customer()
             : base(Global.Logger, Global.Configuration)
        {

        }
   }
}