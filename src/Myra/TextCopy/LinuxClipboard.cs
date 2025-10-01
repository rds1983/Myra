using System;
using System.IO;

static class LinuxClipboard
{
    private enum ClipCmd
    {
        Incompatible,
        XClip,
        XSel, 
    }
    private static ClipCmd Cmd = ClipCmd.Incompatible;
    
    public static string GetCompatibilityProblem()
    {
        return @"Myra: At least one package needs to be installed on the system: 'xclip', 'xsel'. Clipboard function will be limited.";
    }
    
    /// <summary>
    /// Return true if 'xclip' or 'xsel' is installed on the system.
    /// </summary>
    public static bool SystemIsCompatible()
    {
        ClipCmd clipCmd = ClipCmd.Incompatible;

        if(TestBashCommandExists("xsel --version")) //dash differences are intentional
        {
            clipCmd = ClipCmd.XSel;
        }
        else if (TestBashCommandExists("xclip -version"))
        {
            clipCmd = ClipCmd.XClip;
        }
        
        Cmd = clipCmd;
        return Cmd != ClipCmd.Incompatible;
    }
    private static bool TestBashCommandExists(string cmd)
    {
        string bashMsg;
        try
        {
            bashMsg = BashRunner.Run(cmd);
        }
        catch (Exception e)
        {
            bashMsg = e.Message;
        }
        return !bashMsg.Contains("Could not execute process.");
    }
    
    public static void SetText(string text)
    {
        var tempFileName = Path.GetTempFileName();
        File.WriteAllText(tempFileName, text);
        try
        {
            switch (Cmd)
            {
                case ClipCmd.XClip:
                    BashRunner.Run($"cat {tempFileName} | xclip -sel clip");
                    break;
                case ClipCmd.XSel:
                    BashRunner.Run($"cat {tempFileName} | xsel -ib");
                    break;
                case ClipCmd.Incompatible:
                    throw new SystemException(GetCompatibilityProblem());
                default:
                    throw new NotImplementedException(Cmd.ToString());
            }
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }
    
    public static string GetText()
    {
        var tempFileName = Path.GetTempFileName();
        try
        {
            switch (Cmd)
            {
                case ClipCmd.XClip:
                    BashRunner.Run($"xclip -o -sel clip > {tempFileName}");
                    break;
                case ClipCmd.XSel:
                    BashRunner.Run($"xsel -ob > {tempFileName}");
                    break;
                case ClipCmd.Incompatible:
                    throw new SystemException(GetCompatibilityProblem());
                default:
                    throw new NotImplementedException(Cmd.ToString());
            }
            return File.ReadAllText(tempFileName);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }
}