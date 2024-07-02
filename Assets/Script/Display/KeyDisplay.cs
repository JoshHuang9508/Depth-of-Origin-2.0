using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyDisplay : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject keysDisplayModel;

    //Runtime data
    private PlayerBehaviour player;
    private List<GameObject> keysDisplayList = new();


    private void Update()
    {
        //Get player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>();
        }
        catch
        {
            Debug.LogWarning("Can't find player (sent by SceneLoader.cs)");
        }

        if(player != null)
        {
            SetDisplay();
        }
    }

    private void SetDisplay()
    {
        List<Key> keysList = player.keyList;

        if (keysDisplayList.Count < keysList.Count)
        {
            GameObject item = Instantiate(
                keysDisplayModel,
                transform.position,
                Quaternion.identity,
                transform);
            keysDisplayList.Add(item);
        }

        else if (keysDisplayList.Count > keysList.Count)
        {
            var Item = keysDisplayList[^1];
            keysDisplayList.Remove(Item);
            Destroy(Item);
        }

        foreach (Key keysItem in keysList)
        {
            int indexOfKeyslist = keysList.IndexOf(keysItem);
            foreach (GameObject keysDisplayerItem in keysDisplayList)
            {
                int indexOfKeysDisplayList = keysDisplayList.IndexOf(keysDisplayerItem);

                if (indexOfKeysDisplayList == indexOfKeyslist)
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
