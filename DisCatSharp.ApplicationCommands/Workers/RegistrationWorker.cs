// This file is part of the DisCatSharp project, a fork of DSharpPlus.
//
// Copyright (c) 2021 AITSYS
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisCatSharp.Entities;
using DisCatSharp.Exceptions;
using Microsoft.Extensions.Logging;

namespace DisCatSharp.ApplicationCommands
{
    internal class RegistrationWorker
    {

        

        internal static async Task<List<DiscordApplicationCommand>> RegisterGlobalCommandsAsync(List<DiscordApplicationCommand> Commands)
        {
            var GlobalCommandsOverwriteList = BuildGlobalOverwriteList(Commands);
            var GlobalCommandsCreateList = BuildGlobalCreateList(Commands);
            var GlobalCommandsDeleteList = BuildGlobalDeleteList(Commands);

            if (CheckAnyAndNotNull(GlobalCommandsCreateList) && CheckNotAnyOrNull(GlobalCommandsOverwriteList))
            {
                var cmds = await ApplicationCommandsExtension._client.BulkOverwriteGlobalApplicationCommandsAsync(GlobalCommandsCreateList);
                Commands.AddRange(cmds);
            }
            else if (CheckNotAnyOrNull(GlobalCommandsCreateList) && CheckAnyAndNotNull(GlobalCommandsOverwriteList))
            {
                List<DiscordApplicationCommand> OverwriteList = new();
                foreach (var overwrite in GlobalCommandsOverwriteList)
                {
                    var cmd = overwrite.Value;
                    cmd.Id = overwrite.Key;
                    OverwriteList.Add(cmd);
                }
                var discord_backend_commands = await ApplicationCommandsExtension._client.BulkOverwriteGlobalApplicationCommandsAsync(OverwriteList);
                Commands.AddRange(discord_backend_commands);
            }
            else if (CheckAnyAndNotNull(GlobalCommandsCreateList) && CheckAnyAndNotNull(GlobalCommandsOverwriteList))
            {
                foreach (var cmd in GlobalCommandsCreateList)
                {
                    var discord_backend_command = await ApplicationCommandsExtension._client.CreateGlobalApplicationCommandAsync(cmd);
                    Commands.Add(discord_backend_command);
                }

                foreach (var cmd in GlobalCommandsOverwriteList)
                {
                    var command = cmd.Value;

                    var discord_backend_command = await ApplicationCommandsExtension._client.EditGlobalApplicationCommandAsync(cmd.Key, action =>
                    {
                        action.Name = command.Name;
                        action.NameLocalizations = command.NameLocalizations;
                        action.Description = command.Description;
                        action.DescriptionLocalizations = command.DescriptionLocalizations;
                        if(command.Options != null && command.Options.Any())
                            action.Options = Optional.FromValue(command.Options);
                        action.DefaultPermission = command.DefaultPermission;
                    });
                    Commands.Add(discord_backend_command);
                }
            }

            if (CheckAnyAndNotNull(GlobalCommandsDeleteList))
            {
                foreach (var cmdId in GlobalCommandsDeleteList)
                {
                    try
                    {
                        await ApplicationCommandsExtension._client.DeleteGlobalApplicationCommandAsync(cmdId);
                    }
                    catch (NotFoundException)
                    {
                        ApplicationCommandsExtension._client.Logger.LogError($"Could not delete global command {cmdId}. Please clean up manually");
                    }
                }
            }

            return CheckAnyAndNotNull(Commands) ? Commands : null;
        }

