using System;
using Dungeons.Commands;
using Dungeons.Config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.CommandsNext.Attributes;
using System.Net.Http;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Dungeons
{
    public sealed class Dungeons
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            var configJsonFile = new BotConfig();
            await configJsonFile.ReadJSON();

            var services = new ServiceCollection();
            services.AddSingleton<HttpClient>();

            var serviceProvider = services.BuildServiceProvider();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.DiscordBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += OnClientReady;
            Client.ModalSubmitted += ModalSubmittedHandler;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.DiscordBotPrefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
                Services = serviceProvider
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfiguration = Client.UseSlashCommands();

            slashCommandsConfiguration.RegisterCommands<DungeonSlashCommands>();
            Commands.RegisterCommands<Basic>();
            Commands.RegisterCommands<CheckCommandModule>();



            var basicCommandCount = CountRegisteredCommands<Basic>();


            Console.WriteLine("M     M  OOO   RRRR   DDDD    OOO   RRRR    OOO   W   W  NN  N  III  AAAAAAA");
            Console.WriteLine("MM   MM O   O  R   R  D   D  O   O  R   R  O   O  W W W  NNN N   I   A     A");
            Console.WriteLine("M M M M O   O  RRRR   D   D  O   O  RRRR   O   O  W W W  N N N   I   AAAAAAA");
            Console.WriteLine("M  M  M O   O  R  R   D   D  O   O  R  R   O   O  W W W  N  NN   I   A     A");
            Console.WriteLine("M     M  OOO   R   R  DDDD    OOO   R   R   OOO    W W   N   N  III  A     A");
            Console.WriteLine("============================================= \n" +
                              "Dungeons created by Hexxi with DSharp.NET8 \n" +
                              $"[🔑] Loaded {basicCommandCount} commands from Basic.cs \n" +
                              $"[🔑] Loaded Mythic Keystone list \n" +
                              $"[🔑] Loaded raid member list \n" +
                              $"[🔑] Loaded Raider.IO integration \n" +
                              "=============================================");

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task ModalSubmittedHandler(DiscordClient sender, ModalSubmitEventArgs args)
        {
            if (args.Interaction.Type == InteractionType.ModalSubmit)
            {
                var reportValues = args.Values;
                var reportContent = "";

                foreach (var kvp in reportValues)
                {
                    reportContent += $"{kvp.Key}: {kvp.Value}\n";
                }

                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.Interaction.User.Username} dziękujemy za zgłoszenie błędu!"));
                await BuildAndSendReportEmbed(sender, reportContent, args);
                Console.WriteLine(reportValues);
            }
        }

        private static async Task BuildAndSendReportEmbed(DiscordClient sender, string reportContent, ModalSubmitEventArgs args)
        {
            var targetChannelId = 1238639114098839562; // ID docelowego kanału
            var targetChannel = await sender.GetChannelAsync((ulong)targetChannelId) as DiscordChannel;

            if (targetChannel == null)
            {
                Console.WriteLine("Nie można znaleźć docelowego kanału tekstowego.");
                return;
            }

            var reportEmbed = new DiscordEmbedBuilder()
                .WithAuthor($"⚠️ Nowy raport błędu")
                .WithColor(DiscordColor.Purple)
                .WithDescription(reportContent)
                .WithThumbnail("https://images-ext-1.discordapp.net/external/d92evqK0QnhQruadBs_WlZwo9diNqnvceDMtBpXZjY4/%3Fsize%3D1024/https/cdn.discordapp.com/avatars/349940323579068417/0186db60bb6742236fe5a236aa0d57f4.png?format=webp&quality=lossless")
                .WithFooter($"{args.Interaction.User.Username} zgłosił nowy błąd o godzinie {DateTime.Now}");

            await targetChannel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(reportEmbed));
        }

        private static int CountRegisteredCommands<T>() where T : class
        {
            var type = typeof(T);
            var methods = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0);
            return methods.Count();
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
