using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;
using Microsoft.Extensions.Logging;

namespace NVWebAccess.Objects
{
    public class PayConnection : WebSvcResponseShell<PayConnectionData>
    {
        public override PayConnectionData Data { get; set; }

        public static List<PayConnection> GetPayConnections(WebSvcConnect svc, int CustomerId)
        {
            try
            {
                var nuvCustomer = Customer.GetCustomerById(svc, CustomerId);
                if (nuvCustomer.State != WebSvcResult.Ok)
                    return new List<PayConnection>()
                    {
                        new PayConnection()
                        {
                            State = nuvCustomer.State,
                            Message = nuvCustomer.Message
                        }
                    };

                // enventa websvc call
                var Result = new List<PayConnection>();
                var nuvPayConnections = svc.GetPayConnectionsByCustomer(CustomerId);
                foreach (var Item in nuvPayConnections)
                    if (Item.Status == null)
                        Result.Add(new PayConnection()
                        {
                            State = WebSvcResult.Ok,
                            Data = PayConnectionData.FromDC(Item)
                        });
                    else
                        Result.Add(new PayConnection()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(Item.Messages),
                        });

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<PayConnection>()
                {
                    new PayConnection()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public PayConnection()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }
}
