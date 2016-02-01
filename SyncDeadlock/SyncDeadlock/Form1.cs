using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        int _uiThreadId;
        TaskFactory _uiTaskFactory;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _uiThreadId = Environment.CurrentManagedThreadId;
            _uiTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            Text = "UI thread id: " + _uiThreadId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sync(ShowMessage, Environment.CurrentManagedThreadId).Wait();
        }

        void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        async Task Sync(Action<string> action, int threadId)
        {
            var threadInfo = ", thread id: " + threadId;

            if (_uiThreadId == Environment.CurrentManagedThreadId)
                action("In main thread" + threadInfo);
            else
                await _uiTaskFactory.StartNew(() => action("Call via TaskFactory" + threadInfo));
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Delay(1).ConfigureAwait(false);

            Sync(ShowMessage, Environment.CurrentManagedThreadId).Wait();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Deadlock().Wait();
        }

        async Task Deadlock()
        {
            await _uiTaskFactory.StartNew(() => { });
        }
    }
}
