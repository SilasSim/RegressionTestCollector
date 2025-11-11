using System.IO;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.Parser
{
  public class CollectedPythonDataToRegressionDataObjectsParser : IDataParser<RegressionTestCollectionData>
  {
    public RegressionTestCollectionData Parse(string data, string rootDir = "")
    {
      var dataObjects = new List<RegressionTestDataObject>();

      using var reader = new StringReader(data);
      string? line;

      var currentGroupName = "";
      var currentTestName = "";
      var currentCommand = "";
      var nextLineCouldBeCommand = false;

      while ((line = reader.ReadLine()) != null)
      {


        if (string.IsNullOrWhiteSpace(line))
        {
          continue;
        }

        if (nextLineCouldBeCommand)
        {
          var command = FindCommand(line);
          if (command is not null)
          {
            currentCommand = command;
            dataObjects.Add(new RegressionTestDataObject(currentTestName, currentGroupName, currentCommand));
            nextLineCouldBeCommand = false;
          }
          else
          {
            dataObjects.Add(new RegressionTestDataObject(currentTestName, currentGroupName, ""));
          }
        }

        var groupName = FindTestGroup(line);
        if (groupName is not null)
        {
          currentGroupName = groupName;
          continue;
        }

        var testName = FindTestName(line);
        if (testName is not null)
        {
          currentTestName = testName;

          nextLineCouldBeCommand = true;
          continue;
        }
      }

      return new RegressionTestCollectionData()
      {
        Data = dataObjects
      };
    }

    private static string? FindTestGroup(string line)
    {
      const string searchString = "RegressionTest";
      var index = line.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);

      if (index == -1)
      {
        return null;
      }

      var stringAfter = line.Substring(index + searchString.Length).Trim();
      var groupName = stringAfter.Split(".")[0];
      return groupName;
    }

    private static string? FindTestName(string line)
    {
      const string searchString = "Test: ";
      var index = line.IndexOf(searchString, StringComparison.OrdinalIgnoreCase);

      if (index == -1)
      {
        return null;
      }

      var stringAfter = line.Substring(index + searchString.Length).Trim();
      var testName = stringAfter.Split(" -")[0];
      return testName;
    }

    private static string? FindCommand(string line)
    {
      return line.Contains("se.exe") ? line.Trim() : null;
    }
  }
}
