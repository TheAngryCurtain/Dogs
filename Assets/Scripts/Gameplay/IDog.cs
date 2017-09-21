using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class IDog : MonoBehaviour
{
    public enum eActionType { None, Jump, Special }

    //public System.Action<float> OnThrowMeterUpdated;

    [SerializeField] protected Rigidbody m_Rigidbody;
    [SerializeField] protected Camera m_Camera;
    [SerializeField] protected Transform m_Transform;
    [SerializeField] protected Transform m_MouthTransform;
    [SerializeField] protected Collider m_CatchTrigger;
    [SerializeField] protected Animator m_Animator;

    [SerializeField] private Text DebugBarkText;

    [SerializeField] protected float m_Acceleration = 10f;
    [SerializeField] protected float m_MaxSpeed = 6f;
    [SerializeField] protected float m_StaticJumpForce = 1500f;
    [SerializeField] protected float m_VariableJumpForce = 800f;
    [SerializeField] protected float m_VariableJumpDecay = 0.5f;

    [SerializeField] protected float m_ThrowMeterIncrement = 0.25f;
    [SerializeField] protected float m_MaxThrowPower = 30f;

    [SerializeField] protected float m_MaxHydration = 100f;
    [SerializeField] protected float m_SpeedToThirsty = 3f;
    [SerializeField] protected float m_ThirstDecay = 0.1f;
    [SerializeField] protected float m_MaxThirstSpeedModifier = 0.5f;

    [SerializeField] protected float m_LeanAngle = 15f;
    [SerializeField] protected float m_LerpSpeed = 20f;

    protected float h;
    protected float v;
    protected Vector3 m_Movement;
    protected bool m_Jumping = false;
    protected bool m_Specialing = false;
    protected bool m_Grounded = false;
    protected bool m_UsingSpecial = false;
    protected float m_CurrentJumpForce;
    protected bool m_CanJump = true;
    protected Vector3 m_Velocity;
    protected float m_Speed;
    protected float m_NormalizedSpeed;
    protected float m_ThrowPercent = 0f;
    protected float m_PreviousTriggerValue = 0f;
    protected DiscController m_Disc = null;
    protected RigidbodyConstraints m_DefaultConstraints = RigidbodyConstraints.FreezeRotation;
    protected RigidbodyConstraints m_JumpConstraints = RigidbodyConstraints.None;
    protected bool m_CanDrink = false;
    protected WaterDish m_Dish = null;
    protected float m_CurrentHydration;
    protected eActionType m_LastActionType;
    protected float m_CurrentThirstSpeedModifier = 1f;
    protected bool m_Tumbling = false;

    private string[] m_Vocab = new string[]
    {
        "Bark!",
        "Woof!",
        "Ruff!",
        "Bow Wow!"
    };

    public void SetFollowCamera(Camera cam)
    {
        m_Camera = cam;
    }

    protected virtual void Start()
    {
        //OnThrowMeterUpdated += UIManager.Instance.UpdateThrowMeter;
        //OnHydrationMeterUpdated += UIManager.Instance.UpdateHydration;

        m_CurrentJumpForce = m_VariableJumpForce;
        ModifyHydration(m_MaxHydration);
    }

    protected virtual void OnDestroy()
    {
        //OnThrowMeterUpdated -= UIManager.Instance.UpdateThrowMeter;
        //OnHydrationMeterUpdated -= UIManager.Instance.UpdateHydration;
    }

    public virtual void GetMovement()
    {
        m_Movement = Vector3.zero;
        h = Input.GetAxis("Move_Horizontal");
        v = Input.GetAxis("Move_Vertical");

        if (m_Grounded)
        {
            m_Movement.x = h;
            m_Movement.y = 0f;
            m_Movement.z = v;
            m_Movement = m_Camera.transform.TransformDirection(m_Movement);
        }
    }

    public virtual void ApplyMovement()
    {
        if (m_Tumbling) return;

        m_Velocity = m_Rigidbody.velocity;
        if (m_Grounded)
        {
            m_Velocity.y = 0f;
            m_Speed = m_Velocity.magnitude;
            m_NormalizedSpeed = m_Velocity.normalized.magnitude;

            float modifiedMaxSpeed = m_MaxSpeed * m_CurrentThirstSpeedModifier;
            if (m_Rigidbody.velocity.sqrMagnitude < (modifiedMaxSpeed * modifiedMaxSpeed))
            {
                m_Rigidbody.AddForce(m_Movement * m_Acceleration, ForceMode.Acceleration);
            }
        }
        else
        {
            m_Velocity =  m_Transform.forward * 10f;
            m_Velocity.y = Mathf.Lerp(m_Velocity.y, m_Rigidbody.velocity.y, Time.fixedDeltaTime * m_LerpSpeed);
            m_Velocity.Normalize();
        }

        // get thirsty!
        if (m_Speed > m_SpeedToThirsty)
        {
            ModifyHydration(-m_ThirstDecay);
        }

        // face travel direction
        // lean into turns
        RotateToVelocity();
    }

    private void RotateToVelocity()
    {
        if (m_Velocity != Vector3.zero && !m_UsingSpecial)
        {
            Quaternion rotation = Quaternion.LookRotation(m_Velocity);
            Vector3 tiltAxis = Vector3.Cross(Vector3.up, m_Movement);
            rotation = Quaternion.AngleAxis(m_LeanAngle, tiltAxis) * rotation;
            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, rotation, Time.fixedDeltaTime * m_LerpSpeed));
        }
    }


    //public virtual void ChargeThrow()
    //{
    //    // can't throw without disc
    //    if (m_Disc == null || m_Tumbling) return;

    //    float trigger = Mathf.Abs(Input.GetAxisRaw("Charge Throw"));
    //    if (trigger > 0.1f)
    //    {
    //        m_ThrowPercent = Mathf.Clamp(m_ThrowPercent += m_ThrowMeterIncrement, 0f, 100f);
    //        if (OnThrowMeterUpdated != null)
    //        {
    //            OnThrowMeterUpdated(m_ThrowPercent / 100f);
    //        }
    //    }

    //    if (m_PreviousTriggerValue - trigger > 0.5f)
    //    {
    //        // trigger dumped, throw
    //        m_ThrowPercent /= 100f;
    //        float curveAmount = 0f;
    //        Throw(m_Transform.forward, m_ThrowPercent * m_MaxThrowPower, curveAmount);

    //        m_ThrowPercent = 0f;
    //        if (OnThrowMeterUpdated != null)
    //        {
    //            OnThrowMeterUpdated(m_ThrowPercent);
    //        }
    //    }

    //    m_PreviousTriggerValue = trigger;
    //}

    public virtual void ApplyJump()
    {
        if (m_Tumbling) return;

        if (m_Jumping && m_CanJump)
        {
            if (m_CurrentJumpForce > 0f)
            {
                m_LastActionType = eActionType.Jump;
                m_Rigidbody.AddForce(Vector3.up * m_CurrentJumpForce, ForceMode.Acceleration);
                m_CurrentJumpForce -= m_VariableJumpDecay;
            }
            else if (!m_Grounded)
            {
                m_CurrentJumpForce = 0f;
                m_CanJump = false;
            }
        }
    }

    public void SetWaterInRange(bool inRange, WaterDish dish)
    {
        m_CanDrink = inRange;
        m_Dish = dish; // will be null if not in range
    }

    private void AnimatorUpdate()
    {
        m_Animator.speed = m_NormalizedSpeed;

        m_Animator.SetFloat("Speed", (m_Disc == null ? m_Speed : 0f));
        m_Animator.SetBool("Grounded", m_Grounded);
        m_Animator.SetBool("Moving", m_Speed > 0.1f && m_Grounded);
    }

    public virtual void LocalUpdate()
    {
        if (m_Tumbling) return;

        m_Specialing = Input.GetButtonDown("Special");
        m_Jumping = Input.GetButton("Jump");

        AnimatorUpdate();
        
        if (Input.GetButtonUp("Jump"))
        {
            m_CurrentJumpForce = m_VariableJumpForce;

            if (!m_Grounded)
            {
                m_CanJump = false;
            }
        }

        //if (m_CanDrink && m_CurrentHydration < m_MaxHydration && m_disc == null && Input.GetButtonDown("Drink"))
        if (m_CanDrink && m_Disc == null && Input.GetButtonDown("Drink"))
        {
            m_Transform.LookAt(m_Dish.transform);
            ModifyHydration(m_Dish.Drink());
        }

//#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Transform.position = new Vector3(44.5f, 0.5f, 40f);
        }
