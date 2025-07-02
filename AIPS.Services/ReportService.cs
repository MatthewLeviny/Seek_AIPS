using AIPS.Services.DataReaders;

namespace AIPS.Services;

interface IReportService
{
    void GenerateReport(string filePath);
}
public class ReportService : IReportService
{
    private readonly IFileReader _fileReader;
    private readonly ITrafficCounterService _trafficCounterService;
    
    public ReportService(IFileReader fileReader, ITrafficCounterService trafficCounterService)
    {
        _fileReader = fileReader;
        _trafficCounterService = trafficCounterService;
    }
    public void GenerateReport(string filePath)
    {
        var data = _fileReader.Read(filePath);
        
        var totalCount = _trafficCounterService.TotalCarsSeen(data);
        Console.WriteLine($"Total Cars: {totalCount}"); 
        
        var dayCount = _trafficCounterService.CarsSeenPerDay(data);
        Console.WriteLine("\nCars Per Day:");
        dayCount.ToList().ForEach(kvp => Console.WriteLine($"{kvp.TimeStamp:yyyy-MM-dd} {kvp.Count}"));
        
        var top3HalfHours = _trafficCounterService.MostCarsOverPeriod(3, data);
        Console.WriteLine("\nTop 3 Half Hours:");
        top3HalfHours.ToList().ForEach(entry => Console.WriteLine($"{entry.TimeStamp:yyyy-MM-ddTHH:mm:ss} {entry.Count}"));

        Console.WriteLine("\nThe 1.5 hour period with least cars:");
        var leastCarsOverPeriod = _trafficCounterService.LeastCarsOverPeriod(3, data);
        leastCarsOverPeriod.ToList().ForEach(entry => Console.WriteLine($"{entry.TimeStamp:yyyy-MM-ddTHH:mm:ss} {entry.Count}"));
    }
}