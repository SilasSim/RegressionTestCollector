using System.Windows;
using System.Windows.Input;
using MVVMArchitecture;
using RegressionTestCollector.CollectingStrategies;
using RegressionTestCollector.Models;
using RegressionTestCollector.Properties;

namespace RegressionTestCollector.ViewModel
{
    public class MainViewModel : ViewModelBase

    {
    public SearchTableViewModel SearchTableViewModel { get; private set; }

    public OutputControlViewModel OutputControlViewModel => SearchTableViewModel.OutputControlViewModel;
    public MenuViewModel MenuViewModel { get; private set; }
    private CancellationTokenSource? mLoadingCancelToken;
    public MainViewModel(MenuViewModel menuViewModel, SearchTableViewModel searchTableViewModel)
    {
      MenuViewModel = menuViewModel;
      SearchTableViewModel = searchTableViewModel;

      ChangeThemeCommand = new RelayCommand<string>(ChangeTheme);
      LoadRegressionTestsCommand = new RelayCommand(LoadRegressionTests);
      ChangePythonCommandCommand = new RelayCommand<string>(ChangePythonCommand);
      CancelLoadingCommand = new RelayCommand(CancelLoadingProcess);
      ChangeCollectingStrategyCommand = new RelayCommand<string>(ChangeCollectingStrategy);

      // Is set twice, since the strategy handler needs to be initialized
      CollectingStrategyHandler = new CollectingStrategyHandler(new CSharpCollectingStrategy(OnProgressChanged));
      ChangeCollectingStrategy(Settings.Default.SelectedCollectingMethod);
    }

    private void ChangeTheme(string? obj)
    {
      if (obj is not null)
      {
        MenuViewModel.SelectedTheme = obj;
      }
    }

    private string mLoadingDataText = Resources.Strings.LoadingData;

    public string LoadingDataText
    {
      get => mLoadingDataText;
      set => SetField(ref mLoadingDataText, value);
    }

    private void CancelLoadingProcess()
    {
      mLoadingCancelToken?.Cancel();
      LoadingDataText = Resources.Strings.LoadingProcessStopped;
    }

    private void OnProgressChanged(object? sender, LoadingProgressEventArgs e)
    {
      mLoadingCancelToken?.Token.ThrowIfCancellationRequested();

      Application.Current.Dispatcher.BeginInvoke(new Action(() =>
      {
        LoadingProgress.Current = e.Progress.Current;
        LoadingProgress.Max = e.Progress.Max;
      }));
    }

    public ICommand LoadRegressionTestsCommand { get; }
    public CollectingStrategyHandler CollectingStrategyHandler { get; private set; }

    public LoadingProgress LoadingProgress { get; } = new LoadingProgress(0, 100);

    private bool mIsLoadingData = false;
    public bool IsLoadingData
    {
      get => mIsLoadingData;
      set => SetField(ref mIsLoadingData, value);
    }

    private async void LoadRegressionTests()
    {
      IsLoadingData = true;
      mLoadingCancelToken = new CancellationTokenSource();
      try
      {
        var collectedData = await Task.Run(() => CollectingStrategyHandler.Collect(MenuViewModel.SelectedPythonCommand),
          mLoadingCancelToken.Token);
        if (collectedData is not null)
        {
          if (collectedData.HasError)
          {
            foreach (var error in collectedData.Errors)
            {
              OutputControlViewModel.OutputList.Add(new OutputData(error.Message, OutputType.Error));
            }

          }

          if (collectedData.Data.Count > 0)
          {
            SearchTableViewModel.SourceData.Clear();
            foreach (var data in collectedData.Data)
            {
              SearchTableViewModel.SourceData.Add(new RegressionTestViewModel(data));
            }
          }

          OutputControlViewModel.OutputList.Add(
            new OutputData($"{collectedData.Errors.Count} {Resources.CollectingErrors.LoadingErrorsEncountered}"));
          OutputControlViewModel.OutputList.Add(new OutputData(
            $"{collectedData.Data.Count} {Resources.CollectingErrors.LoadingTestsFound}", OutputType.Success));
        }

      }
      catch (OperationCanceledException)
      {
        OutputControlViewModel.OutputList.Add(new OutputData(Resources.CollectingErrors.LoadingCanceledInfo));
        LoadingDataText = Resources.Strings.LoadingData;
      }
      catch (Exception ex)
      {
        OutputControlViewModel.OutputList.Add(new OutputData(ex.Message, OutputType.Error));
      }
      finally
      {
        IsLoadingData = false;
      }
    }



    public ICommand ChangeCollectingStrategyCommand { get; }
    public ICommand CancelLoadingCommand { get; }
    public ICommand ChangePythonCommandCommand { get; }

    public ICommand ChangeThemeCommand { get; }

    private void ChangePythonCommand(string? obj)
    {
      if (obj is not null)
      {
        MenuViewModel.SelectedPythonCommand = obj;
      }
    }

    private void ChangeCollectingStrategy(string? obj)
    {
      if (obj is not null)
      {
        if (obj.Equals(Resources.Strings.MenuCollectingMethodCSharpName))
        {
          CollectingStrategyHandler.ChangeCollectingStrategy(new CSharpCollectingStrategy(OnProgressChanged));

        }
        else if (obj.Equals(Resources.Strings.MenuCollectingMethodPythonName))
        {
          CollectingStrategyHandler.ChangeCollectingStrategy(new PythonCollectingStrategy());
        }
        MenuViewModel.SelectedStrategy = obj;
      }
    }
  }
}
