using System;
using System.Collections.Generic;
using System.Linq;

namespace LastBoundary
{
    public class Channel
    {
        #region Fields 
        private int number = -1;//Number of channel in order
        private string name = null;//name of the channel
        private Uri link = null;//Link to a channel
        #endregion

        #region Properties
        public int Number { get => number; private set => number = value; }
        public string Name { get => name; set => name = value; }
        public Uri Link { get => link; set => link = value; }
        #endregion

        #region Constructors
        //without it, can't use in settings
        public Channel()
        {
            number = -1;
            name = null;
            link = null;
        }
        /// <summary>
        /// Instantiation of channel
        /// </summary>
        /// <param name="Number">number of channel in order</param>
        /// <param name="Name">Name of channel</param>
        /// <param name="sLink">String which contains Link to channel</param>
        public Channel(int Number, string Name, string sLink)
        {
            number = Number;
            name = Name;
            Uri.TryCreate(sLink, UriKind.RelativeOrAbsolute, out link);

        }

        /// <summary>
        /// Instantiation of channel
        /// </summary>
        /// <param name="Number">number of channel in order</param>
        /// <param name="Name">Name of channel</param>
        /// <param name="sLink">Uri which contains Link to channel</param>
        public Channel(int Number, string Name, Uri sLink)
        {
            number = Number;
            name = Name;
            link = sLink;
        }
        #endregion
    }
}
