using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using HappyPenguin.Entities;
using HappyPenguin.Spawning;
using HappyPenguin.Unity;
using HappyPenguin.Controllers;
using HappyPenguin.Effects;

namespace HappyPenguin.Entities
{
	public sealed class EntityManager
	{
		private readonly TargetableSymbolManager symbolManager;
		private readonly List<EntityBehaviour> entities;
		
		public event EventHandler<EffectEventArgs> EffectsReleased;
		private void InvokeEffectsReleased(IEnumerable<Effect> effects)
		{
			var handler = EffectsReleased;
			if (handler == null) {
				return;
			}
			var e = new EffectEventArgs(effects);
			handler(this, e);
		}

		public void ThrowSnowball(TargetableEntityBehaviour target) {
			var snowball = DisplaySnowball();
			target.TargetHit += OnTargetHit;
			snowball.DedicatedTarget = target;
			snowball.DetachZoneReached += 
				(sender, e) => LaunchSnowball(sender as SnowballBehaviour, target);
		}

		private void OnTargetHit(object sender, BehaviourEventArgs<SnowballBehaviour> e) {
			var target = sender as TargetableEntityBehaviour;
			target.TargetHit -= OnTargetHit;
			target.HideSymbols();
			
			// die, snowball, die
			e.Behaviour.Dispose();
			
			if (target is CreatureBehaviour) {
				ProcessCreatureHit(target as CreatureBehaviour);
			} else {
				ProcessPerkHit(target as PerkBehaviour);
			}
		}

		private void ProcessCreatureHit(CreatureBehaviour creature) {
			var creatureRetreat = GameObjectRegistry.GetObject("creature_retreat");
			creature.IsRetreating = true;
			creature.Dive(creatureRetreat, 2000);
		}

		private void ProcessPerkHit(PerkBehaviour perk) {
			InvokeEffectsReleased(perk.HitEffects);
			var ground = perk.transform.position + new Vector3(0, -100, 0);
			perk.Speed = 10;
			perk.MoveTo(ground, false);
		}

		private void LaunchSnowball(SnowballBehaviour snowball, TargetableEntityBehaviour target) {
			//creature got disposed while throwing
			if (target == null) {
				snowball.Dispose();
				return;
			}
			
			var root = GameObjectRegistry.GetObject("entity_root");
			snowball.transform.parent = root.transform;
			snowball.Speed = 700;
			snowball.Throw(target.gameObject);
			snowball.IsReleased = true;
		}

		private SnowballBehaviour DisplaySnowball() {
			var snowball = Resources.Load("Environment/Snowball");
			var instance = GameObject.Instantiate(snowball, Vector3.zero, Quaternion.identity) as GameObject;
			
			instance.transform.parent = Player.rightHandPoint.transform;
			instance.transform.localPosition = Vector3.zero;
			
			var component = instance.GetComponentInChildren<SnowballBehaviour>();
			if (component == null) {
				throw new ApplicationException("SnowballBehaviour not found.");
			}
			
			AddEnvironmentalEntity(component);
			return component;
		}

		public EntityManager() {
			entities = new List<EntityBehaviour>();
			symbolManager = new TargetableSymbolManager();
		}

		public PlayerBehaviour Player { get; set; }

		public IEnumerable<EntityBehaviour> Entities {
			get { return entities; }
		}

		public IEnumerable<CreatureBehaviour> FindCreatures() {
			return Entities.Where(x => x is CreatureBehaviour).Select(x => x as CreatureBehaviour).ToList();
		}

		public IEnumerable<PerkBehaviour> FindPerks() {
			return Entities.Where(x => x is PerkBehaviour).Select(x => x as PerkBehaviour).ToList();
		}

		public IEnumerable<TargetableEntityBehaviour> FindTargetables() {
			return Entities.Where(x => x is TargetableEntityBehaviour).Select(x => x as TargetableEntityBehaviour).ToList();
		}

		public TargetableEntityBehaviour FindFittingTargetable(string symbolChain) {
			var targets = FindTargetables();
			return targets.FirstOrDefault(x => x.SymbolChain == symbolChain);
		}
		
