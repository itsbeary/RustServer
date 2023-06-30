using System;
using UnityEngine;

namespace VacuumBreather
{
	// Token: 0x020009CA RID: 2506
	public class PidController
	{
		// Token: 0x06003BAD RID: 15277 RVA: 0x001601F4 File Offset: 0x0015E3F4
		public PidController(float kp, float ki, float kd)
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
			this.Kp = kp;
			this.Ki = ki;
			this.Kd = kd;
			this._integralMax = 1000f / this.Ki;
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06003BAE RID: 15278 RVA: 0x00160276 File Offset: 0x0015E476
		// (set) Token: 0x06003BAF RID: 15279 RVA: 0x0016027E File Offset: 0x0015E47E
		public float Kp
		{
			get
			{
				return this._kp;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
				}
				this._kp = value;
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06003BB0 RID: 15280 RVA: 0x0016029F File Offset: 0x0015E49F
		// (set) Token: 0x06003BB1 RID: 15281 RVA: 0x001602A8 File Offset: 0x0015E4A8
		public float Ki
		{
			get
			{
				return this._ki;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
				}
				this._ki = value;
				this._integralMax = 1000f / this.Ki;
				this._integral = Mathf.Clamp(this._integral, -this._integralMax, this._integralMax);
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06003BB2 RID: 15282 RVA: 0x00160304 File Offset: 0x0015E504
		// (set) Token: 0x06003BB3 RID: 15283 RVA: 0x0016030C File Offset: 0x0015E50C
		public float Kd
		{
			get
			{
				return this._kd;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
				}
				this._kd = value;
			}
		}

		// Token: 0x06003BB4 RID: 15284 RVA: 0x00160330 File Offset: 0x0015E530
		public float ComputeOutput(float error, float delta, float deltaTime)
		{
			this._integral += error * deltaTime;
			this._integral = Mathf.Clamp(this._integral, -this._integralMax, this._integralMax);
			float num = delta / deltaTime;
			return Mathf.Clamp(this.Kp * error + this.Ki * this._integral + this.Kd * num, -1000f, 1000f);
		}

		// Token: 0x040036B5 RID: 14005
		private const float MaxOutput = 1000f;

		// Token: 0x040036B6 RID: 14006
		private float _integralMax;

		// Token: 0x040036B7 RID: 14007
		private float _integral;

		// Token: 0x040036B8 RID: 14008
		private float _kp;

		// Token: 0x040036B9 RID: 14009
		private float _ki;

		// Token: 0x040036BA RID: 14010
		private float _kd;
	}
}
