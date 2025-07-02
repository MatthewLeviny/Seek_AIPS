using AIPS.Services;
using AIPS.Services.DataReaders;

namespace AIPS.ConsoleApp;

class Program
{
    static void Main()
    {
        var runner = new ReportService(new TextFileReader(), new TrafficCounterService());
        runner.GenerateReport("data.txt");
    }
}