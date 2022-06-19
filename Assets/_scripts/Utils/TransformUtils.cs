using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class TransformUtils
    {
        internal static Transform FindDeepChild(this Transform aParent, string aName)
        {
            if (aParent == null) return null;
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}

