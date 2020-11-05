using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;

public class ReadTwitchChat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _RefreshConnectionTimer = 60;
    private float _Timer;

    [Header("Twitch")]
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    [SerializeField] private string _Username = ""; //Twitch user name
    [SerializeField] private string _OauthToken = ""; //Get token from https://twitchapps.com/tmi
    [SerializeField] private string _Channelname = ""; //Twitch channel name

    void Start()
    {
        Connect();
    }

    void Update()
    {
        //Check connection
        if (!twitchClient.Connected)
            Connect();

        _Timer -= 1 * Time.deltaTime;
        if (_Timer <= 0)
        {
            Connect();
            _Timer = _RefreshConnectionTimer;
        }

        ReadChat();
    }

    private void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + _OauthToken);
        writer.WriteLine("NICK " + _Username);
        writer.WriteLine("USER " + _Username + " 8 * :" + _Username);
        writer.WriteLine("JOIN #" + _Channelname);

        writer.Flush();
    }

    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();

            if (message.Contains("PRIVMSG"))
            {
                //Split
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);

                //Name
                chatName = chatName.Substring(1);

                //Message
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                print(string.Format("{0}: {1}", chatName, message));

                if (message.ToLower().Contains("example"))
                {
                    Debug.Log("<color=green>" + chatName + " has used the command example </color>");
                }
            }
        }
    }

}