using System.Collections;
using UnityEngine;

public class PaintingTransition : MonoBehaviour
{
    [Header("Referencias a los Cuadros")]
    [SerializeField] private GameObject normalPainting; // Referencia al cuadro normal
    [SerializeField] private GameObject darkPainting; // Referencia al cuadro oscuro

    [Header("Configuración de la Transición")]
    [SerializeField] private float lookDuration = 0.5f; // Tiempo que debe mirar el jugador (0.5 segundos)
    [SerializeField] private float transitionDuration = 2f; // Duración de la transición cuando deja de mirar (2 segundos)
    [SerializeField] private float maxViewDistance = 10f; // Distancia máxima para detectar el cuadro

    [Header("Configuración del Jugador")]
    [SerializeField] private Camera playerCamera; // Cámara del jugador (Main Camera del XR Origin)

    [Header("Configuración de Detección")]
    [SerializeField] private DetectionMode detectionMode = DetectionMode.FrustumAndRaycast; // Modo de detección

    // Enum para seleccionar el modo de detección
    public enum DetectionMode
    {
        SingleRaycast,          // Solo un raycast central (método anterior)
        FrustumCheck,           // Solo verificar si está en el frustum de la cámara
        FrustumAndRaycast,      // Frustum + raycast de confirmación (recomendado)
        MultipleRaycasts        // Múltiples raycasts en patrón (más preciso pero más costoso)
    }

    // Variables internas
    private bool hasSeenPainting = false; // Controla si el jugador ya vio el cuadro el tiempo suficiente
    private bool transitionCompleted = false; // Controla si la transición ya se completó
    private float currentLookTime = 0f; // Tiempo acumulado mirando el cuadro
    private bool wasLookingLastFrame = false; // Para detectar cuando deja de mirar
    private Renderer paintingRenderer; // Renderer del cuadro para detección de frustum
    private Collider paintingCollider; // Collider del cuadro para raycast
    private Bounds paintingBounds; // Bounds del cuadro

    private void Start()
    {
        Debug.Log("PaintingTransition: Iniciando script...");

        // Validar referencias
        if (normalPainting == null || darkPainting == null)
        {
            Debug.LogError("PaintingTransition: Faltan referencias a los cuadros. Asigna los prefabs en el Inspector.");
            enabled = false;
            return;
        }

        if (playerCamera == null)
        {
            // Intentar encontrar la cámara automáticamente
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("PaintingTransition: No se encontró la cámara del jugador. Asigna la Main Camera del XR Origin en el Inspector.");
                enabled = false;
                return;
            }
            else
            {
                Debug.Log("PaintingTransition: Cámara encontrada automáticamente: " + playerCamera.name);
            }
        }
        else
        {
            Debug.Log("PaintingTransition: Cámara asignada: " + playerCamera.name);
        }

        // Obtener el renderer y collider del cuadro normal (usamos el normal porque es el que está visible)
        paintingRenderer = normalPainting.GetComponent<Renderer>();
        paintingCollider = normalPainting.GetComponent<Collider>();

        if (paintingRenderer == null)
        {
            Debug.LogError("PaintingTransition: El cuadro normal no tiene Renderer. Necesario para detección de frustum.");
            enabled = false;
            return;
        }

        if (paintingCollider == null)
        {
            Debug.LogWarning("PaintingTransition: El cuadro normal no tiene Collider. Algunos modos de detección no funcionarán correctamente.");
        }

        // Configurar estado inicial: cuadro normal visible, cuadro oscuro invisible
        normalPainting.SetActive(true);
        darkPainting.SetActive(false);

