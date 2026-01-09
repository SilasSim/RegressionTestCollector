namespace RegressionTestCollector.Models
{
  public class ErrorObject
  {
    public ErrorObject(string message)
    {
      Message = message;
    }

    public ErrorObject(string message, Exception ex) : this(message)
    {
      Exception = ex;
    }

    public virtual string Message { get; private set; }
    public virtual Exception? Exception { get; private set; }
  }
}
