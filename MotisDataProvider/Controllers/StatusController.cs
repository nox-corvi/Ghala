using GhalaDataDef;
using GhalaDataPool;
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
public class StatusController(XAuth XAuth, MotisDataAccess.Motis Motis, IConfiguration Configuration, ILogger<StatusController> Logger, GhalaDataPool.Ghala ghalaOption)
    : ControllerBase
{
    private string Token
    {
        get => Configuration["MotisDataProvider:Token"] ?? throw new ArgumentNullException("MotisDataProvider:Token");
    }

    
}