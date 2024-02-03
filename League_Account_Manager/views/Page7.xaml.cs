﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using static League_Account_Manager.lcu;

namespace League_Account_Manager.views;

/// <summary>
///     Interaction logic for Page7.xaml
/// </summary>
public partial class Page7 : Page
{
    public static int yayornay = new();

    private readonly string[] list =
    {
        "C:\\ProgramData\\Riot Games\\",
        "C:\\Riot Games\\Riot Client\\UX\\GPUCache",
        "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Riot Games\\",
        "C:\\Riot Games\\Riot Client\\UX\\databases-incognito",
        "C:\\Users\\" + Environment.UserName + "\\AppData\\LocalLow\\Microsoft\\CryptnetUrlCache\\",
        "C:\\Riot Games\\Riot Client\\UX\\icudtl.dat",
        "C:\\Riot Games\\League of Legends\\databases-off-the-record",
        "C:\\Riot Games\\League of Legends\\debug.log",
        "C:\\Riot Games\\League of Legends\\Logs",
        "C:\\Riot Games\\League of Legends\\Config",
        "C:\\Riot Games\\League of Legends\\icudtl.dat",
        "C:\\Riot Games\\League of Legends\\system.yaml",
        "C:\\Riot Games\\League of Legends\\snapshot_blob.bin",
        "C:\\Riot Games\\League of Legends\\natives_blob.bin",
        "C:\\Riot Games\\Riot Client\\snapshot_blob.bin",
        "C:\\Riot Games\\Riot Client\\natives_blob.bin",
        "C:\\Riot Games\\Riot Client\\UX\\icudtl.dat",
        "C:\\Riot Games\\Riot Client\\UX\\v8_context_snapshot.bin",
        "C:\\Riot Games\\Riot Client\\UX\\snapshot_blob.bin",
        "C:\\Riot Games\\Riot Client\\UX\\natives_blob.bin",
        "C:\\Riot Games\\League of Legends\\DATA",
        "C:\\Riot Games\\League of Legends\\v8_context_snapshot.bin"
    };

    public Page7()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var championsbought = "Files \n";
        Page1.killleaguefunc();
        DeleteFilesAndFolders(list, championsbought);
    }

    public void DeleteFilesAndFolders(string[] paths, string championsbought)
    {
        foreach (var path in paths)
        {
            try
            {


            if (File.Exists(path))
            {
                File.Delete(path);
                championsbought = championsbought + "Deleted Item: " + path + "\n";
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                championsbought = championsbought + "Deleted Item: " + path + "\n";
            }
            else
            {
                championsbought = championsbought + "Failed to delete item or item does not exist: " + path + "\n";
            }
            }
            catch (Exception e)
            {
                championsbought = championsbought + "Failed to delete item or item does not exist: " + path + " , make sure that LAM is running as admin\n";

            }
            success.Text = championsbought;
        }

        championsbought = championsbought + "LOGS HAVE BEEN CLEANED!!!";
        success.Text = championsbought;
    }

    private async void Button_Click1(object sender, RoutedEventArgs e)
    {
        new Window3().ShowDialog();
        if (yayornay == 1)
        {
            var championsbought = "Friends \n";
            var resp = await Connector("league", "get", "/lol-chat/v1/friends", "");
            if (resp.ToString() == "0")
            {
                notif.notificationManager.Show("Error", "League of legends client is not running!",
                    NotificationType.Error, "WindowArea", onClick: notif.donothing);
                return;
            }

            var responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var rankedinfo = JArray.Parse(responseBody2);
            Console.WriteLine(rankedinfo);
            foreach (var VARIABLE in rankedinfo)
            {
                resp = await Connector("league", "delete", "/lol-chat/v1/friends/" + VARIABLE["id"], "");
                championsbought = championsbought + "Deleted Friend: " + VARIABLE["gameName"] + "\n";
                success.Text = championsbought;
                Thread.Sleep(400);
            }
        }
    }

    private async void Button_Click2(object sender, RoutedEventArgs e)
    {
        var friendlist = "";
        var resp = await Connector("league", "get", "/lol-chat/v1/friends", "");
        if (resp.ToString() == "0")
        {
            notif.notificationManager.Show("Error", "League of legends client is not running!",
                NotificationType.Error, "WindowArea", onClick: notif.donothing);
            return;
        }

        var responseBody2 = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        JArray rankedinfo = JArray.Parse(responseBody2);
        Console.WriteLine(rankedinfo);
        foreach (var VARIABLE in rankedinfo)
            try
            {
                var resp2 = await Connector("league", "get",
                    "/lol-match-history/v1/products/lol/" + VARIABLE["puuid"] +
                    "/matches?begIndex=0&endIndex=0", "");
                var Game = await resp2.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rankedinfo2 = JObject.Parse(Game);
                if (rankedinfo2["games"]["gameCount"] == 0)
                {
                    friendlist = friendlist + "Friend name: " + VARIABLE["name"] + " ,RiotID: " +
                                 VARIABLE["gameName"] + "#" + VARIABLE["gameTag"] + " ,LastPlayed: " +
                                 "Inactive account" + "\n";
                    success.Text = friendlist;
                }
                else
                {
                    var date = (long)rankedinfo2["games"]["games"][0]["gameCreation"] / 1000;
                    friendlist = friendlist + "Friend name: " + VARIABLE["name"] + " ,RiotID: " +
                                 VARIABLE["gameName"] + "#" + VARIABLE["gameTag"] + " ,LastPlayed: " +
                                 DateTimeOffset.FromUnixTimeSeconds(date).ToString("dd/MM/yyyy") + "\n";
                    success.Text = friendlist;
                }
            }
            catch (Exception ex)
            {
            }
    }
}