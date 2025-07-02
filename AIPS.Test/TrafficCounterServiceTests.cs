using AIPS.Services;
using AIPS.Services.Types;
using Shouldly;

namespace AIPS.Test;

public class TrafficCounterServiceTests
{
    #region TotalCarsSeen
    [Fact]
    public void TotalCarsSeen_EmptyList_ReturnsZero()
    {
        var input = new List<DataRecording>();

        var cut = new TrafficCounterService();
        var result = cut.TotalCarsSeen(input);
        
        result.ShouldBe(0);
    }

    [Fact]
    public void TotalCarsSeen_SingleElement_ReturnsThatElement()
    {
        var input = new List<DataRecording> { new(DateTime.Now, 5) };
        
        var cut = new TrafficCounterService();
        int result = cut.TotalCarsSeen(input);
        Assert.Equal(5, result);
    }

    [Fact]
    public void TotalCarsSeen_MultipleElements_ReturnsCorrectSum()
    {
        var input = new List<DataRecording>
        {
            new(DateTime.Now, 5),
            new(DateTime.Now, 10),
            new(DateTime.Now, 20)
        };
        
        var cut = new TrafficCounterService();
        int result = cut.TotalCarsSeen(input);
        
        result.ShouldBe(35);
    }

    [Fact]
    public void TotalCarsSeen_AllZeros_ReturnsZero()
    {
        var input = new List<DataRecording>
        {
            new(DateTime.Now, 0),
            new(DateTime.Now, 0)
        };
        
        var cut = new TrafficCounterService();
        int result = cut.TotalCarsSeen(input);
        
        result.ShouldBe(0);
    }

    [Fact]
    public void TotalCarsSeen_NegativeCounts_ReturnsCorrectSum()
    {
        var input = new List<DataRecording>
        {
            new(DateTime.Now, -2),
            new(DateTime.Now, 3)
        };
        
        var cut = new TrafficCounterService();
        int result = cut.TotalCarsSeen(input);
        
        result.ShouldBe(1);   
    }

    [Fact]
    public void TotalCarsSeen_NullList_ThrowsArgumentNullException()
    {
        var cut = new TrafficCounterService();
        Should.Throw<ArgumentNullException>(() => cut.TotalCarsSeen(null));   
    }
    #endregion
    
