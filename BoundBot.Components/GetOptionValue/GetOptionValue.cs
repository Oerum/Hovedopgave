using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BoundBot.Components.GetOptionValue
{
    public static class GetOptionValue
    {
        public static T? GetOptionValues<T>(this SocketSlashCommand command, string optionName) where T : class
        {
            var first = command.Data.Options.FirstOrDefault(x => x.Name == optionName);
            
            if (first is { Value: T value })
            {
                return value;
            }

            // If 'T' is 'string', try converting the value using Convert.ChangeType
            if (typeof(T) == typeof(string) && first?.Value != null)
            {
                return Convert.ChangeType(first.Value, typeof(T)) as T;
            }

            return null;
        }

    }
}
