using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WindowsNetworkManager.Core.Models;

public partial class AdaptersProfileSet : ObservableObject
{
    [ObservableProperty] private string _profileName = "";
    public ObservableCollection<NetworkAdapterChange> ProfileSet { get; set; }= new();
}