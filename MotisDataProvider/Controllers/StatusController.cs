using GhalaDataDef;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using MotisDataAccess;
using MotisDataDef;
using Newtonsoft.Json.Linq;
using Nox.WebApi;
using XAuthPool;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotisWebApi1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class StatusController(XAuth XAuth, MotisDataAccess.Motis Motis, IConfiguration Configuration, ILogger<StatusController> Logger)
    : ControllerBase
{
    private string Token
    {
        get => Configuration["MotisDataProvider:Token"] ?? throw new ArgumentNullException("MotisDataProvider:Token");
    }

    [HttpGet()]
    public Shell<MotisDataDef.Status> CountReleasedERPOrders(DateTime Date)
         => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.Status.CountReleasedERPOrders(Motis, Date)));

    [HttpGet()]
    public Shell<MotisDataDef.Status> CountERPOrdersInProgress()
         => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) => 
                MotisDataAccess.Status.CountERPOrdersInProgress(Motis)));

    [HttpGet()]
    public Shell<MotisDataDef.Status> CountPickOrdersDone(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.Status.CountPickOrdersDone(Motis, Date)));
        
    [HttpGet()]
    public Shell<MotisDataDef.Status> CountERPOrdersDone(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.Status.CountERPOrdersDone(Motis, Date)));


    [HttpGet()]
    public Shell<MotisDataDef.Status> CountNewERPOrders(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.Status.CountNewERPOrders(Motis, Date)));


    [HttpGet()]
    public Shell<MotisDataDef.Status> CountPickOrdersInProgress()
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.Status.CountPickOrdersInProgress(Motis)));
}