using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Nox.Data.SqlServer;
using Nox;
using System.Drawing;
using Nox.WebApi;

namespace GhalaDataDef;


public class Ghala(string ConnectionString)
    : DataModel(ConnectionString)
{
}
