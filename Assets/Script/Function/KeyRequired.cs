using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRequired : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private string keyName;
    [SerializeField] private List<string> keyNames;
    [SerializeField] private bool haveKey;

    public List<PlayerBehaviour.Key> DetectKey(List<PlayerBehaviour.Key> keyList)
    {
        //PlayerBehaviour player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();

        haveKey = false;

        if(keyName != "")
        {
            int indexOfKeyList = -1;

            foreach (var key in keyList)
            {
                if (key.key.Name == keyName)
                {
                    haveKey = true;
                    indexOfKeyList = keyList.IndexOf(key);
                }
            }
            if (haveKey)
            {
                keyList[indexOfKeyList].quantity--;
            }
        }
        if(keyNames.Count != 0)
        {
            List<int> indexOfKeyList = new(); 

            foreach(string keyName in keyNames)
            {
                foreach(var key in keyList)
                {
                    if(key.key.Name == keyName)
                    {
                        indexOfKeyList.Add(keyList.IndexOf(key));
                    }
                }
                if(indexOfKeyList.Count == keyNames.Count)
                {
                    haveKey = true;
                }
            }
            if (haveKey)
            {
                foreach(int index in indexOfKeyList)
                {
                    keyList[index].quantity--;
                }
            }
        }

        return keyList;
    }

    public bool HaveKey { get { return haveKey; } }
}
