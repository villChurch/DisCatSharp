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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using DisCatSharp.Enums;
using DisCatSharp.Exceptions;
using DisCatSharp.Net.Models;
using DisCatSharp.Net.Serialization;

using Newtonsoft.Json;

namespace DisCatSharp.Entities;

/// <summary>
/// Represents a discord thread channel.
/// </summary>
public class DiscordThreadChannel : DiscordChannel
{
	/// <summary>
	/// Gets ID of the owner that started this thread.
	/// </summary>
	[JsonProperty("owner_id", NullValueHandling = NullValueHandling.Ignore)]
	public ulong OwnerId { get; internal set; }

	[JsonProperty("total_message_sent", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public int TotalMessagesSent { get; internal set; }

	/// <summary>
	/// Gets an approximate count of messages in a thread, stops counting at 50.
	/// </summary>
	[JsonProperty("message_count", NullValueHandling = NullValueHandling.Ignore)]
	public int? MessageCount { get; internal set; }

	/// <summary>
	/// Gets an approximate count of users in a thread, stops counting at 50.
	/// </summary>
	[JsonProperty("member_count", NullValueHandling = NullValueHandling.Ignore)]
	public int? MemberCount { get; internal set; }

	/// <summary>
	/// Represents the current member for this thread. This will have a value if the user has joined the thread.
	/// </summary>
	[JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
	public DiscordThreadChannelMember CurrentMember { get; internal set; }

	/// <summary>
	/// Gets the threads metadata.
	/// </summary>
	[JsonProperty("thread_metadata", NullValueHandling = NullValueHandling.Ignore)]
	public DiscordThreadChannelMetadata ThreadMetadata { get; internal set; }

	/// <summary>
	/// Gets the thread members object.
	/// </summary>
	[JsonIgnore]
	public IReadOnlyDictionary<ulong, DiscordThreadChannelMember> ThreadMembers => new ReadOnlyConcurrentDictionary<ulong, DiscordThreadChannelMember>(this.ThreadMembersInternal);

	[JsonProperty("thread_member", NullValueHandling = NullValueHandling.Ignore)]
	[JsonConverter(typeof(SnowflakeArrayAsDictionaryJsonConverter))]
	internal ConcurrentDictionary<ulong, DiscordThreadChannelMember> ThreadMembersInternal;

	/// <summary>
	/// List of applied tag ids.
	/// </summary>
	[JsonIgnore]
	internal IReadOnlyList<ulong> AppliedTagIds
		=> this.AppliedTagIdsInternal;

	/// <summary>
	/// List of applied tag ids.
	/// </summary>
	[JsonProperty("applied_tags", NullValueHandling = NullValueHandling.Ignore)]
	internal List<ulong> AppliedTagIdsInternal;

	/// <summary>
	/// Gets the list of applied tags.
	/// Only applicable for forum channel posts.
	/// </summary>
	[JsonIgnore]
	public IEnumerable<ForumPostTag> AppliedTags
	  => this.AppliedTagIds?.Select(id => this.Parent.GetForumPostTag(id)).Where(x => x != null);

	/// <summary>
	/// Initializes a new instance of the <see cref="DiscordThreadChannel"/> class.
	/// </summary>
	internal DiscordThreadChannel()
	{ }

	#region Methods

	/// <summary>
	/// Modifies the current thread.
	/// </summary>
	/// <param name="action">Action to perform on this thread</param>
	/// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageThreads"/> permission.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	/// <exception cref="NotSupportedException">Thrown when the <see cref="ThreadAutoArchiveDuration"/> cannot be modified. This happens, when the guild hasn't reached a certain boost <see cref="PremiumTier"/>.</exception>
	public Task ModifyAsync(Action<ThreadEditModel> action)
	{
		var mdl = new ThreadEditModel();
		action(mdl);

		var canContinue = !mdl.AutoArchiveDuration.HasValue || !mdl.AutoArchiveDuration.Value.HasValue || Utilities.CheckThreadAutoArchiveDurationFeature(this.Guild, mdl.AutoArchiveDuration.Value.Value);
		if (mdl.Invitable.HasValue)
		{
			canContinue = this.Guild.Features.CanCreatePrivateThreads;
		}
		return canContinue ? this.Discord.ApiClient.ModifyThreadAsync(this.Id, this.Parent.Type, mdl.Name, mdl.Locked, mdl.Archived, mdl.PerUserRateLimit, mdl.AutoArchiveDuration, mdl.Invitable, mdl.AppliedTags, mdl.AuditLogReason) : throw new NotSupportedException($"Cannot modify ThreadAutoArchiveDuration. Guild needs boost tier {(mdl.AutoArchiveDuration.Value.Value == ThreadAutoArchiveDuration.ThreeDays ? "one" : "two")}.");
	}

	/// <summary>
	/// Add a tag to the current thread.
	/// </summary>
	/// <param name="tag">The tag to add.</param>
	/// <param name="reason">The reason for the audit logs.</param>
	/// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageThreads"/> permission.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task AddTagAsync(ForumPostTag tag, string reason = null)
		=> await this.Discord.ApiClient.ModifyThreadAsync(this.Id, this.Parent.Type, null, null, null, null, null, null, new List<ForumPostTag>(this.AppliedTags) { tag }, reason: reason);

	/// <summary>
	/// Remove a tag from the current thread.
	/// </summary>
	/// <param name="tag">The tag to remove.</param>
	/// <param name="reason">The reason for the audit logs.</param>
	/// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageThreads"/> permission.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task RemoveTagAsync(ForumPostTag tag, string reason = null)
		=> await this.Discord.ApiClient.ModifyThreadAsync(this.Id, this.Parent.Type, null, null, null, null, null, null, new List<ForumPostTag>(this.AppliedTags).Where(x => x != tag).ToList(), reason: reason);

	/// <summary>
	/// Archives a thread.
	/// </summary>
	/// <param name="locked">Whether the thread should be locked.</param>
	/// <param name="reason">Reason for audit logs.</param>
	/// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageThreads"/> permission.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task ArchiveAsync(bool locked = true, string reason = null)
		=> this.Discord.ApiClient.ModifyThreadAsync(this.Id, this.Parent.Type, null, locked, true, null, null, null, null, reason: reason);

	/// <summary>
	/// Unarchives a thread.
	/// </summary>
	/// <param name="reason">Reason for audit logs.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task UnarchiveAsync(string reason = null)
		=> this.Discord.ApiClient.ModifyThreadAsync(this.Id, this.Parent.Type, null, null, false, null, null, null, null, reason: reason);

	/// <summary>
	/// Gets the members of a thread. Needs the <see cref="DiscordIntents.GuildMembers"/> intent.
	/// </summary>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task<IReadOnlyList<DiscordThreadChannelMember>> GetMembersAsync()
		=> await this.Discord.ApiClient.GetThreadMembersAsync(this.Id);

	/// <summary>
	/// Adds a member to this thread.
	/// </summary>
	/// <param name="memberId">The member id to be added.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task AddMemberAsync(ulong memberId)
		=> this.Discord.ApiClient.AddThreadMemberAsync(this.Id, memberId);

	/// <summary>
	/// Adds a member to this thread.
	/// </summary>
	/// <param name="member">The member to be added.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task AddMemberAsync(DiscordMember member)
		=> this.AddMemberAsync(member.Id);

	/// <summary>
	/// Gets a member in this thread.
	/// </summary>
	/// <param name="memberId">The id of the member to get.</param>
	/// <exception cref="NotFoundException">Thrown when the member is not part of the thread.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task<DiscordThreadChannelMember> GetMemberAsync(ulong memberId)
		=> this.Discord.ApiClient.GetThreadMemberAsync(this.Id, memberId);


	/// <summary>
	/// Tries to get a member in this thread.
	/// </summary>
	/// <param name="memberId">The id of the member to get.</param>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task<DiscordThreadChannelMember?> TryGetMemberAsync(ulong memberId)
	{
		try
		{
			return await this.GetMemberAsync(memberId).ConfigureAwait(false);
		}
		catch (NotFoundException)
		{
			return null;
		}
	}

	/// <summary>
	/// Gets a member in this thread.
	/// </summary>
	/// <param name="member">The member to get.</param>
	/// <exception cref="NotFoundException">Thrown when the member is not part of the thread.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task<DiscordThreadChannelMember> GetMemberAsync(DiscordMember member)
		=> this.Discord.ApiClient.GetThreadMemberAsync(this.Id, member.Id);

	/// <summary>
	/// Tries to get a member in this thread.
	/// </summary>
	/// <param name="member">The member to get.</param>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task<DiscordThreadChannelMember?> TryGetMemberAsync(DiscordMember member)
	{
		try
		{
			return await this.GetMemberAsync(member).ConfigureAwait(false);
		}
		catch (NotFoundException)
		{
			return null;
		}
	}
	
	/// <summary>
	/// Removes a member from this thread.
	/// </summary>
	/// <param name="memberId">The member id to be removed.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task RemoveMemberAsync(ulong memberId)
		=> this.Discord.ApiClient.RemoveThreadMemberAsync(this.Id, memberId);

	/// <summary>
	/// Removes a member from this thread. Only applicable to private threads.
	/// </summary>
	/// <param name="member">The member to be removed.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task RemoveMemberAsync(DiscordMember member)
		=> this.RemoveMemberAsync(member.Id);

	/// <summary>
	/// Adds a role to this thread. Only applicable to private threads.
	/// </summary>
	/// <param name="roleId">The role id to be added.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task AddRoleAsync(ulong roleId)
	{
		var role = this.Guild.GetRole(roleId);
		var members = await this.Guild.GetAllMembersAsync();
		var roleMembers = members.Where(m => m.Roles.Contains(role));
		foreach (var member in roleMembers)
		{
			await this.Discord.ApiClient.AddThreadMemberAsync(this.Id, member.Id);
		}
	}

	/// <summary>
	/// Adds a role to this thread. Only applicable to private threads.
	/// </summary>
	/// <param name="role">The role to be added.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task AddRoleAsync(DiscordRole role)
		=> this.AddRoleAsync(role.Id);

	/// <summary>
	/// Removes a role from this thread. Only applicable to private threads.
	/// </summary>
	/// <param name="roleId">The role id to be removed.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public async Task RemoveRoleAsync(ulong roleId)
	{
		var role = this.Guild.GetRole(roleId);
		var members = await this.Guild.GetAllMembersAsync();
		var roleMembers = members.Where(m => m.Roles.Contains(role));
		foreach (var member in roleMembers)
		{
			await this.Discord.ApiClient.RemoveThreadMemberAsync(this.Id, member.Id);
		}
	}

	/// <summary>
	/// Removes a role from this thread. Only applicable to private threads.
	/// </summary>
	/// <param name="role">The role to be removed.</param>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task RemoveRoleAsync(DiscordRole role)
		=> this.RemoveRoleAsync(role.Id);

	/// <summary>
	/// Joins a thread.
	/// </summary>
	/// <exception cref="UnauthorizedException">Thrown when the client has no access to this thread.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task JoinAsync()
		=> this.Discord.ApiClient.JoinThreadAsync(this.Id);

	/// <summary>
	/// Leaves a thread.
	/// </summary>
	/// <exception cref="UnauthorizedException">Thrown when the client has no access to this thread.</exception>
	/// <exception cref="NotFoundException">Thrown when the thread does not exist.</exception>
	/// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
	/// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
	public Task LeaveAsync()
		=> this.Discord.ApiClient.LeaveThreadAsync(this.Id);

	/// <summary>
	/// Returns a string representation of this thread.
	/// </summary>
	/// <returns>String representation of this thread.</returns>
	public override string ToString()
		=> this.Type switch
		{
			ChannelType.NewsThread => $"News thread {this.Name} ({this.Id})",
			ChannelType.PublicThread => $"Thread {this.Name} ({this.Id})",
			ChannelType.PrivateThread => $"Private thread {this.Name} ({this.Id})",
			_ => $"Thread {this.Name} ({this.Id})",
		};
	#endregion
}
