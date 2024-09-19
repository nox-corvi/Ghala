using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Nox.WebApi;
using Microsoft.Extensions.Logging;

namespace MotisDataAccess;

public class Motis(IConfiguration configuration, ILogger logger)
    : MotisDataDef.Motis(configuration.GetConnectionString("motis") ?? 
        throw new ArgumentNullException("motis"))
{
    public IConfiguration Configuration { get => configuration; }

    public ILogger Logger { get => logger; }

    #region Access Methods
    //public GhalaOption GetStatus(string KeyValue)
    //{
    //    logger.LogDebug("GetOption {0}", KeyValue);

    //    return GhalaOption.GetWhereKeyValue(this, KeyValue);
    //}

    #endregion

    public Motis(IConfiguration Configuration, ILogger<Motis> Logger) 
        : this(Configuration, (ILogger)Logger) { }
}