    #region CarsSeenPerDay
    [Fact]
    public void CarsSeenPerDay_WithSingleDay_ReturnsSingleEntry()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T10:00:00"), 5),
            new(DateTimeOffset.Parse("2023-07-01T15:30:00"), 7),
            new(DateTimeOffset.Parse("2023-07-01T23:59:59"), 3)
        };

        var cut = new TrafficCounterService();
        var result = cut.CarsSeenPerDay(input).ToList();

        result.Count.ShouldBe(1);
        result[0].TimeStamp.Date.ShouldBe(new DateTime(2023, 7, 1));
        result[0].Count.ShouldBe(15);
    }

    [Fact]
    public void CarsSeenPerDay_WithMultipleDays_ReturnsGroupedCounts()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T10:00:00"), 5),
            new(DateTimeOffset.Parse("2023-07-01T15:30:00"), 10),
            new(DateTimeOffset.Parse("2023-07-02T09:00:00"), 4),
            new(DateTimeOffset.Parse("2023-07-02T21:00:00"), 6)
        };

        var cut = new TrafficCounterService();
        var result = cut.CarsSeenPerDay(input)
            .OrderBy(x => x.TimeStamp)
            .ToList();

        result.Count.ShouldBe(2);

        result[0].TimeStamp.Date.ShouldBe(new DateTime(2023, 7, 1));
        result[0].Count.ShouldBe(15);

        result[1].TimeStamp.Date.ShouldBe(new DateTime(2023, 7, 2));
        result[1].Count.ShouldBe(10);
    }

    [Fact]
    public void CarsSeenPerDay_WithEmptyInput_ReturnsEmpty()
    {
        var input = new List<DataRecording>();

        var cut = new TrafficCounterService();
        var result = cut.CarsSeenPerDay(input);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void CarsSeenPerDay_AllZeroCounts_ReturnsZeroTotals()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 0),
            new(DateTimeOffset.Parse("2023-07-01T12:00:00"), 0)
        };

        var cut = new TrafficCounterService();
        var result = cut.CarsSeenPerDay(input).ToList();

        result.Count.ShouldBe(1);
        result[0].Count.ShouldBe(0);
    }
    
    #endregion
    
    #region LeastCarsOverPeriod
    [Fact]
    public void LeastCarsOverPeriod_SimpleContiguousInput_ReturnsMinSumWindow()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 10),
            new(DateTimeOffset.Parse("2023-07-01T08:30:00"), 5),
            new(DateTimeOffset.Parse("2023-07-01T09:00:00"), 3),
            new(DateTimeOffset.Parse("2023-07-01T09:30:00"), 7),
            new(DateTimeOffset.Parse("2023-07-01T10:00:00"), 12)
        };

        var cut = new TrafficCounterService();
        var result = cut.LeastCarsOverPeriod(3, input).ToList();

        result.Count.ShouldBe(3);
        result[0].TimeStamp.ShouldBe(DateTimeOffset.Parse("2023-07-01T08:30:00"));
        result.Sum(x => x.Count).ShouldBe(15);
    }

    [Fact]
    public void LeastCarsOverPeriod_WithGaps_SkipsNonContiguousWindows()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 10),
            new(DateTimeOffset.Parse("2023-07-01T08:30:00"), 10),
            new(DateTimeOffset.Parse("2023-07-01T10:00:00"), 1), // gap here
            new(DateTimeOffset.Parse("2023-07-01T10:30:00"), 1),
            new(DateTimeOffset.Parse("2023-07-01T11:00:00"), 1)
        };

        var cut = new TrafficCounterService();
        var result = cut.LeastCarsOverPeriod(3, input).ToList();

        // should return [10:00, 10:30, 11:00] because they're the only contiguous 3-period window
        result.Count.ShouldBe(3);
        result[0].TimeStamp.ShouldBe(DateTimeOffset.Parse("2023-07-01T10:00:00"));
        result.Sum(x => x.Count).ShouldBe(3);
    }

    [Fact]
    public void LeastCarsOverPeriod_NoValidWindow_ReturnsEmpty()
    {
        var input = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 1),
            new(DateTimeOffset.Parse("2023-07-01T09:00:00"), 1),
            new(DateTimeOffset.Parse("2023-07-01T10:00:00"), 1) // not 30-min apart
        };

        var cut = new TrafficCounterService();
        var result = cut.LeastCarsOverPeriod(3, input);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void LeastCarsOverPeriod_EmptyInput_ReturnsEmpty()
    {
        var input = new List<DataRecording>();

        var cut = new TrafficCounterService();
        var result = cut.LeastCarsOverPeriod(2, input);

        result.ShouldBeEmpty();
    }
    #endregion

    #region MostCarsOverPeriod

    [Fact]
    public void MostCarsOverPeriod_ReturnsTopPeriods()
    {
        var records = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 10),
            new(DateTimeOffset.Parse("2023-07-01T08:30:00"), 20),
            new(DateTimeOffset.Parse("2023-07-01T09:00:00"), 15),
            new(DateTimeOffset.Parse("2023-07-01T09:30:00"), 5)
        };

        var cut = new TrafficCounterService();
        var result = cut.MostCarsOverPeriod(2, records).ToList();

        result.Count.ShouldBe(2);
        result[0].Count.ShouldBe(20);
        result[1].Count.ShouldBe(15);
    }

    [Fact]
    public void MostCarsOverPeriod_WhenPeriodsMoreThanRecords_ReturnsAll()
    {
        var records = new List<DataRecording>
        {
            new(DateTimeOffset.Parse("2023-07-01T08:00:00"), 10),
            new(DateTimeOffset.Parse("2023-07-01T08:30:00"), 20)
        };

        var cut = new TrafficCounterService();
        var result = cut.MostCarsOverPeriod(5, records).ToList();

        result.Count.ShouldBe(2);
    }

    [Fact]
    public void MostCarsOverPeriod_WithEmptyInput_ReturnsEmpty()
    {
        var records = new List<DataRecording>();

        var cut = new TrafficCounterService();
        var result = cut.MostCarsOverPeriod(3, records);

        result.ShouldBeEmpty();
    }
    
    #endregion
}