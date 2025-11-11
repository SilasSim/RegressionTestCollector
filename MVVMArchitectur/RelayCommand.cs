using System.Windows.Input;

namespace MVVMArchitecture
{
  public class RelayCommand<T> : ICommand
  {
    private readonly Action<T> mExecute;
    private readonly Predicate<T>? mCanExecute;

    public RelayCommand(Action<T> execute, Predicate<T>? canExecute)
    {
      mExecute = execute;
      mCanExecute = canExecute;
    }

    public RelayCommand(Action<T> execute) : this(execute, null) { }

    public bool CanExecute(object? parameter)
    {
      if (parameter is T param)
      {
        return mCanExecute?.Invoke(param) ?? true;
      }

      if (parameter is null && default(T) is null)
      {
        return mCanExecute?.Invoke(default!) ?? true;
      }
      return false;
    }

    public void Execute(object? parameter)
    {
      if (CanExecute(parameter))
      {
        if (parameter is T param)
        {
          mExecute.Invoke(param);
        }
        else if (parameter is null && default(T) is null)
        {
          mExecute.Invoke(default!);
        }
      }
    }

    public event EventHandler? CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }
  }

  public class RelayCommand : ICommand
  {
    private readonly Action mExecute;
    private readonly Func<bool>? mCanExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute)
    {
      mExecute = execute ?? throw new ArgumentNullException(nameof(execute));
      mCanExecute = canExecute;
    }

    public RelayCommand(Action execute) : this(execute, null) { }

    public bool CanExecute(object? parameter)
    {
      return mCanExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
      if (CanExecute(parameter))
      {
        mExecute.Invoke();
      }
    }

    public event EventHandler? CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }
  }
}
