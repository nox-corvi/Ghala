using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;


namespace NVWebAccess
{
    [Serializable]
    public class CartDetail : WebSvcResponseShell<CartData>
    {
        public override CartData Data { get; set; }

        public CartDetail()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class CartDetailData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public decimal ItemGrossAmount { get; set; }
        public decimal ItemNetAmount { get; set; }
        public decimal ItemTaxAmount { get; set; }
        public decimal PriceFactor { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal TaxRate { get; set; }
        public int CartId { get; set; }
        public int SalesPriceUnit { get; set; }
        public PriceDiscountData Discount1 { get; set; }
        public PriceDiscountData Discount2 { get; set; }
        public PriceDiscountData Discount3 { get; set; }
        public PriceDiscountData Discount4 { get; set; }
        public PriceDiscountData Discount5 { get; set; }
        public PriceDiscountData Discount6 { get; set; }
        public string ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string ExternalItemId { get; set; }
        public string ItemText { get; set; }
        public string QuantityUnit { get; set; }
        public int CartDetailStatus { get; set; }
        public int HasAdditionalItems { get; set; }
        public int ItemId { get; set; }

        public static CartDetailData FromDC(dcCartDetail nuvCartDetail)
        {
            return new CartDetailData()
            {
                ItemGrossAmount = nuvCartDetail.decItemGrossAmount.GetValueOrDefault(0),
                ItemNetAmount = nuvCartDetail.decItemNetAmount.GetValueOrDefault(0),
                ItemTaxAmount = nuvCartDetail.decItemTaxAmount.GetValueOrDefault(0),
                PriceFactor = nuvCartDetail.decPriceFactor.GetValueOrDefault(0),
                Quantity = nuvCartDetail.decQuantity.GetValueOrDefault(0),
                SalesPrice = nuvCartDetail.decSalesPrice.GetValueOrDefault(0),
                TaxRate = nuvCartDetail.decTaxRate.GetValueOrDefault(0),
                CartId = (int)nuvCartDetail.lngCartID.GetValueOrDefault(0),
                SalesPriceUnit = (int)nuvCartDetail.lngSalesPriceUnit.GetValueOrDefault(0),
                Discount1 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount1),
                Discount2 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount2),
                Discount3 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount3),
                Discount4 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount4),
                Discount5 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount5),
                Discount6 = PriceDiscountData.FromDC(nuvCartDetail.oDiscount6),
                ArticleId = NZ(nuvCartDetail.sArticleID),
                ArticleName = NZ(nuvCartDetail.sArticleName),
                ExternalItemId = NZ(nuvCartDetail.sExtItemID),
                ItemText = NZ(nuvCartDetail.sItemText),
                QuantityUnit = NZ(nuvCartDetail.sQuantityUnit),
                CartDetailStatus = nuvCartDetail.shtCartDetailStatus.GetValueOrDefault(0),
                HasAdditionalItems = nuvCartDetail.shtHasAdditionalItems.GetValueOrDefault(0),
                ItemId = nuvCartDetail.shtItemID.GetValueOrDefault(-1),
            };
        }

        public dcCartDetail ToDC() =>
            new dcCartDetail()
            {
                decItemGrossAmount = ItemGrossAmount,
                decItemNetAmount = ItemNetAmount,
                decItemTaxAmount = ItemTaxAmount,
                decPriceFactor = PriceFactor,
                decQuantity = Quantity,
                decSalesPrice = SalesPrice,
                decTaxRate = TaxRate,
                lngCartID = CartId,
                lngSalesPriceUnit = SalesPriceUnit,
                oDiscount1 = Discount1.ToDC(),
                oDiscount2 = Discount2.ToDC(),
                oDiscount3 = Discount3.ToDC(),
                oDiscount4 = Discount4.ToDC(),
                oDiscount5 = Discount5.ToDC(),
                oDiscount6 = Discount6.ToDC(),
                sArticleID = ArticleId,
                sArticleName = ArticleName,
                sExtItemID = ExternalItemId,
                sItemText = ItemText,
                sQuantityUnit = QuantityUnit,
                shtCartDetailStatus = (short)CartDetailStatus,
                shtHasAdditionalItems = (short)HasAdditionalItems,
                shtItemID = (short)ItemId,
            };

        public static List<CartDetailData> FromDC(List<dcCartDetail> nuvCartDetails)
        {
            var Result = new List<CartDetailData>();

            foreach (var Item in nuvCartDetails)
                Result.Add(CartDetailData.FromDC(Item));

            return Result;
        }
    }
}
