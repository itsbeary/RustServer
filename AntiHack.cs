using System;
using System.Collections.Generic;
using ConVar;
using Epic.OnlineServices.Reports;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000741 RID: 1857
public static class AntiHack
{
	// Token: 0x0600337A RID: 13178 RVA: 0x0013B568 File Offset: 0x00139768
	public static bool TestNoClipping(Vector3 oldPos, Vector3 newPos, float radius, float backtracking, bool sphereCast, out Collider collider, bool vehicleLayer = false, BaseEntity ignoreEntity = null)
	{
		int num = 429990145;
		if (!vehicleLayer)
		{
			num &= -8193;
		}
		Vector3 normalized = (newPos - oldPos).normalized;
		Vector3 vector = oldPos - normalized * backtracking;
		float magnitude = (newPos - vector).magnitude;
		Ray ray = new Ray(vector, normalized);
		RaycastHit raycastHit;
		bool flag = ((ignoreEntity == null) ? UnityEngine.Physics.Raycast(ray, out raycastHit, magnitude + radius, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, 0f, out raycastHit, magnitude + radius, num, QueryTriggerInteraction.Ignore, ignoreEntity));
		if (!flag && sphereCast)
		{
			flag = ((ignoreEntity == null) ? UnityEngine.Physics.SphereCast(ray, radius, out raycastHit, magnitude, num, QueryTriggerInteraction.Ignore) : GamePhysics.Trace(ray, radius, out raycastHit, magnitude, num, QueryTriggerInteraction.Ignore, ignoreEntity));
		}
		collider = raycastHit.collider;
		return flag && GamePhysics.Verify(raycastHit, null);
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x0013B640 File Offset: 0x00139840
	public static void Cycle()
	{
		float num = UnityEngine.Time.unscaledTime - 60f;
		if (global::AntiHack.groupedLogs.Count > 0)
		{
			global::AntiHack.GroupedLog groupedLog = global::AntiHack.groupedLogs.Peek();
			while (groupedLog.firstLogTime <= num)
			{
				global::AntiHack.GroupedLog groupedLog2 = global::AntiHack.groupedLogs.Dequeue();
				global::AntiHack.LogToConsole(groupedLog2.playerName, groupedLog2.antiHackType, string.Format("{0} (x{1})", groupedLog2.message, groupedLog2.num), groupedLog2.averagePos);
				Facepunch.Pool.Free<global::AntiHack.GroupedLog>(ref groupedLog2);
				if (global::AntiHack.groupedLogs.Count == 0)
				{
					break;
				}
				groupedLog = global::AntiHack.groupedLogs.Peek();
			}
		}
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x0013B6D7 File Offset: 0x001398D7
	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x0013B6E4 File Offset: 0x001398E4
	public static bool ShouldIgnore(BasePlayer ply)
	{
		bool flag;
		using (TimeWarning.New("AntiHack.ShouldIgnore", 0))
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			else if ((ply.IsAdmin || ply.IsDeveloper) && ply.lastAdminCheatTime == 0f)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(2f))
				{
					return true;
				}
			}
			if (ply.IsSpectating())
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		return flag;
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x0013B7C0 File Offset: 0x001399C0
	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool flag;
		using (TimeWarning.New("AntiHack.ValidateMove", 0))
		{
			if (global::AntiHack.ShouldIgnore(ply))
			{
				flag = true;
			}
			else
			{
				bool flag2 = deltaTime > ConVar.AntiHack.maxdeltatime;
				Collider collider;
				if (global::AntiHack.IsNoClipping(ply, ticks, deltaTime, out collider))
				{
					if (flag2)
					{
						return false;
					}
					Analytics.Azure.OnNoclipViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count, collider);
					global::AntiHack.AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
					if (ConVar.AntiHack.noclip_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsSpeeding(ply, ticks, deltaTime))
				{
					if (flag2)
					{
						return false;
					}
					Analytics.Azure.OnSpeedhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
					global::AntiHack.AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
					if (ConVar.AntiHack.speedhack_reject)
					{
						return false;
					}
				}
				if (global::AntiHack.IsFlying(ply, ticks, deltaTime))
				{
					if (flag2)
					{
						return false;
					}
					Analytics.Azure.OnFlyhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
					global::AntiHack.AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
					if (ConVar.AntiHack.flyhack_reject)
					{
						return false;
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x0013B8F4 File Offset: 0x00139AF4
	public static void ValidateEyeHistory(BasePlayer ply)
	{
		using (TimeWarning.New("AntiHack.ValidateEyeHistory", 0))
		{
			for (int i = 0; i < ply.eyeHistory.Count; i++)
			{
				Vector3 vector = ply.eyeHistory[i];
				if (ply.tickHistory.Distance(ply, vector) > ConVar.AntiHack.eye_history_forgiveness)
				{
					global::AntiHack.AddViolation(ply, AntiHackType.EyeHack, ConVar.AntiHack.eye_history_penalty);
					Analytics.Azure.OnEyehackViolation(ply, vector);
				}
			}
			ply.eyeHistory.Clear();
		}
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x0013B980 File Offset: 0x00139B80
	public static bool IsInsideTerrain(BasePlayer ply)
	{
		return global::AntiHack.TestInsideTerrain(ply.transform.position);
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x0013B994 File Offset: 0x00139B94
	public static bool TestInsideTerrain(Vector3 pos)
	{
		bool flag;
		using (TimeWarning.New("AntiHack.TestInsideTerrain", 0))
		{
			if (!TerrainMeta.Terrain)
			{
				flag = false;
			}
			else if (!TerrainMeta.HeightMap)
			{
				flag = false;
			}
			else if (!TerrainMeta.Collision)
			{
				flag = false;
			}
			else
			{
				float terrain_padding = ConVar.AntiHack.terrain_padding;
				float height = TerrainMeta.HeightMap.GetHeight(pos);
				if (pos.y > height - terrain_padding)
				{
					flag = false;
				}
				else
				{
					float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(pos);
					if (pos.y > num - terrain_padding)
					{
						flag = false;
					}
					else if (TerrainMeta.Collision.GetIgnore(pos, 0.01f))
					{
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x0013BA68 File Offset: 0x00139C68
	public static bool IsInsideMesh(Vector3 pos)
	{
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		if (UnityEngine.Physics.Raycast(pos, Vector3.up, out global::AntiHack.isInsideRayHit, 50f, 65537))
		{
			UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
			return Vector3.Dot(Vector3.up, global::AntiHack.isInsideRayHit.normal) > 0f;
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x0013BAC8 File Offset: 0x00139CC8
	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime, out Collider collider)
	{
		collider = null;
		bool flag;
		using (TimeWarning.New("AntiHack.IsNoClipping", 0))
		{
			ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
			if (ConVar.AntiHack.noclip_protection <= 0)
			{
				flag = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					flag = false;
				}
				else
				{
					bool flag2 = ply.transform.parent == null;
					Matrix4x4 matrix4x = (flag2 ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
					Vector3 vector = (flag2 ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint));
					Vector3 vector2 = (flag2 ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint));
					Vector3 vector3 = ply.NoClipOffset();
					float num = ply.NoClipRadius(ConVar.AntiHack.noclip_margin);
					float noclip_backtracking = ConVar.AntiHack.noclip_backtracking;
					bool flag3 = ply.vehiclePauseTime <= 0f;
					if (ConVar.AntiHack.noclip_protection >= 3)
					{
						float num2 = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
						int num3 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
						num2 = Mathf.Max(ticks.Length / (float)num3, num2);
						while (ticks.MoveNext(num2))
						{
							vector2 = (flag2 ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestNoClipping(vector + vector3, vector2 + vector3, num, noclip_backtracking, true, out collider, flag3, null))
							{
								return true;
							}
							vector = vector2;
						}
					}
					else if (ConVar.AntiHack.noclip_protection >= 2)
					{
						if (global::AntiHack.TestNoClipping(vector + vector3, vector2 + vector3, num, noclip_backtracking, true, out collider, flag3, null))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestNoClipping(vector + vector3, vector2 + vector3, num, noclip_backtracking, false, out collider, flag3, null))
					{
						return true;
					}
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x0013BCC0 File Offset: 0x00139EC0
	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool flag;
		using (TimeWarning.New("AntiHack.IsSpeeding", 0))
		{
			ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
			if (ConVar.AntiHack.speedhack_protection <= 0)
			{
				flag = false;
			}
			else
			{
				bool flag2 = ply.transform.parent == null;
				Matrix4x4 matrix4x = (flag2 ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
				Vector3 vector = (flag2 ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint));
				Vector3 vector2 = (flag2 ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint));
				float num = 1f;
				float num2 = 0f;
				float num3 = 0f;
				if (ConVar.AntiHack.speedhack_protection >= 2)
				{
					bool flag3 = ply.IsRunning();
					bool flag4 = ply.IsDucked();
					bool flag5 = ply.IsSwimming();
					bool flag6 = ply.IsCrawling();
					num = (flag3 ? 1f : 0f);
					num2 = ((flag4 || flag5) ? 1f : 0f);
					num3 = (flag6 ? 1f : 0f);
				}
				float speed = ply.GetSpeed(num, num2, num3);
				Vector3 vector3 = vector2 - vector;
				float num4 = vector3.Magnitude2D();
				float num5 = deltaTime * speed;
				if (num4 > num5)
				{
					Vector3 vector4 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetNormal(vector) : Vector3.up);
					float num6 = Mathf.Max(0f, Vector3.Dot(vector4.XZ3D(), vector3.XZ3D())) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
					num4 = Mathf.Max(0f, num4 - num6);
				}
				float num7 = Mathf.Max((ply.speedhackPauseTime > 0f) ? ConVar.AntiHack.speedhack_forgiveness_inertia : ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				float num8 = num7 + Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance, -num8, num8);
				ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance - num5, -num8, num8);
				if (ply.speedhackDistance > num7)
				{
					flag = true;
				}
				else
				{
					ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + num4, -num8, num8);
					if (ply.speedhackDistance > num7)
					{
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
		}
		return flag;
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x0013BF1C File Offset: 0x0013A11C
	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool flag;
		using (TimeWarning.New("AntiHack.IsFlying", 0))
		{
			ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
			if (ConVar.AntiHack.flyhack_protection <= 0)
			{
				flag = false;
			}
			else
			{
				ticks.Reset();
				if (!ticks.HasNext())
				{
					flag = false;
				}
				else
				{
					bool flag2 = ply.transform.parent == null;
					Matrix4x4 matrix4x = (flag2 ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
					Vector3 vector = (flag2 ? ticks.StartPoint : matrix4x.MultiplyPoint3x4(ticks.StartPoint));
					Vector3 vector2 = (flag2 ? ticks.EndPoint : matrix4x.MultiplyPoint3x4(ticks.EndPoint));
					if (ConVar.AntiHack.flyhack_protection >= 3)
					{
						float num = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
						int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
						num = Mathf.Max(ticks.Length / (float)num2, num);
						while (ticks.MoveNext(num))
						{
							vector2 = (flag2 ? ticks.CurrentPoint : matrix4x.MultiplyPoint3x4(ticks.CurrentPoint));
							if (global::AntiHack.TestFlying(ply, vector, vector2, true))
							{
								return true;
							}
							vector = vector2;
						}
					}
					else if (ConVar.AntiHack.flyhack_protection >= 2)
					{
						if (global::AntiHack.TestFlying(ply, vector, vector2, true))
						{
							return true;
						}
					}
					else if (global::AntiHack.TestFlying(ply, vector, vector2, false))
					{
						return true;
					}
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x0013C0A0 File Offset: 0x0013A2A0
	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (verifyGrounded)
		{
			float flyhack_extrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 vector = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(vector - new Vector3(0f, flyhack_extrusion, 0f), true, true, ply) && (EnvironmentManager.Get(vector) & EnvironmentType.Elevator) == (EnvironmentType)0)
			{
				float flyhack_margin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(false);
				Vector3 vector2 = vector + new Vector3(0f, radius - flyhack_extrusion, 0f);
				Vector3 vector3 = vector + new Vector3(0f, height - radius, 0f);
				float num = radius - flyhack_margin;
				ply.isInAir = !UnityEngine.Physics.CheckCapsule(vector2, vector3, num, 1503731969, QueryTriggerInteraction.Ignore);
				if (ply.isInAir)
				{
					int num2 = UnityEngine.Physics.OverlapCapsuleNonAlloc(vector2, vector3, num, global::AntiHack.buffer, 131072, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < num2; i++)
					{
						BasePlayer basePlayer = global::AntiHack.buffer[i].gameObject.ToBaseEntity() as BasePlayer;
						if (!(basePlayer == null) && !(basePlayer == ply) && !basePlayer.isInAir && !basePlayer.isOnPlayer && !basePlayer.TriggeredAntiHack(1f, float.PositiveInfinity) && !basePlayer.IsSleeping())
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int j = 0; j < global::AntiHack.buffer.Length; j++)
					{
						global::AntiHack.buffer[j] = null;
					}
				}
			}
		}
		else
		{
			ply.isInAir = !ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround();
		}
		if (ply.isInAir)
		{
			bool flag = false;
			Vector3 vector4 = newPos - oldPos;
			float num3 = Mathf.Abs(vector4.y);
			float num4 = vector4.Magnitude2D();
			if (vector4.y >= 0f)
			{
				ply.flyhackDistanceVertical += vector4.y;
				flag = true;
			}
			if (num3 < num4)
			{
				ply.flyhackDistanceHorizontal += num4;
				flag = true;
			}
			if (flag)
			{
				float num5 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_vertical_inertia : ConVar.AntiHack.flyhack_forgiveness_vertical, 0f);
				float num6 = ply.GetJumpHeight() + num5;
				if (ply.flyhackDistanceVertical > num6)
				{
					return true;
				}
				float num7 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia : ConVar.AntiHack.flyhack_forgiveness_horizontal, 0f);
				float num8 = 5f + num7;
				if (ply.flyhackDistanceHorizontal > num8)
				{
					return true;
				}
			}
		}
		else
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		return false;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0013C35C File Offset: 0x0013A55C
	public static bool TestIsBuildingInsideSomething(Construction.Target target, Vector3 deployPos)
	{
		if (ConVar.AntiHack.build_inside_check <= 0)
		{
			return false;
		}
		using (List<MonumentInfo>.Enumerator enumerator = TerrainMeta.Path.Monuments.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsInBounds(deployPos))
				{
					return false;
				}
			}
		}
		if (global::AntiHack.IsInsideMesh(deployPos) && global::AntiHack.IsInsideMesh(target.ray.origin))
		{
			global::AntiHack.LogToConsoleBatched(target.player, AntiHackType.InsideGeometry, "Tried to build while clipped inside " + global::AntiHack.isInsideRayHit.collider.name, 25f);
			if (ConVar.AntiHack.build_inside_check > 1)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x0013C414 File Offset: 0x0013A614
	public static void NoteAdminHack(BasePlayer ply)
	{
		global::AntiHack.Ban(ply, "Cheat Detected!");
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x0013C421 File Offset: 0x0013A621
	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (UnityEngine.Time.realtimeSinceStartup - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x0013C454 File Offset: 0x0013A654
	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel <= 0)
		{
			return;
		}
		if (ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				global::AntiHack.LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + ply.violationLevel + ")");
			}
			string text = ply.lastViolationType + " Violation Level " + ply.violationLevel;
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				global::AntiHack.Kick(ply, text);
				return;
			}
			global::AntiHack.Kick(ply, text);
		}
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x0013C4DD File Offset: 0x0013A6DD
	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			global::AntiHack.LogToConsole(ply, type, message);
		}
		Analytics.Azure.OnAntihackViolation(ply, (int)type, message);
		global::AntiHack.LogToEAC(ply, type, message);
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x0013C500 File Offset: 0x0013A700
	public static void LogToConsoleBatched(BasePlayer ply, AntiHackType type, string message, float maxDistance)
	{
		string text = ply.ToString();
		Vector3 position = ply.transform.position;
		using (Queue<global::AntiHack.GroupedLog>.Enumerator enumerator = global::AntiHack.groupedLogs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TryGroup(text, type, message, position, maxDistance))
				{
					return;
				}
			}
		}
		global::AntiHack.GroupedLog groupedLog = Facepunch.Pool.Get<global::AntiHack.GroupedLog>();
		groupedLog.SetInitial(text, type, message, position);
		global::AntiHack.groupedLogs.Enqueue(groupedLog);
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x0013C588 File Offset: 0x0013A788
	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		Debug.LogWarning(string.Concat(new object[]
		{
			ply,
			" ",
			type,
			": ",
			message,
			" at ",
			ply.transform.position
		}));
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x0013C5E4 File Offset: 0x0013A7E4
	private static void LogToConsole(string plyName, AntiHackType type, string message, Vector3 pos)
	{
		Debug.LogWarning(string.Concat(new object[] { plyName, " ", type, ": ", message, " at ", pos }));
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x0013C633 File Offset: 0x0013A833
	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting)
		{
			EACServer.SendPlayerBehaviorReport(PlayerReportsCategory.Exploiting, ply.UserIDString, type + ": " + message);
		}
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x0013C65C File Offset: 0x0013A85C
	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		using (TimeWarning.New("AntiHack.AddViolation", 0))
		{
			ply.lastViolationType = type;
			ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
			ply.violationLevel += amount;
			if ((ConVar.AntiHack.debuglevel >= 2 && amount > 0f) || (ConVar.AntiHack.debuglevel >= 3 && type != AntiHackType.NoClip) || ConVar.AntiHack.debuglevel >= 4)
			{
				global::AntiHack.LogToConsole(ply, type, string.Concat(new object[]
				{
					"Added violation of ",
					amount,
					" in frame ",
					UnityEngine.Time.frameCount,
					" (now has ",
					ply.violationLevel,
					")"
				}));
			}
			global::AntiHack.EnforceViolations(ply);
		}
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x0013C734 File Offset: 0x0013A934
	public static void Kick(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.kicks);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "kick", new object[] { ply.userID, reason });
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x0013C769 File Offset: 0x0013A969
	public static void Ban(BasePlayer ply, string reason)
	{
		global::AntiHack.AddRecord(ply, global::AntiHack.bans);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "ban", new object[] { ply.userID, reason });
	}

	// Token: 0x06003393 RID: 13203 RVA: 0x0013C7A0 File Offset: 0x0013A9A0
	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (records.ContainsKey(ply.userID))
		{
			ulong userID = ply.userID;
			records[userID]++;
			return;
		}
		records.Add(ply.userID, 1);
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x0013C7E2 File Offset: 0x0013A9E2
	public static int GetKickRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.kicks);
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x0013C7EF File Offset: 0x0013A9EF
	public static int GetBanRecord(BasePlayer ply)
	{
		return global::AntiHack.GetRecord(ply, global::AntiHack.bans);
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x0013C7FC File Offset: 0x0013A9FC
	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			return 0;
		}
		return records[ply.userID];
	}

	// Token: 0x04002A64 RID: 10852
	private const int movement_mask = 429990145;

	// Token: 0x04002A65 RID: 10853
	private const int vehicle_mask = 8192;

	// Token: 0x04002A66 RID: 10854
	private const int grounded_mask = 1503731969;

	// Token: 0x04002A67 RID: 10855
	private const int player_mask = 131072;

	// Token: 0x04002A68 RID: 10856
	private static Collider[] buffer = new Collider[4];

	// Token: 0x04002A69 RID: 10857
	private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();

	// Token: 0x04002A6A RID: 10858
	private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();

	// Token: 0x04002A6B RID: 10859
	private const float LOG_GROUP_SECONDS = 60f;

	// Token: 0x04002A6C RID: 10860
	private static Queue<global::AntiHack.GroupedLog> groupedLogs = new Queue<global::AntiHack.GroupedLog>();

	// Token: 0x04002A6D RID: 10861
	public static RaycastHit isInsideRayHit;

	// Token: 0x02000E56 RID: 3670
	private class GroupedLog : Facepunch.Pool.IPooled
	{
		// Token: 0x06005285 RID: 21125 RVA: 0x00008777 File Offset: 0x00006977
		public GroupedLog()
		{
		}

		// Token: 0x06005286 RID: 21126 RVA: 0x001B067A File Offset: 0x001AE87A
		public GroupedLog(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			this.SetInitial(playerName, antiHackType, message, pos);
		}

		// Token: 0x06005287 RID: 21127 RVA: 0x001B068D File Offset: 0x001AE88D
		public void EnterPool()
		{
			this.firstLogTime = 0f;
			this.playerName = string.Empty;
			this.antiHackType = AntiHackType.None;
			this.averagePos = Vector3.zero;
			this.num = 0;
		}

		// Token: 0x06005288 RID: 21128 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06005289 RID: 21129 RVA: 0x001B06BE File Offset: 0x001AE8BE
		public void SetInitial(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			this.firstLogTime = UnityEngine.Time.unscaledTime;
			this.playerName = playerName;
			this.antiHackType = antiHackType;
			this.message = message;
			this.averagePos = pos;
			this.num = 1;
		}

		// Token: 0x0600528A RID: 21130 RVA: 0x001B06F0 File Offset: 0x001AE8F0
		public bool TryGroup(string playerName, AntiHackType antiHackType, string message, Vector3 pos, float maxDistance)
		{
			if (antiHackType != this.antiHackType || playerName != this.playerName || message != this.message)
			{
				return false;
			}
			if (Vector3.SqrMagnitude(this.averagePos - pos) > maxDistance * maxDistance)
			{
				return false;
			}
			Vector3 vector = this.averagePos * (float)this.num;
			this.averagePos = (vector + pos) / (float)(this.num + 1);
			this.num++;
			return true;
		}

		// Token: 0x04004B81 RID: 19329
		public float firstLogTime;

		// Token: 0x04004B82 RID: 19330
		public string playerName;

		// Token: 0x04004B83 RID: 19331
		public AntiHackType antiHackType;

		// Token: 0x04004B84 RID: 19332
		public string message;

		// Token: 0x04004B85 RID: 19333
		public Vector3 averagePos;

		// Token: 0x04004B86 RID: 19334
		public int num;
	}
}
