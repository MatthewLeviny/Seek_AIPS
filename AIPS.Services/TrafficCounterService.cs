using AIPS.Services.Types;

namespace AIPS.Services;

public interface ITrafficCounterService
{
    int TotalCarsSeen(IEnumerable<DataRecording> records);
    IEnumerable<DataRecording> CarsSeenPerDay(IEnumerable<DataRecording> records);
    IEnumerable<DataRecording> LeastCarsOverPeriod(int periods, IList<DataRecording> records);
    IEnumerable<DataRecording> MostCarsOverPeriod(int periods, IList<DataRecording> records);
}

public class TrafficCounterService : ITrafficCounterService
{
    public int TotalCarsSeen(IEnumerable<DataRecording> records)
    {
        return records.Sum(x => x.Count);
    }

    public IEnumerable<DataRecording> CarsSeenPerDay(IEnumerable<DataRecording> records)
    {
        return records.GroupBy(x => x.TimeStamp.Date)
            .Select(x => new DataRecording(x.Key, x.Sum(y => y.Count)));
    }

    public IEnumerable<DataRecording> LeastCarsOverPeriod(int periods, IList<DataRecording> records)
    {
        ArgumentNullException.ThrowIfNull(records);
        if (periods <= 0) throw new ArgumentException("Period count must be positive", nameof(periods));
        
        var recordings = records.OrderBy(r => r.TimeStamp).ToList();
        var start = 0;
        var minSum = int.MaxValue;
        var currentSum = 0;
        var minStartIndex = -1;

        for (var end = 0; end < recordings.Count; end++)
        {
            if (end > start && recordings[end].TimeStamp != recordings[end - 1].TimeStamp.AddMinutes(30))
            {
                currentSum = 0;
                start = end;
            }

            currentSum += recordings[end].Count;

            var windowSize = end - start + 1;
            if (windowSize == periods)
            {
                if (currentSum < minSum)
                {
                    minSum = currentSum;
                    minStartIndex = start;
                }

                currentSum -= recordings[start].Count;
                start++;
            }
        }

        if (minStartIndex == -1)
            return [];

        return recordings.Skip(minStartIndex).Take(periods);
    }

    public IEnumerable<DataRecording> MostCarsOverPeriod(int periods, IList<DataRecording> records)
    {
        return records
            .OrderByDescending(e => e.Count)
            .Take(periods);
    }
}