using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTextManager : Bestagon.Behaviours.ProtectedSceneSingleton<DebugTextManager>
{
    protected override void Destroy()
    {
        //throw new System.NotImplementedException();
    }

    Dictionary<string, TMPro.TMP_Text> textOBjs = new Dictionary<string, TMPro.TMP_Text>();


    public static void Draw(string key, Vector3 position, string value)
    {
        if (Instance.textOBjs.ContainsKey(key) == false)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("DebugText"));
            Instance.textOBjs.Add(key, obj.GetComponent<TMPro.TMP_Text>());
        }
        Instance.textOBjs[key].gameObject.transform.position = position;
        Instance.textOBjs[key].text = value;
    }

    public static void Delete(string key)
    {
        if (Instance.textOBjs.ContainsKey(key))
        {
            Destroy(Instance.textOBjs[key].gameObject);
            Instance.textOBjs.Remove(key);
        }
    }

    
}
