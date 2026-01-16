using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

namespace UnityEngine.XR.Content.Interaction
{
    [RequireComponent(typeof(AudioSource))]
    public class XRKnobLever : XRBaseInteractable
    {
        // --- NUEVO: Enum para elegir el eje ---
        public enum RotationAxis
        {
            X, // Para palancas de pared (bajar/subir)
            Y, // Para dials en mesas (izquierda/derecha) - Default
            Z  // Para volantes o válvulas frontales
        }
        // --------------------------------------

        [Header("Hand GameObjects")]
        public GameObject objectHand;

        [Header("Rotation Settings")]
        [SerializeField] private RotationAxis m_RotationAxis = RotationAxis.Y; // Selector en el Inspector
        [SerializeField] private bool m_InvertDirection = false;

        [Header("SFXs Settings")]
        [SerializeField] private AudioClip moveSFX;
        [SerializeField] private AudioClip activateSFX;
        [SerializeField] private float maxVolume = 1.0f;
        [SerializeField] private float fadeSpeed = 5.0f;

        private float m_TargetVolume = 0f;
        private bool m_WasAboveThreshold;
        private float m_Threshold = 0.5f;

        const float k_ModeSwitchDeadZone = 0.1f;

        private AudioSource m_AudioSource;

        struct TrackedRotation
        {
            float m_BaseAngle;
            float m_CurrentOffset;
            float m_AccumulatedAngle;
            public float totalOffset => m_AccumulatedAngle + m_CurrentOffset;

            public void Reset()
            {
                m_BaseAngle = 0.0f;
                m_CurrentOffset = 0.0f;
                m_AccumulatedAngle = 0.0f;
            }

            public void SetBaseFromVector(Vector3 direction)
            {
                m_AccumulatedAngle += m_CurrentOffset;
                // Atan2 usa (-x, z) asumiendo rotación en Y. 
                // Nosotros le pasaremos los vectores ya "trucados" para que esto siempre funcione.
                m_BaseAngle = Mathf.Atan2(-direction.x, direction.z) * Mathf.Rad2Deg;
                m_CurrentOffset = 0.0f;
            }

            public void SetTargetFromVector(Vector3 direction)
            {
                var targetAngle = Mathf.Atan2(-direction.x, direction.z) * Mathf.Rad2Deg;
                m_CurrentOffset = ShortestAngleDistance(m_BaseAngle, targetAngle, 360.0f);

                if (Mathf.Abs(m_CurrentOffset) > 90.0f)
                {
                    m_BaseAngle = targetAngle;
                    m_AccumulatedAngle += m_CurrentOffset;
                    m_CurrentOffset = 0.0f;
                }
            }
        }

        [Serializable]
        public class ValueChangeEvent : UnityEvent<float> { }

        [SerializeField] Transform m_Handle = null;
        [SerializeField][Range(0.0f, 1.0f)] float m_Value = 0.5f;
        [SerializeField] bool m_ClampedMotion = true;
        [SerializeField] float m_MaxAngle = 90.0f;
        [SerializeField] float m_MinAngle = -90.0f;
        [SerializeField] float m_AngleIncrement = 0.0f;
        [SerializeField] float m_PositionTrackedRadius = 0.1f;
        [SerializeField] float m_TwistSensitivity = 1.5f;
        [SerializeField] ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

        IXRSelectInteractor m_Interactor;
        bool m_PositionDriven = false;
        bool m_UpVectorDriven = false;

        TrackedRotation m_PositionAngles = new TrackedRotation();
        TrackedRotation m_UpVectorAngles = new TrackedRotation();
        TrackedRotation m_ForwardVectorAngles = new TrackedRotation();

        float m_BaseKnobRotation = 0.0f;

        public Transform handle { get => m_Handle; set => m_Handle = value; }
        public float value
        {
            get => m_Value;
            set
            {
                SetValue(value);
                SetKnobRotation(ValueToRotation());
            }
        }
        public bool clampedMotion { get => m_ClampedMotion; set => m_ClampedMotion = value; }
        public float maxAngle { get => m_MaxAngle; set => m_MaxAngle = value; }
        public float minAngle { get => m_MinAngle; set => m_MinAngle = value; }
        public float positionTrackedRadius { get => m_PositionTrackedRadius; set => m_PositionTrackedRadius = value; }
        public ValueChangeEvent onValueChange => m_OnValueChange;

