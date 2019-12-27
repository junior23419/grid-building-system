using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollider : MonoBehaviour
{
    [SerializeField] Material[] materials;

    public bool activate = false;
    BuildingManager buildingManager;
    private void Start()
    {

        buildingManager = GameObject.FindGameObjectWithTag("BuildingManager").GetComponent<BuildingManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag =="Building" && activate)
        {
            transform.parent.GetComponent<Building>().SetDeployable(false);
            gameObject.GetComponent<Renderer>().material = materials[1];
            buildingManager.deployable = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building" && activate)
        {
            Debug.Log("deployable");
            transform.parent.GetComponent<Building>().SetDeployable(true);
            gameObject.GetComponent<Renderer>().material = materials[0];
            buildingManager.deployable = true;
        }
    }
}
