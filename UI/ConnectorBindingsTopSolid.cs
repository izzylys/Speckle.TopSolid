﻿using Speckle.Core.Api;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using Speckle.DesktopUI;
using Speckle.DesktopUI.Utils;
using Speckle.Newtonsoft.Json;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopSolid.Kernel.DB.D3.Curves;
using TopSolid.Kernel.DB.D3.Documents;
using TopSolid.Kernel.DB.D3.Modeling.Documents;
using TopSolid.Kernel.DB.D3.Shapes;
using TopSolid.Kernel.DB.Entities;
using TopSolid.Kernel.DB.Parameters;
using TopSolid.Kernel.G.D3;

namespace EPFL.SpeckleTopSolid.UI.LaunchCommand
{
    public partial class ConnectorBindingsTopSolid : ConnectorBindings
    {
        [Inject]
        private IEventAggregator _events;
        private static string SpeckleKey = "speckle";
        public List<Exception> Exceptions { get; set; } = new List<Exception>();
        public ConnectorBindingsTopSolid()
        {
            /*
            RhinoDoc.EndOpenDocument += RhinoDoc_EndOpenDocument;

            SelectionTimer = new Timer(2000) { AutoReset = true, Enabled = true };
            SelectionTimer.Elapsed += SelectionTimer_Elapsed;
            SelectionTimer.Start();
             */

        }

        internal void GetFileContextAndNotifyUI()
        {
            var streamStates = GetStreamsInFile();

            var appEvent = new ApplicationEvent()
            {
                Type = ApplicationEvent.EventType.DocumentOpened,
                DynamicInfo = streamStates
            };

            NotifyUi(appEvent);
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
            return new List<StreamState>();
        }

        GeometricDocument document = TopSolid.Kernel.UI.Application.CurrentDocument as ModelingDocument;



        public override void AddNewStream(StreamState state)
        {
            //TODO change the way it's done, eventually using the SpeckleStream Class
            //Create a text parameter to hold the Json string
            TopSolid.Kernel.TX.Undo.UndoSequence.UndoCurrent();
            TopSolid.Kernel.TX.Undo.UndoSequence.Start("Test", true);
            TextParameterEntity texte = new TextParameterEntity(document, 0);
            texte.Value = (JsonConvert.SerializeObject(state));
            texte.Name = "TestparamSpeckle";
            document.ParametersFolderEntity.AddEntity(texte);
            TopSolid.Kernel.TX.Undo.UndoSequence.End();


            //Doc.Strings.SetString(SpeckleKey, state.Stream.id, JsonConvert.SerializeObject(state));
        }

