using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumTools
{
	public static E[] EnumArray<E>() where E : Enum
	{ return (E[])Enum.GetValues(typeof(E)); }

	public static int EnumLength<E>() where E : Enum
	{ return EnumArray<E>().Length; }

	public static E FromString<E>(string s) where E : Enum
	{ return (E)Enum.Parse(typeof(E), s, true); }

	public static string ToString<E>(E e) where E : Enum
	{ return Enum.GetName(typeof(E), e); }

	public static string[] StringArray<E>() where E : Enum
	{ return Enum.GetNames(typeof(E)); }
}
