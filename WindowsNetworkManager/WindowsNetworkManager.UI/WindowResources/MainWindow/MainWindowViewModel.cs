using System;
using System.Collections.Generic;
using System.Management;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using WindowsNetworkManager.Core.Logic.UiHelpers;
using WindowsNetworkManager.Core.Models;
using WindowsNetworkManager.UI.WpfHelpers;

namespace WindowsNetworkManager.UI.WindowResources.MainWindow;

/// <summary>
/// The ViewModel for MainWindow
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string _textRunningAsUsernameMessage = "";
    [ObservableProperty] private SessionPersistentState? _localSessionPersistentState;
    
    private readonly ILogger _logger;

    private static int _profileSetCount = 0;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public MainWindowViewModel(
        ILogger logger,
        SessionPersistentState sessionPersistentState
        )
    {
        _logger = logger;
        _localSessionPersistentState = sessionPersistentState;
    }

    private AdaptersProfileSet? GetProfileByName(string profileName)
    {
        foreach (var profile in LocalSessionPersistentState.NetworkProfilesSection)
        {
            if (profile.ProfileName == profileName)
                return profile;
        }

        return null;
    }
    
    [RelayCommand]
    private void CreateNewProfile()
    {
        _profileSetCount++;

        var profile = new AdaptersProfileSet()
        {
            ProfileName = $"Profile {_profileSetCount}"
        };

        foreach (var adapterName in GetSystemAdapters())
        {
            profile.ProfileSet.Add(
                new NetworkAdapterChange()
                {
                    Name = adapterName
                });    
        }

        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        LocalSessionPersistentState.NetworkProfilesSection.Add(profile);
    }
    
    [RelayCommand]
    private void ActivateProfile(object? profileNameToDelete)
    {
        var convertedProfileName = ((string?)profileNameToDelete) ?? "";
        
        var profile = GetProfileByName(convertedProfileName);

        if (profile is null) throw new NullReferenceException();
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        // Work the bools
    }
    
    [RelayCommand]
    private void RenameProfile(object? profileNameToDelete)
    {
        var convertedProfileName = ((string?)profileNameToDelete) ?? "";
        
        var profile = GetProfileByName(convertedProfileName);

        if (profile is null) throw new NullReferenceException();
        if (LocalSessionPersistentState is null) throw new NullReferenceException();

        var userResponse = UserTextDialogPrompter.GetTextFromUser("New Profile Name", "Please enter what you would like to rename the profile to:");
        
        if (!string.IsNullOrEmpty(userResponse))
            profile.ProfileName = userResponse;
    }
    
    [RelayCommand]
    private void DeleteProfile(object? profileNameToDelete)
    {
        var convertedProfileName = ((string?)profileNameToDelete) ?? "";
        
        var profile = GetProfileByName(convertedProfileName);

        if (profile is null) throw new NullReferenceException();
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        LocalSessionPersistentState.NetworkProfilesSection.Remove(profile);
    }
    
    private List<string> GetSystemAdapters()
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

    private void LoadDummyAdaptersData()
    {
        var profile01 = new AdaptersProfileSet()
        {
            ProfileName = "Enable WiFi"
        };
        
        profile01.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                TaskIsEnable = true,
                Name = "Test Adapter 1"
            });
        
        profile01.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                Name = "Test Adapter 2"
            });
        
        profile01.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                Name = "Test Adapter 3"
            });
        
        profile01.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                TaskIsEnable = true,
                Name = "Test Adapter 4"
            });

        var profile02 = new AdaptersProfileSet(){
            ProfileName = "Enable Ethernet"
        };
        
        profile02.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                Name = "Test Adapter 1"
            });
        
        profile02.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                TaskIsEnable = true,
                Name = "Test Adapter 2"
            });
        
        profile02.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                TaskIsEnable = true,
                Name = "Test Adapter 3"
            });
        
        profile02.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                
                Name = "Test Adapter 4"
            });

        var profile03 = new AdaptersProfileSet()
        {
            ProfileName = "Disable All"
        };
        
        profile03.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                
                Name = "Test Adapter 1"
            });
        
        profile03.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                
                Name = "Test Adapter 2"
            });
        
        profile03.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                
                Name = "Test Adapter 3"
            });
        
        profile03.ProfileSet.Add(
            new NetworkAdapterChange()
            {
                
                Name = "Test Adapter 4"
            });

        if (LocalSessionPersistentState is null) throw new NullReferenceException();

        LocalSessionPersistentState.NetworkProfilesSection.Add(profile01);
        LocalSessionPersistentState.NetworkProfilesSection.Add(profile02);
        LocalSessionPersistentState.NetworkProfilesSection.Add(profile03);
        
    }
}