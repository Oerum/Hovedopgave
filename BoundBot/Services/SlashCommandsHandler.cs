using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Text;
using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Application.DatabaseBackup.Interface;
using BoundBot.Application.EmbedBuilder.Interface;
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

namespace BoundBot.Services
{
    internal class SlashCommandsHandler
    {
        private DiscordSocketClient Client { get; }
        private IConfiguration Configuration { get; }
        private readonly HttpClient _httpClient;
        private readonly ILogger<SlashCommandsHandler> _logger;
        private readonly IDiscordConnectionHandler _discordConnectionHandler;
        private readonly IEmbedBuilderImplementation _embedBuilderImplementation;
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

        public SlashCommandsHandler(DiscordSocketClient client, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<SlashCommandsHandler> logger, IDiscordConnectionHandler discordConnectionHandler, IEmbedBuilderImplementation embedBuilderImplementation, IUpdateDiscordImplementation updateDiscordImplementation, IUpdateHwidImplementation updateHwidImplementation, ICheckLicenseImplementation checkLicenseImplementation, IGetSellixCouponImplementation getSellixCouponImplementation, IAdminCheckLicensesImplementation adminCheckLicensesImplementation, IStaffLicenseImplementation staffLicenseImplementation, IExtendLicensesImplementation extendLicensesImplementation, IDatabaseBackupImplementation databaseBackupImplementation, IGrantLicenseImplementation grantLicenseImplementation, IGptDiscordImplementation gtDiscordImplementation, IAlterLicenseImplementation alterLicenseImplementation)
        {
            Client = client;
            Configuration = configuration;
            _logger = logger;
            _discordConnectionHandler = discordConnectionHandler;
            _embedBuilderImplementation = embedBuilderImplementation;
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
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // Let's add a switch statement for the command name so we can handle multiple commands in one event.
            switch (command.Data.Name)
            {
                case "help":
                case null:
                    await Help(command);
                    break;

                case "purchase":
                    await Purchase(command);
                    break;

                case "updatediscord":
                    await _updateDiscordImplementation.UpdateDiscord(command, _httpClient);
                    break;

                case "hwid":
                    await _updateHwidImplementation.UpdateHwid(command, _httpClient);
                    break;

                case "checkme":
                    await _checkLicenseImplementation.CheckLicense(command, _httpClient);
                    break;

                case "coupon":
                    await _getSellixCouponImplementation.GetCoupon(command, _httpClient);
                    break;

                case "checkdb":
                    await _adminCheckLicensesImplementation.CheckLicenses(command, _httpClient);
                    break;

                case "stafflicense":
                    await _staffLicenseImplementation.StaffLicense(command, _httpClient);
                    break;

                case "extendlicense":
                    await _extendLicensesImplementation.Extend(command, _httpClient);
                    break;

                case "dbbackup":
                    await _databaseBackupImplementation.DbBackup(command, _httpClient);
                    break;

                case "grantlicense":
                    await _grantLicenseImplementation.GrantLicense(command, _httpClient);
                    break;

                case "embedbuilder":
                    await _embedBuilderImplementation.EmbedBuilder(command, _httpClient);
                    break;

                case "aimodelupdate":
                    await _gptDiscordImplementation.UpdateFtModel(command, _httpClient);
                    break;

                case "alterlicense":
                    await _alterLicenseImplementation.AlterLicense(command, _httpClient);
                    break;
            }
        }

        private async Task Help(SocketSlashCommand command)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\n**Make sure you use a proper slash command, and not sending it as a text command.**\n");

            foreach (var item in await Client.GetGlobalApplicationCommandsAsync())
            {
                sb.AppendLine($"`/{item.Name}`");
            }

            sb.AppendLine("\n**Some commands may be hidden to you depending on role & claims.**");

            DiscordModelDtoRestModel restModel = new(command);

            var embed = new EmbedBuilder()
                            .WithThumbnailUrl("https://i.imgur.com/dxCVy9r.png")
                            .WithDescription($"Commands can only be executed in #{Client.GetChannelAsync(879325830021529630)}")
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

                var useClient = Client.GetGuild(ulong.Parse(Configuration["Discord:Guid"]!));

                var embedBuiler = new EmbedBuilder();
                embedBuiler.ThumbnailUrl = "https://i.imgur.com/dxCVy9r.png";

                embedBuiler.WithTitle("Purchase Information");

                embedBuiler.AddField("Requirement", "`A# Core Sub`", true);
                embedBuiler.AddField("Services", $"{useClient!.GetTextChannel(useClient.Channels.First(x => x.Name.ToLower().Contains("📢-available-services"))!.Id).Mention ?? useClient.GetTextChannel(860603152280584226).Mention}", true);
                embedBuiler.AddField("Discord ID", $"`{restModel.Model.DiscordId}`", true);
                embedBuiler.AddField("Discord Name", $"`{restModel.Model.DiscordUsername}`", true);
                embedBuiler.AddField("HWID", "`Found In A# Console`", true);
                embedBuiler.AddField("Server Booster", "`/coupon` for 10%", true);
                embedBuiler.AddField("Language Support", "`English Client Only`", true);
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
    }
}
