using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    class Config : ManualConfig
    {
        public Config()
        {
            AddColumn(TargetMethodColumn.Method);
            AddColumn(StatisticColumn.AllStatistics);
            AddLogger(ConsoleLogger.Default);
            UnionRule = ConfigUnionRule.AlwaysUseLocal;
        }
    }
}
