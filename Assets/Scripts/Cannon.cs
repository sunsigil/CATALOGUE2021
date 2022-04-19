using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Machine))]
[RequireComponent(typeof(Combatant))]

public class Cannon : Subcontroller
{
	[SerializeField]
    GameObject bullet_prefab;
	
	Vector3 MouseDirection()
    {
        Vector3 mouse_point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse_point.z = transform.position.z;

        Vector3 mouse_line = mouse_point - transform.position;
        return mouse_line.normalized;
    }

	public void Anticipation(StateSignal signal)
    {
        switch(signal)
        {
            case StateSignal.TICK:
                transform.rotation = NumTools.XY_Rot(MouseDirection(), -90);

				if(Pressed(InputCode.ACTION))
				{
					Bullet bullet = Instantiate(bullet_prefab, transform.parent).GetComponent<Bullet>();
					bullet.transform.position = transform.position;
					bullet.velocity = transform.up;
				}
            break;
        }
    }
}
