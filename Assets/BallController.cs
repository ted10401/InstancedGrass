using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float moveLimit;
    public Material grassMaterial;

    private Transform m_transform;
    private Vector3 m_inputAxis;
    private Vector3 m_position;

    private void Awake()
    {
        m_transform = transform;
    }

    private void Update()
    {
        m_inputAxis.x = Input.GetAxis("Horizontal");
        m_inputAxis.z = Input.GetAxis("Vertical");
        m_inputAxis.Normalize();
        m_transform.Translate(m_inputAxis * moveSpeed * Time.deltaTime);

        m_position = m_transform.position;
        m_position.x = Mathf.Clamp(m_position.x, -moveLimit, moveLimit);
        m_position.z = Mathf.Clamp(m_position.z, -moveLimit, moveLimit);
        m_transform.position = m_position;

        grassMaterial.SetVector("_CharacterPosition", m_position);
    }
}
