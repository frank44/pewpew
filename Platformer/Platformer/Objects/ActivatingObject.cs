#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace Eve
{
    /// <summary>
    /// These objects can be activated and trigger things such as
    /// checkpoints, other objects, etc.
    /// </summary>
    class ActivatingObject : Object
    {
        #region Properties


        /// <summary>
        /// A list of objects that can be triggered based on their objectID
        /// </summary>
        private List<int> objectsToTrigger;


        #endregion


        #region Initialization


        /// <summary>
        /// Constructor to create a new activating object.
        /// </summary>
        public ActivatingObject(string objectType, Vector2 position, int objectID, string[] objectIDs = null) 
            : base(objectType, position, objectID)
        {
            ObjectClass = ObjectClass.Activate;
            if (objectIDs != null)
            {
                objectsToTrigger = new List<int>();
                foreach (string currentObjectID in objectIDs)
                {
                    objectsToTrigger.Add(int.Parse(currentObjectID));
                }
            }
        }


        /// <summary>
        /// Load the sprite of the activating object.
        /// </summary>
        public override void LoadContent(bool isLooping = false)
        {
            base.LoadContent();
        }


        #endregion


        #region Update and Draw


        /// <summary>
        /// Update the object.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        /// <summary>
        /// Draws the object
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 screen, Color color, bool freeze = true)
        {
            base.Draw(gameTime, spriteBatch, screen, color, freeze);
        }


        #endregion


        #region Methods


        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public override Object Clone()
        {
            ActivatingObject clone = new ActivatingObject(objectType, Position, objectID);
            clone.objectsToTrigger = objectsToTrigger;
            clone.sprite = sprite;
            return clone;
        }


        /// <summary>
        /// Activates the object.
        /// </summary>
        public virtual void Activate()
        {
            foreach (Object currentObject in Session.Level.Objects)
            {
                if (objectsToTrigger.Contains(currentObject.ObjectID))
                {
                    ((TriggerObject)currentObject).Trigger();
                }
            }
        }


        #endregion
    }
}
