using Discord;
using Discord.WebSocket;

namespace BoundBot.Components.GetMentionedChannels;

public static class GetMentionedChannels
{
    public static async Task<string> GetMentionedForumChannelsMethod(SocketGuild guild, ulong[] channelIds)
    {
        var tasks = channelIds.Select(async channelId =>
        {
            var textChannel = await Task.Run(() => guild.GetTextChannel(channelId));
            return textChannel;
        });

        var mentionedChannels = await Task.WhenAll(tasks);

        var validChannels = mentionedChannels
            .Where(infoChannel => infoChannel != null)
            .Select(infoChannel => infoChannel.Mention);

        return string.Join("\n", validChannels);
    }
}