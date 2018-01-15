using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscController : MonoBehaviour
{
    [SerializeField] private Transform m_Transform;
    [SerializeField] private Rigidbody m_Rigidbody;
    [SerializeField] private TrailRenderer m_Trail;
    [SerializeField] private Collider m_Collider;
    [SerializeField] private ParticleSystem m_Starburst;
    [SerializeField] private AudioSource m_AudioSource;

    // TODO move this into some sort of Ultimate Rules manager or something later
    [SerializeField] private GameObject m_SafeAreaPrefab;

    [SerializeField] private float m_HoverForce = 2f;

    [SerializeField] private Transform m_ResetPoint;
    [SerializeField] private Transform m_Thrower;

    private GameObject m_SafeArea;
    private bool m_Held = true;
    private float m_ThrowForce;
    private float m_CurveAmount = 0f;

    private void Awake()
    {
        //VSEventManager.Instance.AddListener<GameplayEvents.DogTouchGroundEvent>(DogTouchGroundAfterCatch);
    }

    private void OnDestroy()
    {
        //VSEventManager.Instance.RemoveListener<GameplayEvents.DogTouchGroundEvent>(DogTouchGroundAfterCatch);
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.R))
        {
            // debug reset
            m_Transform.SetParent(null);
            m_Collider.enabled = true;

            m_Transform.position = m_ResetPoint.position;
            m_Transform.rotation = Quaternion.identity;

            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
        }
        
        // TODO create proper launcher?
        if (Input.GetKeyDown(KeyCode.T))
        {
            // debug throw

            //float randThrow = UnityEngine.Random.Range(100f, 115f);
            //float randCurve = UnityEngine.Random.Range(-0.3f, 0.3f);

            //Debug.LogFormat("Throw Force: {0}, Curve Amount: {1}", randThrow, randCurve);

            Throw(m_Thrower.forward, 65f, 0f);
        }
#endif
        // weird edge case where the disc riccochets outside of the terrain
        if (m_Transform.position.y < -1f && !m_Held)
        {
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.DiscTouchGroundEvent());
            m_Held = true;
        }
    }

    public void Reset()
    {
        m_Transform.SetParent(null);
        m_Collider.enabled = true;

        m_Transform.rotation = Quaternion.identity;

        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    public void Throw(Vector3 direction, float force, float curveAmount)
    {
        m_Trail.enabled = true;
        m_Transform.SetParent(null);
        m_Rigidbody.isKinematic = false;

        m_CurveAmount = curveAmount;
        m_ThrowForce = force;
        m_Rigidbody.AddForce(direction * m_ThrowForce);
        m_Collider.enabled = true;

        m_Held = false;
    }

    public void Catch(IDog catchingDog)
    {
        m_Starburst.Play();
        m_Trail.enabled = false;
        m_Held = true;

        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Rigidbody.isKinematic = true;
        m_Collider.enabled = false;
    }

    //private void DogTouchGroundAfterCatch(GameplayEvents.DogTouchGroundEvent e)
    //{
    //    if (m_SafeArea == null)
    //    {
    //        m_SafeArea = (GameObject)Instantiate(m_SafeAreaPrefab);
    //    }

    //    // preserve the saved y value
    //    e.LandPosition.y = m_SafeAreaPrefab.transform.position.y;
    //    m_SafeArea.transform.position = e.LandPosition;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            VSEventManager.Instance.TriggerEvent(new GameplayEvents.DiscTouchGroundEvent());
            m_Held = true;
        }

        m_AudioSource.Play();
    }

    private void FixedUpdate()
    {
        if (!m_Held)
        {
            m_Rigidbody.AddForce(Vector3.up * m_HoverForce, ForceMode.Acceleration);

            // Curve force added each frame
            Vector3 sideDir = Vector3.Cross(m_Transform.up, m_Rigidbody.velocity).normalized;
            m_Rigidbody.AddForce(sideDir * m_CurveAmount);

            Quaternion q = Quaternion.AngleAxis(-m_CurveAmount * (m_ThrowForce / 4f), m_Rigidbody.velocity);
            m_Rigidbody.MoveRotation(q);
        }
    }
}
