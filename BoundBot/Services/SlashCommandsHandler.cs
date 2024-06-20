using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Text;
using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Application.DatabaseBackup.Interface;
using BoundBot.Application.ExtendLicenses.Interface;
using BoundBot.Application.GetCoupon.Interface;
using BoundBot.Application.Gpt.Interface;
using BoundBot.Application.GrantLicense.Interface;
using BoundBot.Application.StaffLicense.Interface;
using BoundBot.Application.UpdateDiscord.Interface;
using BoundBot.Application.UpdateHwid.Interface;
using BoundBot.Components.RestModel;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Microsoft.Extensions.Logging;
using BoundBot.Application.AlterLicense.Interface;
using BoundBot.Components.GetOptionValue;
using Discord.Rest;

namespace BoundBot.Services
{
    internal class SlashCommandsHandler
    {
        private DiscordSocketClient SocketClient { get; }
        private DiscordRestClient RestClient { get; }
        private IConfiguration Configuration { get; }
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlashCommandsHandler> _logger;
        private readonly IDiscordConnectionHandler _discordConnectionHandler;
        private readonly IUpdateDiscordImplementation _updateDiscordImplementation;
        private readonly IUpdateHwidImplementation _updateHwidImplementation;
        private readonly ICheckLicenseImplementation _checkLicenseImplementation;
        private readonly IGetSellixCouponImplementation _getSellixCouponImplementation;
        private readonly IAdminCheckLicensesImplementation _adminCheckLicensesImplementation;
        private readonly IStaffLicenseImplementation _staffLicenseImplementation;
        private readonly IExtendLicensesImplementation _extendLicensesImplementation;
        private readonly IDatabaseBackupImplementation _databaseBackupImplementation;
        private readonly IGrantLicenseImplementation _grantLicenseImplementation;
        private readonly IGptDiscordImplementation _gptDiscordImplementation;
        private readonly IAlterLicenseImplementation _alterLicenseImplementation;

        public SlashCommandsHandler(DiscordSocketClient client, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<SlashCommandsHandler> logger, IDiscordConnectionHandler discordConnectionHandler, IUpdateDiscordImplementation updateDiscordImplementation, IUpdateHwidImplementation updateHwidImplementation, ICheckLicenseImplementation checkLicenseImplementation, IGetSellixCouponImplementation getSellixCouponImplementation, IAdminCheckLicensesImplementation adminCheckLicensesImplementation, IStaffLicenseImplementation staffLicenseImplementation, IExtendLicensesImplementation extendLicensesImplementation, IDatabaseBackupImplementation databaseBackupImplementation, IGrantLicenseImplementation grantLicenseImplementation, IGptDiscordImplementation gtDiscordImplementation, IAlterLicenseImplementation alterLicenseImplementation, DiscordRestClient restClient)
        {
            SocketClient = client;
            Configuration = configuration;
            _logger = logger;
            _discordConnectionHandler = discordConnectionHandler;
            _updateDiscordImplementation = updateDiscordImplementation;
            _updateHwidImplementation = updateHwidImplementation;
            _checkLicenseImplementation = checkLicenseImplementation;
            _getSellixCouponImplementation = getSellixCouponImplementation;
            _adminCheckLicensesImplementation = adminCheckLicensesImplementation;
            _staffLicenseImplementation = staffLicenseImplementation;
            _extendLicensesImplementation = extendLicensesImplementation;
            _databaseBackupImplementation = databaseBackupImplementation;
            _grantLicenseImplementation = grantLicenseImplementation;
            _gptDiscordImplementation = gtDiscordImplementation;
            _httpClient = httpClientFactory.CreateClient("httpClient");
            _alterLicenseImplementation = alterLicenseImplementation;
            RestClient = restClient;
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await (command.Data.Name.ToLower() switch
            {
                "help" => Help(command),
                null => Help(command),
                "purchase" => Purchase(command),
                "updatediscord" => _updateDiscordImplementation.UpdateDiscord(command, _httpClient),
                "hwid" => _updateHwidImplementation.UpdateHwid(command, _httpClient),
                "checkme" => _checkLicenseImplementation.CheckLicense(command, _httpClient),
                "coupon" => _getSellixCouponImplementation.GetCoupon(command, _httpClient),
                "checkdb" => _adminCheckLicensesImplementation.CheckLicenses(command, _httpClient),
                "stafflicense" => _staffLicenseImplementation.StaffLicense(command, _httpClient),
                "extendlicense" => _extendLicensesImplementation.Extend(command, _httpClient),
                "dbbackup" => _databaseBackupImplementation.DbBackup(command, _httpClient),
                "grantlicense" => _grantLicenseImplementation.GrantLicense(command, _httpClient),
                "aimodelupdate" => _gptDiscordImplementation.UpdateFtModel(command, _httpClient),
                "alterlicense" => _alterLicenseImplementation.AlterLicense(command, _httpClient),
                "reply" => Reply(command, SocketClient),
                _ => Help(command),
            });
        }

