using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsNetworkManager.Core.Models;

/// <summary>
/// This holds our main window profiles state that persists between runs of the appliation
/// </summary>
public partial class SessionPersistentState : ObservableObject
{
    [ObservableProperty] private ObservableCollection<AdaptersProfileSet> _networkProfilesSection = new();
}