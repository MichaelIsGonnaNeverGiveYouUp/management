using System.Collections.Generic;

namespace Model;

public class Response
{
    public Dictionary<string, string> headers { get; set; }
    public string request_url { get; set; }
    public string status_code { get; set; }
    public string content { get; set; }
    public string token { get; set; }
    public string message { get; set; }
    
}