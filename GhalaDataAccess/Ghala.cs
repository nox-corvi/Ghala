using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Nox.WebApi;
using Microsoft.Extensions.Logging;
using Nox.Hosting;

namespace GhalaDataAccess;

public class Ghala(IConfiguration Configuration, ILogger Logger)
    : GhalaDataDef.Ghala(Configuration.GetConnectionString("ghala") ?? 
        throw new ArgumentNullException("ghala"))
{
    #region Access Methods
    public DataShell<GhalaDataDef.GhalaOption> GetOption(string KeyValue)
    {
        Logger.LogInformation($"GetOption '{KeyValue}'");

        return GhalaOption.GetWhereKeyValue(this, KeyValue);
    }
    #endregion

    // DI
    public Ghala(IConfiguration Configuration, ILogger<Ghala> Logger)
        : this(Configuration, (ILogger)Logger) { }
}