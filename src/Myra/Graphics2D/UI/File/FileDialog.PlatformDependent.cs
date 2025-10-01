using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Utilities;

namespace Myra.Graphics2D.UI.File
{
    // === Platform dependent code for FileDialog class
    public partial class FileDialog
    {
        /// <summary>
        /// Container for info about a browsable file system device.
        /// </summary>
        public readonly struct DeviceInfo
        {
            public DeviceInfo(string volume, string label, string path)
            {
                VolumeLabel = volume;
                Label = label;
                Path = path;
            }
            
            public readonly string VolumeLabel;
            public readonly string Label;
            public readonly string Path;
        }
        
        /// <summary>
        /// Platform specific code
        /// </summary>
        private static class Platform
        {
            /// <summary>
            /// Return a list of <see cref="DeviceInfo"/> for devices we can visit, depending on platform.
            /// </summary>
            /// <exception cref="PlatformNotSupportedException"></exception>
            public static List<DeviceInfo> GetDrivesOnSystem()
            {
                switch (CurrentPlatform.OS)
                {
                    case OS.Windows:
                        return _GetWindowsDevices();
                    case OS.MacOSX:
                        return _GetWindowsDevices(); //TODO Mac/OSX specific - using old windows code for now!
                    case OS.Linux:
                        return _GetLinuxDevices();
                    default:
                        throw new PlatformNotSupportedException(CurrentPlatform.OS.ToString());
                }
            }

#region Windows
            private static List<DeviceInfo> _GetWindowsDevices()
            {
                List<DeviceInfo> devices = new List<DeviceInfo>(8);
                foreach (DriveInfo d in DriveInfo.GetDrives())
                {
                    switch (d.DriveType)
                    {
                        case DriveType.CDRom: //Acceptable
                        case DriveType.Fixed:
                        case DriveType.Network:
                        case DriveType.Removable:
                            break;
                        case DriveType.NoRootDirectory: //Skip These
                        case DriveType.Unknown:
                        case DriveType.Ram:
                        default:
                            continue;
                    }

                    try
                    {
                        string vol = string.Empty;
                        if (!string.IsNullOrEmpty(d.VolumeLabel) && d.VolumeLabel != d.RootDirectory.FullName)
                            vol = d.VolumeLabel;
                        
                        devices.Add(new DeviceInfo(vol, d.Name, d.RootDirectory.FullName));
                    }
                    catch (Exception)
                    {
                    }
                }
                return devices;
            }
#endregion Windows
#region Linux
            private static List<DeviceInfo> _GetLinuxDevices()
            {
                string tmpFileName = Path.GetTempFileName();
                string[] bashResult;
                try
                {
                    // The all caps words after o directly corelate to output string indexes. Some strings may return empty.
                    BashRunner.Run($"lsblk -no TYPE,NAME,LABEL,MOUNTPOINT --raw > {tmpFileName}");
                    bashResult = System.IO.File.ReadAllLines(tmpFileName);
                }
                finally
                {
                    System.IO.File.Delete(tmpFileName);
                }
                
                const string RawSpace = @"\x20";
                
                List<DeviceInfo> result = new List<DeviceInfo>(8);
                foreach (string deviceLine in bashResult)
                {
                    string[] splits = deviceLine.Split(new[] { ' ' }, StringSplitOptions.None);
                    
                    if(splits[0] != "part") //TYPE
                        continue; // We only want partitioned file systems.

                    splits[2] = splits[2].Replace(RawSpace, " ");
                    if(string.Equals(splits[2], "System Reserved")) //LABEL
                        continue;
                    
                    if(string.IsNullOrEmpty(splits[3]) || string.Equals(splits[3], "/boot"))
                        continue;
                    splits[3] = splits[3].Replace(RawSpace, " ");; //MOUNTPOINT
                    
                    result.Add(new DeviceInfo(splits[1], splits[2], splits[3]));
                }
                return result;
            }
#endregion Linux
            

        }
    }
}