        public override void PersistAndUpdateStreamInFile(StreamState state)
        {
            //Update value of the text parameter in TS
            TopSolid.Kernel.TX.Undo.UndoSequence.UndoCurrent();
            TopSolid.Kernel.TX.Undo.UndoSequence.Start("Test", true);
            var a = document.ParametersFolderEntity.SearchEntity("TestparamSpeckle") as TextParameterEntity;
            a.Value = (JsonConvert.SerializeObject(state));
            TopSolid.Kernel.TX.Undo.UndoSequence.End();
        }
        public List<Exception> OperationErrors { get; set; } = new List<Exception>();
        public override async Task<StreamState> SendStream(StreamState state)
        {
            if (state.Filter != null)
            {
                state.SelectedObjectIds = GetSelectedObjects();
            }


            var commitObject = new Base();

            var streamId = state.Stream.id;
            var client = state.Client;

            //      
            var selectedObjects = new List<Entity>();

            //if (state.Filter != null)
            //{
            //  selectedObjects = GetSelectionFilterObjects(state.Filter);
            //  state.SelectedObjectIds = selectedObjects.Select(x => x.UniqueId).ToList();
            //}
            //else //selection was by cursor
            //{
            //  // TODO: update state by removing any deleted or null object ids

            //selectedObjects = state.SelectedObjectIds.Select(x => CurrentDoc.Document.GetElement(x)).Where(x => x != null).ToList();
            //selectedObjects = state.SelectedObjectIds.Select(TopSolid.Kernel.UI.Selections.CurrentSelections.GetSelectedEntities()).ToString().ToList();
            //}
            //       

            var transports = new List<ITransport>() { new ServerTransport(client.Account, streamId) };
            /* successful test for sending a topSolid line created by code ==> TODO same with line drawn graphically
            TopSolid.Kernel.G.D3.Point point1 = new TopSolid.Kernel.G.D3.Point(0, 0, 0);
            TopSolid.Kernel.G.D3.Point point2 = new TopSolid.Kernel.G.D3.Point(1, 1, 0);
            commitObject = ConvertersSpeckleTopSolid.LineToSpeckle(new TopSolid.Kernel.G.D3.Curves.LineCurve(point1, point2));
            */

            /* //successfully sent a BSpline curve that got converted into Rhino 
            //Getting the curve to send
            PositionedSketchEntity entity = (TopSolid.Kernel.UI.Application.CurrentDocument as ModelingDocument).SketchesFolderEntity.DeepPositionedSketches.First() as PositionedSketchEntity;
            BSplineCurve curve = new BSplineCurve();
            curve = entity.Geometry.Profiles.First().Segments.First().Geometry.GetBSplineCurve(false, false, TopSolid.Kernel.G.Precision.LinearPrecision);
            // commitObject   = ConvertersSpeckleTopSolid.CurveToSpeckle(curve);
            */


            ShapeEntity shape = (TopSolid.Kernel.UI.Application.CurrentDocument as ModelingDocument).ShapesFolderEntity.DeepEntities.First() as ShapeEntity;
            Box box = shape.Geometry.FindBox();
            //if (conversionResult != null)

            var category = "default";
            if (commitObject[category] == null)
            {
                commitObject[category] = new List<Base>();
            }

            ((List<Base>)commitObject[category]).Add(ConvertersSpeckleTopSolid.BoxToSpeckle(box));



            var objectId = await Operations.Send(
          @object: commitObject,
          cancellationToken: state.CancellationTokenSource.Token,
          transports: transports,
          //onProgressAction: dict => UpdateProgress(dict, state.Progress),
          onErrorAction: (s, e) =>
          {
              //OperationErrors.Add(e); // TODO!
              state.Errors.Add(e);
              state.CancellationTokenSource.Cancel();
          }
          );

            if (OperationErrors.Count != 0)
            {
                Globals.Notify("Failed to send.");
                state.Errors.AddRange(OperationErrors);
                return state;
            }

            if (state.CancellationTokenSource.Token.IsCancellationRequested)
            {
                return null;
            }

            var actualCommit = new CommitCreateInput()
            {
                streamId = streamId,
                objectId = objectId,
                branchName = state.Branch.name,
                message = state.CommitMessage != null ? state.CommitMessage : "Hello from TopSolid",//$"Sent {convertedCount} objects from {ConnectorRevitUtils.RevitAppName}.",
                sourceApplication = TopSolid.Kernel.UI.Application.Name,
            };

            if (state.PreviousCommitId != null) { actualCommit.parents = new List<string>() { state.PreviousCommitId }; }

            try
            {
                var commitId = await client.CommitCreate(actualCommit);

                await state.RefreshStream();
                state.PreviousCommitId = commitId;



                //TO DO : Add the Objects Count
                //WriteStateToFile();
                RaiseNotification($" *insert count* objects sent to Speckle 🚀");
            }
            catch (Exception e)
            {
                state.Errors.Add(e);
                Globals.Notify($"Failed to create commit.\n{e.Message}");
            }

            return state;

        }

