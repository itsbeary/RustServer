using System;
using System.Collections.Generic;
using CompanionServer;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public class MapMarker : global::BaseEntity
{
	// Token: 0x060023AB RID: 9131 RVA: 0x000E3E5B File Offset: 0x000E205B
	public override void InitShared()
	{
		if (base.isServer && !MapMarker.serverMapMarkers.Contains(this))
		{
			MapMarker.serverMapMarkers.Add(this);
		}
		base.InitShared();
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000E3E83 File Offset: 0x000E2083
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			MapMarker.serverMapMarkers.Remove(this);
		}
		base.DestroyShared();
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000E3EA0 File Offset: 0x000E20A0
	public virtual AppMarker GetAppMarkerData()
	{
		AppMarker appMarker = Pool.Get<AppMarker>();
		Vector2 vector = CompanionServer.Util.WorldToMap(base.transform.position);
		appMarker.id = this.net.ID;
		appMarker.type = this.appType;
		appMarker.x = vector.x;
		appMarker.y = vector.y;
		return appMarker;
	}

	// Token: 0x04001BB7 RID: 7095
	public AppMarkerType appType;

	// Token: 0x04001BB8 RID: 7096
	public GameObjectRef markerObj;

	// Token: 0x04001BB9 RID: 7097
	public static readonly List<MapMarker> serverMapMarkers = new List<MapMarker>();

	// Token: 0x02000CF5 RID: 3317
	public enum ClusterType
	{
		// Token: 0x04004615 RID: 17941
		None,
		// Token: 0x04004616 RID: 17942
		Vending
	}
}
