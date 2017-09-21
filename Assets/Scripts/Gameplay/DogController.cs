using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] private IDog m_dog;

    private void Update()
    {
        if (m_dog != null)
        {
            m_dog.GetMovement();
            m_dog.LocalUpdate();
            m_dog.Bark();
            //m_dog.ChargeThrow();
        }
    }

    private void FixedUpdate()
    {
        if (m_dog != null)
        {
            m_dog.ApplyMovement();
            m_dog.ApplyJump();
            m_dog.ApplySpecial();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleGroundCollision(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        HandleGroundCollision(collision, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Disc"))
        {
            m_dog.Catch(other.transform);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Dish"))
        {
            WaterDish dish = other.GetComponent<WaterDish>();
            m_dog.SetWaterInRange(true, dish);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Dish"))
        {
            m_dog.SetWaterInRange(false, null);
        }
    }

    private void HandleGroundCollision(Collision col, bool enter)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            m_dog.HandleGroundInteraction(enter);
        }
    }
}