//#endif
    }

    protected void ApplyStaticJump()
    {
        m_Rigidbody.AddForce(Vector3.up * m_StaticJumpForce);
    }

    protected void ModifyHydration(float amount)
    {
        m_CurrentHydration = Mathf.Clamp(m_CurrentHydration + amount, 0f, m_MaxHydration);
        if (m_CurrentHydration <= 0f)
        {
            m_CurrentThirstSpeedModifier = m_MaxThirstSpeedModifier;
        }
        else
        {
            m_CurrentThirstSpeedModifier = 1f;
        }

        VSEventManager.Instance.TriggerEvent(new UIEvents.UpdateHydrationEvent(m_CurrentHydration / m_MaxHydration));
    }

    public virtual void Catch(Transform frisbeeObj)
    {
        DiscController disc = frisbeeObj.GetComponent<DiscController>();
        if (disc != null)
        {
            m_Disc = disc;

            CameraController camControl = m_Camera.GetComponent<CameraController>();
            if (camControl != null)
            {
                camControl.ChangeMode(CameraController.eCameraMode.Free);
            }

            disc.Catch(this);
            frisbeeObj.SetParent(m_MouthTransform);
            frisbeeObj.localPosition = Vector3.zero;
            frisbeeObj.rotation = m_MouthTransform.rotation;

            VSEventManager.Instance.TriggerEvent(new GameplayEvents.DogCatchDiscEvent(m_LastActionType));
        }
    }

    //public virtual void Throw(Vector3 direction, float power, float curveAmount)
    //{
    //    m_Disc.Throw(direction, power, curveAmount);
    //    m_Disc = null;
    //}

    public virtual void HandleGroundInteraction(bool onGround)
    {
        m_Grounded = onGround;
        if (m_Grounded)
        {
            m_LastActionType = eActionType.None;
            RotateToVelocity();
            m_CanJump = true;

            m_Rigidbody.constraints = m_DefaultConstraints;

            // if you've caught the disc and just landed, let it be known
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.DogTouchGroundEvent(m_Disc != null, m_Transform.position));

            // tumble check
            float directionDot = Vector3.Dot(m_Velocity, m_Transform.forward);
            //Debug.Log(directionDot);
            //Debug.Log(m_Speed);
            if (m_Speed > 3f && directionDot < 0.925f)
            {
                Tumble();
            }
        }
    }

    protected virtual void Tumble()
    {
        m_Tumbling = true;
        m_Rigidbody.constraints = m_JumpConstraints;
        m_Rigidbody.angularDrag = 5f;

        StartCoroutine(StopTumbling());
    }

    private IEnumerator StopTumbling()
    {
        yield return new WaitForSeconds(2f);
        m_Rigidbody.constraints = m_DefaultConstraints;
        m_Rigidbody.angularDrag = 0.05f;
        m_Tumbling = false;
    }

    public virtual void Bark()
    {
        if (!m_CanDrink && Input.GetButtonDown("Bark"))
        {
            int randIndex = UnityEngine.Random.Range(0, m_Vocab.Length);
            DebugBarkText.text = m_Vocab[randIndex];
            StartCoroutine(ClearBark());
            //Debug.Log(m_Vocab[randIndex]);
        }
    }

    private IEnumerator ClearBark()
    {
        yield return new WaitForSecondsRealtime(1f);
        DebugBarkText.text = "";
    }

    public abstract void ApplySpecial();

    // TODO
    // move to Utils class
    protected float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y < 0) angle = -angle;

        return angle;
    }
}
