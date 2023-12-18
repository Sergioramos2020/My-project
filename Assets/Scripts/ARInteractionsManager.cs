using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARInteractionsManager : MonoBehaviour
{
    [SerializeField] private Camera aRCamera;
    private ARRaycastManager aRRaycastManager;
     private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject aRPointer;
    private GameObject item3DModel;
    private GameObject itemSelected;

    private bool isInitialPosition;
    private bool isOverUI;
    private bool isOver3DModel;

    private Vector2 initialTouchPos;

    public GameObject Item3DModel 
    { 
        set
        {
            //asignamos el modelo 3d
            item3DModel = value;
            
            //despues de asignado se tomara la posicion del pointer
            item3DModel.transform.position = aRPointer.transform.position;

            //el modelo 3d sera un hijo del pointer para poder desplazar
            item3DModel.transform.parent = aRPointer.transform;
            isInitialPosition = true;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        aRPointer = transform.GetChild(0).gameObject;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        //funcion que permite fijar el modelo 3d
        GameManager.instance.OnMainMenu += SetItemPosition;
    }

   
    // Update is called once per frame
    void Update()
    {
        if (isInitialPosition)
        {
            Vector2 middlePointScreen = new Vector2(Screen.width / 2 , Screen.height / 2); // se define la mitad de la pantalla
            aRRaycastManager.Raycast(middlePointScreen, hits, TrackableType.Planes);

            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;
                aRPointer.SetActive(true);
                isInitialPosition = false;
            }
        }

        //verifico que se haya tocado la pantalla
        if (Input.touchCount > 0) 
        { 
            Touch touchOne = Input.GetTouch(0);

            //verificar que el touch no haya sido en ningun boton de la interfaz con el fin de evitar errores
            if (touchOne.phase == TouchPhase.Began)
            {
                var touchPosition = touchOne.position;
                isOverUI = isTapOverUI(touchPosition);
                isOver3DModel = isTapOver3DModel(touchPosition);
            }

            if(touchOne.phase == TouchPhase.Moved)
            {
                if(aRRaycastManager.Raycast(touchOne.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    if (!isOverUI && isOver3DModel)
                    {
                        transform.position = hitPose.position;
                    }
                }
            }

            //rotar modelos 3d
            if (Input.touchCount == 2)
            {
                Touch touchTwo = Input.GetTouch(1);

                //verifico que uno de los touch ha iniciado
                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
                {
                    initialTouchPos = touchTwo.position - touchOne.position;
                }

                //verifico si alguno de los touch se ha movido, si el usuario ha movido los dedos
                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPos = touchTwo.position - touchOne.position;
                    //calculamos el angulo entre las posiciones inicial y actual con SignedAngle
                    float angle = Vector2.SignedAngle(initialTouchPos, currentTouchPos);

                    //aplicamos esa rotacion al modelo 3d
                    item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.eulerAngles.y - angle, 0);
                    initialTouchPos = currentTouchPos;
                }
            }

            //verificamos que el touch haya sido sobre el modelo 3d
            if (isOver3DModel && item3DModel == null && !isOverUI)
            {
                GameManager.instance.ARPosition();
                item3DModel = itemSelected;
                itemSelected = null;
                aRPointer.SetActive(true);
                transform.position = item3DModel.transform.position;
                item3DModel.transform.parent = aRPointer.transform;
            }

        }
    }

    private bool isTapOver3DModel(Vector2 touchPosition)
    {
        Ray ray = aRCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit3DModel))
        {
            if (hit3DModel.collider.CompareTag("Item"))
            {
                itemSelected = hit3DModel.transform.gameObject;
                return true;
            }
        }
        return false;
    }

    private bool isTapOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x , touchPosition.y);

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);
        
        return result.Count > 0;
        
    }

    //verifico que haya sido asignado un modelo 3d
    private void SetItemPosition()
    {
        if (item3DModel != null)
        {
            //como el modelo ha sido fijado ya no debe ser hijo del pointer
            item3DModel.transform.parent = null;
            
            //desactivo el pointer
            aRPointer.SetActive(false);
            item3DModel = null;
        }


    }

    //funcion para el caso en que quiera eliminar el modelo 3d
    public void DeleteItem()
    {
        Destroy(item3DModel);
        aRPointer.SetActive(false);//desactivo el pointer porque no hay modelo 3d
        GameManager.instance.MainMenu();
    }


}
