using FeederNetInspcetor.Model;
using FeederNetInspector.Classes;
using FeederNetInspector.UI;
using FeederNetInspector.Utils;
using Fiddler;
using Fiddler.WebFormats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using static Fiddler.WebFormats.JSON;

[assembly: RequiredVersion("5.0.0.0")]

namespace FeederNetInspector
{
    public class Main : IAutoTamper //IFiddlerExtension //IAutoTamper
    {
        public static Container container;


        /// <summary>
        /// Called when Fiddler is shutting down
        /// </summary>
        public void OnBeforeUnload()
        {
            /*noop*/
        }

        /// <summary>
        /// Called when Fiddler User Interface is fully available
        /// </summary>
        public void OnLoad()
        {
            /* Load our UI: FeederNetInspector tab */
            TabPage page = new TabPage("FeederNetInspector");
            page.ImageIndex = (int)Fiddler.SessionIcons.Inspector;
            FiddlerApplication.UI.tabsViews.TabPages.Add(page);
            
            ElementHost element = new ElementHost();
            container = new Container();
            element.Child = container;
            element.Dock = DockStyle.Fill;

            page.Controls.Add(element);

            //Register an event handdler to CalculateReport event
            FiddlerApplication.CalculateReport += new CalculateReportHandler(DisplayResults);
        }

        /// <summary>
        /// An event handdler to summary the selected sessions
        /// Fire when user selects a session or switch tab
        /// If no session is selected, oSessions is an empty array
        /// </summary>
        private void DisplayResults(Session[] oSessions)
        {
            // If we're not at FeederNetInspector tab right now, reset our UI and bail out
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                container.Reset();
                if (oSessions.Length > 0)
                {
                    UnselectSessions(oSessions);
                }
                return;
            }
            // Filter sessions in the list view, keep only sessions that contain host name.
            FilterSessionListByHostName(container.GetHostName());

            try
            {
                if (oSessions.Length < 1)
                {
                    return;
                }
                Tuple<List<Request>, List<Request>> captureOutputTuple = getCaptureOutputTuple(oSessions);
                RequestSessionModel requestSessionModel = new RequestSessionModel(captureOutputTuple.Item1);
                //ResponseSessionModel responseSessionModel = new ResponseSessionModel(captureOutputTuple.Item2);
                container.SetTbRequests(requestSessionModel);
                //container.SetTbResponses(responseSessionModel);
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }
        }

        /// <summary>
        /// Unselect sessions
        /// </summary>
        public static void UnselectSessions(Session[] oSessions)
        {
            foreach (Session oSession in oSessions)
            {
                oSession.ViewItem.Selected = false;
            }
        }

