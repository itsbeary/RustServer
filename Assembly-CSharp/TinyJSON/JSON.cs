using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace TinyJSON
{
	// Token: 0x020009DB RID: 2523
	public static class JSON
	{
		// Token: 0x06003C01 RID: 15361 RVA: 0x00161CB0 File Offset: 0x0015FEB0
		public static Variant Load(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			return Decoder.Decode(json);
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x00161CC6 File Offset: 0x0015FEC6
		public static string Dump(object data)
		{
			return JSON.Dump(data, EncodeOptions.None);
		}

		// Token: 0x06003C03 RID: 15363 RVA: 0x00161CD0 File Offset: 0x0015FED0
		public static string Dump(object data, EncodeOptions options)
		{
			if (data != null)
			{
				Type type = data.GetType();
				if (!type.IsEnum && !type.IsPrimitive && !type.IsArray)
				{
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (methodInfo.GetCustomAttributes(false).AnyOfType(typeof(BeforeEncode)) && methodInfo.GetParameters().Length == 0)
						{
							methodInfo.Invoke(data, null);
						}
					}
				}
			}
			return Encoder.Encode(data, options);
		}

		// Token: 0x06003C04 RID: 15364 RVA: 0x00161D4B File Offset: 0x0015FF4B
		public static void MakeInto<T>(Variant data, out T item)
		{
			item = JSON.DecodeType<T>(data);
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x00161D5C File Offset: 0x0015FF5C
		private static Type FindType(string fullName)
		{
			if (fullName == null)
			{
				return null;
			}
			Type type;
			if (JSON.typeCache.TryGetValue(fullName, out type))
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(fullName);
				if (type != null)
				{
					JSON.typeCache.Add(fullName, type);
					return type;
				}
			}
			return null;
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x00161DBC File Offset: 0x0015FFBC
		private static T DecodeType<T>(Variant data)
		{
			if (data == null)
			{
				return default(T);
			}
			Type type = typeof(T);
			if (type.IsEnum)
			{
				return (T)((object)Enum.Parse(type, data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
			{
				return (T)((object)Convert.ChangeType(data, type));
			}
			if (type == typeof(Guid))
			{
				return (T)((object)new Guid(data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					return (T)((object)JSON.decodeArrayMethod.MakeGenericMethod(new Type[] { type.GetElementType() }).Invoke(null, new object[] { data }));
				}
				ProxyArray proxyArray = data as ProxyArray;
				if (proxyArray == null)
				{
					throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
				}
				int[] array = new int[type.GetArrayRank()];
				if (!proxyArray.CanBeMultiRankArray(array))
				{
					throw new DecodeException("Error decoding multidimensional array; JSON data doesn't seem fit this structure.");
				}
				Type elementType = type.GetElementType();
				if (elementType == null)
				{
					throw new DecodeException("Array element type is expected to be not null, but it is.");
				}
				Array array2 = Array.CreateInstance(elementType, array);
				MethodInfo methodInfo = JSON.decodeMultiRankArrayMethod.MakeGenericMethod(new Type[] { elementType });
				try
				{
					methodInfo.Invoke(null, new object[] { proxyArray, array2, 1, array });
				}
				catch (Exception ex)
				{
					throw new DecodeException("Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", ex);
				}
				return (T)((object)Convert.ChangeType(array2, typeof(T)));
			}
			else
			{
				if (typeof(IList).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeListMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { data }));
				}
				if (typeof(IDictionary).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeDictionaryMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { data }));
				}
				ProxyObject proxyObject = data as ProxyObject;
				if (proxyObject == null)
				{
					throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
				}
				string typeHint = proxyObject.TypeHint;
				T t;
				if (typeHint != null && typeHint != type.FullName)
				{
					Type type2 = JSON.FindType(typeHint);
					if (type2 == null)
					{
						throw new TypeLoadException("Could not load type '" + typeHint + "'.");
					}
					if (!type.IsAssignableFrom(type2))
					{
						throw new InvalidCastException(string.Concat(new string[] { "Cannot assign type '", typeHint, "' to type '", type.FullName, "'." }));
					}
					t = (T)((object)Activator.CreateInstance(type2));
					type = type2;
				}
				else
				{
					t = Activator.CreateInstance<T>();
				}
				foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)((ProxyObject)data)))
				{
					FieldInfo fieldInfo = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (fieldInfo == null)
					{
						foreach (FieldInfo fieldInfo2 in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj in fieldInfo2.GetCustomAttributes(true))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj) && ((DecodeAlias)obj).Contains(keyValuePair.Key))
								{
									fieldInfo = fieldInfo2;
									break;
								}
							}
						}
					}
					if (fieldInfo != null)
					{
						bool flag = fieldInfo.IsPublic;
						foreach (object obj2 in fieldInfo.GetCustomAttributes(true))
						{
							if (JSON.excludeAttrType.IsInstanceOfType(obj2))
							{
								flag = false;
							}
							if (JSON.includeAttrType.IsInstanceOfType(obj2))
							{
								flag = true;
							}
						}
						if (flag)
						{
							MethodInfo methodInfo2 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[] { fieldInfo.FieldType });
							if (type.IsValueType)
							{
								object obj3 = t;
								fieldInfo.SetValue(obj3, methodInfo2.Invoke(null, new object[] { keyValuePair.Value }));
								t = (T)((object)obj3);
							}
							else
							{
								fieldInfo.SetValue(t, methodInfo2.Invoke(null, new object[] { keyValuePair.Value }));
							}
						}
					}
					PropertyInfo propertyInfo = type.GetProperty(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (propertyInfo == null)
					{
						foreach (PropertyInfo propertyInfo2 in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj4 in propertyInfo2.GetCustomAttributes(false))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj4) && ((DecodeAlias)obj4).Contains(keyValuePair.Key))
								{
									propertyInfo = propertyInfo2;
									break;
								}
							}
						}
					}
					if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(false).AnyOfType(JSON.includeAttrType))
					{
						MethodInfo methodInfo3 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[] { propertyInfo.PropertyType });
						if (type.IsValueType)
						{
							object obj5 = t;
							propertyInfo.SetValue(obj5, methodInfo3.Invoke(null, new object[] { keyValuePair.Value }), null);
							t = (T)((object)obj5);
						}
						else
						{
							propertyInfo.SetValue(t, methodInfo3.Invoke(null, new object[] { keyValuePair.Value }), null);
						}
					}
				}
				foreach (MethodInfo methodInfo4 in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo4.GetCustomAttributes(false).AnyOfType(typeof(AfterDecode)))
					{
						MethodBase methodBase = methodInfo4;
						object obj6 = t;
						object[] array4;
						if (methodInfo4.GetParameters().Length != 0)
						{
							(array4 = new object[1])[0] = data;
						}
						else
						{
							array4 = null;
						}
						methodBase.Invoke(obj6, array4);
					}
				}
				return t;
			}
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x00162410 File Offset: 0x00160610
		private static List<T> DecodeList<T>(Variant data)
		{
			List<T> list = new List<T>();
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			foreach (Variant variant in ((IEnumerable<Variant>)proxyArray))
			{
				list.Add(JSON.DecodeType<T>(variant));
			}
			return list;
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x00162478 File Offset: 0x00160678
		private static Dictionary<TKey, TValue> DecodeDictionary<TKey, TValue>(Variant data)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			Type typeFromHandle = typeof(TKey);
			ProxyObject proxyObject = data as ProxyObject;
			if (proxyObject == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyObject here, but it is not.");
			}
			foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)proxyObject))
			{
				TKey tkey = (TKey)((object)(typeFromHandle.IsEnum ? Enum.Parse(typeFromHandle, keyValuePair.Key) : Convert.ChangeType(keyValuePair.Key, typeFromHandle)));
				TValue tvalue = JSON.DecodeType<TValue>(keyValuePair.Value);
				dictionary.Add(tkey, tvalue);
			}
			return dictionary;
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x00162524 File Offset: 0x00160724
		private static T[] DecodeArray<T>(Variant data)
		{
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			T[] array = new T[proxyArray.Count];
			int num = 0;
			foreach (Variant variant in ((IEnumerable<Variant>)proxyArray))
			{
				array[num++] = JSON.DecodeType<T>(variant);
			}
			return array;
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x00162598 File Offset: 0x00160798
		private static void DecodeMultiRankArray<T>(ProxyArray arrayData, Array array, int arrayRank, int[] indices)
		{
			int count = arrayData.Count;
			for (int i = 0; i < count; i++)
			{
				indices[arrayRank - 1] = i;
				if (arrayRank < array.Rank)
				{
					JSON.DecodeMultiRankArray<T>(arrayData[i] as ProxyArray, array, arrayRank + 1, indices);
				}
				else
				{
					array.SetValue(JSON.DecodeType<T>(arrayData[i]), indices);
				}
			}
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x001625F8 File Offset: 0x001607F8
		public static void SupportTypeForAOT<T>()
		{
			JSON.DecodeType<T>(null);
			JSON.DecodeList<T>(null);
			JSON.DecodeArray<T>(null);
			JSON.DecodeDictionary<short, T>(null);
			JSON.DecodeDictionary<ushort, T>(null);
			JSON.DecodeDictionary<int, T>(null);
			JSON.DecodeDictionary<uint, T>(null);
			JSON.DecodeDictionary<long, T>(null);
			JSON.DecodeDictionary<ulong, T>(null);
			JSON.DecodeDictionary<float, T>(null);
			JSON.DecodeDictionary<double, T>(null);
			JSON.DecodeDictionary<decimal, T>(null);
			JSON.DecodeDictionary<bool, T>(null);
			JSON.DecodeDictionary<string, T>(null);
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x00162667 File Offset: 0x00160867
		private static void SupportValueTypesForAOT()
		{
			JSON.SupportTypeForAOT<short>();
			JSON.SupportTypeForAOT<ushort>();
			JSON.SupportTypeForAOT<int>();
			JSON.SupportTypeForAOT<uint>();
			JSON.SupportTypeForAOT<long>();
			JSON.SupportTypeForAOT<ulong>();
			JSON.SupportTypeForAOT<float>();
			JSON.SupportTypeForAOT<double>();
			JSON.SupportTypeForAOT<decimal>();
			JSON.SupportTypeForAOT<bool>();
			JSON.SupportTypeForAOT<string>();
		}

		// Token: 0x040036D4 RID: 14036
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x040036D5 RID: 14037
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x040036D6 RID: 14038
		private static readonly Type decodeAliasAttrType = typeof(DecodeAlias);

		// Token: 0x040036D7 RID: 14039
		private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		// Token: 0x040036D8 RID: 14040
		private const BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x040036D9 RID: 14041
		private const BindingFlags staticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x040036DA RID: 14042
		private static readonly MethodInfo decodeTypeMethod = typeof(JSON).GetMethod("DecodeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040036DB RID: 14043
		private static readonly MethodInfo decodeListMethod = typeof(JSON).GetMethod("DecodeList", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040036DC RID: 14044
		private static readonly MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod("DecodeDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040036DD RID: 14045
		private static readonly MethodInfo decodeArrayMethod = typeof(JSON).GetMethod("DecodeArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040036DE RID: 14046
		private static readonly MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod("DecodeMultiRankArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}
}
