using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDish : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_Splash;
    [SerializeField] private GameObject m_Water;
    [SerializeField] private Transform m_WaterFlag;

    [SerializeField] private float m_RefillDelay;
    [SerializeField] private float m_MaxDrinkAmount;
    [SerializeField] private float m_AmountPerDrink;

    private float m_CurrentDrinkAmount;
    private Quaternion m_FlagRotation;

    private void Start()
    {
        SetFull();
    }

    public float Drink()
    {
        if (m_CurrentDrinkAmount <= 0f) return 0f;

        float remainingDrink = m_CurrentDrinkAmount - m_AmountPerDrink;
        if (remainingDrink <= 0f)
        {
            SetEmpty();
            return Mathf.Abs(remainingDrink);
        }
        else
        {
            m_Splash.Play();
            m_CurrentDrinkAmount = remainingDrink;

            return m_AmountPerDrink;
        }
    }

    private void SetFull()
    {
        m_CurrentDrinkAmount = m_MaxDrinkAmount;

        m_FlagRotation = Quaternion.identity;

        m_Water.SetActive(true);
    }

    private void SetEmpty()
    {
        m_CurrentDrinkAmount = 0f;

        Vector3 newEulers = m_WaterFlag.localEulerAngles;
        newEulers.x -= 90f;
        m_FlagRotation = Quaternion.Euler(newEulers);

        m_Water.SetActive(false);

        StartCoroutine(Refill());
    }

    private void Update()
    {
        if (m_WaterFlag.rotation != m_FlagRotation)
        {
            m_WaterFlag.rotation = Quaternion.Slerp(m_WaterFlag.rotation, m_FlagRotation, Time.deltaTime * 10f);
        }
    }

    private IEnumerator Refill()
    {
        yield return new WaitForSeconds(m_RefillDelay);
        SetFull();
    }
}
