using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

public class NetworkAdaptersManager
{
    public List<string> GetSystemAdapters()
    {
        var networkConnections = new List<string>();
        var query = "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus IS NOT NULL";

        using var searcher = new ManagementObjectSearcher(query);

        foreach (ManagementObject networkAdapter in searcher.Get())
        {
            var connectionName = networkAdapter["NetConnectionID"]?.ToString();
            if (!string.IsNullOrEmpty(connectionName))
            {
                networkConnections.Add(connectionName);
            }
        }

        return networkConnections;
    }
    
    public void SetNetworkAdapterState(string adapterName, bool enabled)
    {
        var commandString = "";

        if (enabled)
        {
            commandString = $"""netsh interface set interface "{adapterName}" enable""";
        }
        else
        {
            commandString = $"""netsh interface set interface "{adapterName}" disable""";
        }
        
        var processStartInfo = new ProcessStartInfo("CMD", commandString);
        
        var proc = new Process();
        proc.StartInfo = processStartInfo;           
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; 
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.Verb = "runas";            
        proc.StartInfo.Arguments = $"/env /user:Administrator cmd /K {commandString}";
        proc.StartInfo.CreateNoWindow = true;

        Debug.WriteLine($"Running: {commandString} {proc.StartInfo.Arguments}");
        
        proc.Start();
    }
}