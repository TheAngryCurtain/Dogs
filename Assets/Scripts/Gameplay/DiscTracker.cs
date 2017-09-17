using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscTracker : MonoBehaviour
{
    [SerializeField] private Transform m_Disc;
    [SerializeField] private Transform m_Transform;
    [SerializeField] private GameObject m_QuadObj;

    [SerializeField] private float m_MaxSqrDistanceToScale;
    [SerializeField] private float m_MinScale;
    [SerializeField] private float m_MaxScale;

    private DiscController m_discController;
    private Vector3 m_Position;
    private Vector3 m_Scale = Vector3.one;

    private void Awake()
    {
        m_discController = m_Disc.GetComponent<DiscController>();
        if (m_discController == null)
        {
            Debug.LogError("The Disc lacks a disc controller");
        }

        m_discController.OnGroundHit += OnDiscTouchedGround;
        m_discController.OnCatch += OnDiscCaught;
        m_discController.OnThrow += OnDiscThrown;
    }

    private void OnDestroy()
    {
        m_discController.OnGroundHit -= OnDiscTouchedGround;
        m_discController.OnCatch -= OnDiscCaught;
        m_discController.OnThrow -= OnDiscThrown;
    }

    private void OnDiscTouchedGround()
    {
        ShowTarget(false);
    }

    private void OnDiscCaught()
    {
        ShowTarget(false);
    }

    private void OnDiscThrown()
    {
        ShowTarget(true);
    }

    private void ShowTarget(bool show)
    {
        m_QuadObj.SetActive(show);
    }

    private void Update()
    {
        if (m_Disc != null)
        {
            float currentSqrDistFromTarget = (m_Disc.position - m_Transform.position).sqrMagnitude;
            float percentOfTotal = currentSqrDistFromTarget / m_MaxSqrDistanceToScale;
            percentOfTotal = Mathf.Clamp(percentOfTotal, m_MinScale, m_MaxScale);

            m_Scale.x = percentOfTotal;
            m_Scale.z = percentOfTotal;

            m_Position.x = m_Disc.position.x;
            m_Position.y = m_Transform.position.y;
            m_Position.z = m_Disc.position.z;

            m_Transform.localScale = m_Scale;
            m_Transform.position = m_Position;
        }
    }
}
