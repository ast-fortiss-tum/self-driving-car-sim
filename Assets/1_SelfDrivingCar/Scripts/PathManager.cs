using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System.Globalization;
using System.Collections.Generic;
using System.Collections;

public class PathManager : MonoBehaviour
{
    public CarPath carPath;

    [Header("Path type")]
    public bool doMakeRandomPath = false;
    public bool doLoadPointPath = true;

    [Header("Path making")]
    public Transform startPos;
    public string pathType = "point_path";
    public string trackString = "125.0,1.90000000,25.0@124.97380186765409,1.90000000,26.328851380637243@124.9010292778043,1.90000000,27.68092392919095@124.79041494123265,1.90000000,29.051863676676824@124.65069156872113,1.90000000,30.437316654110482@124.49059187105162,1.90000000,31.832928892507628@124.31884855900617,1.90000000,33.234346422883874@124.14419434336673,1.90000000,34.63721527625491@123.97536193491527,1.90000000,36.03718148363639@123.82108404443377,1.90000000,37.429891076043965@123.69009338270419,1.90000000,38.81099008449329@123.59112266050849,1.90000000,40.17612454000003@123.53290458862867,1.90000000,41.520940473579856@123.52417187784671,1.90000000,42.8410839162484@123.57365723894452,1.90000000,44.13220089902134@123.69009338270419,1.90000000,45.38993745291434@123.88221301990754,1.90000000,46.60993960894304@124.15874886133668,1.90000000,47.78785339812312@124.52843361777352,1.90000000,48.91932485147023@125.0,1.90000000,50.0@125.59424845021107,1.90000000,51.01589259086075@126.31700721066849,1.90000000,51.96128468692697@127.15455069225474,1.90000000,52.84422479280378@128.09315330585218,1.90000000,53.6727614130962@129.11908946234314,1.90000000,54.4549430524093@130.21863357261014,1.90000000,55.19881821534817@131.37806004753548,1.90000000,55.91243540651785@132.58364329800153,1.90000000,56.603843130523416@133.82165773489078,1.90000000,57.28108989196994@135.07837776908556,1.90000000,57.952224195462456@136.34007781146823,1.90000000,58.62529454560604@137.59303227292122,1.90000000,59.30834944700578@138.82351556432693,1.90000000,60.009437404266734@140.01780209656778,1.90000000,60.73660692199392@141.16216628052612,1.90000000,61.49790650479246@142.24288252708433,1.90000000,62.30138465726738@143.24622524712487,1.90000000,63.15508988402378@144.15846885153007,1.90000000,64.0670706896667@144.96588775118232,1.90000000,65.04537557880121@145.72828204227014,1.90000000,66.10409172791825@146.50843027559787,1.90000000,67.24463686142325@147.29707454397212,1.90000000,68.45605530579311@148.0849569401992,1.90000000,69.72739138750467@148.8628195570856,1.90000000,71.04768943303482@149.62140448743773,1.90000000,72.40599376886038@150.351453824062,1.90000000,73.79134872145823@151.0437096597649,1.90000000,75.19279861730524@151.68891408735286,1.90000000,76.5993877828783@152.27780919963226,1.90000000,78.00016054465421@152.8011370894095,1.90000000,79.38416122910985@153.24963984949113,1.90000000,80.74043416272207@153.61405957268352,1.90000000,82.05802367196779@153.8851383517931,1.90000000,83.32597408332381@154.05361827962628,1.90000000,84.533329723267@154.1102414489895,1.90000000,85.66913491827427@154.04574995268933,1.90000000,86.72243399482244@153.85088588353202,1.90000000,87.68227127938836@153.51639133432406,1.90000000,88.53769109844892@153.0088643542039,1.90000000,89.3280538609749@152.31329798250846,1.90000000,90.09695772034138@151.44802679734227,1.90000000,90.83980361963165@150.43138537680971,1.90000000,91.55199250192898@149.28170829901518,1.90000000,92.22892531031668@148.0173301420632,1.90000000,92.86600298787805@146.65658548405813,1.90000000,93.45862647769633@145.21780890310436,1.90000000,94.00219672285485@143.71933497730637,1.90000000,94.49211466643689@142.17949828476856,1.90000000,94.92378125152572@140.61663340359536,1.90000000,95.29259742120465@139.04907491189115,1.90000000,95.59396411855695@137.4951573877604,1.90000000,95.82328228666591@135.97321540930753,1.90000000,95.97595286861484@134.50158355463697,1.90000000,96.04737680748703@133.0985964018531,1.90000000,96.03295504636573@131.7825885290604,1.90000000,95.92808852833424@130.5718945143632,1.90000000,95.72817819647588@129.48484893586607,1.90000000,95.4286249938739@128.4756387626331,1.90000000,95.010599888819@127.48690176692385,1.90000000,94.46490881299408@126.51964873163571,1.90000000,93.80140515457086@125.574890439666,1.90000000,93.02994230172092@124.65363767391219,1.90000000,92.16037364261602@123.75690121727162,1.90000000,91.20255256542775@122.8856918526416,1.90000000,90.16633245832776@122.0410203629196,1.90000000,89.06156670948775@121.22389753100299,1.90000000,87.89810870707939@120.43533413978906,1.90000000,86.68581183927428@119.67634097217524,1.90000000,85.43452949424412@118.94792881105894,1.90000000,84.15411506016058@118.25110843933751,1.90000000,82.8544219251953@117.58689063990829,1.90000000,81.54530347751994@116.95628619566872,1.90000000,80.23661310530618@116.36030588951618,1.90000000,78.93820419672566@115.799960504348,1.90000000,77.65993013995003@115.27626082306156,1.90000000,76.41164432315102@114.79021762855425,1.90000000,75.20320013450021@114.35418731024896,1.90000000,74.00565072652596@113.97658242833691,1.90000000,72.78480379484304@113.65249802186811,1.90000000,71.54264454225307@113.37702912989252,1.90000000,70.28115817155748@113.1452707914601,1.90000000,69.00232988555777@112.9523180456209,1.90000000,67.70814488705544@112.79326593142477,1.90000000,66.400588378852@112.66320948792176,1.90000000,65.08164556374892@112.55724375416183,1.90000000,63.753301644547705@112.47046376919496,1.90000000,62.41754182404985@112.39796457207106,1.90000000,61.076351305056846@112.33484120184019,1.90000000,59.73171529037019@112.2761886975523,1.90000000,58.38561898279141@112.2171020982573,1.90000000,57.040047585121954@112.15267644300526,1.90000000,55.69698630016333@112.07800677084606,1.90000000,54.35842033071705@111.98818812082976,1.90000000,53.0263348795846@111.87831553200627,1.90000000,51.70271514956748@111.74348404342557,1.90000000,50.38954634346716";
    public int smoothPathIter = 0;
    public GameObject locationMarkerPrefab;
    public int markerEveryN = 2;
    public bool doChangeLanes = false;
    public bool invertNodes = false;

