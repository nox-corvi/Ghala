using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;

namespace NVWebAccess
{
    public class PriceDiscountData 
        : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountValue { get; set; }
        public int DiscountId { get; set; }
        public string DiscountFlag { get; set; }
        public string DiscountText { get; set; }
        public string DiscountType { get; set; }

        public static PriceDiscountData FromDC(dcPriceDiscount o)
        {
            return new PriceDiscountData()
            {
                DiscountAmount = o.decDiscountAmount.GetValueOrDefault(0),
                DiscountPercent = o.decDiscountPercent.GetValueOrDefault(0),
                DiscountValue = o.decDiscountValue.GetValueOrDefault(0),
                DiscountId = (int)o.lngDiscountID.GetValueOrDefault(0),
                DiscountFlag = NZ(o.sDiscountFlag),
                DiscountText = NZ(o.sDiscountText),
                DiscountType = NZ(o.sDiscountType)
            };
        }

        public dcPriceDiscount ToDC()
        {
            return new dcPriceDiscount()
            {
                decDiscountAmount = DiscountAmount,
                decDiscountPercent = DiscountPercent,
                decDiscountValue = DiscountValue,
                lngDiscountID = DiscountId,
                sDiscountFlag = DiscountFlag,
                sDiscountText = DiscountText,
                sDiscountType = DiscountType,
            };
        }
    }
}
