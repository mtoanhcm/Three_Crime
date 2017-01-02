using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	[SerializeField] private GameObject[] _suspects;
	[SerializeField] private Transform[] _suspectsChoosePos;
	[SerializeField] private Transform[] _suspectsPosOnChooseBanner;
	[SerializeField] private Transform[] _suspectForAccusations;
	[SerializeField] private GameObject _card;
	[SerializeField] private GameObject _loosePannel;
	[SerializeField] private GameObject _winPannel;
	[SerializeField] private GameObject _resultPannel;
	[SerializeField] private GameObject _highlightAccusationButton;
	[SerializeField] private GameObject _backButton;
	[SerializeField] private GameObject _accusationPannel;
	[SerializeField] private GameObject _boardPannel;
	[SerializeField] private Transform _drawPos;
	[SerializeField] private Transform _eyewitnessCardPos;
	[SerializeField] private Transform _boardPos;
	[SerializeField] private Text _cardLeftText;
	[SerializeField] private Text _popupText;
	[SerializeField] private Text _numAccusationText;
	[SerializeField] private Text _numScoreText;
	[SerializeField] private AudioSource _audioSource;
	[SerializeField] private AudioClip _winClip;

	private int _numOfCard;
	private List<GameObject> _listCardWithSuspects;
	private List<GameObject> _listCardPlayable;
	private List<Transform> _suspectsChoosen;
	private Transform[] _posInBoard;
	private GameObject _eyewitnessCard;
	private int _numCardLeft;
	private int _numAccusation;


	void MakeInstance(){
		if (instance == null) {
			instance = this;
		}
	}

	// Use this for initialization
	void Awake () {
		MakeInstance ();
		_numOfCard = 10;
		_numAccusation = 1;
		_numCardLeft = _numOfCard;
		_listCardWithSuspects = new List<GameObject> ();
		_listCardPlayable = new List<GameObject> ();
		_suspectsChoosen = new List<Transform> ();
		_numScoreText.text = "" + SystemController.instance.totalPoint;
		_cardLeftText.text = "" + _numCardLeft;
		_numAccusationText.text = "" + _numAccusation + "x";
	}

	void Start(){

		_posInBoard = _boardPos.GetComponentsInChildren<Transform> ();

		SuffleSuspects (_suspects);
		GenerateCardWithSuspects ();

		DrawCard ();
	}
	

	#region create card desk then add to _listCardWithSuspects

	void GenerateCardWithSuspects(){
		for (int i = 0; i < _suspects.Length; i++) {
			for (int j = i + 1; j < _suspects.Length; j++) {
				for (int k = j + 1; k < _suspects.Length; k++) {
					CreateCardAndAddSuspectsToCard (_suspects [i], _suspects [j], _suspects [k]);
				}
			}
		}
	}

	/// <summary>
	/// Creates the card and add suspects to card.
	/// </summary>
	/// <param name="_firstSuspect">First suspect.</param>
	/// <param name="_secondSuspect">Second suspect.</param>
	/// <param name="_thirtSuspect">Thirt suspect.</param>
	void CreateCardAndAddSuspectsToCard(GameObject _firstSuspect, GameObject _secondSuspect, GameObject _thirtSuspect){
		GameObject _tempCard = Instantiate (_card, _drawPos.position, Quaternion.identity) as GameObject;
		GameObject _tempFirstSuspect = Instantiate (_firstSuspect, _tempCard.transform.GetChild (0).GetChild(0).position, Quaternion.identity) as GameObject;
		GameObject _tempSecondSuspect = Instantiate (_secondSuspect, _tempCard.transform.GetChild (0).GetChild(1).position, Quaternion.identity) as GameObject;
		GameObject _tempThirtSuspect = Instantiate (_thirtSuspect, _tempCard.transform.GetChild (0).GetChild(2).position, Quaternion.identity) as GameObject;

		_tempFirstSuspect.transform.SetParent (_tempCard.transform.GetChild(0));
		_tempSecondSuspect.transform.SetParent (_tempCard.transform.GetChild(0));
		_tempThirtSuspect.transform.SetParent (_tempCard.transform.GetChild(0));

		_tempFirstSuspect.GetComponent<SpriteRenderer> ().flipY = true;
		_tempSecondSuspect.GetComponent<SpriteRenderer> ().flipY = true;
		_tempThirtSuspect.GetComponent<SpriteRenderer> ().flipY= true;


		_tempSecondSuspect.GetComponent<SpriteRenderer> ().sortingOrder = 5;
		_tempThirtSuspect.GetComponent<SpriteRenderer> ().sortingOrder = 6;

		for (int i = 0; i < 3; i++) {
			Destroy (_tempCard.transform.GetChild (0).GetChild(i).gameObject);
		}

		_listCardWithSuspects.Add (_tempCard);
	}

	/// <summary>
	/// Suffles the suspects.
	/// </summary>
	/// <param name="_allSuspects">All suspects.</param>
	void SuffleSuspects(GameObject[] _allSuspects){
		for (int i = _allSuspects.Length - 1; i > 0; i--) {
			int _random = Random.Range(0,i+1);
			var _temp = _allSuspects[i];
			_allSuspects[i] = _allSuspects[_random];
			_allSuspects[_random] = _temp;
		}
	}
	#endregion

	#region draw random cards and place it on board. Draw eyewitness card

	void DrawCard(){
		DrawCardEyewitness ();
		DrawRandomCard (_numOfCard);

		SetCardPlayableToBoard ();
	}

	void DrawCardEyewitness(){
		_eyewitnessCard = _listCardWithSuspects [Random.Range (0, _listCardWithSuspects.Count)];
		_listCardWithSuspects.Remove (_eyewitnessCard);

		//use for test
		_eyewitnessCard.transform.position = _eyewitnessCardPos.position;

	}

	/// <summary>
	/// Draws the random card.
	/// </summary>
	/// <param name="_cardNum">Card number.</param>
	void DrawRandomCard(int _cardNum){
		while (_listCardPlayable.Count < _cardNum) {
			GameObject randomGO = _listCardWithSuspects [Random.Range (0, _listCardWithSuspects.Count)];
			if (!_listCardPlayable.Contains (randomGO)) {
				_listCardPlayable.Add (randomGO);
			}
		}
	}

	/// <summary>
	/// Sets the card playable to board.
	/// </summary>
	void SetCardPlayableToBoard(){
		for (int i = 1; i < _posInBoard.Length; i++) {
			var _tempScript = _listCardPlayable [i - 1].GetComponent<CardController> ();
			_tempScript.SetPosOnBoard (_posInBoard [i]);
		}
	}
	#endregion

	#region compare suspects in withness card with face up cards on board

	/// <summary>
	/// Compares the with eyewitness card.
	/// Use at CardController.cs --> Click on Card
	/// </summary>
	/// <returns>The with eyewitness card.</returns>
	/// <param name="_myCard">My card.</param>
	public int CompareWithEyewitnessCard(Transform _myCard){
		int _numOfSuspectsExist = 0;
		Transform _tempEyewitnessCard = _eyewitnessCard.transform;
		Transform _tempMyCard = _myCard.transform;
		string[] _suspectsNameOfEyewitness = new string[3];
		string[] _suspectsNameOfCard = new string[3];

		for (int i = 0; i < 3; i++) {
			_suspectsNameOfEyewitness [i] = _tempEyewitnessCard.GetChild (0).GetChild(i).name;
			_suspectsNameOfCard [i] = _tempMyCard.GetChild (0).GetChild (i).name;
		}

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (_suspectsNameOfEyewitness [i] == _suspectsNameOfCard [j]) {
					_numOfSuspectsExist++;
				}
			}
		}

		CardLeftShow ();
		return _numOfSuspectsExist;
	}

	#endregion

	/// <summary>
	/// Show amount of Cards.
	/// </summary>
	void CardLeftShow(){
		_numCardLeft--;
		_cardLeftText.text = "" + _numCardLeft;

		if (_numCardLeft == 0) {
			_highlightAccusationButton.SetActive (true);
		}
	}

	#region choose suspects to win

	/// <summary>
	/// Clicks the choose suspect from list Supects on screen.
	/// </summary>
	/// <param name="_suspect">Suspect.</param>
	public void ClickChooseSuspect(Transform _suspect){
		ToggleSuspectClick (false);
		if (_suspectsChoosen.Count >= 3 && !CheckSuspectIsInAccusationBanner(_suspect)) {
			AllSuspectsReturnToOldPos ();
			_suspectsChoosen.Clear ();
		}

		if (CheckSuspectIsInAccusationBanner (_suspect)) {
			ReturdCardIfCardIsInAccusationBanner (_suspect);
		} else {
			_suspectsChoosen.Add (_suspect);
			_suspect.DOMove (EmptyBoardPannelPosition (), 0.2f).OnComplete (()=>ToggleSuspectClick (true));
		}
	}

	/// <summary>
	/// Alls the suspects return to their old position.
	/// </summary>
	void AllSuspectsReturnToOldPos(){
		foreach (Transform _pos in _suspectsChoosePos) {
			foreach (Transform _suspect in _suspectsChoosen) {
				if (_pos.tag == _suspect.name) {
					_suspect.DOMove (_pos.position, 0.2f).OnComplete (()=>ToggleSuspectClick (true));
				}
			}
		}
	}

	/// <summary>
	/// Returds the card if card is in accusation banner.
	/// Use at CardController.cs --> Click on card
	/// Check if card was clicked when it's on board of suspects and return it to its old position
	/// </summary>
	/// <param name="_card">Card.</param>
	void ReturdCardIfCardIsInAccusationBanner(Transform _card){
		foreach (Transform _pos in _suspectsPosOnChooseBanner) {
			if (_pos.position == _card.position) {
				foreach (Transform _suspectPos in _suspectsChoosePos) {
					if (_card.name == _suspectPos.tag) {
						_card.DOMove (_suspectPos.position, 0.2f).OnComplete (()=>ToggleSuspectClick (true));
						_suspectsChoosen.Remove (_card);
					}
				}
			}
		}
	}

	/// <summary>
	/// Checks the suspect is in accusation banner.
	/// </summary>
	/// <returns><c>true</c>, if suspect is in accusation banner was checked, <c>false</c> otherwise.</returns>
	/// <param name="_card">Card.</param>
	bool CheckSuspectIsInAccusationBanner(Transform _card){
		foreach (Transform _pos in _suspectsPosOnChooseBanner) {
			if (_card.position == _pos.position) {

				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Toggles the suspect click.
	/// Use to enable or disable click on suspects
	/// </summary>
	/// <param name="_value">If set to <c>true</c> value.</param>
	void ToggleSuspectClick(bool _value){
		foreach (Transform _suspect in _suspectForAccusations) {
			_suspect.GetComponent<Image> ().raycastTarget = _value;
		}
	}

	/// <summary>
	/// Check empties position on the board pannel.
	/// </summary>
	/// <returns>empty position.</returns>
	Vector3 EmptyBoardPannelPosition(){
		Vector3 _emptyPos = _suspectsPosOnChooseBanner[0].position;
		bool[] _isFullfill = new bool[_suspectsPosOnChooseBanner.Length];

		for (int i=0; i< _suspectsPosOnChooseBanner.Length;i++) {
			foreach (Transform _suspect in _suspectsChoosen) {
				if (_suspectsPosOnChooseBanner[i].position == _suspect.position) {
					_isFullfill [i] = true;
				}
			}
		}

		for (int i = _isFullfill.Length; i > 0; i--) {
			if (!_isFullfill [i-1]) {
				_emptyPos = _suspectsPosOnChooseBanner [i-1].position;
			}
		}

		return _emptyPos;
	}

	#endregion

	/// <summary>
	/// Enable or disable all cards on Board --> Use to disable all cards when click Accusation button
	/// </summary>
	/// <param name="_value">If set to <c>true</c> value.</param>
	public void SetStateOfPlayableCard(bool _value){
		foreach (GameObject _card in _listCardPlayable) {
			_card.SetActive (_value);
		}
	}

	#region check win

	/// <summary>
	/// Checks the window condition.
	/// </summary>
	/// <returns><c>true</c>, if window condition was checked, <c>false</c> otherwise.</returns>
	bool CheckWinCondition(){
		int _compareNum = 0;
		var _suspectsInEyewitnessCard = _eyewitnessCard.transform.GetComponentsInChildren<Transform> ();

		foreach (Transform _suspect in _suspectsChoosen) {
			foreach (Transform _witnessSuspect in _suspectsInEyewitnessCard) {
				if (_suspect.tag == _witnessSuspect.tag) {
					_compareNum++;
				}
			}
		}
			
		if (_compareNum >= 3) {
			return true;
		} else {
			return false;
		}
	}

	public void FinalAccusation(){
		int _tempScore = SystemController.instance.totalPoint;

		if (CheckWinCondition()) {
			_resultPannel.SetActive (true);
			_winPannel.SetActive (true);
			_tempScore++;
			_audioSource.PlayOneShot (_winClip);
		} else {
			_numAccusation--;
			_backButton.SetActive (true);
			if (_numAccusation <= 0) {
				_numAccusation = 0;
				_tempScore = 0;
				_resultPannel.SetActive (true);
				_loosePannel.SetActive (true);
			}
			_numAccusationText.text = "" + _numAccusation + "x";
			_popupText.DOFade (1, 0.5f).OnComplete(()=>_popupText.DOFade(0,0.5f).SetDelay(0.5f));
		}
			
		_numScoreText.text = "" + _tempScore;
		SystemController.instance.totalPoint = _tempScore;
	}

	public void ClickResetGame(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void ClickBackToBoard(){
		_backButton.SetActive (false);
		_accusationPannel.SetActive (false);
		_boardPannel.SetActive (true);
		SetStateOfPlayableCard (true);
	}

	public void ClickTutorialButton(){
		SystemController.instance.ClickTutorialButton ();
	}

	#endregion
}