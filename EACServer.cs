using System;
using System.Collections.Concurrent;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;
using Epic.OnlineServices.Reports;
using Network;
using UnityEngine;

// Token: 0x02000747 RID: 1863
public static class EACServer
{
	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x060033B6 RID: 13238 RVA: 0x0013D118 File Offset: 0x0013B318
	private static bool CanEnableGameplayData
	{
		get
		{
			return ConVar.Server.official && ConVar.Server.stats;
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x060033B7 RID: 13239 RVA: 0x0013D128 File Offset: 0x0013B328
	private static bool CanSendAnalytics
	{
		get
		{
			return EACServer.CanEnableGameplayData && EACServer.Interface != null;
		}
	}

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x060033B8 RID: 13240 RVA: 0x0013D13E File Offset: 0x0013B33E
	private static bool CanSendReports
	{
		get
		{
			return EACServer.Reports != null;
		}
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x0013D14B File Offset: 0x0013B34B
	private static IntPtr GenerateCompatibilityClient()
	{
		return (IntPtr)((long)((ulong)(EACServer.clientHandleCounter += 1U)));
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0013D160 File Offset: 0x0013B360
	public static void Encrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				ProtectMessageOptions protectMessageOptions = new ProtectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint num;
				Result result = EACServer.Interface.ProtectMessage(ref protectMessageOptions, dst, out num);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)num);
					return;
				}
				Debug.LogWarning("[EAC] ProtectMessage failed: " + result);
			}
		}
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x0013D218 File Offset: 0x0013B418
	public static void Decrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				UnprotectMessageOptions unprotectMessageOptions = new UnprotectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint num;
				Result result = EACServer.Interface.UnprotectMessage(ref unprotectMessageOptions, dst, out num);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)num);
					return;
				}
				Debug.LogWarning("[EAC] UnprotectMessage failed: " + result);
			}
		}
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0013D2D0 File Offset: 0x0013B4D0
	private static IntPtr GetClient(Connection connection)
	{
		uint num;
		EACServer.connection2client.TryGetValue(connection, out num);
		return (IntPtr)((long)((ulong)num));
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x0013D2F4 File Offset: 0x0013B4F4
	private static Connection GetConnection(IntPtr client)
	{
		Connection connection;
		EACServer.client2connection.TryGetValue((uint)(int)client, out connection);
		return connection;
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x0013D318 File Offset: 0x0013B518
	public static bool IsAuthenticated(Connection connection)
	{
		AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
		EACServer.connection2status.TryGetValue(connection, out antiCheatCommonClientAuthStatus);
		return antiCheatCommonClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x0013D337 File Offset: 0x0013B537
	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.LocalAuthComplete;
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x0013D362 File Offset: 0x0013B562
	private static void OnAuthenticatedRemote(Connection connection)
	{
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x0013D370 File Offset: 0x0013B570
	private static void OnClientAuthStatusChanged(ref OnClientAuthStatusChangedCallbackInfo data)
	{
		using (TimeWarning.New("AntiCheatKickPlayer", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.LocalAuthComplete)
			{
				EACServer.OnAuthenticatedLocal(connection);
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = clientHandle,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				EACServer.OnAuthenticatedRemote(connection);
			}
		}
	}

	// Token: 0x060033C2 RID: 13250 RVA: 0x0013D418 File Offset: 0x0013B618
	private static void OnClientActionRequired(ref OnClientActionRequiredCallbackInfo data)
	{
		using (TimeWarning.New("OnClientActionRequired", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else
			{
				AntiCheatCommonClientAction clientAction = data.ClientAction;
				if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
				{
					Utf8String actionReasonDetailsString = data.ActionReasonDetailsString;
					Debug.Log(string.Format("[EAC] Kicking {0} / {1} ({2})", connection.userid, connection.username, actionReasonDetailsString));
					connection.authStatus = "eac";
					Network.Net.sv.Kick(connection, "EAC: " + actionReasonDetailsString, false);
					if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned || data.ActionReasonCode == AntiCheatCommonClientActionReason.TemporaryBanned)
					{
						connection.authStatus = "eacbanned";
						ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
						{
							2,
							0,
							"<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)"
						});
						if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned)
						{
							Entity.DeleteBy(connection.userid);
						}
					}
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = clientHandle
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)(int)clientHandle, out connection2);
					uint num;
					EACServer.connection2client.TryRemove(connection, out num);
					AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
					EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				}
			}
		}
	}

	// Token: 0x060033C3 RID: 13251 RVA: 0x0013D5AC File Offset: 0x0013B7AC
	private static void SendToClient(ref OnMessageToClientCallbackInfo data)
	{
		IntPtr clientHandle = data.ClientHandle;
		Connection connection = EACServer.GetConnection(clientHandle);
		if (connection == null)
		{
			Debug.LogError("[EAC] Network packet for invalid client: " + clientHandle.ToString());
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EAC);
		netWrite.UInt32((uint)data.MessageData.Count);
		netWrite.Write(data.MessageData.Array, data.MessageData.Offset, data.MessageData.Count);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x0013D644 File Offset: 0x0013B844
	public static void DoStartup()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			AddNotifyClientActionRequiredOptions addNotifyClientActionRequiredOptions = default(AddNotifyClientActionRequiredOptions);
			AddNotifyClientAuthStatusChangedOptions addNotifyClientAuthStatusChangedOptions = default(AddNotifyClientAuthStatusChangedOptions);
			AddNotifyMessageToClientOptions addNotifyMessageToClientOptions = default(AddNotifyMessageToClientOptions);
			BeginSessionOptions beginSessionOptions = new BeginSessionOptions
			{
				LocalUserId = null,
				EnableGameplayData = EACServer.CanEnableGameplayData,
				RegisterTimeoutSeconds = 20U,
				ServerName = ConVar.Server.hostname
			};
			LogGameRoundStartOptions logGameRoundStartOptions = new LogGameRoundStartOptions
			{
				LevelName = global::World.Name
			};
			EOS.Initialize(true, ConVar.Server.anticheatid, ConVar.Server.anticheatkey, ConVar.Server.rootFolder + "/Log.EAC.txt");
			EACServer.Interface = EOS.Interface.GetAntiCheatServerInterface();
			EACServer.Interface.AddNotifyClientActionRequired(ref addNotifyClientActionRequiredOptions, null, new OnClientActionRequiredCallback(EACServer.OnClientActionRequired));
			EACServer.Interface.AddNotifyClientAuthStatusChanged(ref addNotifyClientAuthStatusChangedOptions, null, new OnClientAuthStatusChangedCallback(EACServer.OnClientAuthStatusChanged));
			EACServer.Interface.AddNotifyMessageToClient(ref addNotifyMessageToClientOptions, null, new OnMessageToClientCallback(EACServer.SendToClient));
			EACServer.Interface.BeginSession(ref beginSessionOptions);
			EACServer.Interface.LogGameRoundStart(ref logGameRoundStartOptions);
			return;
		}
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
	}

	// Token: 0x060033C5 RID: 13253 RVA: 0x0013D7AF File Offset: 0x0013B9AF
	public static void DoUpdate()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EOS.Tick();
		}
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x0013D7C4 File Offset: 0x0013B9C4
	public static void DoShutdown()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			if (EACServer.Interface != null)
			{
				Debug.Log("EasyAntiCheat Server Shutting Down");
				EndSessionOptions endSessionOptions = default(EndSessionOptions);
				EACServer.Interface.EndSession(ref endSessionOptions);
				EACServer.Interface = null;
				EOS.Shutdown();
				return;
			}
		}
		else
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
		}
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x0013D858 File Offset: 0x0013BA58
	public static void OnLeaveGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr client = EACServer.GetClient(connection);
				if (client != IntPtr.Zero)
				{
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = client
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)(int)client, out connection2);
				}
				uint num;
				EACServer.connection2client.TryRemove(connection, out num);
				AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
				EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				return;
			}
		}
		else
		{
			AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
			EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
		}
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x0013D8F4 File Offset: 0x0013BAF4
	public static void OnJoinGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr intPtr = EACServer.GenerateCompatibilityClient();
				if (intPtr == IntPtr.Zero)
				{
					Debug.LogError("[EAC] GenerateCompatibilityClient returned invalid client: " + intPtr.ToString());
					return;
				}
				RegisterClientOptions registerClientOptions = new RegisterClientOptions
				{
					ClientHandle = intPtr,
					AccountId = connection.userid.ToString(),
					IpAddress = connection.IPAddressWithoutPort(),
					ClientType = ((connection.authLevel >= 3U && connection.os == "editor") ? AntiCheatCommonClientType.UnprotectedClient : AntiCheatCommonClientType.ProtectedClient),
					ClientPlatform = ((connection.os == "windows") ? AntiCheatCommonClientPlatform.Windows : ((connection.os == "linux") ? AntiCheatCommonClientPlatform.Linux : ((connection.os == "mac") ? AntiCheatCommonClientPlatform.Mac : AntiCheatCommonClientPlatform.Unknown)))
				};
				SetClientDetailsOptions setClientDetailsOptions = new SetClientDetailsOptions
				{
					ClientHandle = intPtr,
					ClientFlags = ((connection.authLevel > 0U) ? AntiCheatCommonClientFlags.Admin : AntiCheatCommonClientFlags.None)
				};
				EACServer.Interface.RegisterClient(ref registerClientOptions);
				EACServer.Interface.SetClientDetails(ref setClientDetailsOptions);
				EACServer.client2connection.TryAdd((uint)(int)intPtr, connection);
				EACServer.connection2client.TryAdd(connection, (uint)(int)intPtr);
				EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
				return;
			}
		}
		else
		{
			EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
			EACServer.OnAuthenticatedLocal(connection);
			EACServer.OnAuthenticatedRemote(connection);
		}
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x0013DA84 File Offset: 0x0013BC84
	public static void OnStartLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x060033CA RID: 13258 RVA: 0x0013DADC File Offset: 0x0013BCDC
	public static void OnFinishLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = true
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x060033CB RID: 13259 RVA: 0x0013DB34 File Offset: 0x0013BD34
	public static void OnMessageReceived(Message message)
	{
		IntPtr client = EACServer.GetClient(message.connection);
		if (client == IntPtr.Zero)
		{
			Debug.LogError("EAC network packet from invalid connection: " + message.connection.userid);
			return;
		}
		byte[] array;
		int num;
		if (!message.read.TemporaryBytesWithSize(out array, out num))
		{
			return;
		}
		ReceiveMessageFromClientOptions receiveMessageFromClientOptions = new ReceiveMessageFromClientOptions
		{
			ClientHandle = client,
			Data = new ArraySegment<byte>(array, 0, num)
		};
		EACServer.Interface.ReceiveMessageFromClient(ref receiveMessageFromClientOptions);
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x0013DBBC File Offset: 0x0013BDBC
	public static void LogPlayerUseWeapon(BasePlayer player, BaseProjectile weapon)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerShooting", 0))
			{
				Vector3 networkPosition = player.GetNetworkPosition();
				Quaternion networkRotation = player.GetNetworkRotation();
				Item item = weapon.GetItem();
				string text = ((item != null) ? item.info.shortname : "unknown");
				LogPlayerUseWeaponOptions logPlayerUseWeaponOptions = default(LogPlayerUseWeaponOptions);
				logPlayerUseWeaponOptions.UseWeaponData = new LogPlayerUseWeaponData?(new LogPlayerUseWeaponData
				{
					PlayerHandle = EACServer.GetClient(player.net.connection),
					PlayerPosition = new Vec3f?(new Vec3f
					{
						x = networkPosition.x,
						y = networkPosition.y,
						z = networkPosition.z
					}),
					PlayerViewRotation = new Quat?(new Quat
					{
						w = networkRotation.w,
						x = networkRotation.x,
						y = networkRotation.y,
						z = networkRotation.z
					}),
					WeaponName = text
				});
				EACServer.Interface.LogPlayerUseWeapon(ref logPlayerUseWeaponOptions);
			}
		}
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x0013DD1C File Offset: 0x0013BF1C
	public static void LogPlayerSpawn(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerSpawn", 0))
			{
				LogPlayerSpawnOptions logPlayerSpawnOptions = default(LogPlayerSpawnOptions);
				logPlayerSpawnOptions.SpawnedPlayerHandle = EACServer.GetClient(player.net.connection);
				EACServer.Interface.LogPlayerSpawn(ref logPlayerSpawnOptions);
			}
		}
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x0013DD94 File Offset: 0x0013BF94
	public static void LogPlayerDespawn(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerDespawn", 0))
			{
				LogPlayerDespawnOptions logPlayerDespawnOptions = default(LogPlayerDespawnOptions);
				logPlayerDespawnOptions.DespawnedPlayerHandle = EACServer.GetClient(player.net.connection);
				EACServer.Interface.LogPlayerDespawn(ref logPlayerDespawnOptions);
			}
		}
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x0013DE0C File Offset: 0x0013C00C
	public static void LogPlayerTakeDamage(BasePlayer player, HitInfo info)
	{
		if (EACServer.CanSendAnalytics && info.Initiator != null && info.Initiator is BasePlayer)
		{
			BasePlayer basePlayer = info.Initiator.ToPlayer();
			if (player.net.connection != null && basePlayer.net.connection != null)
			{
				using (TimeWarning.New("EAC.LogPlayerTakeDamage", 0))
				{
					LogPlayerTakeDamageOptions logPlayerTakeDamageOptions = default(LogPlayerTakeDamageOptions);
					LogPlayerUseWeaponData logPlayerUseWeaponData = default(LogPlayerUseWeaponData);
					logPlayerTakeDamageOptions.AttackerPlayerHandle = EACServer.GetClient(basePlayer.net.connection);
					logPlayerTakeDamageOptions.VictimPlayerHandle = EACServer.GetClient(player.net.connection);
					logPlayerTakeDamageOptions.DamageTaken = info.damageTypes.Total();
					logPlayerTakeDamageOptions.DamagePosition = new Vec3f?(new Vec3f
					{
						x = info.HitPositionWorld.x,
						y = info.HitPositionWorld.y,
						z = info.HitPositionWorld.z
					});
					logPlayerTakeDamageOptions.IsCriticalHit = info.isHeadshot;
					if (player.IsDead())
					{
						logPlayerTakeDamageOptions.DamageResult = AntiCheatCommonPlayerTakeDamageResult.Eliminated;
					}
					else if (player.IsWounded())
					{
						logPlayerTakeDamageOptions.DamageResult = AntiCheatCommonPlayerTakeDamageResult.Downed;
					}
					if (info.Weapon != null)
					{
						Item item = info.Weapon.GetItem();
						if (item != null)
						{
							logPlayerUseWeaponData.WeaponName = item.info.shortname;
						}
						else
						{
							logPlayerUseWeaponData.WeaponName = "unknown";
						}
					}
					else
					{
						logPlayerUseWeaponData.WeaponName = "unknown";
					}
					Vector3 position = basePlayer.eyes.position;
					Quaternion rotation = basePlayer.eyes.rotation;
					Vector3 position2 = player.eyes.position;
					Quaternion rotation2 = player.eyes.rotation;
					logPlayerTakeDamageOptions.AttackerPlayerPosition = new Vec3f?(new Vec3f
					{
						x = position.x,
						y = position.y,
						z = position.z
					});
					logPlayerTakeDamageOptions.AttackerPlayerViewRotation = new Quat?(new Quat
					{
						w = rotation.w,
						x = rotation.x,
						y = rotation.y,
						z = rotation.z
					});
					logPlayerTakeDamageOptions.VictimPlayerPosition = new Vec3f?(new Vec3f
					{
						x = position2.x,
						y = position2.y,
						z = position2.z
					});
					logPlayerTakeDamageOptions.VictimPlayerViewRotation = new Quat?(new Quat
					{
						w = rotation2.w,
						x = rotation2.x,
						y = rotation2.y,
						z = rotation2.z
					});
					logPlayerTakeDamageOptions.PlayerUseWeaponData = new LogPlayerUseWeaponData?(logPlayerUseWeaponData);
					EACServer.Interface.LogPlayerTakeDamage(ref logPlayerTakeDamageOptions);
				}
			}
		}
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x0013E138 File Offset: 0x0013C338
	public static void LogPlayerTick(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net != null && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerTick", 0))
			{
				Vector3 position = player.eyes.position;
				Quaternion rotation = player.eyes.rotation;
				LogPlayerTickOptions logPlayerTickOptions = default(LogPlayerTickOptions);
				logPlayerTickOptions.PlayerHandle = EACServer.GetClient(player.net.connection);
				logPlayerTickOptions.PlayerPosition = new Vec3f?(new Vec3f
				{
					x = position.x,
					y = position.y,
					z = position.z
				});
				logPlayerTickOptions.PlayerViewRotation = new Quat?(new Quat
				{
					w = rotation.w,
					x = rotation.x,
					y = rotation.y,
					z = rotation.z
				});
				logPlayerTickOptions.PlayerHealth = player.Health();
				if (player.IsDucked())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Crouching;
				}
				if (player.isMounted)
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Mounted;
				}
				if (player.IsCrawling())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Prone;
				}
				if (player.IsSwimming())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Swimming;
				}
				if (!player.IsOnGround())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Falling;
				}
				if (player.OnLadder())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.OnLadder;
				}
				if (player.IsFlying)
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Flying;
				}
				EACServer.Interface.LogPlayerTick(ref logPlayerTickOptions);
			}
		}
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x0013E318 File Offset: 0x0013C518
	public static void LogPlayerRevive(BasePlayer source, BasePlayer target)
	{
		if (EACServer.CanSendAnalytics && target.net.connection != null && source != null && source.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerRevive", 0))
			{
				LogPlayerReviveOptions logPlayerReviveOptions = default(LogPlayerReviveOptions);
				logPlayerReviveOptions.RevivedPlayerHandle = EACServer.GetClient(target.net.connection);
				logPlayerReviveOptions.ReviverPlayerHandle = EACServer.GetClient(source.net.connection);
				EACServer.Interface.LogPlayerRevive(ref logPlayerReviveOptions);
			}
		}
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x0013E3BC File Offset: 0x0013C5BC
	public static void SendPlayerBehaviorReport(BasePlayer reporter, PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		if (EACServer.CanSendReports)
		{
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReportedUserId = ProductUserId.FromString(reportedID),
				ReporterUserId = ProductUserId.FromString(reporter.UserIDString),
				Category = reportCategory,
				Message = reportText
			};
			EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
		}
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x0013E428 File Offset: 0x0013C628
	public static void SendPlayerBehaviorReport(PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		if (EACServer.CanSendReports)
		{
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReportedUserId = ProductUserId.FromString(reportedID),
				Category = reportCategory,
				Message = reportText
			};
			EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
		}
	}

	// Token: 0x04002A74 RID: 10868
	private static AntiCheatServerInterface Interface = null;

	// Token: 0x04002A75 RID: 10869
	private static ReportsInterface Reports = null;

	// Token: 0x04002A76 RID: 10870
	private static ConcurrentDictionary<uint, Connection> client2connection = new ConcurrentDictionary<uint, Connection>();

	// Token: 0x04002A77 RID: 10871
	private static ConcurrentDictionary<Connection, uint> connection2client = new ConcurrentDictionary<Connection, uint>();

	// Token: 0x04002A78 RID: 10872
	private static ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus> connection2status = new ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus>();

	// Token: 0x04002A79 RID: 10873
	private static uint clientHandleCounter = 0U;
}
