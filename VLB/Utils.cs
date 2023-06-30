using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009BF RID: 2495
	public static class Utils
	{
		// Token: 0x06003B51 RID: 15185 RVA: 0x0015EE44 File Offset: 0x0015D044
		public static string GetPath(Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return Utils.GetPath(current.parent) + "/" + current.name;
		}

		// Token: 0x06003B52 RID: 15186 RVA: 0x0015EE80 File Offset: 0x0015D080
		public static T NewWithComponent<T>(string name) where T : Component
		{
			return new GameObject(name, new Type[] { typeof(T) }).GetComponent<T>();
		}

		// Token: 0x06003B53 RID: 15187 RVA: 0x0015EEA0 File Offset: 0x0015D0A0
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			T t = self.GetComponent<T>();
			if (t == null)
			{
				t = self.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06003B54 RID: 15188 RVA: 0x0015EECA File Offset: 0x0015D0CA
		public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
		{
			return self.gameObject.GetOrAddComponent<T>();
		}

		// Token: 0x06003B55 RID: 15189 RVA: 0x0015EED7 File Offset: 0x0015D0D7
		public static bool HasFlag(this Enum mask, Enum flags)
		{
			return ((int)mask & (int)flags) == (int)flags;
		}

		// Token: 0x06003B56 RID: 15190 RVA: 0x0015EEEE File Offset: 0x0015D0EE
		public static Vector2 xy(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.y);
		}

		// Token: 0x06003B57 RID: 15191 RVA: 0x0015EF01 File Offset: 0x0015D101
		public static Vector2 xz(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.z);
		}

		// Token: 0x06003B58 RID: 15192 RVA: 0x0015EF14 File Offset: 0x0015D114
		public static Vector2 yz(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.z);
		}

		// Token: 0x06003B59 RID: 15193 RVA: 0x0015EF27 File Offset: 0x0015D127
		public static Vector2 yx(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.x);
		}

		// Token: 0x06003B5A RID: 15194 RVA: 0x0015EF3A File Offset: 0x0015D13A
		public static Vector2 zx(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.x);
		}

		// Token: 0x06003B5B RID: 15195 RVA: 0x0015EF4D File Offset: 0x0015D14D
		public static Vector2 zy(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.y);
		}

		// Token: 0x06003B5C RID: 15196 RVA: 0x0015EF60 File Offset: 0x0015D160
		public static float GetVolumeCubic(this Bounds self)
		{
			return self.size.x * self.size.y * self.size.z;
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x0015EF88 File Offset: 0x0015D188
		public static float GetMaxArea2D(this Bounds self)
		{
			return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z);
		}

		// Token: 0x06003B5E RID: 15198 RVA: 0x0015EFEA File Offset: 0x0015D1EA
		public static Color Opaque(this Color self)
		{
			return new Color(self.r, self.g, self.b, 1f);
		}

		// Token: 0x06003B5F RID: 15199 RVA: 0x0015F008 File Offset: 0x0015D208
		public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
		{
			Vector3 vector = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f) ? Vector3.forward : Vector3.up).normalized * size;
			Vector3 vector2 = position + vector;
			Vector3 vector3 = position - vector;
			vector = Quaternion.AngleAxis(90f, normal) * vector;
			Vector3 vector4 = position + vector;
			Vector3 vector5 = position - vector;
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = color;
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector4, vector5);
			Gizmos.DrawLine(vector2, vector4);
			Gizmos.DrawLine(vector4, vector3);
			Gizmos.DrawLine(vector3, vector5);
			Gizmos.DrawLine(vector5, vector2);
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x0015F0BE File Offset: 0x0015D2BE
		public static Plane TranslateCustom(this Plane plane, Vector3 translation)
		{
			plane.distance += Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
			return plane;
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x0015F0EC File Offset: 0x0015D2EC
		public static bool IsValid(this Plane plane)
		{
			return plane.normal.sqrMagnitude > 0.5f;
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0015F110 File Offset: 0x0015D310
		public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			for (int i = 0; i < 16; i++)
			{
				Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
				matrix4x[i] = color.PackToFloat(floatPackingPrecision);
			}
			return matrix4x;
		}

		// Token: 0x06003B63 RID: 15203 RVA: 0x0015F158 File Offset: 0x0015D358
		public static Color[] SampleInArray(this Gradient self, int samplesCount)
		{
			Color[] array = new Color[samplesCount];
			for (int i = 0; i < samplesCount; i++)
			{
				array[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
			}
			return array;
		}

		// Token: 0x06003B64 RID: 15204 RVA: 0x0015F192 File Offset: 0x0015D392
		private static Vector4 Vector4_Floor(Vector4 vec)
		{
			return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
		}

		// Token: 0x06003B65 RID: 15205 RVA: 0x0015F1C8 File Offset: 0x0015D3C8
		public static float PackToFloat(this Color color, int floatPackingPrecision)
		{
			Vector4 vector = Utils.Vector4_Floor(color * (float)(floatPackingPrecision - 1));
			return 0f + vector.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.y * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.z * (float)floatPackingPrecision + vector.w;
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x0015F21D File Offset: 0x0015D41D
		public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
		{
			if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
			{
				Utils.ms_FloatPackingPrecision = ((SystemInfo.graphicsShaderLevel >= 35) ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low);
			}
			return Utils.ms_FloatPackingPrecision;
		}

		// Token: 0x06003B67 RID: 15207 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void MarkCurrentSceneDirty()
		{
		}

		// Token: 0x04003652 RID: 13906
		private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;

		// Token: 0x04003653 RID: 13907
		private const int kFloatPackingHighMinShaderLevel = 35;

		// Token: 0x02000EF4 RID: 3828
		public enum FloatPackingPrecision
		{
			// Token: 0x04004E18 RID: 19992
			High = 64,
			// Token: 0x04004E19 RID: 19993
			Low = 8,
			// Token: 0x04004E1A RID: 19994
			Undef = 0
		}
	}
}
