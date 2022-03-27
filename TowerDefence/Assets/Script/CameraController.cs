using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private bool DisableMovement=true;
    public float panSpeed = 10f;
    public float panBorderThickness = 10f;
    [SerializeField] private float limitx;
    [SerializeField] private float limity;

    //public float scrollSpeed=10f;

    void Update()
    {
        if (Game_Managers.gameHasEnded)
        {
            this.enabled = false;
            return;
        }

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableMovement = !DisableMovement;
        }

        if (!DisableMovement)
        {
            return;
        }*/
        if ((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - panBorderThickness) && transform.position.y <= limity)
        {
            transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= panBorderThickness) && transform.position.y >= -limity)
        {
            transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= panBorderThickness) && transform.position.x >= -limitx)
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - panBorderThickness) && transform.position.x <= limitx)
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
        /*float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = transform.position;

        pos.z -= scroll * scrollSpeed * Time.deltaTime*1000;

        transform.position = pos;*/
    }
}
