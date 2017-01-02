using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardController : MonoBehaviour {

	[SerializeField] private SpriteRenderer _backCard;
	[SerializeField] private GameObject[] _tokens;
	[SerializeField] private AudioClip _flipSound;
	[SerializeField] private AudioSource _audioSource;
	private Transform _posOnBoard;
	private bool _isFlip;


	void Start(){
		if (_posOnBoard != null) {
			transform.DOMove (_posOnBoard.position, 1f);
		}
	}

	void OnMouseDown(){
		if (!_isFlip) {
			_audioSource.PlayOneShot (_flipSound);
			StartCoroutine (FlipBackToFront ());
			transform.DORotate (new Vector3 (180, 0, 0), 1).OnComplete(AddNumOfSuspects);
			_isFlip = !_isFlip;
		}
	}

	IEnumerator FlipBackToFront(){
		if (transform.eulerAngles.x >= 45) {
			_backCard.sortingOrder = 1;
			yield break;
		}

		yield return new WaitForSeconds (0.1f);
		StartCoroutine (FlipBackToFront ());
	}

	void AddNumOfSuspects(){
		int _tempNum = GameplayController.instance.CompareWithEyewitnessCard (transform);
		var _tempToken = Instantiate (_tokens [_tempNum], transform.GetChild (2).position, Quaternion.identity) as GameObject;
		_tempToken.transform.SetParent (transform.GetChild (2));
	}

	public void SetPosOnBoard(Transform _value){
		this._posOnBoard = _value;
	}
}
