using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoGame.Utilities;

namespace Myra.Graphics2D.UI.File
{
    // === Platform dependent code for FileDialog class
    public partial class FileDialog
    {
        /// <summary>
        /// Platform specific code for <see cref="FileDialog"/>
        /// </summary>
        protected static class Platform
        {
            private static IReadOnlyList<string> _userPaths;
            /// <summary>
            /// Return a predetermined list of directories under the user's HOME folder, depending on OS.
            /// </summary>
            public static IReadOnlyList<string> SystemUserPlacePaths
            {
                get
                {
                    if (_userPaths == null)
                    {
                        switch (CurrentPlatform.OS)
                        {
                            case OS.Windows:
                                _userPaths = _GetWindowsPlaces();
                                break;
                            case OS.MacOSX:
                                _userPaths = _GetWindowsPlaces(); //TODO Mac/OSX specific - using old windows code for now!
                                break;
                            case OS.Linux:
                                _userPaths = _GetLinuxPlaces();
                                break;
                            default:
                                throw new PlatformNotSupportedException(CurrentPlatform.OS.ToString());
                        }
                    }
                    return _userPaths;
                }
            }

            private static string _homePath = string.Empty;
            public static string UserHomePath
            {
                get
                {
                    if (string.IsNullOrEmpty(_homePath))
                    {
                        _homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                            ? Environment.GetEnvironmentVariable("HOME")
                            : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                    }
                    return _homePath;
                }
            }
            
            private static string _osUser = string.Empty;
            /// <summary>
            /// Returns the name of the user logged into the system
            /// </summary>
            public static string SystemUsername
            {
                get
                {
                    if (string.IsNullOrEmpty(_osUser))
                        _osUser = UserHomePath.Split(new char[]{Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries).Last();
                    return _osUser;
                }
            }
            
            /// <summary>
            /// Append <see cref="Location"/> directories under the user's HOME directory.
            /// </summary>
            /// <param name="placeList">What folders to try to add relative to the HOME directory.</param>
            public static void AppendUserPlacesOnSystem(List<Location> appendResult, IReadOnlyList<string> placeList)
            {
                ThrowIfNull(appendResult);
                
                string homePath = UserHomePath;
                var places = new List<string>(placeList.Count);
                
                // Special label for HOME directory
                if (CurrentPlatform.OS != OS.Windows)
                    appendResult.Add(new Location("Home", SystemUsername, homePath, false ));
                else
                    places.Add(homePath);
                
                foreach (string folder in placeList)
                {
                    places.Add(Path.Combine(homePath, folder));
                }
                
                foreach (string path in places)
                {
                    if (!TryGetFileAttributes(path, out _))
                        continue;
                    
                    appendResult.Add(new Location(string.Empty, Path.GetFileName(path), path, false ));
                }
            }
            
            /// <summary>
            /// Append a list of <see cref="Location"/> for devices we can visit, depending on platform.
            /// </summary>
            /// <exception cref="PlatformNotSupportedException"></exception>
            public static void AppendDrivesOnSystem(List<Location> appendResult)
            {
                ThrowIfNull(appendResult);
                
                switch (CurrentPlatform.OS)
                {
                    case OS.Windows:
                        _GetWindowsDrives(appendResult);
                        return;
                    case OS.MacOSX:
                        _GetWindowsDrives(appendResult); //TODO Mac/OSX specific - using old windows code for now!
                        return;
                    case OS.Linux:
                        _GetLinuxDrives(appendResult);
                        return;
                    default:
                        throw new PlatformNotSupportedException(CurrentPlatform.OS.ToString());
                }
            }

#region Windows
            private static void _GetWindowsDrives(List<Location> appendResult)
            {
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
                        
                        appendResult.Add(new Location(vol, d.Name, d.RootDirectory.FullName, true));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            private static IReadOnlyList<string> _GetWindowsPlaces()
            {
                return new[] { "Desktop", "Downloads", "Documents", "Pictures" };
            }
#endregion Windows
#region Linux
            private static void _GetLinuxDrives(List<Location> appendResult)
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
                foreach (string deviceLine in bashResult)
                {
                    string[] splits = deviceLine.Split(new[] { ' ' }, StringSplitOptions.None);
                    
                    if(splits[0] != "part") //TYPE
                        continue; // We only want partitioned file systems.

                    splits[2] = splits[2].Replace(RawSpace, " "); //LABEL
                    if(string.Equals(splits[2], "System Reserved")) 
                        continue;
                    if (string.IsNullOrEmpty(splits[2]))
                    {
                        //Is this the main system partition?
                        if (string.Equals(splits[3], "/home") || string.Equals(splits[3], "/"))
                        {
                            appendResult.Add(new Location(splits[1], "Linux System", "/", true));
                            continue;
                        }
                    }
                    
                    if(string.IsNullOrEmpty(splits[3]) || string.Equals(splits[3], "/boot"))
                        continue;
                    splits[3] = splits[3].Replace(RawSpace, " "); //MOUNTPOINT
                    
                    appendResult.Add(new Location(splits[1], splits[2], splits[3], true));
                }
            }
            private static IReadOnlyList<string> _GetLinuxPlaces()
            {
                return new[] { "Desktop", "Downloads", "Documents", "Pictures" };
            }
#endregion Linux
            
            private static void ThrowIfNull(List<Location> obj)
            {
                if (obj == null)
                    throw new NullReferenceException();
            }
        }
    }
}