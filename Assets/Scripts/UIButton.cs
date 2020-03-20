using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonType
{
	NONE, ATTACK, RUN
}
public class UIButton : UIBehaviour
{
	#region STATIC
	static ButtonType input1 = ButtonType.NONE;
	static ButtonType input2 = ButtonType.NONE;
	public static bool GetInput(ButtonType input)
	{
		return input1 == input || input2 == input;
	}
	#endregion

	Image image;
	public ButtonType input;
	public void update(bool pushed)
	{
		changeColor(pushed);
		if (pushed)
		{

			if (input1 == ButtonType.NONE) input1 = input;
			else if (input2 == ButtonType.NONE) input2 = input;
		}
		else
		{
			if (input1 == input) input1 = ButtonType.NONE;
			else if (input2 == input) input2 = ButtonType.NONE;
		}
	}
	// Todo esto que viene es super cutre ya lo se
	// pero no he sabido hacerlo de otra forma
	void changeColor(bool pushed)
	{
		image.color = pushed ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0.3f);
	}	
	protected override void Start()
	{
		image = GetComponent<Image>();
	}
}
