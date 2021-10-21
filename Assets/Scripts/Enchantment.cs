using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Enchantment")]
public class Enchantment : ScriptableObject
{
    [SerializeField]
    string name;

    [SerializeField]
    Sprite sprite;
}
