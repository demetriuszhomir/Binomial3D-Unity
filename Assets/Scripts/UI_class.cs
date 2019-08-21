using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_class : MonoBehaviour
{
    //класс графического пользовательского интерфейса

    //кнопка построения графика
    public Button Build;
    //поле ввода параметра n
    public InputField n_field;
    //поле ввода шага вероятности
    public InputField probStep_field;
    //контейнер графика
    public GameObject ChartBox;
    //строка подсказок
    public Text hint;
    //стандартный курсор
    public Texture2D cursorDefault;
    //свет
    public Light theLight;
    //предупреждение об ограничении на значения параметров
    public GameObject RangeWarning;
    //материал точки
    public Material dotMat;
    //подсказка к параметрам
    public Text rangeWarning;
    //меню выбора распределения
    public Dropdown distrChoosing;

    //конечный объект последнего отброшенного луча от курсора мыши
    RaycastHit oldHit;
    //флаг нахождения курсора над элементом интерфейса
    bool overUI = false;

    private void Start()
    {
        //метод с инструкциями при запуске скрипта

        //-----------------------
        //установка стандартного курсора мыши

        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void ChangeOverUIbool(bool state)
    {
        //отслеживание нахождения курсора над элементом интерфейса

        overUI = state;

        //регулирование возможности вращения камеры
        if (overUI) ChartBox.GetComponent<Collider>().enabled = false; else ChartBox.GetComponent<Collider>().enabled = true;
    }

    private void FixedUpdate()
    {
        //метод инструкций на каждый синхронизированный кадр

        //-----------------------
        //отображение координат какой-либо точки на графике

        //конечный объект текущего отброшенного луча от курсора мыши
        RaycastHit hit;
        //луч от курсора мыши
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!overUI && Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Values")))
        {
            //при попадании луча на новую точку меняем цвет предыдущей на обычный
            if (oldHit.transform != null && hit.transform != oldHit.transform)
                oldHit.transform.GetComponent<Renderer>().material = dotMat;
            //сохраняем объект точки как последний
            oldHit = hit;

            //выводим координаты в строку подсказок
            hint.text = hit.transform.localPosition.z * 2f + "; " +
                Math.Round(hit.transform.localPosition.y / 10, 6).ToString("0.######") + "; " +
                Math.Round(hit.transform.localPosition.x / 10, 6).ToString("0.######");

            //выделяем точку попадания луча
            hit.transform.GetComponent<Renderer>().material.color = new Color32(0, 171, 255, 255);
        }
        else if (hint.text != "" && char.IsDigit(hint.text[0]))
        {
            //при нахождении курсора не над точкой сбрасываем выделение
            oldHit.transform.GetComponent<Renderer>().material = dotMat;
            //сбрасываем значение строки подсказок
            hint.text = "";
        }
    }

    private void Update()
    {
        //метод инструкций на каждый отрисованный кадр

        //-----------------------
        //обработка нажатий клавиш при вводе

        //построение графика при нажатии Enter
        if (Input.GetButtonDown("Submit") && Build.interactable) BuildMeshChart();
        //переключение полей ввода при нажатии Tab
        if (Input.GetKeyDown(KeyCode.Tab)) if (n_field.isFocused) probStep_field.Select(); else n_field.Select();
    }

    public void ActivateBuildButton()
    {
        //регулирование доступности кнопки построения графика

        //проверка корректности значений полей ввода
        short.TryParse(n_field.text, out Chart_class.n);
        float.TryParse(probStep_field.text, out Chart_class.probStep);
        if (Chart_class.n >= 1 && Chart_class.n <= 50 && Chart_class.probStep >= 0.1f && Chart_class.probStep <= 0.9f)
        {
            //отключение предупреждения об ограничении на значения параметров
            RangeWarning.SetActive(false);

            //включение возможности построения графика
            Build.interactable = true;
        }
        else
        {
            //включение предупреждения об ограничении на значения параметров
            RangeWarning.SetActive(true);

            //отключение возможности построения графика
            Build.interactable = false;
        }
    }

    public void BuildMeshChart()
    {
        //построение графика

        StartCoroutine(ChartBox.GetComponent<Chart_class>().CreateFullChart(distrChoosing.value));
        //установка света
        if (distrChoosing.value == 0)
        {
            theLight.transform.localPosition = new Vector3(0, 5, Chart_class.n * 0.25f - 12.5f);
            theLight.intensity = 6 + Chart_class.n / 12.8f;
            theLight.range = 10.78f + Chart_class.n / 8.1f;
        }
        else
        {
            theLight.transform.localPosition = new Vector3(0, 5, 50 * 0.25f - 12.5f);
            theLight.intensity = 6 + 50 / 12.8f;
            theLight.range = 10.78f + 50 / 8.1f;
        }
    }

    private IEnumerator DOresetRotation()
    {
        //параллельный метод сброса камеры

        //длительность работы метода в секундах
        float duration = 0;
        while (duration < 1)
        {
            //суммирование времени работы метода и времени кадра
            duration += Time.deltaTime;

            //анимация сброса камеры
            ChartBox.transform.rotation = Quaternion.Lerp(ChartBox.transform.rotation, new Quaternion(0, 0, 0, 1), 0.15f);
            //ждать конца текущего кадра в анимации
            yield return new WaitForFixedUpdate();
        }

        //завершение работы метода
        yield return null;
    }

    public void ResetRotation()
    {
        //вызов метода сброса камеры

        StartCoroutine(DOresetRotation());
    }

    public void OnDistrChoosing()
    {
        //выбор распределения

        if (distrChoosing.value == 0)
        {
            //биномиальное распределение

            //меняем подписи
            n_field.GetComponentInParent<Text>().text = "n";
            rangeWarning.text = rangeWarning.text.Replace('r', 'n');
        }
        else
        {
            //отрицательное биномиальное распределение

            n_field.GetComponentInParent<Text>().text = "r";
            rangeWarning.text = rangeWarning.text.Replace('n', 'r');
        }

        //построение графика, если он уже построен
        if (Build.interactable) BuildMeshChart();
    }

    public void GoFullScreen()
    {
        //переключение режима окна между развернутым и неразвернутым

        //максимальное разрешение экрана
        Resolution maxRes = Screen.resolutions[Screen.resolutions.Length - 1];

        if (Screen.fullScreen)
        {
            //в режиме неразвернутого окна стандартное разрешение
            Screen.fullScreen = false;
            Screen.SetResolution(750, 600, false);
        }
        else
        {
            //в режиме развернутого окна максимальное разрешение
            Screen.fullScreen = true;
            Screen.SetResolution(maxRes.width, maxRes.height, true);
        }
    }
}