        //Copied Method from the Autocad Connector, was needed for the Sendstream
        private void UpdateProgress(ConcurrentDictionary<string, int> dict, ProgressReport progress)
        {
            if (progress == null)
            {
                return;
            }

            Execute.PostToUIThread(() =>
            {
                progress.ProgressDict = dict;
                progress.Value = dict.Values.Last();
            });
        }
        //Copied Method from the Autocad Connector, was needed for the Sendstream
        private List<Tuple<Base, string>> FlattenCommitObject(object obj, ISpeckleConverter converter, string layer, StreamState state, ref int count, bool foundConvertibleMember = false)
        {
            var objects = new List<Tuple<Base, string>>();

            if (obj is Base @base)
            {
                //if (converter.CanConvertToNative(@base))
                //{
                //    objects.Add(new Tuple<Base, string>(@base, layer));
                //    return objects;
                //}
                //else
                {
                    int totalMembers = @base.GetDynamicMembers().Count();
                    foreach (var prop in @base.GetDynamicMembers())
                    {
                        count++;

                        // get bake layer name
                        string objLayerName = prop.StartsWith("@") ? prop.Remove(0, 1) : prop;
                        string acLayerName = $"{layer}${objLayerName}";

                        var nestedObjects = FlattenCommitObject(@base[prop], converter, acLayerName, state, ref count, foundConvertibleMember);
                        if (nestedObjects.Count > 0)
                        {
                            objects.AddRange(nestedObjects);
                            foundConvertibleMember = true;
                        }
                    }
                    if (!foundConvertibleMember && count == totalMembers) // this was an unsupported geo
                        state.Errors.Add(new Exception($"Receiving {@base.speckle_type} objects is not supported. Object {@base.id} not baked."));
                    return objects;
                }
            }

            else return objects;
        }
        public override async Task<StreamState> ReceiveStream(StreamState state)
        {
            Exceptions.Clear();

            //var kit = KitManager.GetDefaultKit();
            //var converter = kit.LoadConverter(Utils.AutocadAppName);
            var transport = new ServerTransport(state.Client.Account, state.Stream.id);

            var stream = await state.Client.StreamGet(state.Stream.id);

            if (state.CancellationTokenSource.Token.IsCancellationRequested)
            {
                return null;
            }

            string referencedObject = state.Commit.referencedObject;
            string id = state.Commit.id;

            //if "latest", always make sure we get the latest commit when the user clicks "receive"
            if (id == "latest")
            {
                var res = await state.Client.BranchGet(state.CancellationTokenSource.Token, state.Stream.id, state.Branch.name, 1);
                referencedObject = res.commits.items.FirstOrDefault().referencedObject;
                id = res.id;
            }

            //var commit = state.Commit;

            var commitObject = await Operations.Receive(
              referencedObject,
              state.CancellationTokenSource.Token,
              transport,
              onProgressAction: d => UpdateProgress(d, state.Progress),
              onTotalChildrenCountKnown: num => Execute.PostToUIThread(() => state.Progress.Maximum = num),
              onErrorAction: (message, exception) => { Exceptions.Add(exception); }
              );

            if (Exceptions.Count != 0)
            {
                RaiseNotification($"Encountered error: {Exceptions.Last().Message}");
            }

            int count = 0;
            string layerPrefix = " ";
            ISpeckleConverter converter = null;
            var commitObjs = FlattenCommitObject(commitObject, converter, layerPrefix, state, ref count);
            foreach (var commitObj in commitObjs)
            {
                // create the object's bake layer if it doesn't already exist
                (Base obj, string layerName) = commitObj;



                TopSolid.Kernel.TX.Undo.UndoSequence.UndoCurrent();
                TopSolid.Kernel.TX.Undo.UndoSequence.Start("Test", true);
                //TextParameterEntity texte = new TextParameterEntity(document, 0);
                //texte.Value = (JsonConvert.SerializeObject(state));
                //texte.Name = "TestparamSpeckle";
                //document.ParametersFolderEntity.AddEntity(texte);


                //Polyline poly = @obj;

                var converted = ConvertersSpeckleTopSolid.PolyLinetoTS((Objects.Geometry.Polyline)obj);



                CurveEntity convertedEntity = new CurveEntity(document, 0);
                convertedEntity.Geometry = converted;
                int c = 0;
                string name = "SpeckleProfile";
                if ((TopSolid.Kernel.UI.Application.CurrentDocument as ModelingDocument).ShapesFolderEntity.SearchEntity(name) != null)
                {
                    convertedEntity.Name = name + c.ToString();
                    c++;
                }
                else convertedEntity.Name = name;



                TopSolid.Kernel.TX.Undo.UndoSequence.End();


                //if (convertedEntity != null)
                //{
                //    if (GetOrMakeLayer(layerName, tr, out string cleanName))
                //    {
                //        // record if layer name has been modified
                //        if (!cleanName.Equals(layerName))
                //            changedLayerNames = true;

                //        if (!convertedEntity.Append(cleanName, tr, btr))
                //            state.Errors.Add(new Exception($"Failed to bake object {obj.id} of type {obj.speckle_type}."));
                //    }
                //    else
                //        state.Errors.Add(new Exception($"Could not create layer {layerName} to bake objects into."));
                //}
                //else if (converted == null)
                //{
                //    state.Errors.Add(new Exception($"Failed to convert object {obj.id} of type {obj.speckle_type}."));
                //}
            }

            //using (DocumentLock l = Doc.LockDocument())
            {/*Autocad Blabla
                using (AcadDb.Transaction tr = Doc.Database.TransactionManager.StartTransaction())
                {
                    // set the context doc for conversion - this is set inside the transaction loop because the converter retrieves this transaction for all db editing when the context doc is set!
                    converter.SetContextDocument(Doc);

                    // keep track of conversion progress here
                    var conversionProgressDict = new ConcurrentDictionary<string, int>();
                    conversionProgressDict["Conversion"] = 0;
                    Execute.PostToUIThread(() => state.Progress.Maximum = state.SelectedObjectIds.Count());
                    Action updateProgressAction = () =>
                    {
                        conversionProgressDict["Conversion"]++;
                        UpdateProgress(conversionProgressDict, state.Progress);
                    };

                    // keep track of any layer name changes for notification here
                    bool changedLayerNames = false;

                    // create a commit layer prefix: all nested layers will be concatenated with this
                    var layerPrefix = DesktopUI.Utils.Formatting.CommitInfo(stream.name, state.Branch.name, id);

                    // give converter a way to access the commit info
                    Doc.UserData.Add("commit", layerPrefix);

                    // delete existing commit layers
                    try
                    {
                        DeleteLayersWithPrefix(layerPrefix, tr);
                    }
                    catch
                    {
                        RaiseNotification($"could not remove existing layers starting with {layerPrefix} before importing new geometry.");
                        state.Errors.Add(new Exception($"could not remove existing layers starting with {layerPrefix} before importing new geometry."));
                    }
                
                    // flatten the commit object to retrieve children objs
                    int count = 0;
                    var commitObjs = FlattenCommitObject(commitObject, converter, layerPrefix, state, ref count);

                    // open model space block table record for write
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(Doc.Database.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (var commitObj in commitObjs)
                    {
                        // create the object's bake layer if it doesn't already exist
                        (Base obj, string layerName) = commitObj;

                        var converted = converter.ConvertToNative(obj);
                        var convertedEntity = converted as Entity;

                        if (convertedEntity != null)
                        {
                            if (GetOrMakeLayer(layerName, tr, out string cleanName))
                            {
                                // record if layer name has been modified
                                if (!cleanName.Equals(layerName))
                                    changedLayerNames = true;

                                if (!convertedEntity.Append(cleanName, tr, btr))
                                    state.Errors.Add(new Exception($"Failed to bake object {obj.id} of type {obj.speckle_type}."));
                            }
                            else
                                state.Errors.Add(new Exception($"Could not create layer {layerName} to bake objects into."));
                        }
                        else if (converted == null)
                        {
                            state.Errors.Add(new Exception($"Failed to convert object {obj.id} of type {obj.speckle_type}."));
                        }
                    }

                    // raise any warnings from layer name modification
                    if (changedLayerNames)
                        state.Errors.Add(new Exception($"Layer names were modified: one or more layers contained invalid characters {Utils.invalidChars}"));

                    // remove commit info from doc userdata
                    Doc.UserData.Remove("commit");

                    tr.Commit();
                }*/


            }

            return state;
        }

        public override List<string> GetSelectedObjects()
        {
            List<string> Objs = new List<string>();
            Objs.Add(document.AbsoluteOriginPointEntity.Id.ToString());
            return Objs;
            //var objs = Doc?.Objects.GetSelectedObjects(true, false).Select(obj => obj.Id.ToString()).ToList();
            //return objs;

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
            //Copied from Revit 
            return new List<ISelectionFilter>()
            {
                new ListSelectionFilter {
                Name = "Category", Icon = "Category", Description = "Hello world. This is a something something filter.", Values = new List<string>() { "Boats", "Rafts", "Barges" }
            }
            };

            //copied from Rhino
            /*var layers = Doc.Layers.ToList().Select(layer => layer.Name).ToList();

             return new List<ISelectionFilter>()
             {
                 new ListSelectionFilter { Name = "Layers", Icon = "Filter", Description = "Selects objects based on their layers.", Values = layers }
             };
            */

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
