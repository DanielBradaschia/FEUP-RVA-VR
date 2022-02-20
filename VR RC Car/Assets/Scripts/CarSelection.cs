using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CarSelection : MonoBehaviour
{
    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    Material indicatorMat;

    InputMaster controls;
    RaycastHit hit;
    bool isIndicator = false;
    bool isSelected = false;
    GameObject indicator;
    GameObject selectedCar;

    void Awake()
    {
        controls = new InputMaster();

        controls.Player.Selection.canceled += _ => SelectCar();
        controls.Player.Selection.performed += _ => ShowIndicator();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void SelectCar()
    {
        isSelected = true;
        isIndicator = false;
    }

    void ShowIndicator()
    {
        
        isIndicator = true;
        indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.name = "SelectIndicator";
        indicator.transform.position = new Vector3(0, -10, 0);
        indicator.GetComponent<Renderer>().material = indicatorMat;

        Destroy(indicator.GetComponent<Collider>());
    }
    
    private void Update()
    {
        if (isIndicator)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100000))
            {
                //Show indicator
                indicator.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                indicator.transform.localScale = new Vector3(4f, 4f, 4f);
            }
        }
        else if (isSelected)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 100000))
            {
                Debug.DrawLine(cameraTransform.position, hit.point);
                Collider[] colliders = Physics.OverlapSphere(hit.point, 2f);
            
                foreach (Collider c in colliders)
                {
                    if(c.gameObject.tag == "CarBody")
                    {
                        selectedCar = c.gameObject.transform.parent.gameObject;
                        GameObject.Find("CarManager").GetComponent<CarManager>().SwapCar(selectedCar);
                        break;
                    }
                }
            }
            
            isSelected = false;
        }
        else
        {
            Destroy(GameObject.Find("SelectIndicator"));
        }

    }
}
