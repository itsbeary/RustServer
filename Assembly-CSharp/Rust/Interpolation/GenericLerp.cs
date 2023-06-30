using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B40 RID: 2880
	public class GenericLerp<T> : IDisposable where T : ISnapshot<T>, new()
	{
		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060045AC RID: 17836 RVA: 0x0019697D File Offset: 0x00194B7D
		private int TimeOffsetInterval
		{
			get
			{
				return PositionLerp.TimeOffsetInterval;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x060045AD RID: 17837 RVA: 0x00196984 File Offset: 0x00194B84
		private float LerpTime
		{
			get
			{
				return PositionLerp.LerpTime;
			}
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x0019698C File Offset: 0x00194B8C
		public GenericLerp(IGenericLerpTarget<T> target, int listCount)
		{
			this.target = target;
			this.interpolator = new Interpolator<T>(listCount);
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x001969E0 File Offset: 0x00194BE0
		public void Tick()
		{
			if (this.target == null)
			{
				return;
			}
			float extrapolationTime = this.target.GetExtrapolationTime();
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref GenericLerp<T>.snapshotPrototype);
			if (segment.next.Time >= this.interpolator.last.Time)
			{
				this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
			}
			else
			{
				this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
			}
			if (this.extrapolatedTime > 0f && extrapolationTime > 0f && interpolationSmoothing > 0f)
			{
				float num = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
				segment.tick.Lerp(this.target.GetCurrentState(), segment.tick, num);
			}
			this.target.SetFrom(segment.tick);
		}

		// Token: 0x060045B0 RID: 17840 RVA: 0x00196AFC File Offset: 0x00194CFC
		public void Snapshot(T snapshot)
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			float num = interpolationDelay + interpolationSmoothing + 1f;
			float num2 = this.LerpTime;
			this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - snapshot.Time);
			this.timeOffsetCount++;
			if (this.timeOffsetCount >= this.TimeOffsetInterval / 4)
			{
				this.timeOffset3 = this.timeOffset2;
				this.timeOffset2 = this.timeOffset1;
				this.timeOffset1 = this.timeOffset0;
				this.timeOffset0 = float.MaxValue;
				this.timeOffsetCount = 0;
			}
			GenericLerp<T>.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
			num2 = snapshot.Time + GenericLerp<T>.TimeOffset;
			snapshot.Time = num2;
			this.interpolator.Add(snapshot);
			this.interpolator.Cull(num2 - num);
		}

		// Token: 0x060045B1 RID: 17841 RVA: 0x00196C02 File Offset: 0x00194E02
		public void SnapTo(T snapshot)
		{
			this.interpolator.Clear();
			this.Snapshot(snapshot);
			this.target.SetFrom(snapshot);
		}

		// Token: 0x060045B2 RID: 17842 RVA: 0x00196C22 File Offset: 0x00194E22
		public void SnapToNow(T snapshot)
		{
			snapshot.Time = this.LerpTime;
			this.interpolator.last = snapshot;
			this.Wipe();
		}

		// Token: 0x060045B3 RID: 17843 RVA: 0x00196C4C File Offset: 0x00194E4C
		public void SnapToEnd()
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, 0f, 0f, ref GenericLerp<T>.snapshotPrototype);
			this.target.SetFrom(segment.tick);
			this.Wipe();
		}

		// Token: 0x060045B4 RID: 17844 RVA: 0x00196CA0 File Offset: 0x00194EA0
		public void Dispose()
		{
			this.target = null;
			this.interpolator.Clear();
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
			this.extrapolatedTime = 0f;
			this.timeOffsetCount = 0;
		}

		// Token: 0x060045B5 RID: 17845 RVA: 0x00196CFD File Offset: 0x00194EFD
		private void Wipe()
		{
			this.interpolator.Clear();
			this.timeOffsetCount = 0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
		}

		// Token: 0x04003EA9 RID: 16041
		private Interpolator<T> interpolator;

		// Token: 0x04003EAA RID: 16042
		private IGenericLerpTarget<T> target;

		// Token: 0x04003EAB RID: 16043
		private static T snapshotPrototype = new T();

		// Token: 0x04003EAC RID: 16044
		private static float TimeOffset = 0f;

		// Token: 0x04003EAD RID: 16045
		private float timeOffset0 = float.MaxValue;

		// Token: 0x04003EAE RID: 16046
		private float timeOffset1 = float.MaxValue;

		// Token: 0x04003EAF RID: 16047
		private float timeOffset2 = float.MaxValue;

		// Token: 0x04003EB0 RID: 16048
		private float timeOffset3 = float.MaxValue;

		// Token: 0x04003EB1 RID: 16049
		private int timeOffsetCount;

		// Token: 0x04003EB2 RID: 16050
		private float extrapolatedTime;
	}
}
