using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum Action
{
    Mute = 1,
    ChannelUp,
    ChannelDown,
    VolumeUp,
    VolumeDown,
    ChannelChange
}

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
        /// Log an action to DB
        /// </summary>
        /// <param name="action">Action type</param>
        /// <param name="description">description, null if not necessary</param>
        public static void Log(Action action, string description)
        {
            DBInteraction.InsertLog(action, description);
        }

        public static void SaveLogToFile(string FileName)
        {
            StreamWriter sw = new StreamWriter(FileName,true);
            foreach (var row in DBInteraction.GetLogs())
            {
                sw.WriteLineAsync(row.ToString());
            }
            sw.Close();
        }

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
            sr.Close(); 
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
