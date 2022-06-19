using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    TargetController targetController;
    DataController dataController;

    int tempId;

    public int TempId { get => tempId; set => tempId = value; }

    private void Start()
    {
        dataController = FindObjectOfType<DataController>();

        tempId = Random.Range(0, 1000); //delete. for comfortable debugging

        targetController = GameObject.Find("TargetController").GetComponent<TargetController>();
        targetController.onTargetInitialized(this);
    }

    //For tests
    //private void OnMouseDown()
    //{
    //    if(this.Equals(targetController.ActiveTarget))
    //        Destroy(gameObject);
    //}

    private void OnDestroy()
    {
        if (dataController == null)
            return;

        if (dataController.GameMode == GameMode.PC) //very bad code. Delete. Refactor!!
            return;

        Debug.LogWarning("Destroyed");
        targetController.onTargetCrashed(this);
    }


}
