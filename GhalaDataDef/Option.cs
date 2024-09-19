using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Nox.WebApi;

namespace GhalaDataDef;

[Table("ghala_option")]
public class GhalaOption
    : Template1
{
    [ColumnString("key_value", 50)]
    public string KeyValue { get; set; } = "";

    [ColumnString("data", -1)]
    public string Data { get; set; } = "";
}