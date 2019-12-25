using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    Vector3 truePos;
    bool isDeployable = true;
    Camera camera;
    BuildingManager buildingManager;
    public bool isMoving = false;
    // Start is called before the first frame update
    private void Start()
    {
        this.camera = Camera.main;
        buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
    }

    private void Update()
    {
        if(isMoving)
        {
            RaycastHit hit;
#if UNITY_EDITOR_WIN
            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
#elif UNITY_IOS || UNITY_ANDROID
        Ray ray = this.camera.ScreenPointToRay(Input.GetTouch(0).position);
#endif
            //Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 9;

#if UNITY_EDITOR_WIN
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Plane")
#elif UNITY_IOS || UNITY_ANDROID
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Plane" && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
            {
                transform.position = hit.point + Vector3.up;
            }
        }
        

    }

    private void LateUpdate()
    {
        SnapIntoGrid();
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.up, 90);
    }

    private void SnapIntoGrid()
    {
        truePos.x = Mathf.RoundToInt(transform.position.x);
        truePos.y = Mathf.RoundToInt(transform.position.y);
        truePos.z = Mathf.RoundToInt(transform.position.z);
        //truePos.x = Mathf.RoundToInt(transform.position.x / CustomGrid.gridSize) * CustomGrid.gridSize;
        //truePos.y = Mathf.RoundToInt(transform.position.y / CustomGrid.gridSize) * CustomGrid.gridSize;
        //truePos.z = Mathf.RoundToInt(transform.position.z / CustomGrid.gridSize) * CustomGrid.gridSize;

        transform.position = truePos;
    }

    public void SetDeployable(bool val)
    {
        isDeployable = val;
    }

    public void Deploy()
    {
        if(isDeployable)
            buildingManager.SetUpFinishBuilding();
    }


}
