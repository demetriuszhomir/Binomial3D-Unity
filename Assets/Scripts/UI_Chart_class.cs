using UnityEngine;

public class UI_Chart_class : MonoBehaviour
{
    //класс поведения графика

    //курсор вращения камеры
    public Texture2D cursorRotate;
    //объект графического интерфейса
    public GameObject UI;

    private void OnMouseDrag()
    {
        //при перетаскивании курсора мыши с нажатой левой кнопкой над графиком

        //вращение камеры
        float rotX = Input.GetAxis("Mouse X") * 100 * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * 100 * Mathf.Deg2Rad;
        transform.Rotate(Vector3.forward, rotY);
        transform.Rotate(Vector3.up, -rotX);
    }

    private void OnMouseDown()
    {
        //при нажатии левой кнопкой мыши на график

        //активация курсора вращения
        Cursor.SetCursor(cursorRotate, new Vector2(12, 12), CursorMode.ForceSoftware);
    }

    private void OnMouseUp()
    {
        //при снятии нажатия левой кнопкой мыши на график

        //возврат стандартного курсора
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}