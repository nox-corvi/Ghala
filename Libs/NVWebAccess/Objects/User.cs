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
    public class User 
        : WebSvcResponseShell<UserData>
    {
        public override UserData Data { get; set; }

        public static User CreateUser(WebSvcConnect svc, int WebShopId, int CustomerId, int ContactId, string Email, string Username, string Password)
        {
            try
            {
                // enventa websvc call
                var nuvUser = svc.CreateUser(WebShopId, CustomerId, ContactId, Email, Username, Password);
                if (nuvUser.Status == 1)
                    return new User()
                    {
                        State = WebSvcResult.Ok,
                        Data = UserData.FromDC(nuvUser)
                    };
                else
                    return new User()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvUser.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new User()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }
        public static User GetUserByName(WebSvcConnect svc, int WebShopId, string Name)
        {
            try
            {
                // enventa websvc call
                var nuvUser = svc.GetUserByName(WebShopId, Name);
                if (nuvUser.Status == 1)
                    return new User()
                    {
                        State = WebSvcResult.Ok,
                        Data = UserData.FromDC(nuvUser)
                    };
                else
                    return new User()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvUser.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new User()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static User GetUserByEmail(WebSvcConnect svc, int WebShopId, string Email)
        {
            try
            {
                // enventa websvc call
                var nuvUser = svc.GetUserByEmail(WebShopId, Email);
                if (nuvUser.Status == 0)
                    return new User()
                    {
                        State = WebSvcResult.Ok,
                        Data = UserData.FromDC(nuvUser)
                    };
                else
                    return new User()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvUser.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new User()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static List<User> GetUsersByCustomerId(WebSvcConnect svc, int WebShopId, int CustomerId)
        {
            try
            {
                // enventa websvc call
                var nuvUsers = svc.GetUsersByCustomerId(WebShopId, WebShopId);

                var Result = new List<User>();
                foreach (var Item in nuvUsers)
                {
                    if (Item.Status == 1)
                        Result.Add(new User()
                        {
                            State = WebSvcResult.Ok,
                            Data = UserData.FromDC(Item)
                        });
                    else
                        Result.Add(new User()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(Item.Messages),
                        });
                }

                return Result;

            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new List<User>()
                {
                    new User()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public static User SaveUser(WebSvcConnect svc, User o)
        {
            try
            {
                if (o.State != WebSvcResult.Ok)
                    return o;

                // enventa websvc call
                var nuvUser = svc.SaveUser(o.Data.ToDC());
                if (nuvUser.Status == 1)
                    return new User()
                    {
                        State = WebSvcResult.Ok,
                        Data = UserData.FromDC(nuvUser)
                    };
                else
                    return new User()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvUser.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new User()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static bool ValidateUser(WebSvcConnect svc, int WebShopId, string Username, string Password)
        {
            try
            {
                // enventa websvc call
                return svc.ValidateUser(WebShopId, Username, Password);
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return false;
            }
        }

        public User()
            : base(Global.Logger, Global.Configuration)
        {

        }
    }
}
