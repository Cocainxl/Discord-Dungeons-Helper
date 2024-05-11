using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.EventArgs;
using System.Linq;

namespace Dungeons.Commands
{
    public class Basic : BaseCommandModule
    {

        [Command("help")]
        public async Task HelpCommand(CommandContext cxt)
        {
            var botAvatarUrl = cxt.Client.CurrentUser.AvatarUrl;
            var userId = 349940323579068417;
            var user = await cxt.Guild.GetMemberAsync((ulong)userId);
            var footerText = $"Dungeons Bot created by Hexxi 🥸";

            var helpMessage = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.DarkGreen)
                .WithAuthor("Jak korzystać z Dungeons Bot")
                .WithThumbnail(botAvatarUrl)
                .WithDescription("Znajdziesz tutaj wszystkie potrzebne dla Ciebie komendy, których możesz używać. Jeżeli znalazłeś jakiś błąd wpisz **/bug** i uzupełnij szablon, pomoże mi to szybciej rozwiązać problem i naprawić bota.")
                .AddField("Tworzenie zapisów na dungeon: !zapisy", $"Wpisz **!zapisy [nazwa dungeonu] [poziom klucza]** aby uruchomić zapisy na dany klucz, następnie kliknij w przycisk roli jaką chcesz pełnić na danym dungeonie. \n⚠️ **Niepoprawne użycie komendy**: !zapisy Brackenhide Hollow 6 \n 👍 **Poprawne użycie komendy**: !zapisy Brackenhide_Hollow 6")
                .AddField("Sprawdzanie informacji Raider.IO: !check", "Wpisz **!check [realm] [nazwa postaci]** aby wyświetlić informacje nt. danej postaci pobierane z Raider.IO \n 👍 **Poprawne użycie komendy**: !check BurningLegion Nervvous")
                .WithFooter(footerText);

