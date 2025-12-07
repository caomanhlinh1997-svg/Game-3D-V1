using UnityEngine;

public enum PlatformType
{
    PC,
    Mobile,
    Console
}

public class PlatformConfig : ScriptableObject
{
    public PlatformType CurrentPlatform = PlatformType.PC;
}
