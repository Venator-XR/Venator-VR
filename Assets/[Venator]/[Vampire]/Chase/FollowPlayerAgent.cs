using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FollowPlayerAgent : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        public Transform followTransform;

        [Header("Speed Config")]
        public float normalSpeed = 3.5f;
        public float stunnedSpeed = 0.5f;

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            if (m_Agent != null) m_Agent.speed = normalSpeed;

            if (followTransform == null)
            {
                GameObject jugador = GameObject.FindGameObjectWithTag("MainCamera");
                if (jugador != null) followTransform = jugador.transform;
                else Debug.LogError("followTransform nor Player found");
            }
        }

        void Update()
        {
            if (followTransform != null && m_Agent != null && m_Agent.isOnNavMesh)
            {
                m_Agent.destination = followTransform.position;
            }
        }

        // Method called from PersecutionManager
        public void StunVampire(float seconds)
        {
            Debug.Log("Vampire stunned");
            StartCoroutine(StunCoroutine(seconds));
        }

        IEnumerator StunCoroutine(float seconds)
        {
            m_Agent.speed = stunnedSpeed;
            yield return new WaitForSeconds(seconds);
            m_Agent.speed = normalSpeed;
        }
    }
}