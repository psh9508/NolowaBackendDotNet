using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Models.Configuration
{
    public class DBConfiguration
    {
        public SQLServer SQLServer { get; set; }
    }

    public class SQLServer
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
