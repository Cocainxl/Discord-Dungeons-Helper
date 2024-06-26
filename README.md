# C# DISCORD DUNGEONS HELPER BOT (.NET8)

<img src="https://raw.githubusercontent.com/DSharpPlus/DSharpPlus/master/logo/dsharp%2B_smaller.png" alt="image" width="300"> <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/b/bd/Logo_C_sharp.svg/1820px-Logo_C_sharp.svg.png" alt="c#image" width="100">

# :wave: Welcome wowhead's!
- I would like to introduce you to Discord Dungeons Helper. This is a discord bot designed for the World of Warcraft community.
- In this bot we use the .NET8 framework which allows us to use all the benefits of NET Core 8.
- The bot is currently in the basic version, it will be continuously developed. Watch my repository to stay up to date

# :speaking_head: Information from the author
- If you need any help, feel free to write to me on discord: _cocain
- Watch this series of tutorials, you may find it very helpful  -> [C# Discord Bot Tutorial](https://www.youtube.com/playlist?list=PLcpUxmcrEm_A819eppTt09S6EGVH99TSV)
- **Template from the author of a series of tutorials that I recommend, if you want to make your own bot, use them** | [DSharp Bot Template by @samjesus8](https://github.com/samjesus8/CSharp-Discord-Bot-Template-NET8)
- **DSharpPlus API Documentation** | [API Documentation](https://dsharpplus.github.io/DSharpPlus/api/index.html)
- **Join the Official DSharpPlus Server** | [DSharpPlus Official Server](https://discord.com/invite/dsharpplus)

# :hammer_and_wrench: Setup
- The first step is to create a config.json file in the config folder and fill it in with your token and prefix
  ```
  {
    "token": "ENTER-YOUR-TOKEN-HERE",
    "prefix": "ENTER-YOUR-PREFIX-HERE"
  }
- You can find the token and information on how to create your application here -> [Discord Developer Portal](https://discord.com/developers/docs/intro)
- After building the application in the debug version, copy your config.json and paste it into bin -> debug -> net8.0
  
# 🤖 Current bot features
- !help explaining the bot's functions (currently only in Polish)
- Slashcommand /bug which creates a submited module where users can report bugs (in the Dungeons.cs file you have to set the appropriate channel ID to which reports should be sent)
- !check [realm] [nickname] which checks a character's profile on Raider.IO (currently only available for Europe, if you want to use it on other regions edit the CheckCommandsModule file)
- !zapisy [dungeon name without spaces] [keystone level] creates new dungeon enrollments, click on the button with the role you want (useful tool on mythic keystones) 
