using UnityEngine;
using UnityEngine.UI;

public class UI_Build_class : MonoBehaviour
{
    //класс поведения кнопки построения графика

    //курсор недоступности кнопки
    public Texture2D cursorDisabled;
    //объект графического интерфейса
    public GameObject UI;

    //флаг нахождения курсора над кнопкой
    bool mouseOver;

    private void FixedUpdate()
    {
        //метод инструкций на каждый синхронизированный кадр

        //-----------------------
        //возврат стандартного курсора при доступности кнопки

        if (mouseOver && GetComponent<Button>().interactable) OnMouseLeft_Disabled();
    }

    public void OnMouseEntered_Disabled()
    {
        //при наведении курсора на недоступную кнопку

        //индикация нахождения курсора над кнопкой
        mouseOver = true;
        //активация курсора недоступности кнопки
        if (!GetComponent<Button>().interactable) Cursor.SetCursor(cursorDisabled, new Vector2(8, 8), CursorMode.ForceSoftware);
    }

    public void OnMouseLeft_Disabled()
    {
        //при отведении курсора от недоступной кнопки

        //индикация отсутствия курсора над кнопкой
        mouseOver = false;
        //возврат стандартного курсора
        Cursor.SetCursor(UI.GetComponent<UI_class>().cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
}