        private async Task Help(SocketSlashCommand command)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\n**Make sure you use a proper slash command, and not sending it as a text command.**\n");

            List<string> ignore = ["checkdb", "stafflicense", "extendlicense", "dbbackup", "grantlicense", "embedbuilder", "aimodelupdate", "alterlicense", "reply"];

            foreach (var item in await SocketClient.GetGlobalApplicationCommandsAsync())
            {
                if (ignore.Contains(item.Name.ToLower()))
                    continue;

                sb.AppendLine($"`/{item.Name}`");
            }

            sb.AppendLine("\n**Some commands may be hidden to you depending on role & claims.**");

            DiscordModelDtoRestModel restModel = new(command);

            var embed = new EmbedBuilder()
                            .WithThumbnailUrl("https://i.imgur.com/dxCVy9r.png")
                            .WithDescription($"Commands can only be executed in #{await SocketClient.GetChannelAsync(879325830021529630)}")
                            .AddField("Commands", sb.ToString())
                            .WithColor(Color.DarkOrange)
                            .WithCurrentTimestamp()
                            .Build();

            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embed);
        }

        private async Task Purchase(SocketSlashCommand command)
        {
            try
            {
                DiscordModelDtoRestModel restModel = new(command);

                _logger.LogInformation($"[POST REST] Successfully executed for {command.CommandName}");

                var useClient = SocketClient.GetGuild(ulong.Parse(Configuration["Discord:Guid"]!));

                var embedBuiler = new EmbedBuilder();
                embedBuiler.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";

                embedBuiler.WithTitle("Purchase Information");

                embedBuiler.AddField("Requirement", "`A Core Sub`", true);
                embedBuiler.AddField("Discord ID", $"`{restModel.Model.DiscordId}`", true);
                embedBuiler.AddField("Discord Name", $"`{restModel.Model.DiscordUsername}`", true);
                //embedBuiler.AddField("HWID", "`Found In A Console`", true);
                embedBuiler.AddField("Server Booster", "`/coupon` for 10%", true);
                embedBuiler.AddField("Language Support", "`All Languages Supported (Beta)`", true);
                embedBuiler.AddField("Activation Time", "`Instant`", true);
                embedBuiler.AddField("No Role After Purchase", "`/updatediscord`", true);

                embedBuiler.AddField("Important", "When authenticating with discord upon purchase, " +
                    "\nmake sure it connects the correct account you wish to have the license on!");

                embedBuiler.AddField("Link",
                    "\r\n\r\nThank you for your interest & support in what I do! <:peepolove:1002285157132271746>" +
                    $"\r\n\r\nPurchase here: {Configuration["Store:Link:Sellix"]}");

                embedBuiler.WithColor(Color.DarkOrange);
                embedBuiler.WithCurrentTimestamp();
                await command.RespondAsync(embed: embedBuiler.Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task Reply(SocketSlashCommand command, DiscordSocketClient socketClient)
        {
            try
            {
                await command.DeferAsync(true);

                var guild = await RestClient.GetGuildAsync(Convert.ToUInt64(command.GuildId));
                var user = await guild.GetUserAsync(command.User.Id);

                if (user != null)
                {
                    var role = user.RoleIds;

                    if (role.Contains(ulong.Parse(Configuration["Discord:Role:Admin"]!)) || role.Contains(ulong.Parse(Configuration["Discord:Role:Staff"]!)))
                    {
                        var response = command.GetOptionValues<string>("response")!;

                        var channel = command.Channel;

                        await command.ModifyOriginalResponseAsync(x =>
                        {
                            x.Embed = new EmbedBuilder()
                                .WithDescription("Reply Sent!")
                                .WithColor(Color.DarkOrange)
                                .WithCurrentTimestamp()
                                .Build();
                        });

                        await channel.SendMessageAsync(response);
                    }
                    else
                    {
                        await command.ModifyOriginalResponseAsync(x =>
                        {
                            x.Embed = new EmbedBuilder()
                                .WithDescription("You do not have the required role to use this command.")
                                .WithColor(Color.DarkOrange)
                                .WithCurrentTimestamp()
                                .Build();
                        });
                    }
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(x =>
                    {
                        x.Embed = new EmbedBuilder()
                            .WithDescription("User not found.")
                            .WithColor(Color.DarkOrange)
                            .WithCurrentTimestamp()
                            .Build();
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
