using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Content.Interaction
{
    [RequireComponent(typeof(AudioSource))]
    public class XRKnobLever : XRBaseInteractable
    {
        [Header("Hand GameObjects")]
        public GameObject objectHand;

        [Header("SFXs Settings")]
        [SerializeField] private AudioClip moveSFX;
        [SerializeField] private AudioClip activateSFX;
        [SerializeField] private float maxVolume = 1.0f; // Volumen máximo deseado
        [SerializeField] private float fadeSpeed = 5.0f; // Qué tan rápido sube/baja el volumen (Más alto = más rápido)

        const float k_ModeSwitchDeadZone = 0.1f;

        private AudioSource m_AudioSource;
        private float m_TargetVolume = 0f; // A qué volumen QUEREMOS ir

        // Structs originales...
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
                m_BaseAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                m_CurrentOffset = 0.0f;
            }

            public void SetTargetFromVector(Vector3 direction)
            {
                var targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
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

        // Propiedades públicas...
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

        void Start()
        {
            SetValue(m_Value);
            SetKnobRotation(ValueToRotation());

            // CONFIGURACIÓN DE AUDIO INICIAL
            m_AudioSource = GetComponent<AudioSource>();
            if (moveSFX != null)
            {
                m_AudioSource.clip = moveSFX;
                m_AudioSource.loop = true;
                m_AudioSource.volume = 0f; // Empieza en silencio
                m_AudioSource.Play();      // Siempre reproduciendo
            }
        }

        // --- AÑADIDO: Update normal para gestionar el Fading ---
        void Update()
        {
            if (m_AudioSource != null)
            {
                // MoveTowards hace la magia: mueve el volumen actual hacia el target paso a paso
                m_AudioSource.volume = Mathf.MoveTowards(m_AudioSource.volume, m_TargetVolume, fadeSpeed * Time.deltaTime);

                // Opcional: Si el volumen es casi 0, pausamos para ahorrar CPU (o lo dejamos si quieres reactividad instantánea)
                if (m_AudioSource.volume <= 0.001f && m_AudioSource.isPlaying)
                {
                    // m_AudioSource.Pause(); // Descomenta si quieres optimizar
                }
                else if (m_AudioSource.volume > 0.001f && !m_AudioSource.isPlaying)
                {
                    m_AudioSource.Play();
                }
            }
        }
        // -----------------------------------------------------

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

            // AL SOLTAR: El objetivo es silencio
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

        void UpdateRotation(bool freshCheck = false)
        {
            // (Lógica de rotación original sin cambios hasta abajo...)
            var interactorTransform = m_Interactor.GetAttachTransform(this);

            var localOffset = transform.InverseTransformVector(interactorTransform.position - m_Handle.position);
            localOffset.y = 0.0f;
            var radiusOffset = transform.TransformVector(localOffset).magnitude;
            localOffset.Normalize();

            var localForward = transform.InverseTransformDirection(interactorTransform.forward);
            var localY = Math.Abs(localForward.y);
            localForward.y = 0.0f;
            localForward.Normalize();

            var localUp = transform.InverseTransformDirection(interactorTransform.up);
            localUp.y = 0.0f;
            localUp.Normalize();


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

            var knobRotation = m_BaseKnobRotation - ((m_UpVectorAngles.totalOffset + m_ForwardVectorAngles.totalOffset) * m_TwistSensitivity) - m_PositionAngles.totalOffset;

            if (m_ClampedMotion)
                knobRotation = Mathf.Clamp(knobRotation, m_MinAngle, m_MaxAngle);

            SetKnobRotation(knobRotation);

            var knobValue = (knobRotation - m_MinAngle) / (m_MaxAngle - m_MinAngle);

            // --- NUEVA LÓGICA DE AUDIO (TARGET VOLUME) ---
            float previousValue = m_Value;
            SetValue(knobValue);
            float diff = Mathf.Abs(m_Value - previousValue);

            // Si hay movimiento significativo
            if (diff > 0.0001f) // Umbral muy bajo para detectar cualquier micro-movimiento
            {
                m_TargetVolume = maxVolume; // Queremos subir al máximo
            }
            else
            {
                m_TargetVolume = 0f; // Queremos silencio (pero con fade out gracias al Update)
            }
            // ---------------------------------------------
        }

        void SetKnobRotation(float angle)
        {
            if (m_AngleIncrement > 0)
            {
                var normalizeAngle = angle - m_MinAngle;
                angle = (Mathf.Round(normalizeAngle / m_AngleIncrement) * m_AngleIncrement) + m_MinAngle;
            }

            if (m_Handle != null)
                m_Handle.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
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

            var circleX = transform.right;
            var circleY = transform.forward;

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
    }
}
