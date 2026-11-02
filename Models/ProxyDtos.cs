namespace BuildWorkflowConnect.Models;

public class ProxyExecuteRequest
{
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = "";
    public List<Kv> Headers { get; set; } = new();
    public List<Kv> Params { get; set; } = new();
    public string BodyType { get; set; } = "none";
    public string Body { get; set; } = "";
}

public class ProxyResult
{
    public int Status { get; set; }
    public string StatusText { get; set; } = "";
    public Dictionary<string,string> Headers { get; set; } = new();
    public string Body { get; set; } = "";
    public double DurationMs { get; set; }
    public string? Error { get; set; }
}