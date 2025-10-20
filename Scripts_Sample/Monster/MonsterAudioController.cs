using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SSW.Monster
{
    public class MonsterAudioController : MonoBehaviour
    {
        [Header("Audio Events")]
        [SerializeField] private FMODUnity.EventReference _monsterAttackEvent;
        [SerializeField] private FMODUnity.EventReference _monsterDeathEvent;
        [SerializeField] private FMODUnity.EventReference _monsterSpawnEvent;
        [SerializeField] private FMODUnity.EventReference _monsterWalkEvent;
        [SerializeField] private FMODUnity.EventReference _monsterHitEvent;
        [SerializeField] private FMODUnity.EventReference _monsterRotateEvent;

        [Header("Footstep")]
        [SerializeField] private GameObject _leftFrontToe;
        [SerializeField] private GameObject _leftRearToe;
        [SerializeField] private GameObject _rightFrontToe;
        [SerializeField] private GameObject _rightRearToe;

        [Header("Attack")]
        [SerializeField] private GameObject _attackBone;

        [Header("Death")]
        [SerializeField] private GameObject _deathBone;

        [Header("Spawn")]
        [SerializeField] private GameObject _spawnBone;

        public void PlayAttackSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterAttackEvent, _attackBone);
        }

        public void PlayDeathSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterDeathEvent, gameObject);
        }

        public void PlaySpawnSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterSpawnEvent, gameObject);
        }

        public void PlayWalkSound()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterWalkEvent, gameObject);
        }

        public void RightFrontFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterWalkEvent, _rightFrontToe);
        }
        public void RightRearFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterWalkEvent, _rightRearToe);
        }
        public void LeftFrontFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterWalkEvent, _leftFrontToe);
        }
        public void LeftRearFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_monsterWalkEvent, _leftRearToe);
        }
    }
}