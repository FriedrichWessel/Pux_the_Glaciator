using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Pux.Collections;

namespace Pux.Effects
{
	public sealed class EffectManager
	{
		private readonly GameWorldBehaviour world;
		private Dictionary<Effect, TimeSpan> effects;
		
		public event EventHandler<EffectEventArgs> EffectExpired;
		private void InvokeEffectExpired(Effect effect)
		{
			var handler = EffectExpired;
			if (handler == null) {
				return;
			}
			
			var e = new EffectEventArgs(new[]{effect});
			handler(this, e);
		}

		public EffectManager(GameWorldBehaviour world) {
			this.world = world;
			effects = new Dictionary<Effect, TimeSpan>();
		}

		public void Update() {
			var expiredEffects = new List<Effect>();
			
			foreach (var e in effects) {
				var isExpired = e.Key.IsExpired(e.Value);
				if (isExpired) {
					expiredEffects.Add(e.Key);
					continue;
				}
				
				e.Key.Update(world);
			}
			
			foreach (var e in expiredEffects) {
				e.Stop(world);
				effects.Remove(e);
				InvokeEffectExpired(e);
			}
		}
		
		public bool CanRegisterEffect(Effect effect) {
			return effect.IsStackable || effects.Keys.FirstOrDefault(x => x.GetType() == effect.GetType()) == null;
		}
		
		public void RegisterEffects(IEnumerable<Effect> effects)
		{
			foreach (var e in effects) {
				RegisterEffect(e);
			}
		}

		public void RegisterEffect(Effect e) {
			if (effects.ContainsKey(e)) {
				return;
			}
			effects.Add(e, TimeSpan.FromSeconds(Time.timeSinceLevelLoad));
			e.Start(world);
		}

		public IEnumerable<Effect> Effects {
			get { return effects.Keys; }
		}
	}
}

