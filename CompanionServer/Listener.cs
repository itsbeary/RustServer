using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009EE RID: 2542
	public class Listener : IDisposable, IBroadcastSender<Connection, AppBroadcast>
	{
		// Token: 0x06003CAD RID: 15533 RVA: 0x00163E94 File Offset: 0x00162094
		public Listener(IPAddress ipAddress, int port)
		{
			this.Address = ipAddress;
			this.Port = port;
			this.Limiter = new ConnectionLimiter();
			this._ipTokenBuckets = new TokenBucketList<IPAddress>(50.0, 15.0);
			this._ipBans = new BanList<IPAddress>();
			this._playerTokenBuckets = new TokenBucketList<ulong>(25.0, 3.0);
			this._pairingTokenBuckets = new TokenBucketList<ulong>(5.0, 0.1);
			this._messageQueue = new Queue<Listener.Message>();
			SynchronizationContext syncContext = SynchronizationContext.Current;
			this._server = new WebSocketServer(string.Format("ws://{0}:{1}/", this.Address, this.Port), true);
			this._server.Start(delegate(IWebSocketConnection socket)
			{
				IPAddress address = socket.ConnectionInfo.ClientIpAddress;
				if (!this.Limiter.TryAdd(address) || this._ipBans.IsBanned(address))
				{
					socket.Close();
					return;
				}
				long num = Interlocked.Increment(ref this._nextConnectionId);
				Connection conn = new Connection(num, this, socket);
				socket.OnClose = delegate
				{
					this.Limiter.Remove(address);
					syncContext.Post(delegate(object c)
					{
						((Connection)c).OnClose();
					}, conn);
				};
				socket.OnBinary = new BinaryDataHandler(conn.OnMessage);
				socket.OnError = new Action<Exception>(UnityEngine.Debug.LogError);
			});
			this._stopwatch = new Stopwatch();
			this.PlayerSubscribers = new SubscriberList<PlayerTarget, Connection, AppBroadcast>(this, null);
			this.EntitySubscribers = new SubscriberList<EntityTarget, Connection, AppBroadcast>(this, null);
			this.CameraSubscribers = new SubscriberList<CameraTarget, Connection, AppBroadcast>(this, new double?((double)30));
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x00163FCC File Offset: 0x001621CC
		public void Dispose()
		{
			WebSocketServer server = this._server;
			if (server == null)
			{
				return;
			}
			server.Dispose();
		}

		// Token: 0x06003CAF RID: 15535 RVA: 0x00163FE0 File Offset: 0x001621E0
		internal void Enqueue(Connection connection, MemoryBuffer data)
		{
			Queue<Listener.Message> messageQueue = this._messageQueue;
			lock (messageQueue)
			{
				if (!App.update || this._messageQueue.Count >= App.queuelimit)
				{
					data.Dispose();
				}
				else
				{
					Listener.Message message = new Listener.Message(connection, data);
					this._messageQueue.Enqueue(message);
				}
			}
		}

		// Token: 0x06003CB0 RID: 15536 RVA: 0x00164054 File Offset: 0x00162254
		public void Update()
		{
			if (!App.update)
			{
				return;
			}
			using (TimeWarning.New("CompanionServer.MessageQueue", 0))
			{
				Queue<Listener.Message> messageQueue = this._messageQueue;
				lock (messageQueue)
				{
					this._stopwatch.Restart();
					while (this._messageQueue.Count > 0 && this._stopwatch.Elapsed.TotalMilliseconds < 5.0)
					{
						Listener.Message message = this._messageQueue.Dequeue();
						this.Dispatch(message);
					}
				}
			}
			if (this._lastCleanup >= 3f)
			{
				this._lastCleanup = 0f;
				this._ipTokenBuckets.Cleanup();
				this._ipBans.Cleanup();
				this._playerTokenBuckets.Cleanup();
				this._pairingTokenBuckets.Cleanup();
			}
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x00164154 File Offset: 0x00162354
		private void Dispatch(Listener.Message message)
		{
			Listener.<>c__DisplayClass21_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.message = message;
			using (CS$<>8__locals1.message.Buffer)
			{
				try
				{
					Listener.Stream.SetData(CS$<>8__locals1.message.Buffer.Data, 0, CS$<>8__locals1.message.Buffer.Length);
					CS$<>8__locals1.request = AppRequest.Deserialize(Listener.Stream);
				}
				catch
				{
					DebugEx.LogWarning(string.Format("Malformed companion packet from {0}", CS$<>8__locals1.message.Connection.Address), StackTraceLogType.None);
					CS$<>8__locals1.message.Connection.Close();
					throw;
				}
			}
			CompanionServer.Handlers.IHandler handler;
			if (!this.<Dispatch>g__Handle|21_15<AppEmpty, Info>((AppRequest r) => r.getInfo, out handler, ref CS$<>8__locals1))
			{
				if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CompanionServer.Handlers.Time>((AppRequest r) => r.getTime, out handler, ref CS$<>8__locals1))
				{
					if (!this.<Dispatch>g__Handle|21_15<AppEmpty, Map>((AppRequest r) => r.getMap, out handler, ref CS$<>8__locals1))
					{
						if (!this.<Dispatch>g__Handle|21_15<AppEmpty, TeamInfo>((AppRequest r) => r.getTeamInfo, out handler, ref CS$<>8__locals1))
						{
							if (!this.<Dispatch>g__Handle|21_15<AppEmpty, TeamChat>((AppRequest r) => r.getTeamChat, out handler, ref CS$<>8__locals1))
							{
								if (!this.<Dispatch>g__Handle|21_15<AppSendMessage, SendTeamChat>((AppRequest r) => r.sendTeamMessage, out handler, ref CS$<>8__locals1))
								{
									if (!this.<Dispatch>g__Handle|21_15<AppEmpty, EntityInfo>((AppRequest r) => r.getEntityInfo, out handler, ref CS$<>8__locals1))
									{
										if (!this.<Dispatch>g__Handle|21_15<AppSetEntityValue, SetEntityValue>((AppRequest r) => r.setEntityValue, out handler, ref CS$<>8__locals1))
										{
											if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CheckSubscription>((AppRequest r) => r.checkSubscription, out handler, ref CS$<>8__locals1))
											{
												if (!this.<Dispatch>g__Handle|21_15<AppFlag, SetSubscription>((AppRequest r) => r.setSubscription, out handler, ref CS$<>8__locals1))
												{
													if (!this.<Dispatch>g__Handle|21_15<AppEmpty, MapMarkers>((AppRequest r) => r.getMapMarkers, out handler, ref CS$<>8__locals1))
													{
														if (!this.<Dispatch>g__Handle|21_15<AppPromoteToLeader, PromoteToLeader>((AppRequest r) => r.promoteToLeader, out handler, ref CS$<>8__locals1))
														{
															if (!this.<Dispatch>g__Handle|21_15<AppCameraSubscribe, CameraSubscribe>((AppRequest r) => r.cameraSubscribe, out handler, ref CS$<>8__locals1))
															{
																if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CameraUnsubscribe>((AppRequest r) => r.cameraUnsubscribe, out handler, ref CS$<>8__locals1))
																{
																	if (!this.<Dispatch>g__Handle|21_15<AppCameraInput, CameraInput>((AppRequest r) => r.cameraInput, out handler, ref CS$<>8__locals1))
																	{
																		AppResponse appResponse = Facepunch.Pool.Get<AppResponse>();
																		appResponse.seq = CS$<>8__locals1.request.seq;
																		appResponse.error = Facepunch.Pool.Get<AppError>();
																		appResponse.error.error = "unhandled";
																		CS$<>8__locals1.message.Connection.Send(appResponse);
																		CS$<>8__locals1.request.Dispose();
																		return;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			try
			{
				ValidationResult validationResult = handler.Validate();
				if (validationResult == ValidationResult.Rejected)
				{
					CS$<>8__locals1.message.Connection.Close();
				}
				else if (validationResult != ValidationResult.Success)
				{
					handler.SendError(validationResult.ToErrorCode());
				}
				else
				{
					handler.Execute();
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("AppRequest threw an exception: {0}", ex));
				handler.SendError("server_error");
			}
			Facepunch.Pool.FreeDynamic<CompanionServer.Handlers.IHandler>(ref handler);
		}

		// Token: 0x06003CB2 RID: 15538 RVA: 0x00164584 File Offset: 0x00162784
		public void BroadcastTo(List<Connection> targets, AppBroadcast broadcast)
		{
			MemoryBuffer broadcastBuffer = Listener.GetBroadcastBuffer(broadcast);
			foreach (Connection connection in targets)
			{
				connection.SendRaw(broadcastBuffer.DontDispose());
			}
			broadcastBuffer.Dispose();
		}

		// Token: 0x06003CB3 RID: 15539 RVA: 0x001645E4 File Offset: 0x001627E4
		private static MemoryBuffer GetBroadcastBuffer(AppBroadcast broadcast)
		{
			MemoryBuffer memoryBuffer = new MemoryBuffer(65536);
			Listener.Stream.SetData(memoryBuffer.Data, 0, memoryBuffer.Length);
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.broadcast = broadcast;
			appMessage.ToProto(Listener.Stream);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			return memoryBuffer.Slice((int)Listener.Stream.Position);
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x0016464E File Offset: 0x0016284E
		public bool CanSendPairingNotification(ulong playerId)
		{
			return this._pairingTokenBuckets.Get(playerId).TryTake(1.0);
		}

		// Token: 0x06003CB6 RID: 15542 RVA: 0x00164678 File Offset: 0x00162878
		[CompilerGenerated]
		private bool <Dispatch>g__Handle|21_15<TProto, THandler>(Func<AppRequest, TProto> protoSelector, out CompanionServer.Handlers.IHandler requestHandler, ref Listener.<>c__DisplayClass21_0 A_3) where TProto : class where THandler : BaseHandler<TProto>, new()
		{
			TProto tproto = protoSelector(A_3.request);
			if (tproto == null)
			{
				requestHandler = null;
				return false;
			}
			THandler thandler = Facepunch.Pool.Get<THandler>();
			thandler.Initialize(this._playerTokenBuckets, A_3.message.Connection, A_3.request, tproto);
			requestHandler = thandler;
			return true;
		}

		// Token: 0x0400370A RID: 14090
		private static readonly ByteArrayStream Stream = new ByteArrayStream();

		// Token: 0x0400370B RID: 14091
		private readonly TokenBucketList<IPAddress> _ipTokenBuckets;

		// Token: 0x0400370C RID: 14092
		private readonly BanList<IPAddress> _ipBans;

		// Token: 0x0400370D RID: 14093
		private readonly TokenBucketList<ulong> _playerTokenBuckets;

		// Token: 0x0400370E RID: 14094
		private readonly TokenBucketList<ulong> _pairingTokenBuckets;

		// Token: 0x0400370F RID: 14095
		private readonly Queue<Listener.Message> _messageQueue;

		// Token: 0x04003710 RID: 14096
		private readonly WebSocketServer _server;

		// Token: 0x04003711 RID: 14097
		private readonly Stopwatch _stopwatch;

		// Token: 0x04003712 RID: 14098
		private RealTimeSince _lastCleanup;

		// Token: 0x04003713 RID: 14099
		private long _nextConnectionId;

		// Token: 0x04003714 RID: 14100
		public readonly IPAddress Address;

		// Token: 0x04003715 RID: 14101
		public readonly int Port;

		// Token: 0x04003716 RID: 14102
		public readonly ConnectionLimiter Limiter;

		// Token: 0x04003717 RID: 14103
		public readonly SubscriberList<PlayerTarget, Connection, AppBroadcast> PlayerSubscribers;

		// Token: 0x04003718 RID: 14104
		public readonly SubscriberList<EntityTarget, Connection, AppBroadcast> EntitySubscribers;

		// Token: 0x04003719 RID: 14105
		public readonly SubscriberList<CameraTarget, Connection, AppBroadcast> CameraSubscribers;

		// Token: 0x02000EFC RID: 3836
		private struct Message
		{
			// Token: 0x060053F3 RID: 21491 RVA: 0x001B3EBB File Offset: 0x001B20BB
			public Message(Connection connection, MemoryBuffer buffer)
			{
				this.Connection = connection;
				this.Buffer = buffer;
			}

			// Token: 0x04004E39 RID: 20025
			public readonly Connection Connection;

			// Token: 0x04004E3A RID: 20026
			public readonly MemoryBuffer Buffer;
		}
	}
}
