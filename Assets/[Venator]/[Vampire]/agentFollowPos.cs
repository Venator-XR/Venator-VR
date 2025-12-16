using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class agentFollowPos : MonoBehaviour
    {
        NavMeshAgent m_Agent;
        public Transform objSeguido; // El objetivo a perseguir

        void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();

            // --- AUTO-DETECTAR JUGADOR ---
            // Si la casilla está vacía (pasa al reiniciar la escena), buscamos al Player.
            if (objSeguido == null)
            {
                // Busca el objeto que tenga la etiqueta exacta "Player"
                GameObject jugador = GameObject.FindGameObjectWithTag("Player");

                if (jugador != null)
                {
                    objSeguido = jugador.transform;
                }
                else
                {
                    // Si sale este mensaje rojo en la consola, es que se te olvidó el Tag
                    Debug.LogError("ERROR: ¡El Vampiro no encuentra al jugador! Asegúrate de que tu XR Origin tiene el Tag 'Player'.");
                }
            }
        }

        void Update()
        {
            // Solo intentamos movernos si tenemos a quién seguir
            if (objSeguido != null && m_Agent != null)
            {
                // --- SEGURIDAD EXTRA ---
                // Verificamos si el vampiro está pisando el suelo (NavMesh) antes de moverlo.
                // Esto evita errores si el choque lo empuja fuera del mapa.
                if (m_Agent.isOnNavMesh)
                {
                    m_Agent.destination = objSeguido.position;
                }
            }
        }
    }
}