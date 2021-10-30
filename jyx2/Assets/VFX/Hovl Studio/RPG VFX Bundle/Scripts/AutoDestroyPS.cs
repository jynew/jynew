using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This code destroys the particle's GameObject once it's Start Time is over.
public class AutoDestroyPS : MonoBehaviour
{
    private float timeLeft;

    private void Awake()
    {
        ParticleSystem system = GetComponent<ParticleSystem>();
        var main = system.main;
        timeLeft = main.startLifetimeMultiplier + main.duration;
        Destroy(gameObject, timeLeft);
    }

    /*--------------------------bad variant------------------------
    public void Awake()
    {
        ParticleSystem system = GetComponent<ParticleSystem>();
        var main = system.main;
        timeLeft = main.startLifetimeMultiplier + main.duration;
        //Destroy(gameObject, main.startLifetimeMultiplier + main.duration);
    }
    public void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
    -----------------------------------------------------------*/
}