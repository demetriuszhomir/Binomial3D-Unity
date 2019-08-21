using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.Distributions;

public class Chart_class : MonoBehaviour
{
    //класс графика

    //параметр n
    public static short n;
    //шаг вероятности
    public static float probStep = 0;
    //материал полигонального полотна
    public Material polyMeshMaterial;
    //объект срезов графика
    public Transform Charts;
    //шаблон точки
    public GameObject dotPrefab;
    //аниматор затухающего экрана
    public Animator fadedScreen;

    //полигональное полотно
    GameObject polyMesh;
    //выбранное распределение
    private int distr;
    //зарезервированный параметр n
    private short n_r;

    private float[] Calculate(float pi)
    {
        //функция вычисления координат точек среза

        //массив точек среза
        double[] y;
        if (distr == 0)
        {
            //биномиальное распределение

            y = new double[n + 1];
            for (short x = 0; x <= n; x++) y[x] = Binomial.PMF(pi, n, x);
        }
        else
        {
            //отрицательное биномиальное распределение

            n = n_r;

            y = new double[51];
            for (short x = 0; x <= 50; x++) y[x] = NegativeBinomial.PMF(n, pi, x);
            
            n = 50;
        }

        //возврат массива точек среза в качестве значения функции
        return y.Select(x => (float)Math.Round(x, 6)).ToArray();
    }

    private void Build_aChart(float pi, short num)
    {
        //построение среза

        //задание параметров среза как объекта
        GameObject chart = new GameObject("Chart" + num);
        chart.transform.parent = Charts;
        chart.tag = "Chart";
        chart.transform.localPosition = Vector3.zero;

        //задание массива точек среза
        float[] y = Calculate(pi);
        //размещение точек на срезе
        for (short x = 0; x <= n; x++)
        {
            //задание параметров точки как объекта
            GameObject dot = Instantiate(dotPrefab, chart.transform);
            dot.name = "Value" + x;
            dot.tag = "Value";
            dot.layer = 9;
            dot.transform.localPosition = new Vector3(pi * 10, y[x] * 10, x * 0.5f);
        }
    }

    private void BuildPolyMesh(short nMeshes)
    {
        //построение полигонального полотна

        //вершины полотна
        Vector3[] vertices = GameObject.FindGameObjectsWithTag("Value").Select(x => x.transform.position).ToArray();
        //массив номеров вершин
        int[] indices = new int[nMeshes * ((n + 1) * 2 - 2) * 3];
        //переменная направления нумерации вершин
        short c = -1;
        //нумерация вершин
        for (short i = 0; i < nMeshes; i++)
        {
            for (short j = 0; j < n; j++)
            {
                indices[++c] = j + (n + 1) * i;
                indices[++c] = j + (n + 1) * i + n + 2;
                indices[++c] = j + (n + 1) * i + n + 1;
                indices[++c] = j + (n + 1) * i;
                indices[++c] = j + (n + 1) * i + 1;
                indices[++c] = j + (n + 1) * i + n + 2;
            }
        }

        //задание параметров поверхности полотна
        Mesh theMesh = new Mesh();
        theMesh.vertices = vertices;
        theMesh.triangles = indices;
        theMesh.RecalculateNormals();
        theMesh.RecalculateBounds();

        //задание параметров полотна
        polyMesh = new GameObject("PolyMesh");
        polyMesh.transform.parent = gameObject.transform;
        polyMesh.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = polyMesh.AddComponent(typeof(MeshFilter)) as MeshFilter;
        polyMesh.GetComponent<Renderer>().material = polyMeshMaterial;
        polyMesh.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        polyMesh.GetComponent<Renderer>().receiveShadows = false;
        polyMesh.tag = "PolyMesh";
        polyMesh.layer = 8;
        filter.mesh = theMesh;
    }

    public IEnumerator CreateFullChart(int distr)
    {
        //параллельный метод построения графика

        //определение выбранного распределения
        this.distr = distr;
        //сохраняем значение n
        n_r = n;

        //запуск анимации затемнения экрана
        fadedScreen.Play("ScreenFading_in");
        //ожидание окончания анимации
        yield return new WaitForSeconds(0.134f);

        //удаление предыдущего графика
        foreach (Transform child in Charts) Destroy(child.gameObject);
        foreach (GameObject aPolyMesh in GameObject.FindGameObjectsWithTag("PolyMesh")) Destroy(aPolyMesh);
        //сброс камеры
        transform.rotation = new Quaternion(0, 0, 0, 1);
        //ожидание смены кадров после удаления
        yield return new WaitForEndOfFrame();

        //параметр вероятности среза
        float pi = probStep;
        //количество срезов
        short nCharts = (short)Mathf.CeilToInt(1 / probStep);
        //построение срезов графика
        Build_aChart(0.02f, 0);
        for (short num = 1; num <= nCharts; num++)
        {
            Build_aChart(pi, num);

            //увеличение параметра вероятности следующего среза на шаг
            pi = (float)Math.Round(pi + probStep, 6);
            //остался последний срез — значение его параметра вероятности равно 0,98
            if (pi > 0.9f) pi = 0.98f;
        }

        //строим полигональное полотно
        BuildPolyMesh(nCharts);

        //возавращаем значение n при отрицательном распределении
        n = n_r;

        //запуск анимации возврата экрана из затемненного состояния
        fadedScreen.Play("ScreenFading_out");

        //завершение работы метода
        yield return null;
    }
}