        Debug.Log("PaintingTransition: Estado inicial configurado. Normal visible, Oscuro oculto.");
        Debug.Log("PaintingTransition: Modo de detección: " + detectionMode.ToString());
        Debug.Log("PaintingTransition: Esperando que el jugador mire el cuadro durante " + lookDuration + " segundos...");
    }

    private void Update()
    {
        // Si la transición ya se completó, no hacer nada más
        if (transitionCompleted)
        {
            return;
        }

        // Verificar si el jugador está mirando el cuadro según el modo seleccionado
        bool isLookingNow = false;

        switch (detectionMode)
        {
            case DetectionMode.SingleRaycast:
                isLookingNow = IsLookingAtPainting_SingleRaycast();
                break;
            case DetectionMode.FrustumCheck:
                isLookingNow = IsLookingAtPainting_FrustumCheck();
                break;
            case DetectionMode.FrustumAndRaycast:
                isLookingNow = IsLookingAtPainting_FrustumAndRaycast();
                break;
            case DetectionMode.MultipleRaycasts:
                isLookingNow = IsLookingAtPainting_MultipleRaycasts();
                break;
        }

        // Si está mirando el cuadro
        if (isLookingNow)
        {
            wasLookingLastFrame = true;

            // Si aún no ha visto el cuadro el tiempo suficiente, acumular tiempo
            if (!hasSeenPainting)
            {
                currentLookTime += Time.deltaTime;

                // Log cada 0.1 segundos para no saturar la consola
                if (currentLookTime % 0.1f < Time.deltaTime)
                {
                    Debug.Log("PaintingTransition: Jugador mirando el cuadro. Tiempo acumulado: " + currentLookTime.ToString("F2") + "s / " + lookDuration + "s");
                }

                // Si ha mirado el tiempo suficiente, marcar como visto
                if (currentLookTime >= lookDuration)
                {
                    hasSeenPainting = true;
                    Debug.Log("PaintingTransition: ¡Umbral alcanzado! El jugador ha visto el cuadro. Esperando a que deje de mirarlo para iniciar transición...");
                }
            }
        }
        else // No está mirando el cuadro
        {
            // Detectar si ACABA DE DEJAR de mirar (transición de mirando a no mirando)
            if (wasLookingLastFrame && hasSeenPainting && !transitionCompleted)
            {
                Debug.Log("PaintingTransition: ¡El jugador dejó de mirar el cuadro! Iniciando transición...");
                StartTransition();
            }

            wasLookingLastFrame = false;

            // Si no ha completado el umbral de tiempo, resetear
            if (!hasSeenPainting && currentLookTime > 0f)
            {
                Debug.Log("PaintingTransition: Jugador dejó de mirar antes de completar el umbral. Reseteando tiempo acumulado.");
                currentLookTime = 0f;
            }
        }
    }

    /// <summary>
    /// MODO 1: Verifica si el jugador está mirando el cuadro mediante un solo raycast central
    /// </summary>
    private bool IsLookingAtPainting_SingleRaycast()
    {
        // Crear un raycast desde el centro de la cámara hacia adelante
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Realizar el raycast
        if (Physics.Raycast(ray, out hit, maxViewDistance))
        {
            // Log del objeto golpeado (solo cuando no estaba mirando antes para evitar spam)
            if (!wasLookingLastFrame)
            {
                Debug.Log("PaintingTransition [SingleRaycast]: Raycast impactó: " + hit.collider.gameObject.name + " (Distancia: " + hit.distance.ToString("F2") + "m)");
            }

            // Verificar si el objeto golpeado es uno de los cuadros
            GameObject hitObject = hit.collider.gameObject;
            
            // Comprobar si es el cuadro normal o el oscuro (o sus padres)
            if (hitObject == normalPainting || hitObject == darkPainting ||
                hitObject.transform.parent == normalPainting.transform || 
                hitObject.transform.parent == darkPainting.transform)
            {
                if (!wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [SingleRaycast]: ¡Raycast detectó el cuadro correctamente!");
                }
                return true;
            }
            else
            {
                if (wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [SingleRaycast]: Raycast golpeó otro objeto, no es el cuadro.");
                }
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// MODO 2: Verifica si el cuadro está dentro del frustum de la cámara (campo de visión)
    /// </summary>
    private bool IsLookingAtPainting_FrustumCheck()
    {
        // Obtener los planos del frustum de la cámara
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        
        // Obtener los bounds del cuadro
        paintingBounds = paintingRenderer.bounds;

        // Verificar si los bounds están dentro del frustum
        bool inFrustum = GeometryUtility.TestPlanesAABB(frustumPlanes, paintingBounds);

        if (inFrustum)
        {
            // Verificar también la distancia
            float distance = Vector3.Distance(playerCamera.transform.position, paintingBounds.center);
            
            if (distance <= maxViewDistance)
            {
                if (!wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [FrustumCheck]: ¡Cuadro visible en el frustum! Distancia: " + distance.ToString("F2") + "m");
                }
                return true;
            }
            else
            {
                if (wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [FrustumCheck]: Cuadro en frustum pero demasiado lejos: " + distance.ToString("F2") + "m");
                }
            }
        }

        return false;
    }

    /// <summary>
    /// MODO 3: Verifica frustum + raycast de confirmación (RECOMENDADO)
    /// Primero verifica si está en el campo de visión, luego confirma con raycast que no hay obstrucciones
    /// </summary>
    private bool IsLookingAtPainting_FrustumAndRaycast()
    {
        // Paso 1: Verificar si está en el frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        paintingBounds = paintingRenderer.bounds;
        bool inFrustum = GeometryUtility.TestPlanesAABB(frustumPlanes, paintingBounds);

        if (!inFrustum)
        {
            // Si no está en el frustum, no hace falta hacer raycast
            return false;
        }

        // Verificar distancia
        float distance = Vector3.Distance(playerCamera.transform.position, paintingBounds.center);
        if (distance > maxViewDistance)
        {
            if (wasLookingLastFrame)
            {
                Debug.Log("PaintingTransition [FrustumAndRaycast]: Cuadro en frustum pero demasiado lejos: " + distance.ToString("F2") + "m");
            }
            return false;
        }

        if (!wasLookingLastFrame)
        {
            Debug.Log("PaintingTransition [FrustumAndRaycast]: Cuadro visible en frustum. Verificando obstrucciones con raycast...");
        }

        // Paso 2: Hacer raycast hacia el centro del cuadro para confirmar que no hay obstrucciones
        Vector3 directionToPainting = (paintingBounds.center - playerCamera.transform.position).normalized;
        Ray ray = new Ray(playerCamera.transform.position, directionToPainting);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxViewDistance))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            // Verificar si el primer objeto golpeado es el cuadro
            if (hitObject == normalPainting || hitObject == darkPainting ||
                hitObject.transform.parent == normalPainting.transform || 
                hitObject.transform.parent == darkPainting.transform)
            {
                if (!wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [FrustumAndRaycast]: ¡Cuadro confirmado visible sin obstrucciones! Distancia: " + hit.distance.ToString("F2") + "m");
                }
                return true;
            }
            else
            {
                if (wasLookingLastFrame)
                {
                    Debug.Log("PaintingTransition [FrustumAndRaycast]: Hay obstrucción. Objeto bloqueando: " + hitObject.name);
                }
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// MODO 4: Múltiples raycasts en patrón para mejor cobertura
    /// Lanza varios raycasts en diferentes puntos del cuadro
    /// </summary>
    private bool IsLookingAtPainting_MultipleRaycasts()
    {
        // Primero verificar si está en el frustum para optimizar
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        paintingBounds = paintingRenderer.bounds;
        bool inFrustum = GeometryUtility.TestPlanesAABB(frustumPlanes, paintingBounds);

        if (!inFrustum)
        {
            return false;
        }

        // Verificar distancia
        float distance = Vector3.Distance(playerCamera.transform.position, paintingBounds.center);
        if (distance > maxViewDistance)
        {
            return false;
        }

        // Definir puntos de muestra en el cuadro: centro, esquinas y puntos medios
        Vector3[] samplePoints = new Vector3[]
        {
            paintingBounds.center, // Centro
            paintingBounds.center + new Vector3(paintingBounds.extents.x * 0.5f, paintingBounds.extents.y * 0.5f, 0), // Superior derecha
            paintingBounds.center + new Vector3(-paintingBounds.extents.x * 0.5f, paintingBounds.extents.y * 0.5f, 0), // Superior izquierda
            paintingBounds.center + new Vector3(paintingBounds.extents.x * 0.5f, -paintingBounds.extents.y * 0.5f, 0), // Inferior derecha
            paintingBounds.center + new Vector3(-paintingBounds.extents.x * 0.5f, -paintingBounds.extents.y * 0.5f, 0), // Inferior izquierda
        };

        int successfulRaycasts = 0;
        int totalRaycasts = samplePoints.Length;

        // Lanzar raycast a cada punto
        foreach (Vector3 point in samplePoints)
        {
            Vector3 direction = (point - playerCamera.transform.position).normalized;
            Ray ray = new Ray(playerCamera.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxViewDistance))
            {
                GameObject hitObject = hit.collider.gameObject;
                
                // Verificar si golpeó el cuadro
                if (hitObject == normalPainting || hitObject == darkPainting ||
                    hitObject.transform.parent == normalPainting.transform || 
                    hitObject.transform.parent == darkPainting.transform)
                {
                    successfulRaycasts++;
                }
            }
        }

        // Si al menos 3 de 5 raycasts golpean el cuadro, consideramos que está siendo visto
        bool isVisible = successfulRaycasts >= 3;

        if (isVisible && !wasLookingLastFrame)
        {
            Debug.Log("PaintingTransition [MultipleRaycasts]: Cuadro visible. Raycasts exitosos: " + successfulRaycasts + "/" + totalRaycasts);
        }

        return isVisible;
    }

    /// <summary>
    /// Inicia la transición entre el cuadro normal y el oscuro
    /// </summary>
    private void StartTransition()
    {
        Debug.Log("PaintingTransition: Iniciando transición. Comenzando corrutina...");
        StartCoroutine(TransitionCoroutine());
    }

    /// <summary>
    /// Corrutina que maneja la transición gradual entre los cuadros
    /// </summary>
    private IEnumerator TransitionCoroutine()
    {
        float elapsedTime = 0f;

        Debug.Log("PaintingTransition: Corrutina iniciada. Duración total: " + transitionDuration + " segundos.");

        // Durante la transición, intercambiar los cuadros gradualmente
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calcular el progreso de la transición (0 a 1)
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Log del progreso cada 0.5 segundos
            if (elapsedTime % 0.5f < Time.deltaTime)
            {
                Debug.Log("PaintingTransition: Progreso de transición: " + (progress * 100f).ToString("F1") + "% (" + elapsedTime.ToString("F1") + "s / " + transitionDuration + "s)");
            }

            // Esperar al siguiente frame
            yield return null;
        }

        // Al finalizar la transición: ocultar el cuadro normal y mostrar el oscuro
        normalPainting.SetActive(false);
        darkPainting.SetActive(true);

        transitionCompleted = true;

        Debug.Log("PaintingTransition: ¡Transición completada! Cuadro normal oculto, cuadro oscuro visible.");
    }
}