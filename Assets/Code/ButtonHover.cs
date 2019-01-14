using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler {

    public Sprite button, buttonOnHover;
    private Image thisObject;

    void Start(){
        thisObject = transform.GetComponent<Image>();
        if (thisObject == null)
            Debug.Log("broken " + this.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData){
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnSelect(BaseEventData eventData){
        if (buttonOnHover == null)
            return;

        if (thisObject == null)
            return;

        thisObject.sprite = buttonOnHover;
    }

    public void OnDeselect(BaseEventData eventData){
        if (button == null)
            return;
        if (thisObject == null)
            return;

        thisObject.sprite = button;
    }
}
