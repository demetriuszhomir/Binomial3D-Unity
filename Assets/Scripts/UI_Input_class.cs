using UnityEngine;

public class UI_Input_class : MonoBehaviour
{
    //класс поведения полей ввода

    //курсор над полем ввода
    public Texture2D cursorInput;
    //объект графического интерфейса
    public GameObject UI;

    public void OnMouseEntered()
    {
        //при наведении курсора на поле ввода

        //активация курсора над полем ввода
        Cursor.SetCursor(cursorInput, new Vector2(8, 8), CursorMode.ForceSoftware);
    }

    public void OnMouseLeft()
    {
        //при отведении курсора от поля ввода

        //возврат стандартного курсора
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}