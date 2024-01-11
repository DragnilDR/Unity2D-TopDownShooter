using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public Light lightning;
    public float min;
    public float max;
    private bool isFlickering = false;
    private float timeDelay;

    public enum TypeLight { Lamp, PoliceLight, Fire };
    public TypeLight typeLight;

    private void Start()
    {
        lightning = GetComponent<Light>();
    }

    private void Update()
    {
        if (isFlickering == false)
        {
            if (typeLight == TypeLight.Lamp)
            {
                StartCoroutine(FlickeringLampLight());
            }
            else if (typeLight == TypeLight.PoliceLight)
            {
                StartCoroutine(FlickeringPoliceLight());
            }
            else if (typeLight == TypeLight.Fire)
            {
                StartCoroutine(FlickeringFireLight());
            }
        }
    }

    private IEnumerator FlickeringLampLight()
    {
        isFlickering = true;
        lightning.enabled = false;
        timeDelay = Random.Range(min, max);
        yield return new WaitForSeconds(timeDelay);
        lightning.enabled = true;
        timeDelay = Random.Range(min, max);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }

    private IEnumerator FlickeringPoliceLight()
    {
        isFlickering = true;
        lightning.color = Color.red;
        timeDelay = 1f;
        yield return new WaitForSeconds(timeDelay);
        lightning.color = Color.blue;
        timeDelay = 1f;
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }

    private IEnumerator FlickeringFireLight()
    {
        isFlickering = true;
        lightning.intensity = Random.Range(3f, 4f);
        timeDelay = .1f;
        yield return new WaitForSeconds(timeDelay);
        lightning.intensity = Random.Range(3f, 4f);
        timeDelay = .1f;
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }
}
