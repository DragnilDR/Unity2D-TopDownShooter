using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_Room : MonoBehaviour
{
    [SerializeField] private enum TypeLoc { Loc1, Loc2, Loc3, Loc4 };
    [SerializeField] private TypeLoc typeLoc;

    [SerializeField] private Vector2 cameraLoc1min;
    [SerializeField] private Vector2 cameraLoc1max;
    [SerializeField] private Vector2 cameraLoc2min;
    [SerializeField] private Vector2 cameraLoc2max;
    [SerializeField] private Vector2 cameraLoc3min;
    [SerializeField] private Vector2 cameraLoc3max;
    [SerializeField] private Vector2 cameraLoc4min;
    [SerializeField] private Vector2 cameraLoc4max;

    [SerializeField] private Vector3 playerChange;
    private CameraFollow cam;

    [SerializeField] private GameObject loc1;
    [SerializeField] private GameObject loc2;
    [SerializeField] private GameObject loc3;
    [SerializeField] private GameObject loc4;

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && typeLoc == TypeLoc.Loc1)
        {
            cam.maxPos = cameraLoc1max;
            cam.minPos = cameraLoc1min;
            collision.transform.position += playerChange;
            loc1.SetActive(true);
            loc2.SetActive(false);
            loc3.SetActive(false);
            loc4.SetActive(false);
        }
        else if (collision.gameObject.tag == "Player" && typeLoc == TypeLoc.Loc2)
        {
            cam.maxPos = cameraLoc2max;
            cam.minPos = cameraLoc2min;
            collision.transform.position += playerChange;
            loc1.SetActive(false);
            loc2.SetActive(true);
            loc3.SetActive(false);
            loc4.SetActive(false);
        }
        else if (collision.gameObject.tag == "Player" && typeLoc == TypeLoc.Loc3)
        {
            cam.maxPos = cameraLoc3max;
            cam.minPos = cameraLoc3min;
            collision.transform.position += playerChange;
            loc1.SetActive(false);
            loc2.SetActive(false);
            loc3.SetActive(true);
            loc4.SetActive(false);
        }
        else if (collision.gameObject.tag == "Player" && typeLoc == TypeLoc.Loc4)
        {
            cam.maxPos = cameraLoc4max;
            cam.minPos = cameraLoc4min;
            collision.transform.position += playerChange;
            loc1.SetActive(false);
            loc2.SetActive(false);
            loc3.SetActive(false);
            loc4.SetActive(true);
        }
    }
}
