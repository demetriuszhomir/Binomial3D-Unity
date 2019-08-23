using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.Distributions;

public class Chart_class : MonoBehaviour
{
    //chart class

    //n parameter
    public static short n;
    //probability step
    public static float probStep = 0;
    public Material polyMeshMaterial;
    //chart slice object
    public Transform Charts;
    public GameObject dotPrefab;
    public Animator fadedScreen;

    GameObject polyMesh;
    private int distr;
    //reserved n parameter
    private short n_r;

    private float[] Calculate(float pi)
    {
        //calculating the coordinates of the slice points

        //points array
        double[] y;
        if (distr == 0)
        {
            //binomial

            y = new double[n + 1];
            for (short x = 0; x <= n; x++) y[x] = Binomial.PMF(pi, n, x);
        }
        else
        {
            //negative binomial

            n = n_r;

            y = new double[51];
            for (short x = 0; x <= 50; x++) y[x] = NegativeBinomial.PMF(n, pi, x);
            
            n = 50;
        }

        return y.Select(x => (float)Math.Round(x, 6)).ToArray();
    }

    private void Build_aChart(float pi, short num)
    {
        //building the slice

        GameObject chart = new GameObject("Chart" + num);
        chart.transform.parent = Charts;
        chart.tag = "Chart";
        chart.transform.localPosition = Vector3.zero;

        //points array
        float[] y = Calculate(pi);
        //points placement
        for (short x = 0; x <= n; x++)
        {
            GameObject dot = Instantiate(dotPrefab, chart.transform);
            dot.name = "Value" + x;
            dot.tag = "Value";
            dot.layer = 9;
            dot.transform.localPosition = new Vector3(pi * 10, y[x] * 10, x * 0.5f);
        }
    }

    private void BuildPolyMesh(short nMeshes)
    {
        //building poly mesh

        Vector3[] vertices = GameObject.FindGameObjectsWithTag("Value").Select(x => x.transform.position).ToArray();
        int[] indices = new int[nMeshes * ((n + 1) * 2 - 2) * 3];
        //vertices numbering direction
        short c = -1;
        //vertices numbering
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

        Mesh theMesh = new Mesh();
        theMesh.vertices = vertices;
        theMesh.triangles = indices;
        theMesh.RecalculateNormals();
        theMesh.RecalculateBounds();

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
        //building the chart (async)

        //setting the chosen distribution
        this.distr = distr;
        //saving current n
        n_r = n;

        //fading the screen
        fadedScreen.Play("ScreenFading_in");
        yield return new WaitForSeconds(0.134f);

        //deleting previous chart
        foreach (Transform child in Charts) Destroy(child.gameObject);
        foreach (GameObject aPolyMesh in GameObject.FindGameObjectsWithTag("PolyMesh")) Destroy(aPolyMesh);
        //camera reset
        transform.rotation = new Quaternion(0, 0, 0, 1);
        yield return new WaitForEndOfFrame();

        //probability of a slice
        float pi = probStep;
        //amount of slices
        short nCharts = (short)Mathf.CeilToInt(1 / probStep);
        //building slices
        Build_aChart(0.02f, 0);
        for (short num = 1; num <= nCharts; num++)
        {
            Build_aChart(pi, num);

            //increasing  probability of the next slice by one step
            pi = (float)Math.Round(pi + probStep, 6);
            //last slice remains - value of its probability parameter must be 0.98
            if (pi > 0.9f) pi = 0.98f;
        }

        //building poly mesh
        BuildPolyMesh(nCharts);

        //returning n if distribution is negative
        n = n_r;

        //fading the screen out
        fadedScreen.Play("ScreenFading_out");
        
        yield return null;
    }
}