using UnityEngine;

public class TurnLightOnWithDelay : MonoBehaviour
{
    Light lt;

    public float delay = 0f;

    // Start is called before the first frame update
    void Start()
    {
        lt = transform.gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0)
            delay -= Time.deltaTime;
        else
            lt.enabled = true;
    }
}
