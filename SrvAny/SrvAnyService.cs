using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SrvAny
{
    public partial class SrvAnyService : ServiceBase
    {
        private CancellationTokenSource cancellationTokenSource;
        private Task task;

        public SrvAnyService()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("SrvAnyService"))
            {
                EventLog.CreateEventSource("SrvAnyService", "System");
            }
            this.srvAnyEventLog.Source = "SrvAnyService";
            this.srvAnyEventLog.Log = "System";
            this.srvAnyEventLog.WriteEntry($"{this.ServiceName} was initialized.", EventLogEntryType.Information);
        }

        protected override void OnStart(string[] args)
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            this.cancellationTokenSource = new CancellationTokenSource();
            var token = this.cancellationTokenSource.Token;
            this.task = this.Start(commandLineArgs[1], token);
        }

        private async Task Start(string prog_path, CancellationToken token) {
            await Task.Run(() =>
            {
                Process proc = null;
                try
                {
                    while (true)
                    {
                        var app = new ProcessStartInfo
                        {
                            FileName = prog_path,
                            UseShellExecute = true
                        };

                        if (proc is null)
                        {
                            this.srvAnyEventLog.WriteEntry($"{this.ServiceName} was started.", EventLogEntryType.Information);
                            proc = Process.Start(app);
                        }

                        Thread.Sleep(5000);

                        token.ThrowIfCancellationRequested();

                        if (proc.HasExited)
                        {
                            this.srvAnyEventLog.WriteEntry($"{this.ServiceName} was stopped abnormally. Try to restart.", EventLogEntryType.Error);
                            proc = null;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    bool is_closed = false;
                    try
                    {
                        if (!(proc is null))
                        {
                            is_closed = proc.CloseMainWindow();
                        }

                        if (!is_closed)
                        {
                            proc.Kill();
                        }

                        this.srvAnyEventLog.WriteEntry($"{this.ServiceName} was stopped normally.", EventLogEntryType.Information);
                    }
                    catch (Exception ex)
                    {
                        this.srvAnyEventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                    }
                }
                catch (Exception ex)
                {
                    this.srvAnyEventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                }
            });
        }

        protected override void OnStop()
        {
            this.cancellationTokenSource.Cancel();
            this.task.Wait();
            this.cancellationTokenSource = null;
        }
    }
}
