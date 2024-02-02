using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crosscutting;
using Discord.WebSocket;

namespace BoundBot.Components.RestModel
{
    public class DiscordModelDtoRestModel
    {
        public DiscordModelDto Model { get; set; } = new DiscordModelDto();

        public DiscordModelDtoRestModel(SocketSlashCommand command)
        {
            var guildUser = (SocketGuildUser)command.User;

            Model.DiscordUsername = guildUser.Username;
            if (guildUser.Discriminator != "0" && guildUser.Discriminator != "0000")
            {
                Model.DiscordUsername = Model.DiscordUsername + "#" + guildUser.Discriminator;
            }
            Model.DiscordId = guildUser.Id.ToString();
            Model.Channel = command.Channel.Id.ToString();
            Model.Command = command.CommandName;

            var roles = guildUser.Roles.Select(role => role.Id.ToString()).ToList();
            Model.Roles = roles.Count == 0 ? new List<string>() { "0000000000" } : roles;
            Model.RefreshToken = string.Empty;
        }

        //public string Debug(SocketSlashCommand command)
        //{
        //    RestModel restModel = new RestModel(command);

        //    foreach (var ele in restModel.GetType().GetProperties())
        //    {
        //        if (ele.Name != "Roles")
        //        {
        //            _logger.LogInformation($"{ele.Name}: {ele.GetValue(restModel)}");
        //        }
        //        else
        //        {
        //            foreach (var role in restModel.Model.Roles!)
        //            {
        //                _logger.LogInformation("Roles: " + role + ", ");
        //            }
        //        }
        //    }
        //}
    }
}
