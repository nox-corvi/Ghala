using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nox;
using Nox.Data;
using Nox.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MotisDataPool.Data;

public class TrackingInfo(IConfiguration Configuration, ILogger Logger)
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

    public Task<MotisDataDef.MotisTrackingInfo> GetTrackingInfoAsync(string OrderId, int FixedItemPos)
        => RestGetAsync<MotisDataDef.MotisTrackingInfo>($"TrackingInfo/GetTrackingInfo?OrderId={OrderId}&FixedItemPos={FixedItemPos}", Token);

    public MotisDataDef.MotisTrackingInfo GetTrackingInfo(string OrderId, int FixedItemPos)
        => RestGet<MotisDataDef.MotisTrackingInfo>($"TrackingInfo/GetTrackingInfo?OrderId={OrderId}&FixedItemPos={FixedItemPos}", Token);

    public TrackingInfo(IConfiguration Configuration, ILogger<TrackingInfo> Logger)
        : this(Configuration, (ILogger)Logger) { }
}
