using MotisDataAccess;
using GhalaDataPool;
using XAuthPool;
using Microsoft.AspNetCore.Mvc;
using Nox.WebApi;

namespace MotisDataProvider.Controllers;


[ApiController]
[Route("v1/[action]")]
public class MotisV1(XAuth XAuth, Motis Motis, Ghala Ghala, ILogger<MotisV1> Logger)
    : Nox.Web.WebApi(Logger)
{
    protected override string RootKey => "Motis";

    #region Status
    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> StatusCountReleasedERPOrders(DateTime Date)
         => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountReleasedERPOrders(Motis, Date)));

    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> StatusCountERPOrdersInProgress()
         => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountERPOrdersInProgress(Motis)));

    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> StatusCountPickOrdersDone(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountPickOrdersDone(Motis, Date)));

    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> StatusCountERPOrdersDone(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountERPOrdersDone(Motis, Date)));


    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> StatusCountNewERPOrders(DateTime Date)
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountNewERPOrders(Motis, Ghala, Date)));


    [HttpGet()]
    public Shell<MotisDataDef.MotisStatus> CountPickOrdersInProgress()
        => Logger.LogShell(Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read status"), (s) =>
                MotisDataAccess.MotisStatus.CountPickOrdersInProgress(Motis)));
    #endregion
}
