using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegressionTestCollector.Models;

namespace RegressionTestCollector.ViewModel
{
  public class RegressionTestViewModel : ViewModelBase
  {
    public RegressionTestViewModel(RegressionTestDataObject regressionTestData)
    {
      _regressionTestData = regressionTestData;
    }

    private RegressionTestDataObject _regressionTestData;

    public RegressionTestDataObject RegressionTestData
    {
      get => _regressionTestData;
      set => SetField(ref _regressionTestData, value);
    }

    private bool _isDebug = false;

    public bool IsDebug
    {
      get => _isDebug;
      set => SetField(ref _isDebug, value);
    }
  }
}
