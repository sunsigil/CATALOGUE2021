using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Widget<D> : MonoBehaviour
where D : class
{
	protected D _data;
	public D data => _data;

	public virtual void Bind(D data){ _data = data; }
	public bool IsBound(){ return _data != null; }
}