        /// <summary>
        /// Filter sessions in the list view, keep only sessions that contain host name
        /// </summary>
        private void FilterSessionListByHostName(string hostName)
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                // 3 -> index of column host name
                if (!item.SubItems[3].Text.Contains(hostName))
                {
                    lvItems.Remove(item);
                }
            }
        }

        /*private static void UpdateUi()
        {
            container.ToggleLabelLoading();
            Tuple<string, string> captureOutputTuple = getCaptureOutputTuple(oSessions);
            RequestSessionModel requestSessionModel = new RequestSessionModel();
            ResponseSessionModel responseSessionModel = new ResponseSessionModel();
            requestSessionModel.RequestBody = captureOutputTuple.Item1;
            responseSessionModel.ResponseBody = captureOutputTuple.Item2;
            container.SetTbRequests(requestSessionModel);
            container.SetTbResponses(responseSessionModel);
            container.ToggleLabelLoading();
        }*/

        private static Tuple<List<Request>, List<Request>> getCaptureOutputTuple(Session[] oSessions)
        {
            string requestOutput = "";
            string responseOutput = "";
            List<Request> requestList = new List<Request>();
            foreach (Session oSession in oSessions)
            {
                // Request
                string requestHeaders = oSession.RequestHeaders.ToString();
                string requestBody = oSession.GetRequestBodyAsString();
                Request request = new Request(requestHeaders, requestBody);
                requestList.Add(request);

                // Response
                string responseHeaders = oSession.ResponseHeaders.ToString();
                string responseBody = oSession.GetResponseBodyAsString();


                /*ArrayList request = new ArrayList();
                request.Add("<u><b>Request Header</b></u>");
                request.Add(requestHeaders);
                request.Add("<u><b>Request Body</b></u>");
                request.Add(requestBody);

                ArrayList response = new ArrayList();
                response.Add("<u><b>Response Header</b></u>");
                response.Add(responseHeaders);
                response.Add("<u><b>Response Body</b></u>");
                response.Add(responseBody);


                requestOutput += "======= Session Object As String =======\n\n";
                requestOutput += oSession.ToString();
                requestOutput += "\n\n----- Host name -----\n\n";
                requestOutput += oSession.hostname;
                requestOutput += "\n\n----- Request Body As String -----\n\n";
                string requestBodyAsString = oSession.GetRequestBodyAsString();
                if (requestBodyAsString!="")
                {
                    JSONParseResult requestBodyAsJsonObj = (JSONParseResult)JSON.JsonDecode(requestBodyAsString);
                    Hashtable requestBodayAsHashtable = (Hashtable)requestBodyAsJsonObj.JSONObject;
                    if (requestBodayAsHashtable.ContainsKey("password"))
                    {
                        string password = (string)requestBodayAsHashtable["password"];
                        bool isEncrypted = PasswordAdvisor.IsEncrypted(password);
                    }
                    requestOutput += requestBodyAsString;
                }
                
                requestOutput += "\n\n\n=====================================\n\n\n";

                // Response
                responseOutput += "======= Session Object As String =======\n\n";
                responseOutput += oSession.ToString();
                responseOutput += "\n\n----- Host name -----\n\n";
                responseOutput += oSession.hostname;
                responseOutput += "\n\n----- Response Body As String -----\n\n";
                responseOutput += oSession.GetResponseBodyAsString();
                responseOutput += "\n\n========================================\n\n\n";*/
            }

            return new Tuple<List<Request>, List<Request>>(requestList, requestList);
        }

        /// <summary>
        /// Called before the user can edit a request using the Fiddler Inspectors
        /// </summary>
        public void AutoTamperRequestBefore(Session oSession)
        {
            /*noop*/
            // If we're not showing the FeederNetInspector tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }
            // begin filter
            // hide all request by default
            oSession["ui-hide"] = "true";
            string sessionHostName = oSession.hostname;
            string filterHostName = container.GetHostName();
            // list of processes to filter on
            string[] processlist = { "chrome", "iexplore", "microsoftedge", "msedge", "firefox", "torch" };
            for (var j = 0; j < processlist.Length; j++)
            {
                if (oSession.LocalProcess.Contains(processlist[j]))
                {
                    // Show item in web session list of Fiddler if sessionHostName contains filterHostName
                    if (sessionHostName.Contains(filterHostName))
                    {
                        oSession["ui-hide"] = null;
                    }
                }
            }
            // end filter
        }

        private static void IsPersonalInformationExposed(Session oSession, string bodyAsString)
        {
            // check if contains personal information P.I, highlight item if yes
            if (bodyAsString != "")
            {
                try
                {
                    JSONParseResult requestBodyAsJsonObj = (JSONParseResult)JSON.JsonDecode(bodyAsString);
                    if (requestBodyAsJsonObj.JSONObject != null)
                    {
                        Hashtable requestBodayAsHashtable = (Hashtable)requestBodyAsJsonObj.JSONObject;
                        if (requestBodayAsHashtable.ContainsKey("password"))
                        {
                            string password = (string)requestBodayAsHashtable["password"];
                            bool isEncrypted = PasswordAdvisor.IsEncrypted(password);
                            // if not encrypted, password may be plain text so that we highlight the session
                            if (!isEncrypted)
                            {
                                oSession.ViewItem.BackColor = Color.Yellow;
                            }
                        }
                    }
                } catch (Exception)
                {
                    /* Failed type casting - do nothing */
                }
                
            }
        }

        /// <summary>
        /// Called after the user has had the chance to edit the request using the Fiddler Inspectors, but before the request is sent
        /// </summary>
        public void AutoTamperRequestAfter(Session oSession)
        {
            /*noop*/
         }

        /// <summary>
        /// Called before the user can edit a response using the Fiddler Inspectors, unless streaming.
        /// </summary>
        public void AutoTamperResponseBefore(Session oSession)
        {
            /*noop*/
        }

        /// <summary>
        /// Called after the user edited a response using the Fiddler Inspectors.  Not called when streaming.
        /// Process filter the session and high light the session if personal information (P.I) exposed
        /// Currently we check only weak and no excrpted password
        /// </summary>
        public void AutoTamperResponseAfter(Session oSession)
        {
            // If we're not showing the FeederNetInspector tab right now, bail out.
            if (FiddlerApplication.UI.tabsViews.SelectedTab.Text != "FeederNetInspector")
            {
                return;
            }
            // begin filter
            // hide all request by default
            oSession["ui-hide"] = "true";
            string sessionHostName = oSession.hostname;
            string filterHostName = container.GetHostName();
            // list of processes to filter on
            string[] processlist = { "chrome", "iexplore", "microsoftedge", "msedge", "firefox", "torch" };
            for (var j = 0; j < processlist.Length; j++)
            {
                if (oSession.LocalProcess.Contains(processlist[j]))
                {
                    // Show item in web session list of Fiddler if sessionHostName contains filterHostName
                    if (sessionHostName.Contains(filterHostName))
                    {
                        oSession["ui-hide"] = null;
                        IsPersonalInformationExposed(oSession, oSession.GetRequestBodyAsString());
                        IsPersonalInformationExposed(oSession, oSession.GetResponseBodyAsString());
                    }
                }
            }
            // end filter
        }

        /// <summary>
        /// Called Fiddler returns a self-generated HTTP error (for instance DNS lookup failed, etc)
        /// </summary>
        public void OnBeforeReturningError(Session oSession)
        {
            /*noop*/
        }

        public static void CaptureAll()
        {
            try
            {
                SelectAllSessions();
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }

        }

        public static void SelectAllSessions()
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                item.Selected = true;
            }
        }

        public static void CaptureWithHostName(string hostName)
        {
            try
            {
                SelectAllSessionWithHostName(hostName);
            }
            catch (Exception e)
            {
                FiddlerApplication.Log.LogString(e.ToString());
                MessageBox.Show(e.Message, "Errors");
            }

        }

        public static void SelectAllSessionWithHostName(string hostName)
        {
            ListView.ListViewItemCollection lvItems = FiddlerApplication.UI.lvSessions.Items;
            foreach (ListViewItem item in lvItems)
            {
                // 3 -> index of column host name
                if (item.SubItems[3].Text.Contains(hostName))
                {
                    item.Selected = true;
                    item.BackColor = Color.Yellow;
                }
                else
                {
                    item.BackColor = FiddlerApplication.UI.lvSessions.BackColor;
                    item.Selected = false;
                }
            }
        }
    }

}