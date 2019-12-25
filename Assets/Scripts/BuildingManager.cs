using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildingManager : MonoBehaviour
{

    public enum MODE { NONE, BUILDING,REMOVING};
    MODE mode;
    [SerializeField] Text modeText;
    [SerializeField] GameObject[] previews;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject buildingsParent;
    [SerializeField] Material[] materials;
    int currentPreview = -1;


    GameObject lastRayCastted;
    Material lastMaterial;

    void Start()
    {

        mode = MODE.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartBuildAndShowPreview(0);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopBuilding();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            SetModeRemove();
        }

        if (mode == MODE.REMOVING) 
        {
            RayToBuilding();
        }
    }

    public void SetModeRemove()
    {
        mode = MODE.REMOVING;
        modeText.text = mode.ToString();
    }

    public void StopBuilding()
    {
        if(mode == MODE.BUILDING)
        {
            previews[currentPreview].SetActive(false);
        }
        else if(mode == MODE.REMOVING && lastRayCastted != null)
        {
            ClearRemovingModeSetup();
        }
        mode = MODE.NONE;
        modeText.text = mode.ToString();
    }

    public void StartBuildAndShowPreview(int itemIndex)
    {
        currentPreview = itemIndex;
        mode = MODE.BUILDING;
        modeText.text = mode.ToString();
        previews[itemIndex].SetActive(true);
    }

    public void SetUpFinishBuilding()
    {
        GameObject building = Instantiate(prefabs[currentPreview], buildingsParent.transform);
        building.transform.position = previews[currentPreview].transform.position;
        building.transform.rotation = previews[currentPreview].transform.rotation;
        StopBuilding();
    }

    public void RayToBuilding()
    {
        RaycastHit hit;
#if UNITY_EDITOR_WIN
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
        int layerMask = 1 << 8 ;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Building")
        {
            if (lastRayCastted == null || !GameObject.ReferenceEquals(lastRayCastted,hit.collider.gameObject))
            {
                if (lastRayCastted != null)
                {
                    lastRayCastted.GetComponent<Renderer>().material = lastMaterial;
                }
                //set color of both;
                lastMaterial = hit.transform.gameObject.GetComponent<Renderer>().material;
                hit.transform.gameObject.GetComponent<Renderer>().material = materials[0];

                //Set last material;
            }

            if ( Input.GetMouseButtonDown(0))
            {
                Destroy(hit.transform.parent.gameObject);
            }

            lastRayCastted = hit.collider.gameObject;

        }
        else // setcolor on ray exit object
        {
            if(lastRayCastted !=null)
            {
                ClearRemovingModeSetup();
            }
        }
    }
    
    private void ClearRemovingModeSetup()
    {
        lastRayCastted.GetComponent<Renderer>().material = lastMaterial;
        lastRayCastted = null;
    }

    public void Deploy()
    {
        if(mode == MODE.BUILDING)
        {
            previews[currentPreview].GetComponent<Building>().Deploy();
        }
    }

    public void Rotate()
    {
        if (mode == MODE.BUILDING)
        {
            previews[currentPreview].GetComponent<Building>().Rotate();
        }
    }
}
