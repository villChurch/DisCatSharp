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

namespace DisCatSharp.ApplicationCommands.Enums;

/// <summary>
/// Defines how are command cooldowns applied.
/// </summary>
public enum CooldownBucketType : int
{

	/// <summary>
	/// Denotes that the command will have its cooldown applied globally.
	/// </summary>
	Global = 0,

	/// <summary>
	/// Denotes that the command will have its cooldown applied per-user.
	/// </summary>
	User = 1,

	/// <summary>
	/// Denotes that the command will have its cooldown applied per-channel.
	/// </summary>
	Channel = 2,

	/// <summary>
	/// Denotes that the command will have its cooldown applied per-guild. In DMs, this applies the cooldown per-channel.
	/// </summary>
	Guild = 4
}
