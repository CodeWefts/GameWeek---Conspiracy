using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private Vector3 m_Direction;

    private float m_HorizontalInput;
    private float m_VerticalInput;

    private string m_HorizontalAxis = "Horizontal";
    private string m_VerticalAxis = "Vertical";

    // Update is called once per frame
    void Update()
    {
        m_HorizontalInput = Input.GetAxis(m_HorizontalAxis);
        m_VerticalInput = Input.GetAxis(m_VerticalAxis);

        m_Direction = new Vector3(m_HorizontalInput, 0, m_VerticalInput);
        m_Direction = m_Direction.normalized;

        transform.position += m_Direction * m_Speed * Time.deltaTime;
    }
}
