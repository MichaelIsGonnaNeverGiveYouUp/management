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

    public override string ToString()
    {
        string result = $"Headers: {headers}\nRequest_url: {request_url}\nstatus_code: {status_code}\n";
        result += $"Content: {content}\nToken: {token}\nMessage: {message}";
        return result;
    }
    
}