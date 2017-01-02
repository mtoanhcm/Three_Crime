using UnityEngine;
using System.Collections;

public class Accusation : MonoBehaviour {

	[SerializeField] private GameObject _board;
	[SerializeField] private GameObject _accusationPanel;
	private bool _isAccusation;

	public void ClickAccusationButton(){
		if (!_isAccusation) {
			transform.GetChild (0).gameObject.SetActive (false);
			_board.SetActive (false);
			GameplayController.instance.SetStateOfPlayableCard (false);
			_accusationPanel.SetActive (true);

		} else {
			GameplayController.instance.FinalAccusation ();
		}

		_isAccusation = !_isAccusation;
	}
}
