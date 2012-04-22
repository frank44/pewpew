#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Eve
{
    /// <summary>
    /// Manages a video to be played for cutscenes.
    /// </summary>
    class VideoManager
    {
        #region Properties


        private Video video;
        private VideoPlayer videoPlayer;
        private Vector2 position;
        private Vector2 scale;
        private bool loop;
        private double timer;


        /// <summary>
        /// Event raised when video is completed.
        /// </summary>
        public event EventHandler Finished;


        #endregion


        #region Initialization


        /// <summary>  
        /// Video manager lets you add a video and play and stop and such  
        /// </summary>  
        /// <param name="content">pass in content from game1.cs class</param>  
        /// <param name="videoFileLocation">full string file path</param>  
        /// <param name="Position">position to play video, using 0,0 as origin</param>  
        /// <param name="Scale">scale of the video</param>  
        /// <param name="Loop">bool to tell weather or not to loop the video</param>  
        public VideoManager(ContentManager content, string videoFileLocation, Vector2 Position, Vector2 Scale, bool Loop)
        {
            video = content.Load<Video>(videoFileLocation);
            videoPlayer = new VideoPlayer();
            loop = Loop;
            scale = Scale;
            position = Position;
        }


        #endregion


        #region Update and Draw

       
        /// <summary>  
        /// updates the player  
        /// </summary> 
        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (!loop)
                if (timer > video.Duration.TotalSeconds)
                {
                    videoPlayer.Stop();
                    VideoCompleted();
                }
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color color)
        {
            spriteBatch.Draw(videoPlayer.GetTexture(), position, color);
        }


        #endregion


        #region Methods


        public void Start()
        {
            timer = 0;
            if (videoPlayer.State != MediaState.Playing)
                videoPlayer.Play(video);
        }


        public void Stop()
        {
            videoPlayer.Stop();
        }


        public void Pause()
        {
            if (videoPlayer.State == MediaState.Paused)
                videoPlayer.Resume();
            else if (videoPlayer.State == MediaState.Playing)
                videoPlayer.Pause();
        }


        /// <summary>
        /// Method for raising the video completed event.
        /// </summary>
        public void VideoCompleted()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }


        #endregion
    }
}