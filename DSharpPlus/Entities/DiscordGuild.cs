﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Net.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSharpPlus.Entities
{
    /// <summary>
    /// Represents a Discord guild.
    /// </summary>
    public class DiscordGuild : SnowflakeObject, IEquatable<DiscordGuild>
    {
        /// <summary>
        /// Gets the guild's name.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the guild icon's hash.
        /// </summary>
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconHash { get; internal set; }

        /// <summary>
        /// Gets the guild icon's url.
        /// </summary>
        [JsonIgnore]
        public string IconUrl => !string.IsNullOrWhiteSpace(this.IconHash) ? $"https://cdn.discordapp.com/icons/{this.Id.ToString(CultureInfo.InvariantCulture)}/{IconHash}.jpg" : null;

        /// <summary>
        /// Gets the guild splash's hash.
        /// </summary>
        [JsonProperty("splash", NullValueHandling = NullValueHandling.Ignore)]
        public string SplashHash { get; internal set; }

        /// <summary>
        /// Gets the guild splash's url.
        /// </summary>
        [JsonIgnore]
        public string SplashUrl => !string.IsNullOrWhiteSpace(this.SplashHash) ? $"https://cdn.discordapp.com/splashes/{this.Id.ToString(CultureInfo.InvariantCulture)}/{SplashHash}.jpg" : null;

        /// <summary>
        /// Gets the ID of the guild's owner.
        /// </summary>
        [JsonProperty("owner_id", NullValueHandling = NullValueHandling.Ignore)]
        internal ulong OwnerId { get; set; }

        /// <summary>
        /// Gets the guild's owner.
        /// </summary>
        [JsonIgnore]
        public DiscordMember Owner => this.Members.FirstOrDefault(xm => xm.Id == this.OwnerId) ?? this.Discord.ApiClient.GetGuildMemberAsync(this.Id, this.OwnerId).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the guild's voice region ID.
        /// </summary>
        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string RegionId { get; internal set; }

        /// <summary>
        /// Gets the guild's AFK voice channel ID.
        /// </summary>
        [JsonProperty("afk_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        internal ulong AfkChannelId { get; set; } = 0;

        /// <summary>
        /// Gets the guild's AFK voice channel.
        /// </summary>
        [JsonIgnore]
        public DiscordChannel AfkChannel => this.Channels.FirstOrDefault(xc => xc.Id == this.AfkChannelId);

        /// <summary>
        /// Gets the guild's AFK timeout.
        /// </summary>
        [JsonProperty("afk_timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int AfkTimeout { get; internal set; }

        /// <summary>
        /// Gets whether this guild has the guild embed enabled.
        /// </summary>
        [JsonProperty("embed_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool EmbedEnabled { get; internal set; }

        /// <summary>
        /// Gets the ID of the channel from the guild's embed.
        /// </summary>
        [JsonProperty("embed_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        internal ulong EmbedChannelId { get; set; }

        /// <summary>
        /// Gets the channel from the guild's embed.
        /// </summary>
        [JsonIgnore]
        public DiscordChannel EmbedChannel => this.Channels.FirstOrDefault(xc => xc.Id == this.EmbedChannelId);

        /// <summary>
        /// Gets the guild's verification level.
        /// </summary>
        [JsonProperty("verification_level", NullValueHandling = NullValueHandling.Ignore)]
        public VerificationLevel VerificationLevel { get; internal set; }

        /// <summary>
        /// Gets the guild's default notification settings.
        /// </summary>
        [JsonProperty("default_message_notifications", NullValueHandling = NullValueHandling.Ignore)]
        public DefaultMessageNotifications DefaultMessageNotifications { get; internal set; }

        /// <summary>
        /// Gets a collection of this guild's roles.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordRole> Roles => this._roles_lazy.Value;
        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordRole> _roles;
        [JsonIgnore]
        private Lazy<IReadOnlyList<DiscordRole>> _roles_lazy;

        /// <summary>
        /// Gets a collection of this guild's emojis.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordEmoji> Emojis => this._emojis_lazy.Value;
        [JsonProperty("emojis", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordEmoji> _emojis;
        [JsonIgnore]
        private Lazy<IReadOnlyList<DiscordEmoji>> _emojis_lazy;

        /// <summary>
        /// Gets a collection of this guild's features.
        /// </summary>
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string> Features { get; internal set; }

        /// <summary>
        /// Gets the required multi-factor authentication level for this guild.
        /// </summary>
        [JsonProperty("mfa_level", NullValueHandling = NullValueHandling.Ignore)]
        public MfaLevel MfaLevel { get; internal set; }

        /// <summary>
        /// Gets this guild's join date.
        /// </summary>
        [JsonProperty("joined_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset JoinedAt { get; internal set; }

        /// <summary>
        /// Gets whether this guild is considered to be a large guild.
        /// </summary>
        [JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsLarge { get; internal set; }

        /// <summary>
        /// Gets whether this guild is unavailable.
        /// </summary>
        [JsonProperty("unavailable", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUnavailable { get; internal set; }

        /// <summary>
        /// Gets the total number of members in this guild.
        /// </summary>
        [JsonProperty("member_count", NullValueHandling = NullValueHandling.Ignore)]
        public int MemberCount { get; internal set; }

        /// <summary>
        /// Gets a collection of all the voice states for this guilds.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordVoiceState> VoiceStates => this._voice_states_lazy.Value;
        [JsonProperty("voice_states", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordVoiceState> _voice_states;
        [JsonIgnore]
        private Lazy<IReadOnlyList<DiscordVoiceState>> _voice_states_lazy;

        /// <summary>
        /// Gets a collection of all the members that belong to this guild.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordMember> Members => this._members_lazy.Value;
        [JsonProperty("members", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordMember> _members;
        [JsonIgnore]
        private Lazy<IReadOnlyList<DiscordMember>> _members_lazy;

        /// <summary>
        /// Gets a collection of all the channels associated with this guild.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordChannel> Channels => this._channels_lazy.Value;
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordChannel> _channels;
        [JsonIgnore]
        private Lazy<IReadOnlyList<DiscordChannel>> _channels_lazy;

        /// <summary>
        /// Gets the guild member for current user.
        /// </summary>
        [JsonIgnore]
        public DiscordMember CurrentMember => this._current_member_lazy.Value;
        [JsonIgnore]
        private Lazy<DiscordMember> _current_member_lazy;

        /// <summary>
        /// Gets the @everyone role for this guild.
        /// </summary>
        [JsonIgnore]
        public DiscordRole EveryoneRole => this._roles.FirstOrDefault(xr => xr.Id == this.Id);

        /// <summary>
        /// Gets whether the current user is the guild's owner.
        /// </summary>
        [JsonProperty("is_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOwner => this.OwnerId == this.Discord.CurrentUser.Id;

        // /// <summary>
        // /// Gets channels ordered in a manner in which they'd be ordered in the UI of the discord client.
        // /// </summary>
        // [JsonIgnore]
        // public IEnumerable<DiscordChannel> OrderedChannels =>
        //    this._channels.OrderBy(xc => xc.Parent?.Position).ThenBy(xc => xc.Type).ThenBy(xc => xc.Position);

        [JsonIgnore]
        internal bool IsSynced { get; set; }

        internal DiscordGuild()
        {
            this._roles_lazy = new Lazy<IReadOnlyList<DiscordRole>>(() => new ReadOnlyCollection<DiscordRole>(this._roles));
            this._emojis_lazy = new Lazy<IReadOnlyList<DiscordEmoji>>(() => new ReadOnlyCollection<DiscordEmoji>(this._emojis));
            this._voice_states_lazy = new Lazy<IReadOnlyList<DiscordVoiceState>>(() => new ReadOnlyCollection<DiscordVoiceState>(this._voice_states));
            this._channels_lazy = new Lazy<IReadOnlyList<DiscordChannel>>(() => new ReadOnlyCollection<DiscordChannel>(this._channels));
            this._members_lazy = new Lazy<IReadOnlyList<DiscordMember>>(() => new ReadOnlyCollection<DiscordMember>(this._members));

            this._current_member_lazy = new Lazy<DiscordMember>(() => this._members.FirstOrDefault(xm => xm.Id == this.Discord.CurrentUser.Id));
        }

        #region Guild Methods
        /// <summary>
        /// Deletes this guild. Requires the caller to be the owner of the guild.
        /// </summary>
        /// <returns></returns>
        public Task DeleteAsync() =>
            this.Discord.ApiClient.DeleteGuildAsync(this.Id);

        /// <summary>
        /// Modifies this guild.
        /// </summary>
        /// <param name="name">New name.</param>
        /// <param name="region">New voice region.</param>
        /// <param name="icon">New icon.</param>
        /// <param name="verification_level">New verification level.</param>
        /// <param name="default_message_notifications">New default notification settings.</param>
        /// <param name="afk_channel">New voice AFK channel.</param>
        /// <param name="afk_timeout">New timeout after users are going to be moved to the voice AFK channel in seconds.</param>
        /// <param name="owner">New owner. This can only be changed by current owner.</param>
        /// <param name="splash">New invite splash.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns>The modified guild object.</returns>
        public async Task<DiscordGuild> ModifyAsync(string name = null, string region = null, Stream icon = null, VerificationLevel? verification_level = null,
            DefaultMessageNotifications? default_message_notifications = null, DiscordChannel afk_channel = null, int? afk_timeout = null, DiscordMember owner = null, Stream splash = null,
            string reason = null)
        {
            if (afk_channel != null && afk_channel.Type != ChannelType.Voice)
                throw new ArgumentException("AFK channel needs to be a voice channel.");

            string iconb64 = null;
            if (icon != null)
                using (var imgtool = new ImageTool(icon))
                    iconb64 = imgtool.GetBase64();

            string splashb64 = null;
            if (splash != null)
                using (var imgtool = new ImageTool(splash))
                    splashb64 = imgtool.GetBase64();

            return await this.Discord.ApiClient.ModifyGuildAsync(this.Id, name, region, verification_level, default_message_notifications, afk_channel?.Id, afk_timeout, iconb64, owner?.Id, splashb64, reason);
        }

        /// <summary>
        /// Bans a specified member from this guild.
        /// </summary>
        /// <param name="member">Member to ban.</param>
        /// <param name="delete_message_days">How many days to remove messages from.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task BanMemberAsync(DiscordMember member, int delete_message_days = 0, string reason = null) =>
            this.Discord.ApiClient.CreateGuildBanAsync(this.Id, member.Id, delete_message_days, reason);

        /// <summary>
        /// Bans a specified user by ID. This doesn't require the user to be in this guild.
        /// </summary>
        /// <param name="user_id">ID of the user to ban.</param>
        /// <param name="delete_message_days">How many days to remove messages from.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task BanMemberAsync(ulong user_id, int delete_message_days = 0, string reason = null) =>
            this.Discord.ApiClient.CreateGuildBanAsync(this.Id, user_id, delete_message_days, reason);

        /// <summary>
        /// Unbans a user from this guild.
        /// </summary>
        /// <param name="user">User to unban.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task UnbanMemberAsync(DiscordUser user, string reason = null) =>
            this.Discord.ApiClient.RemoveGuildBanAsync(this.Id, user.Id, reason);

        /// <summary>
        /// Unbans a user by ID.
        /// </summary>
        /// <param name="user_id">ID of the user to unban.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task UnbanMemberAsync(ulong user_id, string reason = null) =>
            this.Discord.ApiClient.RemoveGuildBanAsync(this.Id, user_id, reason);

        /// <summary>
        /// Leaves this guild.
        /// </summary>
        /// <returns></returns>
        public Task LeaveAsync() =>
            this.Discord.ApiClient.LeaveGuildAsync(Id);

        /// <summary>
        /// Gets the bans for this guild.
        /// </summary>
        /// <returns>Collection of bans in this guild.</returns>
        public Task<IReadOnlyList<DiscordBan>> GetBansAsync() =>
            this.Discord.ApiClient.GetGuildBansAsync(Id);

        /// <summary>
        /// Creates a new channel in this guild.
        /// </summary>
        /// <param name="name">Name of the new channel.</param>
        /// <param name="type">Type of the new channel.</param>
        /// <param name="parent">Category to put this channel in.</param>
        /// <param name="bitrate">Bitrate of the channel. Applies to voice only.</param>
        /// <param name="user_limit">Maximum number of users in the channel. Applies to voice only.</param>
        /// <param name="overwrites">Permission overwrites for this channel.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns>The newly-created channel.</returns>
        public Task<DiscordChannel> CreateChannelAsync(string name, ChannelType type, DiscordChannel parent = null, int? bitrate = null, int? user_limit = null, IEnumerable<DiscordOverwrite> overwrites = null, string reason = null)
        {
            if (type != ChannelType.Text && type != ChannelType.Voice && type != ChannelType.Category)
                throw new ArgumentException("Channel type must be text, voice, or category.", nameof(type));

            return this.Discord.ApiClient.CreateGuildChannelAsync(this.Id, name, type, parent?.Id, bitrate, user_limit, overwrites, reason);
        }

        /// <summary>
        /// Estimates the number of users to be pruned.
        /// </summary>
        /// <param name="days">Minimum number of inactivity days required for users to be pruned.</param>
        /// <returns>Number of users that will be pruned.</returns>
        public Task<int> GetPruneCountAsync(int days) =>
            this.Discord.ApiClient.GetGuildPruneCountAsync(this.Id, days);

        /// <summary>
        /// Prunes inactive users from this guild.
        /// </summary>
        /// <param name="days">Minimum number of inactivity days required for users to be pruned.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns>Number of users pruned.</returns>
        public Task<int> PruneAsync(int days, string reason = null) =>
            this.Discord.ApiClient.BeginGuildPruneAsync(this.Id, days, reason);

        /// <summary>
        /// Gets integrations attached to this guild.
        /// </summary>
        /// <returns>Collection of integrations attached to this guild.</returns>
        public Task<IReadOnlyList<DiscordIntegration>> GetIntegrationsAsync() =>
            this.Discord.ApiClient.GetGuildIntegrationsAsync(this.Id);

        /// <summary>
        /// Attaches an integration from current user to this guild.
        /// </summary>
        /// <param name="integration">Integration to attach.</param>
        /// <returns>The integration after being attached to the guild.</returns>
        public Task<DiscordIntegration> AttachUserIntegrationAsync(DiscordIntegration integration) =>
            this.Discord.ApiClient.CreateGuildIntegrationAsync(Id, integration.Type, integration.Id);

        /// <summary>
        /// Modifies an integration in this guild.
        /// </summary>
        /// <param name="integration">Integration to modify.</param>
        /// <param name="expire_behaviour">Number of days after which the integration expires.</param>
        /// <param name="expire_grace_period">Length of grace period which allows for renewing the integration.</param>
        /// <param name="enable_emoticons">Whether emotes should be synced from this integration.</param>
        /// <returns>The modified integration.</returns>
        public Task<DiscordIntegration> ModifyIntegrationAsync(DiscordIntegration integration, int expire_behaviour, int expire_grace_period, bool enable_emoticons) =>
            this.Discord.ApiClient.ModifyGuildIntegrationAsync(Id, integration.Id, expire_behaviour, expire_grace_period, enable_emoticons);

        /// <summary>
        /// Removes an integration from this guild.
        /// </summary>
        /// <param name="integration">Integration to remove.</param>
        /// <returns></returns>
        public Task DeleteIntegrationAsync(DiscordIntegration integration) =>
            this.Discord.ApiClient.DeleteGuildIntegrationAsync(Id, integration);

        /// <summary>
        /// Forces re-synchronization of an integration for this guild.
        /// </summary>
        /// <param name="integration">Integration to synchronize.</param>
        /// <returns></returns>
        public Task SyncIntegrationAsync(DiscordIntegration integration) =>
            this.Discord.ApiClient.SyncGuildIntegrationAsync(Id, integration.Id);

        /// <summary>
        /// Gets the guild widget.
        /// </summary>
        /// <returns>This guild's widget.</returns>
        public Task<DiscordGuildEmbed> GetEmbedAsync() =>
            this.Discord.ApiClient.GetGuildEmbedAsync(Id);

        /// <summary>
        /// Gets the voice regions for this guild.
        /// </summary>
        /// <returns>Voice regions available for this guild.</returns>
        public Task<IReadOnlyList<DiscordVoiceRegion>> GetVoiceRegionsAsync() =>
            this.Discord.ApiClient.GetGuildVoiceRegionsAsync(this.Id);

        /// <summary>
        /// Gets all the invites created for all the channels in this guild.
        /// </summary>
        /// <returns>A collection of invites.</returns>
        public Task<IReadOnlyList<DiscordInvite>> GetInvitesAsync() =>
            this.Discord.ApiClient.GetGuildInvitesAsync(this.Id);

        /// <summary>
        /// Gets all the webhooks created for all the channels in this guild.
        /// </summary>
        /// <returns>A collection of webhooks this guild has.</returns>
        public Task<IReadOnlyList<DiscordWebhook>> GetWebhooksAsync() =>
            this.Discord.ApiClient.GetGuildWebhooksAsync(this.Id);

        /// <summary>
        /// Kicks a member from this guild.
        /// </summary>
        /// <param name="member">Member to kick.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task RemoveMemberAsync(DiscordMember member, string reason = null) =>
            this.Discord.ApiClient.RemoveGuildMemberAsync(this.Id, member.Id, reason);

        /// <summary>
        /// Gets a member of this guild by his user ID.
        /// </summary>
        /// <param name="user_id">ID of the member to get.</param>
        /// <returns>The requested member.</returns>
        public async Task<DiscordMember> GetMemberAsync(ulong user_id)
        {
            var mbr = this._members.FirstOrDefault(xm => xm.Id == user_id);
            if (mbr != null)
                return mbr;

            mbr = await this.Discord.ApiClient.GetGuildMemberAsync(Id, user_id);
            this._members.Add(mbr);
            return mbr;
        }

        /// <summary>
        /// Requests a full list of members from Discord.
        /// </summary>
        /// <returns>A collection of all members in this guild.</returns>
        public async Task<IReadOnlyList<DiscordMember>> GetAllMembersAsync()
        {
            var recmbr = new List<DiscordMember>(this.MemberCount + 1);

            var recd = 1000;
            var last = 0ul;
            while (recd == 1000)
            {
                var mbrs = await this.Discord.ApiClient.ListGuildMembersAsync(this.Id, 1000, last == 0 ? null : (ulong?)last);
                recd = mbrs.Count;

                var mbr = mbrs.LastOrDefault();
                if (mbr != null)
                    last = mbr.Id;
                else
                    last = 0;

                recmbr.AddRange(mbrs);
            }

            var recids = recmbr.Select(xm => xm.Id);

            // clear the cache of users who weren't received
            for (int i = 0; i < this._members.Count; i++)
                if (!recids.Contains(this._members[i].Id))
                    this._members.RemoveAt(i--);

            var curids = this._members.Select(xm => xm.Id);

            // ignore members who already exist in the cache
            var newmbr = recmbr.Where(xm => !curids.Contains(xm.Id))
                .Select(xm => { xm.Discord = this.Discord; xm._guild_id = this.Id; return xm; });

            // add new members
            this._members.AddRange(newmbr);
            this.MemberCount = this._members.Count;

            return this.Members;
        }

        /// <summary>
        /// Gets all the channels this guild has.
        /// </summary>
        /// <returns>A collection of this guild's channels.</returns>
        public Task<IReadOnlyList<DiscordChannel>> GetChannelsAsync() =>
            this.Discord.ApiClient.GetGuildChannelsAsync(this.Id);

        /// <summary>
        /// Requests a chunk of members from Discord.
        /// </summary>
        /// <param name="limit">Maximum number of members to return.</param>
        /// <param name="after">ID of the last member in the previous chunk.</param>
        /// <returns>A collection of members in this chunk.</returns>
        public Task<IReadOnlyList<DiscordMember>> ListMembersAsync(int? limit = null, ulong? after = null) =>
            this.Discord.ApiClient.ListGuildMembersAsync(Id, limit, after);

        /// <summary>
        /// Modifies a role in this guild.
        /// </summary>
        /// <param name="role">Role to modify.</param>
        /// <param name="name">New name.</param>
        /// <param name="permissions">New permissions.</param>
        /// <param name="color">New color.</param>
        /// <param name="hoist">Whether the role is to be hoisted.</param>
        /// <param name="mentionable">Whether the role is to be mentionable.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task UpdateRoleAsync(DiscordRole role, string name = null, Permissions? permissions = null, DiscordColor? color = null, bool? hoist = null, bool? mentionable = null, string reason = null) =>
            this.Discord.ApiClient.ModifyGuildRoleAsync(Id, role.Id, name, permissions, color?.Value, hoist, mentionable, reason);

        /// <summary>
        /// Modifies position of the given role in the role hierarchy.
        /// </summary>
        /// <param name="role">Role to modify.</param>
        /// <param name="position">New position.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task UpdateRolePositionAsync(DiscordRole role, int position, string reason = null)
        {
            var roles = this._roles.Where(xr => xr.Id != this.Id).OrderByDescending(xr => xr.Position).ToArray();
            var pmds = new RestGuildRoleReorderPayload[roles.Length];
            for (var i = 0; i < roles.Length; i++)
            {
                pmds[i] = new RestGuildRoleReorderPayload { RoleId = roles[i].Id };

                if (roles[i].Id == role.Id)
                    pmds[i].Position = position;
                else
                    pmds[i].Position = roles[i].Position <= position ? roles[i].Position - 1 : roles[i].Position;
            }

            return this.Discord.ApiClient.ModifyGuildRolePosition(this.Id, pmds, reason);
        }

        /// <summary>
        /// Creates a new role in this guild.
        /// </summary>
        /// <param name="name">Name of the role.</param>
        /// <param name="permissions">Permissions for the role.</param>
        /// <param name="color">Color for the role.</param>
        /// <param name="hoist">Whether the role is to be hoisted.</param>
        /// <param name="mentionable">Whether the role is to be mentionable.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns>The newly-created role.</returns>
        public Task<DiscordRole> CreateRoleAsync(string name = null, Permissions? permissions = null, DiscordColor? color = null, bool? hoist = null, bool? mentionable = null, string reason = null) =>
            this.Discord.ApiClient.CreateGuildRole(this.Id, name, permissions, color?.Value, hoist, mentionable, reason);

        /// <summary>
        /// Deletes a role from this guild.
        /// </summary>
        /// <param name="role">Role to delete.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task DeleteRoleAsync(DiscordRole role, string reason = null) =>
            this.Discord.ApiClient.DeleteRoleAsync(this.Id, role.Id, reason);

        /// <summary>
        /// Gets a role from this guild by its ID.
        /// </summary>
        /// <param name="id">ID of the role to get.</param>
        /// <returns>Requested role.</returns>
        public DiscordRole GetRole(ulong id) =>
            this._roles.FirstOrDefault(xr => xr.Id == id);

        /// <summary>
        /// Gets a channel from this guild by its ID.
        /// </summary>
        /// <param name="id">ID of the channel to get.</param>
        /// <returns>Requested channel.</returns>
        public DiscordChannel GetChannel(ulong id) =>
            this._channels.FirstOrDefault(xc => xc.Id == id);

        /// <summary>
        /// Grants a role to a member. 
        /// </summary>
        /// <param name="member">Member to grant the role to.</param>
        /// <param name="role">Role to grant.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task GrantRoleAsync(DiscordMember member, DiscordRole role, string reason = null) =>
            this.Discord.ApiClient.AddGuildMemberRoleAsync(this.Id, member.Id, role.Id, reason);

        /// <summary>
        /// Revokes a role from a member. 
        /// </summary>
        /// <param name="member">Member to revoke the role from.</param>
        /// <param name="role">Role to revoke.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns></returns>
        public Task RevokeRoleAsync(DiscordMember member, DiscordRole role, string reason) =>
            this.Discord.ApiClient.RemoveGuildMemberRoleAsync(this.Id, member.Id, role.Id, reason);

        /// <summary>
        /// Gets audit log entries for this guild.
        /// </summary>
        /// <param name="limit">Maximum number of entries to fetch.</param>
        /// <param name="by_member">Filter by member responsible.</param>
        /// <param name="action_type">Filter by action type.</param>
        /// <returns>A collection of requested audit log entries.</returns>
        public async Task<IReadOnlyList<DiscordAuditLogEntry>> GetAuditLogsAsync(int? limit = null, DiscordMember by_member = null, AuditLogActionType? action_type = null)
        {
            var gms = this._members.ToDictionary(xm => xm.Id, xm => xm);

            var alrs = new List<AuditLog>();
            int ac = 1, tc = 0, rmn = 100;
            ulong last = 0;
            while (ac > 0)
            {
                rmn = limit != null ? limit.Value - tc : 100;
                rmn = Math.Min(100, rmn);
                if (rmn <= 0) break;

                var alr = await this.Discord.ApiClient.GetAuditLogsAsync(this.Id, rmn, null, last == 0 ? null : (ulong?)last, by_member?.Id, (int?)action_type);
                ac = alr.Entries.Count();
                tc += ac;
                if (ac > 0)
                {
                    last = alr.Entries.Last().Id;
                    alrs.Add(alr);
                }
            }

            var amr = alrs.SelectMany(xa => xa.Users)
                .GroupBy(xu => xu.Id)
                .Select(xgu => xgu.First());

            var ahr = alrs.SelectMany(xa => xa.Webhooks)
                .GroupBy(xh => xh.Id)
                .Select(xgh => xgh.First());

            var ams = amr.Select(xau => gms.ContainsKey(xau.Id) ? gms[xau.Id] : new DiscordMember { Discord = this.Discord, Username = xau.Username, Discriminator = xau.Discriminator, Id = xau.Id, _guild_id = this.Id });
            var amd = ams.ToDictionary(xm => xm.Id, xm => xm);

            Dictionary<ulong, DiscordWebhook> ahd = null;
            if (ahr.Any())
            {
                var whr = await this.GetWebhooksAsync();
                var whs = whr.ToDictionary(xh => xh.Id, xh => xh);

                var amh = ahr.Select(xah => whs.ContainsKey(xah.Id) ? whs[xah.Id] : new DiscordWebhook { Discord = this.Discord, Name = xah.Name, Id = xah.Id, AvatarHash = xah.AvatarHash, ChannelId = xah.ChannelId, GuildId = xah.GuildId, Token = xah.Token });
                ahd = amh.ToDictionary(xh => xh.Id, xh => xh);
            }

            var acs = alrs.SelectMany(xa => xa.Entries).OrderByDescending(xa => xa.Id);
            var entries = new List<DiscordAuditLogEntry>();
            foreach (var xac in acs)
            {
                DiscordAuditLogEntry entry = null;
                ulong t1, t2;
                int t3, t4;
                bool p1, p2;
                switch (xac.ActionType)
                {
                    case AuditLogActionType.GuildUpdate:
                        entry = new DiscordAuditLogGuildEntry
                        {
                            Target = this
                        };

                        var entrygld = entry as DiscordAuditLogGuildEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "name":
                                    entrygld.NameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "owner_id":
                                    entrygld.OwnerChange = new PropertyChange<DiscordMember>
                                    {
                                        Before = gms.ContainsKey(xc.OldValueUlong) ? gms[xc.OldValueUlong] : await this.GetMemberAsync(xc.OldValueUlong),
                                        After = gms.ContainsKey(xc.NewValueUlong) ? gms[xc.NewValueUlong] : await this.GetMemberAsync(xc.NewValueUlong)
                                    };
                                    break;

                                case "icon_hash":
                                    entrygld.IconChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString != null ? $"https://cdn.discordapp.com/icons/{this.Id}/{xc.OldValueString}.webp" : null,
                                        After = xc.OldValueString != null ? $"https://cdn.discordapp.com/icons/{this.Id}/{xc.NewValueString}.webp" : null
                                    };
                                    break;

                                case "verification_level":
                                    entrygld.VerificationLevelChange = new PropertyChange<VerificationLevel>
                                    {
                                        Before = (VerificationLevel)(long)xc.OldValue,
                                        After = (VerificationLevel)(long)xc.NewValue
                                    };
                                    break;

                                case "afk_channel_id":
                                    ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entrygld.AfkChannelChange = new PropertyChange<DiscordChannel>
                                    {
                                        Before = this._channels.FirstOrDefault(xch => xch.Id == t1),
                                        After = this._channels.FirstOrDefault(xch => xch.Id == t2)
                                    };
                                    break;

                                case "widget_channel_id":
                                    ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entrygld.EmbedChannelChange = new PropertyChange<DiscordChannel>
                                    {
                                        Before = this._channels.FirstOrDefault(xch => xch.Id == t1),
                                        After = this._channels.FirstOrDefault(xch => xch.Id == t2)
                                    };
                                    break;

                                case "splash_hash":
                                    entrygld.SplashChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString != null ? $"https://cdn.discordapp.com/splashes/{this.Id}/{xc.OldValueString}.webp?size=2048" : null,
                                        After = xc.NewValueString != null ? $"https://cdn.discordapp.com/splashes/{this.Id}/{xc.NewValueString}.webp?size=2048" : null
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in guild update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelDelete:
                    case AuditLogActionType.ChannelUpdate:
                        entry = new DiscordAuditLogChannelEntry
                        {
                            Target = this._channels.FirstOrDefault(xch => xch.Id == xac.TargetId.Value)
                        };

                        var entrychn = entry as DiscordAuditLogChannelEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "name":
                                    entrychn.NameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValue != null ? xc.OldValueString : null,
                                        After = xc.NewValue != null ? xc.NewValueString : null
                                    };
                                    break;

                                case "type":
                                    p1 = ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entrychn.TypeChange = new PropertyChange<ChannelType?>
                                    {
                                        Before = p1 ? (ChannelType?)t1 : null,
                                        After = p2 ? (ChannelType?)t2 : null
                                    };
                                    break;

                                case "permission_overwrites":
                                    var olds = xc.OldValues?.OfType<JObject>()
                                        ?.Select(xjo => xjo.ToObject<DiscordOverwrite>())
                                        ?.Select(xo => { xo.Discord = this.Discord; return xo; });

                                    var news = xc.NewValues?.OfType<JObject>()
                                        ?.Select(xjo => xjo.ToObject<DiscordOverwrite>())
                                        ?.Select(xo => { xo.Discord = this.Discord; return xo; });

                                    entrychn.OverwriteChange = new PropertyChange<IReadOnlyList<DiscordOverwrite>>
                                    {
                                        Before = olds != null ? new ReadOnlyCollection<DiscordOverwrite>(new List<DiscordOverwrite>(olds)) : null,
                                        After = news != null ? new ReadOnlyCollection<DiscordOverwrite>(new List<DiscordOverwrite>(news)) : null
                                    };
                                    break;

                                case "topic":
                                    entrychn.TopicChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in channel update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.OverwriteCreate:
                    case AuditLogActionType.OverwriteDelete:
                    case AuditLogActionType.OverwriteUpdate:
                        entry = new DiscordAuditLogOverwriteEntry
                        {
                            Target = this._channels.FirstOrDefault(xc => xc.Id == xac.TargetId.Value)?.PermissionOverwrites.FirstOrDefault(xo => xo.Id == xac.Options.Id),
                            Channel = this._channels.FirstOrDefault(xc => xc.Id == xac.TargetId.Value)
                        };

                        var entryovr = entry as DiscordAuditLogOverwriteEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "deny":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entryovr.DenyChange = new PropertyChange<Permissions?>
                                    {
                                        Before = p1 ? (Permissions?)t1 : null,
                                        After = p2 ? (Permissions?)t2 : null
                                    };
                                    break;

                                case "allow":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entryovr.AllowChange = new PropertyChange<Permissions?>
                                    {
                                        Before = p1 ? (Permissions?)t1 : null,
                                        After = p2 ? (Permissions?)t2 : null
                                    };
                                    break;

                                case "type":
                                    entryovr.TypeChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "id":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entryovr.TargetIdChange = new PropertyChange<ulong?>
                                    {
                                        Before = p1 ? (ulong?)t1 : null,
                                        After = p2 ? (ulong?)t2 : null
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in overwrite update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.Kick:
                        entry = new DiscordAuditLogKickEntry
                        {
                            Target = amd.ContainsKey(xac.TargetId.Value) ? amd[xac.TargetId.Value] : null
                        };
                        break;

                    case AuditLogActionType.Prune:
                        entry = new DiscordAuditLogPruneEntry
                        {
                            Days = xac.Options.DeleteMemberDays,
                            Toll = xac.Options.MembersRemoved
                        };
                        break;

                    case AuditLogActionType.Ban:
                    case AuditLogActionType.Unban:
                        entry = new DiscordAuditLogBanEntry
                        {
                            Target = amd.ContainsKey(xac.TargetId.Value) ? amd[xac.TargetId.Value] : null
                        };
                        break;

                    case AuditLogActionType.MemberUpdate:
                    case AuditLogActionType.MemberRoleUpdate:
                        entry = new DiscordAuditLogMemberUpdateEntry
                        {
                            Target = amd.ContainsKey(xac.TargetId.Value) ? amd[xac.TargetId.Value] : null
                        };

                        var entrymbu = entry as DiscordAuditLogMemberUpdateEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "nick":
                                    entrymbu.NicknameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "deaf":
                                    entrymbu.DeafenChange = new PropertyChange<bool?>
                                    {
                                        Before = xc.OldValue != null ? (bool?)xc.OldValue : null,
                                        After = xc.NewValue != null ? (bool?)xc.NewValue : null
                                    };
                                    break;

                                case "mute":
                                    entrymbu.MuteChange = new PropertyChange<bool?>
                                    {
                                        Before = xc.OldValue != null ? (bool?)xc.OldValue : null,
                                        After = xc.NewValue != null ? (bool?)xc.NewValue : null
                                    };
                                    break;

                                case "$add":
                                    entrymbu.AddedRoles = new ReadOnlyCollection<DiscordRole>(xc.NewValues.Select(xo => (ulong)xo["id"]).Select(xul => this._roles.FirstOrDefault(xr => xr.Id == xul)).ToList());
                                    break;

                                case "$remove":
                                    entrymbu.RemovedRoles = new ReadOnlyCollection<DiscordRole>(xc.NewValues.Select(xo => (ulong)xo["id"]).Select(xul => this._roles.FirstOrDefault(xr => xr.Id == xul)).ToList());
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in member update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.RoleCreate:
                    case AuditLogActionType.RoleDelete:
                    case AuditLogActionType.RoleUpdate:
                        entry = new DiscordAuditLogRoleUpdateEntry
                        {
                            Target = this._roles.FirstOrDefault(xr => xr.Id == xac.TargetId.Value)
                        };

                        var entryrol = entry as DiscordAuditLogRoleUpdateEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "name":
                                    entryrol.NameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "color":
                                    p1 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t3);
                                    p2 = int.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t4);

                                    entryrol.ColorChange = new PropertyChange<int?>
                                    {
                                        Before = p1 ? (int?)t3 : null,
                                        After = p2 ? (int?)t4 : null
                                    };
                                    break;

                                case "permissions":
                                    entryrol.PermissionChange = new PropertyChange<Permissions?>
                                    {
                                        Before = xc.OldValue != null ? (Permissions?)(long)xc.OldValue : null,
                                        After = xc.NewValue != null ? (Permissions?)(long)xc.NewValue : null
                                    };
                                    break;

                                case "position":
                                    entryrol.PositionChange = new PropertyChange<int?>
                                    {
                                        Before = xc.OldValue != null ? (int?)(long)xc.OldValue : null,
                                        After = xc.NewValue != null ? (int?)(long)xc.NewValue : null,
                                    };
                                    break;

                                case "mentionable":
                                    entryrol.MentionableChange = new PropertyChange<bool?>
                                    {
                                        Before = xc.OldValue != null ? (bool?)xc.OldValue : null,
                                        After = xc.NewValue != null ? (bool?)xc.NewValue : null
                                    };
                                    break;

                                case "hoist":

                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in role update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.InviteDelete:
                    case AuditLogActionType.InviteUpdate:
                        entry = new DiscordAuditLogInviteEntry();
                        var inv = new DiscordInvite
                        {
                            Discord = this.Discord,
                            Guild = new DiscordInviteGuild
                            {
                                Discord = this.Discord,
                                Id = this.Id,
                                Name = this.Name,
                                SplashHash = this.SplashHash
                            }
                        };

                        var entryinv = entry as DiscordAuditLogInviteEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "max_age":
                                    p1 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t3);
                                    p2 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t4);

                                    entryinv.MaxAgeChange = new PropertyChange<int?>
                                    {
                                        Before = p1 ? (int?)t3 : null,
                                        After = p2 ? (int?)t4 : null
                                    };
                                    break;

                                case "code":
                                    inv.Code = xc.OldValueString ?? xc.NewValueString;

                                    entryinv.CodeChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "temporary":
                                    entryinv.TemporaryChange = new PropertyChange<bool?>
                                    {
                                        Before = xc.OldValue != null ? (bool?)xc.OldValue : null,
                                        After = xc.NewValue != null ? (bool?)xc.NewValue : null
                                    };
                                    break;

                                case "inviter_id":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entryinv.InviterChange = new PropertyChange<DiscordMember>
                                    {
                                        Before = amd.ContainsKey(t1) ? amd[t1] : null,
                                        After = amd.ContainsKey(t2) ? amd[t2] : null,
                                    };
                                    break;

                                case "channel_id":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entryinv.ChannelChange = new PropertyChange<DiscordChannel>
                                    {
                                        Before = p1 ? this._channels.FirstOrDefault(xch => xch.Id == t1) : null,
                                        After = p2 ? this._channels.FirstOrDefault(xch => xch.Id == t2) : null
                                    };

                                    var ch = entryinv.ChannelChange.Before ?? entryinv.ChannelChange.After;
                                    var cht = ch?.Type;
                                    inv.Channel = new DiscordInviteChannel
                                    {
                                        Discord = this.Discord,
                                        Id = p1 ? t1 : t2,
                                        Name = ch?.Name,
                                        Type = cht != null ? cht.Value : ChannelType.Unknown
                                    };
                                    break;

                                case "uses":
                                    p1 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t3);
                                    p2 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t4);

                                    entryinv.UsesChange = new PropertyChange<int?>
                                    {
                                        Before = p1 ? (int?)t3 : null,
                                        After = p2 ? (int?)t4 : null
                                    };
                                    break;

                                case "max_uses":
                                    p1 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t3);
                                    p2 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t4);

                                    entryinv.MaxUsesChange = new PropertyChange<int?>
                                    {
                                        Before = p1 ? (int?)t3 : null,
                                        After = p2 ? (int?)t4 : null
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in invite update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }

                        entryinv.Target = inv;
                        break;

                    case AuditLogActionType.WebhookCreate:
                    case AuditLogActionType.WebhookDelete:
                    case AuditLogActionType.WebhookUpdate:
                        entry = new DiscordAuditLogWebhookEntry
                        {
                            Target = ahd.ContainsKey(xac.TargetId.Value) ? ahd[xac.TargetId.Value] : null
                        };

                        var entrywhk = entry as DiscordAuditLogWebhookEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "name":
                                    entrywhk.NameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                case "channel_id":
                                    p1 = ulong.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t1);
                                    p2 = ulong.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t2);

                                    entrywhk.ChannelChange = new PropertyChange<DiscordChannel>
                                    {
                                        Before = p1 ? this._channels.FirstOrDefault(xch => xch.Id == t1) : null,
                                        After = p2 ? this._channels.FirstOrDefault(xch => xch.Id == t2) : null
                                    };
                                    break;

                                case "type": // ???
                                    p1 = int.TryParse(xc.OldValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t3);
                                    p2 = int.TryParse(xc.NewValue as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out t4);

                                    entrywhk.TypeChange = new PropertyChange<int?>
                                    {
                                        Before = p1 ? (int?)t3 : null,
                                        After = p2 ? (int?)t4 : null
                                    };
                                    break;

                                case "avatar_hash":
                                    entrywhk.AvatarChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString != null ? $"https://cdn.discordapp.com/splashes/{entrywhk.Target.Id.ToString(CultureInfo.InvariantCulture)}/{xc.OldValueString}.png?size=1024" : null,
                                        After = xc.NewValueString != null ? $"https://cdn.discordapp.com/splashes/{entrywhk.Target.Id.ToString(CultureInfo.InvariantCulture)}/{xc.NewValueString}.png?size=1024" : null
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in webhook update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.EmojiCreate:
                    case AuditLogActionType.EmojiDelete:
                    case AuditLogActionType.EmojiUpdate:
                        entry = new DiscordAuditLogEmojiEntry
                        {
                            Target = this._emojis.FirstOrDefault(xe => xe.Id == xac.TargetId.Value)
                        };

                        var entryemo = entry as DiscordAuditLogEmojiEntry;
                        foreach (var xc in xac.Changes)
                        {
                            switch (xc.Key.ToLowerInvariant())
                            {
                                case "name":
                                    entryemo.NameChange = new PropertyChange<string>
                                    {
                                        Before = xc.OldValueString,
                                        After = xc.NewValueString
                                    };
                                    break;

                                default:
                                    this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown key in member update: {xc.Key}; this should be reported to devs", DateTime.Now);
                                    break;
                            }
                        }
                        break;

                    case AuditLogActionType.MessageDelete:
                        entry = new DiscordAuditLogMessageEntry
                        {
                            Channel = this._channels.FirstOrDefault(xc => xc.Id == xac.Options?.Id),
                            MessageCount = xac.Options?.MessageCount
                        };

                        var entrymsg = entry as DiscordAuditLogMessageEntry;

                        if (entrymsg.Channel != null)
                        {
                            DiscordMessage msg = null;
                            if (this.Discord is DiscordClient dc && dc.MessageCache?.TryGet(xm => xm.Id == xac.TargetId.Value && xm.ChannelId == entrymsg.Channel.Id, out msg) == true)
                                entrymsg.Target = msg;
                            else
                                entrymsg.Target = new DiscordMessage { Discord = this.Discord, Id = xac.TargetId.Value };
                        }
                        break;

                    default:
                        this.Discord.DebugLogger.LogMessage(LogLevel.Warning, "DSharpPlus", $"Unknown audit log action type: {((int)xac.ActionType).ToString(CultureInfo.InvariantCulture)}; this should be reported to devs", DateTime.Now);
                        break;
                }

                if (entry == null)
                    continue;

                switch (xac.ActionType)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.EmojiCreate:
                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.OverwriteCreate:
                    case AuditLogActionType.RoleCreate:
                    case AuditLogActionType.WebhookCreate:
                        entry.ActionCategory = AuditLogActionCategory.Create;
                        break;

                    case AuditLogActionType.ChannelDelete:
                    case AuditLogActionType.EmojiDelete:
                    case AuditLogActionType.InviteDelete:
                    case AuditLogActionType.MessageDelete:
                    case AuditLogActionType.OverwriteDelete:
                    case AuditLogActionType.RoleDelete:
                    case AuditLogActionType.WebhookDelete:
                        entry.ActionCategory = AuditLogActionCategory.Delete;
                        break;

                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.EmojiUpdate:
                    case AuditLogActionType.InviteUpdate:
                    case AuditLogActionType.MemberRoleUpdate:
                    case AuditLogActionType.MemberUpdate:
                    case AuditLogActionType.OverwriteUpdate:
                    case AuditLogActionType.RoleUpdate:
                    case AuditLogActionType.WebhookUpdate:
                        entry.ActionCategory = AuditLogActionCategory.Update;
                        break;

                    default:
                        entry.ActionCategory = AuditLogActionCategory.Other;
                        break;
                }

                entry.Discord = this.Discord;
                entry.ActionType = xac.ActionType;
                entry.Id = xac.Id;
                entry.Reason = xac.Reason;
                entry.UserResponsible = amd[xac.UserId];
                entries.Add(entry);
            }

            return new ReadOnlyCollection<DiscordAuditLogEntry>(entries);
        }

        /// <summary>
        /// Sends a guild sync request for this guild. This fills the guild's member and presence information, and starts dispatching additional events.
        /// 
        /// This can only be done for user tokens.
        /// </summary>
        /// <returns></returns>
        public Task SyncAsync() =>
            this.Discord is DiscordClient dc ? dc.SyncGuildsAsync(this) : Task.Delay(0);

        /// <summary>
        /// Acknowledges all the messages in this guild. This is available to user tokens only.
        /// </summary>
        /// <returns></returns>
        public Task AcknowledgeAsync()
        {
            if (this.Discord.Configuration.TokenType == TokenType.User)
                return this.Discord.ApiClient.AcknowledgeGuildAsync(this.Id);
            throw new InvalidOperationException("ACK can only be used when logged in as regular user.");
        }

        /// <summary>
        /// Gets the default channel for this member.
        /// </summary>
        /// <returns>This member's default channel.</returns>
        public DiscordChannel GetDefaultChannel()
        {
            return this._channels.Where(xc => xc.Type == ChannelType.Text)
                .OrderBy(xc => xc.Position)
                .FirstOrDefault(xc => (xc.PermissionsFor(this.CurrentMember) & Permissions.ReadMessages) == Permissions.ReadMessages);
        }
        #endregion

        /// <summary>
        /// Returns a string representation of this guild.
        /// </summary>
        /// <returns>String representation of this guild.</returns>
        public override string ToString()
        {
            return string.Concat("Guild ", this.Id, "; ", this.Name);
        }

        /// <summary>
        /// Checks whether this <see cref="DiscordGuild"/> is equal to another object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Whether the object is equal to this <see cref="DiscordGuild"/>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DiscordGuild);
        }

        /// <summary>
        /// Checks whether this <see cref="DiscordGuild"/> is equal to another <see cref="DiscordGuild"/>.
        /// </summary>
        /// <param name="e"><see cref="DiscordGuild"/> to compare to.</param>
        /// <returns>Whether the <see cref="DiscordGuild"/> is equal to this <see cref="DiscordGuild"/>.</returns>
        public bool Equals(DiscordGuild e)
        {
            if (ReferenceEquals(e, null))
                return false;

            if (ReferenceEquals(this, e))
                return true;

            return this.Id == e.Id;
        }

        /// <summary>
        /// Gets the hash code for this <see cref="DiscordGuild"/>.
        /// </summary>
        /// <returns>The hash code for this <see cref="DiscordGuild"/>.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Gets whether the two <see cref="DiscordGuild"/> objects are equal.
        /// </summary>
        /// <param name="e1">First member to compare.</param>
        /// <param name="e2">Second member to compare.</param>
        /// <returns>Whether the two members are equal.</returns>
        public static bool operator ==(DiscordGuild e1, DiscordGuild e2)
        {
            var o1 = e1 as object;
            var o2 = e2 as object;

            if ((o1 == null && o2 != null) || (o1 != null && o2 == null))
                return false;

            if (o1 == null && o2 == null)
                return true;

            return e1.Id == e2.Id;
        }

        /// <summary>
        /// Gets whether the two <see cref="DiscordGuild"/> objects are not equal.
        /// </summary>
        /// <param name="e1">First member to compare.</param>
        /// <param name="e2">Second member to compare.</param>
        /// <returns>Whether the two members are not equal.</returns>
        public static bool operator !=(DiscordGuild e1, DiscordGuild e2) =>
            !(e1 == e2);
    }

    /// <summary>
    /// Represents guild verification level.
    /// </summary>
    public enum VerificationLevel : int
    {
        /// <summary>
        /// No verification. Anyone can join and chat right away.
        /// </summary>
        None = 0,

        /// <summary>
        /// Low verification level. Users are required to have a verified email attached to their account in order to be able to chat.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium verification level. Users are required to have a verified email attached to their account, and account age need to be at least 5 minutes in order to be able to chat.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// (╯°□°）╯︵ ┻━┻ verification level. Users are required to have a verified email attached to their account, account age need to be at least 5 minutes, and they need to be in the server for at least 10 minutes in order to be able to chat.
        /// </summary>
        High = 3,

        /// <summary>
        /// ┻━┻ ﾐヽ(ಠ益ಠ)ノ彡┻━┻ verification level. Users are required to have a verified phone number attached to their account.
        /// </summary>
        Highest = 4
    }

    /// <summary>
    /// Represents default notification level for a guild.
    /// </summary>
    public enum DefaultMessageNotifications : int
    {
        /// <summary>
        /// All messages will trigger push notifications.
        /// </summary>
        AllMessages = 0,

        /// <summary>
        /// Only messages that mention the user (or a role he's in) will trigger push notifications.
        /// </summary>
        MentionsOnly = 1
    }

    /// <summary>
    /// Represents multi-factor authentication level required by a guild to use administrator functionality.
    /// </summary>
    public enum MfaLevel : int
    {
        /// <summary>
        /// Multi-factor authentication is not required to use administrator functionality.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Multi-factor authentication is required to use administrator functionality.
        /// </summary>
        Enabled = 1
    }
}
