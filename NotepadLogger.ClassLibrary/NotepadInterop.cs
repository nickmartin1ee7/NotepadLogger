using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NotepadLogger.ClassLibrary
{
    public class NotepadInterop : IDisposable
    {
        private const int WM_SETTEXT = 0x000C;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;
        private const int EM_SETSEL = 0x00B1;
        private const int EM_REPLACESEL = 0x00C2; 

        private Process _notepadProcess;

        private IntPtr _mainWindowHandle
        {
            get
            {
                _notepadProcess.WaitForInputIdle();
                return _notepadProcess.MainWindowHandle;
            }
        }

        private IntPtr _editWindowHandle => FindWindowEx(_mainWindowHandle, IntPtr.Zero, "Edit", null);

        #region Interop Externs

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);
        
        #endregion

        public NotepadInterop()
        {
            _notepadProcess = Process.Start("notepad.exe");
        }

        ~NotepadInterop()
        {
            Dispose();
        }

        public void SetText(string text)
        {
            var result = SendMessage(_editWindowHandle, WM_SETTEXT, 0, text);

            if (result != 1)
            {
                throw new ApplicationException($"Failed to set notepad text to: {text}{Environment.NewLine}Error number: {result}");
            }
        }
        
        public void AppendText(string text)
        {
            var bufferLength = SendMessage(_editWindowHandle, WM_GETTEXTLENGTH, 0, null);
            SendMessage(_editWindowHandle, EM_SETSEL, (IntPtr)bufferLength, (IntPtr)bufferLength);
            var result = SendMessage(_editWindowHandle, EM_REPLACESEL, 1, text);

            if (result != 1)
            {
                throw new ApplicationException($"Failed to set notepad text to: {text}{Environment.NewLine}Error number: {result}");
            }
        }
        
        public void Dispose()
        {
            _notepadProcess.Kill();
            _notepadProcess.Dispose();
        }
    }
}