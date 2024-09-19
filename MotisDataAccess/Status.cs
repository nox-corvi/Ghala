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

public class Status(Motis Motis, IConfiguration Configuration, ILogger Logger)
    : Shell<MotisDataDef.Status>
{
    public Shell<MotisDataDef.Status> CountReleasedERPOrders(DateTime Date)
        => CountReleasedERPOrders(Motis, Date);

    public static Shell<MotisDataDef.Status> CountReleasedERPOrders(Motis Motis, DateTime Date)
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
                    return new(StateEnum.Failure, "not found");
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.Status> CountERPOrdersInProgress()
        => CountERPOrdersInProgress(Motis);

    public static Shell<MotisDataDef.Status> CountERPOrdersInProgress(Motis Motis)
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
            return new Shell<MotisDataDef.Status>(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.Status> CountPickOrdersDone(DateTime Date)
        => CountPickOrdersDone(Motis, Date);

    public static Shell<MotisDataDef.Status> CountPickOrdersDone(Motis Motis, DateTime Date)
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

    public Shell<MotisDataDef.Status> CountERPOrdersDone(DateTime Date)
        => CountERPOrdersDone(Motis, Date);

    public static Shell<MotisDataDef.Status> CountERPOrdersDone(Motis Motis, DateTime Date)
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

    public Shell<MotisDataDef.Status> CountNewERPOrders(DateTime Date)
        => CountNewERPOrders(Motis, Date);
    public static Shell<MotisDataDef.Status> CountNewERPOrders(Motis Motis, DateTime Date)
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
        try
        {
            var Result = new Shell<MotisDataDef.Status>(StateEnum.Success)
            {
                Data = new()
            };

            using (var DbA = new SqlDbAccess(Motis.ConnectionString))
            {
                var Option = new GhalaDataPool.GhalaOption(Motis.Configuration, Motis.Logger);

                var OptionItem = Option.GetOption("DELIVERY LEAD TIME");

                if (OptionItem.State == StateEnum.Success)
                {
                    var DeliveryLeadTime = int.Parse(OptionItem.First().Data);

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
                } else
                    return new(StateEnum.Failure, "option not found");
            }

            return Result;
        }
        catch (Exception ex)
        {
            return new(StateEnum.Error, Helpers.SerializeException(ex));
        }
    }

    public Shell<MotisDataDef.Status> CountPickOrdersInProgress()
        => CountPickOrdersInProgress(Motis);
    public static Shell<MotisDataDef.Status> CountPickOrdersInProgress(Motis Motis)
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
            var Result = new Shell<MotisDataDef.Status>(StateEnum.Success)
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

    private static List<MotisDataDef.Status> ReaderToObjectList(SqlDataReader r)
    {
        return new List<MotisDataDef.Status>()
        {
            new MotisDataDef.Status()
            {
                D = r.GetString(r.GetOrdinal("d")),
                CH = r.GetInt32(r.GetOrdinal("CH")),
                CP = r.GetInt32(r.GetOrdinal("CP")),
            }
        };
    }


    public Status(Motis Motis, IConfiguration Configuration, ILogger<Status> Logger)
        : this(Motis, Configuration, (ILogger)Logger) { }
}
