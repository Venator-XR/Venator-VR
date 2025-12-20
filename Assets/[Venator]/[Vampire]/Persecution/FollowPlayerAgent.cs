using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FollowPlayerAgent : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        public Transform objSeguido;

        [Header("Configuraci�n de Velocidad")]
        public float velocidadNormal = 3.5f;
        public float velocidadAturdido = 0.5f; // Casi quieto

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            if (m_Agent != null) m_Agent.speed = velocidadNormal;

            if (objSeguido == null)
            {
                GameObject jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null) objSeguido = jugador.transform;
            }
        }

        void Update()
        {
            if (objSeguido != null && m_Agent != null && m_Agent.isOnNavMesh)
            {
                m_Agent.destination = objSeguido.position;
            }
        }

        // Esta funci�n la llamaremos desde el script de PersecutionManager
        public void RalentizarVampiro(float tiempo)
        {
            StartCoroutine(CorrutinaRalentizar(tiempo));
        }

        IEnumerator CorrutinaRalentizar(float tiempo)
        {
            m_Agent.speed = velocidadAturdido;
            yield return new WaitForSeconds(tiempo);
            m_Agent.speed = velocidadNormal;
        }
    }
}