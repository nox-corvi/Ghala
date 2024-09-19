using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MotisDataAccess;
using Nox.WebApi;
using XAuthPool;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotisWebApi1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PickingZoneController(XAuth XAuth, Motis Motis, IConfiguration Configuration, ILogger<PickingZoneController> Logger)
    : ControllerBase
{
    private KeyValue Token
    {
        get => new KeyValue("Token",
            Configuration["MotisDataProvider:Token"])
                ?? throw new ArgumentNullException("MotisDataProvider:Token");
    }

    [HttpGet()]
    public Shell<MotisDataDef.PickingZone> GetPickingZones(string Token)
            => Shell.SuccessHandler(
                XAuth.ValidateToken(Token, "read pickingzone"), (s) =>
                {
                    return Shell.SuccessHandler(PickingZone.GetPickingZones(Motis),
                        (r) => r);
                });
}
