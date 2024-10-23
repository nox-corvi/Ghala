//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Nox.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Nox.WebApi;

//namespace MotisDataAccess;

//public class GhalaOption(IConfiguration Configuration, ILogger<GhalaOption> Logger)
//{
//    private static readonly object _lock = new object();

//    protected static GhalaOption _self;

//    private string _DPDTrackingURL = null!;
//    private string _UPSTrackingURL = null!;
//    private string _GLSTrackingURL = null!;
//    private string _TrackingURLWildcard = null!;
//    private int? _DeliveryLeadTime = null!;
//    private string _Token = null!;

//    #region Properties
//    public static string DPDTrackingURL
//    {
//        get => _self._DPDTrackingURL
//            ?? (_self._DPDTrackingURL = GetOption("TRACKING URL DPD"));
//    }
//    public static string UPSTrackingURL
//    {
//        get => _self._UPSTrackingURL
//            ?? (_self._UPSTrackingURL = GetOption("TRACKING URL UPS"));
//    }
//    public static string GLSTrackingURL
//    {
//        get => _self._GLSTrackingURL
//            ?? (_self._GLSTrackingURL = GetOption("TRACKING URL GLS"));
//    }

//    public static string TrackingURLWildcard
//    {
//        get => _self._TrackingURLWildcard
//            ?? (_self._TrackingURLWildcard = GetOption("TRACKING URL WILDCARD"));
//    }

//    public static int DeliveryLeadTime
//    {
//        get => _self._DeliveryLeadTime
//            ?? (int)(_self._DeliveryLeadTime = int.Parse(GetOption("DELIVERY LEAD TIME")));
//    }

//    //public static string Token
//    //{
//    //    get => _self._Token ?? Global.Token;
//    //}
//    #endregion

//    protected static SingleDataResponseShell GetOption(string KeyValue)
//    {
//        var Result = new GhalaDataPool.GhalaOption(Nox.Global.Configuration).GetOption(KeyValue);
//        if (Result.State == StateEnum.Success)
//            return Result.First().KeyValue;
//        else
//            throw new Exception("blub");

//    }

//    public GhalaOption(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }

//    static GhalaOption()
//    {
//        _self = new GhalaOption();
//    }
//}
