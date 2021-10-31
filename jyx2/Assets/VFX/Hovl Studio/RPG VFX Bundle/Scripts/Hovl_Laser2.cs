using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class Hovl_Laser2 : MonoBehaviour
{
    public float laserScale = 1;
    public Color laserColor = new Vector4(1,1,1,1);
    public GameObject HitEffect;
    public GameObject FlashEffect;
    public float HitOffset = 0;

    public float MaxLength;

    private bool UpdateSaver = false;
    private ParticleSystem laserPS;
    private ParticleSystem[] Flash;
    private ParticleSystem[] Hit;
    private Material laserMat;
    private int particleCount;
    private ParticleSystem.Particle[] particles;
    private Vector3[] particlesPositions;
    private float dissovleTimer = 0;
    private bool startDissovle = false;

    void Start()
    {
        laserPS = GetComponent<ParticleSystem>();
        laserMat = GetComponent<ParticleSystemRenderer>().material;
        Flash = FlashEffect.GetComponentsInChildren<ParticleSystem>();
        Hit = HitEffect.GetComponentsInChildren<ParticleSystem>();
        laserMat.SetFloat("_Scale", laserScale);
    }

    void Update()
    {
        if (laserPS != null && UpdateSaver == false)
        {
            //Set start laser point
            laserMat.SetVector("_StartPoint", transform.position);
            //Set end laser point
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, MaxLength))
            {
                particleCount = Mathf.RoundToInt(hit.distance / (2 * laserScale));
                if (particleCount < hit.distance / (2 * laserScale))
                {
                    particleCount += 1;
                }
                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", hit.distance);
                laserMat.SetVector("_EndPoint", hit.point);
                if (Hit != null)
                {
                    HitEffect.transform.position = hit.point + hit.normal * HitOffset;
                    HitEffect.transform.LookAt(hit.point);
                    foreach (var AllHits in Hit)
                    {
                        if (!AllHits.isPlaying) AllHits.Play();
                    }
                    foreach (var AllFlashes in Flash)
                    {
                        if (!AllFlashes.isPlaying) AllFlashes.Play();
                    }
                }
            }
            else
            {
                //End laser position if doesn't collide with object
                var EndPos = transform.position + transform.forward * MaxLength;
                var distance = Vector3.Distance(EndPos, transform.position);
                particleCount = Mathf.RoundToInt(distance / (2 * laserScale));
                if (particleCount < distance / (2 * laserScale))
                {
                    particleCount += 1;
                }
                particlesPositions = new Vector3[particleCount];
                AddParticles();

                laserMat.SetFloat("_Distance", distance);
                laserMat.SetVector("_EndPoint", EndPos);
                if (Hit != null)
                {
                    HitEffect.transform.position = EndPos;
                    foreach (var AllPs in Hit)
                    {
                        if (AllPs.isPlaying) AllPs.Stop();
                    }
                }
            }          
        }

        if (startDissovle)
        {
            dissovleTimer += Time.deltaTime;
            laserMat.SetFloat("_Dissolve", dissovleTimer*5);
        }
    }

    void AddParticles()
    {
        //Old particles settings
        /* MA
        var normalDistance = particleCount;
        var sh = LaserPS.shape;
        sh.radius = normalDistance;
        sh.position = new Vector3(0,0, normalDistance);
        LaserPS.emission.SetBursts(new[] { new ParticleSystem.Burst(0f, particleCount + 1) });
        */

        particles = new ParticleSystem.Particle[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            particlesPositions[i] = new Vector3(0f, 0f, 0f) + new Vector3(0f, 0f, i * 2 * laserScale);
            particles[i].position = particlesPositions[i];
            particles[i].startSize3D = new Vector3(0.001f, 0.001f, 2 * laserScale);
            particles[i].startColor = laserColor;
        }
        laserPS.SetParticles(particles, particles.Length);
    }

    public void DisablePrepare()
    {
        transform.parent = null;
        dissovleTimer = 0;
        startDissovle = true;
        UpdateSaver = true;
        if (Flash != null && Hit != null)
        {
            foreach (var AllHits in Hit)
            {
                if (AllHits.isPlaying) AllHits.Stop();
            }
            foreach (var AllFlashes in Flash)
            {
                if (AllFlashes.isPlaying) AllFlashes.Stop();
            }
        }
    }
}
 