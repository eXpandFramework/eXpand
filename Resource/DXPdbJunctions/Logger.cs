using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DXPdbJunctions{
    public class Logger {
        public Logger() {
            _text = new BindingList<TextItem>();
        }
        static Logger _instance;
        public static Logger Instance {
            get { return _instance ?? (_instance = new Logger()); }
        }
        public string PlainText {
            get{
                return Text.Aggregate("", (current, item) => current + (item.Text + Environment.NewLine));
            }
        }

        readonly BindingList<TextItem> _text;

        public BindingList<TextItem> Text {
            get { return _text; }
        }

        public void AddText(string text, TextItemStatus status) {
            _text.Add(new TextItem{ Index = _text.Count + 1, Text = text, Status = status });
                SendLogMessage(_text[_text.Count - 1]);
        }
        public int SuccesfullyBuildProjectCount { get; set; }

        int _progressValue;

        public int ProgressValue {
            get { return _progressValue; }
            set {
                _progressValue = value;
                if (!ApplicationSettings.Instance.CommandPromptMode) {
                    MainForm.Instance.SetProgressValue(_progressValue);
                }
            }
        }
        int maxProgressValue;
        public int MaxProgressValue {
            get { return maxProgressValue; }
            set {
                maxProgressValue = value;
                if (!ApplicationSettings.Instance.CommandPromptMode)
                    MainForm.Instance.SetMaxProgressValue(maxProgressValue);
            }
        }
        void SendLogMessage(TextItem text) {
            if (ApplicationSettings.Instance.CommandPromptMode)
                Console.Write(text.Text + Environment.NewLine);
            else
                MainForm.Instance.UpdateLog(text);
        }

        internal void SaveToFile(string p) {
            using (StreamWriter sw = File.CreateText(p)) {
                sw.Write(Logger.Instance.PlainText);
                sw.Flush();
                sw.Close();
            }
        }
    }
    public class TextItem {
        public int Index { get; set; }
        public string Text { get; set; }
        public TextItemStatus Status { get; set; }
    }
    public enum TextItemStatus {
        Info,
        Warning,
        Error,
        Success
    }
}
