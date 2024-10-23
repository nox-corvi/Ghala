using Libs.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nox;
using Nox.Data;
using Nox.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhalaDataPool;

public class Ghala(IConfiguration Configuration, ILogger Logger)
    : RestApi(Configuration, Logger)
{
    public override string ConfigKey => "GhalaDataProvider";

    public async Task<SingleDataResponseShell> GetOptionAsync(string KeyValue)
        => await Logger.LogShellAsync(await RestGetAsync<SingleDataResponseShell>(BuildApiPath("GetOption", PA(KeyValue)), Token));

    public SingleDataResponseShell GetOption(string KeyValue)
        => Logger.LogShell(RestGet<SingleDataResponseShell>(BuildApiPath("GetOption", PA(KeyValue)), Token));

    // DI-Constructor
    public Ghala(IConfiguration Configuration, ILogger<Ghala> Logger)
        : this(Configuration, (ILogger)Logger) { }
}