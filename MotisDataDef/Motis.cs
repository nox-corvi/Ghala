using Nox.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotisDataDef;

public class Motis
    : DataModel
{
    public Motis(string ConnectionString)
        : base(ConnectionString)
    {
    }
}
