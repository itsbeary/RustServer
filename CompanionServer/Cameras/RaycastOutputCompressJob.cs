using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A14 RID: 2580
	public struct RaycastOutputCompressJob : IJob
	{
		// Token: 0x06003D5D RID: 15709 RVA: 0x00167CD0 File Offset: 0x00165ED0
		public void Execute()
		{
			int num = this.rayOutputs.Length * 4;
			if (this.data.Length < num)
			{
				throw new InvalidOperationException("Not enough data buffer available to compress rays");
			}
			NativeArray<int> nativeArray = new NativeArray<int>(64, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num2 = 0;
			for (int i = 0; i < this.rayOutputs.Length; i++)
			{
				int num3 = this.rayOutputs[i];
				ushort num4 = RaycastOutputCompressJob.RayDistance(num3);
				byte b = RaycastOutputCompressJob.RayAlignment(num3);
				byte b2 = RaycastOutputCompressJob.RayMaterial(num3);
				int num5 = (int)((num4 / 128 * 3 + (ushort)(b / 16 * 5) + (ushort)(b2 * 7)) & 63);
				int num6 = nativeArray[num5];
				if (num6 == num3)
				{
					this.data[num2++] = (byte)(0 | num5);
				}
				else
				{
					int num7 = (int)(num4 - RaycastOutputCompressJob.RayDistance(num6));
					int num8 = (int)(b - RaycastOutputCompressJob.RayAlignment(num6));
					if (b2 == RaycastOutputCompressJob.RayMaterial(num6) && num7 >= -15 && num7 <= 16 && num8 >= -3 && num8 <= 4)
					{
						this.data[num2++] = (byte)(64 | num5);
						this.data[num2++] = (byte)((num7 + 15 << 3) | (num8 + 3));
					}
					else if (b2 == RaycastOutputCompressJob.RayMaterial(num6) && num8 == 0 && num7 >= -127 && num7 <= 128)
					{
						this.data[num2++] = (byte)(128 | num5);
						this.data[num2++] = (byte)(num7 + 127);
					}
					else if (b2 < 63)
					{
						nativeArray[num5] = num3;
						this.data[num2++] = 192 | b2;
						this.data[num2++] = (byte)(num4 >> 2);
						this.data[num2++] = (byte)(((int)(num4 & 3) << 6) | (int)b);
					}
					else
					{
						nativeArray[num5] = num3;
						this.data[num2++] = byte.MaxValue;
						this.data[num2++] = (byte)(num4 >> 2);
						this.data[num2++] = (byte)(((int)(num4 & 3) << 6) | (int)b);
						this.data[num2++] = b2;
					}
				}
			}
			nativeArray.Dispose();
			this.dataLength[0] = num2;
		}

		// Token: 0x06003D5E RID: 15710 RVA: 0x00167F31 File Offset: 0x00166131
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ushort RayDistance(int ray)
		{
			return (ushort)(ray >> 16);
		}

		// Token: 0x06003D5F RID: 15711 RVA: 0x00167F38 File Offset: 0x00166138
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte RayAlignment(int ray)
		{
			return (byte)(ray >> 8);
		}

		// Token: 0x06003D60 RID: 15712 RVA: 0x00167F3E File Offset: 0x0016613E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte RayMaterial(int ray)
		{
			return (byte)ray;
		}

		// Token: 0x04003787 RID: 14215
		[Unity.Collections.ReadOnly]
		public NativeArray<int> rayOutputs;

		// Token: 0x04003788 RID: 14216
		[WriteOnly]
		public NativeArray<int> dataLength;

		// Token: 0x04003789 RID: 14217
		[WriteOnly]
		public NativeArray<byte> data;
	}
}
