// Inspired by https://github.com/keijiro/Boids

using UnityEngine;
using GPUInstancer;

public class BoidController : MonoBehaviour
{
    public int spawnCount = 10;

    public float spawnRadius = 4.0f;

    [Range(0.1f, 20.0f)]
    public float velocity = 6.0f;

    [Range(0.0f, 0.9f)]
    public float velocityVariation = 0.5f;

    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Range(0.1f, 10.0f)]
    public float neighborDist = 2.0f;

    public Transform centerTransform;
    public Texture2D noiseTexture;
    public string variationBufferName = "colorBuffer";

    private Matrix4x4[] _spawnArray;
    private Vector4[] _variationArray;
    private GPUInstancerPrefabManager _prefabManager;
    private ComputeShader _gpuiBoidsCS;
    private float[] _centerTransformArray = new float[16];
    private ComputeBuffer _transformDataBuffer;

    // CS params
    public static class BoidProperties
    {
        public static readonly int BP_boidsData = Shader.PropertyToID("boidsData");
        public static readonly int BP_bufferSize = Shader.PropertyToID("bufferSize");
        public static readonly int BP_controllerTransform = Shader.PropertyToID("controllerTransform");
        public static readonly int BP_controllerVelocity = Shader.PropertyToID("controllerVelocity");
        public static readonly int BP_controllerVelocityVariation = Shader.PropertyToID("controllerVelocityVariation");
        public static readonly int BP_controllerRotationCoeff = Shader.PropertyToID("controllerRotationCoeff");
        public static readonly int BP_controllerNeighborDist = Shader.PropertyToID("controllerNeighborDist");
        public static readonly int BP_time = Shader.PropertyToID("time");
        public static readonly int BP_deltaTime = Shader.PropertyToID("deltaTime");
        public static readonly int BP_noiseTexture = Shader.PropertyToID("noiseTexture");
    }

    void Start()
    {
        _spawnArray = new Matrix4x4[spawnCount];
        _variationArray = new Vector4[spawnCount];
        for (var i = 0; i < spawnCount; i++)
            Spawn(i);

        _prefabManager = FindObjectOfType<GPUInstancerPrefabManager>();

        if (_prefabManager != null && _prefabManager.prototypeList.Count > 0)
        {
            GPUInstancerPrefabPrototype prototype = (GPUInstancerPrefabPrototype)_prefabManager.prototypeList[0];
            GPUInstancerAPI.InitializeWithMatrix4x4Array(_prefabManager, prototype, _spawnArray);
            GPUInstancerAPI.DefineAndAddVariationFromArray(_prefabManager, prototype, variationBufferName, _variationArray);
            _transformDataBuffer = GPUInstancerAPI.GetTransformDataBuffer(_prefabManager, prototype);
        }

    }

    public void Spawn(int index)
    {
        _spawnArray[index] = Matrix4x4.TRS(centerTransform.position + Random.insideUnitSphere * spawnRadius, Random.rotation, Vector3.one);
        _variationArray[index] = Random.ColorHSV();
    }

    void Update()
    {
        if (_gpuiBoidsCS == null)
            _gpuiBoidsCS = Resources.Load<ComputeShader>("GPUIBoids");

        if (_gpuiBoidsCS == null || _transformDataBuffer == null)
            return;

        centerTransform.localToWorldMatrix.Matrix4x4ToFloatArray(_centerTransformArray);

        _gpuiBoidsCS.SetBuffer(0, BoidProperties.BP_boidsData, _transformDataBuffer);
        _gpuiBoidsCS.SetTexture(0, BoidProperties.BP_noiseTexture, noiseTexture);
        _gpuiBoidsCS.SetInt(BoidProperties.BP_bufferSize, spawnCount);
        _gpuiBoidsCS.SetFloats(BoidProperties.BP_controllerTransform, _centerTransformArray);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_controllerVelocity, velocity);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_controllerVelocityVariation, velocityVariation);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_controllerRotationCoeff, rotationCoeff);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_controllerNeighborDist, neighborDist);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_time, Time.time);
        _gpuiBoidsCS.SetFloat(BoidProperties.BP_deltaTime, Time.deltaTime);

        _gpuiBoidsCS.Dispatch(0, Mathf.CeilToInt(spawnCount / GPUInstancerConstants.COMPUTE_SHADER_THREAD_COUNT), 1, 1);
    }
}
