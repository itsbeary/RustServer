﻿using System;
using Network;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;

// Token: 0x02000927 RID: 2343
public static class NetworkWriteEx
{
	// Token: 0x06003848 RID: 14408 RVA: 0x0014EF70 File Offset: 0x0014D170
	public static void WriteObject<T>(this NetWrite write, T obj)
	{
		if (typeof(T) == typeof(Vector3))
		{
			Vector3 vector = GenericsUtil.Cast<T, Vector3>(obj);
			write.Vector3(vector);
			return;
		}
		if (typeof(T) == typeof(Ray))
		{
			Ray ray = GenericsUtil.Cast<T, Ray>(obj);
			write.Ray(ray);
			return;
		}
		if (typeof(T) == typeof(float))
		{
			write.Float(GenericsUtil.Cast<T, float>(obj));
			return;
		}
		if (typeof(T) == typeof(short))
		{
			write.Int16(GenericsUtil.Cast<T, short>(obj));
			return;
		}
		if (typeof(T) == typeof(ushort))
		{
			write.UInt16(GenericsUtil.Cast<T, ushort>(obj));
			return;
		}
		if (typeof(T) == typeof(int))
		{
			write.Int32(GenericsUtil.Cast<T, int>(obj));
			return;
		}
		if (typeof(T) == typeof(uint))
		{
			write.UInt32(GenericsUtil.Cast<T, uint>(obj));
			return;
		}
		if (typeof(T) == typeof(byte[]))
		{
			write.Bytes(GenericsUtil.Cast<T, byte[]>(obj));
			return;
		}
		if (typeof(T) == typeof(long))
		{
			write.Int64(GenericsUtil.Cast<T, long>(obj));
			return;
		}
		if (typeof(T) == typeof(ulong))
		{
			write.UInt64(GenericsUtil.Cast<T, ulong>(obj));
			return;
		}
		if (typeof(T) == typeof(string))
		{
			write.String(GenericsUtil.Cast<T, string>(obj));
			return;
		}
		if (typeof(T) == typeof(sbyte))
		{
			write.Int8(GenericsUtil.Cast<T, sbyte>(obj));
			return;
		}
		if (typeof(T) == typeof(byte))
		{
			write.UInt8(GenericsUtil.Cast<T, byte>(obj));
			return;
		}
		if (typeof(T) == typeof(bool))
		{
			write.Bool(GenericsUtil.Cast<T, bool>(obj));
			return;
		}
		if (typeof(T) == typeof(Color))
		{
			Color color = GenericsUtil.Cast<T, Color>(obj);
			write.Color(color);
			return;
		}
		if (typeof(T) == typeof(NetworkableId))
		{
			write.EntityID(GenericsUtil.Cast<T, NetworkableId>(obj));
			return;
		}
		if (typeof(T) == typeof(ItemContainerId))
		{
			write.ItemContainerID(GenericsUtil.Cast<T, ItemContainerId>(obj));
			return;
		}
		if (typeof(T) == typeof(ItemId))
		{
			write.ItemID(GenericsUtil.Cast<T, ItemId>(obj));
			return;
		}
		IProto proto;
		if ((proto = obj as IProto) != null)
		{
			proto.WriteToStream(write);
			return;
		}
		Debug.LogError(string.Concat(new object[]
		{
			"NetworkData.Write - no handler to write ",
			obj,
			" -> ",
			obj.GetType()
		}));
	}
}
