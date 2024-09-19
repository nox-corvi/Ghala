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
    public class Cart 
        : WebSvcResponseShell<CartData>
    {
        public override CartData Data { get; set; }

        public static Cart CreateCart(WebSvcConnect svc, int WebShopId, int CustomerId, string Username, string ExternalCartId)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.CreateCart(WebShopId, CustomerId, Username, ExternalCartId);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart SubmitCart(WebSvcConnect svc, int WebShopId, int CartId, string Username)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.SubmitCart(WebShopId, CartId, Username);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Cart SaveCart(WebSvcConnect svc)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.SaveCart(Data.ToDC());
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart CreateDetailFromArticle(WebSvcConnect svc, int CartId, string ArticleId, decimal definableAttribute1, decimal definableAttribute2, decimal Quantity, string QuantityUnit, string ExternalPositionId)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.CreateCartDetailFromArticle(CartId, ArticleId, definableAttribute1, definableAttribute2, Quantity, QuantityUnit, ExternalPositionId);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartDetailAddDiscountAmount(WebSvcConnect svc, int CartId, int ItemId, string DiscountText, decimal DiscountAmount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartDetailAddDiscountAmount(CartId, ItemId, DiscountText, DiscountAmount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }


        public static Cart ChangeCartDetailAddDiscountPercent(WebSvcConnect svc, int CartId, int ItemId, string DiscountText, decimal DiscountAmount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartDetailAddDiscountPercent(CartId, ItemId, DiscountText, DiscountAmount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }


        public static Cart ChangeCartDeliveryAddress(WebSvcConnect svc, int CartId, string Company1, string Company2, string Company3, string Company4, string Street, string ZipCode, string City, string CountryCode, string Country)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartDeliveryAddress(CartId, Company1, Company2, Company3, Company4, Street, ZipCode, City, CountryCode, Country);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartPaymentCondition(WebSvcConnect svc, int CartId, int PaymentTypeId, int PaymentConditionId)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartPaymentCondition(CartId, PaymentTypeId, PaymentConditionId);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartInvoiceAddress(WebSvcConnect svc, int CartId, string Company1, string Company2, string Company3, string Company4, string Street, string ZipCode, string City, string CountryCode, string Country)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartInvoiceAddress(CartId, Company1, Company2, Company3, Company4, Street, ZipCode, City, CountryCode, Country);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartOrderText(WebSvcConnect svc, int CartId, string OrderText)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartOrderText(CartId, OrderText);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartMessageToMerchant(WebSvcConnect svc, int CartId, string MessageToMerchant)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartMessageToMerchant(CartId, MessageToMerchant);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartCostCenterNo(WebSvcConnect svc, int CartId, string CostCenterNo)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartCostCenterNo(CartId, CostCenterNo);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartContact(WebSvcConnect svc, int CartId, long ContactId)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartContact(CartId, ContactId);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartIgnoreMasterDiscount(WebSvcConnect svc, int CartId, bool IgnoreMasterDiscount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartIgnoreMasterDiscount(CartId, IgnoreMasterDiscount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart ChangeCartShippingCondition(WebSvcConnect svc, int CartId, int ShippingConditionId, decimal ShippingCosts)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.ChangeCartShippingCondition(CartId, ShippingConditionId, ShippingCosts);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Cart SetCartDiscount1(WebSvcConnect svc, int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.SetCartDiscount1(CartId, DiscountText, DiscountPercent, DiscountAmount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }
        public static Cart SetCartDiscount2(WebSvcConnect svc, int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.SetCartDiscount2(CartId, DiscountText, DiscountPercent, DiscountAmount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }
        public static Cart SetCartDiscount3(WebSvcConnect svc, int CartId, string DiscountText, decimal DiscountPercent, decimal DiscountAmount)
        {
            try
            {
                using (var nuv = new WebSvcConnect())
                {
                    // enventa websvc call
                    var nuvCart = svc.SetCartDiscount3(CartId, DiscountText, DiscountPercent, DiscountAmount);
                    if (nuvCart.Status == 1)
                        return new Cart()
                        {
                            State = WebSvcResult.Ok,
                            Data = CartData.FromDC(nuvCart),
                        };
                    else
                        return new Cart()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(nuvCart.Messages),
                        };
                }
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Cart()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Cart() 
            : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class CartData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public bool GrossOrder { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? AlterationDate { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public DateTime? SignDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public decimal DiscountPercent1 { get; set; }
        public decimal DiscountPercent2 { get; set; }
        public decimal DiscountPercent3 { get; set; }
        public decimal PackingCosts { get; set; }
        public decimal ShippingCosts { get; set; }
        public decimal TotalGrossAmount { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }
        public int CartId { get; set; }
        public int ContactId { get; set; }
        public string SalesOrderId { get; set; }
        public int TourId { get; set; }
        public AddressData DeliveryAddress { get; set; }
        public AddressData InvoiceAddress { get; set; }
        public string CartName { get; set; }
        public string CartOderType { get; set; }
        public string ContactName { get; set; }
        public string CurrencyCode { get; set; }
        public string DiscountText1 { get; set; }
        public string DiscountText2 { get; set; }
        public string DiscountText3 { get; set; }
        public string Email { get; set; }
        public string ExternalCartId { get; set; }
        public string ForeignOrderId { get; set; }
        public string LanguageCode { get; set; }
        public string Memo { get; set; }
        public string MemoInternal { get; set; }
        public string PayConnectionId { get; set; }
        public string PaymentId { get; set; }
        public string SalesOrderType { get; set; }
        public string SessionId { get; set; }
        public string Username { get; set; }
        public string UserSign { get; set; }
        public string UserSubmit { get; set; }
        public int CartStatus { get; set; }
        public int PaymentConditionId { get; set; }
        public int PaymentTypeId { get; set; }
        public int ShippingConditionId { get; set; }
        public int WebShopId { get; set; }

        public List<CartDetailData> Positions { get; set; }

        public static CartData FromDC(dcCart nuvCart)
        {
            return new CartData()
            {
                GrossOrder = nuvCart.bGrossOrder.GetValueOrDefault(false),
                DeliveryDate = nuvCart.dtDeliveryDate,
                AlterationDate = nuvCart.dtAlterationDate,
                EntryDate = nuvCart.dtEntryDate,
                ProcessingDate = nuvCart.dtProcessingDate,
                SignDate = nuvCart.dtSignDate,
                SubmitDate = nuvCart.dtSubmitDate,
                DiscountPercent1 = nuvCart.decDiscountPercent1.GetValueOrDefault(0),
                DiscountPercent2 = nuvCart.decDiscountPercent2.GetValueOrDefault(0),
                DiscountPercent3 = nuvCart.decDiscountPercent3.GetValueOrDefault(0),
                PackingCosts = nuvCart.decPackingCosts.GetValueOrDefault(0),
                ShippingCosts = nuvCart.decShippingCosts.GetValueOrDefault(0),
                TotalGrossAmount = nuvCart.decTotalGrossAmount.GetValueOrDefault(0),
                TotalNetAmount = nuvCart.decTotalNetAmount.GetValueOrDefault(0),
                TotalTaxAmount = nuvCart.decTotalTaxAmount.GetValueOrDefault(0),
                CustomerId = (int)nuvCart.lngCustomerID.GetValueOrDefault(-1),
                ProjectId = (int)nuvCart.lngProjectID.GetValueOrDefault(-1),
                CartId = (int)nuvCart.lngCartID.GetValueOrDefault(-1),
                ContactId = (int)nuvCart.lngContactID.GetValueOrDefault(-1),
                SalesOrderId = N<string>(nuvCart.lngSalesOrderID, ""),
                TourId = (int)nuvCart.lngTourID.GetValueOrDefault(-1),
                DeliveryAddress = AddressData.FromDC(nuvCart.oDeliveryAddress),
                Positions = CartDetailData.FromDC(nuvCart.oDetails),
                InvoiceAddress = AddressData.FromDC(nuvCart.oInvoiceAddress),
                CartName = NZ(nuvCart.sCartName),
                CartOderType = NZ(nuvCart.sCartOrderType),
                ContactName = NZ(nuvCart.sContactName),
                CurrencyCode = NZ(nuvCart.sCurrencyCode),
                DiscountText1 = NZ(nuvCart.sDiscountText1),
                DiscountText2 = NZ(nuvCart.sDiscountText2),
                DiscountText3 = NZ(nuvCart.sDiscountText3),
                Email = NZ(nuvCart.sEMail),
                ExternalCartId = NZ(nuvCart.sExtCartID),
                ForeignOrderId = NZ(nuvCart.sForeignOrderID),
                LanguageCode = NZ(nuvCart.sLanguageCode),
                Memo = NZ(nuvCart.sMemo),
                MemoInternal = NZ(nuvCart.sMemoInternal),
                PayConnectionId = NZ(nuvCart.sPayConnectionID),
                PaymentId = NZ(nuvCart.sPaymentID),
                SalesOrderType = NZ(nuvCart.sSalesOrderType),
                SessionId = NZ(nuvCart.sSessionID),
                Username = NZ(nuvCart.sUsername),
                UserSign = NZ(nuvCart.sUserSign),
                CartStatus = (int)nuvCart.shtCartStatus.GetValueOrDefault(-1),
                PaymentConditionId = (int)nuvCart.shtPaymentConditionID.GetValueOrDefault(-1),
                PaymentTypeId = (int)nuvCart.shtPaymentTypeID.GetValueOrDefault(-1),
                ShippingConditionId = (int)nuvCart.shtShippingConditionID.GetValueOrDefault(-1),
                WebShopId = (int)nuvCart.shtWebshopID.GetValueOrDefault(-1),
            };
        }

        public dcCart ToDC()
        {
            var q = new dcCartDetailList();
            q.AddRange(Positions.Select(f => f.ToDC()));

            return new dcCart()
            {
                bGrossOrder = GrossOrder,
                dtDeliveryDate = DeliveryDate,
                dtAlterationDate = AlterationDate,
                dtEntryDate = EntryDate,
                dtProcessingDate = ProcessingDate,
                dtSignDate = SignDate,
                dtSubmitDate = SubmitDate,
                decDiscountPercent1 = DiscountPercent1,
                decDiscountPercent2 = DiscountPercent2,
                decDiscountPercent3 = DiscountPercent3,
                decPackingCosts = PackingCosts,
                decShippingCosts = ShippingCosts,
                decTotalGrossAmount = TotalGrossAmount,
                decTotalNetAmount = TotalNetAmount,
                decTotalTaxAmount = TotalTaxAmount,
                lngCustomerID = CustomerId < 0 ? (long?)null : CustomerId,
                lngProjectID = ProjectId < 0 ? (long?)null : ProjectId,
                lngCartID = CartId < 0 ? (long?)null : CartId,
                lngContactID = ContactId < 0 ? (long?)null : ContactId,
                lngSalesOrderID = SalesOrderId == "" ? (long?)null : long.Parse(SalesOrderId),
                lngTourID = TourId < 0 ? (long?)null : TourId,
                oDeliveryAddress = AddressData.ToDC(DeliveryAddress),
                oDetails = q,
                oInvoiceAddress = AddressData.ToDC(InvoiceAddress),
                sCartName = CartName,
                sCartOrderType = CartOderType,
                sContactName = ContactName,
                sCurrencyCode = CurrencyCode,
                sDiscountText1 = DiscountText1,
                sDiscountText2 = DiscountText2,
                sDiscountText3 = DiscountText3,
                sEMail = Email,
                sExtCartID = ExternalCartId,
                sForeignOrderID = ForeignOrderId,
                sLanguageCode = LanguageCode,
                sMemo = Memo,
                sMemoInternal = MemoInternal,
                sPayConnectionID = PayConnectionId,
                sPaymentID = PaymentId,
                sSalesOrderType = SalesOrderType,
                sSessionID = SessionId,
                sUsername = Username,
                sUserSign = UserSign,
                shtCartStatus = CartStatus < 0 ? (short?)null : (short)CartStatus,
                shtPaymentConditionID = PaymentConditionId < 0 ? (short?)null : (short)PaymentConditionId,
                shtPaymentTypeID = PaymentTypeId < 0 ? (short?)null : (short)PaymentTypeId,
                shtShippingConditionID = ShippingConditionId < 0 ? (short?)null : (short)ShippingConditionId,
                shtWebshopID = WebShopId < 0 ? (short?)null : (short)WebShopId,
            };
        }
    }
}
