/*
 I had Alex Hackl's menu code ( http://wiki.unity3d.com/index.php?title=JoystickButtonMenu ) 
 up as a reference when writing this. But the code in this file is a complete rewrite, 
 so I'll release this code as PUBLIC DOMAIN instead of CC license. Much thanks to Alex 
 for posting his code which made mine much easier to write.
 Enjoy! You can contact me, Erik Hermansen, at info@seespacelabs.com if you like.
*/
using UnityEngine;
namespace StickMenu
{
	public class Menu {
		//Change these to match what you've defined in InputManager.
		private const string SELECT_AXIS = "360_VerticalDPAD";
		private const string SELECT_BUTTON = "360_AButton";
		private const string BACK_BUTTON = "360_BButton";
		//Constants for drawing menu options.
		private const float RECT_CY = 30f;
		private const float RECT_CX = 220f;
		private const float TEXT_INDENT_CX = 50f;
		//Input freeze intervals to help the menu control work intuitively.
		private const float BUTTON_FREEZE_DELAY = .1f;
		private const float AXIS_FREEZE_DELAY = .2f;
		private float noInputUntil = -1f;
		//Each menu option button will be in one of these three states.
		private enum ButtonState {
			up,
			down,
			over
		}
		//Menu state data.
		private struct MenuOption {
			public string name;
			public Rect rect;
			public Rect textRect;
			public ButtonState state;
		}
		private MenuOption[] options;
		private int selectedNo = 0;
		//Pass in names to be displayed in menu options.
		public Menu (string[] optionNames) {
			SetOptions (optionNames);
		}
		//Can be called for initial setup or at any time after.
		public void SetOptions (string[] optionNames) {
			string oldSelectedName = "";
			if (this.options != null && this.options.Length > 0)
				oldSelectedName = this.options[this.selectedNo].name.ToLower();
			this.selectedNo = 0;
			this.options = new MenuOption[optionNames.Length];
			float rectX = Screen.width / 2 - RECT_CX / 2;
			float rectY = Screen.height / 2 - (this.options.Length * RECT_CY) / 2;
			int i = 0;
			foreach (string optionName in optionNames) {
				MenuOption mo = new MenuOption ();
				mo.name = optionName;
				if (optionName.ToLower() == oldSelectedName) //If changing options in a preexisting menu, preserve original selection.
					this.selectedNo = i;
				mo.rect = new Rect (rectX, rectY, RECT_CX, RECT_CY);
				mo.textRect = new Rect (rectX + TEXT_INDENT_CX, rectY, RECT_CX - TEXT_INDENT_CX, RECT_CY);
				mo.state = ButtonState.up;
				options [i++] = mo;
				rectY += RECT_CY;
			}
			options [selectedNo].state = ButtonState.over;
		}
		//Must be called from OnGUI of a MonoBehavior class. Change this method 
		//if you want to draw your menu differently.
		public void Display () {
			Texture upTexture = (Texture)GUI.skin.button.normal.background;
			Texture overTexture = (Texture)GUI.skin.button.hover.background;
			Texture downTexture = (Texture)GUI.skin.button.active.background;
			foreach (MenuOption mo in options) {
				if (mo.state == ButtonState.down) {
					GUI.DrawTexture (mo.rect, downTexture);
					GUI.skin.label.normal.textColor = GUI.skin.button.active.textColor;
				} else if (mo.state == ButtonState.over) {
					GUI.DrawTexture (mo.rect, overTexture);
					GUI.skin.label.normal.textColor = GUI.skin.button.hover.textColor;
				} else {
					GUI.DrawTexture (mo.rect, upTexture);
					GUI.skin.label.normal.textColor = GUI.skin.button.normal.textColor;
				}
				GUI.Label (mo.textRect, mo.name);
			}
		}
		//Returns true if user has made a selection by clicking. Also handles update 
		//of menu state based on controller input. Call from Update() method of a 
		//MonoBehavior object.
		public bool CheckForSelection () {
			float now = Time.realtimeSinceStartup; //Using .realtime instead of .time so that menus are immune to pausing via Time.timeScale = 0.
			if (now < this.noInputUntil) return false; //Check for previously clicked option that was animated in down state. 
			if (this.options [this.selectedNo].state == ButtonState.down) { 
				this.options [this.selectedNo].state = ButtonState.over; return true; 
			} 
			//When user clicks option, it is shown temporarily in down state. 
			if (Input.GetButtonDown(SELECT_BUTTON)) { 
				this.options [this.selectedNo].state = ButtonState.down; 
				this.noInputUntil = now + BUTTON_FREEZE_DELAY; //Add the freeze so I can see button displayed in down state. 
				return false; 
			}
			//When user clicks back button, select a menu option called "back" if 
			//present and show temporary down state. 
			if (Input.GetButtonDown (BACK_BUTTON)) { 
				//Check for back button in menu to jump to. 
				int backButtonNo = findBackButton (this.options); 
				if (backButtonNo == -1) return false; //No back button. 
				this.options [this.selectedNo].state = ButtonState.up; 
				this.selectedNo = backButtonNo; 
				this.options [this.selectedNo].state = ButtonState.down; 
				this.noInputUntil = now + BUTTON_FREEZE_DELAY; //Add the freeze so I can see button displayed in down state. 
				return false;
			} 
			//Check for up/down menu movement. 
			float axisValue = Input.GetAxis (SELECT_AXIS); 
			if (axisValue > .1f) {
				this.options [this.selectedNo].state = ButtonState.up;
				if (++this.selectedNo == this.options.Length)
					this.selectedNo = 0; //Loop selection to top.
				this.options [this.selectedNo].state = ButtonState.over;
				this.noInputUntil = now + AXIS_FREEZE_DELAY; //Add the freeze so menu traversal isn't too fast.
			} else if (axisValue < -.1f) {
				this.options [this.selectedNo].state = ButtonState.up;
				if (--this.selectedNo == -1)
					this.selectedNo = this.options.Length - 1; //Loop selection to bottom.
				this.options [this.selectedNo].state = ButtonState.over;
				this.noInputUntil = now + AXIS_FREEZE_DELAY; //Add the freeze so menu traversal isn't too fast.
			}
			return false;
		}
		//Returns current selection of menu. In lower case for use in 
		//case-insensitive comparisons.
		public string Selection () {
			return options[this.selectedNo].name.ToLower();
		}
		//Look for a button called "back".
		private static int findBackButton (MenuOption[] mos) {
			for (int i = 0; i < mos.Length; ++i) {
				if (mos [i].name.ToLower () == "back")
					return i;
			}
			return -1; //No match found.
		}
	}
}