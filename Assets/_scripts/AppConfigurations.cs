using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConfigurations : MonoBehaviour
{
    public static string GameName = "arithmetic";

    public bool RunInBackground = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = RunInBackground;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
