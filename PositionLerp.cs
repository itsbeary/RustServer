using System;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class PositionLerp : IDisposable
{
	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06001E0E RID: 7694 RVA: 0x000CD281 File Offset: 0x000CB481
	// (set) Token: 0x06001E0F RID: 7695 RVA: 0x000CD289 File Offset: 0x000CB489
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			this.enabled = value;
			if (this.enabled)
			{
				this.OnEnable();
				return;
			}
			this.OnDisable();
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001E10 RID: 7696 RVA: 0x000CD2A7 File Offset: 0x000CB4A7
	public static float LerpTime
	{
		get
		{
			return Time.time;
		}
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000CD2AE File Offset: 0x000CB4AE
	private void OnEnable()
	{
		PositionLerp.InstanceList.Add(this);
		this.enabledTime = PositionLerp.LerpTime;
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000CD2C6 File Offset: 0x000CB4C6
	private void OnDisable()
	{
		PositionLerp.InstanceList.Remove(this);
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x000CD2D4 File Offset: 0x000CB4D4
	public void Initialize(IPosLerpTarget target)
	{
		this.target = target;
		this.Enabled = true;
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000CD2E4 File Offset: 0x000CB4E4
	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float num2 = PositionLerp.LerpTime;
		this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - serverTime);
		this.timeOffsetCount++;
		if (this.timeOffsetCount >= PositionLerp.TimeOffsetInterval / 4)
		{
			this.timeOffset3 = this.timeOffset2;
			this.timeOffset2 = this.timeOffset1;
			this.timeOffset1 = this.timeOffset0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffsetCount = 0;
		}
		PositionLerp.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
		num2 = serverTime + PositionLerp.TimeOffset;
		if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && serverTime < this.lastServerTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: server time ",
				serverTime,
				" < ",
				this.lastServerTime
			}));
		}
		else if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && num2 < this.lastClientTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: client time ",
				num2,
				" < ",
				this.lastClientTime
			}));
		}
		else
		{
			this.lastClientTime = num2;
			this.lastServerTime = serverTime;
			this.interpolator.Add(new TransformSnapshot(num2, position, rotation));
		}
		this.interpolator.Cull(num2 - num);
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000CD4AE File Offset: 0x000CB6AE
	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		this.Snapshot(position, rotation, PositionLerp.LerpTime - PositionLerp.TimeOffset);
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000CD4C3 File Offset: 0x000CB6C3
	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		this.interpolator.Clear();
		this.Snapshot(position, rotation, serverTime);
		this.target.SetNetworkPosition(position);
		this.target.SetNetworkRotation(rotation);
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000CD4F1 File Offset: 0x000CB6F1
	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		this.interpolator.last = new TransformSnapshot(PositionLerp.LerpTime, position, rotation);
		this.Wipe();
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x000CD510 File Offset: 0x000CB710
	public void SnapToEnd()
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, 0f, 0f, ref PositionLerp.snapshotPrototype);
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		this.Wipe();
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x000CD57C File Offset: 0x000CB77C
	public void Wipe()
	{
		this.interpolator.Clear();
		this.timeOffsetCount = 0;
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x000CD5BC File Offset: 0x000CB7BC
	public static void WipeAll()
	{
		foreach (PositionLerp positionLerp in PositionLerp.InstanceList)
		{
			positionLerp.Wipe();
		}
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x000CD60C File Offset: 0x000CB80C
	protected void DoCycle()
	{
		if (this.target == null)
		{
			return;
		}
		float interpolationInertia = this.target.GetInterpolationInertia();
		float num = ((interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, PositionLerp.LerpTime - this.enabledTime) : 1f);
		float extrapolationTime = this.target.GetExtrapolationTime();
		float num2 = this.target.GetInterpolationDelay() * num;
		float num3 = this.target.GetInterpolationSmoothing() * num;
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, num2, extrapolationTime, num3, ref PositionLerp.snapshotPrototype);
		if (segment.next.Time >= this.interpolator.last.Time)
		{
			this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		else
		{
			this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
		}
		if (this.extrapolatedTime > 0f && extrapolationTime > 0f && num3 > 0f)
		{
			float num4 = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * num3);
			segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, num4);
			segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, num4);
		}
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		if (PositionLerp.DebugDraw)
		{
			this.target.DrawInterpolationState(segment, this.interpolator.list);
		}
		if (PositionLerp.LerpTime - this.lastClientTime > 10f)
		{
			if (this.idleDisable == null)
			{
				this.idleDisable = new Action(this.target.LerpIdleDisable);
			}
			InvokeHandler.Invoke(this.target as Behaviour, this.idleDisable, 0f);
		}
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x000CD810 File Offset: 0x000CBA10
	public void TransformEntries(Matrix4x4 matrix)
	{
		Quaternion rotation = matrix.rotation;
		for (int i = 0; i < this.interpolator.list.Count; i++)
		{
			TransformSnapshot transformSnapshot = this.interpolator.list[i];
			transformSnapshot.pos = matrix.MultiplyPoint3x4(transformSnapshot.pos);
			transformSnapshot.rot = rotation * transformSnapshot.rot;
			this.interpolator.list[i] = transformSnapshot;
		}
		this.interpolator.last.pos = matrix.MultiplyPoint3x4(this.interpolator.last.pos);
		this.interpolator.last.rot = rotation * this.interpolator.last.rot;
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000CD8D8 File Offset: 0x000CBAD8
	public Quaternion GetEstimatedAngularVelocity()
	{
		if (this.target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((prev.rot.eulerAngles - next.rot.eulerAngles) / (prev.Time - next.Time));
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000CD98C File Offset: 0x000CBB8C
	public Vector3 GetEstimatedVelocity()
	{
		if (this.target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Vector3.zero;
		}
		return (prev.pos - next.pos) / (prev.Time - next.Time);
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000CDA30 File Offset: 0x000CBC30
	public void Dispose()
	{
		this.target = null;
		this.idleDisable = null;
		this.interpolator.Clear();
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
		this.lastClientTime = 0f;
		this.lastServerTime = 0f;
		this.extrapolatedTime = 0f;
		this.timeOffsetCount = 0;
		this.Enabled = false;
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x000CDAB1 File Offset: 0x000CBCB1
	public static void Clear()
	{
		PositionLerp.InstanceList.Clear();
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x000CDAC0 File Offset: 0x000CBCC0
	public static void Cycle()
	{
		PositionLerp[] buffer = PositionLerp.InstanceList.Values.Buffer;
		int count = PositionLerp.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	// Token: 0x040016FE RID: 5886
	private static ListHashSet<PositionLerp> InstanceList = new ListHashSet<PositionLerp>(8);

	// Token: 0x040016FF RID: 5887
	public static bool DebugLog = false;

	// Token: 0x04001700 RID: 5888
	public static bool DebugDraw = false;

	// Token: 0x04001701 RID: 5889
	public static int TimeOffsetInterval = 16;

	// Token: 0x04001702 RID: 5890
	public static float TimeOffset = 0f;

	// Token: 0x04001703 RID: 5891
	public const int TimeOffsetIntervalMin = 4;

	// Token: 0x04001704 RID: 5892
	public const int TimeOffsetIntervalMax = 64;

	// Token: 0x04001705 RID: 5893
	private bool enabled = true;

	// Token: 0x04001706 RID: 5894
	private Action idleDisable;

	// Token: 0x04001707 RID: 5895
	private Interpolator<TransformSnapshot> interpolator = new Interpolator<TransformSnapshot>(32);

	// Token: 0x04001708 RID: 5896
	private IPosLerpTarget target;

	// Token: 0x04001709 RID: 5897
	private static TransformSnapshot snapshotPrototype = default(TransformSnapshot);

	// Token: 0x0400170A RID: 5898
	private float timeOffset0 = float.MaxValue;

	// Token: 0x0400170B RID: 5899
	private float timeOffset1 = float.MaxValue;

	// Token: 0x0400170C RID: 5900
	private float timeOffset2 = float.MaxValue;

	// Token: 0x0400170D RID: 5901
	private float timeOffset3 = float.MaxValue;

	// Token: 0x0400170E RID: 5902
	private int timeOffsetCount;

	// Token: 0x0400170F RID: 5903
	private float lastClientTime;

	// Token: 0x04001710 RID: 5904
	private float lastServerTime;

	// Token: 0x04001711 RID: 5905
	private float extrapolatedTime;

	// Token: 0x04001712 RID: 5906
	private float enabledTime;
}
