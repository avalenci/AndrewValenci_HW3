using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    [Header("Set in Inspector")]

    public GameObject   prefabProjectile;
    public GameObject   line;
    public float        velocityMult = 8f;

    [Header("Set Dynamically")]

    public LineRenderer lineRenderer;
    public GameObject   launchPoint;
    public Vector3      launchPos;
    public GameObject   projectile;
    public bool         aimingMode;
    public Rigidbody projectileRigidbody;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    private void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);

        line = Instantiate<GameObject>(line);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        launchPos = launchPointTrans.position;
    }
    private void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        aimingMode = true;

        projectile = Instantiate(prefabProjectile) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, GameObject.Find("FrontPeg").transform.position);
        lineRenderer.SetPosition(1, projPos);
        lineRenderer.SetPosition(2, GameObject.Find("BackPeg").transform.position);
        lineRenderer.enabled = true;

        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();

            ProjectileLine.S.poi = projectile;
        }
    }
}
