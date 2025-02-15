// This file is part of the DisCatSharp project, based off DSharpPlus.
//
// Copyright (c) 2021-2022 AITSYS
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DisCatSharp.Entities;
using DisCatSharp.Enums;

namespace DisCatSharp.VoiceNext;

/// <summary>
/// The discord client extensions.
/// </summary>
public static class DiscordClientExtensions
{
	/// <summary>
	/// Creates a new VoiceNext client with default settings.
	/// </summary>
	/// <param name="client">Discord client to create VoiceNext instance for.</param>
	/// <returns>VoiceNext client instance.</returns>
	public static VoiceNextExtension UseVoiceNext(this DiscordClient client)
		=> UseVoiceNext(client, new VoiceNextConfiguration());

	/// <summary>
	/// Creates a new VoiceNext client with specified settings.
	/// </summary>
	/// <param name="client">Discord client to create VoiceNext instance for.</param>
	/// <param name="config">Configuration for the VoiceNext client.</param>
	/// <returns>VoiceNext client instance.</returns>
	public static VoiceNextExtension UseVoiceNext(this DiscordClient client, VoiceNextConfiguration config)
	{
		if (client.GetExtension<VoiceNextExtension>() != null)
			throw new InvalidOperationException("VoiceNext is already enabled for that client.");

		var vnext = new VoiceNextExtension(config);
		client.AddExtension(vnext);
		return vnext;
	}

	/// <summary>
	/// Creates new VoiceNext clients on all shards in a given sharded client.
	/// </summary>
	/// <param name="client">Discord sharded client to create VoiceNext instances for.</param>
	/// <param name="config">Configuration for the VoiceNext clients.</param>
	/// <returns>A dictionary of created VoiceNext clients.</returns>
	public static async Task<IReadOnlyDictionary<int, VoiceNextExtension>> UseVoiceNextAsync(this DiscordShardedClient client, VoiceNextConfiguration config)
	{
		var modules = new Dictionary<int, VoiceNextExtension>();
		await client.InitializeShardsAsync().ConfigureAwait(false);

		foreach (var shard in client.ShardClients.Select(xkvp => xkvp.Value))
		{
			var vnext = shard.GetExtension<VoiceNextExtension>();
			vnext ??= shard.UseVoiceNext(config);

			modules[shard.ShardId] = vnext;
		}

		return new ReadOnlyDictionary<int, VoiceNextExtension>(modules);
	}

	/// <summary>
	/// Gets the active instance of VoiceNext client for the DiscordClient.
	/// </summary>
	/// <param name="client">Discord client to get VoiceNext instance for.</param>
	/// <returns>VoiceNext client instance.</returns>
	public static VoiceNextExtension GetVoiceNext(this DiscordClient client)
		=> client.GetExtension<VoiceNextExtension>();

	/// <summary>
	/// Retrieves a <see cref="VoiceNextExtension"/> instance for each shard.
	/// </summary>
	/// <param name="client">The shard client to retrieve <see cref="VoiceNextExtension"/> instances from.</param>
	/// <returns>A dictionary containing <see cref="VoiceNextExtension"/> instances for each shard.</returns>
	public static async Task<IReadOnlyDictionary<int, VoiceNextExtension>> GetVoiceNextAsync(this DiscordShardedClient client)
	{
		await client.InitializeShardsAsync().ConfigureAwait(false);
		var extensions = new Dictionary<int, VoiceNextExtension>();

		foreach (var shard in client.ShardClients.Values)
		{
			extensions.Add(shard.ShardId, shard.GetExtension<VoiceNextExtension>());
		}

		return new ReadOnlyDictionary<int, VoiceNextExtension>(extensions);
	}

	/// <summary>
	/// Connects to this voice channel using VoiceNext.
	/// </summary>
	/// <param name="channel">Channel to connect to.</param>
	/// <returns>If successful, the VoiceNext connection.</returns>
	public static Task<VoiceNextConnection> ConnectAsync(this DiscordChannel channel)
	{
		if (channel == null)
			throw new NullReferenceException();

		if (channel.Guild == null)
			throw new InvalidOperationException("VoiceNext can only be used with guild channels.");

		if (channel.Type != ChannelType.Voice && channel.Type != ChannelType.Stage)
			throw new InvalidOperationException("You can only connect to voice or stage channels.");

		if (channel.Discord is not DiscordClient discord || discord == null)
			throw new NullReferenceException();

		var vnext = discord.GetVoiceNext();
		if (vnext == null)
			throw new InvalidOperationException("VoiceNext is not initialized for this Discord client.");

		var vnc = vnext.GetConnection(channel.Guild);
		return vnc != null
			? throw new InvalidOperationException("VoiceNext is already connected in this guild.")
			: vnext.ConnectAsync(channel);
	}
}
