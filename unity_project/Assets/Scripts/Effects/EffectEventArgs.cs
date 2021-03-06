using System.Collections.Generic;
using System;

namespace Pux.Effects
{
	public sealed class EffectEventArgs : EventArgs
	{
		public EffectEventArgs(IEnumerable<Effect> effects) {
			Effects = effects;
		}
		
		public IEnumerable<Effect> Effects {
			get;
			private set;
		}
	}
}

