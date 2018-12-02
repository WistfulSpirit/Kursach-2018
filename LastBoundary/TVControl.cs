using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastBoundary
{
    internal class TVControl
    {
        #region FormsInteractionFields
        public delegate void FilterValueChanged(int Value);//Delegate for Func which is called after some filter value changes
        public static FilterValueChanged BrightnessChanged;
        public static FilterValueChanged ContrastChanged;
        #endregion

        #region Fields
        private static Channel previousChannel = null;
        private static Channel currentChannel = null;
        private static List<Channel> channelList = new List<Channel>();
        private static int contrast = 50;
        private static int brightness = 50;
        private static bool isOn = false;
        private static bool isMute = false;
        private static readonly int maxVolume = 100;
        private static readonly int minVolume = 0;
        #endregion

        #region Properties
        public static List<Channel> ChannelList { get => channelList; set => channelList = value; }
        public static Channel CurrentChannel { get => currentChannel; set => currentChannel = value; }
        public static Channel PreviousChannel { get => previousChannel; set => previousChannel = value; }
        public static bool IsOn { get => isOn; set => isOn = value; }
        public static bool IsMute { get => isMute; set => isMute = value; }
        public static int Contrast { get => contrast; set => contrast = value; }
        public static int Brightness { get => brightness; set => brightness = value; }
        public static int MaxVolume => maxVolume;
        public static int MinVolume => minVolume;
        #endregion


        /// <summary>
        /// Get ChannelList contained in file
        /// </summary>
        /// <param name="FilePath">Path to File with channels</param>
        /// <returns>List<Channel></returns>
        public static List<Channel> GetChannelsFromFile(string FilePath)//Retrive channel list from special file
        {
            StreamReader sr = new StreamReader(FilePath);
            string line;
            channelList.Clear();//Clear previous channels list
            string[] channelInf;//will contain splitted string from file 
            if (sr != null)
            {
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    channelInf = line.Split('*');//
                    channelList.Add(new Channel(i, channelInf[0].Trim(), channelInf[1].Trim()));
                }
            }
            DBInteraction.UpdateChannelsTable(channelList);
            return channelList;
        }

        /// <summary>
        /// Get rows from Data Base
        /// </summary>
        /// <returns>List<Channel></returns>
        public static List<Channel> GetChannelsFromDataBase()
        {
            return channelList = DBInteraction.GetChannels();
        }

    }
}
