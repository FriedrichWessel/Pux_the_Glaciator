using System;
using System.Linq;
using UnityEngine;
using Pux.Entities;
using Pux.Controllers;

namespace Pux.Entities
{
	public static class EntityBehaviourExtensions
	{
		public static EntityBehaviour Stop(this EntityBehaviour entity) {
			entity.ClearControllers();
			return entity;
		}

		public static EntityBehaviour FadeAnimation(this EntityBehaviour entity, string name, int fadeDurationInMilliseconds) {
			return entity.FadeAnimation(name, fadeDurationInMilliseconds, false);
		}

		public static EntityBehaviour FadeAnimation(this EntityBehaviour entity, string name, int fadeDurationInMilliseconds, bool isLooped) {
			var current = entity.FindAnimationState(name);
			
			current.layer = 0;
			current.wrapMode = isLooped ? WrapMode.Loop : WrapMode.Once;
			var seconds = (float)TimeSpan.FromMilliseconds(fadeDurationInMilliseconds).TotalSeconds;
			var animation = FindAnimationBehaviour(entity);
			animation.CrossFade(name, seconds, PlayMode.StopSameLayer);
			return entity;
		}

		public static EntityBehaviour PlayAnimation(this EntityBehaviour entity, string name, bool isLooped) {
	
			var current = entity.FindAnimationState(name);
			current.layer = 0;
			current.wrapMode = isLooped ? WrapMode.Loop : WrapMode.Once;
			var animation = entity.FindAnimationBehaviour();
			animation.Play(name, PlayMode.StopSameLayer);
			return entity;
		}
		
		public static AnimationState FindAnimationState(this EntityBehaviour entity, string name){
			var animation = entity.FindAnimationBehaviour();
			var current = animation[name];
			if (current == null) {
				throw new ApplicationException(string.Format("invalid animation name {0}", name));
			}
			
			return current;
		}
		
		public static Animation FindAnimationBehaviour(this EntityBehaviour entity){
			var animation = entity.gameObject.animation;
			if(animation == null){
				animation = entity.gameObject.GetComponentInChildren<Animation>() as Animation;
				if(animation == null)
					throw new MissingComponentException();
			}
			return animation;
		}

		public static EntityBehaviour PlayAnimation(this EntityBehaviour entity, string name) {
			entity.PlayAnimation(name, false);
			return entity;
		}
		
		public static bool IsPlaying(this EntityBehaviour entity, string name){
			var animation = entity.FindAnimationBehaviour();
			return animation.IsPlaying(name);
		}

		public static EntityBehaviour SwimTo(this EntityBehaviour entity, Vector3 target) {
			entity.DequeueController("move");
			
			var seaTarget = new Vector3(target.x, Environment.SeaLevel, target.z);
			entity.QueueController("move", new LinearMovementController(seaTarget));
			return entity;
		}
		
		public static EntityBehaviour MoveTo(this EntityBehaviour entity, Vector3 target) {
			return entity.MoveTo(target, true);
		}
		
		public static EntityBehaviour MoveTo(this EntityBehaviour entity, Vector3 target, bool lookAt) {
			entity.DequeueController("move");
			entity.DequeueController("float");
			
			entity.QueueController("move", new LinearMovementController(target, lookAt));
			return entity;
		}

		public static EntityBehaviour Float(this EntityBehaviour entity) {
			if (entity.IsControllerAttached("float")) {
				return entity;
			}
			entity.QueueController("float", new FloatController(Environment.SeaLevel));
			return entity;
		}
		
		public static EntityBehaviour Follow(this EntityBehaviour entity, GameObject target, Action action) {
			entity.DequeueController("move");
			var c = new LinearObjectFollowMovementController(target);
			if (action != null) {
				c.ControllerFinished += (sender, e) => action();
			}
			entity.QueueController("move", c);
			return entity;
		}
		
		public static EntityBehaviour Throw(this EntityBehaviour entity, GameObject target) {
			return entity.Throw(target, null);
		}
		
		public static EntityBehaviour Throw(this EntityBehaviour entity, GameObject target, Action action) {
			entity.DequeueController("move");
			var c = new LinearObjectFollowMovementController(target){
				IsYAxisIgnored = false,
				IsFinishedOnCatchup = true
			};
			if (action != null) {
				c.ControllerFinished += (sender, e) => action();
			}
			entity.QueueController("move", c);
			return entity;
		}
		
		public static EntityBehaviour Follow(this EntityBehaviour entity, GameObject target) {
			return entity.Follow(target, null);
		}

		public static EntityBehaviour Dive(this EntityBehaviour entity, GameObject vanishingPoint, int flatness) {
			entity.DequeueController("move");
			entity.DequeueController("float");
			
			var arc = new ArcMovementController(entity, vanishingPoint, flatness);
			entity.QueueController("move", arc);
			return entity;
		}
	}
}

