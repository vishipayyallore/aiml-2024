using System.Text;

namespace AAI.TextAnalyticsApp.Services;

public class JsonContent(string content) : StringContent(content, Encoding.UTF8, "application/json")
{
}
