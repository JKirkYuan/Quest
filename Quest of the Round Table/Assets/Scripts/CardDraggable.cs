﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
							IPointerEnterHandler, IPointerExitHandler {

	public Transform parentToReturnTo = null;
	public Transform placeholderParent = null;

	public GameObject placeholder = null;


	public void OnPointerEnter(PointerEventData eventData) {

		Debug.Log ("Pointer entered card.");

		//CardDraggable card = eventData.pointerDrag.GetComponent<CardDraggable>();


	}

	public void OnPointerExit(PointerEventData eventData) {

		Debug.Log ("Pointer exited card.");

		//CardDraggable card = eventData.pointerDrag.GetComponent<CardDraggable>();

	}


	public void OnBeginDrag(PointerEventData eventData) {
		Debug.Log ("Begin drag.");

		placeholder = new GameObject();
		placeholder.transform.SetParent(this.transform.parent);

		LayoutElement emptyLayout = placeholder.AddComponent<LayoutElement>();
		emptyLayout.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
		emptyLayout.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
		emptyLayout.flexibleWidth = 0;
		emptyLayout.flexibleHeight = 0;

		placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());


		parentToReturnTo = this.transform.parent;
		placeholderParent = parentToReturnTo;

		this.transform.SetParent(this.transform.parent.parent);

		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}


	public void OnDrag(PointerEventData eventData) {
		
		this.transform.position = eventData.position;

		if (placeholder.transform.parent != placeholderParent)
			placeholder.transform.SetParent(placeholderParent);

		int newSiblingIndex = placeholderParent.childCount;

		for (int i = 0; i < placeholderParent.childCount; i++) {
			if (this.transform.position.x < placeholderParent.GetChild(i).position.x) {

				newSiblingIndex = i;

				if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
					newSiblingIndex--;
				}
				break;
			}
		}
		placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}


	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log ("End drag.");
		this.transform.SetParent(parentToReturnTo);
		this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());

		GetComponent<CanvasGroup>().blocksRaycasts = true;
		Destroy(placeholder);
	}

}