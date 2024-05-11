using System;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;

public class CheckCommandModule : BaseCommandModule
{
    private readonly HttpClient _httpClient;
    private const string RaiderIOBaseUrl = "https://raider.io/api/v1/characters/profile";


    public CheckCommandModule(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [Command("check")]
    public async Task Check(CommandContext ctx, string realm, string characterName)
    {
        try
        {
            string url = $"{RaiderIOBaseUrl}?region=eu&realm={realm}&name={characterName}&fields=gear%2Cmythic_plus_best_runs%2Cmythic_plus_recent_runs%2Craid_progression%2Cmythic_plus_scores_by_season:current";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var characterData = JObject.Parse(responseBody);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{characterData["name"]} - {characterData["realm"]}",
                Color = DiscordColor.DarkRed,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = $"{characterData["thumbnail_url"]}"
                },
            };

            if (characterData["gear"] != null && characterData["gear"]["item_level_equipped"] != null)
            {
                embed.AddField("Item Level", $"{characterData["gear"]["item_level_equipped"]}", true);
                embed.AddField("Klasa postaci", $"{characterData["class"]}", true);
                embed.AddField("Specjalizacja", $"{characterData["active_spec_name"]}", true);
                embed.AddField("Frakcja", $"{characterData["faction"]}", true);
            }

            var rioScore = characterData["mythic_plus_scores_by_season"];
            Console.WriteLine($"{characterData}");
            if (rioScore != null && rioScore.HasValues)
            {
                Console.WriteLine("dziala przed rioscoreinfo");
                string rioScoreInfo = "";
                foreach (var season in rioScore)
                {
                    var allScore = season["scores"]["all"];
                    if (allScore != null)
                    {
                        rioScoreInfo += $"{allScore.Value<float>()} \n";
                    }
                }
                embed.AddField("Rio", rioScoreInfo, true);
                Console.WriteLine($"{rioScoreInfo}");
            }

            var bestRuns = characterData["mythic_plus_best_runs"];
            if (bestRuns != null && bestRuns.HasValues)
            {
                string bestRunsInfo = "";
                int count = 0;
                foreach (var run in bestRuns)
                {
                    if (count >= 5) break;
                    bestRunsInfo += $"{run["dungeon"]} +{run["mythic_level"]} ({run["score"]} score) \n";
                    count++;
                }
                embed.AddField("Najlepsze klucze (5)", bestRunsInfo);
            }

            var recentRuns = characterData["mythic_plus_recent_runs"];
            if (recentRuns != null && recentRuns.HasValues)
            {
                string recentRunsInfo = "";
                int count = 0;
                foreach (var run in recentRuns)
                {
                    if (count >= 5) break;
                    recentRunsInfo += $"{run["dungeon"]} +{run["mythic_level"]} ({run["score"]} score)\n";
                    count++;
                }
                embed.AddField("Ostatnie klucze Mythic Plus (5)", recentRunsInfo);
            }

            else
            {
                embed.AddField("Ostatnie klucze Mythic Plus", "Brak danych");
            }

            await ctx.RespondAsync(embed: embed.Build());
        }
        catch (Exception ex)
        {
            await ctx.RespondAsync($"Wystąpił błąd: {ex.Message}");
        }
    }
}