using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    class MenuScreen : GameScreen
    {
        #region Fields

        //Information about which option is selected
        protected List<Texture2D> menu;
        protected int menuIndex;

        #endregion


        #region Initialitization

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion


        #region Handle Input

        public override void HandleInput()
        {
            int previousMenuIndex = menuIndex;

            // Move to the previous menu entry
            if (InputManager.IsActionTriggered(InputManager.Action.CursorUp))
            {
                menuIndex = menuIndex--;
                if (menuIndex < 0)
                    menuIndex = menu.Count - 1;
            }

            // Move to the next menu entry
            if (InputManager.IsActionTriggered(InputManager.Action.CursorDown))
            {
                menuIndex = menuIndex++;
                if (menuIndex >= menu.Count)
                    menuIndex = 0;
            }

            // Accept or cancel the menu
            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                //AudioManager.PlayCue("Continue");
               // OnSelectEntry(selectedEntry);
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Back) ||
                InputManager.IsActionTriggered(InputManager.Action.ExitGame))
            {
                OnCancel();
            }
            else if (menuIndex != previousMenuIndex)
            {
                //AudioManager.PlayCue("MenuMove");
            }
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        #endregion
    }
}
