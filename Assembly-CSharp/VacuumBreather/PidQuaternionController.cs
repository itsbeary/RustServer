using System;
using UnityEngine;

namespace VacuumBreather
{
	// Token: 0x020009CB RID: 2507
	public class PidQuaternionController
	{
		// Token: 0x06003BB5 RID: 15285 RVA: 0x001603A0 File Offset: 0x0015E5A0
		public PidQuaternionController(float kp, float ki, float kd)
		{
			if (kp < 0f)
			{
				throw new ArgumentOutOfRangeException("kp", "kp must be a non-negative number.");
			}
			if (ki < 0f)
			{
				throw new ArgumentOutOfRangeException("ki", "ki must be a non-negative number.");
			}
			if (kd < 0f)
			{
				throw new ArgumentOutOfRangeException("kd", "kd must be a non-negative number.");
			}
			this._internalController = new PidController[]
			{
				new PidController(kp, ki, kd),
				new PidController(kp, ki, kd),
				new PidController(kp, ki, kd),
				new PidController(kp, ki, kd)
			};
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06003BB6 RID: 15286 RVA: 0x00160433 File Offset: 0x0015E633
		// (set) Token: 0x06003BB7 RID: 15287 RVA: 0x00160444 File Offset: 0x0015E644
		public float Kp
		{
			get
			{
				return this._internalController[0].Kp;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
				}
				this._internalController[0].Kp = value;
				this._internalController[1].Kp = value;
				this._internalController[2].Kp = value;
				this._internalController[3].Kp = value;
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06003BB8 RID: 15288 RVA: 0x001604A1 File Offset: 0x0015E6A1
		// (set) Token: 0x06003BB9 RID: 15289 RVA: 0x001604B0 File Offset: 0x0015E6B0
		public float Ki
		{
			get
			{
				return this._internalController[0].Ki;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
				}
				this._internalController[0].Ki = value;
				this._internalController[1].Ki = value;
				this._internalController[2].Ki = value;
				this._internalController[3].Ki = value;
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06003BBA RID: 15290 RVA: 0x0016050D File Offset: 0x0015E70D
		// (set) Token: 0x06003BBB RID: 15291 RVA: 0x0016051C File Offset: 0x0015E71C
		public float Kd
		{
			get
			{
				return this._internalController[0].Kd;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
				}
				this._internalController[0].Kd = value;
				this._internalController[1].Kd = value;
				this._internalController[2].Kd = value;
				this._internalController[3].Kd = value;
			}
		}

		// Token: 0x06003BBC RID: 15292 RVA: 0x0016057C File Offset: 0x0015E77C
		public static Quaternion MultiplyAsVector(Matrix4x4 matrix, Quaternion quaternion)
		{
			Vector4 vector = new Vector4(quaternion.w, quaternion.x, quaternion.y, quaternion.z);
			Vector4 vector2 = matrix * vector;
			return new Quaternion(vector2.y, vector2.z, vector2.w, vector2.x);
		}

		// Token: 0x06003BBD RID: 15293 RVA: 0x001605CD File Offset: 0x0015E7CD
		public static Quaternion ToEulerAngleQuaternion(Vector3 eulerAngles)
		{
			return new Quaternion(eulerAngles.x, eulerAngles.y, eulerAngles.z, 0f);
		}

		// Token: 0x06003BBE RID: 15294 RVA: 0x001605EC File Offset: 0x0015E7EC
		public Vector3 ComputeRequiredAngularAcceleration(Quaternion currentOrientation, Quaternion desiredOrientation, Vector3 currentAngularVelocity, float deltaTime)
		{
			Quaternion quaternion = QuaternionExtensions.RequiredRotation(currentOrientation, desiredOrientation);
			Quaternion quaternion2 = Quaternion.identity.Subtract(quaternion);
			Quaternion quaternion3 = PidQuaternionController.ToEulerAngleQuaternion(currentAngularVelocity) * quaternion;
			Matrix4x4 matrix4x = new Matrix4x4
			{
				m00 = -quaternion.x * -quaternion.x + -quaternion.y * -quaternion.y + -quaternion.z * -quaternion.z,
				m01 = -quaternion.x * quaternion.w + -quaternion.y * -quaternion.z + -quaternion.z * quaternion.y,
				m02 = -quaternion.x * quaternion.z + -quaternion.y * quaternion.w + -quaternion.z * -quaternion.x,
				m03 = -quaternion.x * -quaternion.y + -quaternion.y * quaternion.x + -quaternion.z * quaternion.w,
				m10 = quaternion.w * -quaternion.x + -quaternion.z * -quaternion.y + quaternion.y * -quaternion.z,
				m11 = quaternion.w * quaternion.w + -quaternion.z * -quaternion.z + quaternion.y * quaternion.y,
				m12 = quaternion.w * quaternion.z + -quaternion.z * quaternion.w + quaternion.y * -quaternion.x,
				m13 = quaternion.w * -quaternion.y + -quaternion.z * quaternion.x + quaternion.y * quaternion.w,
				m20 = quaternion.z * -quaternion.x + quaternion.w * -quaternion.y + -quaternion.x * -quaternion.z,
				m21 = quaternion.z * quaternion.w + quaternion.w * -quaternion.z + -quaternion.x * quaternion.y,
				m22 = quaternion.z * quaternion.z + quaternion.w * quaternion.w + -quaternion.x * -quaternion.x,
				m23 = quaternion.z * -quaternion.y + quaternion.w * quaternion.x + -quaternion.x * quaternion.w,
				m30 = -quaternion.y * -quaternion.x + quaternion.x * -quaternion.y + quaternion.w * -quaternion.z,
				m31 = -quaternion.y * quaternion.w + quaternion.x * -quaternion.z + quaternion.w * quaternion.y,
				m32 = -quaternion.y * quaternion.z + quaternion.x * quaternion.w + quaternion.w * -quaternion.x,
				m33 = -quaternion.y * -quaternion.y + quaternion.x * quaternion.x + quaternion.w * quaternion.w
			};
			Quaternion quaternion4 = this.ComputeOutput(quaternion2, quaternion3, deltaTime);
			quaternion4 = PidQuaternionController.MultiplyAsVector(matrix4x, quaternion4);
			Quaternion quaternion5 = quaternion4.Multiply(-2f) * Quaternion.Inverse(quaternion);
			return new Vector3(quaternion5.x, quaternion5.y, quaternion5.z);
		}

		// Token: 0x06003BBF RID: 15295 RVA: 0x00160998 File Offset: 0x0015EB98
		private Quaternion ComputeOutput(Quaternion error, Quaternion delta, float deltaTime)
		{
			return new Quaternion
			{
				x = this._internalController[0].ComputeOutput(error.x, delta.x, deltaTime),
				y = this._internalController[1].ComputeOutput(error.y, delta.y, deltaTime),
				z = this._internalController[2].ComputeOutput(error.z, delta.z, deltaTime),
				w = this._internalController[3].ComputeOutput(error.w, delta.w, deltaTime)
			};
		}

		// Token: 0x040036BB RID: 14011
		private readonly PidController[] _internalController;
	}
}
