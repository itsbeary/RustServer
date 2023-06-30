using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Rust.Water5
{
	// Token: 0x02000B2D RID: 2861
	public class OceanSimulation : IDisposable
	{
		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x0600453E RID: 17726 RVA: 0x00194F3A File Offset: 0x0019313A
		public int Spectrum0
		{
			get
			{
				return this.spectrum0;
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x0600453F RID: 17727 RVA: 0x00194F42 File Offset: 0x00193142
		public int Spectrum1
		{
			get
			{
				return this.spectrum1;
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06004540 RID: 17728 RVA: 0x00194F4A File Offset: 0x0019314A
		public float SpectrumBlend
		{
			get
			{
				return this.spectrumBlend;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06004541 RID: 17729 RVA: 0x00194F52 File Offset: 0x00193152
		public int Frame0
		{
			get
			{
				return this.frame0;
			}
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06004542 RID: 17730 RVA: 0x00194F5A File Offset: 0x0019315A
		public int Frame1
		{
			get
			{
				return this.frame1;
			}
		}

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06004543 RID: 17731 RVA: 0x00194F62 File Offset: 0x00193162
		public float FrameBlend
		{
			get
			{
				return this.frameBlend;
			}
		}

		// Token: 0x06004544 RID: 17732 RVA: 0x00194F6C File Offset: 0x0019316C
		public OceanSimulation(OceanSettings oceanSettings)
		{
			this.oceanSettings = oceanSettings;
			OceanSimulation.oneOverOctave0Scale = 1f / oceanSettings.octaveScales[0];
			OceanSimulation.beaufortValues = new float[oceanSettings.spectrumSettings.Length];
			for (int i = 0; i < oceanSettings.spectrumSettings.Length; i++)
			{
				OceanSimulation.beaufortValues[i] = oceanSettings.spectrumSettings[i].beaufort;
			}
			this.simData = oceanSettings.LoadSimData();
			this.spectrumRanges = oceanSettings.spectrumRanges;
			this.depthAttenuationFactor = oceanSettings.depthAttenuationFactor;
			this.distanceAttenuationFactor = oceanSettings.distanceAttenuationFactor;
		}

		// Token: 0x06004545 RID: 17733 RVA: 0x00195004 File Offset: 0x00193204
		public void Update(float time, float dt, float beaufort)
		{
			this.currentTime = time % 18f;
			this.deltaTime = dt;
			OceanSimulation.FindFrames(this.currentTime, out this.frame0, out this.frame1, out this.frameBlend);
			OceanSimulation.FindSpectra(beaufort, out this.spectrum0, out this.spectrum1, out this.spectrumBlend);
		}

		// Token: 0x06004546 RID: 17734 RVA: 0x0019505C File Offset: 0x0019325C
		private static void FindSpectra(float beaufort, out int spectrum0, out int spectrum1, out float spectrumT)
		{
			beaufort = Mathf.Clamp(beaufort, 0f, 10f);
			spectrum0 = (spectrum1 = 0);
			spectrumT = 0f;
			for (int i = 1; i < OceanSimulation.beaufortValues.Length; i++)
			{
				float num = OceanSimulation.beaufortValues[i - 1];
				float num2 = OceanSimulation.beaufortValues[i];
				if (beaufort >= num && beaufort <= num2)
				{
					spectrum0 = i - 1;
					spectrum1 = i;
					spectrumT = math.remap(num, num2, 0f, 1f, beaufort);
					return;
				}
			}
		}

		// Token: 0x06004547 RID: 17735 RVA: 0x001950D4 File Offset: 0x001932D4
		public static void FindFrames(float time, out int frame0, out int frame1, out float frameBlend)
		{
			frame0 = (int)math.floor(time * 4f);
			frame1 = (int)math.floor(time * 4f);
			frame1 = (frame1 + 1) % 72;
			frameBlend = math.remap((float)frame0 * 0.25f, (float)(frame0 + 1) * 0.25f, 0f, 1f, time);
		}

		// Token: 0x06004548 RID: 17736 RVA: 0x00195130 File Offset: 0x00193330
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Trace(Ray ray, float maxDist, out Vector3 result)
		{
			float num = Mathf.Lerp(this.spectrumRanges[this.spectrum0], this.spectrumRanges[this.spectrum1], this.spectrumBlend);
			if (num <= 0.1f)
			{
				Plane plane = new Plane(Vector3.up, -0f);
				float num2;
				if (plane.Raycast(ray, out num2) && num2 < maxDist)
				{
					result = ray.GetPoint(num2);
					return true;
				}
				result = Vector3.zero;
				return false;
			}
			else
			{
				float num3 = -num;
				Vector3 point = ray.GetPoint(maxDist);
				if (ray.origin.y > num && point.y > num)
				{
					result = Vector3.zero;
					return false;
				}
				if (ray.origin.y < num3 && point.y < num3)
				{
					result = Vector3.zero;
					return false;
				}
				Vector3 vector = ray.origin;
				Vector3 direction = ray.direction;
				float num4 = 0f;
				float num5 = 2f / (math.abs(direction.y) + 1f);
				result = vector;
				if (direction.y <= -0.99f)
				{
					result.y = this.GetHeight(vector);
					return math.lengthsq(result - vector) < maxDist * maxDist;
				}
				float num6;
				if (vector.y >= num + 0f)
				{
					num6 = (num4 = -(vector.y - num - 0f) / direction.y);
					vector += num6 * direction;
					if (num4 >= maxDist)
					{
						result = Vector3.zero;
						return false;
					}
				}
				int num7 = 0;
				for (;;)
				{
					float height = this.GetHeight(vector);
					num6 = num5 * (vector.y - height - 0f);
					vector += num6 * direction;
					num4 += num6;
					if (num7 >= 16 || num6 < 0.1f)
					{
						goto IL_1DD;
					}
					if (num4 >= maxDist)
					{
						break;
					}
					num7++;
				}
				return false;
				IL_1DD:
				if (num6 < 0.1f)
				{
					result = vector;
					return true;
				}
				if (direction.y < 0f)
				{
					num6 = -(vector.y + num - 0f) / direction.y;
					Vector3 vector2 = vector;
					Vector3 vector3 = vector + num6 * ray.direction;
					for (int i = 0; i < 16; i++)
					{
						vector = (vector2 + vector3) * 0.5f;
						float height2 = this.GetHeight(vector);
						if (vector.y - height2 - 0f > 0f)
						{
							vector2 = vector;
						}
						else
						{
							vector3 = vector;
						}
						if (math.abs(vector.y - height2) < 0.1f)
						{
							vector.y = height2;
							break;
						}
					}
					result = vector;
					return true;
				}
				return false;
			}
		}

		// Token: 0x06004549 RID: 17737 RVA: 0x001953E5 File Offset: 0x001935E5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float MinLevel()
		{
			return -Mathf.Lerp(this.spectrumRanges[this.spectrum0], this.spectrumRanges[this.spectrum1], this.spectrumBlend);
		}

		// Token: 0x0600454A RID: 17738 RVA: 0x0019540D File Offset: 0x0019360D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float MaxLevel()
		{
			return Mathf.Lerp(this.spectrumRanges[this.spectrum0], this.spectrumRanges[this.spectrum1], this.spectrumBlend);
		}

		// Token: 0x0600454B RID: 17739 RVA: 0x00195434 File Offset: 0x00193634
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetHeight(Vector3[,,] simData, Vector3 position, float time, float beaufort, float distAttenFactor, float depthAttenFactor)
		{
			float x = TerrainMeta.Position.x;
			float z = TerrainMeta.Position.z;
			float x2 = TerrainMeta.OneOverSize.x;
			float z2 = TerrainMeta.OneOverSize.z;
			float num = (position.x - x) * x2;
			float num2 = (position.z - z) * z2;
			Vector2 vector = new Vector2(num, num2);
			float num3 = ((TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.GetCoarseDistanceToShore(vector) : 0f);
			float num4 = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(vector) : 0f);
			float num5 = Mathf.Clamp01(num3 / distAttenFactor);
			float num6 = Mathf.Clamp01(Mathf.Abs(num4) / depthAttenFactor);
			Vector3 vector2 = Vector3.zero;
			vector2 = OceanSimulation.GetDisplacement(simData, position, time, beaufort);
			vector2 = OceanSimulation.GetDisplacement(simData, position - vector2, time, beaufort);
			vector2 = OceanSimulation.GetDisplacement(simData, position - vector2, time, beaufort);
			return OceanSimulation.GetDisplacement(simData, position - vector2, time, beaufort).y * num5 * num6;
		}

		// Token: 0x0600454C RID: 17740 RVA: 0x00195540 File Offset: 0x00193740
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetDisplacement(Vector3[,,] simData, Vector3 position, float time, float beaufort)
		{
			int num;
			int num2;
			float num3;
			OceanSimulation.FindFrames(time, out num, out num2, out num3);
			int num4;
			int num5;
			float num6;
			OceanSimulation.FindSpectra(beaufort, out num4, out num5, out num6);
			return OceanSimulation.GetDisplacement(simData, position, num, num2, num3, num4, num5, num6);
		}

		// Token: 0x0600454D RID: 17741 RVA: 0x00195574 File Offset: 0x00193774
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetDisplacement(Vector3[,,] simData, Vector3 position, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
		{
			float num = position.x * OceanSimulation.oneOverOctave0Scale;
			float num2 = position.z * OceanSimulation.oneOverOctave0Scale;
			return OceanSimulation.GetDisplacement(simData, num, num2, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
		}

		// Token: 0x0600454E RID: 17742 RVA: 0x001955B0 File Offset: 0x001937B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetDisplacement(Vector3[,,] simData, float normX, float normZ, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
		{
			normX -= Mathf.Floor(normX);
			normZ -= Mathf.Floor(normZ);
			float num = normX * 256f - 0.5f;
			float num2 = normZ * 256f - 0.5f;
			int num3 = Mathf.FloorToInt(num);
			int num4 = Mathf.FloorToInt(num2);
			float num5 = num - (float)num3;
			float num6 = num2 - (float)num4;
			int num7 = num3;
			int num8 = num4;
			int num9 = num3 + 1;
			int num10 = num4 + 1;
			Vector3 displacement = OceanSimulation.GetDisplacement(simData, num7, num8, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
			Vector3 displacement2 = OceanSimulation.GetDisplacement(simData, num9, num8, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
			Vector3 displacement3 = OceanSimulation.GetDisplacement(simData, num7, num10, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
			Vector3 displacement4 = OceanSimulation.GetDisplacement(simData, num9, num10, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
			Vector3 vector = Vector3.LerpUnclamped(displacement, displacement2, num5);
			Vector3 vector2 = Vector3.LerpUnclamped(displacement3, displacement4, num5);
			return Vector3.LerpUnclamped(vector, vector2, num6);
		}

		// Token: 0x0600454F RID: 17743 RVA: 0x0019568C File Offset: 0x0019388C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetDisplacement(Vector3[,,] simData, int x, int y, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
		{
			int num = x * 256 + y;
			Vector3 vector = Vector3.LerpUnclamped(simData[spectrum0, frame0, num], simData[spectrum1, frame0, num], spectrumBlend);
			Vector3 vector2 = Vector3.LerpUnclamped(simData[spectrum0, frame1, num], simData[spectrum1, frame1, num], spectrumBlend);
			return Vector3.LerpUnclamped(vector, vector2, frameBlend);
		}

		// Token: 0x06004550 RID: 17744 RVA: 0x000063A5 File Offset: 0x000045A5
		public void Dispose()
		{
		}

		// Token: 0x06004551 RID: 17745 RVA: 0x001956E4 File Offset: 0x001938E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeightRaw(Vector3 position)
		{
			Vector3 vector = Vector3.zero;
			vector = this.GetDisplacement(position);
			vector = this.GetDisplacement(position - vector);
			vector = this.GetDisplacement(position - vector);
			return this.GetDisplacement(position - vector).y;
		}

		// Token: 0x06004552 RID: 17746 RVA: 0x00195730 File Offset: 0x00193930
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetDisplacement(Vector3 position)
		{
			float num = position.x * OceanSimulation.oneOverOctave0Scale;
			float num2 = position.z * OceanSimulation.oneOverOctave0Scale;
			return this.GetDisplacement(num, num2);
		}

		// Token: 0x06004553 RID: 17747 RVA: 0x00195760 File Offset: 0x00193960
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetDisplacement(float normX, float normZ)
		{
			normX -= Mathf.Floor(normX);
			normZ -= Mathf.Floor(normZ);
			float num = normX * 256f - 0.5f;
			float num2 = normZ * 256f - 0.5f;
			int num3 = Mathf.FloorToInt(num);
			int num4 = Mathf.FloorToInt(num2);
			float num5 = num - (float)num3;
			float num6 = num2 - (float)num4;
			int num7 = (num3 + 256) % 256;
			int num8 = (num4 + 256) % 256;
			int num9 = (num3 + 1 + 256) % 256;
			int num10 = (num4 + 1 + 256) % 256;
			Vector3 displacement = this.GetDisplacement(num7, num8);
			Vector3 displacement2 = this.GetDisplacement(num9, num8);
			Vector3 displacement3 = this.GetDisplacement(num7, num10);
			Vector3 displacement4 = this.GetDisplacement(num9, num10);
			Vector3 vector = Vector3.LerpUnclamped(displacement, displacement2, num5);
			Vector3 vector2 = Vector3.LerpUnclamped(displacement3, displacement4, num5);
			return Vector3.LerpUnclamped(vector, vector2, num6);
		}

		// Token: 0x06004554 RID: 17748 RVA: 0x00195840 File Offset: 0x00193A40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetDisplacement(int x, int z)
		{
			int num = x * 256 + z;
			Vector3 vector = Vector3.LerpUnclamped(this.simData[this.spectrum0, this.frame0, num], this.simData[this.spectrum1, this.frame0, num], this.spectrumBlend);
			Vector3 vector2 = Vector3.LerpUnclamped(this.simData[this.spectrum0, this.frame1, num], this.simData[this.spectrum1, this.frame1, num], this.spectrumBlend);
			return Vector3.LerpUnclamped(vector, vector2, this.frameBlend);
		}

		// Token: 0x06004555 RID: 17749 RVA: 0x001958F0 File Offset: 0x00193AF0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float GetHeight(Vector3 position)
		{
			float x = TerrainMeta.Position.x;
			float z = TerrainMeta.Position.z;
			float x2 = TerrainMeta.OneOverSize.x;
			float z2 = TerrainMeta.OneOverSize.z;
			float num = (position.x - x) * x2;
			float num2 = (position.z - z) * z2;
			Vector2 vector = new Vector2(num, num2);
			float num3 = ((TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.GetCoarseDistanceToShore(vector) : 0f);
			float num4 = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(vector) : 0f);
			float num5 = Mathf.Clamp01(num3 / this.distanceAttenuationFactor);
			float num6 = Mathf.Clamp01(Mathf.Abs(num4) / this.depthAttenuationFactor);
			return this.GetHeightRaw(position) * num5 * num6;
		}

		// Token: 0x04003E1C RID: 15900
		public const int octaveCount = 3;

		// Token: 0x04003E1D RID: 15901
		public const int simulationSize = 256;

		// Token: 0x04003E1E RID: 15902
		public const int physicsSimulationSize = 256;

		// Token: 0x04003E1F RID: 15903
		public const int physicsFrameRate = 4;

		// Token: 0x04003E20 RID: 15904
		public const int physicsLooptime = 18;

		// Token: 0x04003E21 RID: 15905
		public const int physicsFrameCount = 72;

		// Token: 0x04003E22 RID: 15906
		public const float phsyicsDeltaTime = 0.25f;

		// Token: 0x04003E23 RID: 15907
		public const float oneOverPhysicsSimulationSize = 0.00390625f;

		// Token: 0x04003E24 RID: 15908
		public const int physicsFrameSize = 65536;

		// Token: 0x04003E25 RID: 15909
		public const int physicsSpectrumOffset = 4718592;

		// Token: 0x04003E26 RID: 15910
		private OceanSettings oceanSettings;

		// Token: 0x04003E27 RID: 15911
		private float[] spectrumRanges;

		// Token: 0x04003E28 RID: 15912
		private float distanceAttenuationFactor;

		// Token: 0x04003E29 RID: 15913
		private float depthAttenuationFactor;

		// Token: 0x04003E2A RID: 15914
		private static float oneOverOctave0Scale;

		// Token: 0x04003E2B RID: 15915
		private static float[] beaufortValues;

		// Token: 0x04003E2C RID: 15916
		private int spectrum0;

		// Token: 0x04003E2D RID: 15917
		private int spectrum1;

		// Token: 0x04003E2E RID: 15918
		private float spectrumBlend;

		// Token: 0x04003E2F RID: 15919
		private int frame0;

		// Token: 0x04003E30 RID: 15920
		private int frame1;

		// Token: 0x04003E31 RID: 15921
		private float frameBlend;

		// Token: 0x04003E32 RID: 15922
		private float currentTime;

		// Token: 0x04003E33 RID: 15923
		private float prevUpdateComputeTime;

		// Token: 0x04003E34 RID: 15924
		private float deltaTime;

		// Token: 0x04003E35 RID: 15925
		public OceanDisplacementShort3[,,] simData;
	}
}
