using UnityEngine;

public class UI_Input_class : MonoBehaviour
{
    //fields behavior

    public Texture2D cursorInput;
    public GameObject UI;

    public void OnMouseEntered()
    {
        //on mouse over a field

        //show input cursor
        Cursor.SetCursor(cursorInput, new Vector2(8, 8), CursorMode.ForceSoftware);
    }

    public void OnMouseLeft()
    {
        //on mouse left a field

        //show standard cursor
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}