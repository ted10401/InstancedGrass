using UnityEngine;
using UnityEngine.Rendering;

public class InstancedGrass : MonoBehaviour
{
    public struct GrassData
    {
        public Vector4 data;

        public GrassData(Vector4 data)
        {
            this.data = data;
        }
    }

    public Transform groundTransform;
    public int grassCount;
    public float grassRange;
    public GrassData[] grassDatas;
    public Mesh grassMesh;
    public Material grassMaterial;
    public int subMeshIndex = 0;
    private Bounds m_drawBounds;
    private ComputeBuffer m_positionBuffer;
    private ComputeBuffer m_argsBuffer;
    private uint[] m_args = new uint[5] { 0, 0, 0, 0, 0 };
    private MaterialPropertyBlock m_materialPropertyBlock;

    private void Awake()
    {
        m_materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        DrawInstancedGrass();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateGroundScale();
            UpdateGrassDatas();
        }
    }

    private void DrawInstancedGrass()
    {
        if(grassMesh == null || grassMaterial == null || m_drawBounds == null || m_argsBuffer == null)
        {
            return;
        }

        Graphics.DrawMeshInstancedIndirect(grassMesh, subMeshIndex, grassMaterial, m_drawBounds, m_argsBuffer, 0, m_materialPropertyBlock, ShadowCastingMode.On, true);
    }

    private void UpdateGroundScale()
    {
        groundTransform.localScale = Vector3.one * grassRange / 5;
    }

    private void UpdateGrassDatas()
    {
        m_drawBounds = new Bounds(Vector3.zero, Vector3.one * grassRange * 2);
        grassDatas = new GrassData[grassCount];
        for(int i = 0; i < grassCount; i++)
        {
            grassDatas[i] = new GrassData(new Vector4(Random.Range(-grassRange, grassRange), 0, Random.Range(-grassRange, grassRange), Random.Range(0.5f, 1.0f)));
        }

        UpdateComputeBuffer();
    }

    private void UpdateComputeBuffer()
    {
        ReleaseBuffer();

        if (!Application.isPlaying)
        {
            return;
        }

        if(grassDatas == null || grassCount == 0)
        {
            return;
        }

        m_positionBuffer = new ComputeBuffer(grassCount, sizeof(float) * 4);
        m_positionBuffer.SetData(grassDatas);
        grassMaterial.SetBuffer("positionBuffer", m_positionBuffer);

        m_argsBuffer = new ComputeBuffer(1, m_args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        if (grassMesh != null)
        {
            m_args[0] = grassMesh.GetIndexCount(subMeshIndex);
            m_args[1] = (uint)grassCount;
            m_args[2] = grassMesh.GetIndexStart(subMeshIndex);
            m_args[3] = grassMesh.GetBaseVertex(subMeshIndex);
        }
        else
        {
            m_args[0] = m_args[1] = m_args[2] = m_args[3] = 0;
        }
        m_argsBuffer.SetData(m_args);
    }

    private void ReleaseBuffer()
    {
        if (m_positionBuffer != null)
        {
            m_positionBuffer.Release();
            m_positionBuffer = null;
        }

        if (m_argsBuffer != null)
        {
            m_argsBuffer.Release();
            m_argsBuffer = null;
        }
    }

    private void OnDisable()
    {
        ReleaseBuffer();
    }
}
