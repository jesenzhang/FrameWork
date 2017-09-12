using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SkillExportHelper  {


	public static void ExportSkill(Skill skill){
		ExportSkill (skill, "Export/Skills/" + skill.skillID + ".bytes");
	}

	public static void ExportSkill(Skill skill,string path){
		Example.Skill skillData = new Example.Skill();



		List<Example.SkillParticle> skillParticles = new List<Example.SkillParticle> ();

		var particles = skill.GetComponentsInChildren<SkillParticle> ();
		foreach (var particle in particles) {
			ExportSkillParticle (particle,skillParticles); 
		}

		Example.SkillTimeLine timeline = ExportSkillTimeline (SkillManager.GetSkillEventRoot(skill).gameObject,skillParticles); 

		//use uint better
		int targetMask = 0;
		foreach (var mask in skill.targetTypes) {
			targetMask |= 1 << (int)mask;
		}

		skillData.Id = ConvertSkillID(skill.skillID);
		skillData.skillType = skill.skillType;
		skillData.TargetMask = targetMask;
		skillData.SkillCD = skill.skillCD;
		skillData.SkillTime = skill.skillTime;
		skillData.CastingDistance = skill.castingDistance;
		skillData.Timeline = timeline;

		var data = Example.Skill.SerializeToBytes (skillData);
		File.WriteAllBytes (path, data);

		Debug.LogFormat ("Export Skill {0} to {1}", skill.name, path);
	}

	private static Example.SkillTimeLine ExportSkillTimeline(GameObject go,List<Example.SkillParticle> particles){
		Example.SkillTimeLine timeline = new Example.SkillTimeLine ();

		List<Example.SkillAction> skillActions = new List<Example.SkillAction> ();

		var actions = go.GetComponentsInChildren<SkillAction> ();
		foreach (var action in actions) {
			ExportSkillAction (action,skillActions);
		}

		timeline.Actions = skillActions;
		timeline.Particles = particles;

		return timeline;
	}

	private static Example.SkillParticle ExportSkillParticle(SkillParticle particle,List<Example.SkillParticle> particles){
		Example.SkillParticle skillParticle = new Example.SkillParticle ();
		List<Example.SkillAction> skillActions = new List<Example.SkillAction> ();

		var actions = particle.GetComponentsInChildren<SkillAction> ();
		foreach (var action in actions) {
			ExportSkillAction (action,skillActions);
		}

		skillParticle.Id = particles.Count;
		skillParticle.StartTime = particle.startFrame;
		skillParticle.Duration = particle.duration;
		skillParticle.Effect = particle.effectName;
		//skillParticle.HitShape = particle.hitEffectName;
		//skillParticle.Path = particle.hitEffectTime; 
		skillParticle.Actions = skillActions;
		particles.Add (skillParticle);
		return skillParticle;
	}

	private static Example.SkillAction ExportSkillAction(SkillAction action,List<Example.SkillAction> actions){
		Example.SkillAction skillAction = new Example.SkillAction ();
		actions.Add (skillAction);
		return skillAction;
	}

	private static int ConvertSkillID(string strID){
		return 0;
	}
}
