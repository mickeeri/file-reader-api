
using System.Collections.Generic;

namespace FileReaderAPI.Models 
{
  public class Result
  {
    public string FileName { get; set; }
    public string ProcessedContent { get; set; }
    public List<string> MostCommonWords { get; set; }
  }
}