        // --- NUEVO: Propiedad para cambiar eje desde código si hace falta ---
        public RotationAxis axisToRotate
        {
            get => m_RotationAxis;
            set { m_RotationAxis = value; SetKnobRotation(ValueToRotation()); }
        }

        public bool invertDirection
        {
            get => m_InvertDirection;
            set => m_InvertDirection = value;
        }

        void Start()
        {
            SetValue(m_Value);
            SetKnobRotation(ValueToRotation());

            m_AudioSource = GetComponent<AudioSource>();
            if (moveSFX != null)
            {
                m_AudioSource.clip = moveSFX;
                m_AudioSource.loop = true;
                m_AudioSource.volume = 0f;
                m_AudioSource.Play();
            }
        }

        void Update()
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.volume = Mathf.MoveTowards(m_AudioSource.volume, m_TargetVolume, fadeSpeed * Time.deltaTime);

                if (m_AudioSource.volume <= 0.001f && m_AudioSource.isPlaying) { }
                else if (m_AudioSource.volume > 0.001f && !m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            if (objectHand != null) objectHand.SetActive(false);
            base.OnDisable();
        }

        void StartGrab(SelectEnterEventArgs args)
        {
            if (objectHand != null) objectHand.SetActive(true);
            m_Interactor = args.interactorObject;
            m_PositionAngles.Reset();
            m_UpVectorAngles.Reset();
            m_ForwardVectorAngles.Reset();
            UpdateBaseKnobRotation();
            UpdateRotation(true);
        }

