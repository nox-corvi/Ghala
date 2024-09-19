using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Nox;
using Nox.Data;
using Nox.IO.DF;
using Nox.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MotisDataPool.Data;

public class Status(IConfiguration Configuration, ILogger Logger)
    : RestClient(Configuration["MotisDataProvider:URL"] ??
        throw new ArgumentNullException("MotisDataProvider:URL"), Logger)
{
    private const string DTF = "yyyy-MM-dd";

    private KeyValue Token
    {
        get => new KeyValue("Token",
            Configuration["MotisDataProvider:Token"])
                ?? throw new ArgumentNullException("MotisDataProvider:Token");
    }

    public Task<Shell<MotisDataDef.Status>> CountReleasedERPOrdersAsync(DateTime Date)
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountReleasedERPOrders?Date={Date.ToString(DTF)}", Token);
    public Shell<MotisDataDef.Status> CountReleasedERPOrders(DateTime Date)
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountReleasedERPOrders?Date={Date.ToString(DTF)}", Token);

    public Task<Shell<MotisDataDef.Status>> CountERPOrdersInProgressAsync()
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountERPOrdersInProgress", Token);
    public Shell<MotisDataDef.Status> CountERPOrdersInProgress()
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountERPOrdersInProgress", Token);

    public Task<Shell<MotisDataDef.Status>> CountPickOrdersDoneAsync(DateTime Date)
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountPickOrdersDone?Date={Date.ToString(DTF)}", Token);
    public Shell<MotisDataDef.Status> CountPickOrdersDone(DateTime Date)
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountPickOrdersDone?Date={Date.ToString(DTF)}", Token);

    public Task<Shell<MotisDataDef.Status>> CountERPOrdersDoneAsync(DateTime Date)
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountERPOrdersDone?Date={Date.ToString(DTF)}", Token);
    public Shell<MotisDataDef.Status> CountERPOrdersDone(DateTime Date)
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountERPOrdersDone?Date={Date.ToString(DTF)}", Token);

    public Task<Shell<MotisDataDef.Status>> CountNewERPOrdersAsync(DateTime Date)
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountNewERPOrders?Date={Date.ToString(DTF)}", Token);
    public Shell<MotisDataDef.Status> CountNewERPOrders(DateTime Date)
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountNewERPOrders?Date={Date.ToString(DTF)}", Token);

    public Task<Shell<MotisDataDef.Status>> CountPickOrdersInProgressAsync()
        => RestGetAsync<Shell<MotisDataDef.Status>>($"Status/CountPickOrdersInProgress", Token);
    public Shell<MotisDataDef.Status> CountPickOrdersInProgress()
        => RestGet<Shell<MotisDataDef.Status>>($"Status/CountPickOrdersInProgress", Token);

    public Status(IConfiguration Configuration, ILogger<Status> Logger)
        : this(Configuration, (ILogger)Logger) { }
}
