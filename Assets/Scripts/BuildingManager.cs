﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildingManager : MonoBehaviour
{

    public enum MODE { NONE, BUILDING,REMOVING,MOVING};
    MODE mode;
    [SerializeField] Text modeText;
    [SerializeField] GameObject[] previews;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject buildingsParent;
    [SerializeField] Material[] materials;
    int currentPreview = -1;
    [HideInInspector] public bool deployable = true;
    GameObject movingTarget;
    GameObject lastRayCastted;
    Material lastMaterial;
    Vector3 lastPos;
    Quaternion lastRotation;

    void Start()
    {
        mode = MODE.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    StartBuildAndShowPreview(0);
        //}
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    StopBuilding();
        //}
        //if(Input.GetKeyDown(KeyCode.R))
        //{
        //    SetModeRemove();
        //}

        if (mode == MODE.REMOVING || mode == MODE.MOVING) 
        {
            RayToBuilding();
        }
    }

    public void SetModeMove()
    {
        SwitchMode(MODE.MOVING);
    }

    public void SetModeRemove()
    {
        SwitchMode(MODE.REMOVING);
    }


    public void StartBuildAndShowPreview(int itemIndex)
    {
        SwitchMode(MODE.BUILDING);
        currentPreview = itemIndex;
        previews[itemIndex].SetActive(true);
    }

    public void SetUpFinishBuilding()
    {
        GameObject building = Instantiate(prefabs[currentPreview], buildingsParent.transform);
        building.transform.position = previews[currentPreview].transform.position;
        building.transform.rotation = previews[currentPreview].transform.rotation;
        SwitchMode(MODE.NONE);
    }

    public void RayToBuilding()
    {
        RaycastHit hit;
#if UNITY_EDITOR_WIN
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        int layerMask = 1 << 8 ;
        
#if UNITY_EDITOR_WIN
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Building")
#elif UNITY_IOS || UNITY_ANDROID
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Building" && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {

            if (lastRayCastted == null || !GameObject.ReferenceEquals(lastRayCastted, hit.collider.gameObject))
            {
                if (lastRayCastted != null)
                {
                    ClearRemovingModeSetup();
                }
                lastRayCastted = hit.collider.gameObject;
                //set color of both;
                lastMaterial = hit.transform.gameObject.GetComponent<Renderer>().material;
                Debug.Log("parent" + lastRayCastted.transform.parent.name);
                foreach (Transform child in lastRayCastted.transform.parent)
                {
                    if(child.tag == "Building")
                        child.gameObject.GetComponent<Renderer>().material = materials[mode == MODE.REMOVING ? 1 : 0];
                }
                //hit.transform.gameObject.GetComponent<Renderer>().material = materials[mode == MODE.REMOVING ? 1 : 0];
                if(mode == MODE.MOVING)
                {
                    lastPos = hit.transform.parent.position;
                    lastRotation = hit.transform.parent.rotation;

                    hit.transform.parent.gameObject.GetComponent<Building>().isMoving = true;
                    //hit.transform.GetComponent<BuildingCollider>().activate = true;
                    foreach (Transform child in lastRayCastted.transform.parent)
                    {
                        //child.GetComponent<Renderer>().material = lastMaterial;
                        if (child.tag == "Building")
                            child.gameObject.GetComponent<BuildingCollider>().activate = true;
                    }
                }
                //Set last material;
            }
            
            
            
            
        }
    }
    
    private void ClearRemovingModeSetup()
    {
        if(!deployable)
        {
            lastRayCastted.transform.parent.position = lastPos;
            lastRayCastted.transform.parent.rotation = lastRotation;

        }
        
        //lastRayCastted.GetComponent<Renderer>().material = lastMaterial;
        lastRayCastted.transform.parent.GetComponent<Building>().isMoving = false;
        //lastRayCastted.transform.GetComponent<BuildingCollider>().activate = false;

        foreach (Transform child in lastRayCastted.transform.parent)
        {
            if (child.tag == "Building")
            {
                child.GetComponent<Renderer>().material = lastMaterial;
                child.GetComponent<BuildingCollider>().activate = false;
            }
                
        }
        lastRayCastted = null;
    }

    public void Deploy()
    {
        if(mode == MODE.BUILDING)
        {
            previews[currentPreview].GetComponent<Building>().Deploy();
        }
        else if(mode == MODE.MOVING)
        {

            ClearRemovingModeSetup();

        }
        else if(mode == MODE.REMOVING)
        {
            Destroy(lastRayCastted.transform.parent.gameObject);
            lastRayCastted = null;
        }
        SwitchMode(MODE.NONE);
        deployable = true;
    }

    public void Rotate()
    {
        if (mode == MODE.BUILDING)
        {
            previews[currentPreview].GetComponent<Building>().Rotate();
        }
        else if(mode == MODE.MOVING)
        {
            lastRayCastted.transform.parent.GetComponent<Building>().Rotate();
        }
    }

    public void Remove()
    {
        if (lastRayCastted)
            Destroy(lastRayCastted);

        lastRayCastted = null;
    }

    public void SetModeNone()
    {
        SwitchMode(MODE.NONE);
    }

    public void SwitchMode(MODE value)
    {
        if(lastRayCastted)
        {
            ClearRemovingModeSetup();
            
        }

        if (currentPreview != -1)
            previews[currentPreview].SetActive(false);


        mode = value;

        modeText.text = mode.ToString();
    }


}
