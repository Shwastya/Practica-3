using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISoundButtonBehaviour : MonoBehaviour
{
	// Sprites de imagen Activado y Desactivado.
	public Sprite SoundOn;
	public Sprite SoundOff;

	public Image buttonImage;   // Imagen mostrada en la interfaz

	public Slider slider;

	private float oldVolume = -1f;

	void Start()
	{
		// slider = GetComponent<Slider>();
		slider.value = GameManager.instance.volumen;
	}


	public void toggleSound()   // Funcion que se llamara al pulsar encima
	{
		//Invertir el valor de GameManager.instance.soundEnabled
		// TODO
		if (GameManager.instance.volumen > 0)
		{
			oldVolume = GameManager.instance.volumen;
			GameManager.instance.toggleSound(!GameManager.instance.soundEnabled);
			GameManager.instance.volumen = 0.0f;
			slider.value = 0.0f;
		}
		else
		{
			GameManager.instance.volumen = oldVolume;
			GameManager.instance.toggleSound(!GameManager.instance.soundEnabled);
			slider.value = oldVolume;
		}
		
		// Actualizar la imagen con el sprite correspondiente (buttonImage.sprite = SoundOn/SoundOff).
		// TODO
		toggleImage();
		// updateSlide();
	}
	private void toggleImage() { buttonImage.sprite = (GameManager.instance.soundEnabled) ? SoundOn : SoundOff; }
	public void slideSound()
	{
		GameManager.instance.updateVolume(slider.value);

		if (slider.value == 0)
		{
			GameManager.instance.toggleSound(false);
			buttonImage.sprite = SoundOff;
		}
		else
		{
			GameManager.instance.toggleSound(true);
			buttonImage.sprite = SoundOn;
		}			
	}		
}
