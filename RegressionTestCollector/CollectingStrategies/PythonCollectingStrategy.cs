using Microsoft.Win32;
using RegressionTestCollector.Models;
using RegressionTestCollector.Parser;
using RegressionTestCollector.Utils;

namespace RegressionTestCollector.CollectingStrategies;

/// <summary>
/// Collecting strategy that uses an external python script
/// </summary>
public class PythonCollectingStrategy : ICollectingStrategy
{

  public Action<string>? OnOutput = null;
  public Action<string>? OnError = null;

  public IDataParser<RegressionTestCollectionData> DataParser { get; private set; } =
    new CollectedPythonDataToRegressionDataObjectsParser();


  public PythonCollectingStrategy() { }

  public PythonCollectingStrategy(Action<string>? onOutput, Action<string>? onError)
  {
    OnOutput = onOutput;
    OnError = onError;
  }

  public RegressionTestCollectionData Collect(string pythonCommand)
  {
    var dialog = new OpenFileDialog();
    dialog.Filter = "Python (*.py)|*.py";

    if (dialog.ShowDialog() == true)
    {
      var path = dialog.FileName;
      var output = PythonHelper.GenerateAndReadPythonScriptOutput(path, pythonCommand);
      return DataParser.Parse(output);
    }

    return new RegressionTestCollectionData();
  }
}