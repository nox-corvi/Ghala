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
    public class Contact : WebSvcResponseShell<ContactData>
    {
        public override ContactData Data { get; set; }

        public static Contact GetContactById(WebSvcConnect svc, int ContactId)
        {
            try
            {
                // enventa websvc call
                var nuvContact = svc.GetContactById(ContactId);
                if (nuvContact.Status == 1)
                    return new Contact()
                    {
                        State = WebSvcResult.Ok,
                        Data = ContactData.FromDC(nuvContact)
                    };
                else
                    return new Contact()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvContact.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Contact()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Contact CreateContact(WebSvcConnect svc, int CustomerId, string FormOfAddress, string LastName, string FirstName)
        {
            try
            {
                // enventa websvc call
                var nuvContact = svc.CreateContact(CustomerId, FormOfAddress, LastName, FirstName);
                if (nuvContact.Status == 1)
                    return new Contact()
                    {
                        State = WebSvcResult.Ok,
                        Data = ContactData.FromDC(nuvContact)
                    };
                else
                    return new Contact()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvContact.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Contact()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public static Contact SaveContact(WebSvcConnect svc, Contact o)
        {
            try
            {
                if (o.State != WebSvcResult.Ok)
                    return o;

                var nuvContact = svc.SaveContact(o.Data.CustomerId, o.Data.ToDC());
                if (nuvContact.Status == 1)
                    return new Contact()
                    {
                        State = WebSvcResult.Ok,
                        Data = ContactData.FromDC(nuvContact)
                    };
                else
                    return new Contact()
                    {
                        State = WebSvcResult.NoResult,
                        Message = Helper.nuvMessageToString(nuvContact.Messages),
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));
                return new Contact()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }


        public static List<Contact> GetContactsByCustomer(WebSvcConnect svc, int CustomerId)
        {
            try
            {
                // enventa websvc call
                var nuvContact = svc.GetContactByCustomer(CustomerId);

                var Result = new List<Contact>();
                foreach (var Item in nuvContact)
                    if (Item.Status == null)
                        Result.Add(new Contact()
                        {
                            State = WebSvcResult.Ok,
                            Data = ContactData.FromDC(Item)
                        });
                    else
                        Result.Add(new Contact()
                        {
                            State = WebSvcResult.NoResult,
                            Message = Helper.nuvMessageToString(Item.Messages),
                        });

                return Result;
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));

                return new List<Contact>()
                {
                    new Contact()
                    {
                        State = WebSvcResult.Error,
                        Message = SerializeException(ex)
                    }
                };
            }
        }

        public static Contact GetContactByEmail(WebSvcConnect svc, int CustomerId, string Email)
        {
            try
            {
                // enventa websvc call
                var ContactList = Contact.GetContactsByCustomer(svc, CustomerId);
                var ContactMatch = ContactList.FindAll(match => match.State == WebSvcResult.Ok).FirstOrDefault(match => match.Data.Email.Equals(Email, StringComparison.InvariantCultureIgnoreCase));

                if (ContactMatch != null)
                    return ContactMatch;
                else
                    return new Contact()
                    {
                        State = WebSvcResult.NoResult,
                        Message = $"{Email} not found for Customer #{CustomerId}"
                    };
            }
            catch (Exception ex)
            {
                Global.Logger.LogError(SerializeException(ex));

                return new Contact()
                {
                    State = WebSvcResult.Error,
                    Message = SerializeException(ex)
                };
            }
        }

        public Contact()
             : base(Global.Logger, Global.Configuration)
        {

        }
    }

    [Serializable()]
    public class ContactData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// ArtikelID
        /// </summary>
        public int ContactId { get; set; } = -1;

        /// <summary>
        /// Der zugehörige Kunde
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Stadt
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Firma 1
        /// </summary>
        public string Company1 { get; set; }

        /// <summary>
        /// Firma 2
        /// </summary>
        public string Company2 { get; set; }

        /// <summary>
        /// Land
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Länder-Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Abteilung
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Fax
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Vorname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Nachname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Briefanrede
        /// </summary>
        public string FormOfAddress { get; set; }

        /// <summary>
        /// Telefon
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Straße
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Titel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Postleitzahl
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Mobilefunk
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Code 1
        /// </summary>
        public string Code1 { get; set; }

        /// <summary>
        /// Code 2
        /// </summary>
        public string Code2 { get; set; }

        /// <summary>
        /// Code 3
        /// </summary>
        public string Code3 { get; set; }

        /// <summary>
        /// Konvertiert ein dcArticle-Objekt in ein 
        /// </summary>
        /// <param name="nuvArticle">ein NV dc-Article-Objekt</param>
        /// <returns>Ein ArticleInfoObject</returns>
        internal static ContactData FromDC(dcContact nuvContact)
        {
            return new ContactData()
            {
                CustomerId = (int)nuvContact.lngCustomerID.GetValueOrDefault(-1),
                ContactId = (int)nuvContact.lngContactID,
                City = NZ(nuvContact.sCity),
                Company1 = NZ(nuvContact.sCompany1),
                Company2 = NZ(nuvContact.sCompany2),
                Country = NZ(nuvContact.sCountry),
                CountryCode = NZ(nuvContact.sCountryCode),
                Department = NZ(nuvContact.sDepartment),
                Position = NZ(nuvContact.sPosition),
                Email = NZ(nuvContact.sEmail),
                Fax = NZ(nuvContact.sFax),
                FirstName = NZ(nuvContact.sFirstName),
                LastName = NZ(nuvContact.sLastName),
                FormOfAddress = NZ(nuvContact.sFormOfAddress),
                Phone = NZ(nuvContact.sPhone),
                Mobile = NZ(nuvContact.sMobile),
                Street = NZ(nuvContact.sStreet),
                Title = NZ(nuvContact.sTitle),
                ZipCode = NZ(nuvContact.sZipCode),
                Code1 = NZ(nuvContact.sCode1),
                Code2 = NZ(nuvContact.sCode2),
                Code3 = NZ(nuvContact.sCode3),

            };
        }

        internal dcContact ToDC()
        {
            return new dcContact()
            {
                lngContactID = ContactId,
                lngCustomerID = this.CustomerId,
                sCity = this.City,
                sCompany1 = this.Company1,
                sCompany2 = this.Company2,
                sCountry = this.Country,
                sCountryCode = this.CountryCode,
                sDepartment = this.Department,
                sPosition = this.Position,
                sEmail = this.Email,
                sFax = this.Fax,
                sFirstName = this.FirstName,
                sLastName = this.LastName,
                sFormOfAddress = this.FormOfAddress,
                sPhone = this.Phone,
                sMobile = this.Mobile,
                sStreet = this.Street,
                sTitle = this.Title,
                sZipCode = this.ZipCode,
                sCode1 = this.Code1,
                sCode2 = this.Code2,
                sCode3 = this.Code3,
            };
        }
    }
}