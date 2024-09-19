using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;


namespace NVWebAccess
{
    public class UserProfileData : IWebSvcResponseSeed
    {
        public Guid ObjectId { get; } = Guid.NewGuid();

        /// <summary>
        /// Vorname
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Nachname
        /// </summary>
        public string LastName { get; set; }

        public static UserProfileData FromDC(dcUserProfile o)=>new UserProfileData()
        {
            FirstName = NZ(o?.sFirstName),
            LastName = NZ(o?.sLastName),
        };

        public dcUserProfile ToDC() => new dcUserProfile
        {
            sFirstName = FirstName,
            sLastName = LastName
        };
    }
}
