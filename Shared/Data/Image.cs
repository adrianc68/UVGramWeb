public class Image
{
  public string ContentType { get; set; }
  public string MimeType { get; set; }
  public byte[] Data { get; set; }
  public string Url { get; set; }
  public string Filename { get; set; }
  public long Filesize { get; set; }
}