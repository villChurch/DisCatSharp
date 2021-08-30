// This file is part of the DisCatSharp project.
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

using System;
using System.Threading.Tasks;
using DisCatSharp.Entities;

namespace DisCatSharp.CommandsNext.Attributes
{
    /// <summary>
    /// Defines that usage of this command is restricted to boosters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RequireBoostingAttribute : CheckBaseAttribute
    {
        /// <summary>
        /// Gets the required boost time.
        /// </summary>
        public DateTime? Since { get; }

        /// <summary>
        /// Gets the required guild.
        /// </summary>
        public DiscordGuild Guild { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireBoostingAttribute"/> class.
        /// </summary>
        /// <param name="since">Boosting since.</param>
        public RequireBoostingAttribute(DateTime? since = null)
        {
            this.Guild = null;
            this.Since = since;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireBoostingAttribute"/> class.
        /// </summary>
        /// <param name="guild">Target guild.</param>
        /// <param name="since">Boosting since.</param>
        public RequireBoostingAttribute(DiscordGuild guild, DateTime? since = null)
        {
            this.Guild = guild;
            this.Since = since;
        }

        /// <summary>
        /// Executes the a check.
        /// </summary>
        /// <param name="ctx">The command context.</param>
        /// <param name="help">If true, help - returns true.</param>
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (this.Guild != null)
            {
                var member = await this.Guild.GetMemberAsync(ctx.User.Id);
                return member != null && member.PremiumSince.HasValue ? this.Since.HasValue ? await Task.FromResult(member.PremiumSince.Value.DateTime <= this.Since) : await Task.FromResult(true) : await Task.FromResult(false);
            }
            else
            {
                return ctx.Member != null && ctx.Member.PremiumSince.HasValue ? this.Since.HasValue ? await Task.FromResult(ctx.Member.PremiumSince.Value.DateTime <= this.Since) : await Task.FromResult(true) : await Task.FromResult(false);
            }
        }
    }
}
