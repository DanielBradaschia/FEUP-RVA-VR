using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    Transform xrrig;
    [SerializeField]
    float speed = 10f;

    InputMaster controls;
    Vector2 move;
    RaycastHit hit;
    bool teleport = false;
    bool teleportIndicator = false;
    GameObject indicator;
    

    void Awake()
    {
        controls = new InputMaster();
        
        controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => move = Vector2.zero;

        controls.Player.Teleport.canceled += ctx => TeleportPlayer();
        controls.Player.Teleport.performed += ctx => ShowIndicator();
    }

    void TeleportPlayer()
    {
        teleport = true;
        teleportIndicator = false;
    }
    
    void ShowIndicator()
    {
        teleportIndicator = true;
        indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.name = "Indicator";
        indicator.transform.position = new Vector3(0, -10, 0);
        indicator.GetComponent<Renderer>().material.color = Color.red;
        Destroy(indicator.GetComponent<Collider>());
    }
    
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        if (move != Vector2.zero)
        {
            
            float cx = cameraTransform.forward.x;
            float cz = cameraTransform.forward.z;
            Vector2 look = new Vector2(cx, cz);
            look = look.normalized;
            cx = look.x;
            cz = look.y;
            move = move.normalized;



            float angle = Vector2.Angle(Vector2.up, move) * Mathf.Deg2Rad;

            if (move.x < 0)
                angle = -angle;

            Debug.Log(angle * Mathf.Rad2Deg);

            Vector2 movement = new Vector2(cx * Mathf.Cos(angle) + cz * Mathf.Sin(angle), -cx * Mathf.Sin(angle) + cz * Mathf.Cos(angle));
            
            if ((transform.position.x + movement.x * speed * Time.deltaTime <= 500 && transform.position.x + movement.x * speed * Time.deltaTime >= -416) &&
                (transform.position.z + movement.y * speed * Time.deltaTime <= 482 && transform.position.z + movement.y * speed * Time.deltaTime >= -482))
            {
                transform.Translate(new Vector3(movement.x, 0, movement.y) * speed * Time.deltaTime);
            }


        }

        if(teleportIndicator)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100000))
            {
                //Show indicator
                float scalingDist = Vector3.Distance(cameraTransform.position, hit.point);
                indicator.transform.position = new Vector3(hit.point.x, (1f + scalingDist / 10)/2, hit.point.z);
                indicator.transform.localScale = new Vector3(0.5f + scalingDist/20, 0.5f + scalingDist / 20, 0.5f + scalingDist / 20);
            }
        }
        else if (teleport)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100000))
            {
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }
            teleport = false;
        }
        else
        {
            Destroy(GameObject.Find("Indicator"));
        }
        
    }
    
}
