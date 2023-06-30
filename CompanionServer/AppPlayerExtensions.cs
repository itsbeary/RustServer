using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009F7 RID: 2551
	public static class AppPlayerExtensions
	{
		// Token: 0x06003CEA RID: 15594 RVA: 0x001653AC File Offset: 0x001635AC
		public static AppTeamInfo GetAppTeamInfo(this global::BasePlayer player, ulong steamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
			if (player != null)
			{
				Vector2 vector = Util.WorldToMap(player.transform.position);
				member.steamId = player.userID;
				member.name = player.displayName ?? "";
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = player.IsConnected;
				AppTeamInfo.Member member2 = member;
				PlayerLifeStory lifeStory = player.lifeStory;
				member2.spawnTime = ((lifeStory != null) ? lifeStory.timeBorn : 0U);
				member.isAlive = player.IsAlive();
				AppTeamInfo.Member member3 = member;
				PlayerLifeStory previousLifeStory = player.previousLifeStory;
				member3.deathTime = ((previousLifeStory != null) ? previousLifeStory.timeDied : 0U);
			}
			else
			{
				member.steamId = steamId;
				member.name = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId) ?? "";
				member.x = 0f;
				member.y = 0f;
				member.isOnline = false;
				member.spawnTime = 0U;
				member.isAlive = false;
				member.deathTime = 0U;
			}
			appTeamInfo.members.Add(member);
			appTeamInfo.leaderSteamId = 0UL;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(member.steamId, true);
			appTeamInfo.leaderMapNotes = Pool.GetList<AppTeamInfo.Note>();
			return appTeamInfo;
		}

		// Token: 0x06003CEB RID: 15595 RVA: 0x001654F8 File Offset: 0x001636F8
		public static AppTeamInfo GetAppTeamInfo(this global::RelationshipManager.PlayerTeam team, ulong requesterSteamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			for (int i = 0; i < team.members.Count; i++)
			{
				ulong num = team.members[i];
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
				if (!basePlayer)
				{
					basePlayer = null;
				}
				Vector2 vector = Util.WorldToMap((basePlayer != null) ? basePlayer.transform.position : Vector3.zero);
				AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
				member.steamId = num;
				AppTeamInfo.Member member2 = member;
				string text;
				if ((text = ((basePlayer != null) ? basePlayer.displayName : null)) == null)
				{
					text = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "";
				}
				member2.name = text;
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = basePlayer != null && basePlayer.IsConnected;
				AppTeamInfo.Member member3 = member;
				uint? num2;
				if (basePlayer == null)
				{
					num2 = null;
				}
				else
				{
					PlayerLifeStory lifeStory = basePlayer.lifeStory;
					num2 = ((lifeStory != null) ? new uint?(lifeStory.timeBorn) : null);
				}
				member3.spawnTime = num2 ?? 0U;
				member.isAlive = basePlayer != null && basePlayer.IsAlive();
				AppTeamInfo.Member member4 = member;
				uint? num3;
				if (basePlayer == null)
				{
					num3 = null;
				}
				else
				{
					PlayerLifeStory previousLifeStory = basePlayer.previousLifeStory;
					num3 = ((previousLifeStory != null) ? new uint?(previousLifeStory.timeDied) : null);
				}
				member4.deathTime = num3 ?? 0U;
				appTeamInfo.members.Add(member);
			}
			appTeamInfo.leaderSteamId = team.teamLeader;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(requesterSteamId, true);
			if (requesterSteamId != team.teamLeader)
			{
				appTeamInfo.leaderMapNotes = AppPlayerExtensions.GetMapNotes(team.teamLeader, false);
			}
			else
			{
				appTeamInfo.leaderMapNotes = Pool.GetList<AppTeamInfo.Note>();
			}
			return appTeamInfo;
		}

		// Token: 0x06003CEC RID: 15596 RVA: 0x001656D4 File Offset: 0x001638D4
		private static List<AppTeamInfo.Note> GetMapNotes(ulong playerId, bool personalNotes)
		{
			List<AppTeamInfo.Note> list = Pool.GetList<AppTeamInfo.Note>();
			PlayerState playerState = SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(playerId);
			if (playerState != null)
			{
				if (personalNotes && playerState.deathMarker != null)
				{
					AppPlayerExtensions.AddMapNote(list, playerState.deathMarker, global::BasePlayer.MapNoteType.Death);
				}
				if (playerState.pointsOfInterest != null)
				{
					foreach (MapNote mapNote in playerState.pointsOfInterest)
					{
						AppPlayerExtensions.AddMapNote(list, mapNote, global::BasePlayer.MapNoteType.PointOfInterest);
					}
				}
			}
			return list;
		}

		// Token: 0x06003CED RID: 15597 RVA: 0x00165764 File Offset: 0x00163964
		private static void AddMapNote(List<AppTeamInfo.Note> result, MapNote note, global::BasePlayer.MapNoteType type)
		{
			Vector2 vector = Util.WorldToMap(note.worldPosition);
			AppTeamInfo.Note note2 = Pool.Get<AppTeamInfo.Note>();
			note2.type = (int)type;
			note2.x = vector.x;
			note2.y = vector.y;
			note2.icon = note.icon;
			note2.colourIndex = note.colourIndex;
			note2.label = note.label;
			result.Add(note2);
		}
	}
}
