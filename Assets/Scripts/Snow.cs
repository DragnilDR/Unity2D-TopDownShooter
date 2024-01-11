using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    private ParticleSystem snow;

    [SerializeField] private float minSnowTime, maxSnowTime;
    [SerializeField] private float snowTime;

    [SerializeField]private bool enableSnow = false;
    private bool enableMethod = false;

    private void Start()
    {
        snow = GetComponent<ParticleSystem>();

        Invoke("RestartSnow", Random.Range(50, 500));
    }

    private void Update()
    {
        SnowTime();
        EnableSnow();
    }

    private void SnowTime()
    {
        if (snowTime <= 0)
        {
            enableSnow = false;
        }
        else
        {
            enableSnow = true;
            snowTime -= Time.deltaTime;
        }
    }

    private void EnableSnow()
    {
        if (enableSnow == true)
        {
            snow.Play();
        }
        else
        {
            snow.Stop();

            if (!enableMethod)
            {
                Invoke("RestartSnow", Random.Range(50, 500));
                enableMethod = true;
            }
        }
    }

    private void RestartSnow()
    {
        snowTime = Random.Range(minSnowTime, maxSnowTime + 1f);
        enableMethod = false;
        snow.Simulate(0.0f, true, true);
    }
}
