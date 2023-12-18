using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    private string itemName;
    private string itemDescription;
    private Sprite itemImage;
    // private double price;
    private GameObject item3DModel;
    private ARInteractionsManager interactionsManager;
    private string urlBundleModel;
    private RawImage imageBundle;

    public string ItemName
    {
        set
        {
            itemName = value;
        }
    }

    public string ItemDescription { set => itemDescription = value; }

    public Sprite ItemImage { set => itemImage = value; }

    //public double Price {  set => price = value; }

    public GameObject Item3DModel { set => item3DModel = value; }

    public string URLBundleModel { set => urlBundleModel = value; }

    public RawImage ImageBundle { get => imageBundle; set => imageBundle = value; }




    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = itemName;
        //transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;
        imageBundle = transform.GetChild(1).GetComponent<RawImage>();
        transform.GetChild(2).GetComponent<Text>().text = itemDescription;
        // transform.GetChild(3).GetComponent<Text>().text = price.ToString();

        var button = GetComponent<Button>();

        //Cuando eliga un objeto o item este llamara a ARPosition
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);

        //asignar el modelo 3d es decir esta es la funcion que crea el modelo 3d
        interactionsManager = FindObjectOfType<ARInteractionsManager>();
    }

    private void Create3DModel()
    {
        //creamo el modelo 3d elegido y lo asigne a ARInteractionsManager
        // interactionsManager.Item3DModel = Instantiate(item3DModel);
        StartCoroutine(DownloadAssetBundle(urlBundleModel));
    }

    IEnumerator DownloadAssetBundle(string urlAssetBundle)
    {
        UnityWebRequest serverRequest = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle);
        yield return serverRequest.SendWebRequest();
        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle model3D = DownloadHandlerAssetBundle.GetContent(serverRequest);
            if (model3D != null)
            {
                interactionsManager.Item3DModel = Instantiate(model3D.LoadAsset(model3D.GetAllAssetNames()[0]) as GameObject);
            }
            else
            {
                Debug.Log("Not a valid Assets Bundle");
            }
        }
        else
        {
            Debug.Log("Error x'c");
        }
    }
}
