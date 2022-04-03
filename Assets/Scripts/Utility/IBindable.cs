using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable
{
	void Bind(object data);
	bool IsBound();
}
