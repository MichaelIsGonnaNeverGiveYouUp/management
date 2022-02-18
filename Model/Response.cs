using System.Collections.Generic;

namespace Model;

public class Response
{
    public List<string> headers { get; set; }
    public string status_code { get; set; }
    public string content { get; set; }
    public string token { get; set; }
    public string message { get; set; }
    
}