        void EndGrab(SelectExitEventArgs args)
        {
            if (objectHand != null) objectHand.SetActive(false);
            m_TargetVolume = 0f;
            m_Interactor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    UpdateRotation();
                }
            }
        }

        // --- LÓGICA DE TRANSFORMACIÓN DE EJES ---
        // Esta función convierte la posición real de la mano a un espacio local 
        // donde el eje Y siempre es el eje de rotación para que las matemáticas no fallen.
        Vector3 TransformToMathSpace(Vector3 localVector)
        {
            switch (m_RotationAxis)
            {
                case RotationAxis.X:
                    // Si rotamos en X, usamos Y y Z para calcular el ángulo
                    // Mapeamos: Y -> X (ancho), X -> Y (altura ignorada), Z -> Z (profundidad)
                    return new Vector3(localVector.y, localVector.x, localVector.z);

                case RotationAxis.Z:
                    // Si rotamos en Z, usamos X e Y para calcular el ángulo
                    // Mapeamos: X -> X, Z -> Y (altura ignorada), Y -> Z (profundidad matemática)
                    return new Vector3(localVector.x, localVector.z, localVector.y);

                case RotationAxis.Y:
                default:
                    // Comportamiento original
                    return localVector;
            }
        }
        // ----------------------------------------

        void UpdateRotation(bool freshCheck = false)
        {
            var interactorTransform = m_Interactor.GetAttachTransform(this);

            var localOffset = transform.InverseTransformPoint(interactorTransform.position);
            var localForward = transform.InverseTransformDirection(interactorTransform.forward);
            var localUp = transform.InverseTransformDirection(interactorTransform.up);

            // 1. Convertimos los vectores al espacio matemático donde Y siempre es el eje de rotación
            localOffset = TransformToMathSpace(localOffset);
            localForward = TransformToMathSpace(localForward);
            localUp = TransformToMathSpace(localUp);

            // 2. Aplanamos la altura (que ahora siempre es Y en nuestro espacio matemático)
            localOffset.y = 0.0f;
            var radiusOffset = transform.TransformVector(localOffset).magnitude; // Ojo: magnitud aproximada
            localOffset.Normalize();

            var localY = Math.Abs(localForward.y); // Y ahora representa el eje paralelo a la rotación
            localForward.y = 0.0f;
            localForward.Normalize();

            localUp.y = 0.0f;
            localUp.Normalize();

            // El resto de la lógica matemática es idéntica a la original, 
            // pero operando sobre los vectores transformados.

            if (m_PositionDriven && !freshCheck)
                radiusOffset *= (1.0f + k_ModeSwitchDeadZone);

            if (radiusOffset >= m_PositionTrackedRadius)
            {
                if (!m_PositionDriven || freshCheck)
                {
                    m_PositionAngles.SetBaseFromVector(localOffset);
                    m_PositionDriven = true;
                }
            }
            else
                m_PositionDriven = false;

            if (!freshCheck)
            {
                if (!m_UpVectorDriven)
                    localY *= (1.0f - (k_ModeSwitchDeadZone * 0.5f));
                else
                    localY *= (1.0f + (k_ModeSwitchDeadZone * 0.5f));
            }

            if (localY > 0.707f)
            {
                if (!m_UpVectorDriven || freshCheck)
                {
                    m_UpVectorAngles.SetBaseFromVector(localUp);
                    m_UpVectorDriven = true;
                }
            }
            else
            {
                if (m_UpVectorDriven || freshCheck)
                {
                    m_ForwardVectorAngles.SetBaseFromVector(localForward);
                    m_UpVectorDriven = false;
                }
            }

            if (m_PositionDriven)
                m_PositionAngles.SetTargetFromVector(localOffset);

            if (m_UpVectorDriven)
                m_UpVectorAngles.SetTargetFromVector(localUp);
            else
                m_ForwardVectorAngles.SetTargetFromVector(localForward);

            var totalAngleChange = ((m_UpVectorAngles.totalOffset + m_ForwardVectorAngles.totalOffset) * m_TwistSensitivity) + m_PositionAngles.totalOffset;

            // 2. Si InvertDirection está activo, cambiamos el signo del movimiento
            if (m_InvertDirection) totalAngleChange *= -1.0f;

            // 3. Aplicamos el cambio (El script original Resta el cambio, así que mantenemos esa lógica)
            var knobRotation = m_BaseKnobRotation - totalAngleChange;
            
            if (m_ClampedMotion)
                knobRotation = Mathf.Clamp(knobRotation, m_MinAngle, m_MaxAngle);

            SetKnobRotation(knobRotation);

            var knobValue = (knobRotation - m_MinAngle) / (m_MaxAngle - m_MinAngle);

            float previousValue = m_Value;
            SetValue(knobValue);
            float diff = Mathf.Abs(m_Value - previousValue);

            bool isAbove = m_Value > m_Threshold;

            if (isAbove != m_WasAboveThreshold)
            {
                if (diff > 0.0012 && activateSFX != null)
                {
                    m_AudioSource.PlayOneShot(activateSFX);
                }
                m_WasAboveThreshold = isAbove;
            }

            if (diff > 0.0002f) m_TargetVolume = maxVolume;
            else m_TargetVolume = 0f;
        }

        void SetKnobRotation(float angle)
        {
            if (m_AngleIncrement > 0)
            {
                var normalizeAngle = angle - m_MinAngle;
                angle = (Mathf.Round(normalizeAngle / m_AngleIncrement) * m_AngleIncrement) + m_MinAngle;
            }

            if (m_Handle != null)
            {
                // --- NUEVO: Aplicar la rotación al eje correcto ---
                switch (m_RotationAxis)
                {
                    case RotationAxis.X:
                        m_Handle.localEulerAngles = new Vector3(angle, 0.0f, 0.0f);
                        break;
                    case RotationAxis.Z:
                        m_Handle.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
                        break;
                    case RotationAxis.Y:
                    default:
                        m_Handle.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
                        break;
                }
                // --------------------------------------------------
            }
        }

        void SetValue(float value)
        {
            if (m_ClampedMotion)
                value = Mathf.Clamp01(value);

            if (m_AngleIncrement > 0)
            {
                var angleRange = m_MaxAngle - m_MinAngle;
                var angle = Mathf.Lerp(0.0f, angleRange, value);
                angle = Mathf.Round(angle / m_AngleIncrement) * m_AngleIncrement;
                value = Mathf.InverseLerp(0.0f, angleRange, angle);
            }

            m_Value = value;
            m_OnValueChange.Invoke(m_Value);
        }

        float ValueToRotation()
        {
            return m_ClampedMotion ? Mathf.Lerp(m_MinAngle, m_MaxAngle, m_Value) : Mathf.LerpUnclamped(m_MinAngle, m_MaxAngle, m_Value);
        }

        void UpdateBaseKnobRotation()
        {
            m_BaseKnobRotation = Mathf.LerpUnclamped(m_MinAngle, m_MaxAngle, m_Value);
        }

        static float ShortestAngleDistance(float start, float end, float max)
        {
            var angleDelta = end - start;
            var angleSign = Mathf.Sign(angleDelta);

            angleDelta = Math.Abs(angleDelta) % max;
            if (angleDelta > (max * 0.5f))
                angleDelta = -(max - angleDelta);

            return angleDelta * angleSign;
        }

        void OnDrawGizmosSelected()
        {
            const int k_CircleSegments = 16;
            const float k_SegmentRatio = 1.0f / k_CircleSegments;

            if (m_PositionTrackedRadius <= Mathf.Epsilon)
                return;

            var circleCenter = transform.position;

            if (m_Handle != null)
                circleCenter = m_Handle.position;

            // --- NUEVO: Gizmos adaptados al eje ---
            Vector3 circleX, circleY;

            switch (m_RotationAxis)
            {
                case RotationAxis.X: // Plano YZ
                    circleX = transform.up;
                    circleY = transform.forward;
                    break;
                case RotationAxis.Z: // Plano XY
                    circleX = transform.right;
                    circleY = transform.up;
                    break;
                case RotationAxis.Y: // Plano XZ (Default)
                default:
                    circleX = transform.right;
                    circleY = transform.forward;
                    break;
            }
            // --------------------------------------

            Gizmos.color = Color.green;
            var segmentCounter = 0;
            while (segmentCounter < k_CircleSegments)
            {
                var startAngle = (float)segmentCounter * k_SegmentRatio * 2.0f * Mathf.PI;
                segmentCounter++;
                var endAngle = (float)segmentCounter * k_SegmentRatio * 2.0f * Mathf.PI;

                Gizmos.DrawLine(circleCenter + (Mathf.Cos(startAngle) * circleX + Mathf.Sin(startAngle) * circleY) * m_PositionTrackedRadius,
                    circleCenter + (Mathf.Cos(endAngle) * circleX + Mathf.Sin(endAngle) * circleY) * m_PositionTrackedRadius);
            }
        }

        void OnValidate()
        {
            if (m_ClampedMotion)
                m_Value = Mathf.Clamp01(m_Value);

            if (m_MinAngle > m_MaxAngle)
                m_MinAngle = m_MaxAngle;

            SetKnobRotation(ValueToRotation());
        }

        public void ForceEndInteractionAndFade()
        {
            // 1. Soltar la mano físicamente si está agarrado
            if (isSelected && interactorsSelecting.Count > 0)
            {
                interactionManager.SelectExit(interactorsSelecting[0], this);
            }

            // 2. Resetear el valor lógico
            m_Value = 0;

            // 3. Arrancar la corrutina de Fade Out
            // Importante: Las corrutinas siguen vivas aunque deshabilitemos el componente justo después
            StartCoroutine(FadeOutAndDisableRoutine());
        }

        private IEnumerator FadeOutAndDisableRoutine()
        {
            // Mientras siga sonando...
            while (m_AudioSource != null && m_AudioSource.volume > 0)
            {
                // Bajamos el volumen usando la misma velocidad que configuraste (fadeSpeed)
                m_AudioSource.volume = Mathf.MoveTowards(m_AudioSource.volume, 0f, fadeSpeed * Time.deltaTime);

                // Esperamos al siguiente frame
                yield return null;
            }

            // 4. Una vez en silencio, paramos el audio y desactivamos el script
            if (m_AudioSource != null)
            {
                m_AudioSource.Stop();
                m_AudioSource.volume = 0;
            }

            // Aquí es donde el script se "suicida" (se desactiva a sí mismo)
            this.enabled = false;
        }
    }
}