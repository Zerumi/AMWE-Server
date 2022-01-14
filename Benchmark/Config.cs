using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;

namespace Benchmark
{
    internal class Config : ManualConfig
    {
        public Config()
        {
            _ = AddColumn(TargetMethodColumn.Method);
            _ = AddColumn(StatisticColumn.AllStatistics);
            _ = AddLogger(ConsoleLogger.Default);
            UnionRule = ConfigUnionRule.AlwaysUseLocal;
        }
    }
}
