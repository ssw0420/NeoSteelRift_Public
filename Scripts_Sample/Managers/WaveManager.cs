using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SSW.Spawner
{
    public class WaveManager : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnerGroup
        {
            public string monsterKey;
            public List<Spawner> spawners;
        }

        [Header("Wave Data")]
        [SerializeField] private SpawnerWaveSO waveData;

        [Header("Spawner Groups")]
        [SerializeField] private List<SpawnerGroup> spawnerGroups;

        private Dictionary<string, List<Spawner>> _spawnersByType = new Dictionary<string, List<Spawner>>();
        private List<Spawner> _activeSpawners = new List<Spawner>();
        // Start is called before the first frame update
        void Start()
        {
            InitializeSpawnerDictionary();
            DeactivateAllRegisteredSpawners();
            StartCoroutine(CoRunWaveTimeline());
        }

        private void InitializeSpawnerDictionary()
        {
            foreach (var group in spawnerGroups)
            {
                _spawnersByType[group.monsterKey] = group.spawners;
            }
        }

        private void DeactivateAllRegisteredSpawners()
        {
            foreach (var group in spawnerGroups)
            {
                foreach (var spawner in group.spawners)
                {
                    if(spawner != null)
                    {
                        spawner.gameObject.SetActive(false);
                    }
                }
            }
        }

        private IEnumerator CoRunWaveTimeline()
        {
            foreach (var phase in waveData.phases)
            {
                // Last Phase Cleanup
                foreach (var spawner in _activeSpawners)
                {
                    if (spawner != null)
                    {
                        spawner.gameObject.SetActive(false);
                    }
                }
                _activeSpawners.Clear();

                // Activate spawners for the current phase
                foreach (var activation in phase.activations)
                {
                    ActivateSpawners(activation.monsterKey, activation.spawnersToActivate);
                }

                // Wait for the duration of the phase
                yield return new WaitForSeconds(phase.duration);
            }
        }

        private void ActivateSpawners(string monsterKey, int count)
        {
            List<Spawner> availableSpawners = _spawnersByType[monsterKey];
            
            List<Spawner> spawnersToActivate = availableSpawners.OrderBy(x => Random.value).Take(count).ToList();
            
            foreach(var spawner in spawnersToActivate)
            {
                spawner.gameObject.SetActive(true);
                _activeSpawners.Add(spawner);
            }
        }
    }
}