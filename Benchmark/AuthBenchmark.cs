using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMWE_Administrator;
using System.Net;
using System.Reflection;

namespace Benchmark
{
    [Config(typeof(Config))]
    public class AuthBenchmark
    {
        [Benchmark(Description = "[AMWE] - AdmAuth (right pass - 5 symbols)")]
        public object AuthWithRightPassword()
        {
            return AuthWindow.AuthUser(new string[] { "Zerumi", Encryption.Encrypt("aav15_"), Assembly.GetExecutingAssembly().GetName().Version.ToString(), Assembly.LoadFrom("ReportHandler.dll").GetName().Version.ToString(), Assembly.LoadFrom("m3md2.dll").GetName().Version.ToString() }, out _);
        }

        [Benchmark(Description = "[AMWE] - AdmAuth (wrong pass - 5 symbols)")]
        public object AuthWithWrongPassword()
        {
            return AuthWindow.AuthUser(new string[] { "Zerumi", Encryption.Encrypt("12345"), Assembly.GetExecutingAssembly().GetName().Version.ToString(), Assembly.LoadFrom("ReportHandler.dll").GetName().Version.ToString(), Assembly.LoadFrom("m3md2.dll").GetName().Version.ToString() }, out _);
        }
    }
}
