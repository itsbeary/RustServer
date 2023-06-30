using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class TrainTrackSpline : WorldSpline
{
	// Token: 0x17000358 RID: 856
	// (get) Token: 0x060027AE RID: 10158 RVA: 0x000F7AB1 File Offset: 0x000F5CB1
	private bool HasNextTrack
	{
		get
		{
			return this.nextTracks.Count > 0;
		}
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x060027AF RID: 10159 RVA: 0x000F7AC1 File Offset: 0x000F5CC1
	private bool HasPrevTrack
	{
		get
		{
			return this.prevTracks.Count > 0;
		}
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x000F7AD1 File Offset: 0x000F5CD1
	public void SetAll(Vector3[] points, Vector3[] tangents, TrainTrackSpline sourceSpline)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = sourceSpline.lutInterval;
		this.isStation = sourceSpline.isStation;
		this.aboveGroundSpawn = sourceSpline.aboveGroundSpawn;
		this.hierarchy = sourceSpline.hierarchy;
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000F7B14 File Offset: 0x000F5D14
	public float GetSplineDistAfterMove(float prevSplineDist, Vector3 askerForward, float distMoved, TrainTrackSpline.TrackSelection trackSelection, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		bool flag = this.IsForward(askerForward, prevSplineDist);
		return this.GetSplineDistAfterMove(prevSplineDist, distMoved, trackSelection, flag, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000F7B40 File Offset: 0x000F5D40
	private float GetSplineDistAfterMove(float prevSplineDist, float distMoved, TrainTrackSpline.TrackSelection trackSelection, bool facingForward, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		float num = (facingForward ? (prevSplineDist + distMoved) : (prevSplineDist - distMoved));
		atEndOfLine = false;
		onSpline = this;
		if (num < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, facingForward, preferredAltA, preferredAltB);
				float num2 = (facingForward ? num : (-num));
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					prevSplineDist = 0f;
					facingForward = !facingForward;
				}
				return trackSelection2.track.GetSplineDistAfterMove(prevSplineDist, num2, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = 0f;
		}
		else if (num > data.Length)
		{
			if (this.HasNextTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, facingForward, preferredAltA, preferredAltB);
				float num3 = (facingForward ? (num - data.Length) : (-(num - data.Length)));
				if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = 0f;
				}
				else
				{
					prevSplineDist = trackSelection3.track.GetLength();
					facingForward = !facingForward;
				}
				return trackSelection3.track.GetSplineDistAfterMove(prevSplineDist, num3, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = data.Length;
		}
		return num;
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000F7C84 File Offset: 0x000F5E84
	public float GetDistance(Vector3 position, float maxError, out float minSplineDist)
	{
		WorldSplineData data = base.GetData();
		float num = maxError * maxError;
		Vector3 vector = base.transform.InverseTransformPoint(position);
		float num2 = float.MaxValue;
		minSplineDist = 0f;
		int num3 = 0;
		int num4 = data.LUTValues.Count;
		if (data.Length > 40f)
		{
			int num5 = 0;
			while ((float)num5 < data.Length + 10f)
			{
				float num6 = Vector3.SqrMagnitude(data.GetPointCubicHermite((float)num5) - vector);
				if (num6 < num2)
				{
					num2 = num6;
					minSplineDist = (float)num5;
				}
				num5 += 10;
			}
			num3 = Mathf.FloorToInt(Mathf.Max(0f, minSplineDist - 10f + 1f));
			num4 = Mathf.CeilToInt(Mathf.Min((float)data.LUTValues.Count, minSplineDist + 10f - 1f));
		}
		for (int i = num3; i < num4; i++)
		{
			WorldSplineData.LUTEntry lutentry = data.LUTValues[i];
			for (int j = 0; j < lutentry.points.Count; j++)
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = lutentry.points[j];
				float num7 = Vector3.SqrMagnitude(lutpoint.pos - vector);
				if (num7 < num2)
				{
					num2 = num7;
					minSplineDist = lutpoint.distance;
					if (num7 < num)
					{
						break;
					}
				}
			}
		}
		return Mathf.Sqrt(num2);
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000F7DD7 File Offset: 0x000F5FD7
	public float GetLength()
	{
		return base.GetData().Length;
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000F7DE4 File Offset: 0x000F5FE4
	public Vector3 GetPosition(float distance)
	{
		return base.GetPointCubicHermiteWorld(distance);
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000F7DED File Offset: 0x000F5FED
	public Vector3 GetPositionAndTangent(float distance, Vector3 askerForward, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermiteWorld = base.GetPointAndTangentCubicHermiteWorld(distance, out tangent);
		if (Vector3.Dot(askerForward, tangent) < 0f)
		{
			tangent = -tangent;
		}
		return pointAndTangentCubicHermiteWorld;
	}

	// Token: 0x060027B7 RID: 10167 RVA: 0x000F7E1C File Offset: 0x000F601C
	public void AddTrackConnection(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		List<TrainTrackSpline.ConnectedTrackInfo> list = ((p == TrainTrackSpline.TrackPosition.Next) ? this.nextTracks : this.prevTracks);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].track == track)
			{
				return;
			}
		}
		Vector3 vector = ((p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 2] : this.points[0]);
		Vector3 vector2 = ((p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 1] : this.points[1]);
		Vector3 vector3 = base.transform.TransformPoint(vector2) - base.transform.TransformPoint(vector);
		Vector3 initialVector = TrainTrackSpline.GetInitialVector(track, p, o);
		float num = Vector3.SignedAngle(vector3, initialVector, Vector3.up);
		int num2 = 0;
		while (num2 < list.Count && list[num2].angle <= num)
		{
			num2++;
		}
		list.Insert(num2, new TrainTrackSpline.ConnectedTrackInfo(track, o, num));
		int num3 = int.MaxValue;
		for (int j = 0; j < list.Count; j++)
		{
			num3 = Mathf.Min(num3, list[j].track.hierarchy);
		}
		float num4 = float.MaxValue;
		int num5 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo = list[k];
			if (connectedTrackInfo.track.hierarchy <= num3)
			{
				float num6 = Mathf.Abs(connectedTrackInfo.angle);
				if (num6 < num4)
				{
					num4 = num6;
					num5 = k;
					if (num4 == 0f)
					{
						break;
					}
				}
			}
		}
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			this.straightestNextIndex = num5;
			return;
		}
		this.straightestPrevIndex = num5;
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000F7FC4 File Offset: 0x000F61C4
	public void RegisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		this.trackUsers.Add(user);
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000F7FD3 File Offset: 0x000F61D3
	public void DeregisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		if (user == null)
		{
			return;
		}
		this.trackUsers.Remove(user);
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x000F7FE8 File Offset: 0x000F61E8
	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		WorldSplineData data = base.GetData();
		Vector3 tangentCubicHermiteWorld = base.GetTangentCubicHermiteWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentCubicHermiteWorld) >= 0f;
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x000F8018 File Offset: 0x000F6218
	public bool HasValidHazardWithin(TrainCar asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, float trackSpeed, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		Vector3 vector = ((trackSpeed >= 0f) ? asker.transform.forward : (-asker.transform.forward));
		bool flag = this.IsForward(vector, askerSplineDist);
		return this.HasValidHazardWithin(asker, vector, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, flag, preferredAltA, preferredAltB);
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x000F8068 File Offset: 0x000F6268
	public bool HasValidHazardWithin(TrainTrackSpline.ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		foreach (TrainTrackSpline.ITrainTrackUser trainTrackUser in this.trackUsers)
		{
			if (trainTrackUser != asker)
			{
				Vector3 vector = trainTrackUser.Position - asker.Position;
				if (Vector3.Dot(askerForward, vector) >= 0f)
				{
					float magnitude = vector.magnitude;
					if (magnitude > minHazardDist && magnitude < maxHazardDist)
					{
						Vector3 worldVelocity = trainTrackUser.GetWorldVelocity();
						if (worldVelocity.sqrMagnitude < 4f || Vector3.Dot(worldVelocity, vector) < 0f)
						{
							return true;
						}
					}
				}
			}
		}
		float num = (movingForward ? (askerSplineDist + minHazardDist) : (askerSplineDist - minHazardDist));
		float num2 = (movingForward ? (askerSplineDist + maxHazardDist) : (askerSplineDist - maxHazardDist));
		if (num2 < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, movingForward, preferredAltA, preferredAltB);
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					askerSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float num3 = Mathf.Max(-num, 0f);
				float num4 = -num2;
				return trackSelection2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, num3, num4, trackSelection, movingForward, preferredAltA, preferredAltB);
			}
		}
		else if (num2 > data.Length && this.HasNextTrack)
		{
			TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, movingForward, preferredAltA, preferredAltB);
			if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = trackSelection3.track.GetLength();
				movingForward = !movingForward;
			}
			float num5 = Mathf.Max(num - data.Length, 0f);
			float num6 = num2 - data.Length;
			return trackSelection3.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, num5, num6, trackSelection, movingForward, preferredAltA, preferredAltB);
		}
		return false;
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000F826C File Offset: 0x000F646C
	public bool HasAnyUsers()
	{
		return this.trackUsers.Count > 0;
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x000F827C File Offset: 0x000F647C
	public bool HasAnyUsersOfType(TrainCar.TrainCarType carType)
	{
		using (HashSet<TrainTrackSpline.ITrainTrackUser>.Enumerator enumerator = this.trackUsers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == carType)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x000F82D8 File Offset: 0x000F64D8
	public bool HasConnectedTrack(TrainTrackSpline tts)
	{
		return this.HasConnectedNextTrack(tts) || this.HasConnectedPrevTrack(tts);
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000F82EC File Offset: 0x000F64EC
	public bool HasConnectedNextTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.nextTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000F834C File Offset: 0x000F654C
	public bool HasConnectedPrevTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.prevTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000F83AC File Offset: 0x000F65AC
	private static Vector3 GetInitialVector(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		Vector3 vector;
		Vector3 vector2;
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			if (o == TrainTrackSpline.TrackOrientation.Reverse)
			{
				vector = track.points[track.points.Length - 1];
				vector2 = track.points[track.points.Length - 2];
			}
			else
			{
				vector = track.points[0];
				vector2 = track.points[1];
			}
		}
		else if (o == TrainTrackSpline.TrackOrientation.Reverse)
		{
			vector = track.points[1];
			vector2 = track.points[0];
		}
		else
		{
			vector = track.points[track.points.Length - 2];
			vector2 = track.points[track.points.Length - 1];
		}
		return track.transform.TransformPoint(vector2) - track.transform.TransformPoint(vector);
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000F8474 File Offset: 0x000F6674
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		for (int i = 0; i < this.nextTracks.Count; i++)
		{
			Color color = Color.white;
			if (this.straightestNextIndex != i && this.nextTracks.Count > 1)
			{
				if (i == 0)
				{
					color = Color.green;
				}
				else if (i == this.nextTracks.Count - 1)
				{
					color = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.nextTracks[i].track, color);
		}
		for (int j = 0; j < this.prevTracks.Count; j++)
		{
			Color color2 = Color.white;
			if (this.straightestPrevIndex != j && this.prevTracks.Count > 1)
			{
				if (j == 0)
				{
					color2 = Color.green;
				}
				else if (j == this.nextTracks.Count - 1)
				{
					color2 = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.prevTracks[j].track, color2);
		}
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000F8560 File Offset: 0x000F6760
	private TrainTrackSpline.ConnectedTrackInfo GetTrackSelection(List<TrainTrackSpline.ConnectedTrackInfo> trackOptions, int straightestIndex, TrainTrackSpline.TrackSelection trackSelection, bool nextTrack, bool trainForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		foreach (TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo in trackOptions)
		{
			if (connectedTrackInfo.track == preferredAltA || connectedTrackInfo.track == preferredAltB)
			{
				return connectedTrackInfo;
			}
		}
		bool flag = nextTrack ^ trainForward;
		if (trackSelection != TrainTrackSpline.TrackSelection.Left)
		{
			if (trackSelection != TrainTrackSpline.TrackSelection.Right)
			{
				return trackOptions[straightestIndex];
			}
			if (!flag)
			{
				return trackOptions[trackOptions.Count - 1];
			}
			return trackOptions[0];
		}
		else
		{
			if (!flag)
			{
				return trackOptions[0];
			}
			return trackOptions[trackOptions.Count - 1];
		}
		TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo2;
		return connectedTrackInfo2;
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000F8628 File Offset: 0x000F6828
	public static bool TryFindTrackNear(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		splineResult = null;
		distResult = 0f;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, maxDist, list, 65536, QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			List<TrainTrackSpline> list2 = Pool.GetList<TrainTrackSpline>();
			float num = float.MaxValue;
			foreach (Collider collider in list)
			{
				collider.GetComponentsInParent<TrainTrackSpline>(false, list2);
				if (list2.Count > 0)
				{
					foreach (TrainTrackSpline trainTrackSpline in list2)
					{
						float num2;
						float distance = trainTrackSpline.GetDistance(pos, 1f, out num2);
						if (distance < num)
						{
							num = distance;
							distResult = num2;
							splineResult = trainTrackSpline;
						}
					}
				}
			}
			Pool.FreeList<TrainTrackSpline>(ref list2);
		}
		Pool.FreeList<Collider>(ref list);
		return splineResult != null;
	}

	// Token: 0x04002060 RID: 8288
	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	// Token: 0x04002061 RID: 8289
	[Tooltip("Can above-ground trains spawn here?")]
	public bool aboveGroundSpawn;

	// Token: 0x04002062 RID: 8290
	public int hierarchy;

	// Token: 0x04002063 RID: 8291
	public static List<TrainTrackSpline> SidingSplines = new List<TrainTrackSpline>();

	// Token: 0x04002064 RID: 8292
	private List<TrainTrackSpline.ConnectedTrackInfo> nextTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04002065 RID: 8293
	private int straightestNextIndex;

	// Token: 0x04002066 RID: 8294
	private List<TrainTrackSpline.ConnectedTrackInfo> prevTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04002067 RID: 8295
	private int straightestPrevIndex;

	// Token: 0x04002068 RID: 8296
	private HashSet<TrainTrackSpline.ITrainTrackUser> trackUsers = new HashSet<TrainTrackSpline.ITrainTrackUser>();

	// Token: 0x02000D2A RID: 3370
	public enum TrackSelection
	{
		// Token: 0x040046F5 RID: 18165
		Default,
		// Token: 0x040046F6 RID: 18166
		Left,
		// Token: 0x040046F7 RID: 18167
		Right
	}

	// Token: 0x02000D2B RID: 3371
	public enum TrackPosition
	{
		// Token: 0x040046F9 RID: 18169
		Next,
		// Token: 0x040046FA RID: 18170
		Prev
	}

	// Token: 0x02000D2C RID: 3372
	public enum TrackOrientation
	{
		// Token: 0x040046FC RID: 18172
		Same,
		// Token: 0x040046FD RID: 18173
		Reverse
	}

	// Token: 0x02000D2D RID: 3373
	private class ConnectedTrackInfo
	{
		// Token: 0x06005078 RID: 20600 RVA: 0x001A9135 File Offset: 0x001A7335
		public ConnectedTrackInfo(TrainTrackSpline track, TrainTrackSpline.TrackOrientation orientation, float angle)
		{
			this.track = track;
			this.orientation = orientation;
			this.angle = angle;
		}

		// Token: 0x040046FE RID: 18174
		public TrainTrackSpline track;

		// Token: 0x040046FF RID: 18175
		public TrainTrackSpline.TrackOrientation orientation;

		// Token: 0x04004700 RID: 18176
		public float angle;
	}

	// Token: 0x02000D2E RID: 3374
	public enum DistanceType
	{
		// Token: 0x04004702 RID: 18178
		SplineDistance,
		// Token: 0x04004703 RID: 18179
		WorldDistance
	}

	// Token: 0x02000D2F RID: 3375
	public interface ITrainTrackUser
	{
		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06005079 RID: 20601
		Vector3 Position { get; }

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x0600507A RID: 20602
		float FrontWheelSplineDist { get; }

		// Token: 0x0600507B RID: 20603
		Vector3 GetWorldVelocity();

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x0600507C RID: 20604
		TrainCar.TrainCarType CarType { get; }
	}
}
