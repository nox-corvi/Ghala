using Libs.Web;
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

namespace MotisDataPool;

public class Motis(IConfiguration Configuration, ILogger Logger)
    : RestApi(Configuration, Logger)
{
    public override string ConfigKey => "MotisDataProvider";

    #region Status

    public async Task<Shell<MotisDataDef.MotisStatus>> CountReleasedERPOrdersAsync(DateTime Date)
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountReleasedERPOrders", $"Date={Date.ToString(DTF)}"), Token));
    public Shell<MotisDataDef.MotisStatus> CountReleasedERPOrders(DateTime Date)
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountReleasedERPOrders", $"Date={Date.ToString(DTF)}"), Token));

    public async Task<Shell<MotisDataDef.MotisStatus>> CountERPOrdersInProgressAsync()
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountERPOrdersInProgress"), Token));
    public Shell<MotisDataDef.MotisStatus> CountERPOrdersInProgress()
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountERPOrdersInProgress"), Token));

    public async Task<Shell<MotisDataDef.MotisStatus>> CountPickOrdersDoneAsync(DateTime Date)
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountPickOrdersDone", $"Date={Date.ToString(DTF)}"), Token));
    public Shell<MotisDataDef.MotisStatus> CountPickOrdersDone(DateTime Date)
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountPickOrdersDone?Date={Date.ToString(DTF)}"), Token));

    public async Task<Shell<MotisDataDef.MotisStatus>> CountERPOrdersDoneAsync(DateTime Date)
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountERPOrdersDone", $"Date={Date.ToString(DTF)}"), Token));
    public Shell<MotisDataDef.MotisStatus> CountERPOrdersDone(DateTime Date)
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountERPOrdersDone", $"Date={Date.ToString(DTF)}"), Token));

    public async Task<Shell<MotisDataDef.MotisStatus>> CountNewERPOrdersAsync(DateTime Date)
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountNewERPOrders", $"Date={Date.ToString(DTF)}"), Token));
    public Shell<MotisDataDef.MotisStatus> CountNewERPOrders(DateTime Date)
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountNewERPOrders", $"Date={Date.ToString(DTF)}"), Token));

    public async Task<Shell<MotisDataDef.MotisStatus>> CountPickOrdersInProgressAsync()
        => await Logger.LogShellAsync(await RestGetAsync<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountPickOrdersInProgress"), Token));
    public Shell<MotisDataDef.MotisStatus> CountPickOrdersInProgress()
        => Logger.LogShell(RestGet<Shell<MotisDataDef.MotisStatus>>(BuildApiPath("StatusCountPickOrdersInProgress"), Token));
    #endregion


    // DI-Constructor
    public Motis(IConfiguration Configuration, ILogger<Motis> Logger)
        : this(Configuration, (ILogger)Logger) { }
}




//public class Motis(IConfiguration Configuration, ILogger Logger)
//    : RestClient(Configuration["MotisDataProvider:URL"] ??
//        throw new ArgumentNullException("MotisDataProvider:URL"), Logger)
//{
//    private const string DTF = "yyyy-MM-dd";

//    private KeyValue Token
//    {
//        get => new KeyValue("Token",
//            Configuration["MotisDataProvider:Token"])
//                ?? throw new ArgumentNullException("MotisDataProvider:Token");
//    }

//    public Task<Shell<MotisDataDef.MotisStatus>> CountReleasedERPOrdersAsync(DateTime Date)
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountReleasedERPOrders?Date={Date.ToString(DTF)}", Token);
//    public Shell<MotisDataDef.MotisStatus> CountReleasedERPOrders(DateTime Date)
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountReleasedERPOrders?Date={Date.ToString(DTF)}", Token);

//    public Task<Shell<MotisDataDef.MotisStatus>> CountERPOrdersInProgressAsync()
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountERPOrdersInProgress", Token);
//    public Shell<MotisDataDef.MotisStatus> CountERPOrdersInProgress()
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountERPOrdersInProgress", Token);

//    public Task<Shell<MotisDataDef.MotisStatus>> CountPickOrdersDoneAsync(DateTime Date)
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountPickOrdersDone?Date={Date.ToString(DTF)}", Token);
//    public Shell<MotisDataDef.MotisStatus> CountPickOrdersDone(DateTime Date)
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountPickOrdersDone?Date={Date.ToString(DTF)}", Token);

//    public Task<Shell<MotisDataDef.MotisStatus>> CountERPOrdersDoneAsync(DateTime Date)
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountERPOrdersDone?Date={Date.ToString(DTF)}", Token);
//    public Shell<MotisDataDef.MotisStatus> CountERPOrdersDone(DateTime Date)
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountERPOrdersDone?Date={Date.ToString(DTF)}", Token);

//    public Task<Shell<MotisDataDef.MotisStatus>> CountNewERPOrdersAsync(DateTime Date)
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountNewERPOrders?Date={Date.ToString(DTF)}", Token);
//    public Shell<MotisDataDef.MotisStatus> CountNewERPOrders(DateTime Date)
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountNewERPOrders?Date={Date.ToString(DTF)}", Token);

//    public Task<Shell<MotisDataDef.MotisStatus>> CountPickOrdersInProgressAsync()
//        => RestGetAsync<Shell<MotisDataDef.MotisStatus>>($"Status/CountPickOrdersInProgress", Token);
//    public Shell<MotisDataDef.MotisStatus> CountPickOrdersInProgress()
//        => RestGet<Shell<MotisDataDef.MotisStatus>>($"Status/CountPickOrdersInProgress", Token);

//    public Motis(IConfiguration Configuration, ILogger<Motis> Logger)
//        : this(Configuration, (ILogger)Logger) { }
//}
