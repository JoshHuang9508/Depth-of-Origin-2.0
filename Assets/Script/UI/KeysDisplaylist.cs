using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeysDisplaylist : MonoBehaviour
{
    [Header("Dynamic Data")]
    [SerializeField] private List<GameObject> keysDisplayList = new();

    [Header("Object Reference")]
    [SerializeField] private GameObject keysDisplayModel;
    [SerializeField] private PlayerBehaviour player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    private void Update()
    {
        List<PlayerBehaviour.Key> keysList = player.GetKeyList;

        if(keysDisplayList.Count < keysList.Count)
        {
            GameObject item = Instantiate(
                keysDisplayModel,
                transform.position,
                Quaternion.identity,
                transform);
            keysDisplayList.Add(item);
        }
        else if(keysDisplayList.Count > keysList.Count)
        {
            var Item = keysDisplayList[keysDisplayList.Count - 1];
            keysDisplayList.Remove(Item);
            Destroy(Item);
        }

        foreach(PlayerBehaviour.Key keysItem in keysList)
        {

            int indexOfKeyslist = keysList.IndexOf(keysItem);
            foreach(GameObject keysDisplayerItem in keysDisplayList)
            {
                int indexOfKeysDisplayList = keysDisplayList.IndexOf(keysDisplayerItem);

                if(indexOfKeysDisplayList == indexOfKeyslist)
                {
                    TextMeshProUGUI text = keysDisplayerItem.GetComponentInChildren<TextMeshProUGUI>();
                    Image image = keysDisplayerItem.GetComponentInChildren<Image>();

                    image.sprite = keysItem.key.Image;

                    text.text = keysItem.key.Name + "  " + keysItem.quantity;
                }
            }

        }
    }
}
