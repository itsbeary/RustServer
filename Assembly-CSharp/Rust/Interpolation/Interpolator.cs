using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B42 RID: 2882
	public class Interpolator<T> where T : ISnapshot<T>, new()
	{
		// Token: 0x060045BC RID: 17852 RVA: 0x00196D53 File Offset: 0x00194F53
		public Interpolator(int listCount)
		{
			this.list = new List<T>(listCount);
		}

		// Token: 0x060045BD RID: 17853 RVA: 0x00196D67 File Offset: 0x00194F67
		public void Add(T tick)
		{
			this.last = tick;
			this.list.Add(tick);
		}

		// Token: 0x060045BE RID: 17854 RVA: 0x00196D7C File Offset: 0x00194F7C
		public void Cull(float beforeTime)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				T t = this.list[i];
				if (t.Time < beforeTime)
				{
					this.list.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060045BF RID: 17855 RVA: 0x00196DCC File Offset: 0x00194FCC
		public void Clear()
		{
			this.list.Clear();
		}

		// Token: 0x060045C0 RID: 17856 RVA: 0x00196DDC File Offset: 0x00194FDC
		public Interpolator<T>.Segment Query(float time, float interpolation, float extrapolation, float smoothing, ref T t)
		{
			Interpolator<T>.Segment segment = default(Interpolator<T>.Segment);
			if (this.list.Count == 0)
			{
				segment.prev = this.last;
				segment.next = this.last;
				segment.tick = this.last;
				return segment;
			}
			float num = time - interpolation - smoothing * 0.5f;
			float num2 = Mathf.Min(time - interpolation, this.last.Time);
			float num3 = num2 - smoothing;
			T t2 = this.list[0];
			T t3 = this.last;
			T t4 = this.list[0];
			T t5 = this.last;
			foreach (T t6 in this.list)
			{
				if (t6.Time < num3)
				{
					t2 = t6;
				}
				else if (t3.Time >= t6.Time)
				{
					t3 = t6;
				}
				if (t6.Time < num2)
				{
					t4 = t6;
				}
				else if (t5.Time >= t6.Time)
				{
					t5 = t6;
				}
			}
			T @new = t.GetNew();
			if (t3.Time - t2.Time <= Mathf.Epsilon)
			{
				@new.Time = num3;
				@new.MatchValuesTo(t3);
			}
			else
			{
				@new.Time = num3;
				@new.Lerp(t2, t3, (num3 - t2.Time) / (t3.Time - t2.Time));
			}
			segment.prev = @new;
			T new2 = t.GetNew();
			if (t5.Time - t4.Time <= Mathf.Epsilon)
			{
				new2.Time = num2;
				new2.MatchValuesTo(t5);
			}
			else
			{
				new2.Time = num2;
				new2.Lerp(t4, t5, (num2 - t4.Time) / (t5.Time - t4.Time));
			}
			segment.next = new2;
			if (new2.Time - @new.Time <= Mathf.Epsilon)
			{
				segment.prev = new2;
				segment.tick = new2;
				return segment;
			}
			if (num - new2.Time > extrapolation)
			{
				segment.prev = new2;
				segment.tick = new2;
				return segment;
			}
			T new3 = t.GetNew();
			new3.Time = num;
			new3.Lerp(@new, new2, Mathf.Min(num - @new.Time, new2.Time + extrapolation - @new.Time) / (new2.Time - @new.Time));
			segment.tick = new3;
			return segment;
		}

		// Token: 0x04003EB3 RID: 16051
		public List<T> list;

		// Token: 0x04003EB4 RID: 16052
		public T last;

		// Token: 0x02000FAC RID: 4012
		public struct Segment
		{
			// Token: 0x04005102 RID: 20738
			public T tick;

			// Token: 0x04005103 RID: 20739
			public T prev;

			// Token: 0x04005104 RID: 20740
			public T next;
		}
	}
}
