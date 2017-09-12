using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 

[ExecuteInEditMode]
public abstract class SkillShape : MonoBehaviour {
	public Example.SkillShape.ShapeType shapeType;
	  
	public float scale = 1;

	public float width = 1;

	public float height = 1;

	public float radius = 1;

	public float angle = 180;

	public virtual bool Intersect(SkillShape other){
		throw new Exception (GetType().Name +".Intersect(SkillShape) not impl");
	}

	public virtual bool Intersect(Vector3 point){
		throw new Exception (GetType().Name +".Intersect(Vector3) not impl");
	}
	 
}
