using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Utilities;

namespace Myra.Graphics2D.UI.File
{
    // === Platform dependent code for FileDialog class
    public partial class FileDialog
    {
        private static class Platform
        {
            public static void InitListedSystemDrives(IList<Widget> widgets)
            {
                switch (CurrentPlatform.OS)
                {
                    case OS.Windows:
                        InitSysDrivesWindows(widgets);
                        return;
                    case OS.MacOSX:
                        //TODO mac specific - using old windows code for now!
                        InitSysDrivesWindows(widgets);
                        return;
                    case OS.Linux:
                        InitSysDrivesLinux(widgets);
                        return;
                }
            }

#region Windows
            private static void InitSysDrivesWindows(IList<Widget> widgets)
            {
                var drives = GetWindowsDevices();
                foreach (var d in drives)
                {
                    try
                    {
                        var s = d.RootDirectory.FullName;

                        if (!string.IsNullOrEmpty(d.VolumeLabel) && d.VolumeLabel != d.RootDirectory.FullName) //VolumeLabel is only supported on win
                        {
                            s += " (" + d.VolumeLabel + ")";
                        }

                        var item = CreateListItem(s, d.RootDirectory.FullName, true);
                        widgets.Add(item);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            private static List<DriveInfo> GetWindowsDevices()
            {
                bool TestDriveType(DriveType type)
                {
                    switch (type)
                    {
                        case DriveType.CDRom:
                        case DriveType.Fixed:
                        case DriveType.Network:
                        case DriveType.Removable:
                            return true;
                        case DriveType.NoRootDirectory:
                        case DriveType.Unknown:
                        case DriveType.Ram:
                        default:
                            return false;
                    }
                }
                
                List<DriveInfo> result = new List<DriveInfo>(8);
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if(TestDriveType(drive.DriveType))
                        result.Add(drive);
                }
                return result;
            }
#endregion Windows
#region Linux
            private static void InitSysDrivesLinux(IList<Widget> widgets)
            {
                // Use bash 'lsblk' to collect device info
                var drives = GetLinuxDevices();
                
                foreach (var d in drives)
                {
                    widgets.Add(CreateListItem($"({d.VolumeLabel}) {d.Label}", d.Path, true));
                }
            }

            private static List<DeviceInfo> GetLinuxDevices()
            {
                const string RawSpace = @"\x20";
                string tmpFileName = Path.GetTempFileName();
                string[] bashResult;
                try
                {
                    BashRunner.Run($"lsblk -n -o TYPE,NAME,LABEL,MOUNTPOINT --raw > {tmpFileName}"); //MODE,
                    bashResult = System.IO.File.ReadAllLines(tmpFileName);
                }
                finally
                {
                    System.IO.File.Delete(tmpFileName);
                }
                
                List<DeviceInfo> result = new List<DeviceInfo>(8);
                foreach (string deviceLine in bashResult)
                {
                    string[] splits = deviceLine.Split(new[] { ' ' }, StringSplitOptions.None);
                    
                    if(splits[0] != "part") //TYPE
                        continue; // We only want partitioned for file systems.

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
            private class DeviceInfo
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
        }

    }
}