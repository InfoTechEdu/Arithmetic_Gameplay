using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] GameObject aim;

    List<Target> targets = new List<Target>();

    private Target activeTarget = null;

    public Target ActiveTarget { get => activeTarget; set => activeTarget = value; }

    private void Awake()
    {
        disableAim();
    }

    public void onTargetInitialized(Target t)
    {
        targets.Add(t);
        if (activeTarget == null)
            next();
    }
    public void onActiveTargetShooted(string param)
    {
        shootTarget(activeTarget, param);
        //targets.Remove(activeTarget);

        next();
        //onTargetCrashed(activeTarget);
    }
    public void onTargetCrashed(Target t)
    {
        targets.Remove(t);

        if (t.Equals(activeTarget))
        {
            activeTarget = null;
            next();
        }
    }

    private void disableAim()
    {
        activeTarget = null;

        aim.transform.SetParent(null);
        aim.SetActive(false);
    }

    private void setAimToTarget()
    {
        aim.SetActive(true);

        aim.transform.SetParent(activeTarget.transform);
        aim.transform.SetAsLastSibling();
        aim.transform.localPosition = Vector3.zero;

        SpriteRenderer parentSpriteRend = activeTarget.GetComponent<SpriteRenderer>();
        aim.GetComponent<SpriteRenderer>().size = 
           new Vector2(parentSpriteRend.bounds.size.x + 1, parentSpriteRend.bounds.size.x + 1);

        aimTarget(activeTarget);
    }

    public void next()
    {
        if (targets.Count == 0)
        {
            activeTarget = null;
            disableAim();
            return;
        }
            

        if (targets.Count == 1)
        {
            if (activeTarget == null)
            {
                activeTarget = targets[0];
                
            }
            else
            {
                return;
            }
        }
            

        if (targets.Count > 1)
        {
            if (targets.IndexOf(activeTarget) == targets.Count - 1) //if in the last position
            {
                activeTarget = targets[0];
            }
            else
            {
                activeTarget = targets[targets.IndexOf(activeTarget) + 1];
            }

        }
        
        setAimToTarget();
    }

    public void last()
    {
        if (targets.Count == 0)
            disableAim();

        if (targets.Count == 1)
            return;

        if(targets.Count > 1)
        {
            if (targets.IndexOf(activeTarget) == 0)
            {
                return;
            }
            else
            {
                activeTarget = targets[targets.IndexOf(activeTarget) - 1];
            }
        }

        setAimToTarget();
    }

    private void aimTarget(Target t)
    {
        MonoBehaviour[] list = t.gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is TargetBehaviour)
            {
                TargetBehaviour targetBehaviour = (TargetBehaviour)mb;
                targetBehaviour.OnTargeted();
            }
        }
    }
    private void shootTarget(Target t, string param)
    {
        MonoBehaviour[] list = t.gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is TargetBehaviour)
            {
                TargetBehaviour targetBehaviour = (TargetBehaviour)mb;
                targetBehaviour.OnShooted(param);
            }
        }
    }



}
