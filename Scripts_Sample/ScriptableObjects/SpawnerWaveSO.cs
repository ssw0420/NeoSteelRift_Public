using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerWaveSO", menuName = "ScriptableObjects/SpawnerWaveSO", order = 1)]
public class SpawnerWaveSO : ScriptableObject
{
    // Phase List
    [System.Serializable]
    public class Phase
    {
        public string phaseName;
        public float duration; // Duration of the phase in seconds
        public List<SpawnerActivation> activations; // List of spawner activations in this phase
    }

    [System.Serializable]
    public class SpawnerActivation
    {
        public string monsterKey;         // The type of monster to activate (e.g., "Rob01")
        public int spawnersToActivate;    // How many of this type of spawner to activate randomly
    }

    public List<Phase> phases; // List of phases in the wave
}
