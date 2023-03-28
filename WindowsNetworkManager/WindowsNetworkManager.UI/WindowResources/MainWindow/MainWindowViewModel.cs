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
    private readonly NetworkAdaptersManager _networkAdaptersManager;
    private readonly UserTextDialogPrompter _userTextDialogPrompter;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public MainWindowViewModel(
        ILogger logger,
        SessionPersistentState sessionPersistentState,
        NetworkAdaptersManager networkAdaptersManager,
        UserTextDialogPrompter userTextDialogPrompter
    )
    {
        _logger = logger;
        _localSessionPersistentState = sessionPersistentState;
        _networkAdaptersManager = networkAdaptersManager;
        _userTextDialogPrompter = userTextDialogPrompter;
    }

    private AdaptersProfileSet? GetProfileByName(string profileName)
    {
        if (LocalSessionPersistentState is null) throw new NullReferenceException();

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
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        LocalSessionPersistentState.ProfileCreationCounter++;

        var profile = new AdaptersProfileSet()
        {
            ProfileName = $"Profile {LocalSessionPersistentState.ProfileCreationCounter}"
        };

        foreach (var adapterName in _networkAdaptersManager.GetSystemAdapters())
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

        foreach (var networkAdapterChange in profile.ProfileSet)
        {
            if (networkAdapterChange.TaskIsDisable)
                _networkAdaptersManager.SetNetworkAdapterState(networkAdapterChange.Name, false);
            
            if (networkAdapterChange.TaskIsEnable)
                _networkAdaptersManager.SetNetworkAdapterState(networkAdapterChange.Name, true);
        }
    }

    [RelayCommand]
    private void RenameProfile(object? profileNameToDelete)
    {
        var convertedProfileName = ((string?)profileNameToDelete) ?? "";

        var profile = GetProfileByName(convertedProfileName);

        if (profile is null) throw new NullReferenceException();
        if (LocalSessionPersistentState is null) throw new NullReferenceException();

        var userResponse = _userTextDialogPrompter.GetTextFromUser("New Profile Name",
            "Please enter what you would like to rename the profile to:");

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
}