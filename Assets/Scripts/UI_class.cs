using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_class : MonoBehaviour
{
    //GUI class

    //chart building button
    public Button Build;
    public InputField n_field;
    public InputField probStep_field;
    //chart container box
    public GameObject ChartBox;
    //hint bar
    public Text hint;
    public Texture2D cursorDefault;
    public Light theLight;
    public GameObject RangeWarning;
    public Material dotMat;
    //range warning object's text component
    public Text rangeWarning;
    public Dropdown distrChoosing;

    RaycastHit oldHit;
    //mouse over UI flag
    bool overUI = false;

    private void Start()
    {
        //setting standard cursor

        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void ChangeOverUIbool(bool state)
    {
        //tracking cursor over UI

        overUI = state;

        //handling the camera's rotation capability
        if (overUI) ChartBox.GetComponent<Collider>().enabled = false; else ChartBox.GetComponent<Collider>().enabled = true;
    }

    private void FixedUpdate()
    {
        //displaying coordinates of a point on the chart

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!overUI && Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Values")))
        {
            //when ray hits a new point, change color of the previous one to standard
            if (oldHit.transform != null && hit.transform != oldHit.transform)
                oldHit.transform.GetComponent<Renderer>().material = dotMat;
            //save the point object as the last one
            oldHit = hit;

            //output coordinates
            hint.text = hit.transform.localPosition.z * 2f + "; " +
                Math.Round(hit.transform.localPosition.y / 10, 6).ToString("0.######") + "; " +
                Math.Round(hit.transform.localPosition.x / 10, 6).ToString("0.######");

            //highlight the point hit by ray
            hit.transform.GetComponent<Renderer>().material.color = new Color32(0, 171, 255, 255);
        }
        else if (hint.text != "" && char.IsDigit(hint.text[0]))
        {
            //when cursor is not over the point, reset selection
            oldHit.transform.GetComponent<Renderer>().material = dotMat;
            //reset hint
            hint.text = "";
        }
    }

    private void Update()
    {
        //keypress handling during input

        //drawing chart on Enter
        if (Input.GetButtonDown("Submit") && Build.interactable) BuildMeshChart();
        //Tab key handling
        if (Input.GetKeyDown(KeyCode.Tab)) if (n_field.isFocused) probStep_field.Select(); else n_field.Select();
    }

    public void ActivateBuildButton()
    {
        //build button availability handling

        //input check
        short.TryParse(n_field.text, out Chart_class.n);
        float.TryParse(probStep_field.text, out Chart_class.probStep);
        if (Chart_class.n >= 1 && Chart_class.n <= 50 && Chart_class.probStep >= 0.1f && Chart_class.probStep <= 0.9f)
        {
            RangeWarning.SetActive(false);

            Build.interactable = true;
        }
        else
        {
            RangeWarning.SetActive(true);

            Build.interactable = false;
        }
    }

    public void BuildMeshChart()
    {
        //building chart

        StartCoroutine(ChartBox.GetComponent<Chart_class>().CreateFullChart(distrChoosing.value));
        //light setup
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
        //reset rotation (async)

        //animation duration
        float duration = 0;
        while (duration < 1)
        {
            duration += Time.deltaTime;

            ChartBox.transform.rotation = Quaternion.Lerp(ChartBox.transform.rotation, new Quaternion(0, 0, 0, 1), 0.15f);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public void ResetRotation()
    {
        //calling coroutine of rotation reset

        StartCoroutine(DOresetRotation());
    }

    public void OnDistrChoosing()
    {
        //distribution choosing

        if (distrChoosing.value == 0)
        {
            //binomial

            //changing fields hints
            n_field.GetComponentInParent<Text>().text = "n";
            rangeWarning.text = rangeWarning.text.Replace('r', 'n');
        }
        else
        {
            //negative binomial

            n_field.GetComponentInParent<Text>().text = "r";
            rangeWarning.text = rangeWarning.text.Replace('n', 'r');
        }

        //rebuild chart
        if (Build.interactable) BuildMeshChart();
    }

    public void GoFullScreen()
    {
        //switching window modes

        Resolution maxRes = Screen.resolutions[Screen.resolutions.Length - 1];

        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            Screen.SetResolution(750, 600, false);
        }
        else
        {
            Screen.fullScreen = true;
            Screen.SetResolution(maxRes.width, maxRes.height, true);
        }
    }
}