        internal static async Task<List<DiscordApplicationCommand>> RegisterGuilldCommandsAsync(ulong GuildId, List<DiscordApplicationCommand> Commands)
        {
            var GuildCommandsOverwriteList = BuildGuildOverwriteList(GuildId, Commands);
            var GuildCommandsCreateList = BuildGuildCreateList(GuildId, Commands);
            var GuildCommandsDeleteList = BuildGuildDeleteList(GuildId, Commands);

            if (CheckAnyAndNotNull(GuildCommandsCreateList) && CheckNotAnyOrNull(GuildCommandsOverwriteList))
            {
                var cmds = await ApplicationCommandsExtension._client.BulkOverwriteGuildApplicationCommandsAsync(GuildId, GuildCommandsCreateList);
                Commands.AddRange(cmds);
            }
            else if (CheckNotAnyOrNull(GuildCommandsCreateList) && CheckAnyAndNotNull(GuildCommandsOverwriteList))
            {
                List<DiscordApplicationCommand> OverwriteList = new();
                foreach (var overwrite in GuildCommandsOverwriteList)
                {
                    var cmd = overwrite.Value;
                    cmd.Id = overwrite.Key;
                    OverwriteList.Add(cmd);
                }
                var discord_backend_commands = await ApplicationCommandsExtension._client.BulkOverwriteGuildApplicationCommandsAsync(GuildId, OverwriteList);
                Commands.AddRange(discord_backend_commands);
            }
            else if (CheckAnyAndNotNull(GuildCommandsCreateList) && CheckAnyAndNotNull(GuildCommandsOverwriteList))
            {
                foreach (var cmd in GuildCommandsCreateList)
                {
                    var discord_backend_command = await ApplicationCommandsExtension._client.CreateGuildApplicationCommandAsync(GuildId, cmd);
                    Commands.Add(discord_backend_command);
                }

                foreach (var cmd in GuildCommandsOverwriteList)
                {
                    var command = cmd.Value;
                    var discord_backend_command = await ApplicationCommandsExtension._client.EditGuildApplicationCommandAsync(GuildId, cmd.Key, action =>
                    {
                        action.Name = command.Name;
                        action.NameLocalizations = command.NameLocalizations;
                        action.Description = command.Description;
                        action.DescriptionLocalizations = command.DescriptionLocalizations;
                        if(command.Options != null && command.Options.Any())
                            action.Options = Optional.FromValue(command.Options);
                        action.DefaultPermission = command.DefaultPermission;
                    });

                    Commands.Add(discord_backend_command);
                }
            }

            if (CheckAnyAndNotNull(GuildCommandsDeleteList))
            {
                foreach (var cmdId in GuildCommandsDeleteList)
                {
                    try
                    {
                        await ApplicationCommandsExtension._client.DeleteGuildApplicationCommandAsync(GuildId, cmdId);
                    }
                    catch (NotFoundException)
                    {
                        ApplicationCommandsExtension._client.Logger.LogError($"Could not delete guild command {cmdId} in guild {GuildId}. Please clean up manually");
                    }
                }
            }

            return CheckAnyAndNotNull(Commands) ? Commands : null;
        }

        private static bool CheckNotAnyOrNull(List<DiscordApplicationCommand> list)
            => list == null || !list.Any();

        private static bool CheckNotAnyOrNull(List<ulong> list)
            => list == null || !list.Any();

        private static bool CheckNotAnyOrNull(Dictionary<ulong, DiscordApplicationCommand> dict)
            => dict == null || !dict.Any() || dict.Keys == null || !dict.Keys.Any();

        private static bool CheckAnyAndNotNull(List<DiscordApplicationCommand> list)
            => list != null && list.Any();

        private static bool CheckAnyAndNotNull(List<ulong> list)
            => list != null && list.Any();

        private static bool CheckAnyAndNotNull(Dictionary<ulong, DiscordApplicationCommand> dict)
            => dict != null && dict.Any() && dict.Keys != null && dict.Keys.Any();

        private static List<ulong> BuildGuildDeleteList(ulong guildId, List<DiscordApplicationCommand> updateList)
        {
            if (ApplicationCommandsExtension._guildDiscordCommands == null || !ApplicationCommandsExtension._guildDiscordCommands.Any())
                return null;
            else if (updateList == null)
            {
                var discord = ApplicationCommandsExtension._guildDiscordCommands.Where(l => l.Key == guildId).First().Value;

                if (discord != null)
                {
                    List<ulong> InvalidCommandIds = new();
                    foreach (var cmd in discord)
                    {
                        InvalidCommandIds.Add(cmd.Id);
                    }

                    return InvalidCommandIds;
                }
                
                return null;
            }
            else
            {
                var discord = ApplicationCommandsExtension._guildDiscordCommands.Where(l => l.Key == guildId).First().Value;

                if (discord != null)
                {
                    List<ulong> InvalidCommandIds = new();
                    foreach (var cmd in discord)
                    {
                        if (!updateList.Any(ul => ul.Name == cmd.Name))
                            InvalidCommandIds.Add(cmd.Id);
                    }

                    return InvalidCommandIds;
                }

                return null;
            }
        }

