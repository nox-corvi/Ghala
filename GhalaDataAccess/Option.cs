using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Nox.WebApi;
using GhalaDataDef;

namespace GhalaDataAccess;


public class GhalaOption(Ghala Ghala)
    : DataShell<GhalaDataDef.GhalaOption>(Ghala)
{
    public DataShell<GhalaDataDef.GhalaOption> GetWhereKeyValue(string KeyValue)
        => GetWhereKeyValue(KeyValue);

    public static DataShell<GhalaDataDef.GhalaOption> GetWhereKeyValue(Ghala Ghala, string KeyValue)
        => new GhalaOption(Ghala).Select("key_value = @key_value",
            new SqlParameter("key_value", KeyValue));
}
