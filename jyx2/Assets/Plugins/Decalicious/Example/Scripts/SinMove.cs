using UnityEngine;

/// <summary>
/// TODO: Document SinMove!
/// </summary>
public class SinMove : MonoBehaviour
{
    public float Offset = 0.0f;
    public float Speed = 1.0f;
    public float Distance = 0.05f;

    private Vector3 _basePosition;
    private float _totalTime = 0.0f;

    void Start()
    {
        _basePosition = transform.position;
        _totalTime = Offset;
    }

    void Update()
    {
        _totalTime += Time.deltaTime * Speed;
        transform.position = _basePosition + Distance * Vector3.up * Mathf.Sin(_totalTime);
    }
}
