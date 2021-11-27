using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_CameraShaker : MonoBehaviour
{
    public Transform cameraObject;
    public float amplitude;
    public float frequency;
    public float duration;
    public float timeRemaining;
    private Vector3 noiseOffset;
    private Vector3 noise;
    private AnimationCurve smoothCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * 720.0f), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));

    void Start()
    {
        float rand = 32.0f;
        noiseOffset.x = Random.Range(0.0f, rand);
        noiseOffset.y = Random.Range(0.0f, rand);
        noiseOffset.z = Random.Range(0.0f, rand);
    }

    public IEnumerator Shake(float amp, float freq, float dur, float wait)
    {
        yield return new WaitForSeconds(wait);
        float rand = 32.0f;
        noiseOffset.x = Random.Range(0.0f, rand);
        noiseOffset.y = Random.Range(0.0f, rand);
        noiseOffset.z = Random.Range(0.0f, rand);
        amplitude = amp;
        frequency = freq;
        duration = dur;
        timeRemaining += dur;
        if (timeRemaining > dur)
        {
            timeRemaining = dur;
        }
    }

    void Update()
    {
        if (timeRemaining <= 0)
            return;

        float deltaTime = Time.deltaTime;
        timeRemaining -= deltaTime;
        float noiseOffsetDelta = deltaTime * frequency;

        noiseOffset.x += noiseOffsetDelta;
        noiseOffset.y += noiseOffsetDelta;
        noiseOffset.z += noiseOffsetDelta;

        noise.x = Mathf.PerlinNoise(noiseOffset.x, 0.0f);
        noise.y = Mathf.PerlinNoise(noiseOffset.y, 1.0f);
        noise.z = Mathf.PerlinNoise(noiseOffset.z, 2.0f);

        noise -= Vector3.one * 0.5f;
        noise *= amplitude;

        float agePercent = 1.0f - (timeRemaining / duration);
        noise *= smoothCurve.Evaluate(agePercent);
    }

    void LateUpdate()
    {
        if (timeRemaining <= 0)
            return;
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;
        positionOffset += noise;
        rotationOffset += noise;
        cameraObject.transform.localPosition = positionOffset;
        cameraObject.transform.localEulerAngles = rotationOffset;
    }
}
