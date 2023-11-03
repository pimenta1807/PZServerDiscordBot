﻿using System;

public static class ServerPath
{
    public static string BasePath = "/home/conteiner/Zomboid/";

    public static string ServerLogsPath()
    {
    #if DEBUG
        return "./Logs/";
    #else
        string path = BasePath + "Logs/";
        return path;
    #endif
    }

    public static string ServerSettingsPath()
    {
        string path = BasePath + "Server/";
        return path;
    }
    public static string ServerBaseSavesPath()
    {
        string path = BasePath + "Saves/";
        return path;
    }

    public static string ServerSavesPath()
    {
        string path = BasePath + "Saves/Multiplayer/" + ServerUtility.GetServerConfigIniFileName(true) + "/";
        return path;
    }

    public static string ServerSavesPlayerPath()
    {
        string path = BasePath + "Saves/Multiplayer/" + ServerUtility.GetServerConfigIniFileName(true) + "_player/";
        return path;
    }

    public static string ServerDatabasePath()
    {
        string path = BasePath + "db/";
        return path;
    }

    public static string ServerLuaPath()
    {
        string path = BasePath + "Lua/";
        return path;
    }

    public static string MapTimeFilePath()
    {
        string path = ServerSavesPath() + "map_t.bin";
        return path;
    }
}