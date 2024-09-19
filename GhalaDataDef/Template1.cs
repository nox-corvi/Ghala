using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Nox.WebApi;

namespace GhalaDataDef;

public class Template1
    : DataRow
{
    [ColumnGuid("id", Order = -10000), PrimaryKey]
    public override Guid Id { get; set; } = Guid.NewGuid();
}
