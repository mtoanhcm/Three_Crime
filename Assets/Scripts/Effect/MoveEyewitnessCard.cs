using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveEyewitnessCard : MonoBehaviour {

	[SerializeField] Transform _target;
	// Use this for initialization
	void Start () {
		transform.DOMove (_target.position, 1);
	}
}
