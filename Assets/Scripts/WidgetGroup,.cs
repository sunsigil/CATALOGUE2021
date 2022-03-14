using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WidgetGroup<W, D> : MonoBehaviour
where W : Widget<D>
where D : class
{
	[SerializeField]
	W widget_prefab;

	protected List<W> _subordinates;
	public List<W> subordinates => _subordinates;

	protected void Assign(W subordinate, D data)
	{
		subordinate.Bind(data);
		_subordinates.Add(subordinate);
	}

	protected void Add(D data)
	{
		W subordinate = Instantiate(widget_prefab);
		subordinate.transform.SetParent(transform);

		Assign(subordinate, data);
	}

	protected abstract void Reform();

	public void Fill(List<D> data)
	{
		int index = 0;

		while(index < _subordinates.Count && index < data.Count)
		{
			Assign(_subordinates[index], data[index]);
			index += 1;
		}

		if(_subordinates.Count > data.Count)
		{
			while(index < _subordinates.Count)
			{
				Destroy(_subordinates[index]);
				_subordinates.RemoveAt(index);
				index += 1;
			}
		}
		else if(_subordinates.Count < data.Count)
		{
			while(index < data.Count)
			{
				Add(data[index]);
			}
		}

		Reform();
	}

	protected virtual void Awake()
	{
		_subordinates = new List<W>();

		W[] children = GetComponentsInChildren<W>();

		foreach(W child in children)
		{
			Assign(child, child.IsBound() ? child.data : null);
		}

		Reform();
	}
}
