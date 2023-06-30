using System;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class CH47FlightTest : MonoBehaviour
{
	// Token: 0x06001899 RID: 6297 RVA: 0x000B7608 File Offset: 0x000B5808
	public void Awake()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x000B7620 File Offset: 0x000B5820
	public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputState_t = default(CH47FlightTest.HelicopterInputState_t);
		helicopterInputState_t.throttle = (Input.GetKey(KeyCode.W) ? 1f : 0f);
		helicopterInputState_t.throttle -= (Input.GetKey(KeyCode.S) ? 1f : 0f);
		helicopterInputState_t.pitch = Input.GetAxis("Mouse Y");
		helicopterInputState_t.roll = -Input.GetAxis("Mouse X");
		helicopterInputState_t.yaw = (Input.GetKey(KeyCode.D) ? 1f : 0f);
		helicopterInputState_t.yaw -= (Input.GetKey(KeyCode.A) ? 1f : 0f);
		helicopterInputState_t.pitch = (float)Mathf.RoundToInt(helicopterInputState_t.pitch);
		helicopterInputState_t.roll = (float)Mathf.RoundToInt(helicopterInputState_t.roll);
		return helicopterInputState_t;
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x000B76F8 File Offset: 0x000B58F8
	public CH47FlightTest.HelicopterInputState_t GetAIInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputState_t = default(CH47FlightTest.HelicopterInputState_t);
		Vector3 vector = Vector3.Cross(Vector3.up, base.transform.right);
		float num = Vector3.Dot(Vector3.Cross(Vector3.up, vector), Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		helicopterInputState_t.yaw = ((num < 0f) ? 1f : 0f);
		helicopterInputState_t.yaw -= ((num > 0f) ? 1f : 0f);
		float num2 = Vector3.Dot(Vector3.up, base.transform.right);
		helicopterInputState_t.roll = ((num2 < 0f) ? 1f : 0f);
		helicopterInputState_t.roll -= ((num2 > 0f) ? 1f : 0f);
		float num3 = Vector3Ex.Distance2D(base.transform.position, this.AIMoveTarget.position);
		float num4 = Vector3.Dot(vector, Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		float num5 = Vector3.Dot(Vector3.up, base.transform.forward);
		if (num3 > 10f)
		{
			helicopterInputState_t.pitch = ((num4 > 0.8f) ? (-0.25f) : 0f);
			helicopterInputState_t.pitch -= ((num4 < -0.8f) ? (-0.25f) : 0f);
			if (num5 < -0.35f)
			{
				helicopterInputState_t.pitch = -1f;
			}
			else if (num5 > 0.35f)
			{
				helicopterInputState_t.pitch = 1f;
			}
		}
		else if (num5 < -0f)
		{
			helicopterInputState_t.pitch = -1f;
		}
		else if (num5 > 0f)
		{
			helicopterInputState_t.pitch = 1f;
		}
		float idealAltitude = this.GetIdealAltitude();
		float y = base.transform.position.y;
		float num6;
		if (y > idealAltitude + CH47FlightTest.altitudeTolerance)
		{
			num6 = -1f;
		}
		else if (y < idealAltitude - CH47FlightTest.altitudeTolerance)
		{
			num6 = 1f;
		}
		else if (num3 > 20f)
		{
			num6 = Mathf.Lerp(0f, 1f, num3 / 20f);
		}
		else
		{
			num6 = 0f;
		}
		Debug.Log("desiredThrottle : " + num6);
		helicopterInputState_t.throttle = num6 * 1f;
		return helicopterInputState_t;
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x000B796B File Offset: 0x000B5B6B
	public float GetIdealAltitude()
	{
		return this.AIMoveTarget.transform.position.y;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000B7984 File Offset: 0x000B5B84
	public void FixedUpdate()
	{
		CH47FlightTest.HelicopterInputState_t aiinputState = this.GetAIInputState();
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, aiinputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(aiinputState.pitch * this.torqueScale.x, aiinputState.yaw * this.torqueScale.y, aiinputState.roll * this.torqueScale.z) * Time.fixedDeltaTime, ForceMode.Force);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime);
		float num = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num2 = Mathf.InverseLerp(this.liftDotMax, 1f, num);
		Vector3 vector = Vector3.up * this.engineThrustMax * 0.5f * this.currentThrottle * num2;
		Vector3 vector2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * (1f - num2);
		float num3 = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(base.transform.up * num3 * num2 * 0.99f, ForceMode.Force);
		this.rigidBody.AddForce(vector, ForceMode.Force);
		this.rigidBody.AddForce(vector2, ForceMode.Force);
		for (int i = 0; i < this.GroundEffects.Length; i++)
		{
			Component component = this.GroundPoints[i];
			Transform transform = this.GroundEffects[i];
			RaycastHit raycastHit;
			if (Physics.Raycast(component.transform.position, Vector3.down, out raycastHit, 50f, 8388608))
			{
				transform.gameObject.SetActive(true);
				transform.transform.position = raycastHit.point + new Vector3(0f, 1f, 0f);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x000B7BE4 File Offset: 0x000B5DE4
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.AIMoveTarget.transform.position, 1f);
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector2 = Vector3.Cross(vector, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * 10f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector2 * 10f);
	}

	// Token: 0x0400113C RID: 4412
	public Rigidbody rigidBody;

	// Token: 0x0400113D RID: 4413
	public float engineThrustMax;

	// Token: 0x0400113E RID: 4414
	public Vector3 torqueScale;

	// Token: 0x0400113F RID: 4415
	public Transform com;

	// Token: 0x04001140 RID: 4416
	public Transform[] GroundPoints;

	// Token: 0x04001141 RID: 4417
	public Transform[] GroundEffects;

	// Token: 0x04001142 RID: 4418
	public float currentThrottle;

	// Token: 0x04001143 RID: 4419
	public float avgThrust;

	// Token: 0x04001144 RID: 4420
	public float liftDotMax = 0.75f;

	// Token: 0x04001145 RID: 4421
	public Transform AIMoveTarget;

	// Token: 0x04001146 RID: 4422
	private static float altitudeTolerance = 1f;

	// Token: 0x02000C4A RID: 3146
	public struct HelicopterInputState_t
	{
		// Token: 0x04004342 RID: 17218
		public float throttle;

		// Token: 0x04004343 RID: 17219
		public float roll;

		// Token: 0x04004344 RID: 17220
		public float yaw;

		// Token: 0x04004345 RID: 17221
		public float pitch;
	}
}
