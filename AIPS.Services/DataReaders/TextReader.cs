using System.Text;
using AIPS.Services.Types;

namespace AIPS.Services.DataReaders;

public interface IFileReader
{
    IList<DataRecording> Read(string filePath);
}

public class TextFileReader : IFileReader
{
    public IList<DataRecording> Read(string filePath)
    {
        var linesConverted = new List<DataRecording>();
        const Int32 bufferSize = 128;
        using var fileStream = File.OpenRead(filePath);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize);
        while (streamReader.ReadLine() is { } line)
        {
            //break split
            var values = line.Split(' ');
            linesConverted.Add(new DataRecording(
                DateTimeOffset.Parse(values[0]),
                int.Parse(values[1]))
            );
        }

        return linesConverted;
    }
}