using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Barotrauma_Circuit_Resolver.Util
{
    internal static class FormUtil
    {
        public static string ShowFolderBrowserDialog()
        {
            using var dialog = new CommonOpenFileDialog
                               {
                                   IsFolderPicker = true,
                                   EnsureFileExists = true,
                                   EnsurePathExists = true
                               };
            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : "";
        }

        public static string ShowFolderBrowserDialog(string defaultDirectory)
        {
            using var dialog = new CommonOpenFileDialog
                               {
                                   IsFolderPicker = true,
                                   EnsureFileExists = true,
                                   EnsurePathExists = true,
                                   DefaultDirectory = defaultDirectory
                               };
            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : "";
        }

        public static string ShowFileBrowserDialog(string extension = "")
        {
            using var dialog = new CommonOpenFileDialog
                               {
                                   EnsureFileExists = true,
                                   EnsurePathExists = true
                               };
            dialog.FileOk += (sender, parameter) =>
                             {
                                 var commonOpenFileDialog = (CommonOpenFileDialog) sender;
                                 var filenames = new Collection<string>();
                                 typeof(CommonOpenFileDialog)
                                     .GetMethod("PopulateWithFileNames", BindingFlags.Instance | BindingFlags.NonPublic)
                                     ?.Invoke(commonOpenFileDialog, new object[] {filenames});
                                 string filename = filenames[0];
                                 if (extension != "" && Path.GetExtension(filename) != extension)
                                 {
                                     parameter.Cancel = true;
                                     MessageBox.Show($"The selected file does not have the extension {extension}.",
                                                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                 }
                             };
            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : "";
        }

        public static Image GetImageFromString(string s)
        {
            byte[] bytes = Convert.FromBase64String(s);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
    }
}
