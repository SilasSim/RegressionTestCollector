namespace RegressionTestCollector.Models
{
    public class RegressionTestCollectionData
  {
    public List<RegressionTestDataObject> Data { get; set; } = new();
    public List<ErrorObject> Errors { get; } = new();
    public bool HasError => Errors.Count > 0;
  }
}
