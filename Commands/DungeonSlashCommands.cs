using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace Dungeons.Commands
{
    public class DungeonSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("bug", "Tworzy report z opisem błędu")]
        public async Task ReportCommand(InteractionContext ctx)
        {
            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Test report modal")
                .WithCustomId("report")
                .AddComponents(new TextInputComponent(label: "Opis błędu", customId: "report-title", "Błąd z zapisami na dungeon...", "", true));
            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }
    }
}
