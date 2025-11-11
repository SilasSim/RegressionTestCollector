using System.Diagnostics;
using System.Text;

namespace RegressionTestCollector.Models
{
  public enum RegressionTestKind
  {
    EqualityTest, FailTest, SuccessTest
  }
  public class RegressionTestConfiguration
  {
    public RegressionTestConfiguration(string name, RegressionTestKind kind)
    {
      Kind = kind;
      Name = name;
    }

    public RegressionTestKind Kind { get; }
    public string Name { get; }
    public string Inifile { get; set; } = "";
    public string Sourcefile { get; set; } = "";
    public string Outfile { get; set; } = "";
    public string Logfile { get; set; } = "";
    public Dictionary<string, string> AdditionalParameter { get; set; } = [];

    public string GetCommandStringForPythonScript()
    {
      var builder = new StringBuilder();

      if (!string.IsNullOrWhiteSpace(Sourcefile))
      {
        builder.Append($"-i \"{Sourcefile}\" ");
      }

      if (!string.IsNullOrWhiteSpace(Outfile))
      {
        builder.Append($"-o \"{Outfile}\" ");
      }

      if (!string.IsNullOrWhiteSpace(Logfile))
      {
        builder.Append($"-g \"{Logfile}\" ");
      }

      if (!string.IsNullOrWhiteSpace(Logfile))
      {
        builder.Append($"-ini \"{Inifile}\" ");
      }

      foreach (var (key, argumentValue) in AdditionalParameter)
      {
        builder.Append($"-{key} \"{argumentValue}\" ");
      }
      return builder.ToString().Trim();
    }
  }
}
