using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
	#region SINGLETON
	protected static GameManager _instance = null;
	public static GameManager instance { get { return _instance; } }
	void Awake () { _instance = this; }
	#endregion

	// Punteros a player y a todos los enemigos (lista 'enemiesList')
	public PlayerBehaviour player = null;
	public List<SkeletonBehaviour> enemiesList = null;	// No requiere inicializacion, se rellena desde el Inspector

	// Lista con los enemigos que quedan vivos
	List<SkeletonBehaviour> currentEnemiesList = null;	

	public AudioClip MainMusic = null;
	public AudioSource sourceMusic = null;

	// Variables internas
	int _score = 0;
	public bool soundEnabled = true;

	public float volumen = 0.0f;
	void Start ()
	{
		currentEnemiesList = new List<SkeletonBehaviour>();
		// Reiniciamos el juego
		// TODO
		reset();

		sourceMusic = gameObject.AddComponent<AudioSource>();
	
		sourceMusic.clip = MainMusic;
		sourceMusic.loop = true;
		
		if (soundEnabled) sourceMusic.Play();
		volumen = 1.0f;		

		sourceMusic.volume = volumen;
	}

	private void reset()		// Funcion para reiniciar el juego
	{
		// Reiniciamos a Player
		// TODO
		player.reset();
		// Incializamos la puntuacion a cero
		// TODO
		_score = 0;
		
		// Rellenamos la lista de enemigos actual.
		currentEnemiesList.Clear();
		foreach (SkeletonBehaviour skeleton in enemiesList)
		{
			skeleton.setPlayer(player);
			skeleton.reset();

			currentEnemiesList.Add(skeleton);
		}
	}

	#region UI EVENTS
	// Evento al pulsar boton 'Start'
	public void onStartGameButton()
	{
		// Ocultamos el menu principal (UIManager)
		// TODO
		UIManager.instance.hideMainMenu();

		// Actualizamos la puntuacion en el panel Score (UIManager)
		// TODO
		_score = 0;
		UIManager.instance.updateScore(_score);

		// Quitamos la pausa a Player
		// TODO
		player.paused = false;

	}

	// Evento al pulsar boton 'Exit'
	public void onExitGameButton()
	{
		// Mostramos el panel principal
		// TODO
		UIManager.instance.showMainMenu();

		// Reseteamos el juego
		// TODO
		reset();
	}

	public void toggleSound(bool enable) 
	{		
		if (sourceMusic != null)
		{
			soundEnabled = enable;

			if (soundEnabled) sourceMusic.Play();
			else sourceMusic.Stop();			
		}
	}
	public void updateVolume(float newVol) 
	{
		if (sourceMusic != null)
		{
			volumen = newVol;
			sourceMusic.volume = volumen;
		}		
	}
	#endregion

	#region GAME EVENTS
	// Evento al ser notificado por un enemigo (cuando muere)
	public void notifyEnemyKilled(SkeletonBehaviour enemy)
	{
		// Eliminamos enemigo de la lista actual
		currentEnemiesList.Remove(enemy);

		// Subimos 10 puntos y actualizamos la puntuacion en la UI
		// TODO
		_score += 10;
		UIManager.instance.updateScore(_score);

		// Si no quedan enemmigos
	
		if (currentEnemiesList.Count == 0)	// KEEP
		{
			// Mostrar panel de 'Mision cumplida' y pausar a Player
			// TODO			
			player.paused = true;
			const bool win = true;
			UIManager.instance.showEndPanel(win);
			player.speedBrake();			
		}
	}

	// Evento al ser notificado por player (cuando muere)
	public void notifyPlayerDead()
	{
		// Mostrar panel de 'Mision fallida' y pausar a Player
		// TODO		
		const bool win = false;
		UIManager.instance.showEndPanel(win);
		player.paused = true;		

	}
	#endregion
}
