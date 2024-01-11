using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    private Camera cachedCamera;

    public Transform player;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    [SerializeField] private float threshold;

    private Vector3 velocity = Vector3.zero;

    public Vector2 maxPos;
    public Vector2 minPos;

    private void Start()
    {
        Instance = this;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        cachedCamera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        if (player.gameObject.activeSelf && PauseMenu.Instance.pauseGame == false)
        {
            Vector3 mousePos = cachedCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = (player.position + mousePos) / 2;

            targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
            targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);
            targetPos.z = -2f;

            Vector3 movePosition = targetPos + offset;

            transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
                                         Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
                                         transform.position.z);
        }
    }
}
