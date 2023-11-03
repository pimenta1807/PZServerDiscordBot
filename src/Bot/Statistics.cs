using System;
using System.Diagnostics;

public static class Statistics
{
    public static double GetTotalCPUUsagePercentage()
    {
        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        return cpuCounter.NextValue();
    }

    public static double GetTotalRAMUsagePercentage()
    {
        var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        var totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
        var availableMemory = ramCounter.NextValue();
        var usedMemory = totalMemory - availableMemory;
        return (usedMemory / totalMemory) * 100;
    }

    public static string GetPercentageValueProgressBar(string title, double val)
    {
        char padChar  = ' ';
        char fillChar = '#';
        int  barWidth = 20;

        int repeatCount = ((int)(val - (val % 5))) / 5;
        string progressBar = string.Format("{0} [{1}] {2}%", title, new string(fillChar, repeatCount) + new string(padChar, barWidth - repeatCount), val.ToString("0.##"));
        
        return progressBar;
    }
}
