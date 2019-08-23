using UnityEngine;
using UnityEngine.UI;

public class UI_Build_class : MonoBehaviour
{
    //behavior of the chart building button

    //button unavailable cursor
    public Texture2D cursorDisabled;
    public GameObject UI;

    //mouse over a button flag
    bool mouseOver;

    private void FixedUpdate()
    {
        //return the standard cursor when a button is available

        if (mouseOver && GetComponent<Button>().interactable) OnMouseLeft_Disabled();
    }

    public void OnMouseEntered_Disabled()
    {
        //on mouse over an unavailable button

        mouseOver = true;
        //showing the cursor of unavailability
        if (!GetComponent<Button>().interactable) Cursor.SetCursor(cursorDisabled, new Vector2(8, 8), CursorMode.ForceSoftware);
    }

    public void OnMouseLeft_Disabled()
    {
        //on mouse left an unavailable button

        mouseOver = false;
        //return the standard cursor
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}