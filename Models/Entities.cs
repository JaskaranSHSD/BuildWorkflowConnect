using System.Text.Json;

namespace BuildWorkflowConnect.Models;

public class EnvironmentEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "Default";
    public List<EnvironmentVariable> Variables { get; set; } = new();
}

public class EnvironmentVariable
{
    public int Id { get; set; }
    public int EnvironmentId { get; set; }
    public EnvironmentEntity? Environment { get; set; }
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

public class ApiGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = "New Group";
    public List<ApiRequest> Requests { get; set; } = new();
}

public class ApiRequest
{
    public int Id { get; set; }
    public int? GroupId { get; set; }
    public ApiGroup? Group { get; set; }
    public string Name { get; set; } = "New Request";
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = "";
    public string BodyType { get; set; } = "none";
    public string Body { get; set; } = "";
    public string HeadersJson { get; set; } = "[]";
    public string ParamsJson { get; set; } = "[]";
    public string AuthType { get; set; } = "none";
    public string AuthJson { get; set; } = "{}";
}

public static class PhvJson
{
    public static List<Kv> FromJson(string? s)
        => string.IsNullOrWhiteSpace(s) ? new() : JsonSerializer.Deserialize<List<Kv>>(s) ?? new();
    public static string ToJson(List<Kv> v) => JsonSerializer.Serialize(v);
}
public class Kv { public string Key { get; set; } = ""; public string Value { get; set; } = ""; }
public class AuthDto { public string Type { get; set; } = "none"; public string Token { get; set; } = ""; public string Username { get; set; } = ""; public string Password { get; set; } = ""; public string Key { get; set; } = ""; public string Value { get; set; } = ""; public string Location { get; set; } = "header"; }