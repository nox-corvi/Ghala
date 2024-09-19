using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nox;
using static Nox.Helpers;
using NVWebAccessSvc;

namespace NVWebAccess
{
    public class Helper
    {
        /// <summary>
        /// Wandelt die Nachrichten in einer dcMessageList in einen String um
        /// </summary>
        /// <param name="Messages">das Nachrichten-Objekt von NuV</param>
        /// <returns>Ein String</returns>
        public static string nuvMessageToString(dcMessageList Messages) => 
            Messages.Count > 0 ? Messages.Select(query => query.Text).Aggregate((a, b) => a + ", " + b) : "";
    }
}
