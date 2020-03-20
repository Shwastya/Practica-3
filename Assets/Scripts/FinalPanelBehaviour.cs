using UnityEngine;
using UnityEngine.UI;

public class FinalPanelBehaviour : MonoBehaviour
{
	// Clase que representa el panel de fin de juego
	// TODO
	static int appearHash = Animator.StringToHash("Appear");
	static int resetHash = Animator.StringToHash("Reset");

	public Text finalText = null;  
	public bool slided = false;

	private Animator _an = null;

	void Start()
	{		
		_an = GetComponent<Animator>();
	}
	public void setPanel(string msg)
	{
		finalText.text = msg;
	}
	public void slidePanel(bool InOut)
	{
		slided = InOut;
		_an.SetTrigger(slided ? appearHash : resetHash);
	
	}
}
