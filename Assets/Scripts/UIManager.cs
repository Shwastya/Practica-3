using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region SINGLETON
	protected static UIManager _instance = null;
	public static UIManager instance { get { return _instance; } }
	void Awake() { _instance = this; }
	#endregion

	// Menu principal
	public GameObject mainMenu			= null;	// Panel del menu principal (Primera pantalla en mostrarse)

	// Sub-menus durante el juego
	public FinalPanelBehaviour endPanel	= null;	// Panel de fin de juego (Dentro de la interfaz del juego)
	public Text scoreText				= null; // Puntuacion del juego

	public Image[] hearts;

	public void showMainMenu()
	{
		// Mostrar objeto mainMenu
		// TODO
		mainMenu.SetActive(true);

		// Ocultar endPanel
		// TODO
		if (endPanel.slided) hideEndPanel();
	}
	public void hideMainMenu()
	{
		// Ocultar objeto mainMenu
		// TODO
		mainMenu.SetActive(false);
	}
	public void showEndPanel(bool win)
	{
		// Mostrar panel fin de juego (ganar o perder)
		// TODO		
		endPanel.setPanel(win ? "Misión cumplida!!" : "Misión fallida!!");
		endPanel.slidePanel(true);
	}
	public void hideEndPanel()
	{
		// Ocultar el panel
		// TODO
		endPanel.slidePanel(false);
	}
	public void updateScore(int score)
	{
		// Actualizar el 'UI text' con la puntuacion 
		// TODO
		scoreText.text = "" + score + "";
	}

	public void resetHearts()
	{
		for (int i = 0; i < hearts.Length; i++)
			hearts[i].enabled = true;		
	}
	public void updateHearts(int lives)
	{
		for (int i = 0; i < hearts.Length; i++)	hearts[i].enabled = false;
		for (int i = 0; i < lives; i++) hearts[i].enabled = true;
	}

}
