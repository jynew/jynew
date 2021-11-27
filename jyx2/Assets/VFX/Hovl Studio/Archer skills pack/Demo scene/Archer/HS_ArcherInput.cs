using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
//Also you need to shoose Firepoint, targets > 1, Aim image from canvas and 2 target markers and camera.
[RequireComponent(typeof(CharacterController))]
public class HS_ArcherInput : MonoBehaviour
{
    public float velocity = 9;
    [Space]

    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;
    private float secondLayerWeight = 0;

    [Space]
    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    private float verticalVel;
    private Vector3 moveVector;
    public bool aimMoving = false;
    private float aimTimer = 0;
    public bool canMove;

    [Space]
    [Header("Effects")]
    public GameObject TargetMarker;
    public GameObject TargetMarker2;
    public GameObject[] Prefabs;
    public GameObject[] PrefabsCast;
    public float[] castingTime; //If 0 - can loop, if > 0 - one shot time
    private bool casting = false;
    public LayerMask collidingLayer = ~0; //Target marker can only collide with scene layer
    private Transform parentObject;

    [Space]
    [Header("Canvas")]
    public Image aim;
    public Vector2 uiOffset;
    public List<Transform> screenTargets = new List<Transform>();
    private Transform target;
    private bool activeTarger = false;
    public Transform FirePoint;
    public float fireRate = 0.1f;
    private float fireCountdown = 0f;
    private bool rotateState = false;

    private AudioSource soundComponent; //Play audio from Prefabs
    private AudioClip clip;
    private AudioSource soundComponentCast; //Play audio from PrefabsCast

