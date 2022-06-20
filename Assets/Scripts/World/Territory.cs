using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Territory")]
public class Territory : ScriptableObject
{
	[SerializeField]
	Prop[] _props;
	public Prop[] props => _props;

	[SerializeField]
	[Range(0,1)]
	float _saturation;
	public float saturation => _saturation;

	[SerializeField]
    string _layer;
	public string layer => _layer;
    [SerializeField]
    int _order;
	public int order => _order;

	[SerializeField]
	Color _colour;
	public Color colour => _colour;
}