            await cxt.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(helpMessage));
        }

        [Command("purge")]
        [Description("Usuwa ostatnie wiadomości z kanału.")]
        public async Task Purge(CommandContext ctx, int count)
        {
            // Sprawdź, czy użytkownik ma wymaganą rolę
            if (!(ctx.Member.Roles.Any(r => r.Id == 978002217623773275) || ctx.Member.Roles.Any(r => r.Id == 974016126390501377)))
            {
                // Oznacz użytkownika i wyślij odpowiedź
                await ctx.RespondAsync($"{ctx.Member.Mention} nie dla psa kiełbasa gagatku, to komenda dla administracji!");
                return;
            }

            if (count < 1 || count > 100)
            {
                await ctx.RespondAsync("Liczba musi być z zakresu od 1 do 100.");
                return;
            }

            var messages = await ctx.Channel.GetMessagesAsync(count + 1);

            await ctx.Channel.DeleteMessagesAsync(messages);

            var response = await ctx.RespondAsync($"Usunięto {count} ostatnich wiadomości.");

            await Task.Delay(TimeSpan.FromSeconds(5));
            await response.DeleteAsync();
        }

        public DateTime createdData { get; set; }

        public class SignUpData
        {
            public DiscordMessage Message { get; set; }
            public DiscordEmbed Embed { get; set; }
            public List<DiscordUser> TankUsers { get; set; }
            public List<DiscordUser> HealUsers { get; set; }
            public List<DiscordUser> DpsUsers { get; set; }
        }

        public class SignUpManager
        {
            private List<SignUpData> signUpDataList;

            public SignUpManager()
            {
                signUpDataList = new List<SignUpData>();
            }

            public async Task<SignUpData> CreateSignUp(CommandContext context, string title, string keyLevel)
            {
                var botAvatarUrl = context.Client.CurrentUser.AvatarUrl;
                var createSignUp = DateTime.Now;

                var buttons = new DiscordButtonComponent(ButtonStyle.Primary, "DPS", "DPS", false);
                var buttons2 = new DiscordButtonComponent(ButtonStyle.Primary, "TANK", "TANK", false);
                var buttons3 = new DiscordButtonComponent(ButtonStyle.Primary, "HEAL", "HEAL", false);

                var embed = new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.DarkRed)
                    .WithTitle($"🔑 {title} +{keyLevel}")
                    .WithThumbnail(botAvatarUrl)
                    .WithDescription($"{context.Member.DisplayName} rozpoczął zapisy na dungeon. Kliknij w przycisk aby wybrać rolę jaką chcesz pełnić. Po zebraniu całej drużyny warto wejść na kanał głosowy. Życzymy miłej zabawy na kluczu i powodzenia!")
                    .WithFooter($"Data stworzenia zapisów: {createSignUp}", $"{botAvatarUrl}")
                    .WithAuthor($"Kreator tworzenia dungeonu")
                    .AddField("🛡️ TANK", "Nikt", true)
                    .AddField("🚑 HEAL", "Nikt", true)
                    .AddField("⚔️ DPS", "Nikt", true);

                var message = await context.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed).AddComponents(buttons, buttons2, buttons3));

                var signUpData = new SignUpData
                {
                    Message = message,
                    Embed = embed.Build(),
                    TankUsers = new List<DiscordUser>(),
                    HealUsers = new List<DiscordUser>(),
                    DpsUsers = new List<DiscordUser>()
                };

                signUpDataList.Add(signUpData);

                return signUpData;
            }

            public async Task UpdateSignUp(SignUpData signUpData)
            {
                var newEmbed = signUpData.Embed;
                newEmbed.Fields[0].Value = string.Join("\n", signUpData.TankUsers.Select(user => user.Mention));
                newEmbed.Fields[1].Value = string.Join("\n", signUpData.HealUsers.Select(user => user.Mention));
                newEmbed.Fields[2].Value = string.Join("\n", signUpData.DpsUsers.Select(user => user.Mention));

                var messageBuilder = new DiscordMessageBuilder().AddEmbed(newEmbed);

                messageBuilder.ClearComponents();

                messageBuilder.AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, "DPS", "DPS", false),
                    new DiscordButtonComponent(ButtonStyle.Primary, "TANK", "TANK", false),
                    new DiscordButtonComponent(ButtonStyle.Primary, "HEAL", "HEAL", false)
                );

                await signUpData.Message.ModifyAsync(messageBuilder);
            }

        }

        [Command("zapisy")]
        public async Task SingUpCommands(CommandContext context, string title, string keyLevel)
        {
            var signUpManager = new SignUpManager();
            var signUpData = await signUpManager.CreateSignUp(context, title, keyLevel);

            async Task ButtonHandler(DiscordClient client, ComponentInteractionCreateEventArgs args, SignUpData data)
            {
                var member = args.User;

                switch (args.Id)
                {
                    case "TANK":
                        if (signUpData.TankUsers.Count < 1 && !signUpData.HealUsers.Contains(member) && !signUpData.DpsUsers.Contains(member))
                        {
                            signUpData.TankUsers.Add(member);
                        }
                        break;
                    case "HEAL":
                        if (signUpData.HealUsers.Count < 1 && !signUpData.TankUsers.Contains(member) && !signUpData.DpsUsers.Contains(member))
                        {
                            signUpData.HealUsers.Add(member);
                        }
                        break;
                    case "DPS":
                        if (signUpData.DpsUsers.Count < 3 && !signUpData.TankUsers.Contains(member) && !signUpData.HealUsers.Contains(member))
                        {
                            signUpData.DpsUsers.Add(member);
                        }
                        break;
                }

                await signUpManager.UpdateSignUp(data);
            }
            context.Client.ComponentInteractionCreated += (client, args) => ButtonHandler(client, args, signUpData);
            await Task.Delay(TimeSpan.FromMinutes(5));
            context.Client.ComponentInteractionCreated -= (client, args) => ButtonHandler(client, args, signUpData);
        }
    }
}
