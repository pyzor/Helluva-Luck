using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomBehaviorSystem {

    private class PoolEntry {
        public WorldTileBehavior Behavior;
        public float CumulativeProbability { get; set; }

        public PoolEntry(WorldTileBehavior behavior) {
            Behavior = behavior;
            CumulativeProbability = 0;
        }
    }


    private List<PoolEntry> _entries;


    public RandomBehaviorSystem() {
        _entries = new List<PoolEntry>();
    }

    public void AddEntry(WorldTileBehavior behavior) {
        if (behavior == null)
            return;
        _entries.Add(new PoolEntry(behavior));
        CalculateWeights();
    }

    /// <summary>
    /// Checks which of the entries procs with the given roll value and returns its behavior.
    /// </summary>
    /// <param name="roll"></param>
    /// <returns></returns>
    public WorldTileBehavior GetBehavior(float roll) {
        if(_entries.Count == 0)
            return null;

        for(int i = 0; i < _entries.Count; i++) {
            var entry = _entries[i];
            if(roll <= entry.CumulativeProbability) {
                return entry.Behavior.Create();
            }
        }

        return null;
    }

    public void ClearEntries() {
        _entries.Clear();
    }

    public void CalculateWeights() {
        float sum = 0;
        for(int i = 0; i < _entries.Count; i++) {
            sum += _entries[i].Behavior.GetProbability();
        }
        float multiplier = (sum != 0) ? 1 / sum : 1;

        _entries.Sort((a, b) => a.Behavior.GetProbability().CompareTo(b.Behavior.GetProbability()));

        sum = 0;
        for (int i = 0; i < _entries.Count; i++) {
            var entry = _entries[i];
            float newProbability = entry.Behavior.GetProbability() * multiplier;
            entry.CumulativeProbability = newProbability + sum;
            sum += newProbability;
        }
    }

}

