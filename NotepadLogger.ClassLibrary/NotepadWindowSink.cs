using System;
using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace NotepadLogger.ClassLibrary
{
    public class NotepadWindowSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NotepadInterop _notepadInterop;

        public NotepadWindowSink(IFormatProvider formatProvider) : this()
        {
            _formatProvider = formatProvider;
        }

        public NotepadWindowSink()
        {
            _notepadInterop = new NotepadInterop();
        }

        ~NotepadWindowSink()
        {
            _notepadInterop.Dispose();
        }
        
        public void Emit(LogEvent logEvent)
        {
            _notepadInterop.AppendText(logEvent.RenderMessage(_formatProvider));
        }
    }
}