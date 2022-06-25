using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<A, B>
where A : Object
where B : Object
{
	A _a;
	public A a => _a;

	B _b;
	public B b => _b;

	public Pair(A a, B b)
	{
		_a = a;
		_b = b;
	}
}
