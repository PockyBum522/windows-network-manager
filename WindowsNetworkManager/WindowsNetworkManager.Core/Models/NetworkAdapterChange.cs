using System;
using System.Transactions;
using WindowsNetworkManager.Core.Models.Enums;

namespace WindowsNetworkManager.Core.Models;

public class NetworkAdapterChange
{
    public NetworkAdapterChange(ChangeTaskEnum changeTaskEnum = ChangeTaskEnum.Uninitialized)
    {
        SetFromEnum(changeTaskEnum);
    }
    
    public string Name { get; set; } = "";
    
    public bool TaskIsEnable { get; set; }
    public bool TaskIsDisable { get; set; }
    public bool TaskIsNoChange { get; set; }

    public void SetFromEnum(ChangeTaskEnum taskToSet)
    {
        switch (taskToSet)
        {
            case ChangeTaskEnum.Uninitialized:
                break;
            
            case ChangeTaskEnum.Enable:
                TaskIsEnable = true;
                TaskIsDisable = false;
                TaskIsNoChange = false;
                break;
            
            case ChangeTaskEnum.Disable:
                TaskIsEnable = false;
                TaskIsDisable = true;
                TaskIsNoChange = false;
                break;
            
            case ChangeTaskEnum.NoChange:
                TaskIsEnable = false;
                TaskIsDisable = false;
                TaskIsNoChange = true;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(taskToSet), taskToSet, null);
        }
    }
}