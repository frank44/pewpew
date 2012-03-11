using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /// <summary>
    /// The confirmation screen for the pause menu's exit option.
    /// </summary>
    class PauseConfirmationScreen : ConfirmationScreen
    {
       #region Events


        /// <summary>
        /// Event handler for when the Yes menu entry is selected.
        /// </summary>
        protected override void YesMenuEntrySelected(object sender, EventArgs e)
        {
            //When Yes is selected, go back to the main menu
            if (Session.IsActive)
            {
                Session.EndSession();
            }
            LoadingScreen.Load(ScreenManager, true, new MainMenuScreen());
        }


        /// <summary>
        /// Event handler for when the No menu entry is selected.
        /// </summary>
        protected override void NoMenuEntrySelected(object sender, EventArgs e)
        {
            if (Session.IsActive)
            {
                //When No is selected, go back to the previous menu screen
                ExitScreen();
            }
        }


        #endregion
    }
}
