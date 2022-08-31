using System.Text;

namespace aspnetenrichedexceptionlogger.Middleware;

public static class EnrichedExcptionMWExtensions
{
    private const string ContextKey = "EnrichedExceptionMiddleware";

    public static void EnrichUnhandledExceptionLogger(this HttpContext context, string key, string value)
    {
        if(!context.Items.ContainsKey(ContextKey))
        {
            context.Items.Add(ContextKey, new Dictionary<string, string>());
        }
        var dict = context.Items[ContextKey] as Dictionary<string, string>;

        if (dict != null) dict[key] = value;
    }

    public static Dictionary<string, string> GetUnhandledExceptionEnrichment(this HttpContext context)
    {
        return (context.Items[ContextKey] as Dictionary<string, string>) ?? new Dictionary<string, string>();
    }
}

public class EnrichedExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EnrichedExceptionMiddleware> _logger;

    public EnrichedExceptionMiddleware(RequestDelegate next, ILogger<EnrichedExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            var enrichment = context.GetUnhandledExceptionEnrichment();
            var enrichmentFormatString = new StringBuilder();
            if(enrichment.Count > 0)
            {
                enrichmentFormatString.Append("Enrichment: ");
                foreach(var key in enrichment.Keys)
                {
                    enrichmentFormatString.Append($" {{{key}}}");
                }
            }
            using(_logger.BeginScope(enrichmentFormatString.ToString(), enrichment.Values.ToArray()))
            {
                _logger.LogError(e, "Unhandled exception occurred");
            }
        }
    }
}