/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    ShrineToken requisite;

    Catalogue catalogue;

    float limit = 5;

    void Awake()
    {
        catalogue = FindObjectOfType<Catalogue>();
    }

    // Update is called once per frame
    void Update()
    {
        if(catalogue.GetShrine(requisite) && transform.position.y < limit)
        {
            transform.position += Vector3.up * Time.deltaTime;
        }
    }
}*/
