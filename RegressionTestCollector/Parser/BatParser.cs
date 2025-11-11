using System.IO;
using System.Text.RegularExpressions;

namespace RegressionTestCollector.Parser
{
  /// <summary>
  /// Extracts the arguments of a bat file
  /// </summary>
  public class BatParser : IDataParser<Dictionary<string, string>>
  {
    public string Qualifier { get; set; }
    public BatParser(string qualifier)
    {
      Qualifier = qualifier;
    }
    public Dictionary<string, string> Parse(string data, string rootDir = "")
    {
      var args = new Dictionary<string, string>();
      using var reader = new StringReader(data);
      string? line;

      while ((line = reader.ReadLine()) != null)
      {
        if (line.StartsWith(Qualifier, StringComparison.OrdinalIgnoreCase))
        {
          var matches = Regex.Matches(line, @"(\w+)=([^\s""']+|""[^""]*""|'[^']*')");

          foreach (Match match in matches)
          {
            string key = match.Groups[1].Value;
            string value = match.Groups[2].Value;

            value = value.Trim('"').Trim('\'');

            args[key] = value;
          }
        }
      }

      return args;
    }
  }
}
