#if UNITY_EDITOR
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Camera cam;
    public float offsetY = 12;
    Vector3 InitialPosition;

    private float ConstrainXValue;
    private float ConstrainYValue;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        InitialPosition = transform.position + new Vector3(0f, offsetY, 0f);
    }

    private void Update()
    {
        CheckConstrains();
        if (ConstrainXValue != 0)
        {
            transform.position = cam.ScreenToWorldPoint(new Vector3(ConstrainXValue, Input.mousePosition.y, 0f) + InitialPosition);
        }
        else if(ConstrainYValue != 0)
        {
            transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, ConstrainYValue, 0f) + InitialPosition);
        }
        else
        transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f) + InitialPosition);

        
        
    }

    private void CheckConstrains()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            ConstrainXValue = Input.mousePosition.x;
        if (Input.GetKeyUp(KeyCode.Y))
            ConstrainXValue = 0;

        if (Input.GetKeyDown(KeyCode.X))
            ConstrainYValue = Input.mousePosition.y;
        if (Input.GetKeyUp(KeyCode.X))
            ConstrainYValue = 0;
    }
}
#endif