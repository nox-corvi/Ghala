using GhalaDataPool;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MotisDataDef;
using Nox;
using Nox.Data.SqlServer;
using Nox.WebApi;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace MotisDataAccess;

public class MotisStatus(Motis Motis, ILogger Logger, GhalaDataPool.Ghala Ghala)
    : Shell<MotisDataDef.MotisStatus>
{
    public Shell<MotisDataDef.MotisStatus> CountReleasedERPOrders(DateTime Date)
        => Logger.LogShell(CountReleasedERPOrders(Motis, Date));

    public static Shell<MotisDataDef.MotisStatus> CountReleasedERPOrders(Motis Motis, DateTime Date)
    {
        try
        {
            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            using (var reader = DbA.GetReader(
                @"select 
	                cast(datepart(dd, @date) as nvarchar) as d, 
	                count(distinct h.AuftragNr) as ch, 
	                count(p.AuftragNr) as cp 
                from 
	                ERPAuftrag  h 
                inner join 
	                ERPAuftragPosition p on
	                h.AuftragMandant = p.Mandant and h.AuftragNr = p.AuftragNr
                where 
	                convert(date, h.TimeStampErfassu) = convert(date, @date)",
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    return new(StateEnum.Success)
                    {
                        Data = ReaderToObjectList(reader)
                    };
                else
                    return new(StateEnum.Failure, Shell.NO_RESULT);
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.MotisStatus> CountERPOrdersInProgress()
        => Logger.LogShell(CountERPOrdersInProgress(Motis));

    public static Shell<MotisDataDef.MotisStatus> CountERPOrdersInProgress(Motis Motis)
    {
        try
        {
            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            using (var reader = DbA.GetReader(
                @"select 
                    '' as d, 
	                count(distinct p.MasterAuftragNr) as ch, 
	                count(p.MasterAuftragNr) as cp  
                from 
	                AuftragsKopf h inner join 
	                Auftragsposition p on 
	                h.AuftragMandant = p.Mandant and h.AuftragNr = p.AuftragNr
                where 
	                h.AuftragsKopfStat = 3"))

                if (reader.Read())
                    return new(StateEnum.Success)
                    {
                        Data = ReaderToObjectList(reader)
                    };
                else
                    return new(StateEnum.Failure, "not found");
        }
        catch (Exception ex)
        {
            return new Shell<MotisDataDef.MotisStatus>(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.MotisStatus> CountPickOrdersDone(DateTime Date)
        => Logger.LogShell(CountPickOrdersDone(Motis, Date));

    public static Shell<MotisDataDef.MotisStatus> CountPickOrdersDone(Motis Motis, DateTime Date)
    {
        try
        {
            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            using (var reader = DbA.GetReader(
                @"select 
                    cast(datepart(dd, @date) as nvarchar) as d, 
	                count(distinct p.AuftragNr) as ch, 
	                count(p.AuftragNr) as cp  
                from 
	                Auftragsposition p inner join 
	                AuftragsKopf h on 
	                p.Mandant = h.AuftragMandant and p.AuftragNr = h.AuftragNr
                where 
	                h.AuftragsKopfStat = 11 and 
	                convert(date, h.VersandZeit) = @date",
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    return new(StateEnum.Success)
                    {
                        Data = ReaderToObjectList(reader)
                    };
                else
                    return new(StateEnum.Failure, "not found");
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.MotisStatus> CountERPOrdersDone(DateTime Date)
        => Logger.LogShell(CountERPOrdersDone(Motis, Date));

    public static Shell<MotisDataDef.MotisStatus> CountERPOrdersDone(Motis Motis, DateTime Date)
    {
        try
        {
            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            using (var reader = DbA.GetReader(
                @"select 
                    cast(datepart(dd, @date) as nvarchar) as d, 
	                count(distinct p.MasterAuftragNr) as ch, 
	                count(p.MasterPosNr) as cp  
                from 
	                Auftragsposition p inner join 
	                AuftragsKopf h on 
	                p.Mandant = h.AuftragMandant and p.AuftragNr = h.AuftragNr
                where 
	                h.AuftragsKopfStat = 11 and 
	                convert(date, h.VersandZeit) = @date",
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    return new(StateEnum.Success)
                    {
                        Data = ReaderToObjectList(reader)
                    };
                else
                    return new(StateEnum.Failure, "not found");
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.MotisStatus> CountNewERPOrders(DateTime Date)
        => Logger.LogShell(CountNewERPOrders(Motis, Ghala, Date));

    public static Shell<MotisDataDef.MotisStatus> CountNewERPOrders(Motis Motis, Ghala ghalaOption, DateTime Date)
    {
        string Qry = @"
            select
                '{0}' as d, 
		        count(distinct h.AuftragNr) as ch, 
		        count(p.AuftragNr) as cp
            from
                ERPAuftrag h
            inner join
                ERPAuftragPosition p on
                h.AuftragMandant = p.Mandant and h.AuftragNr = p.AuftragNr
            where
                h.[Status] = 0 and
                datediff(dd, @date, convert(date, p.Lieferdatum)) {1}";

        var Result = new Shell<MotisDataDef.MotisStatus>(StateEnum.Success)
        {
            Data = new()
        };

        var optionResult = ghalaOption.GetOption("DELIVERY LEAD TIME");
        if (optionResult.State != StateEnum.Success)
        {
            return optionResult.Pass<Shell<MotisDataDef.MotisStatus>>();
        }

        var DeliveryLeadTime = int.Parse(optionResult.AdditionalData1);
        using (var DbA = new SqlDbAccess(Motis.ConnectionString))
        {
            // BACKLOG
            using (var reader = DbA.GetReader(
                string.Format(Qry, "BACKLOG", $"< {DeliveryLeadTime}"),
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    Result.Data.AddRange(ReaderToObjectList(reader));
                else
                    return new(StateEnum.Failure, "not found");

            // DEFAULT

            using (var reader = DbA.GetReader(
                string.Format(Qry, "TODAY", $"= {DeliveryLeadTime}"),
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    Result.Data.AddRange(ReaderToObjectList(reader));
                else
                    return new(StateEnum.Failure, "not found");

            // TOMORROW
            using (var reader = DbA.GetReader(
                string.Format(Qry, "TOMORROW", $"= {DeliveryLeadTime + 1}"),
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    Result.Data.AddRange(ReaderToObjectList(reader));
                else
                    return new(StateEnum.Failure, "not found");

            // FUTURE
            using (var reader = DbA.GetReader(
                string.Format(Qry, "FUTURE", $"> {DeliveryLeadTime + 1}"),
                new SqlParameter("DATE", Date)))

                if (reader.Read())
                    Result.Data.AddRange(ReaderToObjectList(reader));
                else
                    return new(StateEnum.Failure, "not found");

            return Result;
        }
    }

    public Shell<MotisDataDef.MotisStatus> CountPickOrdersInProgress()
        => CountPickOrdersInProgress(Motis);
    public static Shell<MotisDataDef.MotisStatus> CountPickOrdersInProgress(Motis Motis)
    {
        string Qry = @"
            select
                '{0}' as d,
	            count(distinct h.AuftragNr) as ch, 
	            count(t.AuftragNr) as cp 
            from
	            AuftragsKopf h 
            inner join 
	            Fahrauftrag t on
	            h.AuftragNr = t.AuftragNr
            where
	            h.AuftragsKopfStat = 3 
                {1}";
        try
        {
            var Result = new Shell<MotisDataDef.MotisStatus>(StateEnum.Success)
            {
                Data = new()
            };

            using (var DbA = new SqlDbAccess(Nox.Global.Configuration.GetConnectionString("motis")
                ?? throw new ArgumentNullException("connection string must not be null")))
            {
                // TOTAL
                using (var Status = DbA.GetReader(
                    string.Format(Qry, "TOTAL", $"")))

                    if (Status.Read())
                        Result.Data.AddRange(ReaderToObjectList(Status));
                    else
                        return new(StateEnum.Failure, "not found");

                // OPEN
                using (var Status = DbA.GetReader(
                    string.Format(Qry, "OPEN", $"AND t.KommiZone > -1")))

                    if (Status.Read())
                        Result.Data.AddRange(ReaderToObjectList(Status));
                    else
                        return new(StateEnum.Failure, "not found");

                // READY FOR SHIPPING
                using (var Status = DbA.GetReader(
                    string.Format(Qry, "READY FOR SHIPPING", $"AND t.KommiZone = -1")))

                    if (Status.Read())
                        Result.Data.AddRange(ReaderToObjectList(Status));
                    else
                        return new(StateEnum.Failure, "not found");
            }

            return Result;
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    private static List<MotisDataDef.MotisStatus> ReaderToObjectList(SqlDataReader r)
    {
        return new List<MotisDataDef.MotisStatus>()
        {
            new MotisDataDef.MotisStatus()
            {
                D = r.GetString(r.GetOrdinal("d")),
                CH = r.GetInt32(r.GetOrdinal("CH")),
                CP = r.GetInt32(r.GetOrdinal("CP")),
            }
        };
    }

    // DI Constructor
    public MotisStatus(Motis Motis, IConfiguration Configuration, ILogger<MotisStatus> Logger, GhalaDataPool.Ghala ghalaOption)
        : this(Motis, (ILogger)Logger, ghalaOption) { }
}
