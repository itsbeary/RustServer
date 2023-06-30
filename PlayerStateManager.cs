using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000748 RID: 1864
public class PlayerStateManager
{
	// Token: 0x060033D5 RID: 13269 RVA: 0x0013E4AD File Offset: 0x0013C6AD
	public PlayerStateManager(UserPersistance persistence)
	{
		this._cache = new MruDictionary<ulong, PlayerState>(1000, new Action<ulong, PlayerState>(this.FreeOldState));
		this._persistence = persistence;
	}

	// Token: 0x060033D6 RID: 13270 RVA: 0x0013E4D8 File Offset: 0x0013C6D8
	public PlayerState Get(ulong playerId)
	{
		PlayerState playerState2;
		using (TimeWarning.New("PlayerStateManager.Get", 0))
		{
			PlayerState playerState;
			if (this._cache.TryGetValue(playerId, out playerState))
			{
				playerState2 = playerState;
			}
			else
			{
				byte[] playerState3 = this._persistence.GetPlayerState(playerId);
				PlayerState playerState4;
				if (playerState3 != null && playerState3.Length != 0)
				{
					try
					{
						playerState4 = PlayerState.Deserialize(playerState3);
						this.OnPlayerStateLoaded(playerState4);
						this._cache.Add(playerId, playerState4);
						return playerState4;
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("Failed to load player state for {0}: {1}", playerId, ex));
					}
				}
				playerState4 = Pool.Get<PlayerState>();
				this._cache.Add(playerId, playerState4);
				playerState2 = playerState4;
			}
		}
		return playerState2;
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x0013E598 File Offset: 0x0013C798
	public void Save(ulong playerId)
	{
		PlayerState playerState;
		if (!this._cache.TryGetValue(playerId, out playerState))
		{
			return;
		}
		this.SaveState(playerId, playerState);
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x0013E5C0 File Offset: 0x0013C7C0
	private void SaveState(ulong playerId, PlayerState state)
	{
		using (TimeWarning.New("PlayerStateManager.SaveState", 0))
		{
			try
			{
				byte[] array = PlayerState.SerializeToBytes(state);
				this._persistence.SetPlayerState(playerId, array);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to save player state for {0}: {1}", playerId, ex));
			}
		}
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x0013E630 File Offset: 0x0013C830
	private void FreeOldState(ulong playerId, PlayerState state)
	{
		this.SaveState(playerId, state);
		state.Dispose();
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x0013E640 File Offset: 0x0013C840
	public void Reset(ulong playerId)
	{
		this._cache.Remove(playerId);
		this._persistence.ResetPlayerState(playerId);
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x0013E65A File Offset: 0x0013C85A
	private void OnPlayerStateLoaded(PlayerState state)
	{
		state.unHostileTimestamp = Math.Min(state.unHostileTimestamp, TimeEx.currentTimestamp + 1800.0);
	}

	// Token: 0x04002A7A RID: 10874
	private readonly MruDictionary<ulong, PlayerState> _cache;

	// Token: 0x04002A7B RID: 10875
	private readonly UserPersistance _persistence;
}