        private static List<DiscordApplicationCommand> BuildGuildCreateList(ulong guildId, List<DiscordApplicationCommand> updateList)
        {
            if (ApplicationCommandsExtension._guildDiscordCommands == null || !ApplicationCommandsExtension._guildDiscordCommands.Any() || updateList == null)
                return updateList;
            else
            {
                var discord = ApplicationCommandsExtension._guildDiscordCommands.Where(l => l.Key == guildId).First().Value;
                List<DiscordApplicationCommand> NewCommands = new();
                foreach (var cmd in updateList)
                {
                    if (discord == null || !discord.Any(d => d.Name == cmd.Name))
                    {
                        NewCommands.Add(cmd);
                    }
                }

                return NewCommands;
            }
        }

        private static Dictionary<ulong, DiscordApplicationCommand> BuildGuildOverwriteList(ulong guildId, List<DiscordApplicationCommand> updateList)
        {
            if (ApplicationCommandsExtension._guildDiscordCommands == null || !ApplicationCommandsExtension._guildDiscordCommands.Any() || !ApplicationCommandsExtension._guildDiscordCommands.Any(l => l.Key == guildId) || updateList == null)
                return null;
            else
            {
                var discord = ApplicationCommandsExtension._guildDiscordCommands.Where(l => l.Key == guildId).First().Value;
                if (discord != null)
                {
                    Dictionary<ulong, DiscordApplicationCommand> UpdateCommands = new();
                    foreach (var cmd in updateList)
                    {
                        if (discord.Any(d => d.Name == cmd.Name))
                        {
                            UpdateCommands.Add(discord.Where(d => d.Name == cmd.Name).First().Id, cmd);
                        }
                    }
                    return UpdateCommands;
                }

                return null;
            }
        }

        private static List<ulong> BuildGlobalDeleteList(List<DiscordApplicationCommand> updateList = null)
        {
            if (ApplicationCommandsExtension._globalDiscordCommands == null || !ApplicationCommandsExtension._globalDiscordCommands.Any())
                return null;
            else if (updateList == null)
            {
                var discord = ApplicationCommandsExtension._globalDiscordCommands;

                if (discord != null)
                {
                    List<ulong> InvalidCommandIds = new();
                    foreach (var cmd in discord)
                    {
                        InvalidCommandIds.Add(cmd.Id);
                    }

                    return InvalidCommandIds;
                }
                
                return null;
            }
            else
            {
                var discord = ApplicationCommandsExtension._globalDiscordCommands;

                if (discord != null)
                {
                    List<ulong> InvalidCommandIds = new();
                    foreach (var cmd in discord)
                    {
                        if (!updateList.Any(ul => ul.Name == cmd.Name))
                            InvalidCommandIds.Add(cmd.Id);
                    }

                    return InvalidCommandIds;
                }

                return null;
            }
        }

        private static List<DiscordApplicationCommand> BuildGlobalCreateList(List<DiscordApplicationCommand> updateList)
        {
            if (ApplicationCommandsExtension._globalDiscordCommands == null || !ApplicationCommandsExtension._globalDiscordCommands.Any() || updateList == null)
                return updateList;
            else
            {
                var discord = ApplicationCommandsExtension._globalDiscordCommands;
                List<DiscordApplicationCommand> NewCommands = new();
                foreach (var cmd in updateList)
                {
                    if (discord == null || !discord.Any(d => d.Name == cmd.Name))
                    {
                        NewCommands.Add(cmd);
                    }
                }

                return NewCommands;
            }
        }

        private static Dictionary<ulong, DiscordApplicationCommand> BuildGlobalOverwriteList(List<DiscordApplicationCommand> updateList)
        {
            if (ApplicationCommandsExtension._globalDiscordCommands == null || !ApplicationCommandsExtension._globalDiscordCommands.Any() || updateList == null)
                return null;
            else
            {
                var discord = ApplicationCommandsExtension._globalDiscordCommands;
                if (discord != null)
                {
                    Dictionary<ulong, DiscordApplicationCommand> UpdateCommands = new();
                    foreach (var cmd in updateList)
                    {
                        if (discord.Any(d => d.Name == cmd.Name))
                        {
                            UpdateCommands.Add(discord.Where(d => d.Name == cmd.Name).First().Id, cmd);
                        }
                    }

                    return UpdateCommands;
                }

                return null;
            }
        }
    }
}
