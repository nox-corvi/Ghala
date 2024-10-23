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

namespace MotisDataPool.Data;


//public class Motis(IConfiguration Configuration, ILogger Logger)
//    : RestApi(Configuration, Logger)
//{
//    public override string ConfigKey => "MotisDataProvider";

//    #region Status

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

//    #endregion


//    // DI-Constructor
//    public Motis(IConfiguration Configuration, ILogger<Motis> Logger)
//        : this(Configuration, (ILogger)Logger) { }
//}
