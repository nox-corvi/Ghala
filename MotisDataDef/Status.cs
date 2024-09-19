using Microsoft.Extensions.Configuration;
using Nox;
using Nox.Data.SqlServer;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Nox.WebApi;

namespace MotisDataDef;

public class Status
{
    public string D { get; set; } = "";

    public int CH { get; set; } = 0;

    public int CP { get; set; } = 0;
}