		public void AddEnvironmentalEntity(EnvironmentEntityBehaviour env)
		{
			env.GrimReaperAppeared += (sender, e) => VoidEnvironmental(env);
			entities.Add(env);
		}
		
		private void VoidEnvironmental(EnvironmentEntityBehaviour env)
		{
			if (env == null) {
				return;
			}
			entities.Remove(env);
			GameObject.Destroy(env.gameObject);
		}

		public void SpawnCreature(CreatureTypes type) {
			var creatureSpawn = GameObjectRegistry.GetObject("creature_spawn");
			
			var creature = DisplayCreature(type, creatureSpawn.transform.position);
			creature.GrimReaperAppeared += (sender, e) => VoidTargetable(creature);
			creature.SwimTo(Player.gameObject.transform.position).Float();
			
			symbolManager.RegisterTargetable(creature);
			entities.Add(creature);
		}

		public void SpawnPerk(PerkTypes type) {
			// outsource into init method
			var perkSpawn = GameObjectRegistry.GetObject("perk_spawn");
			var perkImpact = GameObjectRegistry.GetObject("perk_impact");
			var perkRetreat = GameObjectRegistry.GetObject("perk_retreat");
			
			var perk = DisplayPerk(type, perkSpawn.transform.position);
			perk.GrimReaperAppeared += (sender, e) => VoidTargetable(perk);
			perk.DequeueController("move");
			perk.DequeueController("float");
			perk.Speed = 240;
			
			var arc = new ArcMovementController(perk, perkImpact, 48);
			arc.ControllerFinished += (sender, e) => {
				perk.Speed = 20;
				perk.MoveTo(perkRetreat.transform.position, false);
				perk.QueueController("impact", new WaterImpactController(Environment.SeaLevel) { Strength = 12, Duration = TimeSpan.FromSeconds(10) });
			};
			perk.QueueController("move", arc);
			
			symbolManager.RegisterTargetable(perk);
			entities.Add(perk);
		}

		public void VoidTargetable(TargetableEntityBehaviour targetable) {
			if (targetable == null) {
				return;
			}
			entities.Remove(targetable);
			GameObject.Destroy(targetable.gameObject);
		}

		private PerkBehaviour DisplayPerk(PerkTypes type, Vector3 position) {
			var resource = GetPerkResourceByType(type);
			var perkSpawn = GameObjectRegistry.GetObject("perk_spawn");
			var root = GameObjectRegistry.GetObject("entity_root");
			var gameObject = GameObject.Instantiate(resource, perkSpawn.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = root.transform;
			return gameObject.GetComponentInChildren<PerkBehaviour>();
		}

		private CreatureBehaviour DisplayCreature(CreatureTypes type, Vector3 position) {
			var target = Player.transform.position;
			var direction = position - target;
			
			var leveledPosition = new Vector3(position.x, Environment.SeaLevel, position.z);
			var quaternion = Quaternion.LookRotation(direction, Vector3.up);
			var resource = GetCreatureResourceByType(type);
			var root = GameObjectRegistry.GetObject("entity_root");
			var gameObject = GameObject.Instantiate(resource, leveledPosition, quaternion) as GameObject;
			gameObject.transform.parent = root.transform;
			return gameObject.GetComponentInChildren<CreatureBehaviour>();
		}

		private UnityEngine.Object GetPerkResourceByType(PerkTypes type) {
			var name = string.Empty;
			
			switch (type) {
			case PerkTypes.Nuke:
				name = "Perks/Nuke";
				break;
			case PerkTypes.Health:
				name = "Perks/Health";
				break;
			}
			if (string.IsNullOrEmpty(name)) {
				throw new ApplicationException("perk type unknown.");
			}
			
			return Resources.Load(name);
		}

		private UnityEngine.Object GetCreatureResourceByType(CreatureTypes type) {
			var name = string.Empty;
			switch (type) {
			case CreatureTypes.Pike:
				name = "Creatures/Pike";
				break;
			case CreatureTypes.Shark:
				name = "Creatures/Shark";
				break;
			case CreatureTypes.Whale:
				name = "Creatures/Whale";
				break;
			}
			if (string.IsNullOrEmpty(name)) {
				throw new ApplicationException("creature type unknown.");
			}
			return Resources.Load(name);
		}
	}
}

