using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleSkillShape : SkillShape {


	
	public override bool Intersect (SkillShape other)
	{
		return base.Intersect (other);
	}

	public override bool Intersect (Vector3 point)
	{
		point.y = transform.position.y;
		return (transform.position - point).sqrMagnitude <= radius * radius * scale * scale;
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red; 
		//float r = radius * scale;
		//Gizmos.DrawWireSphere (transform.position,r);
		Gizmos.DrawSphere(transform.position,0.1f);
	}
}
