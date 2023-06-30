using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B53 RID: 2899
	public class WaypointSet : MonoBehaviour, IServerComponent
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06004623 RID: 17955 RVA: 0x00198D28 File Offset: 0x00196F28
		// (set) Token: 0x06004624 RID: 17956 RVA: 0x00198D30 File Offset: 0x00196F30
		public List<WaypointSet.Waypoint> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06004625 RID: 17957 RVA: 0x00198D39 File Offset: 0x00196F39
		public WaypointSet.NavModes NavMode
		{
			get
			{
				return this.navMode;
			}
		}

		// Token: 0x06004626 RID: 17958 RVA: 0x00198D44 File Offset: 0x00196F44
		private void OnDrawGizmos()
		{
			for (int i = 0; i < this.Points.Count; i++)
			{
				Transform transform = this.Points[i].Transform;
				if (transform != null)
				{
					if (this.Points[i].IsOccupied)
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.cyan;
					}
					Gizmos.DrawSphere(transform.position, 0.25f);
					Gizmos.color = Color.cyan;
					if (i + 1 < this.Points.Count)
					{
						Gizmos.DrawLine(transform.position, this.Points[i + 1].Transform.position);
					}
					else if (this.NavMode == WaypointSet.NavModes.Loop)
					{
						Gizmos.DrawLine(transform.position, this.Points[0].Transform.position);
					}
					Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
					foreach (Transform transform2 in this.Points[i].LookatPoints)
					{
						Gizmos.DrawSphere(transform2.position, 0.1f);
						Gizmos.DrawLine(transform.position, transform2.position);
					}
				}
			}
		}

		// Token: 0x04003F1A RID: 16154
		[SerializeField]
		private List<WaypointSet.Waypoint> _points = new List<WaypointSet.Waypoint>();

		// Token: 0x04003F1B RID: 16155
		[SerializeField]
		private WaypointSet.NavModes navMode;

		// Token: 0x02000FB3 RID: 4019
		public enum NavModes
		{
			// Token: 0x04005125 RID: 20773
			Loop,
			// Token: 0x04005126 RID: 20774
			PingPong
		}

		// Token: 0x02000FB4 RID: 4020
		[Serializable]
		public struct Waypoint
		{
			// Token: 0x04005127 RID: 20775
			public Transform Transform;

			// Token: 0x04005128 RID: 20776
			public float WaitTime;

			// Token: 0x04005129 RID: 20777
			public Transform[] LookatPoints;

			// Token: 0x0400512A RID: 20778
			[NonSerialized]
			public bool IsOccupied;
		}
	}
}
