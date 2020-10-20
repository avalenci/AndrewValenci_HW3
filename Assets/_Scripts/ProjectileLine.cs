using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour
{
    static public ProjectileLine S;

    [Header("Set in Inspector")]
    public float            minDist = 0.1f;
    public GameObject[] lines = new GameObject[4];

    private GameObject      _poi;
    private List<Vector3>   points;
    private int lineNumb;
    private bool firstShot;

    private void Awake()
    {
        S = this;

        firstShot = true;

        for (int i = 0; i < 4; i++)
        {
            lines[i] = Instantiate<GameObject>(lines[i]);
            Material mat = lines[i].GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = (float)(1.0 - (0.25 * i));
            mat.color = c;
            lines[i].GetComponent<Renderer>().material = mat;
        }

        lines[lineNumb].GetComponent<LineRenderer>().enabled = false;
        points = new List<Vector3>();
    }

    public GameObject poi
    {
        get
        {
            return _poi;
        }
        set
        {
            _poi = value;
            if (_poi != null)
            {
                NextShot();
            }
        }
    }

    public void Clear()
    {
        _poi = null;
        foreach (GameObject line in lines)
        {
            line.GetComponent<LineRenderer>().enabled = false;
        }
        lineNumb = 0;
        firstShot = true;
        points = new List<Vector3>();
    }

    public void NextShot()
    {
        if (lineNumb == 3)
        {
            Clear();
        } else if (firstShot)
        {
            firstShot = false;
        } else
        {
            lineNumb++;
        }
        points = new List<Vector3>();

        AddPoint(lines[lineNumb].GetComponent<LineRenderer>());
    }

    public void AddPoint(LineRenderer line)
    {
        Vector3 pt = _poi.transform.position;
        if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            return;
        }
        if (points.Count == 0)
        {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;

            points.Add(pt + launchPosDiff);
            points.Add(pt);

            line.positionCount = 2;

            line.SetPosition(0, points[0]);
            line.SetPosition(1, points[1]);

            line.enabled = true;
        } else
        {
            points.Add(pt);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, lastPoint);
            line.enabled = true;
        }
    }

    public Vector3 lastPoint
    {
        get
        {
            if (points == null)
            {
                return (Vector3.zero);
            }
            return points[points.Count - 1];
        }
    }

    private void FixedUpdate()
    {
        if (poi == null)
        {
            if (FollowCam.POI != null)
            {
                if (FollowCam.POI.tag == "Projectile")
                {
                    poi = FollowCam.POI;
                } else
                {
                    return;
                }
            } else
            {
                return;
            }
        }

        AddPoint(lines[lineNumb].GetComponent<LineRenderer>());
        if (FollowCam.POI == null)
        {
            poi = null;
        }
    }
}
