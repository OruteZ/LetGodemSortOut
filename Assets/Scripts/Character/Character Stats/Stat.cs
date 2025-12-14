using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Serialization;


namespace OZ_Character.Stats
{
	[Serializable]
	public class Stat
	{
		public float baseValue;

		protected bool isDirty = true;
		protected float lastBaseValue;

		protected float value;
		public virtual float Value {
			get {
				if (isDirty || Math.Abs(lastBaseValue - baseValue) > float.Epsilon) {
					lastBaseValue = baseValue;
					value = CalculateFinalValue();
					isDirty = false;
				}
				return value;
			}
		}

		protected readonly List<StatModifier> statModifiers;
		public readonly ReadOnlyCollection<StatModifier> statModifiersRO;

		public Stat()
		{
			statModifiers = new List<StatModifier>();
			statModifiersRO = statModifiers.AsReadOnly();
		}

		public Stat(float baseValue) : this()
		{
			this.baseValue = baseValue;
		}

		public virtual void AddModifier(StatModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if (statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			int numRemovals = statModifiers.RemoveAll(mod => mod.source == source);

			if (numRemovals <= 0) return false;
			isDirty = true;
			return true;
		}

		protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
		{
			if (a.order < b.order)
				return -1;
			
			if (a.order > b.order)
				return 1;
			
			return 0; //equal
		}
		
		protected virtual float CalculateFinalValue()
		{
			float finalValue = baseValue;
			float sumPercentAdd = 0;

			statModifiers.Sort(CompareModifierOrder);

			for (int i = 0; i < statModifiers.Count; i++)
			{
				StatModifier mod = statModifiers[i];

				if (mod.type == StatModType.FLAT)
				{
					finalValue += mod.value;
				}
				else if (mod.type == StatModType.PERCENT_ADD)
				{
					sumPercentAdd += mod.value;

					if (i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.PERCENT_ADD)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
				}
				else if (mod.type == StatModType.PERCENT_MULT)
				{
					finalValue *= 1 + mod.value;
				}
			}

			// Workaround for float calculation errors, like displaying 12.00001 instead of 12
			return (float)Math.Round(finalValue, 4);
		}
	}
}
