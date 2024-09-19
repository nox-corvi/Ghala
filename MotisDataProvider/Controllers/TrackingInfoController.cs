using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MotisDataAccess;
using Nox;
using Nox.WebApi;
using XAuthPool;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotisWebApi1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TrackingInfoController(IConfiguration Configuration, XAuth XAuth, Motis Motis, ILogger<TrackingInfoController> Logger)
    : ControllerBase
{
    private string Token
    {
        get => Configuration["MotisDataProvider:Token"] ?? throw new ArgumentNullException("MotisDataProvider:Token");
    }

    [HttpGet()]
    public Shell<MotisDataDef.TrackingInfo> GetTrackingInfo(string OrderId, int FixedItemPos, string Branch)
        => Shell.SuccessHandler(
            XAuth.ValidateToken(Token, "read trackinginfo"), (s) =>
                TrackingInfo.GetTrackingByPackingListId(Motis, OrderId, FixedItemPos, Branch));
}
