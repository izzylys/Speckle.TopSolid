﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Speckle.Newtonsoft.Json;
using Speckle.DesktopUI.Utils;
using Stylet;
using StyletIoC;
using Speckle.DesktopUI;

namespace EPFL.SpeckleTopSolid.UI.LaunchCommand
{
  public partial class ConnectorBindingsTopSolid : ConnectorBindings
  {
    [Inject]
    private IEventAggregator _events;

    public ConnectorBindingsTopSolid() { 
        

        }

        internal void GetFileContextAndNotifyUI()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends an event to the UI. The event types are pre-defined and inherit from EventBase.
        /// </summary>
        /// <param name="notifyEvent">The event to be published</param>

        //public virtual void NotifyUi(EventBase notifyEvent)
        //{
        //  //TODO: checked why it's null sometimes
        //  if(_events!=null)
        //    _events.PublishOnUIThread(notifyEvent);
        //}

        /// <summary>
        /// Raise a toast notification which is shown in the bottom of the main UI window.
        /// </summary>
        /// <param name="message">The body of the notification</param>

        //public virtual void RaiseNotification(string message)
        //{
        //    var notif = new ShowNotificationEvent() { Notification = message };
        //    NotifyUi(notif);
        //}

        //public virtual bool CanSelectObjects()
        //{
        //    return false;
        //}

        //public virtual bool CanTogglePreview()
        //{
        //    return false;
        //}

        #region abstract methods

        /// <summary>
        /// Gets the current host application name.
        /// </summary>
        /// <returns></returns>
        public override string GetHostAppName() => TopSolid.Kernel.UI.Application.Name;

        /// <summary>
        /// Gets the current opened/focused file's name.
        /// Make sure to check regarding unsaved/temporary files.
        /// </summary>
        /// <returns></returns>
        public override string GetFileName() => TopSolid.Kernel.UI.Application.CurrentDocument.Name.ToString();

    /// <summary>
    /// Gets the current opened/focused file's id. 
    /// Generate one in here if the host app does not provide one.
    /// </summary>
    /// <returns></returns>
    public override string GetDocumentId() => TopSolid.Kernel.UI.Application.CurrentDocument.PdmDocumentId;

    /// <summary>
    /// Gets the current opened/focused file's locations.
    /// Make sure to check regarding unsaved/temporary files.
    /// </summary>
    /// <returns></returns>
    public override string GetDocumentLocation() => TopSolid.Kernel.UI.Application.CurrentDocument.FilePath;

    /// <summary>
    /// Gets the current opened/focused file's view, if applicable.
    /// </summary>
    /// <returns></returns>
    public override string GetActiveViewName() => TopSolid.Kernel.UI.Application.ActiveDocumentWindow.Name;

        public override List<StreamState> GetStreamsInFile()
        {
            throw new NotImplementedException();
        }

        public override void AddNewStream(StreamState state)
        {
            throw new NotImplementedException();
        }

        public override void PersistAndUpdateStreamInFile(StreamState state)
        {
            throw new NotImplementedException();
        }

        public override Task<StreamState> SendStream(StreamState state)
        {
            throw new NotImplementedException();
        }

        public override Task<StreamState> ReceiveStream(StreamState state)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetSelectedObjects()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetObjectsInView()
        {
            throw new NotImplementedException();
        }

        public override void RemoveStreamFromFile(string streamId)
        {
            throw new NotImplementedException();
        }

        public override void SelectClientObjects(string args)
        {
            throw new NotImplementedException();
        }

        public override List<ISelectionFilter> GetSelectionFilters()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the serialised clients present in the current open host file.
        /// </summary>
        /// <returns></returns>
        //public void List<StreamState> GetStreamsInFile();

        /// <summary>
        /// Adds a new client and persists the info to the host file
        /// </summary>

        //public abstract void AddNewStream(StreamState state);

        /// <summary>
        /// Persists the stream info to the host file; if maintaining a local in memory copy, make sure to update it too.
        /// </summary>

        //public abstract void PersistAndUpdateStreamInFile(StreamState state);

        /// <summary>
        /// Pushes a client's stream
        /// </summary>
        /// <param name="state"></param>
        /// <param name="progress"></param>

        //public abstract Task<StreamState> SendStream(StreamState state);

        /// <summary>
        /// Receives stream data from the server
        /// </summary>
        /// <param name="state"></param>
        /// <param name="progress"></param>
        /// <returns></returns>

        //public abstract Task<StreamState> ReceiveStream(StreamState state);

        /// <summary>
        /// Adds the current selection to the provided client.
        /// </summary>
        //public abstract List<string> GetSelectedObjects();

        ///// <summary>
        ///// Gets a list of objects in the currently active view
        ///// </summary>
        ///// <returns></returns>
        //public abstract List<string> GetObjectsInView();

        ///// <summary>
        ///// Removes a client from the file and updates the host file.
        ///// </summary>
        ///// <param name="args"></param>
        //public abstract void RemoveStreamFromFile(string streamId);

        ///// <summary>
        ///// clients should be able to select/preview/hover one way or another their associated objects
        ///// </summary>
        ///// <param name="args"></param>
        //public abstract void SelectClientObjects(string args);

        /// <summary>
        /// Should return a list of filters that the application supports. 
        /// </summary>
        /// <returns></returns>
        //public abstract List<ISelectionFilter> GetSelectionFilters();

        #endregion
    }
}
