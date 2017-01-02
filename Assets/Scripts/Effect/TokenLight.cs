using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TokenLight : MonoBehaviour {
	
	void Start () {
		transform.DORotate (new Vector3 (0, 0, 180), 2);
	}
}
