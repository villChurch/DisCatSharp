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
using System.Reflection;
using System.Threading.Tasks;

using ConcurrentCollections;

using DisCatSharp.Common.Utilities;

using Microsoft.Extensions.Logging;

namespace DisCatSharp.Interactivity.EventHandling;

/// <summary>
/// EventWaiter is a class that serves as a layer between the InteractivityExtension
/// and the DiscordClient to listen to an event and check for matches to a predicate.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class EventWaiter<T> : IDisposable where T : AsyncEventArgs
{
	DiscordClient _client;
	AsyncEvent<DiscordClient, T> _event;
	AsyncEventHandler<DiscordClient, T> _handler;
	ConcurrentHashSet<MatchRequest<T>> _matchRequests;
	ConcurrentHashSet<CollectRequest<T>> _collectRequests;
	bool _disposed;

	/// <summary>
	/// Creates a new EventWaiter object.
	/// </summary>
	/// <param name="client">Your DiscordClient</param>
	public EventWaiter(DiscordClient client)
	{
		this._client = client;
		var tinfo = this._client.GetType().GetTypeInfo();
		var handler = tinfo.DeclaredFields.First(x => x.FieldType == typeof(AsyncEvent<DiscordClient, T>));
		this._matchRequests = new ConcurrentHashSet<MatchRequest<T>>();
		this._collectRequests = new ConcurrentHashSet<CollectRequest<T>>();
		this._event = (AsyncEvent<DiscordClient, T>)handler.GetValue(this._client);
		this._handler = new AsyncEventHandler<DiscordClient, T>(this.HandleEvent);
		this._event.Register(this._handler);
	}

	/// <summary>
	/// Waits for a match to a specific request, else returns null.
	/// </summary>
	/// <param name="request">Request to match</param>
	/// <returns></returns>
	public async Task<T> WaitForMatchAsync(MatchRequest<T> request)
	{
		T result = null;
		this._matchRequests.Add(request);
		try
		{
			result = await request.Tcs.Task.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			this._client.Logger.LogError(InteractivityEvents.InteractivityWaitError, ex, "An exception occurred while waiting for {0}", typeof(T).Name);
		}
		finally
		{
			request.Dispose();
			this._matchRequests.TryRemove(request);
		}
		return result;
	}

	/// <summary>
	/// Collects the matches async.
	/// </summary>
	/// <param name="request">The request.</param>
	public async Task<ReadOnlyCollection<T>> CollectMatchesAsync(CollectRequest<T> request)
	{
		ReadOnlyCollection<T> result = null;
		this._collectRequests.Add(request);
		try
		{
			await request.Tcs.Task.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			this._client.Logger.LogError(InteractivityEvents.InteractivityWaitError, ex, "An exception occurred while collecting from {0}", typeof(T).Name);
		}
		finally
		{
			result = new ReadOnlyCollection<T>(new HashSet<T>(request.Collected).ToList());
			request.Dispose();
			this._collectRequests.TryRemove(request);
		}
		return result;
	}

	/// <summary>
	/// Handles the event.
	/// </summary>
	/// <param name="client">The client.</param>
	/// <param name="eventArgs">The event's arguments.</param>
	private Task HandleEvent(DiscordClient client, T eventArgs)
	{
		if (!this._disposed)
		{
			foreach (var req in this._matchRequests)
			{
				if (req.Predicate(eventArgs))
				{
					req.Tcs.TrySetResult(eventArgs);
				}
			}

			foreach (var req in this._collectRequests)
			{
				if (req.Predicate(eventArgs))
				{
					req.Collected.Add(eventArgs);
				}
			}
		}

		return Task.CompletedTask;
	}

	~EventWaiter()
	{
		this.Dispose();
	}

	/// <summary>
	/// Disposes this EventWaiter
	/// </summary>
	public void Dispose()
	{
		this._disposed = true;
		this._event?.Unregister(this._handler);

		this._event = null;
		this._handler = null;
		this._client = null;

		this._matchRequests?.Clear();
		this._collectRequests?.Clear();

		this._matchRequests = null;
		this._collectRequests = null;
		GC.SuppressFinalize(this);
	}
}
