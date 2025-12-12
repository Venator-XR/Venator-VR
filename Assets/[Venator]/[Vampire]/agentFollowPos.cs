using UnityEngine;
using UnityEngine.AI;
namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class agentFollowPos : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        public Transform objSeguido;
        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
        }
        void Update()
        {
            m_Agent.destination = objSeguido.position;
        }
    }
}
