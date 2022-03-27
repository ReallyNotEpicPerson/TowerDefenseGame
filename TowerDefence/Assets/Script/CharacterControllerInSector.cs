using UnityEngine;

public class CharacterControllerInSector : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool standStill = false;
    [SerializeField] private GameObject canvas;
    private bool hit=false;


    void Update()
    {
        if (canvas.activeSelf == false)
        {
            standStill = false;
        }
        if (standStill == true)
        {
            return;
        }
        if ((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }
        if ((Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.Space) && hit==true)
        {
            canvas.SetActive(true);
            standStill = true;
        }
    }
    public void TurnOff()
    {
        canvas.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        if(!canvas.activeSelf)
        {
            standStill = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("BS");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        hit = false;
    }
}
