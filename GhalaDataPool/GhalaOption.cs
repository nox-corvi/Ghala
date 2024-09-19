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

public class GhalaOption(IConfiguration Configuration, ILogger Logger)
    : RestClient(Configuration["GhalaDataProvider:URL"] ??
        throw new ArgumentNullException("GhalaDataProvider:URL"), Logger)
{
    private KeyValue Token
    {
        get => new KeyValue("Token",
            Configuration["GhalaDataProvider:Token"])
                ?? throw new ArgumentNullException("GhalaDataProvider:Token");
    }

    public Task<Shell<GhalaDataDef.GhalaOption>> GetOptionAsync(string KeyValue)
        => RestGetAsync<Shell<GhalaDataDef.GhalaOption>>($"Option/GetOption?KeyValue={KeyValue}", Token);

    public Shell<GhalaDataDef.GhalaOption> GetOption(string KeyValue)
        => RestGet<Shell<GhalaDataDef.GhalaOption>>($"Option/GetOption?KeyValue={KeyValue}", Token);

    // DI-Constructor
    public GhalaOption(IConfiguration Configuration, ILogger<GhalaOption> Logger)
        : this(Configuration, (ILogger)Logger) { }
}