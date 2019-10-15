using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Material grassMaterial;

    private Transform m_transform;
    private Vector3 m_inputAxis;

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
        grassMaterial.SetVector("_CharacterPosition", m_transform.position);
    }
}
