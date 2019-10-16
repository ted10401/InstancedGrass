using UnityEngine;

public class BallController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public InstancedGrass instancedGrass;

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
        m_position.x = Mathf.Clamp(m_position.x, -instancedGrass.grassRange, instancedGrass.grassRange);
        m_position.z = Mathf.Clamp(m_position.z, -instancedGrass.grassRange, instancedGrass.grassRange);
        m_transform.position = m_position;

        instancedGrass.grassMaterial.SetVector("_CharacterPosition", m_position);
    }
}
