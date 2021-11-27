using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
//Also you need to shoose Firepoint, targets > 1, Aim image from canvas and 2 target markers and camera.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
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
    public bool canMove;

    [Space]
    [Header("Effects")]
    public GameObject TargetMarker;
    public GameObject TargetMarker2;
    public GameObject[] Prefabs;
    public GameObject[] PrefabsCast;
    public GameObject[] UltimatePrefab;
    private bool canUlt = false;
    private bool useUlt = false;
    private ParticleSystem currEffect;
    private ParticleSystem Effect;
    public float[] castingTime; //If 0 - can loop, if > 0 - one shot time
    private bool casting;
    private Transform parentObject;
    private Transform parentForUlt;
    private int currNumber;
    public LayerMask collidingLayer = ~0; //Target marker can only collide with scene layer
    private bool fastSkillrefresh = false;

    [Space]
    [Header("Canvas")]
    public GameObject[] ultIcons;
    public Image aim;
    public Vector2 uiOffset;
    public List<Transform> screenTargets = new List<Transform>();
    private Transform target;
    private bool activeTarger = false;
    public Transform FirePoint;
    public float fireRate = 0.1f;
    private float fireCountdown = 0f;
    private bool rotateState = false;

    [Space]
    [Header("Sound effects")]
    private AudioSource soundComponent; //Play audio from Prefabs
    private AudioClip clip;
    private AudioSource soundComponentCast; //Play audio from PrefabsCast
    private AudioSource soundComponentUlt; //Play audio from PrefabsCast

    [Space]
    [Header("Camera Shaker script")]
    public CameraShaker cameraShaker;

    void Start()
    {
        casting = false;
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        
        //Get clip from Audiosource from projectile if exist for playing when shooting
        if (Prefabs[8].GetComponent<AudioSource>())
        {
            soundComponent = Prefabs[8].GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        target = screenTargets[targetIndex()];

        if (Input.GetMouseButtonDown(1) && casting == true)
        {
            casting = false;
        }
        if (Input.GetKeyDown("1"))
        {
            if (canUlt)
            {
                useUlt = true;
            }
            else
                StartCoroutine(PreCast(0));
        }
        if (Input.GetKeyDown("2") && casting == false)
        {
            if (canUlt)
            {
                useUlt = true;
            }
            else if (!fastSkillrefresh)
                StartCoroutine(FastPlay(1));
        }
        if (Input.GetKeyDown("3"))
        {
            StartCoroutine(PreCast(2));
        }
        if (Input.GetKeyDown("4"))
        {
            StartCoroutine(PreCast(3));
        }
        if (Input.GetKeyDown("z"))
        {
            StartCoroutine(FrontAttack(4));
        }
        if (Input.GetKeyDown("x"))
        {
            StartCoroutine(FrontAttack(5));
        }
        if (Input.GetKeyDown("c"))
        {
            if (canUlt)
            {
                useUlt = true;
            }
            else
                StartCoroutine(PreCast(6));
        }
        if (Input.GetKeyDown("v"))
        {
            if (canUlt)
            {
                useUlt = true;
            }
            else
                StartCoroutine(FrontAttack(7));
        }

        UserInterface();

        //Disable moving and skills if alrerady using one
        if (!canMove)
            return;

        if (Input.GetMouseButton(0) && aim.enabled == true && activeTarger)
        {
            if (rotateState == false)
            {
                StartCoroutine(RotateToTarget(fireRate, target.position));
            }
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 1, Time.deltaTime * 10);
            if (fireCountdown <= 0f)
            {
                GameObject projectile = Instantiate(PrefabsCast[8], FirePoint.position, FirePoint.rotation);
                projectile.GetComponent<TargetProjectile>().UpdateTarget(target, (Vector3)uiOffset);
                Effect = Prefabs[8].GetComponent<ParticleSystem>();
                Effect.Play();
                //Get Audiosource from Prefabs if exist
                if (Prefabs[8].GetComponent<AudioSource>())
                {
                    soundComponent = Prefabs[8].GetComponent<AudioSource>();
                    clip = soundComponent.clip;
                    soundComponent.PlayOneShot(clip);
                }
                StartCoroutine(cameraShaker.Shake(0.1f, 2, 0.2f, 0));
                fireCountdown = 0;
                fireCountdown += fireRate;
            }
        }
        else
        {
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 0, Time.deltaTime * 10);
        }
        fireCountdown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && aim.enabled == true && activeTarger)
        {
            if (rotateState == false)
            {
                StartCoroutine(RotateToTarget(fireRate, target.position));
            }
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 1, Time.deltaTime * 10);

            GameObject buff = Instantiate(PrefabsCast[9], target.position, target.rotation);
            buff.transform.parent = target;
            ParticleSystem buffPS = buff.GetComponent<ParticleSystem>();
            Destroy(buff, buffPS.main.duration);
            Effect = Prefabs[9].GetComponent<ParticleSystem>();
            Effect.Play();
            if (Prefabs[9].GetComponent<AudioSource>())
            {
                soundComponent = Prefabs[9].GetComponent<AudioSource>();
                clip = soundComponent.clip;
                soundComponent.PlayOneShot(clip);
            }
            StartCoroutine(cameraShaker.Shake(0.15f, 2, 0.2f, 0));
        }
        else
        {
            secondLayerWeight = Mathf.Lerp(secondLayerWeight, 0, Time.deltaTime * 10);
        }

        //Need second layer in the Animator
        if (anim.layerCount > 1) { anim.SetLayerWeight(1, secondLayerWeight); }

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

    public IEnumerator FastPlay(int EffectNumber)
    {
        fastSkillrefresh = true;
        Effect = Prefabs[EffectNumber].GetComponent<ParticleSystem>();
        Effect.Play();
        Transform parentPlace = PrefabsCast[EffectNumber].transform.parent;
        PrefabsCast[EffectNumber].transform.parent = null;
        currEffect = PrefabsCast[EffectNumber].GetComponent<ParticleSystem>();
        currEffect.Play();
        //Get Audiosource from Prefabs if exist
        if (Prefabs[EffectNumber].GetComponent<AudioSource>())
        {
            soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
            var clip = soundComponent.clip;
            soundComponent.PlayOneShot(clip);
        }
        //Get Audiosource from PrefabsCast if exist
        if (PrefabsCast[EffectNumber].GetComponent<AudioSource>())
        {
            soundComponentCast = PrefabsCast[EffectNumber].GetComponent<AudioSource>();
            var clip = soundComponentCast.clip;
            soundComponentCast.PlayOneShot(clip);
        }
        //Shake camera
        if (EffectNumber == 1) StartCoroutine(cameraShaker.Shake(0.3f, 5, 0.5f, 0));
        //Use ult after main skill
        if (UltimatePrefab[EffectNumber] != null)
            StartCoroutine(Ult(EffectNumber, 0f, 1.5f, new Vector3(0, 0, 0), transform.rotation, false));
        yield return new WaitForSeconds(castingTime[EffectNumber]);
        PrefabsCast[EffectNumber].transform.parent = parentPlace;
        PrefabsCast[EffectNumber].transform.position = parentPlace.position;
        fastSkillrefresh = false;
        yield break;
    }

    public IEnumerator Ult(int EffectNumber, float enableTime, float dissableTime, Vector3 pivotPosition, Quaternion pivotRotation, bool ChangePos)
    {
        yield return new WaitForSeconds(enableTime);
        canUlt = true;
        //Enable Ult icon
        ultIcons[EffectNumber].SetActive(true);
        while (true)
        {
            dissableTime -= Time.deltaTime;
            if (UltimatePrefab[EffectNumber] && useUlt)
            {
                //Change Ult skill position and rotation to targetMarker values
                if (ChangePos == true)
                {
                    parentForUlt = UltimatePrefab[EffectNumber].transform.parent;
                    UltimatePrefab[EffectNumber].transform.parent = null;
                    /*(Vector3(1, 1, 1) is a secret number to place effect on the player position because of late using. 
                    After all, it may be that the position passed 1 second ago is out of date.*/
                    if (pivotPosition != new Vector3(1, 1, 1)) UltimatePrefab[EffectNumber].transform.position = pivotPosition;
                    UltimatePrefab[EffectNumber].transform.rotation = pivotRotation;
                }
                //Get Audiosource from Prefabs if exist
                if (UltimatePrefab[EffectNumber].GetComponent<AudioSource>())
                {
                    soundComponentUlt = UltimatePrefab[EffectNumber].GetComponent<AudioSource>();
                    soundComponentUlt.Play(0);
                }
                //Play effect
                ParticleSystem ultPS = UltimatePrefab[EffectNumber].GetComponent<ParticleSystem>();
                ultPS.Play();
                //Shake camera
                if (EffectNumber == 0)  StartCoroutine(cameraShaker.Shake(0.4f, 5, 0.35f, 0.1f));
                if (EffectNumber == 1) StartCoroutine(cameraShaker.Shake(0.15f, 2, 0.2f, 0));
                if (EffectNumber == 6) StartCoroutine(cameraShaker.Shake(0.2f, 7, 3, 0));
                if (EffectNumber == 7) StartCoroutine(cameraShaker.Shake(0.55f, 7.5f, 0.35f, 0));
                //Disable Ult icon
                ultIcons[EffectNumber].SetActive(false);
                canUlt = useUlt = false;
                yield return new WaitForSeconds(ultPS.main.duration);
                if (ChangePos == true)
                {
                    UltimatePrefab[currNumber].transform.parent = parentForUlt;
                    UltimatePrefab[currNumber].transform.localPosition = new Vector3(0, 0, 0);
                }
                yield break;
            }
            if (dissableTime <= 0)
            {
                //Disable Ult icon
                ultIcons[EffectNumber].SetActive(false);
                canUlt = useUlt = false;
                yield break;
            }
            yield return null;
        }
    }

    private void UserInterface()
    {
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + (Vector3)uiOffset);
        Vector3 CornerDistance = screenPos - screenCenter;
        Vector3 absCornerDistance = new Vector3(Mathf.Abs(CornerDistance.x), Mathf.Abs(CornerDistance.y), Mathf.Abs(CornerDistance.z));

        //This way you can find target on the full screen
        //if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        // {screenPos.x > 0 && screenPos.y > 0 && screenPos.z > 0} - disable target if enemy backside
        //Find target near center of the screen     
        if (absCornerDistance.x < screenCenter.x / 3 && absCornerDistance.y < screenCenter.y / 3 && screenPos.x > 0 && screenPos.y > 0 && screenPos.z > 0 //If target is in the middle of the screen
            && !Physics.Linecast(transform.position + (Vector3)uiOffset, target.position + (Vector3)uiOffset * 2, collidingLayer)) //If player can see the target
        {
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenPos, Time.deltaTime * 3000);
            if (!activeTarger)
                activeTarger = true;
        }
        else
        {
            //Another way
            //aim.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            aim.transform.position = Vector3.MoveTowards(aim.transform.position, screenCenter, Time.deltaTime * 3000);
            if (activeTarger)
                activeTarger = false;
        }
    }

    public void MainSoundPlay()
    {
        clip = soundComponent.clip;
        soundComponent.PlayOneShot(clip);
    }

    public void CastSoundPlay()
    {
        soundComponentCast.Play(0);
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
            if (Speed == 0) { transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 20); }
            delay -= Time.deltaTime;
            if (delay <= 0 || transform.rotation == rotation)
            {
                rotateState = false;
                yield break;
            }
            yield return null;
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
                        StartCoroutine(RotateToTarget(1, vecPos));
                    }
                    anim.SetTrigger("FrontAttack");
                    StartCoroutine(cameraShaker.Shake(0.4f, 7, 0.45f, 1f));

                    //Play sound FX if exist
                    if (Prefabs[EffectNumber].GetComponent<AudioSource>())
                    {
                        soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
                        MainSoundPlay();
                    }

                    yield return new WaitForSeconds(1);
                    foreach (var component in Prefabs[EffectNumber].GetComponentsInChildren<FrontAttack>())
                    {
                        component.PrepeareAttack(vecPos);
                    }
                    //Use ult after main skill
                    if (UltimatePrefab[EffectNumber] != null)
                    {
                        if (EffectNumber == 7)
                        {
                            StartCoroutine(Ult(EffectNumber, 0f, 1.5f, new Vector3(1, 1, 1), Quaternion.LookRotation(forwardCamera), true));
                        }
                        else
                            StartCoroutine(Ult(EffectNumber, 0f, 1.5f, new Vector3(0, 0, 0), Quaternion.LookRotation(forwardCamera), false));
                    }
                    yield return new WaitForSeconds(castingTime[EffectNumber]);
                    StopCasting(EffectNumber);
                    aim.enabled = true;
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
        if (PrefabsCast[EffectNumber] && TargetMarker)
        {
            //Waiting for confirm or deny
            while (true)
            {
                aim.enabled = false;
                TargetMarker.SetActive(true);
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
                    aim.enabled = true;
                    TargetMarker.SetActive(false);
                    soundComponentCast = null;
                    if (rotateState == false)
                    {
                        StartCoroutine(RotateToTarget(1, hit.point));
                    }
                    casting = true;
                    PrefabsCast[EffectNumber].transform.position = hit.point;
                    PrefabsCast[EffectNumber].transform.rotation = Quaternion.LookRotation(forwardCamera);
                    parentObject = PrefabsCast[EffectNumber].transform.parent;
                    PrefabsCast[EffectNumber].transform.parent = null;
                    currEffect = PrefabsCast[EffectNumber].GetComponent<ParticleSystem>();
                    Effect = Prefabs[EffectNumber].GetComponent<ParticleSystem>();
                    //Get Audiosource from Prefabs if exist
                    if (Prefabs[EffectNumber].GetComponent<AudioSource>())
                    {
                        soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
                    }
                    //Get Audiosource from PrefabsCast if exist
                    if (PrefabsCast[EffectNumber].GetComponent<AudioSource>())
                    {
                        soundComponentCast = PrefabsCast[EffectNumber].GetComponent<AudioSource>();
                    }
                    StartCoroutine(OnCast(EffectNumber));
                    StartCoroutine(Attack(EffectNumber));
                    //Use ult after main skill
                    if (UltimatePrefab[EffectNumber] != null)
                        StartCoroutine(Ult(EffectNumber, 0.5f, castingTime[EffectNumber], hit.point, Quaternion.LookRotation(forwardCamera), true));
                    yield break;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    aim.enabled = true;
                    TargetMarker.SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }
        else if (casting == false)
        {
            Effect = Prefabs[EffectNumber].GetComponent<ParticleSystem>();
            ////Get Audiosource from prefab if exist
            if (Prefabs[EffectNumber].GetComponent<AudioSource>())
            {
                soundComponent = Prefabs[EffectNumber].GetComponent<AudioSource>();
            }
            casting = true;
            StartCoroutine(Attack(EffectNumber));
            yield break;
        }
        else
            yield break;
    }

    IEnumerator OnCast(int EffectNumber)
    {
        while (true)
        {
            if (casting)
            {
                if (castingTime[EffectNumber] == 0)
                {
                    //Play PrefabCast VFX
                    currEffect.Play();
                    if (soundComponentCast)
                    {
                        CastSoundPlay();  
                    }
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    //Play PrefabCast VFX
                    currEffect.Play();
                    //Camera shake
                    if (EffectNumber == 0) StartCoroutine(cameraShaker.Shake(0.2f, 5, 2, 1.5f));
                    if (EffectNumber == 3) StartCoroutine(cameraShaker.Shake(0.6f, 6, 0.3f, 1.45f));
                    if (soundComponentCast)
                    {
                        CastSoundPlay();
                    }
                    yield return new WaitForSeconds(castingTime[EffectNumber]);
                    yield break;
                }
            }
            else yield break;
        }
    }

    //For standing after skill animation
    private void SetAnimZero()
    {
        anim.SetFloat("InputMagnitude", 0);
        anim.SetFloat("InputZ", 0);
        anim.SetFloat("InputX", 0);
    }

    public IEnumerator Attack(int EffectNumber)
    {
        //Block moving after using the skill
        canMove = false;
        SetAnimZero();

        while (true)
        {
            if (casting)
            {
                if (castingTime[EffectNumber] == 0)
                {
                    //Activate animation
                    if (EffectNumber == 2)
                    {
                        anim.SetTrigger("Attack1");
                        StartCoroutine(cameraShaker.Shake(0.2f, 6, 1.5f, 0));
                    }
                    //Activate repeating casting on the character
                    Effect.Play();
                    //Play sound FX if exist
                    if (soundComponent)
                    {
                        MainSoundPlay();
                    }
                    yield return new WaitForSeconds(0.9f);
                }
                else
                {
                    //Activate animation
                    if (EffectNumber == 0 || EffectNumber == 6)
                    {
                        anim.SetTrigger("Attack1");
                        if(EffectNumber == 0) StartCoroutine(cameraShaker.Shake(0.2f, 5, 3, 0));
                        if (EffectNumber == 6) StartCoroutine(cameraShaker.Shake(0.45f, 6, 0.5f, 0.8f));
                    }
                    if (EffectNumber == 3)
                    {
                        anim.SetTrigger("Attack2");
                        StartCoroutine(cameraShaker.Shake(0.2f, 5, 3, 0));
                    }
                    /*if (EffectNumber == 1)
                    {
                        anim.SetTrigger("Idle");
                    }*/
                    Effect.Play();
                    //Play sound FX if exist
                    if (soundComponent)
                    {
                        MainSoundPlay();
                    }
                    yield return new WaitForSeconds(castingTime[EffectNumber]);
                    StopCasting(EffectNumber);
                    yield break;
                }
            }
            else
            {
                //yield return new WaitForSeconds(Effect.main.duration);
                StopCasting(EffectNumber);
                yield break;
            }
            yield return null;
        }
    }

    public void StopCasting(int EffectNumber)
    {
        soundComponent = null;
        soundComponentCast = null;
        if (PrefabsCast[EffectNumber])
        {
            PrefabsCast[EffectNumber].transform.parent = parentObject;
            PrefabsCast[EffectNumber].transform.localPosition = new Vector3(0, 0, 0);
        }
        if (EffectNumber == 2)
            anim.Play("Blend Tree");
        currNumber = EffectNumber;
        casting = false;
        canMove = true;
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
            //You can use desiredMoveDirection if using InputMagnitude instead of Horizontal&Vertical axis
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), desiredRotationSpeed);
            //Limit back speed
            if (InputZ < -0.5)
                controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 1.5f));
            //else if (InputX < -0.1 || InputX > 0.1)
            //    controller.Move(desiredMoveDirection * Time.deltaTime * (velocity / 1.2f));
            else
                controller.Move(desiredMoveDirection * Time.deltaTime * velocity);
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
