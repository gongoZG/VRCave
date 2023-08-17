#region Assembly Unity.XR.Interaction.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// D:\NF\CarTest\Library\ScriptAssemblies\Unity.XR.Interaction.Toolkit.dll
// Decompiled with ICSharpCode.Decompiler 6.1.0.5902
#endregion

using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [CanSelectMultiple(false)]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/XR Grab Interactable", 11)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/api/UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable.html")]
    public class XRGrabInteractable : XRBaseInteractable
    {
        public enum AttachPointCompatibilityMode
        {
            Default,
            Legacy
        }

        private const float k_DefaultTighteningAmount = 0.5f;

        private const float k_DefaultSmoothingAmount = 5f;

        private const float k_VelocityDamping = 1f;

        private const float k_VelocityScale = 1f;

        private const float k_AngularVelocityDamping = 1f;

        private const float k_AngularVelocityScale = 1f;

        private const int k_ThrowSmoothingFrameCount = 20;

        private const float k_DefaultAttachEaseInTime = 0.15f;

        private const float k_DefaultThrowSmoothingDuration = 0.25f;

        private const float k_DefaultThrowVelocityScale = 1.5f;

        private const float k_DefaultThrowAngularVelocityScale = 1f;

        [SerializeField]
        private Transform m_AttachTransform;

        [SerializeField]
        private float m_AttachEaseInTime = 0.15f;

        [SerializeField]
        private MovementType m_MovementType = MovementType.Instantaneous;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_VelocityDamping = 1f;

        [SerializeField]
        private float m_VelocityScale = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_AngularVelocityDamping = 1f;

        [SerializeField]
        private float m_AngularVelocityScale = 1f;

        [SerializeField]
        private bool m_TrackPosition = true;

        [SerializeField]
        private bool m_SmoothPosition;

        [SerializeField]
        [Range(0f, 20f)]
        private float m_SmoothPositionAmount = 5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_TightenPosition = 0.5f;

        [SerializeField]
        private bool m_TrackRotation = true;

        [SerializeField]
        private bool m_SmoothRotation;

        [SerializeField]
        [Range(0f, 20f)]
        private float m_SmoothRotationAmount = 5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_TightenRotation = 0.5f;

        [SerializeField]
        private bool m_ThrowOnDetach = true;

        [SerializeField]
        private float m_ThrowSmoothingDuration = 0.25f;

        [SerializeField]
        private AnimationCurve m_ThrowSmoothingCurve = AnimationCurve.Linear(1f, 1f, 1f, 0f);

        [SerializeField]
        private float m_ThrowVelocityScale = 1.5f;

        [SerializeField]
        private float m_ThrowAngularVelocityScale = 1f;

        [SerializeField]
        [FormerlySerializedAs("m_GravityOnDetach")]
        private bool m_ForceGravityOnDetach;

        [SerializeField]
        private bool m_RetainTransformParent = true;

        [SerializeField]
        private AttachPointCompatibilityMode m_AttachPointCompatibilityMode;

        private Vector3 m_InteractorLocalPosition;

        private Quaternion m_InteractorLocalRotation;

        private Vector3 m_TargetWorldPosition;

        private Quaternion m_TargetWorldRotation;

        private float m_CurrentAttachEaseTime;

        private MovementType m_CurrentMovementType;

        private bool m_DetachInLateUpdate;

        private Vector3 m_DetachVelocity;

        private Vector3 m_DetachAngularVelocity;

        private int m_ThrowSmoothingCurrentFrame;

        private readonly float[] m_ThrowSmoothingFrameTimes = new float[20];

        private readonly Vector3[] m_ThrowSmoothingVelocityFrames = new Vector3[20];

        private readonly Vector3[] m_ThrowSmoothingAngularVelocityFrames = new Vector3[20];

        private Rigidbody m_Rigidbody;

        private Vector3 m_LastPosition;

        private Quaternion m_LastRotation;

        private bool m_WasKinematic;

        private bool m_UsedGravity;

        private float m_OldDrag;

        private float m_OldAngularDrag;

        private Transform m_OriginalSceneParent;

        private TeleportationProvider m_TeleportationProvider;

        private Pose m_PoseBeforeTeleport;

        public Transform attachTransform
        {
            get
            {
                return m_AttachTransform;
            }
            set
            {
                m_AttachTransform = value;
            }
        }

        public float attachEaseInTime
        {
            get
            {
                return m_AttachEaseInTime;
            }
            set
            {
                m_AttachEaseInTime = value;
            }
        }

        public MovementType movementType
        {
            get
            {
                return m_MovementType;
            }
            set
            {
                m_MovementType = value;
                if (base.isSelected)
                {
                    SetupRigidbodyDrop(m_Rigidbody);
                    UpdateCurrentMovementType();
                    SetupRigidbodyGrab(m_Rigidbody);
                }
            }
        }

        public float velocityDamping
        {
            get
            {
                return m_VelocityDamping;
            }
            set
            {
                m_VelocityDamping = value;
            }
        }

        public float velocityScale
        {
            get
            {
                return m_VelocityScale;
            }
            set
            {
                m_VelocityScale = value;
            }
        }

        public float angularVelocityDamping
        {
            get
            {
                return m_AngularVelocityDamping;
            }
            set
            {
                m_AngularVelocityDamping = value;
            }
        }

        public float angularVelocityScale
        {
            get
            {
                return m_AngularVelocityScale;
            }
            set
            {
                m_AngularVelocityScale = value;
            }
        }

        public bool trackPosition
        {
            get
            {
                return m_TrackPosition;
            }
            set
            {
                m_TrackPosition = value;
            }
        }

        public bool smoothPosition
        {
            get
            {
                return m_SmoothPosition;
            }
            set
            {
                m_SmoothPosition = value;
            }
        }

        public float smoothPositionAmount
        {
            get
            {
                return m_SmoothPositionAmount;
            }
            set
            {
                m_SmoothPositionAmount = value;
            }
        }

        public float tightenPosition
        {
            get
            {
                return m_TightenPosition;
            }
            set
            {
                m_TightenPosition = value;
            }
        }

        public bool trackRotation
        {
            get
            {
                return m_TrackRotation;
            }
            set
            {
                m_TrackRotation = value;
            }
        }

        public bool smoothRotation
        {
            get
            {
                return m_SmoothRotation;
            }
            set
            {
                m_SmoothRotation = value;
            }
        }

        public float smoothRotationAmount
        {
            get
            {
                return m_SmoothRotationAmount;
            }
            set
            {
                m_SmoothRotationAmount = value;
            }
        }

        public float tightenRotation
        {
            get
            {
                return m_TightenRotation;
            }
            set
            {
                m_TightenRotation = value;
            }
        }

        public bool throwOnDetach
        {
            get
            {
                return m_ThrowOnDetach;
            }
            set
            {
                m_ThrowOnDetach = value;
            }
        }

        public float throwSmoothingDuration
        {
            get
            {
                return m_ThrowSmoothingDuration;
            }
            set
            {
                m_ThrowSmoothingDuration = value;
            }
        }

        public AnimationCurve throwSmoothingCurve
        {
            get
            {
                return m_ThrowSmoothingCurve;
            }
            set
            {
                m_ThrowSmoothingCurve = value;
            }
        }

        public float throwVelocityScale
        {
            get
            {
                return m_ThrowVelocityScale;
            }
            set
            {
                m_ThrowVelocityScale = value;
            }
        }

        public float throwAngularVelocityScale
        {
            get
            {
                return m_ThrowAngularVelocityScale;
            }
            set
            {
                m_ThrowAngularVelocityScale = value;
            }
        }

        public bool forceGravityOnDetach
        {
            get
            {
                return m_ForceGravityOnDetach;
            }
            set
            {
                m_ForceGravityOnDetach = value;
            }
        }

        public bool retainTransformParent
        {
            get
            {
                return m_RetainTransformParent;
            }
            set
            {
                m_RetainTransformParent = value;
            }
        }

        public AttachPointCompatibilityMode attachPointCompatibilityMode
        {
            get
            {
                return m_AttachPointCompatibilityMode;
            }
            set
            {
                m_AttachPointCompatibilityMode = value;
            }
        }

        [Obsolete("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach")]
        public bool gravityOnDetach
        {
            get
            {
                return forceGravityOnDetach;
            }
            set
            {
                forceGravityOnDetach = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_CurrentMovementType = m_MovementType;
            m_Rigidbody = GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
            {
                Debug.LogError("Grab Interactable does not have a required Rigidbody.", this);
            }
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    if (base.isSelected)
                    {
                        if (m_CurrentMovementType == MovementType.Kinematic)
                        {
                            PerformKinematicUpdate(updatePhase);
                        }
                        else if (m_CurrentMovementType == MovementType.VelocityTracking)
                        {
                            PerformVelocityTrackingUpdate(Time.deltaTime, updatePhase);
                        }
                    }

                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (base.isSelected)
                    {
                        IXRSelectInteractor interactor2 = base.interactorsSelecting[0];
                        if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default)
                        {
                            UpdateInteractorLocalPose(interactor2);
                        }

                        UpdateTarget(interactor2, Time.deltaTime);
                        SmoothVelocityUpdate(interactor2);
                        if (m_CurrentMovementType == MovementType.Instantaneous)
                        {
                            PerformInstantaneousUpdate(updatePhase);
                        }
                    }

                    break;
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    if (base.isSelected)
                    {
                        IXRSelectInteractor interactor = base.interactorsSelecting[0];
                        if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default)
                        {
                            UpdateInteractorLocalPose(interactor);
                        }

                        UpdateTarget(interactor, Time.deltaTime);
                        if (m_CurrentMovementType == MovementType.Instantaneous)
                        {
                            PerformInstantaneousUpdate(updatePhase);
                        }
                    }

                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    if (m_DetachInLateUpdate)
                    {
                        if (!base.isSelected)
                        {
                            Detach();
                        }

                        m_DetachInLateUpdate = false;
                    }

                    break;
            }
        }

        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            if (!(m_AttachTransform != null))
            {
                return base.GetAttachTransform(interactor);
            }

            return m_AttachTransform;
        }

        private Vector3 GetWorldAttachPosition(IXRInteractor interactor)
        {
            Transform attachTransform = interactor.GetAttachTransform(this);
            if (!m_TrackRotation)
            {
                return attachTransform.position + base.transform.TransformDirection(m_InteractorLocalPosition);
            }

            return attachTransform.position + attachTransform.rotation * m_InteractorLocalPosition;
        }

        private Quaternion GetWorldAttachRotation(IXRInteractor interactor)
        {
            if (!m_TrackRotation)
            {
                return m_TargetWorldRotation;
            }

            return interactor.GetAttachTransform(this).rotation * m_InteractorLocalRotation;
        }

        private void UpdateTarget(IXRInteractor interactor, float timeDelta)
        {
            Vector3 worldAttachPosition = GetWorldAttachPosition(interactor);
            Quaternion worldAttachRotation = GetWorldAttachRotation(interactor);
            if (m_AttachEaseInTime > 0f && m_CurrentAttachEaseTime <= m_AttachEaseInTime)
            {
                float t = m_CurrentAttachEaseTime / m_AttachEaseInTime;
                m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, worldAttachPosition, t);
                m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, worldAttachRotation, t);
                m_CurrentAttachEaseTime += timeDelta;
                return;
            }

            if (m_SmoothPosition)
            {
                m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, worldAttachPosition, m_SmoothPositionAmount * timeDelta);
                m_TargetWorldPosition = Vector3.Lerp(m_TargetWorldPosition, worldAttachPosition, m_TightenPosition);
            }
            else
            {
                m_TargetWorldPosition = worldAttachPosition;
            }

            if (m_SmoothRotation)
            {
                m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, worldAttachRotation, m_SmoothRotationAmount * timeDelta);
                m_TargetWorldRotation = Quaternion.Slerp(m_TargetWorldRotation, worldAttachRotation, m_TightenRotation);
            }
            else
            {
                m_TargetWorldRotation = worldAttachRotation;
            }
        }

        private void PerformInstantaneousUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic || updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                if (m_TrackPosition)
                {
                    base.transform.position = m_TargetWorldPosition;
                }

                if (m_TrackRotation)
                {
                    base.transform.rotation = m_TargetWorldRotation;
                }
            }
        }

        private void PerformKinematicUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                if (m_TrackPosition)
                {
                    Vector3 position = (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default) ? m_TargetWorldPosition : (m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass + m_Rigidbody.position);
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.MovePosition(position);
                }

                if (m_TrackRotation)
                {
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    m_Rigidbody.MoveRotation(m_TargetWorldRotation);
                }
            }
        }

        private void PerformVelocityTrackingUpdate(float timeDelta, XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (updatePhase != 0)
            {
                return;
            }

            if (m_TrackPosition)
            {
                m_Rigidbody.velocity *= 1f - m_VelocityDamping;
                Vector3 a = ((m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default) ? (m_TargetWorldPosition - base.transform.position) : (m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass)) / timeDelta;
                if (!float.IsNaN(a.x))
                {
                    m_Rigidbody.velocity += a * m_VelocityScale;
                }
            }

            if (!m_TrackRotation)
            {
                return;
            }

            m_Rigidbody.angularVelocity *= 1f - m_AngularVelocityDamping;
            (m_TargetWorldRotation * Quaternion.Inverse(base.transform.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f)
            {
                angle -= 360f;
            }

            if (Mathf.Abs(angle) > Mathf.Epsilon)
            {
                Vector3 a2 = axis * (angle * ((float)Math.PI / 180f)) / timeDelta;
                if (!float.IsNaN(a2.x))
                {
                    m_Rigidbody.angularVelocity += a2 * m_AngularVelocityScale;
                }
            }
        }

        private void UpdateInteractorLocalPose(IXRInteractor interactor)
        {
            if (m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Legacy)
            {
                UpdateInteractorLocalPoseLegacy(interactor);
                return;
            }

            Transform attachTransform = GetAttachTransform(interactor);
            Vector3 direction = base.transform.position - attachTransform.position;
            Vector3 vector = m_InteractorLocalPosition = attachTransform.InverseTransformDirection(direction);
            m_InteractorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(base.transform.rotation) * attachTransform.rotation);
        }

        private void UpdateInteractorLocalPoseLegacy(IXRInteractor interactor)
        {
            Transform attachTransform = GetAttachTransform(interactor);
            Vector3 direction = m_Rigidbody.worldCenterOfMass - attachTransform.position;
            Vector3 interactorLocalPosition = attachTransform.InverseTransformDirection(direction);
            Vector3 lossyScale = interactor.GetAttachTransform(this).lossyScale;
            lossyScale = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
            interactorLocalPosition.Scale(lossyScale);
            m_InteractorLocalPosition = interactorLocalPosition;
            m_InteractorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(base.transform.rotation) * attachTransform.rotation);
        }

        private void UpdateCurrentMovementType()
        {
            XRBaseInteractor xRBaseInteractor = base.interactorsSelecting[0] as XRBaseInteractor;
            m_CurrentMovementType = (((xRBaseInteractor != null) ? xRBaseInteractor.selectedInteractableMovementTypeOverride : null) ?? m_MovementType);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            Grab();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            Drop();
        }

        protected virtual void Grab()
        {
            Transform transform = base.transform;
            m_OriginalSceneParent = transform.parent;
            //transform.SetParent(null);
            UpdateCurrentMovementType();
            SetupRigidbodyGrab(m_Rigidbody);
            m_DetachVelocity = Vector3.zero;
            m_DetachAngularVelocity = Vector3.zero;
            m_TargetWorldPosition = ((m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default) ? transform.position : m_Rigidbody.worldCenterOfMass);
            m_TargetWorldRotation = transform.rotation;
            m_CurrentAttachEaseTime = 0f;
            IXRSelectInteractor interactor = base.interactorsSelecting[0];
            UpdateInteractorLocalPose(interactor);
            SmoothVelocityStart(interactor);
        }

        protected virtual void Drop()
        {
            if (m_RetainTransformParent && m_OriginalSceneParent != null && !m_OriginalSceneParent.gameObject.activeInHierarchy)
            {
                if (!EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Debug.LogWarning("Retain Transform Parent is set to true, and has a non-null Original Scene Parent. However, the old parent is deactivated so we are choosing not to re-parent upon dropping.", this);
                }
            }
            else if (m_RetainTransformParent && base.gameObject.activeInHierarchy)
            {
                base.transform.SetParent(m_OriginalSceneParent);
            }

            SetupRigidbodyDrop(m_Rigidbody);
            m_CurrentMovementType = m_MovementType;
            m_DetachInLateUpdate = true;
            SmoothVelocityEnd();
        }

        protected virtual void Detach()
        {
            if (m_ThrowOnDetach)
            {
                m_Rigidbody.velocity = m_DetachVelocity;
                m_Rigidbody.angularVelocity = m_DetachAngularVelocity;
            }
        }

        protected virtual void SetupRigidbodyGrab(Rigidbody rigidbody)
        {
            m_WasKinematic = rigidbody.isKinematic;
            m_UsedGravity = rigidbody.useGravity;
            m_OldDrag = rigidbody.drag;
            m_OldAngularDrag = rigidbody.angularDrag;
            rigidbody.isKinematic = true;// (m_CurrentMovementType == MovementType.Kinematic || m_CurrentMovementType == MovementType.Instantaneous);
            rigidbody.useGravity = false;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }

        protected virtual void SetupRigidbodyDrop(Rigidbody rigidbody)
        {
            rigidbody.isKinematic = m_WasKinematic;
            rigidbody.useGravity = m_UsedGravity;
            rigidbody.drag = m_OldDrag;
            rigidbody.angularDrag = m_OldAngularDrag;
            if (!base.isSelected)
            {
                m_Rigidbody.useGravity |= m_ForceGravityOnDetach;
            }
        }

        private void SmoothVelocityStart(IXRInteractor interactor)
        {
            SetTeleportationProvider(interactor);
            Transform attachTransform = interactor.GetAttachTransform(this);
            m_LastPosition = attachTransform.position;
            m_LastRotation = attachTransform.rotation;
            Array.Clear(m_ThrowSmoothingFrameTimes, 0, m_ThrowSmoothingFrameTimes.Length);
            Array.Clear(m_ThrowSmoothingVelocityFrames, 0, m_ThrowSmoothingVelocityFrames.Length);
            Array.Clear(m_ThrowSmoothingAngularVelocityFrames, 0, m_ThrowSmoothingAngularVelocityFrames.Length);
            m_ThrowSmoothingCurrentFrame = 0;
        }

        private void SmoothVelocityEnd()
        {
            if (m_ThrowOnDetach)
            {
                Vector3 smoothedVelocityValue = GetSmoothedVelocityValue(m_ThrowSmoothingVelocityFrames);
                Vector3 smoothedVelocityValue2 = GetSmoothedVelocityValue(m_ThrowSmoothingAngularVelocityFrames);
                m_DetachVelocity = smoothedVelocityValue * m_ThrowVelocityScale;
                m_DetachAngularVelocity = smoothedVelocityValue2 * m_ThrowAngularVelocityScale;
            }

            ClearTeleportationProvider();
        }

        private void SmoothVelocityUpdate(IXRInteractor interactor)
        {
            Transform attachTransform = interactor.GetAttachTransform(this);
            Vector3 position = attachTransform.position;
            Quaternion rotation = attachTransform.rotation;
            m_ThrowSmoothingFrameTimes[m_ThrowSmoothingCurrentFrame] = Time.time;
            m_ThrowSmoothingVelocityFrames[m_ThrowSmoothingCurrentFrame] = (position - m_LastPosition) / Time.deltaTime;
            Quaternion quaternion = rotation * Quaternion.Inverse(m_LastRotation);
            m_ThrowSmoothingAngularVelocityFrames[m_ThrowSmoothingCurrentFrame] = new Vector3(Mathf.DeltaAngle(0f, quaternion.eulerAngles.x), Mathf.DeltaAngle(0f, quaternion.eulerAngles.y), Mathf.DeltaAngle(0f, quaternion.eulerAngles.z)) / Time.deltaTime * ((float)Math.PI / 180f);
            m_ThrowSmoothingCurrentFrame = (m_ThrowSmoothingCurrentFrame + 1) % 20;
            m_LastPosition = position;
            m_LastRotation = rotation;
        }

        private Vector3 GetSmoothedVelocityValue(Vector3[] velocityFrames)
        {
            Vector3 a = default(Vector3);
            float num = 0f;
            for (int i = 0; i < 20; i++)
            {
                int num2 = ((m_ThrowSmoothingCurrentFrame - i - 1) % 20 + 20) % 20;
                if (m_ThrowSmoothingFrameTimes[num2] == 0f)
                {
                    break;
                }

                float num3 = (Time.time - m_ThrowSmoothingFrameTimes[num2]) / m_ThrowSmoothingDuration;
                float num4 = m_ThrowSmoothingCurve.Evaluate(Mathf.Clamp(1f - num3, 0f, 1f));
                a += velocityFrames[num2] * num4;
                num += num4;
                if (Time.time - m_ThrowSmoothingFrameTimes[num2] > m_ThrowSmoothingDuration)
                {
                    break;
                }
            }

            if (num > 0f)
            {
                return a / num;
            }

            return Vector3.zero;
        }

        private void OnBeginTeleportation(LocomotionSystem locomotionSystem)
        {
            Transform transform = locomotionSystem.xrOrigin.Origin.transform;
            m_PoseBeforeTeleport = new Pose(transform.position, transform.rotation);
        }

        private void OnEndTeleportation(LocomotionSystem locomotionSystem)
        {
            Transform transform = locomotionSystem.xrOrigin.Origin.transform;
            Vector3 vector = transform.position - m_PoseBeforeTeleport.position;
            Quaternion quaternion = transform.rotation * Quaternion.Inverse(m_PoseBeforeTeleport.rotation);
            for (int i = 0; i < 20 && m_ThrowSmoothingFrameTimes[i] != 0f; i++)
            {
                m_ThrowSmoothingVelocityFrames[i] = quaternion * m_ThrowSmoothingVelocityFrames[i];
            }

            m_LastPosition += vector;
            m_LastRotation = quaternion * m_LastRotation;
        }

        private void SetTeleportationProvider(IXRInteractor interactor)
        {
            ClearTeleportationProvider();
            Transform transform = interactor?.transform;
            if (!(transform == null))
            {
                m_TeleportationProvider = transform.GetComponentInParent<TeleportationProvider>();
                if (!(m_TeleportationProvider == null))
                {
                    m_TeleportationProvider.beginLocomotion += OnBeginTeleportation;
                    m_TeleportationProvider.endLocomotion += OnEndTeleportation;
                }
            }
        }

        private void ClearTeleportationProvider()
        {
            if (!(m_TeleportationProvider == null))
            {
                m_TeleportationProvider.beginLocomotion -= OnBeginTeleportation;
                m_TeleportationProvider.endLocomotion -= OnEndTeleportation;
                m_TeleportationProvider = null;
            }
        }
    }
}
#if false // Decompilation log
'235' items in cache
------------------
Resolve: 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\NetStandard\ref\2.0.0\netstandard.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll'
------------------
Resolve: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEditor.CoreModule.dll'
------------------
Resolve: 'Unity.InputSystem, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.InputSystem, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\NF\CarTest\Library\ScriptAssemblies\Unity.InputSystem.dll'
------------------
Resolve: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.XRModule.dll'
------------------
Resolve: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.AnimationModule.dll'
------------------
Resolve: 'UnityEngine.SpatialTracking, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.SpatialTracking, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\NF\CarTest\Library\ScriptAssemblies\UnityEngine.SpatialTracking.dll'
------------------
Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll'
------------------
Resolve: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.AudioModule.dll'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\NF\CarTest\Library\ScriptAssemblies\UnityEngine.UI.dll'
------------------
Resolve: 'Unity.XR.CoreUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.XR.CoreUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\NF\CarTest\Library\ScriptAssemblies\Unity.XR.CoreUtils.dll'
------------------
Resolve: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.InputLegacyModule.dll'
------------------
Resolve: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.UIModule.dll'
------------------
Resolve: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.Physics2DModule.dll'
------------------
Resolve: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.IMGUIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'D:\Unity\2021.1.21f1\Editor\Data\Managed\UnityEngine\UnityEngine.IMGUIModule.dll'
#endif
