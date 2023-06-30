using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009EA RID: 2538
	public class Connection : IConnection
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06003C82 RID: 15490 RVA: 0x001635F8 File Offset: 0x001617F8
		// (set) Token: 0x06003C83 RID: 15491 RVA: 0x00163600 File Offset: 0x00161800
		public long ConnectionId { get; private set; }

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06003C84 RID: 15492 RVA: 0x00163609 File Offset: 0x00161809
		public IPAddress Address
		{
			get
			{
				return this._connection.ConnectionInfo.ClientIpAddress;
			}
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x0016361B File Offset: 0x0016181B
		public Connection(long connectionId, Listener listener, IWebSocketConnection connection)
		{
			this.ConnectionId = connectionId;
			this._listener = listener;
			this._connection = connection;
			this._subscribedEntities = new HashSet<EntityTarget>();
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x00163644 File Offset: 0x00161844
		public void OnClose()
		{
			if (this._subscribedPlayer != null)
			{
				this._listener.PlayerSubscribers.Remove(this._subscribedPlayer.Value, this);
				this._subscribedPlayer = null;
			}
			foreach (EntityTarget entityTarget in this._subscribedEntities)
			{
				this._listener.EntitySubscribers.Remove(entityTarget, this);
			}
			this._subscribedEntities.Clear();
			IRemoteControllable currentCamera = this._currentCamera;
			if (currentCamera != null)
			{
				currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
			}
			CameraTarget cameraTarget;
			if (Connection.TryGetCameraTarget(this._currentCamera, out cameraTarget))
			{
				this._listener.CameraSubscribers.Remove(cameraTarget, this);
			}
			this._currentCamera = null;
			this._cameraViewerSteamId = 0UL;
			this._isControllingCamera = false;
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x0016373C File Offset: 0x0016193C
		public void OnMessage(System.Span<byte> data)
		{
			if (!App.update || App.queuelimit <= 0 || data.Length > App.maxmessagesize)
			{
				return;
			}
			MemoryBuffer memoryBuffer = new MemoryBuffer(data.Length);
			data.CopyTo(memoryBuffer);
			this._listener.Enqueue(this, memoryBuffer.Slice(data.Length));
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x0016379C File Offset: 0x0016199C
		public void Close()
		{
			IWebSocketConnection connection = this._connection;
			if (connection == null)
			{
				return;
			}
			connection.Close();
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x001637B0 File Offset: 0x001619B0
		public void Send(AppResponse response)
		{
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.response = response;
			Connection.MessageStream.Position = 0L;
			appMessage.ToProto(Connection.MessageStream);
			int num = (int)Connection.MessageStream.Position;
			Connection.MessageStream.Position = 0L;
			MemoryBuffer memoryBuffer = new MemoryBuffer(num);
			Connection.MessageStream.Read(memoryBuffer.Data, 0, num);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			this.SendRaw(memoryBuffer.Slice(num));
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x00163834 File Offset: 0x00161A34
		public void Subscribe(PlayerTarget target)
		{
			PlayerTarget? subscribedPlayer = this._subscribedPlayer;
			if (subscribedPlayer != null && (subscribedPlayer == null || subscribedPlayer.GetValueOrDefault() == target))
			{
				return;
			}
			this.EndViewing();
			if (this._subscribedPlayer != null)
			{
				this._listener.PlayerSubscribers.Remove(this._subscribedPlayer.Value, this);
				this._subscribedPlayer = null;
			}
			this._listener.PlayerSubscribers.Add(target, this);
			this._subscribedPlayer = new PlayerTarget?(target);
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x001638CB File Offset: 0x00161ACB
		public void Subscribe(EntityTarget target)
		{
			if (this._subscribedEntities.Add(target))
			{
				this._listener.EntitySubscribers.Add(target, this);
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06003C8C RID: 15500 RVA: 0x001638ED File Offset: 0x00161AED
		public IRemoteControllable CurrentCamera
		{
			get
			{
				return this._currentCamera;
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06003C8D RID: 15501 RVA: 0x001638F5 File Offset: 0x00161AF5
		public bool IsControllingCamera
		{
			get
			{
				return this._isControllingCamera;
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06003C8E RID: 15502 RVA: 0x001638FD File Offset: 0x00161AFD
		public ulong ControllingSteamId
		{
			get
			{
				return this._cameraViewerSteamId;
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06003C8F RID: 15503 RVA: 0x00163905 File Offset: 0x00161B05
		// (set) Token: 0x06003C90 RID: 15504 RVA: 0x0016390D File Offset: 0x00161B0D
		public InputState InputState { get; set; }

		// Token: 0x06003C91 RID: 15505 RVA: 0x00163918 File Offset: 0x00161B18
		public bool BeginViewing(IRemoteControllable camera)
		{
			if (this._subscribedPlayer == null)
			{
				return false;
			}
			CameraTarget cameraTarget;
			if (!Connection.TryGetCameraTarget(camera, out cameraTarget))
			{
				if (this._currentCamera == camera)
				{
					IRemoteControllable currentCamera = this._currentCamera;
					if (currentCamera != null)
					{
						currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
					}
					this._currentCamera = null;
					this._isControllingCamera = false;
					this._cameraViewerSteamId = 0UL;
				}
				return false;
			}
			if (this._currentCamera == camera)
			{
				this._listener.CameraSubscribers.Add(cameraTarget, this);
				return true;
			}
			CameraTarget cameraTarget2;
			if (Connection.TryGetCameraTarget(this._currentCamera, out cameraTarget2))
			{
				this._listener.CameraSubscribers.Remove(cameraTarget2, this);
				this._currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
				this._currentCamera = null;
				this._isControllingCamera = false;
				this._cameraViewerSteamId = 0UL;
			}
			ulong steamId = this._subscribedPlayer.Value.SteamId;
			if (!camera.CanControl(steamId))
			{
				return false;
			}
			this._listener.CameraSubscribers.Add(cameraTarget, this);
			this._currentCamera = camera;
			this._isControllingCamera = this._currentCamera.InitializeControl(new CameraViewerId(steamId, this.ConnectionId));
			this._cameraViewerSteamId = steamId;
			InputState inputState = this.InputState;
			if (inputState != null)
			{
				inputState.Clear();
			}
			return true;
		}

		// Token: 0x06003C92 RID: 15506 RVA: 0x00163A60 File Offset: 0x00161C60
		public void EndViewing()
		{
			CameraTarget cameraTarget;
			if (Connection.TryGetCameraTarget(this._currentCamera, out cameraTarget))
			{
				this._listener.CameraSubscribers.Remove(cameraTarget, this);
			}
			IRemoteControllable currentCamera = this._currentCamera;
			if (currentCamera != null)
			{
				currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
			}
			this._currentCamera = null;
			this._isControllingCamera = false;
			this._cameraViewerSteamId = 0UL;
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x00163AC8 File Offset: 0x00161CC8
		public void SendRaw(MemoryBuffer data)
		{
			try
			{
				this._connection.Send(data);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to send message to app client {0}: {1}", this._connection.ConnectionInfo.ClientIpAddress, ex));
			}
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x00163B18 File Offset: 0x00161D18
		private static bool TryGetCameraTarget(IRemoteControllable camera, out CameraTarget target)
		{
			global::BaseEntity baseEntity = ((camera != null) ? camera.GetEnt() : null);
			if (camera.IsUnityNull<IRemoteControllable>() || baseEntity == null || !baseEntity.IsValid())
			{
				target = default(CameraTarget);
				return false;
			}
			target = new CameraTarget(baseEntity.net.ID);
			return true;
		}

		// Token: 0x040036FC RID: 14076
		private static readonly MemoryStream MessageStream = new MemoryStream(1048576);

		// Token: 0x040036FD RID: 14077
		private readonly Listener _listener;

		// Token: 0x040036FE RID: 14078
		private readonly IWebSocketConnection _connection;

		// Token: 0x040036FF RID: 14079
		private PlayerTarget? _subscribedPlayer;

		// Token: 0x04003700 RID: 14080
		private readonly HashSet<EntityTarget> _subscribedEntities;

		// Token: 0x04003701 RID: 14081
		private IRemoteControllable _currentCamera;

		// Token: 0x04003702 RID: 14082
		private ulong _cameraViewerSteamId;

		// Token: 0x04003703 RID: 14083
		private bool _isControllingCamera;
	}
}