    [Header("Random path parameters")]
    public int numSpans = 100;
    public float turnInc = 1f;
    public float spanDist = 5f;
    public bool sameRandomPath = true;
    public int randSeed = 2;

    [Header("Aux")]
    public GameObject[] initAfterCarPathLoaded; // Scripts using the IWaitCarPath interface to init after loading the CarPath

    Vector3 span = Vector3.zero;
    GameObject generated_mesh;

    void Awake()
    {
        if (sameRandomPath)
            Random.InitState(randSeed);

        InitCarPath();
    }

    public void InitCarPath(string trackString)
    {
        MakePointPath(trackString);
        if (carPath == null) // if no carPath was created, skip the following block of code
        {
            return;
        }

        if (invertNodes)
        {
            CarPath new_carPath = new CarPath();
            for (int i = carPath.nodes.Count - 1; i >= 0; i--)
            {
                PathNode node = carPath.nodes[i];
                new_carPath.nodes.Add(node);
                new_carPath.centerNodes.Add(node);
            }
            carPath = new_carPath;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(InitAfterCarPathLoaded(initAfterCarPathLoaded));
    }

    public void InitCarPath()
    {
        if (doMakeRandomPath)
        {
            MakeRandomPath();
        }
        else if (doLoadPointPath)
        {
            MakePointPath();
        }

        if (carPath == null) // if no carPath was created, skip the following block of code
        {
            return;
        }

        if (invertNodes)
        {
            CarPath new_carPath = new CarPath();
            for (int i = carPath.nodes.Count - 1; i >= 0; i--)
            {
                PathNode node = carPath.nodes[i];
                new_carPath.nodes.Add(node);
                new_carPath.centerNodes.Add(node);
            }
            carPath = new_carPath;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(InitAfterCarPathLoaded(initAfterCarPathLoaded));

    }

    public Vector3 GetPathStart()
    {
        return startPos.position;
    }

    public Vector3 GetPathEnd()
    {
        int iN = carPath.nodes.Count - 1;

        if (iN < 0)
            return GetPathStart();

        return carPath.nodes[iN].pos;
    }

    float nfmod(float a, float b) // formula for negative and positive modulo
    {
        return a - b * Mathf.Floor(a / b);
    }


    void buildCarPathMakePointPath(string[] lines)
    {
        Vector3 np = Vector3.zero;

        float offsetY = -0.2f; // It is for the car I guess, such that the car is a bit higher than the road
        List<Vector3> points = new List<Vector3>();

        foreach (string line in lines)
        {
            string[] tokens = line.Split(',');

            if (tokens.Length != 3)
                continue;
            np.x = float.Parse(tokens[0], CultureInfo.InvariantCulture.NumberFormat);
            np.y = float.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat) + offsetY;
            np.z = float.Parse(tokens[2], CultureInfo.InvariantCulture.NumberFormat);

            points.Add(np);
        }

        while (smoothPathIter > 0)
        {
            points = Chaikin(points);
            smoothPathIter--;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[(int)nfmod(i, (points.Count))];
            Vector3 previous_point = points[(int)nfmod(i - 1, (points.Count))];
            Vector3 next_point = points[(int)nfmod(i + 1, (points.Count))];

            PathNode p = new PathNode();
            p.pos = point;
            p.rotation = Quaternion.LookRotation(next_point - previous_point, Vector3.up);
            carPath.nodes.Add(p);
            carPath.centerNodes.Add(p);
        }
    }

    void MakePointPath(string trackString)
    {
        string[] lines;
        if (!trackString.Equals("none"))
        {
            this.trackString = trackString;
            lines = trackString.Split('@');
        }
        else
        {
            throw new PlayerPrefsException("Track string must not be none");
        }

        carPath = new CarPath("point_path");

        buildCarPathMakePointPath(lines);
    }

    void MakePointPath()
    {
        string[] lines;
        if (!trackString.Equals("none") && pathType.Equals("point_path"))
        {
            lines = trackString.Split('@');
            carPath = new CarPath("point_path");
        }
        else
        {
            throw new PlayerPrefsException("Track string must not be none");
        }

        buildCarPathMakePointPath(lines);
    }

    public List<Vector3> Chaikin(List<Vector3> pts)
    {
        List<Vector3> newPts = new List<Vector3>();

        newPts.Add(pts[0]);

        for (int i = 0; i < pts.Count - 2; i++)
        {
            newPts.Add(pts[i] + (pts[i + 1] - pts[i]) * 0.75f);
            newPts.Add(pts[i + 1] + (pts[i + 2] - pts[i + 1]) * 0.25f);
        }

        //newPts.Add(pts[pts.Count - 1]);
        return newPts;
    }

    void MakeRandomPath()
    {
        carPath = new CarPath();

        Vector3 s = startPos.position;
        float turn = 0f;
        s.y = 0.5f;

        span.x = 0f;
        span.y = 0f;
        span.z = spanDist;

        List<Vector3> points = new List<Vector3>();

        for (int iS = 0; iS < numSpans; iS++)
        {
            Vector3 np = s;
            points.Add(np);

            float t = UnityEngine.Random.Range(-1.0f * turnInc, turnInc);

            turn += t;

            Quaternion rot = Quaternion.Euler(0.0f, turn, 0f);
            span = rot * span.normalized;

            if (SegmentCrossesPath(np + (span.normalized * 100.0f), 90.0f, points.ToArray()))
            {
                //turn in the opposite direction if we think we are going to run over the path
                turn *= -0.5f;
                rot = Quaternion.Euler(0.0f, turn, 0f);
                span = rot * span.normalized;
            }

            span *= spanDist;

            s = s + span;
        }

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[(int)nfmod(i, (points.Count))];
            Vector3 previous_point = points[(int)nfmod(i - 1, (points.Count))];
            Vector3 next_point = points[(int)nfmod(i + 1, (points.Count))];

            PathNode p = new PathNode();
            p.pos = point;
            p.rotation = Quaternion.LookRotation(next_point - previous_point, Vector3.up); ;
            carPath.nodes.Add(p);
            carPath.centerNodes.Add(p);
        }

    }

    public bool SegmentCrossesPath(Vector3 posA, float rad, Vector3[] posN)
    {
        foreach (Vector3 pn in posN)
        {
            float d = (posA - pn).magnitude;

            if (d < rad)
                return true;
        }

        return false;
    }

    public IEnumerator InitAfterCarPathLoaded(GameObject[] scriptList)
    {
        if (carPath != null)
        {
            foreach (GameObject go in scriptList) // Init each Object that need a carPath
            {
                try
                {
                    IWaitCarPath script = go.GetComponent<IWaitCarPath>();
                    if (script != null)
                    {
                        script.Init();
                    }
                    else
                    {
                        Debug.LogError("Provided GameObject doesn't contain an IWaitCarPath script");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("Could not initialize: {0}, Exception: {1}", go.name, e));
                }
            }
        }

        else
        {
            Debug.LogError("No carPath loaded"); yield return null;
        }
        yield return null;
    }

}
