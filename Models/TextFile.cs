
namespace FileReaderAPI.Models 
{
  public class TextFile
  {
    public string Name { get; set; }
    public long Length { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
    public string MostCommonWord { get; set; }
  }
}
