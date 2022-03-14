using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Territory Profile")]
public class TerritoryProfile : ScriptableObject
{
	[SerializeField]
	Prop[] _props;
	public Prop[] props => _props;

	[SerializeField]
	[Range(0,1)]
	float _saturation;
	public float saturation => _saturation;

	[SerializeField]
	bool _override_sorting;
	public bool override_sorting => _override_sorting;
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
