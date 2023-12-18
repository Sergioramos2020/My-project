using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Runtime.Serialization;

public class ServerManager : MonoBehaviour
{
    //creamos el campos para asignar el link del archivo json que es descargado
    [SerializeField] private string jsonURL;
    [SerializeField] private ItemButtonManager itemButtonManager;
    [SerializeField] private GameObject buttonsContainer;

    [Serializable]
    public struct Items
    {
        [Serializable]
        public struct Item
        {
            public string Name;
            public string Description;
            public string URLBundleModel;
            public string URLImageModel;
        }

        public Item[] items;
    }

    public Items newItemsCollection = new Items();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetJsonData());
        GameManager.instance.OnItemsMenu += CreateButtons;
    }

    private void CreateButtons()
    {
        foreach (var item in newItemsCollection.items)
        {
            ItemButtonManager itemButton;
            itemButton = Instantiate(itemButtonManager, buttonsContainer.transform);
            itemButton.name = item.Name;
            itemButton.ItemName = item.Name;
            itemButton.ItemDescription = item.Description;
            itemButton.URLBundleModel = item.URLBundleModel;
            StartCoroutine(GetBundleImage(item.URLImageModel, itemButton));
        }

        GameManager.instance.OnItemsMenu -= CreateButtons;
    }



    //funcion que descargara el archivo json y lo asignara a la estructura que se ha creado
    IEnumerator GetJsonData()
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(jsonURL);
        yield return serverRequest.SendWebRequest();
        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            newItemsCollection = JsonUtility.FromJson<Items>(serverRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error :c");
        }
    }

    IEnumerator GetBundleImage(string urlImage, ItemButtonManager button)
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(urlImage);
        serverRequest.downloadHandler = new DownloadHandlerTexture();
        yield return serverRequest.SendWebRequest();
        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            button.ImageBundle.texture = ((DownloadHandlerTexture)serverRequest.downloadHandler).texture;
        }
        else
        {
            Debug.Log("Error :c");
        }
    }
}
