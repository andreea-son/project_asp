using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Project.Client;

public class ApiErrorResponseDto
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? TraceId { get; set; }

    [JsonProperty("errors")]
    private object ErrorsObject
    {
        set
        {
            switch (value)
            {
                case JArray array:
                    Errors = array.ToObject<List<string>>();
                    break;
                case JObject obj:
                    ValidationErrors = obj.ToObject<Dictionary<string, List<string>>>();
                    break;
            }
        }
    }

    public List<string>? Errors { get; set; }
    public Dictionary<string, List<string>>? ValidationErrors { get; set; }
}