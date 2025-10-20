using System.Collections;
using System.Collections.Generic;
using SSW.BossMonster.BossWeapon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace SSW.BossMonster
{
    public class BossBTController : MonoBehaviour
    {
        private BTNode _rootNode;
        private BTBlackboard _blackboard;
        [SerializeField] private BossAnimatorController _animatorController;
        [SerializeField] private NavMeshAgent _agent;
        void Awake()
        {
            _blackboard = GetComponent<BTBlackboard>();
            if (_blackboard == null)
            {
                enabled = false; // Disable the controller if the blackboard is not set up
                return;
            }

            BossBulletSpawnerController[] bossBulletSpawnerControllers = GetComponentsInChildren<BossBulletSpawnerController>();
            foreach (var bossBulletSpawnerController in bossBulletSpawnerControllers)
            {
                bossBulletSpawnerController.Initialize(_blackboard);
            }

            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }
            _animatorController.Initialize(_blackboard);
        }

        void Start()
        {
            _rootNode = new SequenceNode(_blackboard, new List<BTNode>
            {
                new CheckIfDeadNode(_blackboard),

                new SelectorNode(_blackboard, new List<BTNode>
                {
                    new SequenceNode(_blackboard, new List<BTNode>
                    {
                        new CheckPlayerDistanceNode(_blackboard, 10f, ComparisonMode.LessThanOrEqual),
                        new CheckWeaponAliveNode(_blackboard),
                        new MoveToTargetNode(_blackboard, _agent, _blackboard.Target),
                        new RotateTowardsTargetNode(_blackboard, _agent),
                        new ParallelNode(_blackboard, new List<BTNode>
                        {
                            new LookAroundNode(_blackboard, _agent),
                            new RepeatShootNode(_blackboard, _agent)
                        })
                    }),

                    new SequenceNode(_blackboard, new List<BTNode>
                    {
                        new CheckPlayerDistanceNode(_blackboard, 10f, ComparisonMode.LessThanOrEqual),
                        new MoveToTargetNode(_blackboard, _agent, _blackboard.Target),
                        new ParallelNode(_blackboard, new List<BTNode>
                        {
                            new LookAroundNode(_blackboard, _agent),
                            new RepeatSpawnAreaAttackNode(_blackboard, _blackboard.AreaAttackPrefab)
                        })
                    }),

                    new SequenceNode(_blackboard, new List<BTNode>
                    {
                        new CheckPlayerDistanceNode(_blackboard, 10f, ComparisonMode.GreaterThan),
                        new ParallelNode(_blackboard, new List<BTNode>
                        {
                            new PatrolNode(_blackboard, _agent, _blackboard.PatrolPoints),
                            new RepeatSpawnAreaAttackNode(_blackboard, _blackboard.AreaAttackPrefab)
                        })
                    })
                })
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (_blackboard.IsDead)
            {
                if (_rootNode != null && _rootNode.State != NodeState.Failure)
                {
                    _rootNode.OnAbort();
                }

                if (_agent.enabled)
                {
                    _agent.enabled = false;
                }
                return;
            }
            _rootNode?.Tick();
        }
    }
}