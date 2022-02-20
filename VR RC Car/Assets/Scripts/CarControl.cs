using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class CarControl : MonoBehaviour
{
    
    InputMaster controls;
    Vector3 initialPosition;
    Quaternion initialRotation;
    IEnumerator coroutine;

    [SerializeField]
    GameObject cone;

    public WheelCollider frontLeftW, frontRightW;
    public WheelCollider rearLeftW, rearRightW;
    public Transform frontLeftT, frontRightT;
    public Transform rearLeftT, rearRightT;
    public ParticleSystem particle;
    public float maxSteerAngle = 30;
    public float maxMotorForce = 100;
    public float decelerationForce = 1;
    public float brakeTorque = 10;

    public float explosion_radius = 5;
    public float explosion_power = 250000;

    public void Explode()
    {
        particle.Play();
        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in mr)
        {
            m.enabled = false;
        }
        coroutine = Respawn(2);
        StartCoroutine(coroutine);
    }


    void Awake ()
    {
        controls = new InputMaster();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in mr)
        {
            m.enabled = true;
        }
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        particle.Stop();

        
    }

    void SlowDown ()
    {
        frontLeftW.motorTorque = 0;
        frontRightW.motorTorque = 0;
    }

    void Steer (float horizontalInput)
    {
        float m_steeringAngle = maxSteerAngle * horizontalInput;
        frontLeftW.steerAngle = m_steeringAngle;
        frontRightW.steerAngle = m_steeringAngle;
    }

    void Stop_steer ()
    {
        frontLeftW.steerAngle = 0;
        frontRightW.steerAngle = 0;
    }

    void Acceleration (float accelerationValue)
    {
        if(accelerationValue > 0.2){
            
            frontLeftW.wheelDampingRate = 0;
            frontRightW.wheelDampingRate = 0;
            frontLeftW.motorTorque = maxMotorForce * accelerationValue * 10;
            frontRightW.motorTorque = maxMotorForce * accelerationValue * 10;
           
        } else {
            Deceleration();
        }

    }

    void Deceleration ()
    {
        frontLeftW.wheelDampingRate = decelerationForce;
        frontRightW.wheelDampingRate = decelerationForce;
        frontLeftW.motorTorque = 0;
        frontRightW.motorTorque = 0;
        
    }

    void Brake (float brakeValue)
    {

        if(frontLeftW.rpm > 10){
            frontLeftW.wheelDampingRate = 50 * brakeValue;
            frontRightW.wheelDampingRate = 50 * brakeValue;
        }
        else{
            if(brakeValue > 0.2){
                frontLeftW.wheelDampingRate = 0;
                frontRightW.wheelDampingRate = 0;
                frontLeftW.motorTorque = -maxMotorForce * brakeValue;
                frontRightW.motorTorque = -maxMotorForce * brakeValue;
            } else {
                Deceleration();
            }
        }
        
    }

    void ResetCar ()
    {
        transform.rotation = Quaternion.identity;
    }

    void OnCollisionEnter (Collision collision)
    {

        Vector3 impulse = collision.impulse/Time.fixedDeltaTime;
        float impulse_max =  Mathf.Max(Mathf.Max(Mathf.Abs(impulse.x), Mathf.Abs(impulse.y)), Mathf.Abs(impulse.z));

        Debug.Log("Biggest impulse is: " + impulse_max);


        if(impulse_max > 170000){

            Debug.Log("Impulse is bigger than threshold");
            Vector3 explosionPos = transform.position;

            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosion_radius);

            foreach (Collider hit in colliders){

                Debug.Log("Found collision");
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null){
                    Debug.Log("EXPLOSION");
                    rb.AddExplosionForce(explosion_power, explosionPos, explosion_radius,3);
                    Explode();
                }
            }
            
        }
    }
    
    void OnEnable ()
    {
        controls.Enable();
    }

    void OnDisable ()
    {
        controls.Disable();
    }

    void UpdateWheelPoses ()
    {
        UpdateWheelPose(frontLeftW, frontLeftT);
        UpdateWheelPose(frontRightW, frontRightT);
        UpdateWheelPose(rearLeftW, rearLeftT);
        UpdateWheelPose(rearRightW, rearRightT);
    }

    void UpdateWheelPose (WheelCollider _collider, Transform _transform)
    {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    void FixedUpdate ()
    {
        cone.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        controls.Car.Steer.canceled += ctx => Stop_steer();

        controls.Car.Accelerate.performed += ctx => Acceleration(ctx.ReadValue<float>());

        controls.Car.Backwards.performed += ctx => Brake(ctx.ReadValue<float>());

        controls.Car.Steer.performed += ctx => Steer(ctx.ReadValue<float>());

        controls.Car.Reset.performed += _ => ResetCar();

        UpdateWheelPoses();
    }


}
