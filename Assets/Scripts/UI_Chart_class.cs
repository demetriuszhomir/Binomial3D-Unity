using UnityEngine;

public class UI_Chart_class : MonoBehaviour
{
    //chart behavior

    //rotate cursor
    public Texture2D cursorRotate;
    public GameObject UI;

    private void OnMouseDrag()
    {
        //on mouse dragging the chart

        //camera rotating
        float rotX = Input.GetAxis("Mouse X") * 100 * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * 100 * Mathf.Deg2Rad;
        transform.Rotate(Vector3.forward, rotY);
        transform.Rotate(Vector3.up, -rotX);
    }

    private void OnMouseDown()
    {
        //on clicking the chart

        //showing rotate cursor
        Cursor.SetCursor(cursorRotate, new Vector2(12, 12), CursorMode.ForceSoftware);
    }

    private void OnMouseUp()
    {
        //on unclicking the chart

        //return the standard cursor
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}