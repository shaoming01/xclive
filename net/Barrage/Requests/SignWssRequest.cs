using System.Diagnostics.CodeAnalysis;

namespace Barrage.Requests
{
    public class SignWssRequest
    {
        [NotNull]
        public string? ApiKey { get; set; }

        public string? BrowserName { get; set; }

        public string? BrowserVersion { get; set; }

        [NotNull]
        public string? UserUniqueId { get; set; }

        [NotNull]
        public string? RoomId { get; set; }
    }
}
