﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DataModel;
using PeerManager;

namespace WpfMockup
{
    // Methods in this class are called by the message handler, updating the actions of p2p clients
    public class ExampleViewModel : INotifyPropertyChanged
    {
        // Elements from the view are bound to these properties
        public string InitName { get; set; } = "PlayerNameHere";
        public string InitColor { get; set; } = "Color";
        public string ChatHistory { get; set; } = "Welcome!\n";                     // textbox
        public ObservableCollection<PlayerData> PlayerList { get; set; }            // Collection of connected players
        public int ThisPlayer { get; set; }
        
        public ExampleViewModel()
        {
            PlayerList = new ObservableCollection<PlayerData>() {null, null, null, null, null};
            UpdatePlayerData(0, true, "System", "");
            UpdatePlayerData(1, false, "Player1", "Pink");
            UpdatePlayerData(2, false, "Player2", "Blue");
            UpdatePlayerData(3, false, "Player3", "Green");
            UpdatePlayerData(4, false, "Player4", "Yellow");

            OnPropertyChanged(null);                    // null indicates OnPropertyChanged should update all properties
        }

        


        public void DisplayChatMessage(int index, string text)
        {
            ChatHistory += PlayerList[index].Name + ": " + text + "\n";
            OnPropertyChanged("ChatHistory");
        }

        public void UpdatePlayerData(int index, string name, string color)
        {
            PlayerList[index].Name = name;
            PlayerList[index].Color = color;
            DisplayChatMessage(0, "Updated Player" + index);
        }

        public void UpdatePlayerData(int index, bool isFull, string name, string color)
        {
            PlayerList[index] = new PlayerData(isFull, name, color, new Tile("grass", 0));
            DisplayChatMessage(0, "Updated Player" + index);
        }






        public void InitThisPlayer(int playerNum, string name, string color)
        {
            ThisPlayer = playerNum;
            PlayerList[ThisPlayer].IsOccupied = true;
            UpdatePlayerData(playerNum, true, name, color);
            ChatHistory += String.Format("Hi, {0}! You have joined as Player {1}\n", name, playerNum);
            OnPropertyChanged("ChatHistory");
        }

        public void ReceiveMessage(SerializedMessage message)
        {
            switch (message.Purpose)
            {
                case Purpose.Init:
                    ThisPlayer = message.PeerId;
                    InitThisPlayer(ThisPlayer, InitName, InitColor);
                    break;
                case Purpose.Chat:
                    DisplayChatMessage(message.PeerId, message.Text);
                    break;
                case Purpose.Deal:
                    break;
                case Purpose.Select:
                    break;
                case Purpose.Tile:
                    break;
                case Purpose.Player:
                    UpdatePlayerData(message.PeerId, message.PlayerName, message.Color);
                    break;
                default:
                    DisplayChatMessage(0, "Error: Network message not recognized");
                    break;
            }
        }

        // OnPropertyChanged must be called to tell a view bound to this implementation to get updated property
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
