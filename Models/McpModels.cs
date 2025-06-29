using System.Text.Json.Serialization;

namespace mssqlMCP.Models
{
    /// <summary>
    /// Models for JSON-RPC 2.0 MCP protocol
    /// </summary>
    public class McpNotification
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonPropertyName("method")]
        public string Method { get; set; } = null!;
    }

    /// <summary>
    /// Represents a tool capabilities notification in the MCP protocol
    /// </summary>
    public class ToolsCapabilities
    {
        [JsonPropertyName("tools")]
        public ToolsCapabilityInfo Tools { get; set; } = new ToolsCapabilityInfo();

        public class ToolsCapabilityInfo
        {
            [JsonPropertyName("listChanged")]
            public bool ListChanged { get; set; } = true;
        }
    }
}
