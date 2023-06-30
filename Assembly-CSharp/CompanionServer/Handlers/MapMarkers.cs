using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A07 RID: 2567
	public class MapMarkers : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D3A RID: 15674 RVA: 0x00166AC8 File Offset: 0x00164CC8
		public override void Execute()
		{
			AppMapMarkers appMapMarkers = Pool.Get<AppMapMarkers>();
			appMapMarkers.markers = Pool.GetList<AppMarker>();
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam != null)
			{
				using (List<ulong>.Enumerator enumerator = playerTeam.members.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ulong num = enumerator.Current;
						global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
						if (!(basePlayer == null))
						{
							appMapMarkers.markers.Add(MapMarkers.GetPlayerMarker(basePlayer));
						}
					}
					goto IL_9A;
				}
			}
			if (base.Player != null)
			{
				appMapMarkers.markers.Add(MapMarkers.GetPlayerMarker(base.Player));
			}
			IL_9A:
			foreach (MapMarker mapMarker in MapMarker.serverMapMarkers)
			{
				if (mapMarker.appType != AppMarkerType.Undefined)
				{
					appMapMarkers.markers.Add(mapMarker.GetAppMarkerData());
				}
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.mapMarkers = appMapMarkers;
			base.Send(appResponse);
		}

		// Token: 0x06003D3B RID: 15675 RVA: 0x00166BEC File Offset: 0x00164DEC
		private static AppMarker GetPlayerMarker(global::BasePlayer player)
		{
			AppMarker appMarker = Pool.Get<AppMarker>();
			Vector2 vector = Util.WorldToMap(player.transform.position);
			appMarker.id = player.net.ID;
			appMarker.type = AppMarkerType.Player;
			appMarker.x = vector.x;
			appMarker.y = vector.y;
			appMarker.steamId = player.userID;
			return appMarker;
		}
	}
}
