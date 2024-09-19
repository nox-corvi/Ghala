using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;


namespace NVWebAccess
{
    public class UserData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        public bool IsLockOut { get; set; }

        public DateTime? LastLogin { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public DateTime? LockOut { get; set; }
        public DateTime? Registration { get; set; }
        public DateTime? Alteration { get; set; }
        public DateTime? EntryDate { get; set; }
        public int ContactId { get; set; }
        public int CustomerId { get; set; }
        public UserProfileData UserProfile { get; set; }
        public string Email { get; set; }
        public string LockOutBy { get; set; }
        public string Password { get; set; }
        public string PasswordAnswer { get; set; }
        public string PasswordQuestion { get; set; }
        public string UserName { get; set; }
        public int WebShopId { get; set; }

        public static UserData FromDC(dcUser o) => new UserData()
        {
            IsLockOut = o.bLockOut.GetValueOrDefault(false),
            LastLogin = o.dtLastLogin,
            LastPasswordChange = o.dtLastPasswordChange,
            LockOut = o.dtLockOut,
            Registration = o.dtRegistration,
            Alteration = o.dtAlterationDate,
            EntryDate = o.dtEntryDate,
            ContactId= (int)o.lngContactID.GetValueOrDefault(-1),
            CustomerId = (int)o.lngCustomerID.GetValueOrDefault(-1),
            UserProfile = UserProfileData.FromDC(o.oProfile),
            Email = NZ(o.sEmail),
            LockOutBy = NZ(o.sLockOutBy),
            Password = NZ(o.sPassword),
            PasswordAnswer = NZ(o.sPasswordAnswer),
            PasswordQuestion = NZ(o.sPasswordQuestion),
            UserName = NZ(o.sUserName),
            WebShopId = o.shtWebshopID.GetValueOrDefault(-1),
        };

        public dcUser ToDC() => new dcUser()
        {
            bLockOut = IsLockOut,
            dtLastLogin = LastLogin,
            dtLastPasswordChange = LastPasswordChange,
            dtLockOut = LockOut,
            dtRegistration = Registration,
            dtAlterationDate = Alteration,
            dtEntryDate = EntryDate,
            lngContactID = ContactId,
            lngCustomerID = CustomerId,
            oProfile = UserProfile.ToDC(),
            sEmail = Email,
            sLockOutBy = LockOutBy,
            sPassword = Password,
            sPasswordAnswer = PasswordAnswer,
            sPasswordQuestion = PasswordQuestion,
            sUserName = UserName,
            shtWebshopID = (short)WebShopId,
        };
    }
}
