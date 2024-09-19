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
    public class Address : WebSvcResponseShell<AddressData>
    {
        public override AddressData Data { get; set; }

        public Address()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    public class AddressData : IWebSvcResponseSeed
    {
        public enum AddressTypeEnum
        {
            Invoice = 1,
            Delivery = 2
        }

        public Guid ObjectId { get; } = Guid.NewGuid();

        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public AddressTypeEnum AddressType { get; set; }
        public int RouteBaseId { get; set; }
        public string Company1 { get; set; }
        public string Company2 { get; set; }
        public string Company3 { get; set; }
        public string Company4 { get; set; }
        public string ZipBox { get; set; }
        public string PostBox { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public static AddressData FromDC(dcAddress Address)
        {
            return new AddressData()
            {
                CustomerId = (int)Address.lngCustomerID.GetValueOrDefault(0),
                AddressId = (int)Address.lngAddressID.GetValueOrDefault(0),
                AddressType = (AddressTypeEnum)Enum.Parse(typeof(AddressTypeEnum), Address.shtAddressType.GetValueOrDefault(2).ToString()),
                RouteBaseId = (int)Address.lngRouteBaseID.GetValueOrDefault(0),
                Company1 = NZ(Address.sCompany1),
                Company2 = NZ(Address.sCompany2),
                Company3 = NZ(Address.sCompany3),
                Company4 = NZ(Address.sCompany4),
                ZipBox = NZ(Address.sZipBox),
                PostBox = NZ(Address.sPostBox),
                Street = NZ(Address.sStreet),
                ZipCode = NZ(Address.sZipCode),
                City = NZ(Address.sCity),
                Country = NZ(Address.sCountry),
                CountryCode = NZ(Address.sCountryCode),
            };
        }

        public static dcAddress ToDC(AddressData Address)
        {
            return new dcAddress()
            {
                lngCustomerID = Address.CustomerId,
                lngAddressID = Address.AddressId,
                shtAddressType = (short)Address.AddressType,
                lngRouteBaseID = Address.RouteBaseId,
                sCompany1 = Address.Company1,
                sCompany2 = Address.Company2,
                sCompany3 = Address.Company3,
                sCompany4 = Address.Company4,
                sZipBox = Address.ZipBox,
                sPostBox = Address.PostBox,
                sStreet = Address.Street,
                sZipCode = Address.ZipCode,
                sCity = Address.City,
                sCountry = Address.Country,
                sCountryCode = Address.CountryCode,
            };
        }

        public static List<dcAddress> ToDC(List<AddressData> Addresses)
        {
            var Result = new List<dcAddress>();
            foreach (var Address in Addresses)
                Result.Add(AddressData.ToDC(Address));

            return Result;
        }

        public static List<AddressData> FromDC(List<dcAddress> Addresses)
        {
            var Result = new List<AddressData>();
            foreach (var Address in Addresses)
                Result.Add(AddressData.FromDC(Address));

            return Result;
        }
    }
}