    [Space]
    [Header("Camera Shaker script")]
    public HS_CameraShaker cameraShaker;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        target = screenTargets[targetIndex()];
    }

    void Update()
    {
        UserInterface();
        //Disable moving and skills if alrerady using one
        if (aimTimer > 0)
        {
            aimTimer -= Time.deltaTime;
            aimMoving = true;
        }
        else
        {
            if (aimMoving) InputMagnitude();
            aimMoving = false;
        }

        //Need second layer in the Animator
        if (anim.layerCount > 1) { anim.SetLayerWeight(1, secondLayerWeight); }

        if (!canMove)
            return;

        target = screenTargets[targetIndex()];

        if (Input.GetMouseButton(0) && aim.enabled == true)
        {
            aimTimer = 2;
            if (activeTarger)
            {
                if (fireCountdown <= 0f)
                {
                    if (rotateState == false)
                    {
                        StartCoroutine(RotateToTarget(fireRate, target.position));
                        //enable turn animation if the turn deviation to the target is more than 20 degrees
                        var lookPos = target.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        var angle = Quaternion.Angle(transform.rotation, rotation);
                        if (angle > 20)
                        {
                            //turn animation
                            anim.SetFloat("InputX", 0.3f);
                        }
                    }

                    anim.SetTrigger("MaskAttack1");
                    secondLayerWeight = Mathf.Lerp(secondLayerWeight, 0.5f, Time.deltaTime * 60);
                    PrefabsCast[8].GetComponent<ParticleSystem>().Play();
                    GameObject projectile = Instantiate(Prefabs[5], FirePoint.position, FirePoint.rotation);
                    projectile.GetComponent<TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset);
                    StartCoroutine(cameraShaker.Shake(0.1f, 2, 0.2f, 0));
                    fireCountdown = 0;
                    fireCountdown += fireRate;
                }
            }
        }
        else if (aimTimer < 1)
        {
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 0, Time.deltaTime * 2);
        }
        fireCountdown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && aim.enabled == true && activeTarger)
        {
            if (rotateState == false)
            {
                StartCoroutine(RotateToTarget(fireRate, target.position));
            }
            PrefabsCast[7].GetComponent<ParticleSystem>().Play();
            StartCoroutine(Attack(4));
            if (PrefabsCast[7].GetComponent<AudioSource>())
            {
                soundComponentCast = PrefabsCast[7].GetComponent<AudioSource>();
                clip = soundComponentCast.clip;
                soundComponentCast.PlayOneShot(clip);
            }
        }

        if (Input.GetKeyDown("1") && aim.enabled == true && activeTarger)
        {
            if (rotateState == false)
            {
                StartCoroutine(RotateToTarget(fireRate, target.position));
            }
            for (int i = 0; i < 3; i++)
            {
                PrefabsCast[i].GetComponent<ParticleSystem>().Play();
            }
            if (PrefabsCast[0].GetComponent<AudioSource>())
            {
                soundComponentCast = PrefabsCast[0].GetComponent<AudioSource>();
                clip = soundComponentCast.clip;
                soundComponentCast.PlayOneShot(clip);
            }
            StartCoroutine(Attack(0));
        }

        if (Input.GetMouseButtonDown(1) && casting == true)
        {
            casting = false;
        }

        if (Input.GetKeyDown("2"))
        {
            StartCoroutine(Attack(1));
            PrefabsCast[3].GetComponent<ParticleSystem>().Play();
            if (PrefabsCast[3].GetComponent<AudioSource>())
            {
                soundComponentCast = PrefabsCast[3].GetComponent<AudioSource>();
                clip = soundComponentCast.clip;
                soundComponentCast.PlayOneShot(clip);
            }
        }

        if (Input.GetKeyDown("3"))
        {
            StartCoroutine(FrontAttack(2));
        }

        if (Input.GetKeyDown("4"))
        {
            StartCoroutine(PreCast(3));
        }

        if (Input.GetKeyDown("z") && aim.enabled == true)
        {
            aimTimer = 2;
            if (activeTarger)
            {
                if (fireCountdown <= 0f)
                {
                    if (rotateState == false)
                    {
                        StartCoroutine(RotateToTarget(fireRate, target.position));
                        //enable turn animation if the turn deviation to the target is more than 20 degrees
                        var lookPos = target.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        var angle = Quaternion.Angle(transform.rotation, rotation);
                        if (angle > 20)
                        {
                            //turn animation
                            anim.SetFloat("InputX", 0.3f);
                        }
                    }             
                    StartCoroutine(cameraShaker.Shake(0.4f, 3, 0.3f, 0.9f));
                    fireCountdown = 0;
                    fireCountdown += fireRate;
                }
                PrefabsCast[9].GetComponent<ParticleSystem>().Play();
                PrefabsCast[10].GetComponent<ParticleSystem>().Play();
                if (PrefabsCast[10].GetComponent<AudioSource>())
                {
                    soundComponentCast = PrefabsCast[10].GetComponent<AudioSource>();
                    clip = soundComponentCast.clip;
                    soundComponentCast.PlayOneShot(clip);
                }
                StartCoroutine(Attack(6));
            }
        }

        if (Input.GetKeyDown("x") && aim.enabled == true)
        {
            StartCoroutine(PreCast(7));
        }

        if (Input.GetKeyDown("c") && casting == false)
        {
            StartCoroutine(FrontAttack(8));
        }

        if (Input.GetKeyDown("v") && casting == false)
        {
            StartCoroutine(Attack(9));
        }

        InputMagnitude();

        //If you don't need the character grounded then get rid of this part.
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel = 0;
        }
        else
        {
            verticalVel -= 1f * Time.deltaTime;
        }
        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector);
    }

    public IEnumerator Attack(int EffectNumber)
    {
        //Block moving after using the skill
        if (EffectNumber != 6) canMove = false;

        SetAnimZero();
        while (true)
        {
            if (EffectNumber == 0)
            {
                anim.SetTrigger("Attack1");
                yield return new WaitForSeconds(castingTime[EffectNumber]);
                StartCoroutine(cameraShaker.Shake(0.4f, 7, 0.45f, 0));
                GameObject projectile = Instantiate(Prefabs[0], FirePoint.position, FirePoint.rotation);
                projectile.GetComponent<TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset);
                yield return new WaitForSeconds(0.2f);
            }
            if (EffectNumber == 1)
            {
                anim.SetTrigger("AoE");
                yield return new WaitForSeconds(castingTime[EffectNumber]);
                parentObject = Prefabs[EffectNumber].transform.parent;
                Prefabs[EffectNumber].transform.parent = null;
                Prefabs[EffectNumber].GetComponent<ParticleSystem>().Play();
                StartCoroutine(cameraShaker.Shake(0.4f, 7, 0.6f, 0));
                yield return new WaitForSeconds(castingTime[EffectNumber]);
            }
            if (EffectNumber == 4)
            {
                anim.SetTrigger("Attack2");
                if (PrefabsCast[7].GetComponent<AudioSource>())
                {
                    soundComponentCast = PrefabsCast[7].GetComponent<AudioSource>();
                    clip = soundComponentCast.clip;
                    soundComponentCast.PlayOneShot(clip);
                }
                yield return new WaitForSeconds(castingTime[EffectNumber]);
                StartCoroutine(cameraShaker.Shake(0.4f, 8, 0.45f, 0));
                GameObject projectile = Instantiate(Prefabs[4], FirePoint.position, FirePoint.rotation);
                projectile.GetComponent<TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset);
                yield return new WaitForSeconds(0.3f);
            }
            if (EffectNumber == 6)
            {
                anim.SetTrigger("MaskAttack2");
                secondLayerWeight = Mathf.Lerp(secondLayerWeight, 1f, Time.deltaTime * 60);
                yield return new WaitForSeconds(castingTime[EffectNumber]);
                parentObject = Prefabs[EffectNumber].transform.parent;
                Prefabs[EffectNumber].transform.parent = null;
                Prefabs[EffectNumber].transform.position = target.position;
                Prefabs[EffectNumber].GetComponent<ParticleSystem>().Play();
                if (Prefabs[EffectNumber].GetComponent<AudioSource>())
                {
                    soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
                    clip = soundComponent.clip;
                    soundComponent.PlayOneShot(clip);
                }
                StartCoroutine(cameraShaker.Shake(0.3f, 8, 1.1f, 0.2f));
                yield return new WaitForSeconds(1.5f);
            }
            canMove = true;
            if (EffectNumber == 1 || EffectNumber == 6)
            {
                yield return new WaitForSeconds(0.5f);
                Prefabs[EffectNumber].transform.parent = parentObject;
                Prefabs[EffectNumber].transform.localPosition = new Vector3(0, 0, 0);
                Prefabs[EffectNumber].transform.localRotation = Quaternion.identity;
            }
            if (EffectNumber == 9)
            {
                Prefabs[EffectNumber].GetComponent<ParticleSystem>().Play();
                if (Prefabs[EffectNumber].GetComponent<AudioSource>())
                {
                    soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
                    clip = soundComponent.clip;
                    soundComponent.PlayOneShot(clip);
                }
            }
            yield break;
        }
    }

    public IEnumerator FrontAttack(int EffectNumber)
    {
        if (TargetMarker2 && casting == false)
        {
            aim.enabled = false;
            TargetMarker2.SetActive(true);
            //Waiting for confirm or deny
            while (true)
            {
                var forwardCamera = Camera.main.transform.forward;
                forwardCamera.y = 0.0f;
                TargetMarker2.transform.rotation = Quaternion.LookRotation(forwardCamera);
                var vecPos = transform.position + forwardCamera * 4;

                if (Input.GetMouseButtonDown(0) && casting == false)
                {
                    casting = true;
                    canMove = false;
                    SetAnimZero();
                    TargetMarker2.SetActive(false);
                    if (rotateState == false)
                    {
                        StartCoroutine(RotateToTarget(0.5f, vecPos));
                    }
                    anim.SetTrigger("Attack2");
                    if (EffectNumber == 2)
                    {
                        PrefabsCast[4].GetComponent<ParticleSystem>().Play();
                        soundComponentCast = PrefabsCast[4].GetComponent<AudioSource>();
                        clip = soundComponentCast.clip;
                        soundComponentCast.PlayOneShot(clip);
                    }
                    if (EffectNumber == 8)
                    {
                        if (PrefabsCast[13].GetComponent<AudioSource>())
                        {
                            soundComponentCast = PrefabsCast[13].GetComponent<AudioSource>();
                            clip = soundComponentCast.clip;
                            soundComponentCast.PlayOneShot(clip);
                        }
                        PrefabsCast[13].GetComponent<ParticleSystem>().Play();
                        yield return new WaitForSeconds(castingTime[EffectNumber]);
                    }

                    //Use FrontAttack script if exist (it has own settings)
                    if (Prefabs[EffectNumber].GetComponent<FrontAttack>() != null)
                    {
                        StartCoroutine(cameraShaker.Shake(0.5f, 6, 1.3f, 0.0f));
                        parentObject = Prefabs[EffectNumber].transform.parent;
                        Prefabs[EffectNumber].transform.parent = null;
                        foreach (var component in Prefabs[EffectNumber].GetComponentsInChildren<FrontAttack>())
                        {
                            component.playMeshEffect = true;
                        }
                        yield return new WaitForSeconds(0.2f);
                        aim.enabled = true;
                        canMove = true;
                        yield return new WaitForSeconds(0.8f);
                        Prefabs[EffectNumber].transform.parent = parentObject;
                        Prefabs[EffectNumber].transform.localPosition = new Vector3(0, 0, 0);
                        Prefabs[EffectNumber].transform.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        parentObject = Prefabs[EffectNumber].transform.parent;
                        Prefabs[EffectNumber].transform.parent = null;
                        Prefabs[EffectNumber].transform.rotation = Quaternion.LookRotation(forwardCamera);
                        var effect = Prefabs[EffectNumber].GetComponent<ParticleSystem>();
                        effect.Play();
                        StartCoroutine(cameraShaker.Shake(0.5f, 7, 0.6f, 0.26f));
                        yield return new WaitForSeconds(castingTime[EffectNumber]);
                        aim.enabled = true;
                        canMove = true;
                        yield return new WaitForSeconds(0.5f);
                        Prefabs[EffectNumber].transform.parent = parentObject;
                        Prefabs[EffectNumber].transform.localPosition = new Vector3(0, 1, 0);
                        Prefabs[EffectNumber].transform.localRotation = Quaternion.identity;
                    }
                    casting = false;
                    yield break;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    TargetMarker2.SetActive(false);
                    aim.enabled = true;
                    yield break;
                }
                yield return null;
            }
        }
    }

    public IEnumerator PreCast(int EffectNumber)
    {
        if (PrefabsCast[EffectNumber] && TargetMarker && casting == false)
        {
            aim.enabled = false;
            TargetMarker.SetActive(true);
            //Waiting for confirm or deny
            while (true)
            {
                var forwardCamera = Camera.main.transform.forward;
                forwardCamera.y = 0.0f;
                RaycastHit hit;
                Ray ray = new Ray(Camera.main.transform.position + new Vector3(0, 2, 0), Camera.main.transform.forward);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, collidingLayer))
                {
                    TargetMarker.transform.position = hit.point;
                    TargetMarker.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.LookRotation(forwardCamera);
                }
                else
                {
                    aim.enabled = true;
                    TargetMarker.SetActive(false);
                }

                if (Input.GetMouseButtonDown(0) && casting == false)
                {
                    canMove = false;
                    casting = true;
                    aim.enabled = true;
                    TargetMarker.SetActive(false);
                    if (rotateState == false)
                    {
                        StartCoroutine(RotateToTarget(0.5f, hit.point));
                    }

                    if (EffectNumber == 3)
                    {
                        anim.SetTrigger("UpAttack");
                        if (PrefabsCast[5].GetComponent<AudioSource>())
                        {
                            soundComponentCast = PrefabsCast[5].GetComponent<AudioSource>();
                            clip = soundComponentCast.clip;
                            soundComponentCast.PlayOneShot(clip);
                        }
                        StartCoroutine(cameraShaker.Shake(0.4f, 9, 0.4f, 0.2f));
                        for (int i = 5; i <= 6; i++)
                        {
                            PrefabsCast[i].GetComponent<ParticleSystem>().Play();
                        }
                        yield return new WaitForSeconds(castingTime[EffectNumber]);
                        StartCoroutine(cameraShaker.Shake(0.5f, 7, 1.4f, 0));
                        parentObject = Prefabs[3].transform.parent;
                        Prefabs[3].transform.position = hit.point;
                        Prefabs[3].transform.rotation = Quaternion.LookRotation(forwardCamera);
                        Prefabs[3].transform.parent = null;
                        Prefabs[3].GetComponent<ParticleSystem>().Play();
                        if (PrefabsCast[3].GetComponent<AudioSource>())
                        {
                            soundComponent = Prefabs[3].GetComponent<AudioSource>();
                            clip = soundComponent.clip;
                            soundComponent.PlayOneShot(clip);
                        }
                    }

                    if (EffectNumber == 7)
                    {
                        anim.SetTrigger("UpAttack2");
                        StartCoroutine(cameraShaker.Shake(0.4f, 8, 0.4f, 0.2f));
                        PrefabsCast[11].GetComponent<ParticleSystem>().Play();
                        if (PrefabsCast[11].GetComponent<AudioSource>())
                        {
                            soundComponentCast = PrefabsCast[11].GetComponent<AudioSource>();
                            clip = soundComponentCast.clip;
                            soundComponentCast.PlayOneShot(clip);
                        }
                        PrefabsCast[12].GetComponent<ParticleSystem>().Play();
                        yield return new WaitForSeconds(castingTime[EffectNumber]);
                        StartCoroutine(cameraShaker.Shake(0.3f, 7, 0.4f, 0));
                        parentObject = Prefabs[7].transform.parent;
                        Prefabs[7].transform.position = hit.point;
                        Prefabs[7].transform.rotation = Quaternion.LookRotation(forwardCamera);
                        Prefabs[7].transform.parent = null;
                        Prefabs[7].GetComponent<ParticleSystem>().Play();
                        if (Prefabs[7].GetComponent<AudioSource>())
                        {
                            soundComponent = Prefabs[7].GetComponent<AudioSource>();
                            clip = soundComponent.clip;
                            soundComponent.PlayOneShot(clip);
                        }
                    }

                    canMove = true;
                    if(EffectNumber == 3 && EffectNumber == 7)
                    {
                        yield return new WaitForSeconds(2);
                        Prefabs[EffectNumber].transform.parent = parentObject;
                        Prefabs[EffectNumber].transform.localPosition = new Vector3(0, 1, 0);
                        Prefabs[EffectNumber].transform.localRotation = Quaternion.identity;
                    }
                    casting = false;

                    yield break;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    TargetMarker.SetActive(false);
                    aim.enabled = true;
                    yield break;
                }
                yield return null;
            }
        }
    }

    public void StopCasting(int EffectNumber)
    {
        Prefabs[EffectNumber].transform.parent = parentObject;
        Prefabs[EffectNumber].transform.localPosition = new Vector3(0, 0, 0);
        /*if (EffectNumber == 2)
            anim.Play("Blend Tree");*/
        casting = false;
        canMove = true;
    }

    //For standing after skill animation
    private void SetAnimZero()
    {
        anim.SetFloat("InputMagnitude", 0);
        anim.SetFloat("InputZ", 0);
        anim.SetFloat("InputX", 0);
    }

    //Rotate player to target when attack
    public IEnumerator RotateToTarget(float rotatingTime, Vector3 targetPoint)
    {
        rotateState = true;
        float delay = rotatingTime;
        var lookPos = targetPoint - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        while (true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 20);
            delay -= Time.deltaTime;
            if (delay <= 0 || transform.rotation == rotation)
            {
                rotateState = false;
                yield break;
            }
            yield return null;
        }
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //Movement vector
        desiredMoveDirection = forward * InputZ + right * InputX;

        //Character diagonal movement faster fix
        desiredMoveDirection.Normalize();

        if (blockRotationPlayer == false)
        {
            if (aimMoving)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), desiredRotationSpeed);
                //Limit back speed
                if (InputZ < -0.3)
                    controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 2.4f));
                else if (InputX < -0.1 || InputX > 0.1)
                    controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 2.2f));
                else
                    controller.Move(desiredMoveDirection * Time.deltaTime * velocity / 1.8f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
                controller.Move(desiredMoveDirection * Time.deltaTime * velocity);
            }
        }
    }

    void InputMagnitude()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
        anim.SetFloat("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Change blend trees moving animation
        anim.SetBool("AimMoving", aimMoving);

        //Physically move player
        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("InputMagnitude", Speed, StopAnimTime, Time.deltaTime);
        }
    }

    private void UserInterface()
    {
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + (Vector3)uiOffset);
        Vector3 CornerDistance = screenPos - screenCenter;
        Vector3 absCornerDistance = new Vector3(Mathf.Abs(CornerDistance.x), Mathf.Abs(CornerDistance.y), Mathf.Abs(CornerDistance.z));

        if (absCornerDistance.x < screenCenter.x / 3 && absCornerDistance.y < screenCenter.y / 3 && screenPos.x > 0 && screenPos.y > 0 && screenPos.z > 0 //If target is in the middle of the screen
            && !Physics.Linecast(transform.position + (Vector3)uiOffset, target.position + (Vector3)uiOffset * 2, collidingLayer)) //If player can see the target
        {
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenPos, Time.deltaTime * 3000);
            if (!activeTarger)
                activeTarger = true;
        }
        else
        {
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenCenter, Time.deltaTime * 3000);
            if (activeTarger)
                activeTarger = false;
        }
    }

    public int targetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }
        return index;
    }
}
