using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionTestCollector.Models
{
  public enum OutputType
  {
    Info,
    Error,
    Success
  }
  public class OutputData
  {
    public OutputData(string text, OutputType outputType = OutputType.Info)
    {
      Text = text;
      OutputType = outputType;
    }
    public string Text { get; private set; }
    public OutputType OutputType { get; private set; }
  }
}
