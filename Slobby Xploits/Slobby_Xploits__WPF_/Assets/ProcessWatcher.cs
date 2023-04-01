using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace Slobby_Xploits__WPF_.Assets
{
	internal class ProcessWatcher : IDisposable
	{
		public event ProcessWatcher.OnProcessCreatedDelegate Created
		{
			[CompilerGenerated]
			add
			{
				ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate = this.Created;
				ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate2;
				do
				{
					onProcessCreatedDelegate2 = onProcessCreatedDelegate;
					ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate3 = (ProcessWatcher.OnProcessCreatedDelegate)Delegate.Combine(onProcessCreatedDelegate2, value);
					onProcessCreatedDelegate = Interlocked.CompareExchange<ProcessWatcher.OnProcessCreatedDelegate>(ref this.Created, onProcessCreatedDelegate3, onProcessCreatedDelegate2);
				}
				while (onProcessCreatedDelegate != onProcessCreatedDelegate2);
			}
			[CompilerGenerated]
			remove
			{
				ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate = this.Created;
				ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate2;
				do
				{
					onProcessCreatedDelegate2 = onProcessCreatedDelegate;
					ProcessWatcher.OnProcessCreatedDelegate onProcessCreatedDelegate3 = (ProcessWatcher.OnProcessCreatedDelegate)Delegate.Remove(onProcessCreatedDelegate2, value);
					onProcessCreatedDelegate = Interlocked.CompareExchange<ProcessWatcher.OnProcessCreatedDelegate>(ref this.Created, onProcessCreatedDelegate3, onProcessCreatedDelegate2);
				}
				while (onProcessCreatedDelegate != onProcessCreatedDelegate2);
			}
		}

		public ProcessWatcher(string processName)
		{
			this._processname = processName;
			this._timer = new System.Timers.Timer();
			this._timer.Elapsed += this.TimerOnElapsed;
			this._timer.Start();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName(this._processname);
			if (processesByName.Length != 1)
			{
				return;
			}
			this.OnProcessCreated(processesByName[0]);
		}

		protected virtual void OnProcessCreated(Process process)
		{
			this._timer.Stop();
			this._process = process;
			this._process.EnableRaisingEvents = true;
			this._process.Exited += delegate(object self, EventArgs e)
			{
				this._timer.Start();
			};
			ProcessWatcher.OnProcessCreatedDelegate created = this.Created;
			if (created == null)
			{
				return;
			}
			created(this, process);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing)
			{
				this._timer.Dispose();
			}
			this._disposed = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[CompilerGenerated]
		private void <OnProcessCreated>b__9_0(object self, EventArgs e)
		{
			this._timer.Start();
		}

		public readonly System.Timers.Timer _timer;

		private readonly string _processname;

		private bool _disposed;

		private Process _process;

		[CompilerGenerated]
		private ProcessWatcher.OnProcessCreatedDelegate Created;

		public delegate void OnProcessCreatedDelegate(object sender, Process process);
	}
}
