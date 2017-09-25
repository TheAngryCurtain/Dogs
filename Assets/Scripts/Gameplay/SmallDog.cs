using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDog : IDog
{
    [SerializeField] private float m_FlipTorque = 20f;

    private float m_MinAngle = 5f;

    // NOTE TO SELF
    // the issue with random, super high jumps is because input is being checked for in fixed update, which, during low frame rates, can fire twice!
    // since rigidbodies are being used, the application of forces must be done in fixed update, but checking for input should still be done in update
    // link: http://answers.unity3d.com/questions/1108929/getkeydown-occasionally-fires-twice.html
    public override void ApplySpecial()
    {
        if (!m_Tumbling && m_Grounded && m_Specialing)
        {
            m_Specialing = false;
            m_UsingSpecial = true;
            m_LastActionType = eActionType.Special;
            ApplyStaticJump();

            // back flip
            m_Rigidbody.constraints = m_JumpConstraints;
            m_Rigidbody.AddTorque(m_Transform.right * -m_FlipTorque);

            // add a side spin to the back flip
            if (m_Movement != Vector3.zero)
            {
                float angle = Utils.SignedAngle(m_Rigidbody.velocity, m_Movement);
                if (Mathf.Abs(angle) > m_MinAngle)
                {
                    m_Rigidbody.AddTorque(m_Transform.up * (angle / 2f));
                }
            }
        }
    }

    public override void ApplyJump()
    {
        if (!m_UsingSpecial)
        {
            base.ApplyJump();
        }
    }

    public override void HandleGroundInteraction(bool onGround)
    {
        base.HandleGroundInteraction(onGround);
        if (m_Grounded)
        {
            m_UsingSpecial = false;
        }
    }
}
