using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AF8 RID: 2808
	public class FPNativeList<[IsUnmanaged] T> : Pool.IPooled where T : struct, ValueType
	{
		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x060043B2 RID: 17330 RVA: 0x0018EEB5 File Offset: 0x0018D0B5
		public NativeArray<T> Array
		{
			get
			{
				return this._array;
			}
		}

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x060043B3 RID: 17331 RVA: 0x0018EEBD File Offset: 0x0018D0BD
		public int Count
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x170005FD RID: 1533
		public T this[int index]
		{
			get
			{
				return this._array[index];
			}
			set
			{
				this._array[index] = value;
			}
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x0018EEE4 File Offset: 0x0018D0E4
		public void Add(T item)
		{
			this.EnsureCapacity(this._length + 1);
			int length = this._length;
			this._length = length + 1;
			this._array[length] = item;
		}

		// Token: 0x060043B7 RID: 17335 RVA: 0x0018EF1C File Offset: 0x0018D11C
		public void Clear()
		{
			for (int i = 0; i < this._array.Length; i++)
			{
				this._array[i] = default(T);
			}
			this._length = 0;
		}

		// Token: 0x060043B8 RID: 17336 RVA: 0x0018EF5B File Offset: 0x0018D15B
		public void Resize(int count)
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = new NativeArray<T>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._length = count;
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x0018EF8C File Offset: 0x0018D18C
		public void EnsureCapacity(int requiredCapacity)
		{
			if (!this._array.IsCreated || this._array.Length < requiredCapacity)
			{
				int num = Mathf.Max(this._array.Length * 2, requiredCapacity);
				NativeArray<T> nativeArray = new NativeArray<T>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				if (this._array.IsCreated)
				{
					this._array.CopyTo(nativeArray.GetSubArray(0, this._array.Length));
					this._array.Dispose();
				}
				this._array = nativeArray;
			}
		}

		// Token: 0x060043BA RID: 17338 RVA: 0x0018F00F File Offset: 0x0018D20F
		public void EnterPool()
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = default(NativeArray<T>);
			this._length = 0;
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x04003CE0 RID: 15584
		private NativeArray<T> _array;

		// Token: 0x04003CE1 RID: 15585
		private int _length;
	}
}
