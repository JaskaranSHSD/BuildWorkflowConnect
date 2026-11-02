using BuildWorkflowConnect.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuildWorkflowConnect.Controllers;

[ApiController]
[Route("api/proxy")]
public class ProxyController(IHttpClientFactory factory) : ControllerBase
{
    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] ProxyExecuteRequest req)
    {
        var result = new ProxyResult();
        var sw = Stopwatch.StartNew();
        try
        {
            var http = factory.CreateClient("proxy");
            http.Timeout = TimeSpan.FromSeconds(30);

            var url = req.Url;
            var query = new List<string>();
            foreach (var p in req.Params)
            {
                if (string.IsNullOrWhiteSpace(p.Key)) continue;
                query.Add($"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}");
            }
            if (query.Count > 0)
                url += (url.Contains('?') ? "&" : "?") + string.Join("&", query);

            using var message = new HttpRequestMessage(new HttpMethod(req.Method), url);
            foreach (var h in req.Headers)
            {
                if (string.IsNullOrWhiteSpace(h.Key)) continue;
                message.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
            if (!HttpMethods.IsGet(req.Method) && !HttpMethods.IsHead(req.Method) && req.BodyType != "none")
            {
                var content = new StringContent(req.Body ?? "");
                if (req.BodyType == "json" && !req.Headers.Any(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)))
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                else if (req.BodyType == "form")
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                else if (req.BodyType == "text")
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                message.Content = content;
            }

            using var response = await http.SendAsync(message);
            result.Status = (int)response.StatusCode;
            result.StatusText = response.ReasonPhrase ?? "";
            foreach (var h in response.Headers) result.Headers[h.Key] = string.Join(", ", h.Value);
            foreach (var h in response.Content.Headers) result.Headers[h.Key] = string.Join(", ", h.Value);
            result.Body = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
            result.Status = 0;
            result.StatusText = ex.Message;
            result.Body = ex.ToString();
        }
        finally
        {
            sw.Stop();
            result.DurationMs = sw.Elapsed.TotalMilliseconds;
        }
        return Ok(result);
    }
}