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

using System.Collections.Generic;

using DisCatSharp.Entities;
using DisCatSharp.Enums;

using Newtonsoft.Json;

namespace DisCatSharp.Net.Abstractions;

/// <summary>
/// The transport application.
/// </summary>
internal sealed class TransportApplication
{
	/// <summary>
	/// Gets or sets the id.
	/// </summary>
	[JsonProperty("id", NullValueHandling = NullValueHandling.Include)]
	public ulong Id { get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	[JsonProperty("name", NullValueHandling = NullValueHandling.Include)]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the icon hash.
	/// </summary>
	[JsonProperty("icon", NullValueHandling = NullValueHandling.Include)]
	public string IconHash { get; set; }

	/// <summary>
	/// Gets or sets the description.
	/// </summary>
	[JsonProperty("description", NullValueHandling = NullValueHandling.Include)]
	public string Description { get; set; }

	/// <summary>
	/// Gets or sets the summary.
	/// </summary>
	[JsonProperty("summary", NullValueHandling = NullValueHandling.Include)]
	public string Summary { get; set; }

	/// <summary>
	/// Whether the bot is public.
	/// </summary>
	[JsonProperty("bot_public", NullValueHandling = NullValueHandling.Include)]
	public bool IsPublicBot { get; set; }

	/// <summary>
	/// Gets or sets the flags.
	/// </summary>
	[JsonProperty("flags", NullValueHandling = NullValueHandling.Include)]
	public ApplicationFlags Flags { get; set; }

	/// <summary>
	/// Gets or sets the terms of service url.
	/// </summary>
	[JsonProperty("terms_of_service_url", NullValueHandling = NullValueHandling.Include)]
	public string TermsOfServiceUrl { get; set; }

	/// <summary>
	/// Gets or sets the privacy policy url.
	/// </summary>
	[JsonProperty("privacy_policy_url", NullValueHandling = NullValueHandling.Include)]
	public string PrivacyPolicyUrl { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the bot requires code grant.
	/// </summary>
	[JsonProperty("bot_require_code_grant", NullValueHandling = NullValueHandling.Include)]
	public bool BotRequiresCodeGrant { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the bot is a hook.
	/// </summary>
	[JsonProperty("hook", NullValueHandling = NullValueHandling.Ignore)]
	public bool IsHook { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the bot requires code grant.
	/// </summary>
	[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
	public string Type { get; set; }

	/// <summary>
	/// Gets or sets the rpc origins.
	/// </summary>
	[JsonProperty("rpc_origins", NullValueHandling = NullValueHandling.Ignore)]
	public IList<string> RpcOrigins { get; set; }

	/// <summary>
	/// Gets or sets the owner.
	/// </summary>
	[JsonProperty("owner", NullValueHandling = NullValueHandling.Include)]
	public TransportUser Owner { get; set; }

	/// <summary>
	/// Gets or sets the team.
	/// </summary>
	[JsonProperty("team", NullValueHandling = NullValueHandling.Include)]
	public TransportTeam Team { get; set; }

	/// <summary>
	/// Gets or sets the verify key.
	/// </summary>
	[JsonProperty("verify_key", NullValueHandling = NullValueHandling.Include)]
	public Optional<string> VerifyKey { get; set; }

	/// <summary>
	/// Gets or sets the guild id.
	/// </summary>
	[JsonProperty("guild_id")]
	public Optional<ulong> GuildId { get; set; }

	/// <summary>
	/// Gets or sets the primary sku id.
	/// </summary>
	[JsonProperty("primary_sku_id")]
	public Optional<ulong> PrimarySkuId { get; set; }

	/// <summary>
	/// Gets or sets the slug.
	/// </summary>
	[JsonProperty("slug")]
	public Optional<string> Slug { get; set; }

	/// <summary>
	/// Gets or sets the cover image hash.
	/// </summary>
	[JsonProperty("cover_image")]
	public Optional<string> CoverImageHash { get; set; }

	/// <summary>
	/// Gets or sets the custom install url.
	/// </summary>
	[JsonProperty("custom_install_url")]
	public string CustomInstallUrl { get; set; }

	/// <summary>
	/// Gets or sets the install params.
	/// </summary>
	[JsonProperty("install_params", NullValueHandling = NullValueHandling.Include)]
	public DiscordApplicationInstallParams InstallParams { get; set; }

	/// <summary>
	/// Gets or sets the tags.
	/// </summary>
	[JsonProperty("tags", NullValueHandling = NullValueHandling.Include)]
	public IEnumerable<string> Tags { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TransportApplication"/> class.
	/// </summary>
	internal TransportApplication()
	{ }
}
