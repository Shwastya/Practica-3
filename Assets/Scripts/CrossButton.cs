using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum InputType 
{
	NONE, UP, DOWN, LEFT, RIGHT,
	UP_LEFT, UP_RIGHT,
	DOWN_LEFT, DOWN_RIGHT
}

public class CrossButton : UIBehaviour
{
	#region STATIC
	static InputType input1 = InputType.NONE;
	static InputType input2 = InputType.NONE;
	public static bool GetInput(InputType input)
	{
		return input1 == input || input2 == input;
	}
	#endregion

	Image image;
	public InputType input;
	public void update(bool pushed)
	{
		changeColor(pushed);

		if(pushed)
		{
			
			if (input1 == InputType.NONE ) input1 = input;
			else if(input2 == InputType.NONE) input2 = input;
		}
		else
		{
			if (input1 == input) input1 = InputType.NONE;
			else if (input2 == input) input2 = InputType.NONE;
		}
	}
	// Todo esto que viene es super cutre ya lo se
	// pero no he sabido hacerlo de otra forma
	void changeColor(bool pushed)
	{
		if (input == InputType.UP_LEFT)
		{
			keyDir("UP").color = colorFix(pushed);
			keyDir("LEFT").color = colorFix(pushed);
		}
		else if (input == InputType.UP_RIGHT)
		{
			keyDir("UP").color = colorFix(pushed);
			keyDir("RIGHT").color = colorFix(pushed);
		}
		else if (input == InputType.DOWN_LEFT)
		{
			keyDir("DOWN").color = colorFix(pushed);
			keyDir("LEFT").color = colorFix(pushed);
		}
		else if (input == InputType.DOWN_RIGHT)
		{
			keyDir("DOWN").color = colorFix(pushed);
			keyDir("RIGHT").color = colorFix(pushed);
		}
		else image.color = colorFix(pushed);
	}
	Color colorFix(bool c)
	{
		return c ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0.3f);
	}
	Image keyDir(string dir)
	{
		return GameObject.Find(dir).GetComponent<Image>();
	}
	protected override void Start ()
	{
		image = GetComponent<Image>();
	}
}