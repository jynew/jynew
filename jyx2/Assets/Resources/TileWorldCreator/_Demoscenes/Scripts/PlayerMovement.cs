using UnityEngine;
using System.Collections;
using TileWorld;

public class PlayerMovement : MonoBehaviour
{
	public TileWorldCreator creator;
	
	public float speed = 6f;
    public bool randomMovement;
	Vector3 movement;					
	Vector3 lastMovement;
	
	Rigidbody playerRigidbody;
    Plane planeGround = new Plane(Vector3.up, Vector3.up);
    Vector3 targetPos;

    void Awake ()
	{
		creator = GameObject.Find("TileWorldCreator").GetComponent<TileWorldCreator>();
       
		playerRigidbody = GetComponent <Rigidbody> ();
	}

    void Start()
    {
        if (randomMovement)
        {
            SetNewTarget();
        }
    }
	
	void FixedUpdate ()
	{    
		Move ();

        if (!randomMovement)
        {
            Turning();
        }
        else
        {
            TurnToPos();
        }
	}
	
    void OnGUI()
    {
        if (!randomMovement)
        {
            GUILayout.Label("move player by using the arrow keys and the mouse");
        }
    }

	void Move ()
	{
        if (!randomMovement)
        {
            if (Input.GetKey("up"))
            {
                this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
        }
        else
        {
            float _dist = Vector3.Distance(this.transform.position, targetPos);
            if (_dist < 1)
            {
                SetNewTarget();
            }
            this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
	}
	
	void Turning ()
	{
        // Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		
        float _dist = 1000f;
        // Perform the raycast
		if (planeGround.Raycast(camRay, out _dist ))
        {
            Vector3 _hit = camRay.GetPoint(_dist);

            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = _hit - transform.position;
			
            // Ensure the vector is entirely along the floor plane.
			playerToMouse.y = 0f;
			
            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			
            // Set the player's rotation to this new rotation.
			playerRigidbody.MoveRotation (newRotation);
		}
	}

    void SetNewTarget()
    {
        float _x = Random.Range(0, creator.configuration.global.width);
        float _z = Random.Range(0, creator.configuration.global.height);

        targetPos = new Vector3(_x, 0f, _z);
    }

    void TurnToPos()
    {
       
        Vector3 _playerToPos = targetPos - transform.position;

        Quaternion _newRot = Quaternion.LookRotation(_playerToPos);

        playerRigidbody.MoveRotation(_newRot);
    }
}